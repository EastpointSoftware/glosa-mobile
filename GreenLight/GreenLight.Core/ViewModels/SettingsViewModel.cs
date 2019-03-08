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

using Xamarin.Essentials;

using MvvmCross.Core.ViewModels;

using GreenLight.Core.Helpers;

namespace GreenLight.Core.ViewModels
{
    public class SettingsViewModel : MvxViewModel
    {
        #region Construction
        public SettingsViewModel()
        {
            _appVersionBuild = $"Build: {AppInfo.VersionString} ({AppInfo.BuildString})";
        }
        #endregion

        #region App Life-Cycle
        #endregion

        #region Properties
        public int Confidence
        {
            get => _confidence;
            set
            {
                SetProperty(ref _confidence, value);
                Settings.SPATTimeIntervalConfidence = value;
            }
        }

        public bool EnableTextToSpeech
        {
            get => _enableTextToSpeech;
            set
            {
                SetProperty(ref _enableTextToSpeech, value);
                Settings.EnableTextToSpeech = value;
            }
        }

        public string VehicleId {
            get => _vehicleId;
            set
            {
                SetProperty(ref _vehicleId, value);
                Settings.UniqueVehicleDeviceAppId = value;
            }
        }

        public bool EnableAdvancedCalculator
        {
            get => _enableAdvancedCalculator;
            set
            {
                SetProperty(ref _enableAdvancedCalculator, value);
                Settings.EnableAdvancedCalculator = value;
            }
        }

        public bool EnableTestRoute
        {
            get => _enableTestRoute;
            set
            {
                SetProperty(ref _enableTestRoute, value);
                Settings.EnableTestRoute = value;
            }
        }

        public int SpeedLimit
        {
            get => _speedLimit;
            set
            {
                SetProperty(ref _speedLimit, value);
                Settings.SpeedLimit = value;
            }
        }

        public bool EnableIntersectionMode
        {
            get => _enableIntersectionMode;
            set
            {
                SetProperty(ref _enableIntersectionMode, value);
                Settings.EnableIntersectionMode = value;
            }
        }

        public string IntersectionId
        {
            get => _intersectionId;
            set
            {
                SetProperty(ref _intersectionId, value);
                Settings.IntersectionId = value;
            }
        }

        public int MaximumInvokeDistance
        {
            get => _maximumInvokedistance;
            set
            {
                SetProperty(ref _maximumInvokedistance, value);
                Settings.MaximumInvokeDistance = value;
                RaisePropertyChanged("MaximumInvokeDistanceDisplay");
            }
        }

        public int MaximumInvokeDistanceDisplay
        {
            get => Constants.CALCULATOR_MINIMUM_DISTANCE_CHECK_METERS + MaximumInvokeDistance;
        }

        public bool EnableGPSTime
        {
            get => _enableGPSTime;
            set
            {
                SetProperty(ref _enableGPSTime, value);
                Settings.SNTPSyncEnabled = value;
            }
        }

        public bool EnableGLOSAAdvisory
        {
            get => _enableGLOSAAdvisory;
            set
            {
                SetProperty(ref _enableGLOSAAdvisory, value);
                Settings.GLOSAAdvisoryEnabled = value;
            }
        }

        public bool EnableWiFiMode
        {
            get => _enableWiFiMode;
            set
            {
                SetProperty(ref _enableWiFiMode, value);
                Settings.EnableWiFiMode = value;
            }
        }

        public bool EnableWiFiOnlyMode
        {
            get => _enableWiFiOnlyMode;
            set
            {
                SetProperty(ref _enableWiFiOnlyMode, value);
                Settings.EnableWiFiModeOnly = value;
            }
        }

        public string WiFiNetworkName
        {
            get => _wifiNetworkName;
            set
            {
                SetProperty(ref _wifiNetworkName, value);
                Settings.WiFiNetworkName = value;
            }
        }

        public string WiFiNetworkPassword
        {
            get => _wifiNetworkPassword;
            set
            {
                SetProperty(ref _wifiNetworkPassword, value);
                Settings.WiFiNetworkPassword = value;
            }
        }

        public string AppVersionBuild
        {
            //get => $"Build: {AppInfo.VersionString} ({AppInfo.BuildString})";
            get => _appVersionBuild;
        }

        #endregion

        #region Command
        /// <summary>
		/// Maneuver selected
		/// </summary>
		public IMvxCommand ChangeLaneManeverkCommand => _changeLaneManeuverCommand ??
           (_changeLaneManeuverCommand = new MvxCommand(ChangeLaneManever));
        #endregion

        #region Implementation
        private void ChangeLaneManever()
        {
           
        }

        private void ChangeTimings()
        {

        }

        #endregion

        #region Member Variables

        private bool _enableTextToSpeech = Settings.EnableTextToSpeech;
        private int _confidence = Settings.SPATTimeIntervalConfidence;
        private string _vehicleId = Settings.UniqueVehicleDeviceAppId;
        private bool _enableAdvancedCalculator = Settings.EnableAdvancedCalculator;
        private bool _enableTestRoute = Settings.EnableTestRoute;
        private bool _enableIntersectionMode = Settings.EnableIntersectionMode;
        private string _intersectionId = Settings.IntersectionId;
        private int _maximumInvokedistance = Settings.MaximumInvokeDistance;
        private int _speedLimit = Settings.SpeedLimit;
        private bool _enableGPSTime = Settings.SNTPSyncEnabled;
        private bool _enableGLOSAAdvisory = Settings.GLOSAAdvisoryEnabled;

        private bool _enableWiFiMode = Settings.EnableWiFiMode;
        private bool _enableWiFiOnlyMode = Settings.EnableWiFiModeOnly;
        private string _wifiNetworkName = Settings.WiFiNetworkName;
        private string _wifiNetworkPassword = Settings.WiFiNetworkPassword;
        private string _appVersionBuild;

        private IMvxCommand _changeLaneManeuverCommand;
        #endregion
    }
}
