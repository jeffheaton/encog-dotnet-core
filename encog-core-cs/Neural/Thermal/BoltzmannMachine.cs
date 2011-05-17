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
    ///
    public class BoltzmannMachine : ThermalNetwork
    {
        /// <summary>
        /// The property for run cycles.
        /// </summary>
        ///
        public const String RUN_CYCLES = "runCycles";

        /// <summary>
        /// The property for anneal cycles.
        /// </summary>
        ///
        public const String ANNEAL_CYCLES = "annealCycles";

        /// <summary>
        /// The number of cycles to anneal for.
        /// </summary>
        ///
        private int annealCycles;

        /// <summary>
        /// Count used to internally determine if a neuron is "off".
        /// </summary>
        ///
        private int[] off;

        /// <summary>
        /// Count used to internally determine if a neuron is "on".
        /// </summary>
        ///
        private int[] on;

        /// <summary>
        /// The number of cycles to run the network through before annealing.
        /// </summary>
        ///
        private int runCycles;

        /// <summary>
        /// The current temperature of the neural network. The higher the
        /// temperature, the more random the network will behave.
        /// </summary>
        ///
        private double temperature;

        /// <summary>
        /// The thresholds.
        /// </summary>
        ///
        private double[] threshold;

        /// <summary>
        /// Default constructors.
        /// </summary>
        ///
        public BoltzmannMachine()
        {
            annealCycles = 100;
            runCycles = 1000;
        }

        /// <summary>
        /// Construct a Boltzmann machine with the specified number of neurons.
        /// </summary>
        ///
        /// <param name="neuronCount">The number of neurons.</param>
        public BoltzmannMachine(int neuronCount) : base(neuronCount)
        {
            annealCycles = 100;
            runCycles = 1000;

            threshold = new double[neuronCount];
        }


        /// <value>the annealCycles to set</value>
        public int AnnealCycles
        {
            /// <returns>the annealCycles</returns>
            get { return annealCycles; }
            /// <param name="annealCycles_0">the annealCycles to set</param>
            set { annealCycles = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override int InputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return NeuronCount; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override int OutputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return NeuronCount; }
        }


        /// <value>the runCycles to set</value>
        public int RunCycles
        {
            /// <returns>the runCycles</returns>
            get { return runCycles; }
            /// <param name="runCycles_0">the runCycles to set</param>
            set { runCycles = value; }
        }


        /// <summary>
        /// Set the network temperature.
        /// </summary>
        ///
        /// <value>The temperature to operate the network at.</value>
        public double Temperature
        {
            /// <returns>The temperature the network is currently operating at.</returns>
            get { return temperature; }
            /// <summary>
            /// Set the network temperature.
            /// </summary>
            ///
            /// <param name="temperature_0">The temperature to operate the network at.</param>
            set { temperature = value; }
        }


        /// <summary>
        /// Set the thresholds.
        /// </summary>
        ///
        /// <value>The thresholds.</value>
        public double[] Threshold
        {
            /// <returns>the threshold</returns>
            get { return threshold; }
            /// <summary>
            /// Set the thresholds.
            /// </summary>
            ///
            /// <param name="t">The thresholds.</param>
            set { threshold = value; }
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
        public override sealed MLData Compute(MLData input)
        {
            var result = new BiPolarMLData(input.Count);
            EngineArray.ArrayCopy(input.Data, CurrentState.Data);
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
            temperature *= d;
        }

        /// <summary>
        /// Run the network until thermal equilibrium is established.
        /// </summary>
        ///
        public void EstablishEquilibrium()
        {
            int count = NeuronCount;

            if (on == null)
            {
                on = new int[count];
                off = new int[count];
            }

            for (int i = 0; i < count; i++)
            {
                on[i] = 0;
                off[i] = 0;
            }

            for (int n = 0; n < runCycles*count; n++)
            {
                Run((int) RangeRandomizer.Randomize(0, count - 1));
            }
            for (int n_0 = 0; n_0 < annealCycles*count; n_0++)
            {
                var i_1 = (int) RangeRandomizer.Randomize(0, count - 1);
                Run(i_1);
                if (CurrentState.GetBoolean(i_1))
                {
                    on[i_1]++;
                }
                else
                {
                    off[i_1]++;
                }
            }

            for (int i_2 = 0; i_2 < count; i_2++)
            {
                CurrentState.SetBoolean(i_2, on[i_2] > off[i_2]);
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
            double sum, probability;

            int count = NeuronCount;

            sum = 0;
            for (j = 0; j < count; j++)
            {
                sum += GetWeight(i, j)*((CurrentState.GetBoolean(j)) ? 1 : 0);
            }
            sum -= threshold[i];
            probability = 1/(1 + BoundMath.Exp(-sum/temperature));
            if (RangeRandomizer.Randomize(0, 1) <= probability)
            {
                CurrentState.SetBoolean(i, true);
            }
            else
            {
                CurrentState.SetBoolean(i, false);
            }
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