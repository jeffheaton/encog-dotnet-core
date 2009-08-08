using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Construct a Bidirectional Access Memory (BAM) neural network.  This
    /// neural network type learns to associate one pattern with another.  The
    /// two patterns do not need to be of the same length.  This network has two 
    /// that are connected to each other.  Though they are labeled as input and
    /// output layers to Encog, they are both equal, and should simply be thought
    /// of as the two layers that make up the net.
    /// </summary>
    public class BAMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The F1 layer.
        /// </summary>
        public static String TAG_F1 = "F1";

        /// <summary>
        /// The F2 layer.
        /// </summary>
        public static String TAG_F2 = "F2";

        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        private int f1Neurons;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        private int f2Neurons;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BAMPattern));


       /// <summary>
       /// Unused, a BAM has no hidden layers.
       /// </summary>
       /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A BAM network has no hidden layers.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
            throw new PatternError(str);
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        public void Clear()
        {
            this.f1Neurons = this.f2Neurons = 0;

        }

        /// <summary>
        /// The generated network.
        /// </summary>
        /// <returns></returns>
        public BasicNetwork Generate()
        {
            BasicNetwork network = new BasicNetwork(new BAMLogic());

            ILayer f1Layer = new BasicLayer(new ActivationBiPolar(), false,
                    f1Neurons);
            ILayer f2Layer = new BasicLayer(new ActivationBiPolar(), false,
                    f2Neurons);
            ISynapse synapseInputToOutput = new WeightedSynapse(f1Layer,
                    f2Layer);
            ISynapse synapseOutputToInput = new WeightedSynapse(f2Layer,
                    f1Layer);
            f1Layer.AddSynapse(synapseInputToOutput);
            f2Layer.AddSynapse(synapseOutputToInput);

            network.TagLayer(BAMPattern.TAG_F1, f1Layer);
            network.TagLayer(BAMPattern.TAG_F2, f2Layer);

            network.Structure.FinalizeStructure();
            network.Structure.FinalizeStructure();

            f1Layer.Y = PatternConst.START_Y;
            f2Layer.Y = PatternConst.START_Y;

            f1Layer.X = PatternConst.START_X;
            f2Layer.X = PatternConst.INDENT_X;

            return network;
        }

        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str = "A BAM network can't specify a custom activation function.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
            get
            {
                String str = "A BAM network can't specify a custom activation function.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }

        }

        /// <summary>
        ///  Set the F1 neurons.
        /// </summary>
        public int F1Neurons
        {
            get
            {
                return this.f1Neurons;
            }
            set
            {
                this.f1Neurons = value;
            }
        }

        /// <summary>
        /// Set the F2 neurons.
        /// </summary>
        public int F2Neurons
        {
            get
            {
                return this.f2Neurons;
            }
            set
            {
                this.f2Neurons = value;
            }
        }

        /// <summary>
        /// The number of input neurons.  This will fail, a BAM has no input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                String str = "A BAM network has no input layer, consider getting F1 layer.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
            set
            {
                String str = "A BAM network has no input layer, consider setting F1 layer.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
        }

        /// <summary>
        /// The number of output neurons.  This will fail, a BAM has no output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get
            {
                String str = "A BAM network has no output layer, consider getting F2 layer.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
            set
            {
                String str = "A BAM network has no output layer, consider setting F2 layer.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
        }
    }
}
