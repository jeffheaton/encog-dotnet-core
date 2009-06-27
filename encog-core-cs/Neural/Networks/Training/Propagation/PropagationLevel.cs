// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using log4net;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Holds a level worth of information used by each of the propagation methods. A
    /// level is defined as all of the layers that feed a single next layer. In a
    /// pure feedforward neural network there will be only one layer per level.
    /// However, recurrent neural networks will contain multiple layers per level.
    /// </summary>
    public class PropagationLevel
    {

        /// <summary>
        /// The number of neurons on this level.
        /// </summary>
        private int neuronCount;

        /// <summary>
        /// The layers that make up this level.
        /// </summary>
        private IList<ILayer> layers = new List<ILayer>();

        /// <summary>
        /// All outgoing synapses from this level.
        /// </summary>
        private IList<PropagationSynapse> outgoing =
            new List<PropagationSynapse>();

        /// <summary>
        /// The differences between the actual and expected output for this
        /// level.
        /// </summary>
        private double[] deltas;

        /// <summary>
        /// The calculated threshold gradients for this level.
        /// </summary>
        private double[] thresholdGradients;

        /// <summary>
        /// The last iteration's calculated threshold gradients.
        /// </summary>
        private double[] lastThresholdGradients;

        /// <summary>
        /// The deltas to be applied to the threshold values.
        /// </summary>
        private double[] thresholdDeltas;

        /// <summary>
        /// The propagation class that this level belongs to.
        /// </summary>
        private Propagation propagation;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Construct a propagation level.
        /// </summary>
        /// <param name="propagation">The propagation object that created this.</param>
        /// <param name="layer">The initial layer, others can be added later.</param>
        public PropagationLevel(Propagation propagation, ILayer layer)
        {
            this.neuronCount = layer.NeuronCount;
            this.deltas = new double[this.neuronCount];
            this.thresholdGradients = new double[this.neuronCount];
            this.lastThresholdGradients = new double[this.neuronCount];
            this.layers.Add(layer);
            this.propagation = propagation;
            this.thresholdDeltas = new double[this.neuronCount];
        }

        /// <summary>
        /// Construct a propagation level with a list of outgoing synapses.
        /// </summary>
        /// <param name="propagation">The propagation object that created this.</param>
        /// <param name="outgoing">The outgoing synapses.</param>
        public PropagationLevel(Propagation propagation,
                 IList<ISynapse> outgoing)
        {
            int count = 0;

            this.propagation = propagation;
            this.outgoing.Clear();

            foreach (ISynapse synapse in outgoing)
            {
                count += synapse.FromNeuronCount;
                if (!this.layers.Contains(synapse.FromLayer))
                {
                    this.layers.Add(synapse.FromLayer);
                }
                PropagationSynapse propSynapse = new PropagationSynapse(
                       synapse);
                this.outgoing.Add(propSynapse);
            }

            this.neuronCount = count;

            this.deltas = new double[this.neuronCount];
            this.thresholdGradients = new double[this.neuronCount];
            this.lastThresholdGradients = new double[this.neuronCount];
            this.thresholdDeltas = new double[this.neuronCount];
        }

        /// <summary>
        /// Call this method to accumulate the threshold gradients during
        /// a batch.
        /// </summary>
        /// <param name="index">The index of the gradient to modify.</param>
        /// <param name="value">The value to be added to the existing gradients.</param>
        public void AccumulateThresholdGradient(int index,
                 double value)
        {
            this.thresholdGradients[index] += value;
        }

        /// <summary>
        /// Determine the previous synapses from this level.
        /// </summary>
        /// <returns>A list of the previous synapses.</returns>
        public List<ISynapse> DeterminePreviousSynapses()
        {
            List<ISynapse> result = new List<ISynapse>();

            foreach (ILayer layer in this.layers)
            {
                ICollection<ISynapse> synapses = this.propagation.Network
                       .Structure.GetPreviousSynapses(layer);

                // add all teachable synapses
                foreach (ISynapse synapse in synapses)
                {
                    if (synapse.IsTeachable)
                    {
                        result.Add(synapse);
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// Get the actual output from the specified neuron.
        /// </summary>
        /// <param name="index">The neuron needed.</param>
        /// <returns>The actual output from that neuron.</returns>
        public double GetActual(int index)
        {
            int currentIndex = index;

            // is this the output layer, if so then we need to return the output
            // from
            // the entire network.
            if (this.outgoing.Count == 0)
            {
                INeuralData actual = this.propagation.OutputHolder.Output;
                return actual[index];
            }

            // not the output layer, so we need output from one of the previous
            // layers.
            foreach (PropagationSynapse synapse in this.outgoing)
            {
                int count = synapse.Synapse.FromNeuronCount;

                if (currentIndex < count)
                {
                    INeuralData actual = this.propagation.OutputHolder
                            .Result[synapse.Synapse];
                    return actual[currentIndex];
                }

                currentIndex -= count;
            }

            throw new NeuralNetworkError(
                    "Could not find actual value while propagation training.");
        }


        /// <summary>
        /// The differences between the ideal and actual output.
        /// </summary>
        public double[] Deltas
        {
            get
            {
                return this.deltas;
            }
        }

        /// <summary>
        /// Get the specified threshold gradient, from the last iteration
        /// of training.
        /// </summary>
        public double[] LastThresholdGradent
        {
            get
            {
                return this.lastThresholdGradients;
            }
        }

        /// <summary>
        /// All layers associated with this level.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return this.layers;
            }
        }

        /// <summary>
        /// The neuron count for this level.
        /// </summary>
        public int NeuronCount
        {
            get
            {
                return this.neuronCount;
            }
        }

        /// <summary>
        /// The outgoing synapses for this level.
        /// </summary>
        public IList<PropagationSynapse> Outgoing
        {
            get
            {
                return this.outgoing;
            }
        }

        /// <summary>
        /// The threshold deltas.
        /// </summary>
        public double[] ThresholdDelta
        {
            get
            {
                return this.thresholdDeltas;
            }
        }


        /// <summary>
        /// The threshold gradients.
        /// </summary>
        public double[] ThresholdGradients
        {
            get
            {
                return this.thresholdGradients;
            }
        }





        /// <summary>
        /// Convert object to string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[PropagationLevel(");
            result.Append(this.neuronCount);
            result.Append("):");
            foreach (ILayer layer in this.layers)
            {
                result.Append(layer.ToString());
            }
            result.Append("]");
            return result.ToString();
        }
    }

}

