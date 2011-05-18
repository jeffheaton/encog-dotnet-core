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

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BoltzmannPattern()
        {
            annealCycles = 100;
            runCycles = 1000;
            temperature = 0.0d;
        }

        /// <summary>
        /// Set the number of annealing cycles per run.
        /// </summary>
        public int AnnealCycles
        {
            get { return annealCycles; }
            set { annealCycles = value; }
        }


        /// <summary>
        /// Set the number of cycles per run.
        /// </summary>
        public int RunCycles
        {
            get { return runCycles; }
            set { runCycles = value; }
        }


        /// <summary>
        /// Set the temperature.
        /// </summary>
        public double Temperature
        {
            get { return temperature; }
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
            set { neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. This is the same as the number of input
        /// neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { neuronCount = value; }
        }

        #endregion
    }
}