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
    public class AdvancedAdvisorySpeedCalculator
    {
        #region Internal Class
 
        public enum CalculationErrors
        {
            NoErrors,
            NotEnoughTimeToReact,
            InvalidDistance,
            InvalidTimeToNode,
            InvalidVelocity,
            InvalidReactionTime,
            InvalidConditionForRemainingTimeComponent,
            InvalidCombinationOfTimeToNodeAndDistanceForAccelerationPolarity,
            TooFarToCalculateAccurately,
            AdvisorySpeedAboveSpeedLimit,
            AdvisorySpeedBelowHalfSpeedLimit,
        }

        public enum AccelerationPolarity
        {
            Accelerate,
            Decelerate
        }
        #endregion  

        #region Construction
        public AdvancedAdvisorySpeedCalculator(double accelerationMagnitude = _defaultAccelerationMagnitude, double reactionTime = _defaultReactionTime)
        {
            AccelerationMagnitude = accelerationMagnitude;
            ReactionTime = reactionTime;
        }
        #endregion

        #region App Life-Cycle

        #endregion

        #region Properties
        /// <summary>
        /// in m^2. Expected to be <0 for deceleration values. Probably should be treated as MAXIMUM allowable acceleration.
        /// </summary>
        public double AccelerationMagnitude { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public double ReactionTime { get; set; }

        public AccelerationPolarity AccelerateOrDecelerate { get; set; }
        #endregion

        #region Implementation
        public CalculationResult Calculate(AccelerationPolarity accelerationPolarity, double timeToNode, double distanceToNode, double currentSpeed)
        {
            var result = new CalculationResult();
            CorrectAccelerationToPolarity(accelerationPolarity, AccelerationMagnitude);

            var argsCheckResult = ValidateArguments(distanceToNode, timeToNode, currentSpeed, ReactionTime, AccelerationMagnitude);
            if (argsCheckResult != CalculationErrors.NoErrors)
            {
                // Error detected.
                result.AdvisorySpeed = null;
            } else {
                var advisorySpeed = currentSpeed + AccelerationMagnitude * (timeToNode - ReactionTime -
                    Math.Sqrt( Math.Pow(timeToNode - ReactionTime,2) - (2*(distanceToNode - timeToNode*currentSpeed))/AccelerationMagnitude)
                    );

                result.AdvisorySpeed = advisorySpeed;
            }

            result.Errors = argsCheckResult;
            return result;
        }

        private void CorrectAccelerationToPolarity(AccelerationPolarity accelerationPolarity, double accelerationMagnitude)
        {
            if ((accelerationPolarity == AccelerationPolarity.Accelerate) &&
                 (accelerationMagnitude <= 0))
                accelerationMagnitude *= -1;

            if ((accelerationPolarity == AccelerationPolarity.Decelerate) &&
                 (accelerationMagnitude >= 0))
                accelerationMagnitude *= -1;
        }

        private CalculationErrors ValidateArguments(double D, double T, double v, double tReact, double a)
        {
            if (D <= 0) return CalculationErrors.InvalidDistance;
            if (T <= 0) return CalculationErrors.InvalidTimeToNode;
            if (v < 0) return CalculationErrors.InvalidVelocity;
            if (tReact < 0) return CalculationErrors.InvalidReactionTime;

            if ((a > 0) && (D / v > T)) return CalculationErrors.InvalidCombinationOfTimeToNodeAndDistanceForAccelerationPolarity;
            if ((a< 0) && (D/v < T)) return CalculationErrors.InvalidCombinationOfTimeToNodeAndDistanceForAccelerationPolarity;

            // valid conditions
            if (T < ReactionTime) return CalculationErrors.NotEnoughTimeToReact;
            if ((a > 0) && (a < (2 * (D - T * v)) / (Math.Pow(T - ReactionTime, 2)))) return CalculationErrors.InvalidConditionForRemainingTimeComponent;

            // only accept advisory speed >0 
            if ((a < 0) && (a > (2 * (D - T * v)) / (Math.Pow(T - ReactionTime, 2)))) return CalculationErrors.InvalidConditionForRemainingTimeComponent;

            return CalculationErrors.NoErrors;
        }
        #endregion

        #region Member private variables
        private const double _defaultAccelerationMagnitude = 5.0;
        private const double _defaultReactionTime = 3.0;
        private const AccelerationPolarity _defaultPolarity = AccelerationPolarity.Accelerate;
        #endregion  
    }
}
