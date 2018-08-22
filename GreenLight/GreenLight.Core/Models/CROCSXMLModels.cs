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

namespace GreenLight.Core.Models
{
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private MapData mapDataField;
            private SPAT sPATField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public MapData MapData
            {
                get
                {
                    return this.mapDataField;
                }
                set
                {
                    this.mapDataField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public SPAT SPAT
            {
                get
                {
                    return this.sPATField;
                }
                set
                {
                    this.sPATField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class MapData
        {

            private byte msgIDField;

            private byte msgIssueRevisionField;

            private MapDataIntersections intersectionsField;

            /// <remarks/>
            public byte msgID
            {
                get
                {
                    return this.msgIDField;
                }
                set
                {
                    this.msgIDField = value;
                }
            }

            /// <remarks/>
            public byte msgIssueRevision
            {
                get
                {
                    return this.msgIssueRevisionField;
                }
                set
                {
                    this.msgIssueRevisionField = value;
                }
            }

            /// <remarks/>
            public MapDataIntersections intersections
            {
                get
                {
                    return this.intersectionsField;
                }
                set
                {
                    this.intersectionsField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersections
        {

            private MapDataIntersectionsIntersectionGeometry intersectionGeometryField;

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometry IntersectionGeometry
            {
                get
                {
                    return this.intersectionGeometryField;
                }
                set
                {
                    this.intersectionGeometryField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometry
        {

            private MapDataIntersectionsIntersectionGeometryID idField;

            private byte revisionField;

            private MapDataIntersectionsIntersectionGeometryRefPoint refPointField;

            private MapDataIntersectionsIntersectionGeometryGenericLane[] laneSetField;

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryID id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            public byte revision
            {
                get
                {
                    return this.revisionField;
                }
                set
                {
                    this.revisionField = value;
                }
            }

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryRefPoint refPoint
            {
                get
                {
                    return this.refPointField;
                }
                set
                {
                    this.refPointField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("GenericLane", IsNullable = false)]
            public MapDataIntersectionsIntersectionGeometryGenericLane[] laneSet
            {
                get
                {
                    return this.laneSetField;
                }
                set
                {
                    this.laneSetField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryID
        {

            private ushort idField;

            /// <remarks/>
            public ushort id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryRefPoint
        {

            private uint latField;

            private int longField;

            /// <remarks/>
            public uint lat
            {
                get
                {
                    return this.latField;
                }
                set
                {
                    this.latField = value;
                }
            }

            /// <remarks/>
            public int @long
            {
                get
                {
                    return this.longField;
                }
                set
                {
                    this.longField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLane
        {

            private byte laneIDField;

            private MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributes laneAttributesField;

            private MapDataIntersectionsIntersectionGeometryGenericLaneNodeList nodeListField;

            private MapDataIntersectionsIntersectionGeometryGenericLaneConnection[] connectsToField;

            /// <remarks/>
            public byte laneID
            {
                get
                {
                    return this.laneIDField;
                }
                set
                {
                    this.laneIDField = value;
                }
            }

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributes laneAttributes
            {
                get
                {
                    return this.laneAttributesField;
                }
                set
                {
                    this.laneAttributesField = value;
                }
            }

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryGenericLaneNodeList nodeList
            {
                get
                {
                    return this.nodeListField;
                }
                set
                {
                    this.nodeListField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Connection", IsNullable = false)]
            public MapDataIntersectionsIntersectionGeometryGenericLaneConnection[] connectsTo
            {
                get
                {
                    return this.connectsToField;
                }
                set
                {
                    this.connectsToField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributes
        {

            private byte directionalUseField;

            private uint sharedWithField;

            private MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributesLaneType laneTypeField;

            /// <remarks/>
            public byte directionalUse
            {
                get
                {
                    return this.directionalUseField;
                }
                set
                {
                    this.directionalUseField = value;
                }
            }

            /// <remarks/>
            public uint sharedWith
            {
                get
                {
                    return this.sharedWithField;
                }
                set
                {
                    this.sharedWithField = value;
                }
            }

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributesLaneType laneType
            {
                get
                {
                    return this.laneTypeField;
                }
                set
                {
                    this.laneTypeField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneLaneAttributesLaneType
        {

            private byte vehicleField;

            /// <remarks/>
            public byte vehicle
            {
                get
                {
                    return this.vehicleField;
                }
                set
                {
                    this.vehicleField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneNodeList
        {

            private MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNode[] nodesField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Node", IsNullable = false)]
            public MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNode[] nodes
            {
                get
                {
                    return this.nodesField;
                }
                set
                {
                    this.nodesField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNode
        {

            private MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDelta deltaField;

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDelta delta
            {
                get
                {
                    return this.deltaField;
                }
                set
                {
                    this.deltaField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDelta
        {

            private MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDeltaNodeLatLon nodeLatLonField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("node-LatLon")]
            public MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDeltaNodeLatLon nodeLatLon
            {
                get
                {
                    return this.nodeLatLonField;
                }
                set
                {
                    this.nodeLatLonField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneNodeListNodeDeltaNodeLatLon
        {

            private int lonField;

            private uint latField;

            /// <remarks/>
            public int lon
            {
                get
                {
                    return this.lonField;
                }
                set
                {
                    this.lonField = value;
                }
            }

            /// <remarks/>
            public uint lat
            {
                get
                {
                    return this.latField;
                }
                set
                {
                    this.latField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneConnection
        {

            private MapDataIntersectionsIntersectionGeometryGenericLaneConnectionConnectingLane connectingLaneField;

            private byte signalGroupField;

            /// <remarks/>
            public MapDataIntersectionsIntersectionGeometryGenericLaneConnectionConnectingLane connectingLane
            {
                get
                {
                    return this.connectingLaneField;
                }
                set
                {
                    this.connectingLaneField = value;
                }
            }

            /// <remarks/>
            public byte signalGroup
            {
                get
                {
                    return this.signalGroupField;
                }
                set
                {
                    this.signalGroupField = value;
                }
            }
        }

        /// <remarks/>
        
        
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MapDataIntersectionsIntersectionGeometryGenericLaneConnectionConnectingLane
        {

            private byte laneField;

            private ulong maneuverField;

            /// <remarks/>
            public byte lane
            {
                get
                {
                    return this.laneField;
                }
                set
                {
                    this.laneField = value;
                }
            }

            /// <remarks/>
            public ulong maneuver
            {
                get
                {
                    return this.maneuverField;
                }
                set
                {
                    this.maneuverField = value;
                }
            }
        }

    /// <remarks/>



    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SPAT
    {

        private byte msgIDField;

        private SPATIntersections intersectionsField;

        /// <remarks/>
        public byte msgID
        {
            get
            {
                return this.msgIDField;
            }
            set
            {
                this.msgIDField = value;
            }
        }

        /// <remarks/>
        public SPATIntersections intersections
        {
            get
            {
                return this.intersectionsField;
            }
            set
            {
                this.intersectionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersections
    {

        private SPATIntersectionsIntersectionState intersectionStateField;

        /// <remarks/>
        public SPATIntersectionsIntersectionState IntersectionState
        {
            get
            {
                return this.intersectionStateField;
            }
            set
            {
                this.intersectionStateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersectionsIntersectionState
    {

        private SPATIntersectionsIntersectionStateID idField;

        private byte revisionField;

        private ulong statusField;

        private uint moyField;

        private ushort timeStampField;

        private SPATIntersectionsIntersectionStateMovementState[] statesField;

        /// <remarks/>
        public SPATIntersectionsIntersectionStateID id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public byte revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        public ulong status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public uint moy
        {
            get
            {
                return this.moyField;
            }
            set
            {
                this.moyField = value;
            }
        }

        /// <remarks/>
        public ushort timeStamp
        {
            get
            {
                return this.timeStampField;
            }
            set
            {
                this.timeStampField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("MovementState", IsNullable = false)]
        public SPATIntersectionsIntersectionStateMovementState[] states
        {
            get
            {
                return this.statesField;
            }
            set
            {
                this.statesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersectionsIntersectionStateID
    {

        private ushort idField;

        /// <remarks/>
        public ushort id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersectionsIntersectionStateMovementState
    {

        private byte signalGroupField;

        private SPATIntersectionsIntersectionStateMovementStateMovementEvent[] statetimespeedField;

        /// <remarks/>
        public byte signalGroup
        {
            get
            {
                return this.signalGroupField;
            }
            set
            {
                this.signalGroupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("state-time-speed")]
        [System.Xml.Serialization.XmlArrayItemAttribute("MovementEvent", IsNullable = false)]
        public SPATIntersectionsIntersectionStateMovementStateMovementEvent[] statetimespeed
        {
            get
            {
                return this.statetimespeedField;
            }
            set
            {
                this.statetimespeedField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersectionsIntersectionStateMovementStateMovementEvent
    {

        private string eventStateField;

        private SPATIntersectionsIntersectionStateMovementStateMovementEventTiming timingField;

        /// <remarks/>
        public string eventState
        {
            get
            {
                return this.eventStateField;
            }
            set
            {
                this.eventStateField = value;
            }
        }

        /// <remarks/>
        public SPATIntersectionsIntersectionStateMovementStateMovementEventTiming timing
        {
            get
            {
                return this.timingField;
            }
            set
            {
                this.timingField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SPATIntersectionsIntersectionStateMovementStateMovementEventTiming
    {

        private ushort startTimeField;

        private ushort minEndTimeField;

        /// <remarks/>
        public ushort startTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public ushort minEndTime
        {
            get
            {
                return this.minEndTimeField;
            }
            set
            {
                this.minEndTimeField = value;
            }
        }
    }
}
