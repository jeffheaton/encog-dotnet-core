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
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Specific;
using Encog.Util;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// Implements a Boltzmann machine.
    /// </summary>
    [Serializable]
    public class BoltzmannMachine : ThermalNetwork
    {
        /// <summary>
        /// The property for run cycles.
        /// </summary>
        ///
        public const String ParamRunCycles = "runCycles";

        /// <summary>
        /// The property for anneal cycles.
        /// </summary>
        ///
        public const String ParamAnnealCycles = "annealCycles";

        /// <summary>
        /// The number of cycles to anneal for.
        /// </summary>
        ///
        private int _annealCycles;

        /// <summary>
        /// Count used to internally determine if a neuron is "off".
        /// </summary>
        [NonSerialized]
        private int[] _off;

        /// <summary>
        /// Count used to internally determine if a neuron is "on".
        /// </summary>
        [NonSerialized]
        private int[] _on;

        /// <summary>
        /// The number of cycles to run the network through before annealing.
        /// </summary>
        ///
        private int _runCycles;

        /// <summary>
        /// The current temperature of the neural network. The higher the
        /// temperature, the more random the network will behave.
        /// </summary>
        ///
        private double _temperature;

        /// <summary>
        /// The thresholds.
        /// </summary>
        ///
        private double[] _threshold;

        /// <summary>
        /// Default constructors.
        /// </summary>
        ///
        public BoltzmannMachine()
        {
            _annealCycles = 100;
            _runCycles = 1000;
        }

        /// <summary>
        /// Construct a Boltzmann machine with the specified number of neurons.
        /// </summary>
        public BoltzmannMachine(int neuronCount) : base(neuronCount)
        {
            _annealCycles = 100;
            _runCycles = 1000;

            _threshold = new double[neuronCount];
        }


        /// <value>the annealCycles to set</value>
        public int AnnealCycles
        {
            get { return _annealCycles; }
            set { _annealCycles = value; }
        }


        /// <inheritdoc/>
        public override int InputCount
        {
            get { return NeuronCount; }
        }



        /// <inheritdoc/>
        public override int OutputCount
        {
            get { return NeuronCount; }
        }


        /// <value>the runCycles to set</value>
        public int RunCycles
        {
            get { return _runCycles; }
            set { _runCycles = value; }
        }


        /// <summary>
        /// Set the network temperature.
        /// </summary>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }


        /// <summary>
        /// Set the thresholds.
        /// </summary>
        public double[] Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        /// <summary>
        /// Note: for Boltzmann networks, you will usually want to call the "run"
        /// method to compute the output.
        /// This method can be used to copy the input data to the current state. A
        /// single iteration is then run, and the new current state is returned.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The new current state.</returns>
        public override sealed IMLData Compute(IMLData input)
        {
            var result = new BiPolarMLData(input.Count);
			input.CopyTo(CurrentState.Data, 0, input.Count);
            Run();
            EngineArray.ArrayCopy(CurrentState.Data, result.Data);
            return result;
        }

        /// <summary>
        /// Decrease the temperature by the specified amount.
        /// </summary>
        ///
        /// <param name="d">The amount to decrease by.</param>
        public void DecreaseTemperature(double d)
        {
            _temperature *= d;
        }

        /// <summary>
        /// Run the network until thermal equilibrium is established.
        /// </summary>
        ///
        public void EstablishEquilibrium()
        {
            int count = NeuronCount;

            if (_on == null)
            {
                _on = new int[count];
                _off = new int[count];
            }

            for (int i = 0; i < count; i++)
            {
                _on[i] = 0;
                _off[i] = 0;
            }

            for (int n = 0; n < _runCycles*count; n++)
            {
                Run((int) RangeRandomizer.Randomize(0, count - 1));
            }
            for (int n = 0; n < _annealCycles*count; n++)
            {
                var i = (int) RangeRandomizer.Randomize(0, count - 1);
                Run(i);
                if (CurrentState.GetBoolean(i))
                {
                    _on[i]++;
                }
                else
                {
                    _off[i]++;
                }
            }

            for (int i = 0; i < count; i++)
            {
                CurrentState.SetBoolean(i, _on[i] > _off[i]);
            }
        }


        /// <summary>
        /// Run the network for all neurons present.
        /// </summary>
        ///
        public void Run()
        {
            int count = NeuronCount;
            for (int i = 0; i < count; i++)
            {
                Run(i);
            }
        }

        /// <summary>
        /// Run the network for the specified neuron.
        /// </summary>
        ///
        /// <param name="i">The neuron to run for.</param>
        public void Run(int i)
        {
            int j;

            int count = NeuronCount;

            double sum = 0;
            for (j = 0; j < count; j++)
            {
                sum += GetWeight(i, j)*((CurrentState.GetBoolean(j)) ? 1 : 0);
            }
            sum -= _threshold[i];
            double probability = 1/(1 + BoundMath.Exp(-sum/_temperature));
            CurrentState.SetBoolean(i, RangeRandomizer.Randomize(0, 1) <= probability);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // nothing needed here
        }
    }
}
