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

using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Helpers
{
    public static class LocationHelper
    {
        public static bool IsLocationTimestampRecent(DateTimeOffset timestamp)
        {
            // seconds
            bool isRecent = (DateTime.Now - timestamp).TotalSeconds < 5;
            return isRecent;
        }
        
        public static bool IsLocationNewer(DateTimeOffset newLocationTimestamp, DateTimeOffset locationTimestamp)
        {
            var timeDelta = (newLocationTimestamp - locationTimestamp).TotalSeconds;
            return timeDelta > 0;
        }

        public static bool FilterForAccuracy(double locationAccuracy, double requiredAccuracy)
        {
            // 20m
            return locationAccuracy <= requiredAccuracy;
        }

        public static int DeltaOfVehicleToLaneDirection(double? vehicleHeading, double? laneHeading)
        {
            int vehicleDegreesInt = Convert.ToInt32(vehicleHeading);
            
            int laneDegreesInt = Convert.ToInt32(laneHeading);

            var delta = laneDegreesInt - vehicleDegreesInt;

            return delta;
        }

        public static double? HeadingBetweenTwoGPSLocations(GPSLocation fromLocation, GPSLocation toLocation)
        {
            var previousVehicleLocation = fromLocation;
            var currentVehicleLocation = toLocation;

            var lat1 = previousVehicleLocation.Latitude;
            var lon1 = previousVehicleLocation.Longitude;

            var lat2 = currentVehicleLocation.Latitude;
            var lon2 = currentVehicleLocation.Longitude;

            var bearing = DegreeBearing(lat1, lon1, lat2, lon2);

            return bearing;
        }

        public static double DegreeBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = Distance.Deg2rad(lon2 - lon1);
            var dPhi = Math.Log(
                Math.Tan(Distance.Deg2rad(lat2) / 2 + Math.PI / 4) / Math.Tan(Distance.Deg2rad(lat1) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        public static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (Distance.Rad2deg(radians) + 360) % 360;
        }

    }
}
