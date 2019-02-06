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

using GreenLight.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenLight.Core.Helpers
{
    /// <summary>
    /// Finds the closest traffic node to the direction of movement. Instanciate it and get the index of the closest traffic light node.
    /// </summary>
    public class NodeFinder
    {
        #region internal class (public)
        public class GPSLocation
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        public class Direction
        {
            private const double RadConst90 = Math.PI / 2.0;
            private const double RadConst180 = Math.PI;

            public double DeltaLatitude { get; set; }
            public double DeltaLongitude { get; set; }

            /// <summary>
            /// Getter for Angle.
            /// </summary>
            /// <value>The angle from North clockwise (going East) in Radians.</value>
            public double? Angle
            {
                get { return GetAngle(); }

            }

            /// <summary>
            /// Returns angle in radians. Doubles the difference in Latitutde so uniform the axes (remember GPS Latitude is from +/- 90, Longitude from +/- 180).
            /// </summary>
            /// <returns></returns>
            private double? GetAngle()
            {
                if (double.IsNaN(DeltaLatitude) || double.IsNaN(DeltaLongitude))
                    throw new NullReferenceException("DeltaLatitude or DeltaLongitude is not set appropriately.");

                var theta = Math.Atan2(Math.Abs(DeltaLatitude *2), Math.Abs(DeltaLongitude)); // in radians

                if (DeltaLatitude < 0 && DeltaLongitude >= 0) theta += RadConst90;
                if (DeltaLatitude >= 0 && DeltaLongitude >= 0) theta = 90 - theta;
                if (DeltaLatitude >= 0 && DeltaLongitude < 0) theta += RadConst180 + RadConst90;
                if (DeltaLatitude < 0 && DeltaLongitude < 0) theta += RadConst180;

                return theta;
            }

        }

        public class TrafficNode
        {
            public string ID { get; set; }
            public GPSLocation GPSLocation { get; set; }
            public double? distance { get; set; }
            public double? angleDiff { get; set; }
        }

        #endregion

        #region Construction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpsLocations">Location History with the last element being the most recent</param>
        /// <param name="trafficNodePositions">List of GPS location data for traffic nodes, converted fraom MAP data</param>
        /// <param name="locationWindowSize">number of GPS locations to be used to determine direction and appropriate node. Must be smaller than gpsLocations.Count. Default is 3.</param>
        /// <param name="searchRadius">Search radius for the traffic nodes. If no traffic nodes were found within the search radius, ClosestTrafficNode will return null. </param>
        /// <param name="searchAngle">Deviation of direction, expressed in degrees, that the algorithm can tolerate. If no traffic nodes are found within this deviation tolerance ClosestTrafficNode will return null. </param>
        public NodeFinder(List<GPSLocation> vehicleGpsHistory, List<TrafficNode> trafficNodePositions, int locationWindowSize = DefaultLocationWindowSize, double searchRadius = DefaultSearchRadius, double searchAngle = DefaultSearchAngle)
        {
            Initialise(vehicleGpsHistory, trafficNodePositions, locationWindowSize, searchRadius, searchAngle);
            ComputeClosestTrafficNode(locationWindowSize, vehicleGpsHistory);
        }

        /// <summary>
        /// Constructor which takes tuples and sets the processing window size
        /// </summary>
        /// <param name="vehicleGpsHistory">Tuples of vehicle's past locations with format <latitude, longitude> where last entry is the most recent</param>
        /// <param name="trafficNodePositions">Tuples of traffic Node's GPS locations with the format <latitude, longitude></param>
        /// <param name="locationWindowSize">number of GPS locations to be used to determine direction and appropriate node. Must be smaller than gpsLocations.Count. Default is 3.</param>
        /// <param name="searchRadius">Search radius for the traffic nodes. If no traffic nodes were found within the search radius, ClosestTrafficNode will return null. </param>
        /// <param name="searchAngle">Deviation of direction, expressed in degrees, that the algorithm can tolerate. If no traffic nodes are found within this deviation tolerance ClosestTrafficNode will return null. </param>
        public NodeFinder(List<Tuple<double, double>> vehicleGpsHistory, List<Tuple<double, double, string>> trafficNodePositions, int locationWindowSize = DefaultLocationWindowSize, double searchRadius = DefaultSearchRadius, double searchAngle = DefaultSearchAngle)
        {
            var vehicleLocations = ConvertTuplesToGPSLocations(vehicleGpsHistory);
            var trafficNodes = ConvertTuplesToTrafficNodess(trafficNodePositions);

            Initialise(vehicleLocations, trafficNodes, locationWindowSize, searchRadius, searchAngle);
            ComputeClosestTrafficNode(locationWindowSize, vehicleLocations);
        }

        private void Initialise(List<GPSLocation> vehicleGgpsHistory, List<TrafficNode> trafficNodes, int nWindow, double searchRadius, double searchAngle)
        {
            if (nWindow < 2)
                throw new ArgumentOutOfRangeException("locationWindowSize", nWindow, string.Format("Processing window size cannot be samaller than 2. Default is 3. Currently it is set to {0}.", nWindow));

            if (vehicleGgpsHistory.Count < nWindow +1 )
                throw new ArgumentOutOfRangeException("vehicleGpsHistory", vehicleGgpsHistory.Count, string.Format("Number of elements in the list of GPS Locations smaller than processing window size +1. Processing window size is {0}.", nWindow));
            
            // TODO: add valid checks for searchRadius and searchAngle
            if (searchAngle < 0)
                searchAngle = Math.Abs(searchAngle);

            if (searchRadius < 0)
                throw new ArgumentOutOfRangeException("searchRadius", searchRadius, "Search Radius (in metres) cannot be less than 0");

            NLocationWindow = (nWindow > vehicleGgpsHistory.Count) ? vehicleGgpsHistory.Count : nWindow;

            TrafficNodes = trafficNodes;
            VehicleLocationHistory = vehicleGgpsHistory;
            SearchAngle = searchAngle; // assign to public variable
            _searchAngleRad = searchAngle * Deg2RadConst; // note:all logic in radians, user interacts with degrees.
            SearchRadius = searchRadius;
        }
        #endregion

        #region App Life-Cycle
        #endregion

        #region Properties


        /// <summary>
        /// Number of GPS locations used to determine the direction and speed of vehicle
        /// </summary>
        public int NLocationWindow { get; private set; }

        public List<TrafficNode> TrafficNodes { get; private set; }

        /// <summary>
        /// Location History with the last element being the most recent
        /// </summary>
        public List<GPSLocation> VehicleLocationHistory { get; private set; }

        public Direction CurrentDirection { get; private set; }

        public double SearchRadius { get; private set; }

        public double SearchAngle { get; private set; }

        public TrafficNode ClosestTrafficNode { get; private set; }

        public int? ClosestTrafficNodeIndex { get; private set; }

        #endregion

        #region Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpsLocationTuple">Tuple in the order of < Latitude, Longitude ></param>
        /// <returns></returns>
        public GPSLocation ConvertTupleToGPSLocation(Tuple<double, double> gpsLocationTuple)
        {
            return new GPSLocation()
            {
                Latitude = gpsLocationTuple.Item1,
                Longitude = gpsLocationTuple.Item2
            };
        }

        public List<GPSLocation> ConvertTuplesToGPSLocations(List<Tuple<double, double>> gpsLocations)
        {
            var locations = new List<GPSLocation>();
            foreach (var gpsLocation in gpsLocations)
            {
                locations.Add(ConvertTupleToGPSLocation(gpsLocation));
            }
            return locations;

        }

        public TrafficNode ConvertTupleToTrafficNode(Tuple<double, double, string> trafficNodeTuple)
        {
            return new TrafficNode()
            {
                ID = trafficNodeTuple.Item3,
                GPSLocation = new GPSLocation()
                {
                    Latitude = trafficNodeTuple.Item1,
                    Longitude = trafficNodeTuple.Item2
                }
            };
        }

        public List<TrafficNode> ConvertTuplesToTrafficNodess(List<Tuple<double, double, string>> trafficNodeTuples)
        {
            var locations = new List<TrafficNode>();
            foreach (var trafficNode in trafficNodeTuples)
            {
                locations.Add(ConvertTupleToTrafficNode(trafficNode));
            }
            return locations;

        }

        public void UpdateVehicleLocationHistory(List<Tuple<double, double>> vehicleLocationTuples)
        {
            if (vehicleLocationTuples.Count < NLocationWindow + 1)
                throw new ArgumentOutOfRangeException("vehicleLocationTuples", vehicleLocationTuples.Count, string.Format("Number of elements in the list of GPS Locations smaller than processing window size +1. Processing window size is {0}.", NLocationWindow));

            VehicleLocationHistory = ConvertTuplesToGPSLocations(vehicleLocationTuples);
        }

        public void UpdateVehicleLocationHistory(List<GPSLocation> vehicleLocations)
        {
            VehicleLocationHistory = vehicleLocations;
        }

        public void UpdateNodePositions(List<Tuple<double, double, string>> trafficNodeTuples)
        {
            TrafficNodes = ConvertTuplesToTrafficNodess(trafficNodeTuples);
        }

        public void UpdateNodePositions(List<TrafficNode> trafficNodes)
        {
            TrafficNodes = trafficNodes;
        }
        
        public void UpdateLocationWindowSize(int newLocationAveragingWindowSize)
        {
            if (newLocationAveragingWindowSize < 2)
                throw new ArgumentOutOfRangeException("newLocationAveragingWindowSize", newLocationAveragingWindowSize, "This number has to be greater than 2");

            NLocationWindow = newLocationAveragingWindowSize;
        }

        public void UpdateSearchRadius(double newSearchRadius)
        {
            if (newSearchRadius < 0)
                throw new ArgumentOutOfRangeException("searchRadius", newSearchRadius, "Search Radius (in metres) cannot be less than 0");

            SearchRadius = newSearchRadius;
        }

        public void UpdateSearchAngle(double newSearchAngle)
        {
            if (newSearchAngle < 0)
                newSearchAngle = Math.Abs(newSearchAngle);

            SearchAngle = newSearchAngle;
        }

        public void Recompute()
        {
            ComputeClosestTrafficNode(NLocationWindow, VehicleLocationHistory);
        }

        private void ComputeClosestTrafficNode(int locationWindowSize, List<GPSLocation> vehicleLocations)
        {
            CurrentDirection = GetMovementDirection(VehicleLocationHistory, locationWindowSize);
            ClosestTrafficNodeIndex = GetLlikelyNodeIndex(vehicleLocations.Last(), CurrentDirection, TrafficNodes);
            ClosestTrafficNode = (ClosestTrafficNodeIndex != null) ? TrafficNodes[(int)ClosestTrafficNodeIndex] : null;
        }

        private Direction GetMovementDirection(List<GPSLocation> gpsLocations, int nWindow)
        {
            // Use average to take out "spike" like errors if they exist (i.e. apply low pass filter

            var elementCount = gpsLocations.Count();
            
            // Get average of last N elements
            var avrLatPointUltimate = gpsLocations.Skip(elementCount - nWindow).Average(x => x.Latitude);
            var avrLonPointUltimate = gpsLocations.Skip(elementCount - nWindow).Average(x => x.Longitude);

            // Get average of 2nd last N elements 
            var avrLatPointPenultimate = gpsLocations.Skip(elementCount - nWindow -1).Take(nWindow).Average(x => x.Latitude);
            var avrLonPointPenultimate = gpsLocations.Skip(elementCount - nWindow -1).Take(nWindow).Average(x => x.Longitude);

            var delLatitude = avrLatPointUltimate - avrLatPointPenultimate;
            var delLongitude = avrLonPointUltimate - avrLonPointPenultimate;

            return new Direction() { DeltaLatitude = delLatitude, DeltaLongitude = delLongitude };
        }

        private int? GetLlikelyNodeIndex(GPSLocation currentPosition, Direction direction, List<TrafficNode> trafficNodes)
        {
            // TODO: Trim out the obvious nodes out of the list

            // Order traffic Nodes w.r.t distance from vehicle
            UpdateNodeDistances(currentPosition, trafficNodes);

            // TODO: Only use Nodes that are within the search radius?

            // Order remaining traffic noces w.r.t direction difference 
            UpdateNodeAngleDiffs(currentPosition, direction, trafficNodes);

            // Only closest node in the right direction
            var nodesInTheRightDirection = trafficNodes.OrderBy(a => a.angleDiff).Where(a =>
                                                                                        (a.angleDiff >= (-1 * _searchAngleRad)) &&
                                                                                        (a.angleDiff <= _searchAngleRad));

            var nodesInTheRightRange = nodesInTheRightDirection.Where(a => a.distance < SearchRadius);
            if (nodesInTheRightRange.Count() > 0)
                return trafficNodes.IndexOf(nodesInTheRightRange.OrderBy(a => a.distance).First());
            else
                return null;

        }

        private void UpdateNodeAngleDiffs(GPSLocation currentPosition, Direction direction, List<TrafficNode> trafficNodes)
        {
            foreach (var trafficNode in trafficNodes)
            {
                trafficNode.angleDiff = GetDifferenceInAngles(currentPosition, direction, trafficNode.GPSLocation);
            }
        }

        private double? GetDifferenceInAngles(GPSLocation currentPosition, Direction movementDirection, GPSLocation trafficNodeLocation)
        {
            var tmpDirection = new Direction()
            {
                DeltaLatitude = trafficNodeLocation.Latitude - currentPosition.Latitude,
                DeltaLongitude = trafficNodeLocation.Longitude - currentPosition.Longitude
            };

            return tmpDirection.Angle - movementDirection.Angle;


        }

        private void UpdateNodeDistances(GPSLocation currentPosition, List<TrafficNode> trafficNodes)
        {
            foreach (var trafficNode in trafficNodes)
            {
                trafficNode.distance = GetDistanceBetweenCoordinates(currentPosition,  trafficNode.GPSLocation);
            }

        }

        private double GetDistanceBetweenCoordinates(GPSLocation location1, GPSLocation location2)
        {
            // using Haversine's formulae
            // http://www.movable-type.co.uk/scripts/latlong.html
            // https://stackoverflow.com/questions/365826/calculate-distance-between-2-gps-coordinates


            var dLatitude = (location2.Latitude * Deg2RadConst) - (location1.Latitude * Deg2RadConst);
            var dLongitude = (location2.Longitude * Deg2RadConst) - (location1.Longitude * Deg2RadConst);
            var lat1Rad = location1.Latitude * Deg2RadConst;
            var lat2Rad = location2.Latitude * Deg2RadConst;

            // compute Haversine's formulae
            var a = Math.Pow(Math.Sin(dLatitude / 2), 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Pow(Math.Sin(dLongitude / 2.0), 2);

            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = EarthsRadius * c;

            return d * 1000; // in meters

        }

        private double GetDistanceBetweenPointAndLine(Direction direction, GPSLocation pointOfInterest)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Member Variables
        private const double EarthsRadius = 6378.137; // in km
        private const double Deg2RadConst = Math.PI / 180.0;
        private const int DefaultLocationWindowSize = 3;
        private const double DefaultSearchRadius = 500.0; // in metres
        private const double DefaultSearchAngle = 50.0; // in degrees
        private double _searchAngleRad;
#endregion
    }
}

