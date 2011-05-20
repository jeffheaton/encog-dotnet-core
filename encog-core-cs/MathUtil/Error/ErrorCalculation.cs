//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.MathUtil.Error
{
    /// <summary>
    /// Calculate the error of a neural network. Encog currently supports three error
    /// calculation modes. See ErrorCalculationMode for more info.
    /// </summary>
    public class ErrorCalculation
    {
        /// <summary>
        /// The current error calculation mode.
        /// </summary>
        private static ErrorCalculationMode mode = ErrorCalculationMode.MSE;

        /// <summary>
        /// The overall error.
        /// </summary>
        private double globalError;

        /// <summary>
        /// The size of a set.
        /// </summary>
        private int setSize;

        /// <summary>
        /// The error calculation mode, this is static and therefore global to
        /// all Enocg training. If a particular training method only supports a
        /// particular error calculation method, it may override this value. It will
        /// not change the value set here, rather the training will occur with its
        /// preferred training method. Currently the only training method that does
        /// this is Levenberg Marquardt (LMA).
        /// 
        /// The default error mode for Encog is RMS.
        /// </summary>
        public static ErrorCalculationMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Returns the root mean square error for a complete training set. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double Calculate()
        {
            if (setSize == 0)
            {
                return 0;
            }

            switch (Mode)
            {
                case ErrorCalculationMode.RMS:
                    return CalculateRMS();
                case ErrorCalculationMode.MSE:
                    return CalculateMSE();
                default:
                    return CalculateMSE();
            }
        }


        /// <summary>
        /// Calculate the error with MSE. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateMSE()
        {
            if (setSize == 0)
            {
                return 0;
            }
            double err = globalError/setSize;
            return err;
        }


        /// <summary>
        /// Calculate the error with RMS. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateRMS()
        {
            if (setSize == 0)
            {
                return 0;
            }
            double err = Math.Sqrt(globalError/setSize);
            return err;
        }


        /// <summary>
        /// Calculate the error with ARCTAN. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateARCTAN()
        {
            return CalculateMSE();
        }

        /// <summary>
        /// Reset the error accumulation to zero.
        /// </summary>
        public void Reset()
        {
            globalError = 0;
            setSize = 0;
        }

        /// <summary>
        /// Called to update for each number that should be checked.
        /// </summary>
        /// <param name="actual">The actual number.</param>
        /// <param name="ideal">The ideal number.</param>
        public void UpdateError(double[] actual, double[] ideal)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                double delta = ideal[i] - actual[i];

                globalError += delta*delta;
            }

            setSize += ideal.Length;
        }

        /// <summary>
        /// Update the error with single values.
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="ideal">The ideal value.</param>
        public void UpdateError(double actual, double ideal)
        {
            double delta = ideal - actual;

            globalError += delta*delta;

            setSize++;
        }
    }
}
