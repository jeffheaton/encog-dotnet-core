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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Data;
using Encog.Neural.Networks.Pattern;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an ART1 type network.  See ART1Pattern
    /// for more information on this type of network.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ART1Logic : ARTLogic
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(ART1Logic));
#endif

        /// <summary>
        /// The first layer, basically, the input layer.
        /// </summary>
        private ILayer layerF1;

        /// <summary>
        /// The second layer, basically, the output layer.
        /// </summary>
        private ILayer layerF2;

        /// <summary>
        /// The connection from F1 to F2.
        /// </summary>
        private ISynapse synapseF1toF2;

        /**
         * The connection from F2 to F1.
         */
        private ISynapse synapseF2toF1;

        /// <summary>
        /// Last winner in F2 layer.
        /// </summary>
        private int winner;

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
        /// Allows members of the F2 layer to be inhibited.
        /// </summary>
        private bool[] inhibitF2;

        /// <summary>
        /// Tracks if there was no winner.
        /// </summary>
        private int noWinner;

        /// <summary>
        /// The output from the F1 layer.
        /// </summary>
        private BiPolarNeuralData outputF1;

        /// <summary>
        /// The output from the F2 layer.
        /// </summary>
        private BiPolarNeuralData outputF2;


        /// <summary>
        /// Reset the weight matrix back to starting values.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < this.layerF1.NeuronCount; i++)
            {
                for (int j = 0; j < this.layerF2.NeuronCount; j++)
                {
                    this.synapseF1toF2.WeightMatrix[i, j] =
                            (this.b1 - 1) / this.d1 + 0.2;
                    this.synapseF2toF1.WeightMatrix[j, i] =
                            this.l / (this.l - 1 + this.layerF1.NeuronCount) - 0.1;
                }
            }
        }

        /// <summary>
        /// Adjust the weights for the pattern just presented.
        /// </summary>
        public void AdjustWeights()
        {
            double magnitudeInput;

            for (int i = 0; i < this.layerF1.NeuronCount; i++)
            {
                if (this.outputF1.GetBoolean(i))
                {
                    magnitudeInput = Magnitude(this.outputF1);
                    this.synapseF1toF2.WeightMatrix[i, this.winner] = 1;
                    this.synapseF2toF1.WeightMatrix[this.winner, i] =
                            this.l / (this.l - 1 + magnitudeInput);
                }
                else
                {
                    this.synapseF1toF2.WeightMatrix[i, this.winner] = 0;
                    this.synapseF2toF1.WeightMatrix[this.winner, i] = 0;
                }
            }
        }

        /// <summary>
        /// Compute the output from the ART1 network.  This can be called directly
        /// or used by the BasicNetwork class.  Both input and output should be
        /// bipolar numbers.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="output">The output from the network.</param>
        public void Compute(BiPolarNeuralData input,
                 BiPolarNeuralData output)
        {
            int i;
            bool resonance, exhausted;
            double magnitudeInput1, magnitudeInput2;

            for (i = 0; i < this.layerF2.NeuronCount; i++)
            {
                this.inhibitF2[i] = false;
            }
            resonance = false;
            exhausted = false;
            do
            {
                SetInput(input);
                ComputeF2();
                GetOutput(output);
                if (this.winner != this.noWinner)
                {
                    ComputeF1(input);
                    magnitudeInput1 = Magnitude(input);
                    magnitudeInput2 = Magnitude(this.outputF1);
                    if ((magnitudeInput2 / magnitudeInput1) < this.vigilance)
                    {
                        this.inhibitF2[this.winner] = true;
                    }
                    else
                    {
                        resonance = true;
                    }
                }
                else
                {
                    exhausted = true;
                }
            } while (!(resonance || exhausted));
            if (resonance)
            {
                AdjustWeights();
            }
        }

        /// <summary>
        /// Compute the output from the F1 layer.
        /// </summary>
        /// <param name="input">The input to the F1 layer.</param>
        private void ComputeF1(BiPolarNeuralData input)
        {
            double sum, activation;

            for (int i = 0; i < this.layerF1.NeuronCount; i++)
            {
                sum = this.synapseF1toF2.WeightMatrix[i, this.winner]
                        * (this.outputF2.GetBoolean(this.winner) ? 1 : 0);
                activation = ((input.GetBoolean(i) ? 1 : 0) + this.d1 * sum - this.b1)
                        / (1 + this.a1
                                * ((input.GetBoolean(i) ? 1 : 0) + this.d1 * sum) + this.c1);
                this.outputF1.SetBoolean(i, activation > 0);
            }
        }

        /// <summary>
        /// Compute the output from the F2 layer. 
        /// </summary>
        private void ComputeF2()
        {
            int i, j;
            double Sum, maxOut;

            maxOut = Double.NegativeInfinity;
            this.winner = this.noWinner;
            for (i = 0; i < this.layerF2.NeuronCount; i++)
            {
                if (!this.inhibitF2[i])
                {
                    Sum = 0;
                    for (j = 0; j < this.layerF1.NeuronCount; j++)
                    {
                        Sum += this.synapseF2toF1.WeightMatrix[i, j]
                                * (this.outputF1.GetBoolean(j) ? 1 : 0);
                    }
                    if (Sum > maxOut)
                    {
                        maxOut = Sum;
                        this.winner = i;
                    }
                }
                this.outputF2.SetBoolean(i, false);
            }
            if (this.winner != this.noWinner)
            {
                this.outputF2.SetBoolean(this.winner, true);
            }
        }


        /// <summary>
        /// Copy the output from the network to another object.
        /// </summary>
        /// <param name="output">The target object for the output from the network.</param>
        private void GetOutput(BiPolarNeuralData output)
        {
            for (int i = 0; i < this.layerF2.NeuronCount; i++)
            {
                output.SetBoolean(i, this.outputF2.GetBoolean(i));
            }
        }


        /// <summary>
        /// The winning neuron.
        /// </summary>
        public int Winner
        {
            get
            {
                return this.winner;
            }
        }

        /// <summary>
        /// Does this network have a "winner"?
        /// </summary>
        public bool HasWinner
        {
            get
            {
                return this.winner != this.noWinner;
            }
        }

        /// <summary>
        /// Get the magnitude of the specified input.
        /// </summary>
        /// <param name="input">The input to calculate the magnitude for.</param>
        /// <returns>The magnitude of the specified pattern.</returns>
        public double Magnitude(BiPolarNeuralData input)
        {
            double result;

            result = 0;
            for (int i = 0; i < this.layerF1.NeuronCount; i++)
            {
                result += input.GetBoolean(i) ? 1 : 0;
            }
            return result;
        }

        /// <summary>
        /// Set the input to the neural network.
        /// </summary>
        /// <param name="input">The input.</param>
        private void SetInput(BiPolarNeuralData input)
        {
            double activation;

            for (int i = 0; i < this.layerF1.NeuronCount; i++)
            {
                activation = (input.GetBoolean(i) ? 1 : 0)
                        / (1 + this.a1 * ((input.GetBoolean(i) ? 1 : 0) + this.b1) + this.c1);
                this.outputF1.SetBoolean(i, (activation > 0));
            }
        }


        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public override void Init(BasicNetwork network)
        {
            base.Init(network);

            this.layerF1 = this.Network.GetLayer(ART1Pattern.TAG_F1);
            this.layerF2 = this.Network.GetLayer(ART1Pattern.TAG_F2);
            this.inhibitF2 = new bool[this.layerF2.NeuronCount];
            this.synapseF1toF2 = this.Network.Structure.FindSynapse(this.layerF1, this.layerF2, true);
            this.synapseF2toF1 = this.Network.Structure.FindSynapse(this.layerF2, this.layerF1, true);
            this.outputF1 = new BiPolarNeuralData(this.layerF1.NeuronCount);
            this.outputF2 = new BiPolarNeuralData(this.layerF2.NeuronCount);

            this.a1 = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_A1);
            this.b1 = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_B1);
            this.c1 = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_C1);
            this.d1 = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_D1);
            this.l = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_L);
            this.vigilance = this.Network.GetPropertyDouble(ARTLogic.PROPERTY_VIGILANCE);

            this.noWinner = this.layerF2.NeuronCount;
            Reset();

        }

        /// <summary>
        /// Compute the output for the BasicNetwork class.
        /// </summary>
        /// <param name="input">The input to the network.</param>
        /// <param name="useHolder">The NeuralOutputHolder to use.</param>
        /// <returns>The output from the network.</returns>
        public override INeuralData Compute(INeuralData input, NeuralOutputHolder useHolder)
        {
            if (!(input is BiPolarNeuralData))
            {
                String str = "Input to ART1 logic network must be BiPolarNeuralData.";
#if logging  
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }

            BiPolarNeuralData output = new BiPolarNeuralData(this.layerF1.NeuronCount);
            Compute((BiPolarNeuralData)input, output);
            return output;
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
        /// The L parameter.
        /// </summary>
        public double L
        {
            get
            {
                return this.l;
            }
            set
            {
                this.l = value;
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
