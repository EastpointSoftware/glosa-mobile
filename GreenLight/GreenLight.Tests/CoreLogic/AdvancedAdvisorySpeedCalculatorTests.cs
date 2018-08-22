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

using Xunit;

using GreenLight.Core.Helpers;

namespace GreenLight.Tests.CoreLogic
{
    public class AdvancedAdvisorySpeedCalculatorTests
    {
        [Fact]
        public void Constructor_NoArgs_SetsAccelerationToDefault()
        {
            // Arrange
            var calculator = new AdvancedAdvisorySpeedCalculator();

            // Act
            var result = calculator.AccelerationMagnitude;

            // Assert
            Assert.Equal(5.0, result);
        }

        [Fact]
        public void Constructor_Acceleration_SetsAcceleration()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator(3);

            var result = calculator.AccelerationMagnitude;

            Assert.Equal(3, result);
        }

        [Fact]
        public void Constructor_NoArgs_SetsReactionTimeToDefault()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator();

            var result = calculator.ReactionTime;

            Assert.Equal(3, result);
        }

        [Fact]
        public void Constructor_ReactionTime_SetsReactionTime()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator(reactionTime:4.0);

            var result = calculator.ReactionTime;

            Assert.Equal(4.0, result);
        }

        [Fact]
        public void Constructor_NoArgs_SetAccelerationPolarityTODefault()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator();

            var result = calculator.AccelerateOrDecelerate;

            Assert.Equal(AdvancedAdvisorySpeedCalculator.AccelerationPolarity.Accelerate, result);
        }

        [Fact]
        public void Calculate_ZerosForArgs_ReturnsError()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator();

            var result = calculator.Calculate(AdvancedAdvisorySpeedCalculator.AccelerationPolarity.Accelerate, 0, 0, 0);

            Assert.NotEqual(AdvancedAdvisorySpeedCalculator.CalculationErrors.NoErrors, result.Errors);
        }

        [Fact]
        public void Calculate_SomeNumbersForAccelerate_ReturnsSomething()
        {
            var calculator = new AdvancedAdvisorySpeedCalculator();

            var result = calculator.Calculate(AdvancedAdvisorySpeedCalculator.AccelerationPolarity.Accelerate, 20.0f, 200.0f, 20.0f);

            Assert.Equal(AdvancedAdvisorySpeedCalculator.CalculationErrors.NoErrors, result.Errors);
        }
    }
}
