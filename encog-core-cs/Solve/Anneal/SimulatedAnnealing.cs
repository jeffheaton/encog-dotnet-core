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
        public double StartTemperature { get; set; }
        
        /// <summary>
        /// The ending temperature.
        /// </summary>
        public double StopTemperature { get; set; }

        /// <summary>
        /// The number of cycles that will be used.
        /// </summary>
        public int Cycles { get; set; }
        
        /// <summary>
        /// The current score.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The current temperature.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        public bool ShouldMinimize { get; set; }


        /// <summary>
        /// Simple constructor, default to should minimize score.
        /// </summary>
        public SimulatedAnnealing()
        {
            this.ShouldMinimize = true;
        }

        /// <summary>
        /// Subclasses should provide a method that evaluates the score for the
        /// current solution.  
        /// </summary>
        /// <returns>Return the score.</returns>
        public abstract double PerformScoreCalculation();

        /// <summary>
        /// Subclasses must provide access to an array that makes up the solution. 
        /// </summary>
        /// <returns>An array that makes up the solution.</returns>
        public abstract UNIT_TYPE[] GetArray();

        /// <summary>
        /// Get a copy of the array.
        /// </summary>
        /// <returns>A copy of the array.</returns>
        public abstract UNIT_TYPE[] GetArrayCopy();


        /// <summary>
        /// Called to perform one cycle of the annealing process.
        /// </summary>
        public void Iteration()
        {
            UNIT_TYPE[] bestArray;

            Score = PerformScoreCalculation();
            bestArray = this.GetArrayCopy();

            this.Temperature = this.StartTemperature;

            for (int i = 0; i < Cycles; i++)
            {
                double curScore;
                Randomize();
                curScore = PerformScoreCalculation();

                if (this.ShouldMinimize)
                {
                    if (curScore < Score)
                    {
                        bestArray = this.GetArrayCopy();
                        Score = curScore;
                    }
                }
                else
                {
                    if (curScore > Score)
                    {
                        bestArray = this.GetArrayCopy();
                        Score = curScore;
                    }
                }

                this.PutArray(bestArray);
                double ratio = Math.Exp(Math.Log(StopTemperature
                        / StartTemperature)
                        / (Cycles - 1));
                Temperature *= ratio;
            }
        }

        /// <summary>
        /// Store the array. 
        /// </summary>
        /// <param name="array">The array to be stored.</param>
        public abstract void PutArray(UNIT_TYPE[] array);

        /// <summary>
        /// Randomize the weight matrix.
        /// </summary>
        public abstract void Randomize();

    }
}
