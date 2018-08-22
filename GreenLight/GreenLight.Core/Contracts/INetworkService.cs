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

using Plugin.Connectivity.Abstractions;

namespace GreenLight.Core.Contracts
{
    /*
        The main NetworkService interface
    */
    /// <summary>
    /// Implement this interface for networking functionality
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Subscribe to this event for changes in connectivity
        /// </summary>
        event ConnectivityChangedEventHandler ConnectivityChangedEvent;

        /// <summary>
        /// Subscribe to this event for changes in connectivity type
        /// </summary>
        event ConnectivityTypeChangedEventHandler ConnectivityTypeChangedEvent;
        
        /// <summary>
        /// Check for cellular connection
        /// </summary>
        /// <returns>True if there is a cellular connection</returns>
        bool IsMobileConnectionAvailable();

        /// <summary>
        /// Check for WiFi connection
        /// </summary>
        /// <returns>True if there is a WiFi connection</returns>
        bool IsWiFiConnectionAvailable();

        /// <summary>
        /// Check for either WiFi or celluar connection
        /// </summary>
        /// <returns>True if there is a connection</returns>
        bool IsConnectionAvailable();

        /// <summary>
        /// Subscribe to this event for changes on the WiFi network
        /// </summary>
        event EventHandler<NetworkServiceEventArgs> NetworkServiceWiFiEvent;

        /// <summary>
        /// Subscribe to this event for changes on the WiFi network
        /// </summary>
        event EventHandler<NetworkServiceErrorEventArgs> NetworkServiceErrorWiFiEvent;
    }

    public class NetworkServiceEventArgs : EventArgs {}
    public class NetworkServiceErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
