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
using System.Linq;

using MvvmCross.Platform;
using MvvmCross.Plugins.Location;
using MvvmCross.Plugins.Messenger;

using GreenLight.Core.Contracts;
using GreenLight.Core.Helpers;
using GreenLight.Core.Models;
using static GreenLight.Core.Helpers.NodeFinder;
using GreenLight.Core.Objects;

namespace GreenLight.Core.Services
{
    public class NavigationService : INavigationService, IDisposable
    {
        #region Properties

        public double Latitude { get { return _latitude; } }
        public double Longitude { get { return _longitude; } }
        public double CurrentSpeed { get { return _currentSpeed; } }
        public double? DeviceHeading { get { return _deviceHeading; } }
        public double? HeadingAccuracy { get { return _headingAccuracy; } }
        
        public bool IsNavigating { get { return HasGPSHistory(); } }
        public bool IsNavigatingToWaypoint { get { return _currentWaypoint != null; } }
        public List<GPSLocation> GPSHistory { get { return GetGPSHistory(); } }

        public kmlDocumentPlacemark Waypoint
        {
            get
            {
                UpdateCurrentPosition();
                return _currentWaypoint;
            }
        }
        public string WayPointId { get { return _currentWaypointId; } }
        public string WaypointDescription {
            get
            {
                UpdateCurrentPosition();
                return _waypointDescription;
            }
        }
        public int DistanceToWaypoint { get { return _currentDistanceToWaypoint; } }

        public string RouteId { get { return _routeId; } }
        public string RouteSession { get { return _routeSession; } }

        #endregion

        public NavigationService()
        {
            _mapDataCache = XMLHelper.LoadMAPData().ToList();
        }

        public NavigationService(IMvxMessenger messenger, IMvxLocationWatcher watcher, ILocationService locationService)
        {
            _messenger = messenger;
            _messageToken = _messenger.Subscribe<LocationMessage>(OnLocationMessage);
            _locationService = locationService;
            _mapDataCache = XMLHelper.LoadMAPData().ToList();
        }

        #region Implementation Public

        public void Start(List<kmlDocumentPlacemark> route, string intersectionId, ulong allowedVehicleManeuvers, int simulatedDirection, List<GPSLocation> simulatedGPSLocations)
        {
            _testRoute = route;
            _currentWaypointId = intersectionId;
            _currentWaypoint = _testRoute.Find(obj => obj.name.Contains(_currentWaypointId));
            _routeDirection = simulatedDirection;

            _routeSession = Guid.NewGuid().ToString();
            _useSimulatedGPS = true;
            _simulatedGPSLocations = simulatedGPSLocations;
        }

        public void Start(List<kmlDocumentPlacemark> route, string routeId, ulong allowedVehicleManeuvers, WaypointDetectionMethod junctionDetectionMethod)
        {
            _testRoute = route;
            _routeId = routeId;
            _waypointDetectionMethod = junctionDetectionMethod;
            _routeDirection = Constants.SETTINGS_ROUTE_DIRECTION_ANY;

            _routeSession = Guid.NewGuid().ToString();
            _routeMode = true;
            _useSimulatedGPS = false;
        }

        public void Stop()
        {
            _testRoute = null;
            _currentWaypoint = null;
            _waypointDescription = "";
            _currentWaypointId = "";
        }

        public kmlDocumentPlacemark LocateWaypointOnRoute(WaypointDetectionMethod method, int waypointIndex = 0)
        {
            return LocateWaypointOnRoute(method, _testRoute, GetGPSHistory(), 0, waypointIndex);
        }

        public kmlDocumentPlacemark LocateWaypointOnRoute(WaypointDetectionMethod method, List<kmlDocumentPlacemark> route, List<GPSLocation> history, double heading, int waypointIndex = 0)
        {
            kmlDocumentPlacemark nearestMAPPlacemark = null;

            var location = history.Last();

            var sortedMAPPoints = GLOSAHelper.SortMAPKMLDataByDistanceFromCurrentLocation(route, location.Latitude, location.Longitude).ToList();

            if (sortedMAPPoints != null && sortedMAPPoints.Count > 0)
            {
                if (method == WaypointDetectionMethod.ShortestDistance)
                {
                    nearestMAPPlacemark = sortedMAPPoints.First();
                }
                else if (method == WaypointDetectionMethod.DeviceHeading)
                {
                    // No valid method chosen
                    throw new Exception("Waypoint Detection Method not implemented.");

                }
                else if (method == WaypointDetectionMethod.GPSHistoryDirection)
                {
                    List<TrafficNode> listOfWaypoints = new List<TrafficNode>();

                    GPSLocation from = history[history.Count - 2];
                    GPSLocation to = history[history.Count - 1];

                    double? vehicleBearing = LocationHelper.HeadingBetweenTwoGPSLocations(from, to);

                    foreach (var mapPoint in sortedMAPPoints)
                    {
                        listOfWaypoints.Add(new TrafficNode()
                        {
                            GPSLocation = KMLHelper.XMLCoordinateStringToGPSLocation(mapPoint.Point.coordinates),
                            ID = mapPoint.name,
                            angleDiff = 0,
                        });
                    }

                    foreach (var waypoint in listOfWaypoints)
                    {
                        double? bearingLocationToWaypoint = LocationHelper.HeadingBetweenTwoGPSLocations(to, waypoint.GPSLocation);
                        double delta = LocationHelper.DeltaOfVehicleToLaneDirection(vehicleBearing, bearingLocationToWaypoint);
                        waypoint.angleDiff = Math.Abs(delta);//Make positive
                        waypoint.distance = Distance.CalculateDistanceBetween2PointsKMs(to.Latitude, to.Longitude, waypoint.GPSLocation.Latitude, waypoint.GPSLocation.Longitude);
                    }

                    var sortedByBearingAndDistance = listOfWaypoints.OrderBy(wp => wp.distance)
                        .ThenBy(wp => wp.angleDiff)
                        .Where(wp => wp.angleDiff >= -50 && wp.angleDiff <= 50 && wp.distance <= 1.0);

                    if (sortedByBearingAndDistance != null && sortedByBearingAndDistance.Count() > waypointIndex)
                    {
                        var tn = sortedByBearingAndDistance.ToList()[waypointIndex];
                        var pm = sortedMAPPoints.Find(mp => mp.name == tn.ID);
                        nearestMAPPlacemark = pm;
                    }
                }
                else
                {
                    // No valid method chosen
                    throw new Exception("Waypoint Detection Method not properly set.");
                }
            }
            return nearestMAPPlacemark;
        }

        public MapData LocateWaypointWithLineOfSight(WaypointDetectionMethod method, int waypointIndex = 0, int viewArc = 0, double distance = 1.0)
        {
            return LocateWaypointWithLineOfSight(method, GetGPSHistory(), waypointIndex, viewArc, distance);
        }

        public MapData LocateWaypointWithLineOfSight(WaypointDetectionMethod method, List<GPSLocation> history, int waypointIndex = 0, int viewArc = 0, double distance = 1.0)
        {
            MapData waypoint = null;

            var location = history.Last();

            var sortedMAPPoints = GLOSAHelper.SortMAPDataByDistanceFromCurrentLocation(_mapDataCache, location.Latitude, location.Longitude);

            if (sortedMAPPoints != null && sortedMAPPoints.Count > 0)
            {
                switch (method)
                {
                    case WaypointDetectionMethod.ShortestDistance:
                        waypoint = sortedMAPPoints.First();
                        break;
                    case WaypointDetectionMethod.GPSHistoryDirection:
                        GPSLocation to = history[history.Count - 1];
                        double? vehicleHeading = LocationHelper.HeadingFromGPSTrail(history);

                        List<IntersectionNode> listOfMAPDataWaypoints = IntersectionNode.IntersectionNodesFromMAPData(_mapDataCache).ToList();
                        var filtered = IntersectionNode.Filter(listOfMAPDataWaypoints, vehicleHeading, history);
                        var sortedByBearingAndDistance = IntersectionNode.SortIntersectionNodes(filtered, to, vehicleHeading, viewArc, distance).ToList();

                        if (sortedByBearingAndDistance.Count() > waypointIndex)
                        {
                            var tn = sortedByBearingAndDistance.ToList()[waypointIndex];
                            waypoint = tn.MapData;
                        }
                        
                        break;
                    default:
                        throw new Exception("Waypoint Detection Method not implemented.");
                        break;
                }
            }
            return waypoint;
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
                UpdateCurrentPosition();
            }

            return @continue;
        }

        #endregion

        #region Implementation Private

        private void StartTimer()
        {
            _timerService = Mvx.Resolve<ITimerService>();
            _timerFinished = false;
            if (_timerRunning == false)
            {
                _timerRunning = true;
                _timerService.StartTimer(TimeSpan.FromMilliseconds(Constants.AdvisorySpeedCalculatorProcessingIntervalMilliseconds), (() => TimerServiceCallback()));
            }
        }

        private bool TimerHasFinished()
        {
            return _timerFinished;
        }

        private void UpdateCurrentPosition()
        {
            _waypointDescription = "[Locating Intersection]";
            _currentDistanceToWaypoint = 0;

            if (IsNavigating == true)
            {
                LocateWaypoint();

                if (IsNavigatingToWaypoint == true)
                {
                    _currentDistanceToWaypoint = Convert.ToInt32(KMLHelper.DistanceFromCurrentIntersection(_currentWaypoint, _latitude, _longitude));
                    _waypointDescription = _currentWaypoint.name;
                }
            }
        }

        /// <summary>
        /// Finds next waypoint by distance from current location
        /// </summary>
        private void LocateWaypoint()
        {
            if (_routeMode == true)
            {
                _currentWaypoint = null;

                var waypoint = LocateWaypointWithLineOfSight(WaypointDetectionMethod.GPSHistoryDirection, 0, 50, 1.0);
                if (waypoint == null)
                    return;
                var waypointID = waypoint.intersections.IntersectionGeometry.id.id.ToString();
                var nextMAPPlacemark = _testRoute.Find(match => match.name.Contains(waypointID));
                if (nextMAPPlacemark != null)
                {
                    var nextIntersectionId = KMLHelper.IntersectionIdOfPlacemark(nextMAPPlacemark);
                    _currentWaypointId = waypoint.intersections.IntersectionGeometry.id.id.ToString();
                    _currentWaypoint = nextMAPPlacemark;
                }
            }
        }

        private List<GPSLocation> GetGPSHistory()
        {
            List<GPSLocation> history = GPSHistoryValues(Constants.GPS_HISTORY_MINIMUM_WINDOW_SIZE, _routeDirection, _currentWaypointId);
            return history;
        }

        private bool HasGPSHistory()
        {
            List<GPSLocation> history = GPSHistoryValues(Constants.GPS_HISTORY_MINIMUM_WINDOW_SIZE, _routeDirection, _currentWaypointId);
            return history.Count > Constants.GPS_HISTORY_MINIMUM_WINDOW_SIZE;
        }

        private List<GPSLocation> GPSHistoryValues(int count, int direction, string intersectionId)
        {
            List<GPSLocation> list = new List<GPSLocation>();

            if (_useSimulatedGPS == false)
            {
                foreach (var message in _gpsLocations)
                {
                    GPSLocation location = new GPSLocation()
                    {
                        Latitude = message.Latitude,
                        Longitude = message.Longitude,
                        TimeStamp = message.Timestamp,
                    };
                    list.Add(location);
                }
            }
            else
            {
                list = _simulatedGPSLocations;
            }

            return list;
        }

        private void AddMessageToGPSHistory(LocationMessage message)
        {
            if (_gpsLocations.Count == 20)
            {
                _gpsLocations.RemoveAt(0);
            }
            if (_gpsLocations.Count == 0)
            {
                _gpsLocations.Add(message);
            }
            else
            {
                // add only if distance in 5 meters from last point
                var lastMessage = _gpsLocations.Last();
                var distance = Distance.CalculateDistanceBetween2PointsKMs(message.Latitude, message.Longitude, lastMessage.Latitude, lastMessage.Longitude);
                if (distance >= 0.005)
                {
                    _gpsLocations.Add(message);
                }
            }
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

        #region Event Handlers

        private void OnLocationMessage(LocationMessage locationMessage)
        {
            _longitude = locationMessage.Longitude;
            _latitude = locationMessage.Latitude;
            _currentSpeed = SpeedConverter.ConvertFromMetersPerSecondToMilePerHour(Convert.ToDouble(locationMessage.Speed));
            _deviceHeading = locationMessage.Heading;
            _headingAccuracy = locationMessage.HeadingAccuracy;

            AddMessageToGPSHistory(locationMessage);
        }

        #endregion

        #region Member Variables

        private IMvxMessenger _messenger;
        private MvxSubscriptionToken _messageToken;
        private ILocationService _locationService;
        private ITimerService _timerService;

        private bool _timerFinished = true;
        private bool _timerRunning = false;

        private bool _useSimulatedGPS;
        private List<GPSLocation> _simulatedGPSLocations;

        private double _longitude;
        private double _latitude;
        private double _currentSpeed;
        private double? _deviceHeading;
        private double? _headingAccuracy;

        private List<LocationMessage> _gpsLocations = new List<LocationMessage>(20);

        private bool _routeMode;
        List<kmlDocumentPlacemark> _testRoute;
        List<MapData> _mapDataCache;
        private string _routeId;
        private int _routeDirection;
        private string _routeSession;

        private int _currentDistanceToWaypoint;
        private kmlDocumentPlacemark _currentWaypoint;
        private string _currentWaypointId;
        private string _waypointDescription;
        private WaypointDetectionMethod _waypointDetectionMethod;

        #endregion
    }
}
