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

#if logging
using log4net;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Activation;
using Encog.Neural.Data;
using System.Runtime.Serialization;
using Encog.Persist;
using Encog.Persist.Persistors;
using Encog.Neural.Networks.Synapse.NEAT;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// Basic functionality that most of the neural layers require. The basic layer
    /// is often used by itself to implement forward or recurrent layers. Other layer
    /// types are based on the basic layer as well.
    /// 
    /// The following summarizes how basic layers calculate the output for a neural
    /// network.
    /// 
    /// Example of a simple XOR network.
    /// 
    /// Input: BasicLayer: 2 Neurons, null biasWeights, null biasActivation
    /// 
    /// Hidden: BasicLayer: 2 Neurons, 2 biasWeights, 1 biasActivation
    /// 
    /// Output: BasicLayer: 1 Neuron, 1 biasWeights, 1 biasActivation
    /// 
    /// Input1Output and Input2Output are both provided.
    /// 
    /// Synapse 1: Input to Hidden Hidden1Activation = (Input1Output *
    /// Input1->Hidden1Weight) + (Input2Output * Input2->Hidden1Weight) +
    /// (HiddenBiasActivation * Hidden1BiasWeight)
    /// 
    /// Hidden1Output = calculate(Hidden1Activation, HiddenActivationFunction)
    /// 
    /// Hidden2Activation = (Input1Output * Input1->Hidden2Weight) + (Input2Output *
    /// Input2->Hidden2Weight) + (HiddenBiasActivation * Hidden2BiasWeight)
    /// 
    /// Hidden2Output = calculate(Hidden2Activation, HiddenActivationFunction)
    /// 
    /// Synapse 2: Hidden to Output
    /// 
    /// Output1Activation = (Hidden1Output * Hidden1->Output1Weight) + (Hidden2Output *
    /// Hidden2->Output1Weight) + (OutputBiasActivation * Output1BiasWeight)
    /// 
    /// Output1Output = calculate(Output1Activation, OutputActivationFunction) 
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicLayer : ILayer
    {
        /// <summary>
        /// The outbound synapse connections from this layer.
        /// </summary>
        private IList<ISynapse> next = new List<ISynapse>();

        /// <summary>
        /// The x-coordinate of this layer, used for GUI rendering.
        /// </summary>
        private int x;

        /// <summary>
        /// The y-coordinate of this layer, used for GUI rendering.
        /// </summary>
        private int y;

        /// <summary>
        /// The id of this layer.
        /// </summary>
        private int id;

        /// <summary>
        /// The bias activation for this layer, normally 1.
        /// </summary>
        private double biasActivation;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicLayer));
#endif

        /// <summary>
        /// Which activation function to use for this layer.
        /// </summary>
        private IActivationFunction activationFunction;

        /// <summary>
        /// The description for this object.
        /// </summary>
        private String description;

        /// <summary>
        /// The name for this object.
        /// </summary>
        private String name;

        /// <summary>
        /// How many neurons does this layer hold.
        /// </summary>
        private int neuronCount;

        /// <summary>
        /// The bias weights for this layer.
        /// </summary>
        private double[] biasWeights;

        /// <summary>
        /// Default constructor, mainly so the workbench can easily create a default
        /// layer.
        /// </summary>
        public BasicLayer()
            : this(1)
        {

        }

        /// <summary>
        /// Construct this layer with a non-default activation function.
        /// </summary>
        /// <param name="activationFunction">The activation function to use.</param>
        /// <param name="hasBias">How many neurons in this layer.</param>
        /// <param name="neuronCount">True if this layer has bias weight values.</param>
        public BasicLayer(IActivationFunction activationFunction,
                 bool hasBias, int neuronCount)
        {
            this.neuronCount = neuronCount;
            this.ActivationFunction = activationFunction;
            this.biasActivation = 1;
            if (hasBias)
            {
                this.biasWeights = new double[neuronCount];
            }
        }

        /// <summary>
        /// Construct this layer with a TANH activation function.
        /// </summary>
        /// <param name="neuronCount">How many neurons in this layer.</param>
        public BasicLayer(int neuronCount)
            : this(new ActivationTANH(), true, neuronCount)
        {

        }

        /// <summary>
        /// Add a layer as the next layer. The layer will be added with a weighted
        /// synapse.
        /// </summary>
        /// <param name="next">The next layer.</param>
        public void AddNext(ILayer next)
        {
            AddNext(next, SynapseType.Weighted);
        }

        /// <summary>
        /// Add a "next" layer.
        /// </summary>
        /// <param name="next">The next layer to add.</param>
        /// <param name="type">The synapse type to use for this layer.</param>
        public void AddNext(ILayer next, SynapseType type)
        {
            ISynapse synapse = null;

            switch (type)
            {
                case SynapseType.OneToOne:
                    synapse = new OneToOneSynapse(this, next);
                    break;
                case SynapseType.Weighted:
                    synapse = new WeightedSynapse(this, next);
                    break;
                case SynapseType.Weightless:
                    synapse = new WeightlessSynapse(this, next);
                    break;
                case SynapseType.Direct:
                    synapse = new DirectSynapse(this, next);
                    break;
                case SynapseType.NEAT:
                    synapse = new NEATSynapse(this, next);
                    break;
                default:
                    throw new NeuralNetworkError("Unknown synapse type");
            }

            if (synapse == null)
            {
                String str = "Unknown synapse type.";
#if logging
                if (BasicLayer.logger.IsErrorEnabled)
                {
                    BasicLayer.logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }
            else
            {
                this.next.Add(synapse);
            }
        }

        /// <summary>
        /// Add a synapse to the list of outbound synapses.  Usually you should 
        /// simply call the addLayer method to add to the outbound list.
        /// </summary>
        /// <param name="synapse">The synapse to add.</param>
        public void AddSynapse(ISynapse synapse)
        {
            this.next.Add(synapse);
        }


        /// <summary>
        /// Clone this object. 
        /// </summary>
        /// <returns>A cloned version of this object.</returns>
        public Object Clone()
        {
            BasicLayer result = new BasicLayer(
                   (IActivationFunction)this.activationFunction.Clone(),
                   this.HasBias, this.NeuronCount);
            return result;

        }

        /// <summary>
        /// Compute the outputs for this layer given the input pattern. The output is
        /// also stored in the fire instance variable.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <returns>The output from this layer.</returns>
        public virtual INeuralData Compute(INeuralData pattern)
        {

            INeuralData result = (INeuralData)pattern.Clone();

            if (this.HasBias)
            {
                // apply the bias values
                for (int i = 0; i < this.biasWeights.Length; i++)
                {
                    result[i] = result[i] + this.biasWeights[i];
                }
            }

            // apply the activation function
            this.ActivationFunction.ActivationFunction(result.Data);

            return result;
        }

        /// <summary>
        /// Create a persistor for this layer.
        /// </summary>
        /// <returns>The new persistor.</returns>
        public virtual IPersistor CreatePersistor()
        {
            return new BasicLayerPersistor();
        }

        /// <summary>
        /// The activation function for this layer.
        /// </summary>
        public virtual IActivationFunction ActivationFunction
        {
            get
            {
                return this.activationFunction;
            }
            set
            {
                this.activationFunction = value;
            }
        }

        /// <summary>
        /// The description.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Get or set the neuron count for this layer. This just sets it, it does not make any
        /// adjustments to the class.  To automatically change the neuron count
        /// refer to the pruning classes.
        /// </summary>
        public int NeuronCount
        {
            get
            {
                return this.neuronCount;
            }
            set
            {
                this.neuronCount = value;
            }
        }

        /**
         * @return The outbound synapse connections.
         */
        public IList<ISynapse> Next
        {
            get
            {
                return this.next;
            }
        }

        /// <summary>
        /// The list of layers that the outbound synapses connect to.
        /// </summary>
        public ICollection<ILayer> NextLayers
        {
            get
            {
                ICollection<ILayer> result = new List<ILayer>();
                foreach (ISynapse synapse in this.next)
                {
                    if (!result.Contains(synapse.ToLayer))
                        result.Add(synapse.ToLayer);
                }
                return result;
            }
        }

        /// <summary>
        /// Set or gets the bias weight array.  This does not modify any of the other values
        /// in the network, it just sets the bias array.  If you want to 
        /// change the structure of the neural network you should use the pruning 
        /// classes.
        /// </summary>
        public double[] BiasWeights
        {
            get
            {
                return this.biasWeights;
            }
            set
            {
                this.biasWeights = value;
            }
        }

        /// <summary>
        /// The x-coordinate. Used when the layer is displayed in a GUI.
        /// </summary>
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// The y-coordinate. Used when the layer is displayed in a GUI.
        /// </summary>
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// True if bias weight values are present.
        /// </summary>
        public bool HasBias
        {
            get
            {
                return this.biasWeights != null;
            }
        }

        /// <summary>
        /// Determine if this layer is connected to another layer.
        /// </summary>
        /// <param name="layer">A layer to check and see if this layer is connected to.</param>
        /// <returns>True if the two layers are connected.</returns>
        public bool IsConnectedTo(ILayer layer)
        {
            foreach (ISynapse synapse in this.next)
            {
                if (synapse.ToLayer == layer)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine if this layer is self-connected.
        /// </summary>
        /// <returns>True if this layer is connected to intself.</returns>
        public bool IsSelfConnected()
        {
            foreach (ISynapse synapse in this.next)
            {
                if (synapse.IsSelfConnected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Process the input pattern.  For the basic layer, nothing is done.
        /// This is how the context layer gets a chance to record the input. 
        /// Other similar functions, where access is needed to the input.
        /// </summary>
        /// <param name="pattern">The input to this layer.</param>
        public virtual void Process(INeuralData pattern)
        {
        }

        /// <summary>
        /// Get the output from this layer when called in a recurrent manor.
        /// For the BaiscLayer, this is not implemented.
        /// </summary>
        /// <returns>The output when called in a recurrent way.</returns>
        public virtual INeuralData Recur()
        {
            return null;
        }

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": neuronCount=");
            result.Append(this.neuronCount);
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// The ID for this layer.
        /// </summary>
        public int ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Compare this layer to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns> The value 0 if the argument layer is equal to this layer; a
        /// value less than 0 if this layer is less
        /// than the argument; and a value greater than 0 if this
        /// layer is greater than the layer argument.</returns>
        public int CompareTo(ILayer other)
        {
            if (other.ID == this.ID)
            {
                return 0;
            }
            else if (other.ID > ID)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// The bias activation, usually 1.  See Layer for more info.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                return this.biasActivation;
            }
            set
            {
                this.biasActivation = value;
            }
        }

    }
}
