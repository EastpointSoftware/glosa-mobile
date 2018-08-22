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
using System.Linq;

using Plugin.Connectivity.Abstractions;
using Plugin.Connectivity;

using GreenLight.Core.Contracts;

namespace GreenLight.Droid.Services
{
    public class NetworkService : INetworkService
    {
        #region Implementation
        event ConnectivityChangedEventHandler INetworkService.ConnectivityChangedEvent
        {
            add
            {
                lock (_objectLock)
                {
                    CrossConnectivity.Current.ConnectivityChanged += value;
                }
            }

            remove
            {
                lock (_objectLock)
                {
                    CrossConnectivity.Current.ConnectivityChanged -= value;
                }
            }
        }

        event ConnectivityTypeChangedEventHandler INetworkService.ConnectivityTypeChangedEvent
        {
            add
            {
                lock (_objectLock)
                {
                    CrossConnectivity.Current.ConnectivityTypeChanged += value;
                }
            }

            remove
            {
                lock (_objectLock)
                {
                    CrossConnectivity.Current.ConnectivityTypeChanged -= value;
                }
            }
        }

        bool INetworkService.IsMobileConnectionAvailable()
        {
            bool isAvailable = false;
            var connectionType = CrossConnectivity.Current.ConnectionTypes.FirstOrDefault<ConnectionType>();

            isAvailable = CrossConnectivity.Current.IsConnected && (connectionType == ConnectionType.Cellular);

            return isAvailable;
        }

        bool INetworkService.IsWiFiConnectionAvailable()
        {
            bool isAvailable = false;
            var connectionType = CrossConnectivity.Current.ConnectionTypes.FirstOrDefault<ConnectionType>();

            isAvailable = CrossConnectivity.Current.IsConnected && (connectionType == ConnectionType.WiFi);

            return isAvailable;
        }

        public bool IsConnectionAvailable()
        {
            bool isAvailable = false;
            var connectionType = CrossConnectivity.Current.ConnectionTypes.FirstOrDefault<ConnectionType>();

            isAvailable = CrossConnectivity.Current.IsConnected;

            return isAvailable;
        }

        public event EventHandler<NetworkServiceErrorEventArgs> NetworkServiceErrorWiFiEvent;

        public event EventHandler<NetworkServiceEventArgs> NetworkServiceWiFiEvent;
        #endregion

        #region Member Variables
        private object _objectLock = new Object();
        #endregion
    }
}
