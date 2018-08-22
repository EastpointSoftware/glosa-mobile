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

using GreenLight.Core.Contracts;

namespace GreenLight.iOS.PlatformImplementations
{
    public class GLOSAWiFiService : IGLOSAWiFiService
    {
        #region Constructor

        public GLOSAWiFiService()
        {
            ServiceEnabled = false;
            ServiceAvailable = true;
            _connected = false;
        }

        #endregion

        #region Properties

        public bool ServiceEnabled { get; }
        public bool ServiceAvailable { get; }
        public bool Connected { get { return _connected; } }

        #endregion

        #region Implementation

        public bool ConnectToNetwork(string SSID, string sharedKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Member Variables

        private bool _connected;

        #endregion
    }
}
