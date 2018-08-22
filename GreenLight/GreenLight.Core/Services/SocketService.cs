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

using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

using Sockets.Plugin;
using Sockets.Plugin.Abstractions;

using GreenLight.Core.Contracts;

namespace GreenLight.Core.Services
{
    public class SocketService : ISocketService
    {
        #region Properties

        public bool ServiceEnabled { get; }
        public bool ServiceAvailable { get; }
        public bool Listening { get; private set; }

        #endregion

        #region Constructor

        public SocketService()
        {
            ServiceEnabled = false;
            ServiceAvailable = true;
            Listening = false;

            _UdpSocketReceiver = new UdpSocketReceiver();
            _UdpSocketReceiver.MessageReceived += _UdpSocketReceiver_MessageReceived;
        }

        #endregion

        #region Implementation

        public async Task<bool> StartListendingForUdpBroadcastsOnPortAsync(int port)
        {
            await _UdpSocketReceiver.StartListeningAsync(port);
            Listening = true;
            return Listening;
        }

        public async Task StopListendingForUdpBroadcasts()
        {
            if (Listening == true)
            {
                await _UdpSocketReceiver.StopListeningAsync();
                Listening = false;
            }
        }

        public async Task<bool> SendBroadcast(string message, string address, int port = 52112)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var udpSockectClient = new UdpSocketClient();
            await udpSockectClient.SendToAsync(bytes, address, port);
            //await udpSockectClient.SendAsync(bytes);
            return true;
        }

        public string GetData()
        {
            return _data;
        }

        private void _UdpSocketReceiver_MessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
        {
            _data = Encoding.UTF8.GetString(e.ByteData, 0, e.ByteData.Length);

            Debug.WriteLine(_data);
        }
        #endregion

        #region Member Variables

        private UdpSocketReceiver _UdpSocketReceiver;
        private string _data;
        #endregion
    }
}
