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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.HyperNEAT.Substrate
{
    /// <summary>
    /// The substrate defines the structure of the produced HyperNEAT network.
    /// 
    /// A substrate is made up of nodes and links. A node has a location that is an
    /// n-dimensional coordinate. Nodes are grouped into input and output clusters.
    /// There can also be hidden neurons between these two.
    /// 
    /// A HyperNEAT network works by training a CPPN that produces the actual
    /// resulting NEAT network. The size of the substrate can then be adjusted to
    /// create larger networks than what the HyperNEAT network was originally trained
    /// with.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/
    /// Encog's NEAT implementation was drawn from the following three Journal
    /// Articles. For more complete BibTeX sources, see NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    [Serializable]
    public class Substrate
    {
        /// <summary>
        /// The dimensions of the network.
        /// </summary>
        private int dimensions;

        /// <summary>
        /// The input nodes.
        /// </summary>
        private IList<SubstrateNode> inputNodes = new List<SubstrateNode>();

        /// <summary>
        /// The output nodes.
        /// </summary>
        private IList<SubstrateNode> outputNodes = new List<SubstrateNode>();

        /// <summary>
        /// The hidden nodes.
        /// </summary>
        private IList<SubstrateNode> hiddenNodes = new List<SubstrateNode>();

        /// <summary>
        /// The links between nodes.
        /// </summary>
        private IList<SubstrateLink> links = new List<SubstrateLink>();

        /// <summary>
        /// The current neuron id.
        /// </summary>
        private int currentNeuronNumber;

        /// <summary>
        /// The number of activation cycles.
        /// </summary>
        public int ActivationCycles { get; set; }

        /// <summary>
        /// Construct a substrate with the specified number of dimensions in the
        /// input/output layers.
        /// </summary>
        /// <param name="theDimensions">The dimensions</param>
        public Substrate(int theDimensions)
        {
            this.dimensions = theDimensions;
            this.currentNeuronNumber = 1;
            this.ActivationCycles = 1;
        }

        /// <summary>
        /// The hidden nodes.
        /// </summary>
        public IList<SubstrateNode> HiddenNodes
        {
            get
            {
                return hiddenNodes;
            }
        }

        /// <summary>
        /// The dimensions.
        /// </summary>
        public int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        /// <summary>
        /// The input nodes.
        /// </summary>
        public IList<SubstrateNode> InputNodes
        {
            get
            {
                return inputNodes;
            }
        }

        /// <summary>
        /// The output nodes.
        /// </summary>
        public IList<SubstrateNode> OutputNodes
        {
            get
            {
                return outputNodes;
            }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get
            {
                return this.inputNodes.Count;
            }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public int OutputCount
        {
            get
            {
                return this.outputNodes.Count;
            }
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        /// <returns>The node.</returns>
        public SubstrateNode CreateNode()
        {
            SubstrateNode result = new SubstrateNode(this.currentNeuronNumber++,
                    this.dimensions);
            return result;
        }

        /// <summary>
        /// Create input node.
        /// </summary>
        /// <returns>An input node.</returns>
        public SubstrateNode CreateInputNode()
        {
            SubstrateNode result = CreateNode();
            this.inputNodes.Add(result);
            return result;
        }

        /// <summary>
        /// Create output node.
        /// </summary>
        /// <returns>An output node.</returns>
        public SubstrateNode CreateOutputNode()
        {
            SubstrateNode result = CreateNode();
            this.outputNodes.Add(result);
            return result;
        }

        /// <summary>
        /// Create hidden node.
        /// </summary>
        /// <returns>A hidden node.</returns>
        public SubstrateNode CreateHiddenNode()
        {
            SubstrateNode result = CreateNode();
            this.hiddenNodes.Add(result);
            return result;
        }

        /// <summary>
        /// Create a link.
        /// </summary>
        /// <param name="inputNode">The from node.</param>
        /// <param name="outputNode">The to node.</param>
        public void CreateLink(SubstrateNode inputNode, SubstrateNode outputNode)
        {
            SubstrateLink link = new SubstrateLink(inputNode, outputNode);
            this.links.Add(link);
        }

        /// <summary>
        /// The links.
        /// </summary>
        public IList<SubstrateLink> Links
        {
            get
            {
                return links;
            }
        }

        /// <summary>
        /// The link count.
        /// </summary>
        public int LinkCount
        {
            get
            {
                return links.Count;
            }
        }

        /// <summary>
        /// The total count of nodes.
        /// </summary>
        public int NodeCount
        {
            get
            {
                return 1 + this.inputNodes.Count + this.outputNodes.Count
                        + this.hiddenNodes.Count;
            }
        }

        /// <summary>
        /// Get the biased nodes.
        /// </summary>
        /// <returns>A list of all nodes that are connected to the bias neuron. This
        /// is typically all non-input neurons.</returns>
        public IList<SubstrateNode> GetBiasedNodes()
        {
            return this.hiddenNodes.Union(this.outputNodes).ToList();
        }
    }
}
