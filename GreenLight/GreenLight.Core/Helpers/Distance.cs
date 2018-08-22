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

namespace GreenLight.Core.Helpers
{
    public static class Distance
    {
        #region Implementation

        public static double CalculateDistanceBetween2PointsMiles(double lat1, double lon1, double lat2, double lon2)
        {
            double miles;
            if (lat1 == lat2 && lon1 == lon2) { return 0.0; }

            var theta = lon1 - lon2;
            var distance = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) +
                           Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) *
                           Math.Cos(Deg2rad(theta));

            distance = Math.Acos(distance);
            if (double.IsNaN(distance)) { return 0.0; }

            distance = Rad2deg(distance);
            miles = distance * 60.0 * 1.1515;

            return miles;
        }

        public static double CalculateDistanceBetween2PointsKMs(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 == lat2 && lon1 == lon2)
                return 0.0;

            var theta = lon1 - lon2;
            var distance = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) +
                           Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) *
                           Math.Cos(Deg2rad(theta));

            distance = Math.Acos(distance);
            if (double.IsNaN(distance))
                return 0.0;

            distance = Rad2deg(distance);
            distance = distance * 60.0 * 1.1515 * 1.609344; // miles to km need to times 1.609344

            return (distance);
        }

        public static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        #endregion
    }
}
