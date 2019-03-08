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
using System.Collections.Generic;

using GreenLight.Core.Models;
using GreenLight.Core.Helpers;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Contracts
{
    public enum VehicleServiceStatus
    {
        Ok,
        NetworkConnectionError,
        GPSNotAvailable,
        GPSPermissionError,
        SNTPError,
    }

    public interface IVehicleService
    {
        /// <summary>
        /// 
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the vehicle service with an intersectionid.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="intersectionId"></param>
        /// <param name="allowedVehicleManeuvers"></param>
        /// <param name="simulatedDirection"></param>
        void Start(List<kmlDocumentPlacemark> route, string intersectionId, ulong allowedVehicleManeuvers, int simulatedDirection, List<GPSLocation> simulatedGPSLocations, AdvisoryCalculatorMode advisoryCalculatorMode);

        /// <summary>
        /// Starts the vehicle service with a route and route id. 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="routeId"></param>
        /// <param name="allowedVehicleManeuvers"></param>
        /// <param name="junctionDetectionMethod"></param>
        void Start(List<kmlDocumentPlacemark> route, string routeIdd, ulong allowedVehicleManeuvers, WaypointDetectionMethod junctionDetectionMetho, AdvisoryCalculatorMode advisoryCalculatorMode);

        /// <summary>
        /// Stops the vehicle service.
        /// </summary>
        /// <returns></returns>
        void Stop();

        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();
        
        event EventHandler<VehicleServiceEventArgs> VehicleEventHandler;
    }

    public class VehicleServiceEventArgs : EventArgs
    {
        public VehicleServiceStatus Status { get; set; }

        /// <summary>
		/// Gets the advisory speed.
		/// </summary>
		/// <value>
		public CalculationResult CalculationResult { get; set; }

        public GLOSAResult GLOSAResult { get; set; }

        /// <summary>
        /// Gets or sets the speed in miles per hour.
        /// </summary>
        public double? CurrentSpeedMPH { get; set; }
        /// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        public double Latitude { get; set; }

        /// <summary>
        /// Vehicle Heading
        /// </summary>
        public double Heading { get; set; }

        public string Debug { get; set; }

        /// <summary>
        /// Gets the intersection name
        /// </summary>
        public string IntersectionDescription { get; set; }

        /// <summary>
        /// Intersection Id
        /// </summary>
        public string IntersectionId { get; set; }

        /// <summary>
        /// Distance to inntersection
        /// </summary>
        public int DistanceToIntersection { get; set; }

        public bool IsWiFiSPATData { get; set; }
    }
}
