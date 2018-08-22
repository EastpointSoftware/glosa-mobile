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

using Xunit;

using GreenLight.Core.Helpers;

namespace GreenLight.Tests.Helpers
{
    public class DistanceTest 
    {
        [Fact]
        public void Deg2rad_CheckReturnValue_ShouldBeEqualAsExpect()
        {
            //arrange
            double inputDegree = 45;
            double expectedResult = 0.78;
            //act
            double actualResult = Distance.Deg2rad(inputDegree);
            //assert
            Assert.Equal(expectedResult, actualResult, 2);
        }

        [Fact]
        public void Rad2deg_CheckReturnValue_ShouldBeEqualAsExpect()
        {
            //arrange
            double inputRadian = 0.78;
            double expectedResult = 44.69;
            //act
            double actualResult = Distance.Rad2deg(inputRadian);
            //assert
            Assert.Equal(expectedResult, actualResult, 2);
        }

        [Fact]
        public void CalculateDistanceBetween2PointsKMs_CheckReturnValue_ShouldBeEqualAsExpect()
        {
            //arrange
            double eastpointLat = 52.2315125;
            double eastpointLong = 0.1491753;
            double miltonTescoLat = 52.2390487;
            double miltonTescoLong = 0.1541519;
            double expectedResult = 0.90;
            //act
            double actualResult = Distance.CalculateDistanceBetween2PointsKMs(eastpointLat, eastpointLong, miltonTescoLat, miltonTescoLong);

            //assert
            Assert.Equal(expectedResult, actualResult, 2);
        }

        [Fact]
        public void CalculateDistanceBetween2PointsMiles_CheckReturnValue_ShouldBeEqualAsExpect()
        {
            //arrange
            double eastpointLat = 52.2315125;
            double eastpointLong = 0.1491753;
            double miltonTescoLat = 52.2390487;
            double miltonTescoLong = 0.1541519;
            double expectedResult = 0.56;
            //act
            double actualResult = Distance.CalculateDistanceBetween2PointsMiles(eastpointLat, eastpointLong, miltonTescoLat, miltonTescoLong);

            //assert
            Assert.Equal(expectedResult, actualResult, 2);
        }
    }
}
