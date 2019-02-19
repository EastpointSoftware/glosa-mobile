/*
GLOSA Mobile. Green Light Optimal Speed Adviosry Mobile Application

Copyright © 2017 Eastpoint Software Limited

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

using MvvmCross.Plugins.Messenger;
using MvvmCross.Plugins.Location;

using GreenLight.Core.Contracts;
using GreenLight.Core.Models;
using GreenLight.Core.Helpers;
using static GreenLight.Core.Helpers.NodeFinder;

public enum DataConnection
{
    Cellular    = 0,
    WiFi_Beacon = 1,
    Both        = 2,
    None        = 3
}

namespace GreenLight.Core.Services
{
    public class VehicleService : IVehicleService, IDisposable
    {
        #region Construction

        /// <summary>
        /// Constructor for the vehicle service. This service relies on have GPS, Internet and SNTP to sync time correctly. If the SNTP is not
        /// available then the device time is used.
        /// </summary>
        /// <param name="locationService"></param>
        /// <param name="dataAnalyticsService"></param>
        /// <param name="sNTPService"></param>
        /// <param name="networkService"></param>
        /// <param name="timerService"></param>
        /// <param name="GLOSAWebService"></param>
        /// <param name="GLOSAWiFiService"></param>
        public VehicleService(IMvxMessenger messenger, IMvxLocationWatcher watcher, ILocationService locationService, IDataAnalyticsService dataAnalyticsService, ISNTPService sNTPService, INetworkService networkService, ITimerService timerService, IGLOSAWebService GLOSAWebService, IGLOSAWiFiService GLOSAWiFiService)
        {
            _GLOSAAnalyticsService = dataAnalyticsService;
            _sntpService = sNTPService;
            _networkService = networkService;
            _timerService = timerService;
            _GLOSAWebService = GLOSAWebService;
            _navigationService = new NavigationService(messenger, watcher, locationService);
            _GLOSAWiFiService = GLOSAWiFiService;
            _locationService = locationService;
        }

        #endregion

        #region Properties
        public event EventHandler<VehicleServiceEventArgs> VehicleEventHandler;
        #endregion

        #region Implementation Public

        public void Start(List<kmlDocumentPlacemark> route, string intersectionId, ulong allowedVehicleManeuvers, int simulatedDirection, List<GPSLocation> simulatedGPSLocations, AdvisoryCalculatorMode advisoryCalculatorMode)
        {
            if (string.IsNullOrEmpty(intersectionId) == true)
                throw new Exception("Intersection not specified");

            if (route == null || route.Count == 0)
                throw new Exception("Route not specified");

            Debug.WriteLine("Starting Vehicle Service");

            _advisoryCalculatorMode = advisoryCalculatorMode;
            _loggingEnabled = true;
            _allowedVehicleManeuvers = allowedVehicleManeuvers;

            _navigationService.Start(route, intersectionId, allowedVehicleManeuvers, simulatedDirection, simulatedGPSLocations);

            _timerFinished = false;
            if (_timerRunning == false)
            {
                _timerRunning = true;
                _timerService.StartTimer(TimeSpan.FromMilliseconds(Constants.AdvisorySpeedCalculatorProcessingIntervalMilliseconds), (() => TimerServiceCallback()));
            }
        }

        public void Start(List<kmlDocumentPlacemark> route, string routeId, ulong allowedVehicleManeuvers, WaypointDetectionMethod junctionDetectionMethod, AdvisoryCalculatorMode advisoryCalculatorMode)
        {
            if (route == null || route.Count == 0)
                throw new Exception("Route not specified");

            Debug.WriteLine("Starting Vehicle Service");

            _advisoryCalculatorMode = advisoryCalculatorMode;
            _loggingEnabled = true;
            _allowedVehicleManeuvers = allowedVehicleManeuvers;

            _navigationService.Start(route, routeId, allowedVehicleManeuvers, junctionDetectionMethod);

            _timerFinished = false;
            if (_timerRunning == false)
            {
                _timerRunning = true;
                _timerService.StartTimer(TimeSpan.FromMilliseconds(Constants.AdvisorySpeedCalculatorProcessingIntervalMilliseconds), (() => TimerServiceCallback()));
            }
        }

        public void Stop()
        {
            _timerFinished = true;
            _navigationService.Stop();

            if (Settings.EnableWiFiMode == true && _socketService != null)
            {
                _socketService.StopListendingForUdpBroadcasts();
            }

            Debug.WriteLine("Stopping Vehicle Service");
        }

        public void Dispose()
        {
            if (_timerService != null)
            {
                _timerService.StopTimer();
                _timerService.Dispose();
            }
        }

        #endregion

        #region Event Loop
        private bool TimerServiceCallback()
        {
            bool @continue = true;

            if (TimerHasFinished())
            {
                @continue = false;
            }
            else
            {
                var before = DateTime.Now;

                if (CheckLocationServices() == false)
                {
                    PostVehicleMessage(VehicleServiceStatus.GPSError);
                    Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : GPS not available");
                    LogDataEvent("GPS not available or enabled");
                    return @continue;
                }

                if (CheckNetworkStatus() == false)
                {
                    PostVehicleMessage(VehicleServiceStatus.NetworkConnectionError);
                    Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : Waiting for connection (WiFi Mode: {Settings.EnableWiFiMode})");
                    LogDataEvent("No Network Connection");
                    return @continue;
                }

                Task.Run(() => SyncTime());

                if (_navigationService.IsNavigating == true && _navigationService.IsNavigatingToWaypoint == true && _navigationService.Waypoint != null)
                {
                    // GET MAP SPAT data of intersection
                    var nextWaypoint = _navigationService.LocateWaypointWithLineOfSight(WaypointDetectionMethod.GPSHistoryDirection, 1, 50);
                    var nexyWaypointId = nextWaypoint != null ? nextWaypoint.intersections.IntersectionGeometry.id.id.ToString() : null;
                    Task.Run(async () => await _GLOSAWebService.SyncMAPSPATAsync(_navigationService.WayPointId, nexyWaypointId));

                    if (HasMapSPATDataFromCellular() == true || HasMapSPATDataFromWiFi() == true)
                    {
                        MapData map = null;
                        SPAT spat = null;
                        var dataConnection = DataConnection.Cellular;

                        if (Settings.EnableWiFiMode == true && HasMapSPATDataFromWiFi() == true)
                        {
                            _isUsingWiFiSPATData = true;
                            map = GetWiFIMAP();
                            spat = GetWiFISPAT();
                            dataConnection = DataConnection.WiFi_Beacon;
                        }

                        // revert to celluar data
                        if (spat == null && HasMapSPATDataFromCellular() == true)
                        {
                            _isUsingWiFiSPATData = false;
                            spat = _GLOSAWebService.SPATData(_navigationService.WayPointId);
                            dataConnection = DataConnection.Cellular;
                        }

                        // revert to celluar data
                        if (map == null && HasMapSPATDataFromCellular() == true)
                        {
                            map = _GLOSAWebService.MAPData(_navigationService.WayPointId);
                        }

                        var history = _navigationService.GPSHistory;

                        DateTime date = CurrentTime();
                        int currentTimeCROCS = GLOSAHelper.ConvertTimeToCROCSTime(date);

                        GLOSAResult glosaResult = GLOSAHelper.TimeToTraficLight(map, spat, history, _navigationService.DeviceHeading, _allowedVehicleManeuvers, currentTimeCROCS);

                        if (glosaResult.Errors == GLOSAErrors.NoErrors)
                        {
                            CalculationResult calculation = AdvisorySpeedCalculationResult.CalculateAdvisorySpeed(_navigationService.DistanceToWaypoint, glosaResult.TimeToTrafficLight, _navigationService.CurrentSpeed, _advisoryCalculatorMode);
                            PostVehicleMessage(calculation, glosaResult);

                            var after = DateTime.Now;
                            double latency = (after - before).TotalMilliseconds;
                            LogDataEvent(calculation, glosaResult, latency, currentTimeCROCS, dataConnection);
                        }
                        else
                        {
                            PostVehicleMessage(null, glosaResult);
                            LogDataEvent("GLOSA Result", null, null, $"{map.intersections.IntersectionGeometry.id.id}", null, 0, Convert.ToInt16(_navigationService.DeviceHeading), glosaResult.Description);
                            Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : GLOSA Error - {glosaResult.Errors}");
                        }
                    }
                    else
                    {
                        PostVehicleMessage(null, null);
                        LogDataEvent($"Waiting for data");
                        Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : Waiting for data {_navigationService.Waypoint.name}");
                    }
                }
                else
                {
                    PostVehicleMessage(null, null);
                    Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : Locating intersection");
                    LogDataEvent("Locating intersection");
                }
                
            }

            return @continue;
        }

        private bool TimerHasFinished()
        {
            return _timerFinished;
        }
        #endregion

        #region Implementation Private

        private async void SyncTime()
        {
            if (_networkService.IsConnectionAvailable())
            {
                if (Settings.SNTPSyncEnabled == true && _sntpService.IsResponseValid() == false)
                {
                    await _sntpService.SyncTimeToServer("143.210.16.201");
                    if (_sntpService.IsResponseValid() == true)
                    {
                        Debug.WriteLine("Time Synced with SNTP");
                    }
                }
            }
        }

        private DateTime CurrentTime()
        {
            DateTime date = DateTime.UtcNow;
            if (Settings.SNTPSyncEnabled == true && _sntpService.IsResponseValid() == true)
            {
                // check tolerances
                if (_sntpService.LocalClockOffset < 1500 && _sntpService.LocalClockOffset > -1500)
                {
                    date = DateTime.Now.AddMilliseconds(_sntpService.LocalClockOffset);
                }
            }
            return date;
        }

        private void PostVehicleMessage(CalculationResult calculationResult, GLOSAResult glosaResult)
        {
            if (VehicleEventHandler != null)
            {
                VehicleEventHandler.Invoke(this, new VehicleServiceEventArgs()
                {
                    Status = VehicleServiceStatus.Ok,
                    CalculationResult = calculationResult,
                    GLOSAResult = glosaResult,
                    Longitude = _navigationService.Longitude,
                    Latitude = _navigationService.Latitude,
                    CurrentSpeedMPH = _navigationService.CurrentSpeed,
                    IntersectionDescription = _navigationService.WaypointDescription,
                    IntersectionId = _navigationService.WayPointId,
                    DistanceToIntersection = _navigationService.DistanceToWaypoint,
                    IsWiFiSPATData = _isUsingWiFiSPATData,
                });
            }
        }

        private void PostVehicleMessage(VehicleServiceStatus status)
        {
            if (VehicleEventHandler != null)
            {
                VehicleEventHandler.Invoke(this, new VehicleServiceEventArgs()
                {
                    Status = status,
                    Longitude = _navigationService.Longitude,
                    Latitude = _navigationService.Latitude,
                    CurrentSpeedMPH = _navigationService.CurrentSpeed,
                    IntersectionDescription = _navigationService.WaypointDescription,
                    IntersectionId = _navigationService.WayPointId,
                    DistanceToIntersection = _navigationService.DistanceToWaypoint,
                    IsWiFiSPATData = _isUsingWiFiSPATData,
                });
            }
        }

        private bool CheckLocationServices()
        {
            if (_locationService.IsAvaliable == false || _locationService.IsEnabled == false)
            {
                return false;
            }

            return true;
        }

        private bool CheckNetworkStatus()
        {
            var hasNetwork = false;

            if (Settings.EnableWiFiMode == true)
            {
                _GLOSAWiFiService.TurnOnWiFi();

                if (_GLOSAWiFiService.IsWithinRangeOfNetwork(Settings.WiFiNetworkName))
                {
                    hasNetwork = _GLOSAWiFiService.isConnectedToNetwork(Settings.WiFiNetworkName);
                    if (hasNetwork == false)
                    {
                        hasNetwork = _GLOSAWiFiService.ConnectToNetwork(Settings.WiFiNetworkName, Settings.WiFiNetworkPassword);
                    }

                    // check if previously connected
                    //if (hasNetwork == false)
                    //{
                    //    var disconnected = _GLOSAWiFiService.hasDisconnectedFromNetwork(Settings.WiFiNetworkName);
                    //    if (disconnected)
                    //    {
                    //        hasNetwork = _GLOSAWiFiService.ConnectToNetwork(Settings.WiFiNetworkName, Settings.WiFiNetworkPassword);
                    //    }
                    //}

                    if (hasNetwork == true)
                    {
                        if (_socketService == null)
                        {
                            _socketService = new SocketService();
                        }

                        if (_socketService.Listening == false)
                        {
                            _socketService.StartListendingForUdpBroadcastsOnPortAsync(52112);
                        }
                    }
                }
            }

            // if only WiFi then do not check cellular
            if (Settings.EnableWiFiMode == false || (Settings.EnableWiFiMode == true && Settings.EnableWiFiModeOnly == false))
            {
                hasNetwork = _networkService.IsConnectionAvailable();
            }

            return hasNetwork;
        }

        private bool HasMapSPATDataFromCellular()
        {
            var hasData = false;

            bool hasMAPDataFromCelluar = _GLOSAWebService.HasMAPDataSyncedForIntersection(_navigationService.WayPointId);
            bool hasSPATDataFromCelluar = _GLOSAWebService.HasMAPSPATDataSyncedForIntersection(_navigationService.WayPointId);


            hasData = hasMAPDataFromCelluar == true && hasSPATDataFromCelluar == true;

            return hasData;
        }

        private bool HasMapSPATDataFromWiFi()
        {
            var hasData = false;
            
            bool hasSPATDataFromWiFi = false;
            bool hasMAPDataFromWiFi = false;

            if (Settings.EnableWiFiMode == true)
            {
                var hasNetwork = _GLOSAWiFiService.isConnectedToNetwork(Settings.WiFiNetworkName);
                hasSPATDataFromWiFi = hasNetwork == true && GetWiFISPAT() != null;
                hasMAPDataFromWiFi = GetWiFIMAP() != null;
            }

            hasData = hasSPATDataFromWiFi == true && hasMAPDataFromWiFi;

            return hasData;
        }

        private SPAT GetWiFISPAT()
        {
            SPAT data = null;
            if (_socketService != null && _socketService.Listening == true)
            {
                try
                {
                    if (Settings.EnableIntersectionMode == true)
                    {
                        //data = XMLHelper.LoadSPATDataForIntersection(Settings.IntersectionId);

                        string s = _socketService.GetData();

                        XmlSerializer serializer = new XmlSerializer(typeof(SPAT));

                        using (TextReader reader = new StringReader(s))
                        {
                            data = serializer.Deserialize(reader) as SPAT;
                        }
                    }
                    else
                    {
                        string s = _socketService.GetData();

                        XmlSerializer serializer = new XmlSerializer(typeof(SPAT));

                        using (TextReader reader = new StringReader(s))
                        {
                            data = serializer.Deserialize(reader) as SPAT;
                        }
                    }
                }
                catch
                {

                }
                finally {

                }
            }

            return data;
        }

        private MapData GetWiFIMAP()
        {
            MapData data = null;

            try
            {
                if (Settings.EnableIntersectionMode == true)
                {
                    string file = $"MAP-{Settings.IntersectionId}.xml";
                    data = XMLHelper.LoadMAPDataFromFile(file);
                }
            }
            catch
            {

            }
            finally
            {

            }

            return data;
        }

        #endregion

        #region Logging

        private void LogDataEvent(CalculationResult calculationResult, GLOSAResult glosaResult, double latency, double currentTimeCROCS, DataConnection dataConnection)
        {
            // Log event to server
            var lane = (MapDataIntersectionsIntersectionGeometryGenericLane)glosaResult.Object;
            var advisoryValue = $"{calculationResult.AdvisorySpeed.ToString()} {calculationResult.Errors.ToString()}";
            var spatValue = glosaResult.CurrentStateTimeMovement.Discription;

            var detailedMessage = $"Device CROCS Time - { currentTimeCROCS} : Lane Id - { lane.laneID} : {spatValue}";
            Debug.WriteLine($"Vehicle Service Timer {DateTime.Now} : Advisory Calculation - {advisoryValue} : {detailedMessage}");

            if (_loggingEnabled == false)
            {
                return;
            }

            LogDataEvent(null, null, advisoryValue, null, detailedMessage, latency, Convert.ToInt16(_navigationService.DeviceHeading), glosaResult.Description, dataConnection);
        }

        private void LogDataEvent(string eventName = null, string value = null, string advisory = null, string map = null, string spat = null, double latency = -1, int heading = -1, string lane = null, DataConnection dataConnection = DataConnection.None)
        {
            if (_loggingEnabled == false)
            {
                return;
            }

            var eventLog = new GLOSAEventLog()
            {
                Latitude = _navigationService.Latitude,
                Longitude = _navigationService.Longitude,
                VehicleId = Settings.UniqueVehicleDeviceAppId,
                DeviceTime = DateTime.UtcNow,
                IntersectionId = _navigationService.WayPointId,
                Distance = _navigationService.DistanceToWaypoint,
                RouteId = _navigationService.RouteId,
                RouteSession = _navigationService.RouteSession,
                TimeOffset = _sntpService.LocalClockOffset,
                Event = eventName,
                Value = value,
                Speed = Math.Round(_navigationService.CurrentSpeed, 2),
                CalculationAdvisory = advisory,
                SPAT = spat,
                Latency = latency,
                AdvisoryEnabled = Settings.GLOSAAdvisoryEnabled,
                MAP = map,
                Lane = lane,
                Heading = heading,
                DataConnection = (int)dataConnection,
            };

            Logger.LogEvent(_GLOSAAnalyticsService, eventLog);
        }

        #endregion

        #region Member Variables

        private IGLOSAWebService _GLOSAWebService;
        private readonly IDataAnalyticsService _GLOSAAnalyticsService;
        private ITimerService _timerService;
        private ISNTPService _sntpService;
        private INetworkService _networkService;
        private INavigationService _navigationService;
        private IGLOSAWiFiService _GLOSAWiFiService;
        private ILocationService _locationService;

        private ISocketService _socketService;
        private bool _isUsingWiFiSPATData = false;

        private bool _timerFinished = true;
        private bool _timerRunning = false;

        private bool _loggingEnabled = false;
        private ulong _allowedVehicleManeuvers;
        private AdvisoryCalculatorMode _advisoryCalculatorMode;

        #endregion
    }
}