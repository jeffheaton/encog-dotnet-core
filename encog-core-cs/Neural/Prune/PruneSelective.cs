//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.MathUtil.Randomize;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Util;

namespace Encog.Neural.Prune
{
    /// <summary>
    /// Prune a neural network selectively. This class allows you to either add or
    /// remove neurons from layers of a neural network. You can also randomize or
    /// stimulate neurons.
    /// No provision is given for removing an entire layer. Removing a layer requires
    /// a totally new set of weights between the layers before and after the removed
    /// one. This essentially makes any remaining weights useless. At this point you
    /// are better off just creating a new network of the desired dimensions.
    /// </summary>
    ///
    public class PruneSelective
    {
        /// <summary>
        /// The network to prune.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// Construct an object prune the neural network.
        /// </summary>
        ///
        /// <param name="network">The network to prune.</param>
        public PruneSelective(BasicNetwork network)
        {
            _network = network;
        }

        /// <value>The network that is being processed.</value>
        public BasicNetwork Network
        {
            get { return _network; }
        }

        /// <summary>
        /// Change the neuron count for the network. If the count is increased then a
        /// zero-weighted neuron is added, which will not affect the output of the
        /// neural network. If the neuron count is decreased, then the weakest neuron
        /// will be removed.
        /// This method cannot be used to remove a bias neuron.
        /// </summary>
        ///
        /// <param name="layer">The layer to adjust.</param>
        /// <param name="neuronCount">The new neuron count for this layer.</param>
        public void ChangeNeuronCount(int layer, int neuronCount)
        {
            if (neuronCount == 0)
            {
                throw new NeuralNetworkError("Can't decrease to zero neurons.");
            }

            int currentCount = _network.GetLayerNeuronCount(layer);

            // is there anything to do?
            if (neuronCount == currentCount)
            {
                return;
            }

            if (neuronCount > currentCount)
            {
                IncreaseNeuronCount(layer, neuronCount);
            }
            else
            {
                DecreaseNeuronCount(layer, neuronCount);
            }
        }

        /// <summary>
        /// Internal function to decrease the neuron count of a layer.
        /// </summary>
        ///
        /// <param name="layer">The layer to affect.</param>
        /// <param name="neuronCount">The new neuron count.</param>
        private void DecreaseNeuronCount(int layer,
                                         int neuronCount)
        {
            // create an array to hold the least significant neurons, which will be
            // removed

            int lostNeuronCount = _network.GetLayerNeuronCount(layer)
                                  - neuronCount;
            int[] lostNeuron = FindWeakestNeurons(layer, lostNeuronCount);

            // finally, actually prune the neurons that the previous steps
            // determined to remove
            for (int i = 0; i < lostNeuronCount; i++)
            {
                Prune(layer, lostNeuron[i] - i);
            }
        }

        /// <summary>
        /// Determine the significance of the neuron. The higher the return value,
        /// the more significant the neuron is.
        /// </summary>
        ///
        /// <param name="layer">The layer to query.</param>
        /// <param name="neuron">The neuron to query.</param>
        /// <returns>How significant is this neuron.</returns>
        public double DetermineNeuronSignificance(int layer,
                                                  int neuron)
        {
            _network.ValidateNeuron(layer, neuron);

            // calculate the bias significance
            double result = 0;

            // calculate the inbound significance
            if (layer > 0)
            {
                int prevLayer = layer - 1;
                int prevCount = _network
                    .GetLayerTotalNeuronCount(prevLayer);
                for (int i = 0; i < prevCount; i++)
                {
                    result += _network.GetWeight(prevLayer, i, neuron);
                }
            }

            // calculate the outbound significance
            if (layer < _network.LayerCount - 1)
            {
                int nextLayer = layer + 1;
                int nextCount = _network.GetLayerNeuronCount(nextLayer);
                for (int i = 0; i < nextCount; i++)
                {
                    result += _network.GetWeight(layer, neuron, i);
                }
            }

            return Math.Abs(result);
        }

        /// <summary>
        /// Find the weakest neurons on a layer. Considers both weight and bias.
        /// </summary>
        ///
        /// <param name="layer">The layer to search.</param>
        /// <param name="count">The number of neurons to find.</param>
        /// <returns>An array of the indexes of the weakest neurons.</returns>
        private int[] FindWeakestNeurons(int layer, int count)
        {
            // create an array to hold the least significant neurons, which will be
            // returned
            var lostNeuronSignificance = new double[count];
            var lostNeuron = new int[count];

            // init the potential lost neurons to the first ones, we will find
            // better choices if we can
            for (int i = 0; i < count; i++)
            {
                lostNeuron[i] = i;
                lostNeuronSignificance[i] = DetermineNeuronSignificance(layer, i);
            }

            // now loop over the remaining neurons and see if any are better ones to
            // remove
            for (int i = count; i < _network.GetLayerNeuronCount(layer); i++)
            {
                double significance = DetermineNeuronSignificance(layer, i);

                // is this neuron less significant than one already chosen?
                for (int j = 0; j < count; j++)
                {
                    if (lostNeuronSignificance[j] > significance)
                    {
                        lostNeuron[j] = i;
                        lostNeuronSignificance[j] = significance;
                        break;
                    }
                }
            }

            return lostNeuron;
        }


        /// <summary>
        /// Internal function to increase the neuron count. This will add a
        /// zero-weight neuron to this layer.
        /// </summary>
        ///
        /// <param name="targetLayer">The layer to increase.</param>
        /// <param name="neuronCount">The new neuron count.</param>
        private void IncreaseNeuronCount(int targetLayer,
                                         int neuronCount)
        {
            // check for errors
            if (targetLayer > _network.LayerCount)
            {
                throw new NeuralNetworkError("Invalid layer " + targetLayer);
            }

            if (neuronCount <= 0)
            {
                throw new NeuralNetworkError("Invalid neuron count " + neuronCount);
            }

            int oldNeuronCount = _network
                .GetLayerNeuronCount(targetLayer);
            int increaseBy = neuronCount - oldNeuronCount;

            if (increaseBy <= 0)
            {
                throw new NeuralNetworkError(
                    "New neuron count is either a decrease or no change: "
                    + neuronCount);
            }

            // access the flat network
            FlatNetwork flat = _network.Structure.Flat;
            double[] oldWeights = flat.Weights;

            // first find out how many connections there will be after this prune.
            int connections = oldWeights.Length;

            // are connections added from the previous layer?
            if (targetLayer > 0)
            {
                int inBoundConnections = _network
                    .GetLayerTotalNeuronCount(targetLayer - 1);
                connections += inBoundConnections*increaseBy;
            }

            // are there connections added from the next layer?
            if (targetLayer < (_network.LayerCount - 1))
            {
                int outBoundConnections = _network
                    .GetLayerNeuronCount(targetLayer + 1);
                connections += outBoundConnections*increaseBy;
            }

            // increase layer count
            int flatLayer = _network.LayerCount - targetLayer - 1;
            flat.LayerCounts[flatLayer] += increaseBy;
            flat.LayerFeedCounts[flatLayer] += increaseBy;

            // allocate new weights now that we know how big the new weights will be
            var newWeights = new double[connections];

            // construct the new weights
            int weightsIndex = 0;
            int oldWeightsIndex = 0;

            for (int fromLayer = flat.LayerCounts.Length - 2; fromLayer >= 0; fromLayer--)
            {
                int fromNeuronCount = _network
                    .GetLayerTotalNeuronCount(fromLayer);
                int toNeuronCount = _network
                    .GetLayerNeuronCount(fromLayer + 1);
                int toLayer = fromLayer + 1;

                for (int toNeuron = 0; toNeuron < toNeuronCount; toNeuron++)
                {
                    for (int fromNeuron = 0; fromNeuron < fromNeuronCount; fromNeuron++)
                    {
                        if ((toLayer == targetLayer)
                            && (toNeuron >= oldNeuronCount))
                        {
                            newWeights[weightsIndex++] = 0;
                        }
                        else if ((fromLayer == targetLayer)
                                 && (fromNeuron > oldNeuronCount))
                        {
                            newWeights[weightsIndex++] = 0;
                        }
                        else
                        {
                            newWeights[weightsIndex++] = _network.Flat.Weights[oldWeightsIndex++];
                        }
                    }
                }
            }

            // swap in the new weights
            flat.Weights = newWeights;

            // reindex
            ReindexNetwork();
        }

        /// <summary>
        /// Prune one of the neurons from this layer. Remove all entries in this
        /// weight matrix and other layers. This method cannot be used to remove a
        /// bias neuron.
        /// </summary>
        ///
        /// <param name="targetLayer">The neuron to prune. Zero specifies the first neuron.</param>
        /// <param name="neuron">The neuron to prune.</param>
        public void Prune(int targetLayer, int neuron)
        {
            // check for errors
            _network.ValidateNeuron(targetLayer, neuron);

            // don't empty a layer
            if (_network.GetLayerNeuronCount(targetLayer) <= 1)
            {
                throw new NeuralNetworkError(
                    "A layer must have at least a single neuron.  If you want to remove the entire layer you must create a new network.");
            }

            // access the flat network
            FlatNetwork flat = _network.Structure.Flat;
            double[] oldWeights = flat.Weights;

            // first find out how many connections there will be after this prune.
            int connections = oldWeights.Length;

            // are connections removed from the previous layer?
            if (targetLayer > 0)
            {
                int inBoundConnections = _network
                    .GetLayerTotalNeuronCount(targetLayer - 1);
                connections -= inBoundConnections;
            }

            // are there connections removed from the next layer?
            if (targetLayer < (_network.LayerCount - 1))
            {
                int outBoundConnections = _network
                    .GetLayerNeuronCount(targetLayer + 1);
                connections -= outBoundConnections;
            }

            // allocate new weights now that we know how big the new weights will be
            var newWeights = new double[connections];

            // construct the new weights
            int weightsIndex = 0;

            for (int fromLayer = flat.LayerCounts.Length - 2; fromLayer >= 0; fromLayer--)
            {
                int fromNeuronCount = _network
                    .GetLayerTotalNeuronCount(fromLayer);
                int toNeuronCount = _network
                    .GetLayerNeuronCount(fromLayer + 1);
                int toLayer = fromLayer + 1;

                for (int toNeuron = 0; toNeuron < toNeuronCount; toNeuron++)
                {
                    for (int fromNeuron = 0; fromNeuron < fromNeuronCount; fromNeuron++)
                    {
                        bool skip = false;
                        if ((toLayer == targetLayer) && (toNeuron == neuron))
                        {
                            skip = true;
                        }
                        else if ((fromLayer == targetLayer)
                                 && (fromNeuron == neuron))
                        {
                            skip = true;
                        }

                        if (!skip)
                        {
                            newWeights[weightsIndex++] = _network.GetWeight(
                                fromLayer, fromNeuron, toNeuron);
                        }
                    }
                }
            }

            // swap in the new weights
            flat.Weights = newWeights;

            // decrease layer count
            int flatLayer = _network.LayerCount - targetLayer - 1;
            flat.LayerCounts[flatLayer]--;
            flat.LayerFeedCounts[flatLayer]--;

            // reindex
            ReindexNetwork();
        }


        /// <param name="low">The low-end of the range.</param>
        /// <param name="high">The high-end of the range.</param>
        /// <param name="targetLayer">The target layer.</param>
        /// <param name="neuron">The target neuron.</param>
        public void RandomizeNeuron(double low, double high,
                                    int targetLayer, int neuron)
        {
            RandomizeNeuron(targetLayer, neuron, true, low, high, false, 0.0d);
        }

        /// <summary>
        /// Assign random values to the network. The range will be the min/max of
        /// existing neurons.
        /// </summary>
        ///
        /// <param name="targetLayer">The target layer.</param>
        /// <param name="neuron">The target neuron.</param>
        public void RandomizeNeuron(int targetLayer, int neuron)
        {
            FlatNetwork flat = _network.Structure.Flat;
            double low = EngineArray.Min(flat.Weights);
            double high = EngineArray.Max(flat.Weights);
            RandomizeNeuron(targetLayer, neuron, true, low, high, false, 0.0d);
        }

        /// <summary>
        /// Used internally to randomize a neuron. Usually called from
        /// randomizeNeuron or stimulateNeuron.
        /// </summary>
        ///
        /// <param name="targetLayer">The target layer.</param>
        /// <param name="neuron">The target neuron.</param>
        /// <param name="useRange">True if range randomization should be used.</param>
        /// <param name="low">The low-end of the range.</param>
        /// <param name="high">The high-end of the range.</param>
        /// <param name="usePercent">True if percent stimulation should be used.</param>
        /// <param name="percent">The percent to stimulate by.</param>
        private void RandomizeNeuron(int targetLayer, int neuron,
                                     bool useRange, double low, double high,
                                     bool usePercent, double percent)
        {
            IRandomizer d;

            if (useRange)
            {
                d = new RangeRandomizer(low, high);
            }
            else
            {
                d = new Distort(percent);
            }

            // check for errors
            _network.ValidateNeuron(targetLayer, neuron);

            // access the flat network
            FlatNetwork flat = _network.Structure.Flat;

            // allocate new weights now that we know how big the new weights will be
            var newWeights = new double[flat.Weights.Length];

            // construct the new weights
            int weightsIndex = 0;

            for (int fromLayer = flat.LayerCounts.Length - 2; fromLayer >= 0; fromLayer--)
            {
                int fromNeuronCount = _network
                    .GetLayerTotalNeuronCount(fromLayer);
                int toNeuronCount = _network
                    .GetLayerNeuronCount(fromLayer + 1);
                int toLayer = fromLayer + 1;

                for (int toNeuron = 0; toNeuron < toNeuronCount; toNeuron++)
                {
                    for (int fromNeuron = 0; fromNeuron < fromNeuronCount; fromNeuron++)
                    {
                        bool randomize = false;
                        if ((toLayer == targetLayer) && (toNeuron == neuron))
                        {
                            randomize = true;
                        }
                        else if ((fromLayer == targetLayer)
                                 && (fromNeuron == neuron))
                        {
                            randomize = true;
                        }

                        double weight = _network.GetWeight(fromLayer,
                                                          fromNeuron, toNeuron);

                        if (randomize)
                        {
                            weight = d.Randomize(weight);
                        }

                        newWeights[weightsIndex++] = weight;
                    }
                }
            }

            // swap in the new weights
            flat.Weights = newWeights;
        }

        /// <summary>
        /// Creat new index values for the network.
        /// </summary>
        ///
        private void ReindexNetwork()
        {
            FlatNetwork flat = _network.Structure.Flat;

            int neuronCount = 0;
            int weightCount = 0;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                if (i > 0)
                {
                    int from = flat.LayerFeedCounts[i - 1];
                    int to = flat.LayerCounts[i];
                    weightCount += from*to;
                }
                flat.LayerIndex[i] = neuronCount;
                flat.WeightIndex[i] = weightCount;
                neuronCount += flat.LayerCounts[i];
            }

            flat.LayerOutput = new double[neuronCount];
            flat.LayerSums = new double[neuronCount];
            flat.ClearContext();

            flat.InputCount = flat.LayerFeedCounts[flat.LayerCounts.Length - 1];
            flat.OutputCount = flat.LayerFeedCounts[0];
        }

        /// <summary>
        /// Stimulate the specified neuron by the specified percent. This is used to
        /// randomize the weights and bias values for weak neurons.
        /// </summary>
        ///
        /// <param name="percent">The percent to randomize by.</param>
        /// <param name="targetLayer">The layer that the neuron is on.</param>
        /// <param name="neuron">The neuron to randomize.</param>
        public void StimulateNeuron(double percent,
                                    int targetLayer, int neuron)
        {
            RandomizeNeuron(targetLayer, neuron, false, 0, 0, true, percent);
        }

        /// <summary>
        /// Stimulate weaker neurons on a layer. Find the weakest neurons and then
        /// randomize them by the specified percent.
        /// </summary>
        ///
        /// <param name="layer">The layer to stimulate.</param>
        /// <param name="count">The number of weak neurons to stimulate.</param>
        /// <param name="percent">The percent to stimulate by.</param>
        public void StimulateWeakNeurons(int layer, int count,
                                         double percent)
        {
            int[] weak = FindWeakestNeurons(layer, count);

            foreach (int element  in  weak)
            {
                StimulateNeuron(percent, layer, element);
            }
        }
    }
}
