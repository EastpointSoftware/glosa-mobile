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
using System.Diagnostics;
using System.Linq;

using GreenLight.Core.Models;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Helpers
{
    #region Internal Class
    public class GLOSAResult
    {
        public List<StateTimeMovementEvent> StateTimeMovementEvents { get; set; }
        public StateTimeMovementEvent CurrentStateTimeMovement { get; set; }
        public GLOSAErrors Errors { get; set; }
        public double TimeToTrafficLight { get; set; }
        public string Description { get; set; }
        public object Object { get; set; }
    }

    public enum GLOSAErrors
    {
        NoErrors,
        UnableToFindProjectedLaneForMovement,
        UnableToFindProjectedStateForLane,
        ProjectedLaneNotInSameDirection,
        UnableToProjectCurrentMovementState,
        UnableToProjectMovementStates,
        MovementStateEndTimeUknown,
        SPATMessagedExpired,
        MAPMessageExpired,
        WebServiceError,
        WebServiceXMLParsingError,
        UnKnownGPSData,
        UnsupportedMode,
    }

    #endregion

    public static class GLOSAHelper
    {
        #region Public
        public static GLOSAResult TimeToTraficLight(MapData map, SPAT spat, List<GPSLocation> gpsHistory, double? deviceHeading, ulong maneuver, int crocsTime)
        {
            GLOSAResult result = new GLOSAResult() { Errors = GLOSAErrors.NoErrors,};

            result = ProjectedLaneForManeuver(map, gpsHistory, deviceHeading, maneuver);

            if (result.Errors == GLOSAErrors.NoErrors)
            {
                var lane = (MapDataIntersectionsIntersectionGeometryGenericLane)result.Object;
                var description = result.Description;
                result = ProjectedSignalTimingsForLane(lane, spat, maneuver, crocsTime);
                result.Object = lane;
                result.Description = description;

                if (result.Errors == GLOSAErrors.NoErrors)
                {
                    var timeToTrafficNode = -1.0;

                    if (result.CurrentStateTimeMovement.MovementEvent == MovementEvent.Green)
                    {
                        timeToTrafficNode = result.CurrentStateTimeMovement.MovementTimespan.TotalSeconds;
                    }
                    else if (result.CurrentStateTimeMovement.MovementEvent == MovementEvent.Red)
                    {
                        //WAIT!
                    }
                    else if (result.CurrentStateTimeMovement.MovementEvent == MovementEvent.Amber)
                    {
                        //TODO
                    }

                    result.TimeToTrafficLight = timeToTrafficNode;
                }
            }
            else
            {
                result.Errors = GLOSAErrors.UnableToFindProjectedLaneForMovement;
            }
                        
            return result;
        }

        public static GLOSAResult IsDirectionOfVehicleInSameDirectionAsLane(object mapId, List<TrafficNode> laneNodes, double? deviceHeading, List<GPSLocation> vehicleGPSHistory, double tolerance, ulong maneuver)
        {
            // Take the nearest node as this should be closest to the target lane's signal
            var firstNode = laneNodes.First().GPSLocation;
            var secondNode = laneNodes[1].GPSLocation;

            // check disatnce between nodes as some nodes are not within 1m. Need to have distance to calculate direction (10m)?
            var distance = Distance.CalculateDistanceBetween2PointsKMs(firstNode.Latitude, firstNode.Longitude, secondNode.Latitude, secondNode.Longitude);
            if (distance <= 0.010)
            {
                var thirdNode = laneNodes[2].GPSLocation;
                secondNode = thirdNode;
            }

            double? laneHeadingDegrees = LocationHelper.DegreeBearing(secondNode.Latitude, secondNode.Longitude, firstNode.Latitude, firstNode.Longitude);
            int laneDegreesInt = Convert.ToInt32(laneHeadingDegrees);

            GPSLocation previousVehicleLocation = vehicleGPSHistory[vehicleGPSHistory.Count - 2];
            GPSLocation currentVehicleLocation = vehicleGPSHistory[vehicleGPSHistory.Count - 1];
            double? vehicleHeadingDegrees = LocationHelper.DegreeBearing(previousVehicleLocation.Latitude, previousVehicleLocation.Longitude, currentVehicleLocation.Latitude, currentVehicleLocation.Longitude);
            int vehicleDegreesInt = Convert.ToInt32(vehicleHeadingDegrees);

            int deviceHeadingInt = Convert.ToInt32(deviceHeading);

            GLOSAResult result = new GLOSAResult();
            var delta = LocationHelper.DeltaOfVehicleToLaneDirection(vehicleHeadingDegrees, laneHeadingDegrees);

            if (delta <= tolerance && delta >= -tolerance)
            {
                result.Errors = GLOSAErrors.NoErrors;
            }
            else
            {
                result.Errors = GLOSAErrors.ProjectedLaneNotInSameDirection;
            }
            result.Description = $"LaneHeading: { laneDegreesInt}, VehicleHeading: { vehicleDegreesInt}, Delta: { delta}";

            return result;
        }


        public static bool IsMAPMessageValid(DateTime timestamp)
        {
            var valid = true;
            var secondsExpired = Math.Abs((DateTime.Now - timestamp).TotalSeconds);

            valid = secondsExpired < Constants.MAP_MESSAGE_VALIDATION_PERIOD_SECONDS;

            return valid;
        }

        public static bool IsSPATMessageValid(DateTime timestamp)
        {
            var valid = true;
            var secondsExpired = Math.Abs((DateTime.Now - timestamp).TotalSeconds);

            valid = secondsExpired < Constants.SPAT_MESSAGE_VALIDATION_PERIOD_SECONDS;

            return valid;
        }

        public static bool IsCROCSTimeValid(int time)
        {
            bool result = (time != Constants.SPAT_MOVEMENT_EVENT_TIMING_UNKNOWN);

            return result;
        }

        public static GLOSAResult ProjectedSignalTimingsForLane(MapDataIntersectionsIntersectionGeometryGenericLane lane, SPAT spat, ulong maneuver, int crocsTime)
        {
            GLOSAResult crocsResult = new GLOSAResult();

            SPATIntersectionsIntersectionStateMovementState state = LocateSignalMovementStateForLane(spat, lane, maneuver);

            if (state != null)
            {
                List<StateTimeMovementEvent> stateTimeMovementEvents = ProjectedMovementEventStates(state, crocsTime);
                crocsResult.StateTimeMovementEvents = stateTimeMovementEvents;

                if (IsTimeWithinCurrentMovementSequence(crocsTime, stateTimeMovementEvents) == true)
                {
                    crocsResult.CurrentStateTimeMovement = FindNextMovementEvent(crocsTime, stateTimeMovementEvents);
                }
                else
                {
                    crocsResult.Errors = GLOSAErrors.UnableToProjectMovementStates;
                }
            }
            else
            {
                crocsResult.Errors = GLOSAErrors.UnableToFindProjectedStateForLane;
            }
            
            return crocsResult;
        }
        public static GLOSAResult ProjectedLaneForManeuver(MapData map, List<GPSLocation> gpsHistory, double? deviceHeading, ulong maneuver)
        {
            List<TrafficNode> trafficNodePositions = ExtractTrafficNodesFromMAP(map, maneuver);

            List<GPSLocation> gpsLocations = gpsHistory;

            GPSLocation location = gpsLocations.First();

            var sortedList = trafficNodePositions.
                OrderBy(m => Distance.CalculateDistanceBetween2PointsKMs(
                m.GPSLocation.Latitude,
                m.GPSLocation.Longitude,
                location.Latitude, location.Longitude)).ToList();

            TrafficNode nearestNode = sortedList[0];

            MapDataIntersectionsIntersectionGeometryGenericLane lane = null;

            MapDataIntersectionsIntersectionGeometryGenericLane[] lanes = map.intersections.IntersectionGeometry.laneSet;
            var possibleLanes = lanes.Where(genricLane => genricLane.laneID.ToString() == nearestNode.ID);

            if (possibleLanes.Count() > 0)
            {
                lane = possibleLanes.First();
            }

            // Verify the lane is in the same direction as the vehicle
            var nodes = ExtractTrafficNodesFromLane(lane);

            // Let's sort all lane nodes from MAP Ref Point
            var refPoint = map.intersections.IntersectionGeometry.refPoint;
            var mapLocation = new GPSLocation()
            {
                Latitude = refPoint.lat / Constants.MAPCoordinateIntConverterUnit,
                Longitude = refPoint.@long / Constants.MAPCoordinateIntConverterUnit,
            };

            // Sort the nodes by distance ascending
            var sortedNodes = nodes.OrderBy(node => Distance.CalculateDistanceBetween2PointsKMs(node.GPSLocation.Latitude, node.GPSLocation.Longitude, mapLocation.Latitude, mapLocation.Longitude)).ToList();

            GLOSAResult result = IsDirectionOfVehicleInSameDirectionAsLane(map.intersections.IntersectionGeometry.id.id, sortedNodes, deviceHeading, gpsHistory, 50, maneuver);
            result.Description = $"LaneId: {lane.laneID}, {result.Description}";
            result.Object = lane;

            return result;
        }

        public static List<TrafficNode> ExtractTrafficNodesFromMAP(MapData map, ulong maneuver)
        {
            List<TrafficNode> trafficNodePositions = new List<TrafficNode>();
            foreach (var genericLane in map.intersections.IntersectionGeometry.laneSet)
            {
                if (genericLane.nodeList.nodes.Length > 0)
                {
                    if (genericLane.connectsTo?.Count(connectingTo => connectingTo.connectingLane.maneuver == maneuver) > 0)
                    {
                        var lanes = ExtractTrafficNodesFromLane(genericLane);
                        trafficNodePositions.AddRange(lanes);
                    }
                }
            }

            return trafficNodePositions;
        }

        public static List<TrafficNode> ExtractTrafficNodesFromLane(MapDataIntersectionsIntersectionGeometryGenericLane lane)
        {
            List<TrafficNode> trafficNodePositions = new List<TrafficNode>();
            foreach (var node in lane.nodeList.nodes)
            {
                TrafficNode trafficNode = new TrafficNode()
                {
                    GPSLocation = new GPSLocation()
                    {
                        Latitude = node.delta.nodeLatLon.lat / Constants.MAPCoordinateIntConverterUnit,
                        Longitude = node.delta.nodeLatLon.lon / Constants.MAPCoordinateIntConverterUnit,
                    },
                    ID = lane.laneID.ToString(),
                };
                trafficNodePositions.Add(trafficNode);
            }

            return trafficNodePositions;
        }

        public static SPATIntersectionsIntersectionStateMovementState LocateSignalMovementStateForLane(SPAT spat, MapDataIntersectionsIntersectionGeometryGenericLane lane, ulong maneuver)
        {
            SPATIntersectionsIntersectionStateMovementState movementState = null;
            if (lane.connectsTo != null)
            {
                var laneConnection = lane.connectsTo.Where(connection => connection.connectingLane.maneuver == maneuver);
                if (laneConnection.Count() > 0)
                {
                    byte signalGroup = laneConnection.First().signalGroup;
                    movementState = spat.intersections.IntersectionState.states.Where(state => state.signalGroup == signalGroup).FirstOrDefault();
                }
            }

            return movementState;
        }

        /// <summary>
        // Amber (permissive-clearance)       : <startTime>21270</startTime> <minEndTime>21300</minEndTime>
        // Red   (stop-And-Remain)            : <startTime>21300</startTime> <minEndTime>21170</minEndTime>
        // RedAmber (pre-Movement)            : <startTime>21170</startTime> <minEndTime>21190</minEndTime>
        // Green (protected-Movement-Allowed) : <startTime>21190</startTime> <minEndTime>21270</minEndTime>

        // TODO - Use legal limits?
        // Regarding the 36002’s This shouldn’t be a problem because there are legal minimums 
        // Red 5 secs, 
        // Amber 2 secs, 
        // Green 7secs, 
        // closing Amber 3 secs.
        /// </summary>
        /// <param name="movementState"></param>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static List<StateTimeMovementEvent> ProjectedMovementEventStates(SPATIntersectionsIntersectionStateMovementState movementState, int crocsTime)
        {
            IList<SPATIntersectionsIntersectionStateMovementStateMovementEvent> movementEvents = movementState.statetimespeed;

            List<StateTimeMovementEvent> stateTimeMovementEvents = new List<StateTimeMovementEvent>();

            foreach (SPATIntersectionsIntersectionStateMovementStateMovementEvent moveEvent in movementEvents)
            {
                StateTimeMovementEvent stateTimeMovementEvent = new StateTimeMovementEvent
                {
                    MovementEvent = GetSignalStateByEventState(moveEvent.eventState),
                    MovementTimespan = TimeSpan.FromMilliseconds(0),
                    MinEndTime = moveEvent.timing.minEndTime,
                    SignalGroupId = movementState.signalGroup,
                };

                if (IsCROCSTimeValid(moveEvent.timing.minEndTime))
                {
                    var phaseCountdown = moveEvent.timing.minEndTime - crocsTime;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(phaseCountdown / Constants.SPAT_MOVEMENT_EVENT_TIMING_UNIT);
                    stateTimeMovementEvent.MovementTimespan = timeSpan;
                }
                
                stateTimeMovementEvents.Add(stateTimeMovementEvent);
            }

            return stateTimeMovementEvents;
        }

        public static MovementEvent GetSignalStateByEventState(string eventState)
        {
            MovementEvent signalState;
            if (eventState == Constants.MovementEventStateRedAmber) signalState = MovementEvent.RedAmber;
            else if (eventState == Constants.MovementEventStateAmber) signalState = MovementEvent.Amber;
            else if (eventState == Constants.MovementEventStateGreen) signalState = MovementEvent.Green;
            else signalState = MovementEvent.Red;

            return signalState;
        }

        public static DateTime SPATMessageTimeStamp(SPAT spat)
        {
            Debug.Assert(spat != null, "Parameter null, SPAT");

            var now = DateTime.Now;
            var timestamp = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            var spanInMinutes = TimeSpan.FromMinutes(spat.intersections.IntersectionState.moy);
            timestamp = timestamp.AddMinutes(spanInMinutes.Minutes);
            timestamp = timestamp.AddMilliseconds(spat.intersections.IntersectionState.timeStamp);

            return timestamp;
        }

        public static int ConvertTimeToCROCSTime(DateTime TimeStamp)
        {
            var currentUTCTimeSecondsPastTheHour = TimeStamp.Second * Constants.SPAT_MOVEMENT_EVENT_TIMING_UNIT + TimeStamp.Minute * 60 * Constants.SPAT_MOVEMENT_EVENT_TIMING_UNIT;
            var currentUTCTimeSecondsPastTheHourInTenthsOfSeconds = currentUTCTimeSecondsPastTheHour;

            return currentUTCTimeSecondsPastTheHourInTenthsOfSeconds;
        }

        public static StateTimeMovementEvent FindNextMovementEvent(int CurrentCROCSTime, List<StateTimeMovementEvent> movementEvents)
        {
            List<StateTimeMovementEvent> currentEvents = movementEvents.Where(item => item.MinEndTime >= CurrentCROCSTime).ToList();
            List<StateTimeMovementEvent> orderedCurrentEvents = currentEvents.OrderBy(movementEvent => movementEvent.MinEndTime).ToList();

            var currentEvent = orderedCurrentEvents.First();

            return currentEvent;
        }

        public static bool IsTimeWithinCurrentMovementSequence(int CurrentCROCSTime, List<StateTimeMovementEvent> movementEvents)
        {
            var current = movementEvents.Any(item => item.MinEndTime >= CurrentCROCSTime);

            if (current == false)
            {
                current = false;
            }

            return current;
        }

        public static bool IsFutureEvent(int CurrentCROCSTime, int movementTime)
        {
            return movementTime > CurrentCROCSTime;
        }

        public static IList<kmlDocumentPlacemark> SortMAPKMLDataByDistanceFromCurrentLocation(IList<kmlDocumentPlacemark> mapData, double latitude, double longitude)
        {
            var sortedList = mapData.
                OrderBy(m => Distance.CalculateDistanceBetween2PointsKMs(
                double.Parse(m.Point.coordinates.Split(',')[1]),
                double.Parse(m.Point.coordinates.Split(',')[0]),
                latitude, longitude)).ToList();

            return sortedList;
        }

        public static IList<MapData> SortMAPDataByDistanceFromCurrentLocation(IList<MapData> mapData, double latitude, double longitude)
        {
            if (mapData == null)
                return mapData;

            var sortedList = mapData.
                OrderBy(m => Distance.CalculateDistanceBetween2PointsKMs(
                double.Parse((m.intersections.IntersectionGeometry.refPoint.lat / Constants.MAPCoordinateIntConverterUnit).ToString()),
                double.Parse((m.intersections.IntersectionGeometry.refPoint.@long / Constants.MAPCoordinateIntConverterUnit).ToString()),
                latitude, longitude)).ToList();

            return sortedList;
        }

        #endregion

        #region TEST

        public static IList<kmlDocumentPlacemark> LoadTestRoute()
        {
            IList<kmlDocumentPlacemark> placemarks = KMLHelper.GLOSATestRoute();

            return placemarks;
        }
    }

    #endregion
}
