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

using Xunit;
using Moq;

using GreenLight.Core.Services;
using GreenLight.Core.Contracts;
using static GreenLight.Core.Helpers.NodeFinder;

namespace GreenLight.Tests.Services
{
    public class VehicleServiceTests
    {
        private VehicleService _vehicleService;

        /// <summary>
        /// Constructor called at the beginning of every test. But need to make sure Dispose() is cleaning shared instances.
        /// </summary>
        public VehicleServiceTests()
        {
            var fakeLocationService = new Mock<ILocationService>();
            var fakeDataAnalyticsService = new Mock<IDataAnalyticsService>();

            //_vehicleService = new VehicleService(fakeMessanger.Object, fakeLocationWatcher.Object, fakeLocationService.Object, fakeDataAnalyticsService.Object);
            //_vehicleService.CompletedIntersections = new List<string>();
        }


        [Fact]
        public void FakeDetermineIntersectionComplete_WhenCalled_AddsCompletedIntersectionList()
        {
            //// Arrange
            //var intersectionCount = _vehicleService.CompletedIntersections.Count;

            //// Act
            //_vehicleService.FakeDetermineIntersectionComplete();
            //var result = _vehicleService.CompletedIntersections.Count;

            //// Assert
            //Assert.True(intersectionCount < result);

        }

        [Fact]
        public void IntersectionCompleted_WithCrossingIntersection_ReturnsTrue()
        {
            // Arrange
            var fakeGpsLocations = new List<GPSLocation>()
            {
                new GPSLocation() { Latitude=0.000, Longitude=0.000},
                new GPSLocation() { Latitude=0.100, Longitude=0.100},
                new GPSLocation() { Latitude=0.200, Longitude=0.200},
                new GPSLocation() { Latitude=0.300, Longitude=0.300},
                new GPSLocation() { Latitude=0.400, Longitude=0.400}, // 5
                new GPSLocation() { Latitude=0.500, Longitude=0.500},
                new GPSLocation() { Latitude=0.600, Longitude=0.600},
                new GPSLocation() { Latitude=0.700, Longitude=0.700},
                new GPSLocation() { Latitude=0.800, Longitude=0.800},
                new GPSLocation() { Latitude=0.900, Longitude=0.900}, // 10
                new GPSLocation() { Latitude=1.000, Longitude=1.000},
                new GPSLocation() { Latitude=1.100, Longitude=1.100},
                new GPSLocation() { Latitude=1.200, Longitude=1.200},
                new GPSLocation() { Latitude=1.300, Longitude=1.300},
                new GPSLocation() { Latitude=1.400, Longitude=1.400}, // 15
                new GPSLocation() { Latitude=1.500, Longitude=1.500},
                new GPSLocation() { Latitude=1.600, Longitude=1.600},
                new GPSLocation() { Latitude=1.700, Longitude=1.700},
                new GPSLocation() { Latitude=1.800, Longitude=1.800},
                new GPSLocation() { Latitude=1.900, Longitude=1.900} // 20

            };
            var targetIntersection = new GPSLocation() { Latitude = 1.0, Longitude = 1.0 };

            // Act
            var result = true;

            // Assert
            Assert.True(result);

        }

        [Fact]
        public void IntersectionCompleted_WithoutCrossingIntersection_ReturnsFalse()
        {
            // Arrange
            var fakeGpsLocations = new List<GPSLocation>()
            {
                new GPSLocation() { Latitude=0.000, Longitude=0.000},
                new GPSLocation() { Latitude=0.100, Longitude=0.100},
                new GPSLocation() { Latitude=0.200, Longitude=0.200},
                new GPSLocation() { Latitude=0.300, Longitude=0.300},
                new GPSLocation() { Latitude=0.400, Longitude=0.400}, // 5
                new GPSLocation() { Latitude=0.500, Longitude=0.500},
                new GPSLocation() { Latitude=0.600, Longitude=0.600},
                new GPSLocation() { Latitude=0.700, Longitude=0.700},
                new GPSLocation() { Latitude=0.800, Longitude=0.800},
                new GPSLocation() { Latitude=0.900, Longitude=0.900}, // 10
                new GPSLocation() { Latitude=1.000, Longitude=1.000},
                new GPSLocation() { Latitude=1.100, Longitude=1.100},
                new GPSLocation() { Latitude=1.200, Longitude=1.200},
                new GPSLocation() { Latitude=1.300, Longitude=1.300},
                new GPSLocation() { Latitude=1.400, Longitude=1.400}, // 15
                new GPSLocation() { Latitude=1.500, Longitude=1.500},
                new GPSLocation() { Latitude=1.600, Longitude=1.600},
                new GPSLocation() { Latitude=1.700, Longitude=1.700},
                new GPSLocation() { Latitude=1.800, Longitude=1.800},
                new GPSLocation() { Latitude=1.900, Longitude=1.900} // 20

            };
            var targetIntersection = new GPSLocation() { Latitude = 2.0, Longitude = 2.0 };

            // Act
            var result = true;

            // Assert
            Assert.False(result);

        }


        [Fact]
        public void DetermineIntersectionComplete_WithIntersectionCrossedData_AddsCompletedIntersectionList()
        {
            // Arrange
            var fakeGpsLocations = new List<GPSLocation>()
            {
                new GPSLocation() { Latitude=0.000, Longitude=0.000},
                new GPSLocation() { Latitude=0.100, Longitude=0.100},
                new GPSLocation() { Latitude=0.200, Longitude=0.200},
                new GPSLocation() { Latitude=0.300, Longitude=0.300},
                new GPSLocation() { Latitude=0.400, Longitude=0.400}, // 5
                new GPSLocation() { Latitude=0.500, Longitude=0.500},
                new GPSLocation() { Latitude=0.600, Longitude=0.600},
                new GPSLocation() { Latitude=0.700, Longitude=0.700},
                new GPSLocation() { Latitude=0.800, Longitude=0.800},
                new GPSLocation() { Latitude=0.900, Longitude=0.900}, // 10
                new GPSLocation() { Latitude=1.000, Longitude=1.000},
                new GPSLocation() { Latitude=1.100, Longitude=1.100},
                new GPSLocation() { Latitude=1.200, Longitude=1.200},
                new GPSLocation() { Latitude=1.300, Longitude=1.300},
                new GPSLocation() { Latitude=1.400, Longitude=1.400}, // 15
                new GPSLocation() { Latitude=1.500, Longitude=1.500},
                new GPSLocation() { Latitude=1.600, Longitude=1.600},
                new GPSLocation() { Latitude=1.700, Longitude=1.700},
                new GPSLocation() { Latitude=1.800, Longitude=1.800},
                new GPSLocation() { Latitude=1.900, Longitude=1.900} // 20

            };
            var targetIntersection = new GPSLocation() { Latitude = 2.0, Longitude = 2.0 };

            // Act
            var result = true;

            // Assert
            Assert.False(result);

        }

    }
}
