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
using System.Diagnostics;

using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Plugins.Location;
using MvvmCross.Plugins.Messenger;

using GreenLight.Core.Contracts;
using GreenLight.Core.Helpers;

using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Services
{
    /// <summary>
    /// Get current location.
    /// </summary>
    public class LocationService : ILocationService
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="messenger">The messenger.</param>
        public LocationService(IMvxLocationWatcher watcher, IMvxMessenger messenger)
        {
            _messenger = messenger;

            try
            {
                _watcher = watcher;
                _watcher.OnPermissionChanged += _permissionChanged;
                _watcher.Start(new MvxLocationOptions()
                {
                    Accuracy = MvxLocationAccuracy.Fine,
                    TrackingMode = MvxLocationTrackingMode.Foreground,
                }, _onLocation, _onError);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                throw e;
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Public

        #endregion

        #region Implementation

        private void _onLocation(MvxGeoLocation location)
        {
            if (LocationHelper.IsLocationTimestampRecent(location.Timestamp.LocalDateTime) == false)
            {
                return;
            }

            if (LocationHelper.FilterForAccuracy(location.Coordinates.Accuracy.Value, 10) == false)
            {
                return;
            }

            _currentLocation = location;

            var message = new LocationMessage(this,
                                                location.Coordinates.Longitude,
                                                location.Coordinates.Latitude,
                                                location.Coordinates.Speed,
                                                location.Coordinates.Heading,
                                                location.Coordinates.HeadingAccuracy,
                                                location.Timestamp.LocalDateTime);

            _messenger.Publish(message);
        }

        private void _onError(MvxLocationError error)
        {
            Mvx.Error("Seen location error {0}", error.Code);
        }

        private void _permissionChanged(object sender, MvxValueEventArgs<MvxLocationPermission> e)
        {
            _permissionGranted = e.Value == MvxLocationPermission.Granted;
        }
       
        #endregion

        #region Member Variables

        private readonly IMvxLocationWatcher _watcher;
        private readonly IMvxMessenger _messenger;
        private MvxGeoLocation _currentLocation;
        private bool _permissionGranted;
        #endregion
    }
}
