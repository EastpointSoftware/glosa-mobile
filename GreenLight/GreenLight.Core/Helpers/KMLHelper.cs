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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

using GreenLight.Core.Models;
using static GreenLight.Core.Helpers.NodeFinder;

//https://www.gridreferencefinder.com/
namespace GreenLight.Core.Helpers
{
    public static class KMLHelper
    {
        public static IList<kmlDocumentPlacemark> LoadPlacemarksFromFile(string file)
        {
            var list = new List<kmlDocumentPlacemark>();
            var assembly = typeof(KMLHelper).GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream(file);

            if (stream != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(kml));
                kml kml = serializer.Deserialize(stream) as kml;
                kmlDocumentPlacemark[] placemark = kml.Document.Placemark;
                list.AddRange(placemark);
            }
            else
            {
                throw new FileNotFoundException("Could not find file", file);
            }

            return list;
        }

        
        public static string IntersectionIdOfPlacemark(kmlDocumentPlacemark placemark)
        {
            string id = null;
            var name = placemark.name;
            var array = name.Split('-');
            if (array != null && array.Count() > 0)
            {
                id = array[0];
            }

            return id;
        }

        public static double DistanceFromCurrentIntersection(kmlDocumentPlacemark kml, double currentLatitude, double currentLongitude)
        {
            var coordinatesString = kml.Point.coordinates;
            string[] parts = coordinatesString.Split(',');
            double longitude = double.Parse(parts[0]);
            double latitude = double.Parse(parts[1]);

            var distanceMeters = Distance.CalculateDistanceBetween2PointsKMs(currentLatitude, currentLongitude, latitude, longitude) * 1000.0;

            return distanceMeters;
        }

        public static GPSLocation XMLCoordinateStringToGPSLocation(string coordinateString)
        {
            string[] parts = coordinateString.Split(',');
            double longitude = double.Parse(parts[0]);
            double latitude = double.Parse(parts[1]);

            GPSLocation targetIntersectionLocation = new GPSLocation()
            {
                Latitude = latitude,
                Longitude = longitude,
            };
            return targetIntersectionLocation;
        }

        public static List<GPSLocation> ConvertToGPSLocationList(List<kmlDocumentPlacemark> placemarks)
        {
            var gpsHistory = new List<GPSLocation>();
            foreach (var placemark in placemarks)
            {
                // https://developers.google.com/kml/documentation/kmlreference?csw=1#coordinates
                // coordinate order = lng/lat
                var coordinatesString = placemark.Point.coordinates;
                string[] parts = coordinatesString.Split(',');
                double longitude = double.Parse(parts[0]);
                double latitude = double.Parse(parts[1]);

                GPSLocation location = new GPSLocation()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                };
                gpsHistory.Add(location);
            }
            return gpsHistory;
        }

        public static IList<kmlDocumentPlacemark> GLOSATestRoute()
        {
            var list = LoadPlacemarksFromFile("GreenLight.Core.Test.GLOSA-MAP-CHACHE.kml") as List<kmlDocumentPlacemark>;

            return list;
        }

        public static kmlDocumentPlacemark GLOSATestRoutePlacemark(string id)
        {
            var list = LoadPlacemarksFromFile("GreenLight.Core.Test.GLOSA-MAP-CHACHE.kml") as List<kmlDocumentPlacemark>;
            kmlDocumentPlacemark placemark = list.Find(obj => obj.name.Contains(id));
            return placemark;
        }

        public static IList<kmlDocumentPlacemark> EastpointTestRoute()
        {
            var list = LoadPlacemarksFromFile("GreenLight.Core.Test.Eastpoint-ReferencePoint.kml");

            return list;
        }

        public static IList<kmlDocumentPlacemark> ICentrumTestRoute()
        {
            var list = LoadPlacemarksFromFile("GreenLight.Core.Test.iCentrum-ReferencePoint.kml");

            return list;
        }

        public static List<GPSLocation> GLOSATestRouteIntersectionHistory(string intersectionId, int directionSettingValue)
        {
            var name = DirectionSettingName(directionSettingValue);

            var list = LoadPlacemarksFromFile($"GreenLight.Core.Test.{intersectionId}-{name}.kml").ToList();

            var gpsHistory = ConvertToGPSLocationList(list);

            return gpsHistory;
        }

        public static string DirectionSettingName(int directionSettingValue)
        {
            string name = "";
            switch (directionSettingValue)
            {
                case Constants.SETTINGS_ROUTE_DIRECTION_ANY:
                    name = "";
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_InBound:
                    name = "IB";
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_OutBound:
                    name = "OB";
                    break;
                default:
                    break;
            }

            return name;
        }
    }
}
