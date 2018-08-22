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

using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Location;
using MvvmCross.Plugins.Messenger;

using GreenLight.Core.Contracts;
using GreenLight.Core.Helpers;

namespace GreenLight.Core.ViewModels
{
    /*
        The inital view for the application
    */
    /// <summary>
    /// This class holds functionality for evaluating the various components
    /// </summary>
    public class FirstViewModel : MvxViewModel
    {
        #region Construction
        /// <summary>
        /// Constructor
        /// </summary>
        public FirstViewModel(IMvxMessenger messenger, IMvxLocationWatcher watcher)
        {
            _alertService = Mvx.Resolve<IAlertService>();

            _messageToken = messenger.Subscribe<LocationMessage>(_onLocationMessage);
        }

        #endregion

        #region App Life-Cycle
        public override void Appearing()
        {
            base.Appearing();
            if (LocationPermissionGranted == false)
            {
                LocationPermissionStatus = "Location Permission Not Granted: Please Enable";
            }
            else
            {
                LocationPermissionStatus = "Location Permission Granted";
                if (_locationService == null)
                {
                    _locationService = Mvx.Resolve<ILocationService>();
                }
            }
        }

        public override void Disappearing()
        {
            base.Disappearing();
            _messageToken.Dispose();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Get the status of the mobile connection
        /// </summary>
        public string MobileStatus
        {
            get { return _mobileStatus; }
            private set { SetProperty(ref _mobileStatus, value); }
        }

        /// <summary>
        /// Get the status of the mobile connection
        /// </summary>
        public string WiFiStatus
        {
            get { return _wiFiStatus; }
            set { SetProperty(ref _wiFiStatus, value); }
        }

        public string LocationPermissionStatus
        {
            get { return _locationPermissionStatus; }
            set { SetProperty(ref _locationPermissionStatus, value); }
        }

        public bool LocationPermissionGranted
        {
            get{ return _permissionGranted; }
            set { SetProperty(ref _permissionGranted, value); }
        }

        public bool EnableButtons
        {
            get { return _enableButtons; }
            set { SetProperty(ref _enableButtons, value); }
        }
        #endregion

        #region Command
        /// <summary>
		/// Left lane click command.
		/// </summary>
		public IMvxCommand ShowSpeedAdvisorykCommand => _openSpeedAdvisoryViewClickCommand ??
           (_openSpeedAdvisoryViewClickCommand = new MvxCommand(ShowSpeedAdvisoryView));

        public IMvxCommand ShowSettingsCommand => _settingsCommand ??
           (_settingsCommand = new MvxCommand(ShowSettingsView));

        public IMvxCommand SendBroadcastCommand => _sendBroadcastCommand ??
        (_settingsCommand = new MvxCommand(SendBroadcast));

        #endregion

        #region Implementation
        private void ShowSpeedAdvisoryView()
        {
            if (string.IsNullOrEmpty(Settings.UniqueVehicleDeviceAppId))
            {
                _alertService.Alert("Please enter your Vehicle Id in Settings.", "Vehicle Id is empty", "Close");
            }
            else
            {
                ShowViewModel<SpeedAdvisoryViewModel>();
            }
        }

        private void ShowSettingsView()
        {
            ShowViewModel<SettingsViewModel>();
        }

        private void SendBroadcast()
        {
            ISocketService socketService = Mvx.Resolve<ISocketService>();
            IIPAddressManager addressManager = Mvx.Resolve<IIPAddressManager>();
            string address = addressManager.GetBroadcastAddress();
            socketService.SendBroadcast("Hello", address, 52112);
        }
        
        private void _onLocationMessage(LocationMessage locationMessage)
        {
            EnableButtons = LocationPermissionGranted;
            if (EnableButtons)
            {
                LocationPermissionStatus = "Location Permission Granted";
            }
        }

        #endregion

        #region Member Variables
        private ILocationService _locationService;
        private IAlertService _alertService;

        private MvxSubscriptionToken _messageToken;
        
        private string _mobileStatus = "";
        private string _wiFiStatus = "";
        private string _locationPermissionStatus = "";
        private bool _permissionGranted = false;
        private bool _enableButtons = false;

        private IMvxCommand _openSpeedAdvisoryViewClickCommand;
        private IMvxCommand _settingsCommand;
        private IMvxCommand _sendBroadcastCommand;
        
        #endregion
    }
}
