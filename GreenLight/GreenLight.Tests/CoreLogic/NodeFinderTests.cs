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

using Xunit;

using GreenLight.Core.Helpers;

namespace GreenLight.Tests
{
    public class NodeFinderTests
    {
        private NodeFinder _nodeFinder;
        private List<Tuple<double, double, string>> _fakeSetTrafficNodeTuples;
        private List<Tuple<double, double, string>> _fakeSetTrafficJunctionTuples;
        private List<Tuple<double, double>> _fakeSetVehicleHistoryTuples;

        /// <summary>
        /// Constructor to tests class. xUnit should create new instances of everything in the constructor for every test.
        /// </summary>
        public NodeFinderTests()
        {
            // Initialise
            _fakeSetVehicleHistoryTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2348011,0.1520),
                new Tuple<double, double>(52.2338011,0.15185),
                new Tuple<double, double>(52.2318011,0.15169109999999364),
                new Tuple<double, double>(52.2300000,0.15090000000),
                new Tuple<double, double>(52.229508018982365, 0.15089035034179688)
            };

            /*
             *
             *(origin) Eastpoint: 52.231801, 0.151691
             * (south) Mathworks: 52.229508, 0.15089
             * (east) Gold Driving Range: 52.229968, 0.154817
             * (north) St John's Innovation: 52.23554, 0.154002
             * (west) Cambridge Consultants: 52.23232, 0.143938
             * 
             * */

            _fakeSetTrafficNodeTuples = new List<Tuple<double, double, string>>
            {
                new Tuple<double, double, string>(52.2318011,0.15169109999999364, "EastPointLtd"),
                new Tuple<double, double, string>(52.229508018982365, 0.15089035034179688, "MathWorks"),
                new Tuple<double, double, string>(52.22996801547011, 0.1548171043395996, "Golf Driving Range"),
                new Tuple<double, double, string>(52.23554016575568, 0.15400171279907227, "St John's Innovation Centre"),
                new Tuple<double, double, string>(52.23232049441707, 0.1439380645751953, "Cambridge Consultants"),
            };

            _fakeSetTrafficJunctionTuples = new List<Tuple<double, double, string>>
            {
                new Tuple<double, double, string>(52.215637, 0.115804, "100-Victoria Road-Harvey Goodwin Ave"),
                new Tuple<double, double, string>(52.21651, 0.126316, "101-A1134-Gilbert Road"),
                new Tuple<double, double, string>(52.220858, 0.134243, "102-A1309-Arbury Road"),
                new Tuple<double, double, string>(52.227415, 0.145447, "103-Kings Hedges-Milton Road"),
                new Tuple<double, double, string>(52.231381, 0.150004, "104-Cambridge Science Park Rd-Milton Road"),
                new Tuple<double, double, string>(52.232770, 0.150722, "105-Cowley Rd-Milton Road"),
                new Tuple<double, double, string>(52.236805, 0.150548, "106-Roundabout-Milton Road"),
                new Tuple<double, double, string>(52.245327, 0.152907, "107-Butt Ln-Ely Road"),
                new Tuple<double, double, string>(52.271292, 0.178041, "108-Denny End Road-Ely Road")
            };

            _nodeFinder = new NodeFinder(_fakeSetVehicleHistoryTuples, _fakeSetTrafficNodeTuples);
        }

        [Fact]
        public void ConvertTupleToGPSLocation_WithData_ReturnsCorrectGPSLocationObject()
        {
            // arrange
            var newFakeGpsTuple = new Tuple<double, double>(10.123456, -10.123456);

            // act
            var gpsObject = _nodeFinder.ConvertTupleToGPSLocation(newFakeGpsTuple);

            // assert
            Assert.True(gpsObject.Latitude.Equals(newFakeGpsTuple.Item1));
            Assert.True(gpsObject.Longitude.Equals(newFakeGpsTuple.Item2));
        }

        [Fact]
        public void ConvertTuplesToGPSLocations_WithListOfGPSTuples_ReturnsCorrectGPSLocationsListObject()
        {
            // arrange
            var newFakeGpsTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double> (1.123456, -1.123456),
                new Tuple<double, double> (2.123456, -2.123456),
                new Tuple<double, double> (3.123456, -3.123456),
                new Tuple<double, double> (4.123456, -4.123456),
                new Tuple<double, double> (5.123456, -5.123456)
            };

            // act
            var gpsObjects = _nodeFinder.ConvertTuplesToGPSLocations(newFakeGpsTuples);

            // assert
            Assert.True(gpsObjects[0].Latitude.Equals(newFakeGpsTuples[0].Item1));
            Assert.True(gpsObjects[0].Longitude.Equals(newFakeGpsTuples[0].Item2));
            Assert.True(gpsObjects[1].Latitude.Equals(newFakeGpsTuples[1].Item1));
            Assert.True(gpsObjects[1].Longitude.Equals(newFakeGpsTuples[1].Item2));
            Assert.True(gpsObjects[2].Latitude.Equals(newFakeGpsTuples[2].Item1));
            Assert.True(gpsObjects[2].Longitude.Equals(newFakeGpsTuples[2].Item2));
            Assert.True(gpsObjects[3].Latitude.Equals(newFakeGpsTuples[3].Item1));
            Assert.True(gpsObjects[3].Longitude.Equals(newFakeGpsTuples[3].Item2));
            Assert.True(gpsObjects[4].Latitude.Equals(newFakeGpsTuples[4].Item1));
            Assert.True(gpsObjects[4].Longitude.Equals(newFakeGpsTuples[4].Item2));
        }

        [Fact]
        public void ConvertTupleToTrafficNode_WithData_ReturnsCorrectTrafficNodeObject()
        {
            var trafficNode = _nodeFinder.ConvertTupleToTrafficNode(_fakeSetTrafficNodeTuples[0]);

            Assert.Equal(trafficNode.GPSLocation.Latitude, _fakeSetTrafficNodeTuples[0].Item1);
            Assert.Equal(trafficNode.GPSLocation.Longitude, _fakeSetTrafficNodeTuples[0].Item2);

        }

        [Fact]
        public void ConvertTuplesToTrafficNodes_WithTupleData_ReturnsCorrectTrafficNodeListObject()
        {
            var newTrafficNodesList = new List<Tuple<double, double, string>>
            {
                new Tuple<double, double, string>(52.2318011,0.15169109999999364, "EastPointLtd"),
                new Tuple<double, double, string>(52.229508018982365, 0.15089035034179688, "MathWorks"),
                new Tuple<double, double, string>(52.22996801547011, 0.1548171043395996, "Golf Driving Range"),
                new Tuple<double, double, string>(52.23554016575568, 0.15400171279907227, "St John's Innovation Centre"),
                new Tuple<double, double, string>(52.23232049441707, 0.1439380645751953, "Cambridge Consultants"),
            };

            // act
            var trafficNodes = _nodeFinder.ConvertTuplesToTrafficNodess(newTrafficNodesList);

            // assert
            Assert.True(trafficNodes[0].GPSLocation.Latitude.Equals(newTrafficNodesList[0].Item1));
            Assert.True(trafficNodes[0].GPSLocation.Longitude.Equals(newTrafficNodesList[0].Item2));
            Assert.True(trafficNodes[1].GPSLocation.Latitude.Equals(newTrafficNodesList[1].Item1));
            Assert.True(trafficNodes[1].GPSLocation.Longitude.Equals(newTrafficNodesList[1].Item2));
            Assert.True(trafficNodes[2].GPSLocation.Latitude.Equals(newTrafficNodesList[2].Item1));
            Assert.True(trafficNodes[2].GPSLocation.Longitude.Equals(newTrafficNodesList[2].Item2));
            Assert.True(trafficNodes[3].GPSLocation.Latitude.Equals(newTrafficNodesList[3].Item1));
            Assert.True(trafficNodes[3].GPSLocation.Longitude.Equals(newTrafficNodesList[3].Item2));
            Assert.True(trafficNodes[4].GPSLocation.Latitude.Equals(newTrafficNodesList[4].Item1));
            Assert.True(trafficNodes[4].GPSLocation.Longitude.Equals(newTrafficNodesList[4].Item2));
        }

        [Fact]
        public void UpdateVehicleLocationHistory_WithTupleData_SetPropertyVehicleLocationHistory()
        {
            var newFakeGpsTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double> (0.123456, -1.123456),
                new Tuple<double, double> (1.123456, -2.123456),
                new Tuple<double, double> (2.123456, -3.123456),
                new Tuple<double, double> (3.123456, -4.123456),
                new Tuple<double, double> (4.123456, -5.123456)
            };

            _nodeFinder.UpdateVehicleLocationHistory(newFakeGpsTuples);

            var result = _nodeFinder.VehicleLocationHistory;

            Assert.Equal(newFakeGpsTuples[0].Item1, _nodeFinder.VehicleLocationHistory[0].Latitude);
            Assert.Equal(newFakeGpsTuples[0].Item2, _nodeFinder.VehicleLocationHistory[0].Longitude);
            Assert.Equal(newFakeGpsTuples[1].Item1, _nodeFinder.VehicleLocationHistory[1].Latitude);
            Assert.Equal(newFakeGpsTuples[1].Item2, _nodeFinder.VehicleLocationHistory[1].Longitude);
            Assert.Equal(newFakeGpsTuples[2].Item1, _nodeFinder.VehicleLocationHistory[2].Latitude);
            Assert.Equal(newFakeGpsTuples[2].Item2, _nodeFinder.VehicleLocationHistory[2].Longitude);
        }

        [Fact]
        public void UpdateNodePositions_WithTupleData_SetPropertyTrafficNodes()
        {
            _fakeSetTrafficNodeTuples = new List<Tuple<double, double, string>>
            {
                new Tuple<double, double, string>(52.2318011,0.15169109999999364, "EastPointLtd"),
                new Tuple<double, double, string>(52.229508018982365, 0.15089035034179688, "MathWorks"),
                new Tuple<double, double, string>(52.22996801547011, 0.1548171043395996, "Golf Driving Range"),
                new Tuple<double, double, string>(52.23554016575568, 0.15400171279907227, "St John's Innovation Centre"),
                new Tuple<double, double, string>(52.23232049441707, 0.1439380645751953, "Cambridge Consultants"),
            };

            _nodeFinder.UpdateNodePositions(_fakeSetTrafficNodeTuples);

            var result = _nodeFinder.TrafficNodes;

            Assert.Equal(_fakeSetTrafficNodeTuples[0].Item1, _nodeFinder.TrafficNodes[0].GPSLocation.Latitude);
            Assert.Equal(_fakeSetTrafficNodeTuples[0].Item2, _nodeFinder.TrafficNodes[0].GPSLocation.Longitude);
            Assert.Equal(_fakeSetTrafficNodeTuples[1].Item1, _nodeFinder.TrafficNodes[1].GPSLocation.Latitude);
            Assert.Equal(_fakeSetTrafficNodeTuples[1].Item2, _nodeFinder.TrafficNodes[1].GPSLocation.Longitude);
            Assert.Equal(_fakeSetTrafficNodeTuples[2].Item1, _nodeFinder.TrafficNodes[2].GPSLocation.Latitude);
            Assert.Equal(_fakeSetTrafficNodeTuples[2].Item2, _nodeFinder.TrafficNodes[2].GPSLocation.Longitude);
        }

        [Fact]
        public void UpdateLocationWindowSize_NewValue_SetPropertyNWindowSize()
        {
            _nodeFinder.UpdateLocationWindowSize(4);

            Assert.Equal(4, _nodeFinder.NLocationWindow);
        }

        [Fact]
        public void UpdateSearchRadius_NewValue_SetPropertySearchRadius()
        {
            _nodeFinder.UpdateSearchRadius(20);
            Assert.Equal(20, _nodeFinder.SearchRadius);
        }

        [Fact]
        public void UpdateSearchAngle_NewValue_SetPropertySearchAngle()
        {
            _nodeFinder.UpdateSearchAngle(45.0);
            Assert.Equal(45.0, _nodeFinder.SearchAngle);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertyTrafficNodes()
        {
            Assert.True(_nodeFinder.TrafficNodes != null);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertyVehicleLocationHistory()
        {
            Assert.True(_nodeFinder.VehicleLocationHistory != null);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertyCurrentDirection()
        {
            Assert.True(_nodeFinder.CurrentDirection != null);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertyNLocationWindowToDefault()
        {
            Assert.Equal(3, _nodeFinder.NLocationWindow);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertySearchRadiusToDefault()
        {
            Assert.Equal(500.0, _nodeFinder.SearchRadius);
        }

        [Fact]
        public void NodeFinder_With2Arguments_SetPropertySearchAngleToDefault()
        {
            Assert.Equal(50.0, _nodeFinder.SearchAngle);
        }

        [Fact]
        public void NodeFinder_With2Arguments_CorrectlyIdentifiesTheClosestNode()
        {
            // Go to MathWorks
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011,0.15169109999999364),
                new Tuple<double, double>(52.2315000,0.1515000000),
                new Tuple<double, double>(52.2300000,0.151350000000),
                new Tuple<double, double>(52.229808018982365, 0.1515035034179688)
            };

            // act
            var nodeFinder = new NodeFinder(fakeGPSTuples, _fakeSetTrafficNodeTuples);

            // assert
            Assert.Equal(1, nodeFinder.ClosestTrafficNodeIndex);

        }

        [Fact]
        public void NodeFinder_WithHistoryGoingEast_SetPropertyDirectionToEastwards()
        {
            // arrange
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011, 0.151),
                new Tuple<double, double>(52.2318011, 0.152),
                new Tuple<double, double>(52.2318011, 0.153),
                new Tuple<double, double>(52.2318011, 0.154),
                new Tuple<double, double>(52.2318011, 0.155)
            };

            _nodeFinder.UpdateVehicleLocationHistory(fakeGPSTuples);
            _nodeFinder.Recompute();

            var direction = _nodeFinder.CurrentDirection;

            // assert
            Assert.True(direction.DeltaLongitude > 0);
        }

        [Fact]
        public void NodeFinder_WithHistoryGoingWest_SetPropertyDirectionToWestward()
        {
            // arrange
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011, 0.153),
                new Tuple<double, double>(52.2318011, 0.152),
                new Tuple<double, double>(52.2318011, 0.151),
                new Tuple<double, double>(52.2318011, 0.150),
                new Tuple<double, double>(52.2318011, 0.149)
            };

            _nodeFinder.UpdateVehicleLocationHistory(fakeGPSTuples);
            _nodeFinder.Recompute();

            var direction = _nodeFinder.CurrentDirection;

            // assert
            Assert.True(direction.DeltaLongitude < 0);
        }

        [Fact]
        public void NodeFinder_WithHistoryGoingNorth_SetPropertyDirectionToNorthwards()
        {
            // arrange
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011, 0.151),
                new Tuple<double, double>(52.3318011, 0.152),
                new Tuple<double, double>(52.4318011, 0.153),
                new Tuple<double, double>(52.5318011, 0.153),
                new Tuple<double, double>(52.6318011, 0.153)
            };

            _nodeFinder.UpdateVehicleLocationHistory(fakeGPSTuples);
            _nodeFinder.Recompute();

            var direction = _nodeFinder.CurrentDirection;

            // assert
            Assert.True(direction.DeltaLatitude > 0);
        }

        [Fact]
        public void NodeFinder_WithHistoryGoingSouth_SetPropertyDirectionToSouthwards()
        {
            // arrange
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011, 0.151),
                new Tuple<double, double>(52.1318011, 0.152),
                new Tuple<double, double>(52.0318011, 0.153),
                new Tuple<double, double>(51.9318011, 0.152),
                new Tuple<double, double>(51.8318011, 0.152)
            };

            _nodeFinder.UpdateVehicleLocationHistory(fakeGPSTuples);
            _nodeFinder.Recompute();

            var direction = _nodeFinder.CurrentDirection;

            // assert
            Assert.True(direction.DeltaLatitude < 0);
        }

        [Fact]
        public void NodeFinder_WithLocationWindowSizeArgument_SetPropertyNWindowSize()
        {
            _nodeFinder = new NodeFinder(_fakeSetVehicleHistoryTuples, _fakeSetTrafficNodeTuples, locationWindowSize: 4);

            Assert.Equal(4, _nodeFinder.NLocationWindow);
        }

        [Fact]
        public void NodeFinder_InvalideWindowSizeArgument_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => _nodeFinder = new NodeFinder(_fakeSetVehicleHistoryTuples, _fakeSetTrafficNodeTuples, 1)
                );

            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void NodeFinder_WithSearchRadiusArgument_SetPropertySearchRadius()
        {
            _nodeFinder = new NodeFinder(_fakeSetVehicleHistoryTuples, _fakeSetTrafficNodeTuples, searchRadius: 400.0);

            Assert.Equal(400.0, _nodeFinder.SearchRadius);

        }

        [Fact]
        public void NodeFinder_WithSearchAngleArgument_SetPropertySearchAngle()
        {
            _nodeFinder = new NodeFinder(_fakeSetVehicleHistoryTuples, _fakeSetTrafficNodeTuples, searchAngle: 10.0);

            Assert.Equal(10.0, _nodeFinder.SearchAngle);
        }

        [Fact]
        public void NodeFinderForJunction_FromScienceParkToKingsHedges_IdentifiesKingsHedges()
        {
            var fakeGPSTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(52.2318011, 0.151),
                new Tuple<double, double>(52.2318011, 0.152),
                new Tuple<double, double>(52.2318011, 0.153),
                new Tuple<double, double>(52.2318011, 0.154),
                new Tuple<double, double>(52.2318011, 0.155)
            };

            //var fakeJunctionLists = new List<>
        }

    }
}
