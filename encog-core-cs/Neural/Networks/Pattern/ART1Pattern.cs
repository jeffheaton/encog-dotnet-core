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
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
#if logging
using log4net;
using Encog.Engine.Network.Activation;
#endif

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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ART1Pattern));
#endif

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons { get; set; }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputNeurons { get; set; }

        /// <summary>
        /// A parameter for F1 layer.
        /// </summary>
        public double A1 { get; set; }

        /// <summary>
        /// B parameter for F1 layer.
        /// </summary>
        public double B1 { get; set; }

        /// <summary>
        /// C parameter for F1 layer.
        /// </summary>
        public double C1 { get; set; }

        /// <summary>
        /// D parameter for F1 layer.
        /// </summary>
        public double D1 { get; set; }

        /// <summary>
        /// L parameter for net.
        /// </summary>
        public double L { get; set; }

        /// <summary>
        /// The vigilance parameter.
        /// </summary>
        public double Vigilance { get; set; }

        /// <summary>
        /// Construct the pattern and set the default values.
        /// </summary>
        public ART1Pattern()
        {
            this.A1 = 1;
            this.B1 = 1.5;
            this.C1 = 5;
            this.D1 = 0.9;
            this.L = 3;
            this.Vigilance = 0.9;
        }

        /// <summary>
        /// This will fail, hidden layers are not supported for this type of
        /// network.
        /// </summary>
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A ART1 network has no hidden layers.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PatternError(str);
        }

        /// <summary>
        /// Clear any properties set for this network.
        /// </summary>
        public void Clear()
        {
            this.InputNeurons = this.OutputNeurons = 0;

        }

        /// <summary>
        /// Generate the neural network.
        /// </summary>
        /// <returns>The generated neural network.</returns>
        public BasicNetwork Generate()
        {
            BasicNetwork network = new BasicNetwork(new ART1Logic());

            int y = PatternConst.START_Y;

            ILayer layerF1 = new BasicLayer(new ActivationLinear(), false, this.InputNeurons);
            ILayer layerF2 = new BasicLayer(new ActivationLinear(), false, this.OutputNeurons);
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

            network.SetProperty(ARTLogic.PROPERTY_A1, this.A1);
            network.SetProperty(ARTLogic.PROPERTY_B1, this.B1);
            network.SetProperty(ARTLogic.PROPERTY_C1, this.C1);
            network.SetProperty(ARTLogic.PROPERTY_D1, this.D1);
            network.SetProperty(ARTLogic.PROPERTY_L, this.L);
            network.SetProperty(ARTLogic.PROPERTY_VIGILANCE, this.Vigilance);

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
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
            get
            {
                String str = "Can't get the activation function for an ART1.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
        }
    }
}
