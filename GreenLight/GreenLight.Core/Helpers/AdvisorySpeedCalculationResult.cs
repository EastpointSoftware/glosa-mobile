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

using static GreenLight.Core.Helpers.AdvancedAdvisorySpeedCalculator;

namespace GreenLight.Core.Helpers
{
    public enum AdvisoryCalculatorMode
    {
        Basic = 0,
        Normal = 1,
        Advanced = 2,
    }

    public class CalculationResult
    {
        public double? AdvisorySpeed { get; set; }
        public CalculationErrors Errors { get; set; }
    }
    public static class AdvisorySpeedCalculationResult
    {
        public static CalculationResult CalculateAdvisorySpeed(int distance, double timeToIntersection, double currentSpeed, AdvisoryCalculatorMode advisoryCalculatorMode)
        {
            CalculationResult calculationResult = new CalculationResult();
            calculationResult.AdvisorySpeed = -1;
            calculationResult.Errors = CalculationErrors.NoErrors;

            if (advisoryCalculatorMode == AdvisoryCalculatorMode.Advanced)
            {
                AdvancedAdvisorySpeedCalculator advancedAdvisorySpeedCalculator = new AdvancedAdvisorySpeedCalculator();
                calculationResult = advancedAdvisorySpeedCalculator.Calculate(AccelerationPolarity.Decelerate, timeToIntersection, distance, currentSpeed);
            }
            else
            {
                BasicSpeedAdvisoryCalculator calculator = new BasicSpeedAdvisoryCalculator();
                calculationResult = calculator.Calculate(distance, timeToIntersection);
            }

            var speedMPH = Convert.ToInt32(calculationResult.AdvisorySpeed) < 0 ? 0 : Convert.ToInt32(calculationResult.AdvisorySpeed);
            speedMPH = Convert.ToInt32(SpeedConverter.ConvertFromMetersPerSecondToMilePerHour(speedMPH));
            if (speedMPH > Settings.SpeedLimit)
            {
                calculationResult.Errors = CalculationErrors.AdvisorySpeedAboveSpeedLimit;
            }
            else if (speedMPH < (Settings.SpeedLimit / 2))
            {
                calculationResult.Errors = CalculationErrors.AdvisorySpeedBelowHalfSpeedLimit;
            }

            return calculationResult;
        }
    }
}
