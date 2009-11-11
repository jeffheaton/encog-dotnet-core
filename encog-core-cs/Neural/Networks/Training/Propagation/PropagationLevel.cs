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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Holds a level worth of information used by each of the propagation methods. A
    /// level is defined as all of the layers that feed a single next layer. In a
    /// pure feedforward neural network there will be only one layer per level.
    /// However, recurrent neural networks will contain multiple layers per level.
    /// </summary>
    public struct PropagationLevel
    {

        /// <summary>
        /// The number of neurons on this level.
        /// </summary>
        public int NeuronCount;

        /// <summary>
        /// The layers that make up this level.
        /// </summary>
        public IList<ILayer> Layers;

        /// <summary>
        /// All outgoing synapses from this level.
        /// </summary>
        public IList<PropagationSynapse> Outgoing;

        /// <summary>
        /// The differences between the actual and expected output for this
        /// level.
        /// </summary>
        public double[] Deltas;

        /// <summary>
        /// The calculated threshold gradients for this level.
        /// </summary>
        public double[] ThresholdGradents;

        /// <summary>
        /// The last iteration's calculated threshold gradients.
        /// </summary>
        public double[] LastThresholdGradents;

        /// <summary>
        /// The deltas to be applied to the threshold values.
        /// </summary>
        public double[] ThresholdDeltas;

        /// <summary>
        /// The propagation class that this level belongs to.
        /// </summary>
        public PropagationUtil Propagation;

        /// <summary>
        /// Construct a propagation level.
        /// </summary>
        /// <param name="propagation">The propagation object that created this.</param>
        /// <param name="layer">The initial layer, others can be added later.</param>
        public PropagationLevel(PropagationUtil propagation, ILayer layer)
        {
            this.NeuronCount = layer.NeuronCount;
            this.Deltas = new double[this.NeuronCount];
            this.ThresholdGradents = new double[this.NeuronCount];
            this.LastThresholdGradents = new double[this.NeuronCount];
            this.Layers = new List<ILayer>();
            this.Layers.Add(layer);
            this.Propagation = propagation;
            this.ThresholdDeltas = new double[this.NeuronCount];
            this.Outgoing = new List<PropagationSynapse>();
        }

        /// <summary>
        /// Construct a propagation level with a list of outgoing synapses.
        /// </summary>
        /// <param name="propagation">The propagation object that created this.</param>
        /// <param name="outgoing">The outgoing synapses.</param>
        public PropagationLevel(PropagationUtil propagation,
                 IList<ISynapse> outgoing)
        {
            this.Outgoing = new List<PropagationSynapse>();
            this.Layers = new List<ILayer>();
            int count = 0;

            this.Propagation = propagation;
            this.Outgoing.Clear();

            foreach (ISynapse synapse in outgoing)
            {
                count += synapse.FromNeuronCount;
                if (!this.Layers.Contains(synapse.FromLayer))
                {
                    this.Layers.Add(synapse.FromLayer);
                }
                PropagationSynapse propSynapse = new PropagationSynapse(
                       synapse);
                this.Outgoing.Add(propSynapse);
            }

            this.NeuronCount = count;

            this.Deltas = new double[this.NeuronCount];
            this.ThresholdGradents = new double[this.NeuronCount];
            this.LastThresholdGradents = new double[this.NeuronCount];
            this.ThresholdDeltas = new double[this.NeuronCount];
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
            this.ThresholdGradents[index] += value;
        }

        /// <summary>
        /// Determine the previous synapses from this level.
        /// </summary>
        /// <returns>A list of the previous synapses.</returns>
        public List<ISynapse> DeterminePreviousSynapses()
        {
            List<ISynapse> result = new List<ISynapse>();

            foreach (ILayer layer in this.Layers)
            {
                ICollection<ISynapse> synapses = this.Propagation.Network
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
            if (this.Outgoing.Count == 0)
            {
                INeuralData actual = this.Propagation.OutputHolder.Output;
                return actual[index];
            }

            // not the output layer, so we need output from one of the previous
            // layers.
            foreach (PropagationSynapse synapse in this.Outgoing)
            {
                int count = synapse.Synapse.FromNeuronCount;

                if (currentIndex < count)
                {
                    INeuralData actual = this.Propagation.OutputHolder
                            .Result[synapse.Synapse];
                    return actual[currentIndex];
                }

                currentIndex -= count;
            }

            throw new NeuralNetworkError(
                    "Could not find actual value while propagation training.");
        }



        /// <summary>
        /// Convert object to string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[PropagationLevel(");
            result.Append(this.NeuronCount);
            result.Append("):");
            foreach (ILayer layer in this.Layers)
            {
                result.Append(layer.ToString());
            }
            result.Append("]");
            return result.ToString();
        }
    }

}

