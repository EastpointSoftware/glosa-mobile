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

using GreenLight.Core.Models;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Contracts
{
    public enum WaypointDetectionMethod
    {
        ShortestDistance,
        DeviceHeading,
        GPSHistoryDirection
    };

    public interface INavigationService
    {
        double Latitude { get; }
        double Longitude { get; }
        double CurrentSpeed { get; }
        double? DeviceHeading { get; }
        double? HeadingAccuracy { get; }
        int DistanceToWaypoint { get; }
        bool IsNavigating { get; }
        bool IsNavigatingToWaypoint { get; }
        List<GPSLocation> GPSHistory { get; }
        string WayPointId { get; }
        kmlDocumentPlacemark Waypoint { get; }
        string WaypointDescription { get; }
        string RouteId { get; }
        string RouteSession { get; }

        /// <summary>
        /// Starts the vehicle service with an intersectionid.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="intersectionId"></param>
        /// <param name="allowedVehicleManeuvers"></param>
        /// <param name="simulatedDirection"></param>
        void Start(List<kmlDocumentPlacemark> route, string intersectionId, ulong allowedVehicleManeuvers, int simulatedDirection, List<GPSLocation> simulatedGPSLocations);

        /// <summary>
        /// Starts the vehicle service with a route and route id. 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="routeId"></param>
        /// <param name="allowedVehicleManeuvers"></param>
        /// <param name="junctionDetectionMethod"></param>
        void Start(List<kmlDocumentPlacemark> route, string routeId, ulong allowedVehicleManeuvers, WaypointDetectionMethod junctionDetectionMethod);

        /// <summary>
        /// Stops the vehicle service.
        /// </summary>
        /// <returns></returns>
        void Stop();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        kmlDocumentPlacemark LocateWaypointOnRoute(WaypointDetectionMethod method, int waypointIndex = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="route"></param>
        /// <param name="history"></param>
        /// <param name="heading"></param>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        kmlDocumentPlacemark LocateWaypointOnRoute(WaypointDetectionMethod method, List<kmlDocumentPlacemark> route, List<GPSLocation> history, double heading, int waypointIndex = 0);
    }
}
