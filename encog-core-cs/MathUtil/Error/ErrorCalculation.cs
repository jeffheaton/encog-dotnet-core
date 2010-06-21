// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Error;
using Encog.Neural.Data;

namespace Encog.MathUtil.Error
{
    /// <summary>
    /// Calculate the error of a neural network. Encog currently supports three error
    /// calculation modes. See ErrorCalculationMode for more info.
    /// </summary>
    public class ErrorCalculation
    {
        /// <summary>
        /// The overall error.
        /// </summary>
        private double globalError;

        /// <summary>
        /// The size of a set.
        /// </summary>
        private int setSize;

        /// <summary>
        /// The current error calculation mode.
        /// </summary>
        private static ErrorCalculationMode mode = ErrorCalculationMode.RMS;

        /// <summary>
        /// Returns the root mean square error for a complete training set. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double Calculate()
        {
            if (this.setSize == 0)
            {
                return 0;
            }

            switch (ErrorCalculation.Mode)
            {
                case ErrorCalculationMode.RMS:
                    return this.CalculateRMS();
                case ErrorCalculationMode.MSE:
                    return this.CalculateMSE();
                case ErrorCalculationMode.ARCTAN:
                    return this.CalculateARCTAN();
                default:
                    return this.CalculateMSE();
            }

        }

        
        /// <summary>
        /// Calculate the error with MSE. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateMSE()
        {
            if (this.setSize == 0)
            {
                return 0;
            }
            double err = this.globalError / this.setSize;
            return err;

        }


        /// <summary>
        /// Calculate the error with RMS. 
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateRMS()
        {
            if (this.setSize == 0)
            {
                return 0;
            }
            double err = Math.Sqrt(this.globalError / this.setSize);
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
            this.globalError = 0;
            this.setSize = 0;
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

                if (Mode == ErrorCalculationMode.ARCTAN)
                    delta = Math.Atan(delta);

                this.globalError += delta * delta;
            }

            this.setSize += ideal.Length;
        }

        /// <summary>
        /// Update the error. 
        /// </summary>
        /// <param name="actual">The actual values.</param>
        /// <param name="ideal">The ideal values.</param>
        public void UpdateError(INeuralData actual, INeuralData ideal)
        {
            UpdateError(actual.Data, ideal.Data);
        }

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
            get
            {
                return ErrorCalculation.mode;
            }
            set
            {
                ErrorCalculation.mode = value;
            }
        }

        /// <summary>
        /// Update the error with single values.
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="ideal">The ideal value.</param>
        public void UpdateError(double actual, double ideal)
        {

            double delta = ideal - actual;

            if ( Mode == ErrorCalculationMode.ARCTAN)
                delta = Math.Atan(delta);

            this.globalError += delta * delta;

            this.setSize++;

        }

    }
}
