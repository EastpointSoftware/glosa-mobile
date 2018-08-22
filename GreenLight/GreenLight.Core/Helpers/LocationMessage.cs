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

using MvvmCross.Plugins.Messenger;

namespace GreenLight.Core.Helpers
{
	/// <summary>
	/// Get the location message through IMvxMessenger.
	/// </summary>
	/// <seealso cref="MvvmCross.Plugins.Messenger.MvxMessage" />
	public class LocationMessage : MvxMessage
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="LocationMessage"/> class.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="longitude">The longitude.</param>
		/// <param name="latitude">The latitude.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="heading">The heading.</param>
		/// <param name="headingAccuracy">The heading accuracy.</param>
		public LocationMessage(object sender, double longitude, double latitude, double? speed, double? heading, double? headingAccuracy, DateTimeOffset timestamp) : base(sender)
		{
			Longitude = longitude;
			Latitude = latitude;
			Speed = speed;
			Heading = heading;
			HeadingAccuracy = headingAccuracy;
            Timestamp = timestamp;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the longitude.
		/// </summary>
		public double Longitude { get; private set; }

		/// <summary>
		/// Gets the latitude.
		/// </summary>
		public double Latitude { get; private set; }

		public double? Speed { get; private set; }
		public double? Heading { get; private set; }
		public double? HeadingAccuracy { get; private set; }
        public DateTimeOffset Timestamp { get; set; }
        #endregion
    }
}
