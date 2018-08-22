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
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Location;
using MvvmCross.Plugins.Messenger;

using GreenLight.Core.Contracts;
using GreenLight.Core.Helpers;
using GreenLight.Core.Services;
using static GreenLight.Core.Helpers.AdvancedAdvisorySpeedCalculator;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.ViewModels
{
	/// <summary>
	/// Provide speed advisory.
	/// </summary>
	/// <seealso cref="MvvmCross.Core.ViewModels.MvxViewModel" />
	public class SpeedAdvisoryViewModel : MvxViewModel, IDisposable
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="SpeedAdvisoryViewModel"/> class.
		/// </summary>
		public SpeedAdvisoryViewModel(IMvxMessenger messenger, IMvxLocationWatcher watcher)
		{
            _vehicleService = Mvx.Resolve<IVehicleService>();
            _textToSpeechService = new TextToSpeechService();
            _cancellationTokenSource = new CancellationTokenSource();
            _GLOSAWiFiService = Mvx.Resolve<IGLOSAWiFiService>();
            // Task.Run(async () => await _textToSpeechService.SpeakAsync("Welcome to GLOSA", _cancellationTokenSource.Token));

            GLOSAMessage = $"Waiting for connection (WiFi Mode: {Settings.EnableWiFiMode})";
            AdvisorySpeedMessage = "";
            Intersection = "[Locating intersection]";
        }

        #endregion

        #region App Life-Cycle
        public override void Appearing()
        {
            base.Appearing();

            _vehicleService.VehicleEventHandler += VehicleServiceEventHandler;

            if (Settings.EnableIntersectionMode == false && Settings.EnableTestRoute == false)
            {
                Debug.WriteLine("Failed To Start Vehicle Service");
                GLOSAMessage = "Unsupported mode. Enable a test mode via settings";
            }
            else
            {
                // Here we are forcing to use the Advanced Calculator
                AdvisoryCalculatorMode advisoryCalculatorMode = AdvisoryCalculatorMode.Basic;
                Settings.EnableAdvancedCalculator = true;
                if (Settings.EnableAdvancedCalculator == true)
                {
                    advisoryCalculatorMode = AdvisoryCalculatorMode.Advanced;
                }

                if (Settings.EnableTestRoute == true)
                {
                    _vehicleService.Start(GLOSAHelper.LoadTestRoute().ToList(), "GLOSA A45", Settings.VehicleManeuverDirection, WaypointDetectionMethod.GPSHistoryDirection, advisoryCalculatorMode);
                }
                else if (Settings.EnableIntersectionMode == true)
                {
                    try
                    {
                        List<GPSLocation> simulatedGPSHistory = KMLHelper.GLOSATestRouteIntersectionHistory(Settings.IntersectionId, Settings.RouteDirectionOption);
                        _vehicleService.Start(GLOSAHelper.LoadTestRoute().ToList(), Settings.IntersectionId, Settings.VehicleManeuverDirection, Settings.RouteDirectionOption, simulatedGPSHistory, advisoryCalculatorMode);
                    }
                    catch (Exception e)
                    {
                        GLOSAMessage = $"Intersection Id not recognised ({Settings.IntersectionId}) or Direction not valid)";
                    }
                }
            }
        }

        public override void Disappearing()
        {
            base.Disappearing();

            _vehicleService.Stop();

            _vehicleService.VehicleEventHandler -= VehicleServiceEventHandler;

            _cancellationTokenSource.Cancel();
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets the right lane count down time.
		/// </summary>
		public string SignalCountDownTime
		{
			get => _signalCountDownTime;
			set => SetProperty(ref _signalCountDownTime, value);
		}

        public string CurrentSpeed
        {
            get => _currentSpeed;
            set => SetProperty(ref _currentSpeed, value);
        }

        /// <summary>
        /// Gets or sets the advisory speed.
        /// </summary>
        public string AdvisorySpeedMessage
		{
			get { return _advisorySpeedMessage; }
			set { SetProperty(ref _advisorySpeedMessage, value); }
		}

        public string GLOSAMessage
        {
            get { return _glosaMessage; }
            set { SetProperty(ref _glosaMessage, value); }
        }


        public string Intersection
        {
            get { return _intersection; }
            set { SetProperty(ref _intersection, value); }
        }

        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }
        
        public string NetworkStatus
        {
            get { return _networkStatus; }
            set { SetProperty(ref _networkStatus, value); }
        }

        public string NetworkType
        {
            get { return _networkType; }
            set { SetProperty(ref _networkType, value); }
        }
        
        #endregion

        #region Command

        public IMvxCommand CallCompleteIntersectionCommand => _completeIntersectionCommand ??
            (_completeIntersectionCommand = new MvxCommand(CompleteIntersectionCommand));

        private void CompleteIntersectionCommand()
        {
            
        }
        #endregion

        #region Implementation Public

        #endregion

        #region Implementation Private
        private void VehicleServiceEventHandler(object sender, VehicleServiceEventArgs e)
        {
            InvokeOnMainThread(() => {
                Location = $"{e.Latitude},{e.Longitude}";
                CurrentSpeed = $"Speed {Convert.ToInt32(e.CurrentSpeedMPH)} MPH";
                GLOSAMessage = Loading();
                AdvisorySpeedMessage = "";
                SignalCountDownTime = "";
                NetworkStatus = "";
                UpdateNetworkType();
                UpdateIntersectionDetails(e);

                if (e.Status == VehicleServiceStatus.Ok)
                {
                    if ((e.CalculationResult != null && e.CalculationResult.Errors != CalculationErrors.NoErrors))
                    {
                        if (Settings.GLOSAAdvisoryEnabled == true)
                        {
                            CalculationErrorEventHandler(e);
                        }
                    }
                    else if ((e.CalculationResult != null))
                    {
                        if (Settings.GLOSAAdvisoryEnabled == true)
                        {
                            UpdateAdvisorySpeed(e);
                        }
                    }

                    if (e.GLOSAResult != null && e.GLOSAResult.Errors != GLOSAErrors.NoErrors)
                    {
                        GLOSAErrorEventHandler(e);
                    }
                    else if (e.GLOSAResult != null)
                    {
                        UpdateSignalPhase(e.GLOSAResult.CurrentStateTimeMovement);
                        UpdateNetworkStatus(e);
                    }
                }
                else
                {
                    string message = "[Unknown Vehicle Status]";
                    switch (e.Status)
                    {
                        case VehicleServiceStatus.Ok:
                            break;
                        case VehicleServiceStatus.NetworkConnectionError:
                            message = "No network connection";
                            break;
                        case VehicleServiceStatus.GPSError:
                            message = "Waiting for GPS";
                            break;
                        case VehicleServiceStatus.SNTPError:
                            message = "SNTP sync failed. Using device time";
                            break;
                        default:
                            break;
                    }

                    GLOSAMessage = $"{message}";
                    Debug.WriteLine(GLOSAMessage);
                }

                if (string.IsNullOrEmpty(e.Debug) == false)
                {
                    GLOSAMessage = e.Debug;
                }
            });
        }

        private int _lodingCount = 0;
        private int _lodingCountMax = 3;
        private string Loading()
        {
            _lodingCount++;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < _lodingCount && i < _lodingCountMax; i++)
            {
                stringBuilder.Append(".");
            }
            
            if (_lodingCount == 3)
            {
                _lodingCount = 0;
            }
            return stringBuilder.ToString();
        }

        private void UpdateIntersectionDetails(VehicleServiceEventArgs e)
        {
            var distanceText = "";
            if (e.DistanceToIntersection <= Constants.CALCULATOR_MAXIMUM_DISTANCE_CHECK_METERS)
            {
                distanceText = $"{e.DistanceToIntersection} m";
            }
            else
            {
                distanceText = $" >500m";
            }

            Intersection = $"{e.IntersectionDescription} {distanceText}";
        }

        private void UpdateAdvisorySpeed(VehicleServiceEventArgs e)
        {
            var speed = Convert.ToInt32(e.CalculationResult.AdvisorySpeed) < 0 ? 0 : Convert.ToInt32(e.CalculationResult.AdvisorySpeed);
            speed = Convert.ToInt32(SpeedConverter.ConvertFromMetersPerSecondToMilePerHour(speed));

            var message = "";

            if (e.CurrentSpeedMPH > speed)
            {
                message = $"Reduce to {speed} MPH";
                //message = $"Reduce speed";
            }
            else if (e.CurrentSpeedMPH <= speed)
            {
                message = $"Maintain speed at {speed} MPH";
                //message = $"Maintain speed";
            }

            if (AdvisorySpeedMessage != message)
            {
                Task.Run(() => TextToSpeech(AdvisorySpeedMessage));
            }
            
            AdvisorySpeedMessage = message;
        }

        private void UpdateSignalPhaseTextToSpeech(StateTimeMovementEvent stateTimeMovementEvent)
        {
            if (GLOSAHelper.IsCROCSTimeValid(stateTimeMovementEvent.MinEndTime))
            {
                string speechMessage = "";
                switch (stateTimeMovementEvent.MovementEvent)
                {
                    case MovementEvent.Green:
                        speechMessage = $"Green for another {stateTimeMovementEvent.MovementTimespan.TotalSeconds} seconds";
                        break;
                    case MovementEvent.Red:
                        case MovementEvent.RedAmber:
                        // Add 2 second to account for amber
                        speechMessage = $"Time to Green {stateTimeMovementEvent.MovementTimespan.TotalSeconds + 2} seconds";
                        break;
                    default:
                        break;
                }

                Task.Run(() => TextToSpeech(speechMessage));
            }
        }

        private void UpdateSignalPhase(StateTimeMovementEvent stateTimeMovementEvent)
        {
            if (_currentSignaState == null || (_currentSignaState != null && _currentSignaState.MovementEvent != stateTimeMovementEvent.MovementEvent))
            {
                Debug.WriteLine("Begin State Time Movement Event");
                _currentSignaState = stateTimeMovementEvent;

                UpdateSignalPhaseTextToSpeech(stateTimeMovementEvent);
            }

            UpdateStateTimeMovementEventMessage(stateTimeMovementEvent);
            UpdateStateTimeMovementEventCountdownTime(stateTimeMovementEvent);
        }

        private void UpdateStateTimeMovementEventMessage(StateTimeMovementEvent stateTimeMovementEvent)
        {
            string displayMessage = "";
            switch (stateTimeMovementEvent.MovementEvent)
            {
                case MovementEvent.Green:
                    displayMessage = $"Green for another";
                    break;
                case MovementEvent.Amber:
                    displayMessage = "A";
                    break;
                case MovementEvent.Red:
                case MovementEvent.RedAmber:
                    displayMessage = $"Time to Green";
                    break;
                //case MovementEvent.RedAmber:
                //    displayMessage = "RA";
                //    break;
                default:
                    break;
            }

            GLOSAMessage = displayMessage;
        }

        private void UpdateStateTimeMovementEventCountdownTime(StateTimeMovementEvent stateTimeMovementEvent)
        {
            string displayTime = $"{Convert.ToInt16(stateTimeMovementEvent.MovementTimespan.TotalSeconds)}";
            switch (stateTimeMovementEvent.MovementEvent)
            {
                case MovementEvent.Green:
                    break;
                case MovementEvent.Amber:
                    displayTime = "";
                    break;
                case MovementEvent.Red:
                    break;
                case MovementEvent.RedAmber:
                    break;
                default:
                    displayTime = "";
                    break;
            }

            SignalCountDownTime = displayTime;
        }

        private void UpdateNetworkStatus(VehicleServiceEventArgs e)
        {
            var spatMode = e.IsWiFiSPATData ? "WiFi" : "Cellular";
            NetworkStatus = $"(SPAT:{spatMode})";
        }

        private void UpdateNetworkType()
        {
            var mode = _GLOSAWiFiService.isConnectedToNetwork(Settings.WiFiNetworkName) ? "W" : "";
            NetworkType = mode;
        }

        private async Task TextToSpeech(string message)
        {
            if (Settings.EnableTextToSpeech == true && String.IsNullOrEmpty(message) == false)
            {
                await _textToSpeechService.SpeakAsync(message, _cancellationTokenSource.Token);
            }
        }

        private void CalculationErrorEventHandler(VehicleServiceEventArgs e)
        {
            var errorMessage = "";

            CalculationErrors? errors = e?.CalculationResult.Errors;
            switch (errors)
            {
                case (CalculationErrors.InvalidDistance | CalculationErrors.NotEnoughTimeToReact):
                    errorMessage = "Distance too close to calculate advisory speed";
                    break;
                case (CalculationErrors.TooFarToCalculateAccurately):
                    errorMessage = "Distance too far to calculate advisory speed";
                    break;
                case (CalculationErrors.AdvisorySpeedAboveSpeedLimit):
                    errorMessage = $"Advisory Speed Above Speed Limit ({Settings.SpeedLimit}+ MPH)";
                    //errorMessage = "";
                    break;
                case (CalculationErrors.AdvisorySpeedBelowHalfSpeedLimit):
                    //var speed = Convert.ToInt32(e.CalculationResult.AdvisorySpeed) < 0 ? 0 : Convert.ToInt32(e.CalculationResult.AdvisorySpeed);
                    //errorMessage = $"Advisory Speed Too Low ({Convert.ToInt32(SpeedConverter.ConvertFromMetersPerSecondToMilePerHour(speed))} MPH)";
                    errorMessage = "";
                    break;
                default:
                    errorMessage = $"Calculation Error {errors}";
                    break;
            }

            AdvisorySpeedMessage = errorMessage;
        }

        private void GLOSAErrorEventHandler(VehicleServiceEventArgs e)
        {
            var errorMessage = "";

            GLOSAErrors? cErrors = e?.GLOSAResult.Errors;
            switch (cErrors)
            {
                case GLOSAErrors.UnsupportedMode:
                    errorMessage = "Unsupported mode. Enable a test mode via settings";
                    break;
                case GLOSAErrors.UnableToFindProjectedLaneForMovement:
                    errorMessage = "Locating lane";
                    break;
                case GLOSAErrors.ProjectedLaneNotInSameDirection:
                    errorMessage = "Locating lane";
                    break;
                case GLOSAErrors.UnableToFindProjectedStateForLane:
                    errorMessage = "Unable to determine signal group";
                    break;
                case GLOSAErrors.UnableToProjectMovementStates:
                    errorMessage = "SPAT message stale";
                    break;
                case GLOSAErrors.SPATMessagedExpired:
                    errorMessage = "Waiting for SPAT message";
                    break;
                case GLOSAErrors.WebServiceError:
                    errorMessage = "Unable to connect to Webservice, possible 404 or 500";
                    break;
                case GLOSAErrors.WebServiceXMLParsingError:
                    errorMessage = "Unable to parse repsonse from Webservice, XML parsing error";
                    break;
                case GLOSAErrors.UnKnownGPSData:
                    errorMessage = "Waiting for GPS";
                    break;
                default:
                    errorMessage = "GLOSA Error Unknown";
                    break;
            }

            GLOSAMessage = $"{errorMessage} ({e.IntersectionId})";
            Debug.WriteLine(GLOSAMessage);
            SignalCountDownTime = "";
        }

        public void Dispose()
        {
            _vehicleService.Dispose();
        }
        #endregion

        #region Member Variables
        private IVehicleService _vehicleService;
        private ITextToSpeechService _textToSpeechService;
        private IGLOSAWiFiService _GLOSAWiFiService;
        
        private string _signalCountDownTime;

        private string _currentSpeed;
        private string _advisorySpeedMessage;
        private string _glosaMessage;
        private string _intersection;
        private string _location;
        private string _networkStatus;
        private string _networkType;
        private StateTimeMovementEvent _currentSignaState;
        private IMvxCommand _completeIntersectionCommand;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

    }
}
