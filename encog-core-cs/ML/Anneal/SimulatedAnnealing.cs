//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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

namespace Encog.ML.Anneal
{
    /// <summary>
    /// Simulated annealing is a common training method. This class implements a
    /// simulated annealing algorithm that can be used both for neural networks, as
    /// well as more general cases. This class is abstract, so a more specialized
    /// simulated annealing subclass will need to be created for each intended use.
    /// This book demonstrates how to use the simulated annealing algorithm to train
    /// feedforward neural networks, as well as find a solution to the traveling
    /// salesman problem.
    /// The name and inspiration come from annealing in metallurgy, a technique
    /// involving heating and controlled cooling of a material to increase the size
    /// of its crystals and reduce their defects. The heat causes the atoms to become
    /// unstuck from their initial positions (a local minimum of the internal energy)
    /// and wander randomly through states of higher energy; the slow cooling gives
    /// them more chances of finding configurations with lower internal energy than
    /// the initial one.
    /// </summary>
    ///
    /// <typeparam name="TUnitType">What type of data makes up the solution.</typeparam>
    public abstract class SimulatedAnnealing<TUnitType>
    {
        /// <summary>
        /// The number of cycles that will be used.
        /// </summary>
        ///
        private int _cycles;

        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        ///
        private bool _shouldMinimize;

        /// <summary>
        /// The current temperature.
        /// </summary>
        ///
        private double _temperature;

        /// <summary>
        /// Construct the object.  Default ShouldMinimize to true.
        /// </summary>
        protected SimulatedAnnealing()
        {
            _shouldMinimize = true;
        }

        /// <summary>
        /// Subclasses must provide access to an array that makes up the solution.
        /// </summary>
        public abstract TUnitType[] Array { 
            get; }


        /// <summary>
        /// Get a copy of the array.
        /// </summary>
        public abstract TUnitType[] ArrayCopy {
            get; }


        /// <value>the cycles to set</value>
        public int Cycles
        {
            get { return _cycles; }
            set { _cycles = value; }
        }


        /// <summary>
        /// Set the score.
        /// </summary>
        public double Score { get; set; }


        /// <value>the startTemperature to set</value>
        public double StartTemperature { get; set; }


        /// <value>the stopTemperature to set</value>
        public double StopTemperature { get; set; }


        /// <value>the temperature to set</value>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }


        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        public bool ShouldMinimize
        {
            get { return _shouldMinimize; }
            set { _shouldMinimize = value; }
        }

        /// <summary>
        /// Subclasses should provide a method that evaluates the score for the
        /// current solution. Those solutions with a lower score are better.
        /// </summary>
        ///
        /// <returns>Return the score.</returns>
        public abstract double PerformCalculateScore();


        /// <summary>
        /// Called to perform one cycle of the annealing process.
        /// </summary>
        ///
        public void Iteration()
        {
            Score = PerformCalculateScore();
            TUnitType[] bestArray = ArrayCopy;

            _temperature = StartTemperature;

            for (int i = 0; i < _cycles; i++)
            {
                Randomize();
                double curScore = PerformCalculateScore();

                if (_shouldMinimize)
                {
                    if (curScore < Score)
                    {
                        bestArray = ArrayCopy;
                        Score = curScore;
                    }
                }
                else
                {
                    if (curScore > Score)
                    {
                        bestArray = ArrayCopy;
                        Score = curScore;
                    }
                }

                PutArray(bestArray);
                double ratio = Math.Exp(Math.Log(StopTemperature
                                                 /StartTemperature)
                                        /(Cycles - 1));
                _temperature *= ratio;
            }
        }

        /// <summary>
        /// Store the array.
        /// </summary>
        ///
        /// <param name="array">The array to be stored.</param>
        public abstract void PutArray(TUnitType[] array);

        /// <summary>
        /// Randomize the weight matrix.
        /// </summary>
        ///
        public abstract void Randomize();
    }
}
