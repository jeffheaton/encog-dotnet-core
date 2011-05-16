using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Thermal;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create a Boltzmann machine.
    /// </summary>
    ///
    public class BoltzmannPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of annealing cycles per run.
        /// </summary>
        ///
        private int annealCycles;

        /// <summary>
        /// The number of neurons in the Boltzmann network.
        /// </summary>
        ///
        private int neuronCount;

        /// <summary>
        /// The number of cycles per run.
        /// </summary>
        ///
        private int runCycles;

        /// <summary>
        /// The current temperature.
        /// </summary>
        ///
        private double temperature;

        public BoltzmannPattern()
        {
            annealCycles = 100;
            runCycles = 1000;
            temperature = 0.0d;
        }

        /// <summary>
        /// Set the number of annealing cycles per run.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public int AnnealCycles
        {
            /// <returns>The number of annealing cycles per run.</returns>
            get { return annealCycles; }
            /// <summary>
            /// Set the number of annealing cycles per run.
            /// </summary>
            ///
            /// <param name="annealCycles_0">The new value.</param>
            set { annealCycles = value; }
        }


        /// <summary>
        /// Set the number of cycles per run.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public int RunCycles
        {
            /// <returns>The number of cycles per run.</returns>
            get { return runCycles; }
            /// <summary>
            /// Set the number of cycles per run.
            /// </summary>
            ///
            /// <param name="runCycles_0">The new value.</param>
            set { runCycles = value; }
        }


        /// <summary>
        /// Set the temperature.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double Temperature
        {
            /// <returns>The temperature.</returns>
            get { return temperature; }
            /// <summary>
            /// Set the temperature.
            /// </summary>
            ///
            /// <param name="temperature_0">The new value.</param>
            set { temperature = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not supported, will throw an exception, Boltzmann networks have no hidden
        /// layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A Boltzmann network has no hidden layers.");
        }

        /// <summary>
        /// Clear any properties set on this network.
        /// </summary>
        ///
        public void Clear()
        {
            neuronCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            var boltz = new BoltzmannMachine(neuronCount);
            boltz.Temperature = temperature;
            boltz.RunCycles = runCycles;
            boltz.AnnealCycles = annealCycles;
            return boltz;
        }


        /// <summary>
        /// Not used, will throw an exception.
        /// </summary>
        ///
        /// <value>Not used.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Not used, will throw an exception.
            /// </summary>
            ///
            /// <param name="activation">Not used.</param>
            set
            {
                throw new PatternError(
                    "A Boltzmann network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons. This is the same as the number of output
        /// neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the number of input neurons. This is the same as the number of output
            /// neurons.
            /// </summary>
            ///
            /// <param name="count">The number of input neurons.</param>
            set { neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. This is the same as the number of input
        /// neurons.
        /// </summary>
        ///
        /// <value>The number of output neurons.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons. This is the same as the number of input
            /// neurons.
            /// </summary>
            ///
            /// <param name="count">The number of output neurons.</param>
            set { neuronCount = value; }
        }

        #endregion
    }
}