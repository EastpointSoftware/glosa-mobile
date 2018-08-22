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
using System.Threading.Tasks;

using Xunit;

using GreenLight.Core;
using GreenLight.Core.Helpers;
using GreenLight.Core.Models;
using GreenLight.Core.Contracts;
using GreenLight.Core.Services;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Tests.Helpers
{
    public class LocationHelperTest
    {
        public DateTimeOffset dateTimeNow = DateTimeOffset.Now;

        [Fact]
        public void IsLocationTimestampRecent_WithDataTimeNow_ShouldReturnTrue()
        {
            //arrange
            bool expectedResult = true;
            //act
            bool actualResult = LocationHelper.IsLocationTimestampRecent(dateTimeNow);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void IsLocationTimestampRecent_WithDataTime1MinuteBefore_ShouldReturnFalse()
        {
            //arrange
            DateTimeOffset dateTime1MinuteBefore = DateTimeOffset.Now.AddMinutes(-1);
            bool expectedResult = false;
            //act
            bool actualResult = LocationHelper.IsLocationTimestampRecent(dateTime1MinuteBefore);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void IsLocationNewer_WithNewerTimestamp_ShouldReturnTrue()
        {
            //arrange
            DateTimeOffset inputDateTime = DateTimeOffset.Now.AddMinutes(1);
            bool expectedResult = true;
            //act
            bool actualResult = LocationHelper.IsLocationNewer(inputDateTime, dateTimeNow);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void IsLocationNewer_WithOlderTimestamp_ShouldReturnFalse()
        {
            //arrange
            DateTimeOffset inputDateTime = DateTimeOffset.Now.AddMinutes(-1);
            bool expectedResult = false;
            //act
            bool actualResult = LocationHelper.IsLocationNewer(inputDateTime, dateTimeNow);
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void IsVehicleHeadingInSameDirectionAsLaneDirection_WithLaneInSameDirection_ShouldReturnTrue()
        {
            // arrange
            //SETTINGS_ROUTE_DIRECTION_NS = 1: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_SN = 2: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_EW = 3;
            //SETTINGS_ROUTE_DIRECTION_WE = 4;
            var intersectionId = "2111";
            var gpsHistory = KMLHelper.GLOSATestRouteIntersectionHistory(intersectionId, 2);

            // Load Map from file
            MapData map = XMLHelper.LoadMAPDataForIntersection(intersectionId);
            GLOSAResult result = GLOSAHelper.ProjectedLaneForManeuver(map, gpsHistory, 0, Constants.MANEUVER_DIRECTION_AHEAD);
            var lane = (MapDataIntersectionsIntersectionGeometryGenericLane)result.Object;
            var nodes = GLOSAHelper.ExtractTrafficNodesFromLane(lane);

            // Let's sort all lane nodes from MAP Ref Point
            var refPoint = map.intersections.IntersectionGeometry.refPoint;
            var mapLocation = new GPSLocation()
            {
                Latitude = refPoint.lat / Constants.MAPCoordinateIntConverterUnit,
                Longitude = refPoint.@long / Constants.MAPCoordinateIntConverterUnit,
            };

            // Sort the nodes by distance ascending
            var sortedNodes = nodes.OrderBy(node => Distance.CalculateDistanceBetween2PointsKMs(node.GPSLocation.Latitude, node.GPSLocation.Longitude, mapLocation.Latitude, mapLocation.Longitude)).ToList();

            bool expectedResult = true;
            //act
            result = GLOSAHelper.IsDirectionOfVehicleInSameDirectionAsLane(map.intersections.IntersectionGeometry.id.id, sortedNodes, 0, gpsHistory, 50, Constants.MANEUVER_DIRECTION_AHEAD);
            bool actualResult = result.Errors == GLOSAErrors.NoErrors;
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async void IsNextWAypointInSameDirection_ShouldReturnTrue()
        {
            // arrange

            //SETTINGS_ROUTE_DIRECTION_NS = 1: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_SN = 2: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_EW = 3;
            //SETTINGS_ROUTE_DIRECTION_WE = 4;

            var intersectionId = "2105";
            var route = KMLHelper.GLOSATestRoute().ToList();
            var gpsHistory = KMLHelper.GLOSATestRouteIntersectionHistory(intersectionId, 4);

            NavigationService navigationService = new NavigationService();
            var waypoint = navigationService.LocateWaypointOnRoute(WaypointDetectionMethod.DeviceHeading, route, gpsHistory, 0, 0);
           
            bool expectedResult = true;
            //act
            bool actualResult = waypoint != null;
            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async void CanRunMultipleGLOSARequests_ShouldReturnTrue()
        {
            // arrange

            //SETTINGS_ROUTE_DIRECTION_NS = 1: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_SN = 2: - Not Supported Yet
            //SETTINGS_ROUTE_DIRECTION_EW = 3;
            //SETTINGS_ROUTE_DIRECTION_WE = 4;

            var intersectionId = "1992";
            var route = KMLHelper.GLOSATestRoute().ToList();
            var gpsHistory = KMLHelper.GLOSATestRouteIntersectionHistory(intersectionId, 3);

            NavigationService navigationService = new NavigationService();

            var waypoint1 = navigationService.LocateWaypointOnRoute(WaypointDetectionMethod.GPSHistoryDirection, route, gpsHistory, 0, 0);
            var waypoint2 = navigationService.LocateWaypointOnRoute(WaypointDetectionMethod.GPSHistoryDirection, route, gpsHistory, 0, 1);
            var webService = new GLOSAWebService(null);

            //await Task.Run(async () => await webService.SyncMAPSPATAsync(KMLHelper.IntersectionIdOfPlacemark(waypoint1)));
            //await Task.Run(async () => await webService.SyncMAPSPATAsync(KMLHelper.IntersectionIdOfPlacemark(waypoint2)));
            var waypoint1Id = KMLHelper.IntersectionIdOfPlacemark(waypoint1);
            var waypoint2Id = waypoint2 != null ? KMLHelper.IntersectionIdOfPlacemark(waypoint2) : null;
            await Task.Run(async () => await webService.SyncMAPSPATAsync(waypoint1Id, waypoint2Id));

            bool expectedResult = true;
            //act
            bool actualResult = waypoint1 != null;
            //assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}