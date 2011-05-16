using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Specific;
using Encog.Util;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// The thermal network forms the base class for Hopfield and Boltzmann machines.
    /// </summary>
    ///
    public abstract class ThermalNetwork : BasicML, MLMethod,
                                           MLAutoAssocation, MLResettable
    {
        /// <summary>
        /// Serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

        /// <summary>
        /// The current state of the thermal network.
        /// </summary>
        ///
        private BiPolarMLData currentState;

        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private int neuronCount;

        /// <summary>
        /// The weights.
        /// </summary>
        ///
        private double[] weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public ThermalNetwork()
        {
        }

        /// <summary>
        /// Construct the network with the specicified neuron count.
        /// </summary>
        ///
        /// <param name="neuronCount_0">The number of neurons.</param>
        public ThermalNetwork(int neuronCount_0)
        {
            neuronCount = neuronCount_0;
            weights = new double[neuronCount_0*neuronCount_0];
            currentState = new BiPolarMLData(neuronCount_0);
        }

        /// <summary>
        /// Set the neuron count.
        /// </summary>
        ///
        /// <value>The neuron count.</value>
        public int NeuronCount
        {
            /// <returns>Get the neuron count for the network.</returns>
            get { return neuronCount; }
            /// <summary>
            /// Set the neuron count.
            /// </summary>
            ///
            /// <param name="c">The neuron count.</param>
            set { neuronCount = value; }
        }

        /// <summary>
        /// Set the weight array.
        /// </summary>
        ///
        /// <value>The weight array.</value>
        public double[] Weights
        {
            /// <returns>The weights.</returns>
            get { return weights; }
            /// <summary>
            /// Set the weight array.
            /// </summary>
            ///
            /// <param name="w">The weight array.</param>
            set { weights = value; }
        }

        /// <summary>
        /// Set the current state.
        /// </summary>
        public BiPolarMLData CurrentState
        {
            get { return currentState; }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    currentState[i] = value[i];
                }
            }
        }

        #region MLAutoAssocation Members

        /// <summary>
        /// from Encog.ml.MLInput
        /// </summary>
        ///
        public abstract int InputCount { /// <summary>
            /// from Encog.ml.MLInput
            /// </summary>
            ///
            get; }


        /// <summary>
        /// from Encog.ml.MLOutput
        /// </summary>
        ///
        public abstract int OutputCount { /// <summary>
            /// from Encog.ml.MLOutput
            /// </summary>
            ///
            get; }


        /// <summary>
        /// from Encog.ml.MLRegression
        /// </summary>
        ///
        public abstract MLData Compute(
            MLData input);

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            CurrentState.Clear();
            EngineArray.Fill(weights, 0.0d);
        }

        #endregion

        /// <summary>
        /// Add to the specified weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value">The value to add.</param>
        public void AddWeight(int fromNeuron, int toNeuron,
                              double value_ren)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            if (index >= weights.Length)
            {
                throw new NeuralNetworkError("Out of range: fromNeuron:"
                                             + fromNeuron + ", toNeuron: " + toNeuron);
            }
            weights[index] += value_ren;
        }


        /// <returns>Calculate the current energy for the network. The network will
        /// seek to lower this value.</returns>
        public double CalculateEnergy()
        {
            double tempE = 0;
            int neuronCount_0 = NeuronCount;

            for (int i = 0; i < neuronCount_0; i++)
            {
                for (int j = 0; j < neuronCount_0; j++)
                {
                    if (i != j)
                    {
                        tempE += GetWeight(i, j)*currentState[i]
                                 *currentState[j];
                    }
                }
            }
            return -1*tempE/2;
        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        ///
        public void Clear()
        {
            EngineArray.Fill(weights, 0);
        }


        /// <summary>
        /// Get a weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <returns>The weight.</returns>
        public double GetWeight(int fromNeuron, int toNeuron)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            return weights[index];
        }


        /// <summary>
        /// Init the network.
        /// </summary>
        ///
        /// <param name="neuronCount_0">The neuron count.</param>
        /// <param name="weights_1">The weights.</param>
        /// <param name="output">The toutpu</param>
        public void Init(int neuronCount_0, double[] weights_1,
                         double[] output)
        {
            if (neuronCount_0 != output.Length)
            {
                throw new NeuralNetworkError("Neuron count(" + neuronCount_0
                                             + ") must match output count(" + output.Length + ").");
            }

            if ((neuronCount_0*neuronCount_0) != weights_1.Length)
            {
                throw new NeuralNetworkError("Weight count(" + weights_1.Length
                                             + ") must be the square of the neuron count(" + neuronCount_0
                                             + ").");
            }

            neuronCount = neuronCount_0;
            weights = weights_1;
            currentState = new BiPolarMLData(neuronCount_0);
            currentState.Data = output;
        }

        public void SetCurrentState(double[] s)
        {
            currentState = new BiPolarMLData(s.Length);
            EngineArray.ArrayCopy(s, currentState.Data);
        }

        /// <summary>
        /// Set the weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value">The value.</param>
        public void SetWeight(int fromNeuron, int toNeuron,
                              double value_ren)
        {
            int index = (toNeuron*neuronCount) + fromNeuron;
            weights[index] = value_ren;
        }
    }
}