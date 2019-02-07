using GreenLight.Core.Helpers;
using GreenLight.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Core.Objects
{
    public class IntersectionNode : TrafficNode
    {
        public MapData MapData { get; set; } 

        public static IList<IntersectionNode> IntersectionNodesFromMAPData(IList<MapData> mapDatas)
        {
            List<IntersectionNode> nodes = new List<IntersectionNode>();
            foreach (var mapPoint in mapDatas)
            {
                var refPoint = mapPoint.intersections.IntersectionGeometry.refPoint;
                var mapLocation = new GPSLocation()
                {
                    Latitude = refPoint.lat / Constants.MAPCoordinateIntConverterUnit,
                    Longitude = refPoint.@long / Constants.MAPCoordinateIntConverterUnit,
                };

                nodes.Add(new IntersectionNode()
                {
                    GPSLocation = mapLocation,
                    ID = mapPoint.intersections.IntersectionGeometry.id.id.ToString(),
                    angleDiff = 0,
                    MapData = mapPoint
                });
            }
            return nodes;
        }

        public static IList<IntersectionNode> SortIntersectionNodes(IList<IntersectionNode> intersectionNodes, GPSLocation refPoint, double? currentBearing, double viewArc, double distance)
        {
            foreach (var node in intersectionNodes)
            {
                double? bearingLocationToWaypoint = LocationHelper.HeadingBetweenTwoGPSLocations(refPoint, node.GPSLocation);
                double delta = LocationHelper.DeltaOfVehicleToLaneDirection(currentBearing, bearingLocationToWaypoint);
                node.angleDiff = Math.Abs(delta);//Make positive
                node.distance = Distance.CalculateDistanceBetween2PointsKMs(refPoint.Latitude, refPoint.Longitude, node.GPSLocation.Latitude, node.GPSLocation.Longitude);
            }

            var ordered = intersectionNodes.OrderBy(wp => wp.distance)
                        .ThenBy(wp => wp.angleDiff)
                        .Where(wp => wp.angleDiff >= -viewArc && wp.angleDiff <= viewArc && wp.distance <= distance);


            return ordered.ToList();
        }

        public static IList<IntersectionNode> Filter(IList<IntersectionNode> intersectionNodes, double? heading, List<GPSLocation> gpsTrail)
        {
            return intersectionNodes.Where(n => GLOSAHelper.ProjectedLaneForManeuver(n.MapData, gpsTrail, heading, Constants.MANEUVER_DIRECTION_AHEAD).Errors == GLOSAErrors.NoErrors).ToList();
        }
    }
}
