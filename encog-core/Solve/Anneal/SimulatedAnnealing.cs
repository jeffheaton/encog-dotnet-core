// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Anneal
{
    /// <summary>
    /// The basis for the simulated annealing method.
    /// </summary>
    /// <typeparam name="UNIT_TYPE"></typeparam>
    abstract public class SimulatedAnnealing<UNIT_TYPE>
    {
        /// <summary>
        /// The starting temperature.
        /// </summary>
        public double StartTemperature
        {
            get
            {
                return startTemperature;
            }
            set
            {
                this.startTemperature = value;
            }
        }

        /// <summary>
        /// The ending temperature.
        /// </summary>
        public double StopTemperature
        {
            get
            {
                return stopTemperature;
            }
            set
            {
                this.stopTemperature = value;
            }
        }

        /// <summary>
        /// The number of cycles that will be used, per iteration.
        /// </summary>
        public int Cycles
        {
            get
            {
                return cycles;
            }
            set
            {
                this.cycles = value;
            }
        }


        /// <summary>
        /// The current error.
        /// </summary>
        public double Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }

        /// <summary>
        /// The current temperature.
        /// </summary>
        public double Temperature
        {
            get
            {
                return temperature;
            }
        }


        /// <summary>
        /// The starting temperature.
        /// </summary>
        private double startTemperature;

        /// <summary>
        /// The ending temperature.
        /// </summary>
        private double stopTemperature;

        /// <summary>
        /// The number of cycles that will be used, per iteration.
        /// </summary>
        private int cycles;

        /// <summary>
        /// The current error.
        /// </summary>
        private double error;

        /// <summary>
        /// The current temperature.
        /// </summary>
        protected double temperature;

        /**
         * Subclasses should provide a method that evaluates the error for the
         * current solution. Those solutions with a lower error are better.
         * 
         * @return Return the error, as a percent.
         * @throws NeuralNetworkError
         *             Should be thrown if any sort of error occurs.
         */
        public abstract double DetermineError();

        /**
         * Subclasses must provide access to an array that makes up the solution.
         * 
         * @return An array that makes up the solution.
         */
        public abstract UNIT_TYPE[] GetArray();

        /**
         * Called to perform one cycle of the annealing process.
         */
        public void Iteration()
        {
            UNIT_TYPE[] bestArray;

            this.Error = DetermineError();
            bestArray = this.GetArrayCopy();

            this.temperature = this.StartTemperature;

            for (int i = 0; i < this.cycles; i++)
            {
                double curError;
                Randomize();
                curError = DetermineError();
                if (curError < this.Error)
                {
                    bestArray = this.GetArrayCopy();
                    this.Error = curError;
                }

                this.PutArray(bestArray);
                double ratio = Math.Exp(Math.Log(this.StopTemperature
                        / this.StartTemperature)
                        / (this.Cycles - 1));
                this.temperature *= ratio;
            }
        }

        /// <summary>
        /// Get a copy of the array.
        /// </summary>
        /// <returns>The new array copy.</returns>
        public abstract UNIT_TYPE[] GetArrayCopy();

        /// <summary>
        /// Use the specified array.
        /// </summary>
        /// <param name="array">The array to use.</param>
        public abstract void PutArray(UNIT_TYPE[] array);

        /// <summary>
        /// Randomize the values.
        /// </summary>
        public abstract void Randomize();
    }
}
