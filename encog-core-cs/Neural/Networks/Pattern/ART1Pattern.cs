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
    /// Pattern to create an ART-1 neural network.
    /// </summary>
    public class ART1Pattern : INeuralNetworkPattern
    {

        /// <summary>
        /// The F1 layer.
        /// </summary>
        public const String TAG_F1 = "F1";

        /// <summary>
        /// The F2 layer.
        /// </summary>
        public const String TAG_F2 = "F2";

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ART1Pattern));

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        private int inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private int outputNeurons;

        /// <summary>
        /// A parameter for F1 layer.
        /// </summary>
        private double a1 = 1;

        /// <summary>
        /// B parameter for F1 layer.
        /// </summary>
        private double b1 = 1.5;

        /// <summary>
        /// C parameter for F1 layer.
        /// </summary>
        private double c1 = 5;

        /// <summary>
        /// D parameter for F1 layer.
        /// </summary>
        private double d1 = 0.9;

        /// <summary>
        /// L parameter for net.
        /// </summary>
        private double l = 3;

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        private double vigilance = 0.9;

        /// <summary>
        /// This will fail, hidden layers are not supported for this type of
        /// network.
        /// </summary>
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A ART1 network has no hidden layers.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
            throw new PatternError(str);
        }

        /// <summary>
        /// Clear any properties set for this network.
        /// </summary>
        public void Clear()
        {
            this.inputNeurons = this.outputNeurons = 0;

        }

        /// <summary>
        /// Generate the neural network.
        /// </summary>
        /// <returns>The generated neural network.</returns>
        public BasicNetwork Generate()
        {
            BasicNetwork network = new BasicNetwork(new ART1Logic());

            int y = PatternConst.START_Y;

            ILayer layerF1 = new BasicLayer(new ActivationLinear(), false, this.inputNeurons);
            ILayer layerF2 = new BasicLayer(new ActivationLinear(), false, this.outputNeurons);
            ISynapse synapseF1toF2 = new WeightedSynapse(layerF1, layerF2);
            ISynapse synapseF2toF1 = new WeightedSynapse(layerF2, layerF1);
            layerF1.Next.Add(synapseF1toF2);
            layerF2.Next.Add(synapseF2toF1);

            // apply tags
            network.TagLayer(BasicNetwork.TAG_INPUT, layerF1);
            network.TagLayer(BasicNetwork.TAG_OUTPUT, layerF2);
            network.TagLayer(ART1Pattern.TAG_F1, layerF1);
            network.TagLayer(ART1Pattern.TAG_F2, layerF2);

            layerF1.X = PatternConst.START_X;
            layerF1.Y = y;
            y += PatternConst.INC_Y;

            layerF2.X = PatternConst.START_X;
            layerF2.Y = y;

            network.SetProperty(ARTLogic.PROPERTY_A1, this.a1);
            network.SetProperty(ARTLogic.PROPERTY_B1, this.b1);
            network.SetProperty(ARTLogic.PROPERTY_C1, this.c1);
            network.SetProperty(ARTLogic.PROPERTY_D1, this.d1);
            network.SetProperty(ARTLogic.PROPERTY_L, this.l);
            network.SetProperty(ARTLogic.PROPERTY_VIGILANCE, this.vigilance);

            network.Structure.FinalizeStructure();

            return network;
        }

        /// <summary>
        /// This method will throw an error, you can't set the activation function
        /// for an ART1. type network.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str = "Can't set the activation function for an ART1.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
            get
            {
                String str = "Can't get the activation function for an ART1.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                return this.inputNeurons;
            }
            set
            {
                this.inputNeurons = value;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get
            {
                return this.outputNeurons;
            }
            set
            {
                this.outputNeurons = value;
            }
        }

        /// <summary>
        /// The A1 parameter.
        /// </summary>
        public double A1
        {
            get
            {
                return this.a1;
            }
            set
            {
                this.a1 = value;
            }
        }

        /// <summary>
        /// The B1 parameter.
        /// </summary>
        public double B1
        {
            get
            {
                return this.b1;
            }
            set
            {
                this.b1 = value;
            }
        }

        /// <summary>
        /// The C1 parameter.
        /// </summary>
        public double C1
        {
            get
            {
                return this.c1;
            }
            set
            {
                this.c1 = value;
            }
        }

        /// <summary>
        /// The D1 parameter.
        /// </summary>
        public double D1
        {
            get
            {
                return this.d1;
            }
            set
            {
                this.d1 = value;
            }
        }

        /// <summary>
        /// The vigilance for the network.
        /// </summary>
        public double Vigilance
        {
            get
            {
                return this.vigilance;
            }
            set
            {
                this.vigilance = value;
            }
        }
    }
}
