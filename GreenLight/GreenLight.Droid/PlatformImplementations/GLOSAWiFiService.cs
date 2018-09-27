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

using Android.App;
using Android.Net.Wifi;

using GreenLight.Core.Contracts;

namespace GreenLight.Droid.PlatformDependencies
{
    public class GLOSAWiFiService : IGLOSAWiFiService
    {
        #region Constructor

        public GLOSAWiFiService()
        {
            ServiceEnabled = false;
            ServiceAvailable = true;
            Connected = false;

            _wifiManager = (WifiManager)Application.Context.GetSystemService(Android.Content.Context.WifiService);

            if (_wifiManager != null)
            {
                // Make sure system doesn't save power by disabling WiFi
                _wifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "myLockId");

                // this is a work around for Android devices that are by default prevented from receiving broadcast messages
                WifiManager.MulticastLock loc = _wifiManager.CreateMulticastLock("Log_Tag");
                loc.Acquire();
            }

            _tryingToConnect = false;
        }

        #endregion

        #region Properties

        public bool ServiceEnabled { get; }
        public bool ServiceAvailable { get; }
        public bool Connected { get; private set; }

        #endregion

        #region Implementation

        public bool ConnectToNetwork(string SSID, string sharedKey)
        {
            Connect(SSID, sharedKey);

            return Connected;
        }

        public bool ReconnectToNetwork(string SSID, string sharedKey)
        {
            Connect(SSID, sharedKey);

            return Connected;
        }

        public bool IsWithinRangeOfNetwork(string SSID)
        {
            bool result = false;

            if (_wifiManager != null)
            {
                var list = _wifiManager.ConfiguredNetworks;
                if (list != null)
                {
                    var results = list.Where(wifiConfiguration => wifiConfiguration.Ssid == String.Format("\"{0}\"", SSID));
                    if (results != null)
                    {
                        result = results.Count() > 0;
                    }
                }
            }

            return result;
        }

        public bool isConnectedToNetwork(string SSID)
        {
            if (_wifiManager == null)
            {
                return false;
            }
                
            var connected = (_wifiManager.ConnectionInfo != null 
                && _wifiManager.ConnectionInfo.SSID == String.Format("\"{0}\"", SSID) 
                && _wifiManager.ConnectionInfo.SupplicantState == SupplicantState.Completed);

            return connected;
        }

        public bool hasDisconnectedFromNetwork(string SSID)
        {
            if (_wifiManager == null)
            {
                return false;
            }

            var disconnected = (_wifiManager.ConnectionInfo != null 
                && _wifiManager.ConnectionInfo.SSID == String.Format("\"{0}\"", SSID) 
                && _wifiManager.ConnectionInfo.SupplicantState == SupplicantState.Disconnected);

            return disconnected;
        }

        public void disconnect()
        {
            if (_wifiManager != null)
            {
                // Return dynamic information about the current Wi-Fi connection, if any is active.
                WifiInfo wifiInfo = _wifiManager.ConnectionInfo;

                if (wifiInfo != null)
                {
                    _wifiManager.Disconnect();
                }
            }
        }

        public void TurnOnWiFi()
        {
            if (_wifiManager != null && _wifiManager.IsWifiEnabled == false)
            {
                _wifiManager.SetWifiEnabled(true);
            }
        }

        public void TurnOffWiFi()
        {
            if (_wifiManager != null && _wifiManager.IsWifiEnabled == true)
            {
                _wifiManager.SetWifiEnabled(false);
            }
        }
        #endregion

        #region Private

        private void Connect(string SSID, string sharedKey)
        {
            // return early
            if (_tryingToConnect == true)
            {
                return;
            }

            try
            {
                _tryingToConnect = true;

                if (_wifiManager != null)
                {
                   
                    // if already connected then no need to connect
                    if (_wifiManager.ConnectionInfo != null && _wifiManager.ConnectionInfo.SSID == SSID)
                    {
                        Connected = true;
                    }
                    else
                    {
                        WifiConfiguration wifiConfig = new WifiConfiguration();
                        wifiConfig.Ssid = String.Format("\"{0}\"", SSID);
                        wifiConfig.PreSharedKey = String.Format("\"{0}\"", sharedKey);

                        // Get or add network to known list.
                        int netId = GetOrAddNetworkId(SSID, wifiConfig);

                        // Make sure network hasn't changed
                        _wifiManager.UpdateNetwork(wifiConfig);

                        // if connected and not connected to network needed then disconnect and reconnect
                        if (_wifiManager.ConnectionInfo != null && _wifiManager.ConnectionInfo.NetworkId != netId)
                        {
                            _wifiManager.Disconnect();
                            _wifiManager.EnableNetwork(netId, true);
                            _wifiManager.Reconnect();
                        }

                        if (_wifiManager.ConnectionInfo != null && _wifiManager.ConnectionInfo.SSID == SSID && _wifiManager.ConnectionInfo.SupplicantState == SupplicantState.Completed)
                        {
                            Connected = true;
                        }
                        else
                        {
                            Connected = false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Connected = false;
            }
            finally
            {
                _tryingToConnect = false;
            }
        }

        private int GetOrAddNetworkId(string SSID, WifiConfiguration wifiConfig)
        {
            // Get or add network to known list.
            int netId;

            // Get the access to the network.
            var network = _wifiManager.ConfiguredNetworks.FirstOrDefault(cn => cn.Ssid == SSID);

            // Sign the eastpoint network id to netId.
            if (network != null) netId = network.NetworkId;
            else
            {
                // Add a new network description to the set of configured networks.
                netId = _wifiManager.AddNetwork(wifiConfig);
            }

            return netId;
        }

        #endregion
        #region Member Variables
        private WifiManager _wifiManager;
        private bool _tryingToConnect;
        #endregion
    }
}