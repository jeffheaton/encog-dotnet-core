using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.ART;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create an ART-1 neural network.
    /// </summary>
    ///
    public class ART1Pattern : NeuralNetworkPattern
    {
        /// <summary>
        /// A parameter for F1 layer.
        /// </summary>
        ///
        private double a1;

        /// <summary>
        /// B parameter for F1 layer.
        /// </summary>
        ///
        private double b1;

        /// <summary>
        /// C parameter for F1 layer.
        /// </summary>
        ///
        private double c1;

        /// <summary>
        /// D parameter for F1 layer.
        /// </summary>
        ///
        private double d1;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        ///
        private double l;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int outputNeurons;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        ///
        private double vigilance;

        public ART1Pattern()
        {
            a1 = 1;
            b1 = 1.5d;
            c1 = 5;
            d1 = 0.9d;
            l = 3;
            vigilance = 0.9d;
        }

        /// <summary>
        /// Set the A1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double A1
        {
            /// <returns>The A1 parameter.</returns>
            get { return a1; }
            /// <summary>
            /// Set the A1 parameter.
            /// </summary>
            ///
            /// <param name="a1_0">The new value.</param>
            set { a1 = value; }
        }


        /// <summary>
        /// Set the B1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double B1
        {
            /// <returns>The B1 parameter.</returns>
            get { return b1; }
            /// <summary>
            /// Set the B1 parameter.
            /// </summary>
            ///
            /// <param name="b1_0">The new value.</param>
            set { b1 = value; }
        }


        /// <summary>
        /// Set the C1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double C1
        {
            /// <returns>The C1 parameter.</returns>
            get { return c1; }
            /// <summary>
            /// Set the C1 parameter.
            /// </summary>
            ///
            /// <param name="c1_0">The new value.</param>
            set { c1 = value; }
        }


        /// <summary>
        /// Set the D1 parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double D1
        {
            /// <returns>The D1 parameter.</returns>
            get { return d1; }
            /// <summary>
            /// Set the D1 parameter.
            /// </summary>
            ///
            /// <param name="d1_0">The new value.</param>
            set { d1 = value; }
        }


        /// <summary>
        /// Set the L parameter.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double L
        {
            /// <returns>The L parameter.</returns>
            get { return l; }
            /// <summary>
            /// Set the L parameter.
            /// </summary>
            ///
            /// <param name="l_0">The new value.</param>
            set { l = value; }
        }


        /// <summary>
        /// Set the vigilance for the network.
        /// </summary>
        ///
        /// <value>The new value.</value>
        public double Vigilance
        {
            /// <returns>The vigilance for the network.</returns>
            get { return vigilance; }
            /// <summary>
            /// Set the vigilance for the network.
            /// </summary>
            ///
            /// <param name="vigilance_0">The new value.</param>
            set { vigilance = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// This will fail, hidden layers are not supported for this type of network.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A ART1 network has no hidden layers.");
        }

        /// <summary>
        /// Clear any properties set for this network.
        /// </summary>
        ///
        public void Clear()
        {
            inputNeurons = 0;
            outputNeurons = 0;
        }

        /// <summary>
        /// Generate the neural network.
        /// </summary>
        ///
        /// <returns>The generated neural network.</returns>
        public MLMethod Generate()
        {
            var art = new ART1(inputNeurons, outputNeurons);
            art.A1 = a1;
            art.B1 = b1;
            art.C1 = c1;
            art.D1 = d1;
            art.L = l;
            art.Vigilance = vigilance;
            return art;
        }


        /// <summary>
        /// This method will throw an error, you can't set the activation function
        /// for an ART1. type network.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// This method will throw an error, you can't set the activation function
            /// for an ART1. type network.
            /// </summary>
            ///
            /// <param name="activation">The activation function.</param>
            set { throw new PatternError("Can't set the activation function for an ART1."); }
        }


        /// <summary>
        /// Set the input neuron (F1 layer) count.
        /// </summary>
        ///
        /// <value>The input neuron count.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the input neuron (F1 layer) count.
            /// </summary>
            ///
            /// <param name="count">The input neuron count.</param>
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron (F2 layer) count.
        /// </summary>
        ///
        /// <value>The output neuron count.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the output neuron (F2 layer) count.
            /// </summary>
            ///
            /// <param name="count">The output neuron count.</param>
            set { outputNeurons = value; }
        }

        #endregion
    }
}