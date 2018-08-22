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

namespace GreenLight.Core.Helpers
{
    public class BasicSpeedAdvisoryCalculator
    {
        #region Construction
        public BasicSpeedAdvisoryCalculator()
        {
           
        }
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="timespanToCurrentMovementStateEnd"></param>
        /// <returns></returns>
        public CalculationResult Calculate(double distance, double timespanToCurrentMovementStateEnd)
        {
            CalculationResult result = new CalculationResult();
            
            result.AdvisorySpeed = GetAdvisorySpeed(distance, timespanToCurrentMovementStateEnd);

            return result;
        }

        private double GetAdvisorySpeed(double distance, double timeToSignalPhaseEnd)
        {
            var advisorySpeed = Math.Round(distance / timeToSignalPhaseEnd, 0);

            if (Double.IsInfinity(advisorySpeed) == true || advisorySpeed < 0)
            {
                advisorySpeed = -1;
            }
            return advisorySpeed;
        }
		#endregion
    }
}
