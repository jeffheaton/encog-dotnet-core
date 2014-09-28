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
using System.Text;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT neuron gene.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/ Encog's NEAT implementation was drawn from
    /// the following three Journal Articles. For more complete BibTeX sources, see
    /// NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    [Serializable]
    public class NEATNeuronGene : NEATBaseGene
    {
        /// <summary>
        /// The neuron type.
        /// </summary>
        public NEATNeuronType NeuronType { get; set; }

        /// <summary>
        /// The activation function.
        /// </summary>
        public IActivationFunction ActivationFunction { get; set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public NEATNeuronGene()
        {

        }

        /// <summary>
        /// Construct a neuron gene.
        /// </summary>
        /// <param name="type">The neuron type.</param>
        /// <param name="theActivationFunction">The activation function.</param>
        /// <param name="id">The neuron id.</param>
        /// <param name="innovationId">The innovation id.</param>
        public NEATNeuronGene(NEATNeuronType type, IActivationFunction theActivationFunction, long id, long innovationId)
        {
            NeuronType = type;
            InnovationId = innovationId;
            Id = id;
            ActivationFunction = theActivationFunction;
        }

        /// <summary>
        /// Construct this gene by comping another.
        /// </summary>
        /// <param name="other">The other gene to copy.</param>
        public NEATNeuronGene(NEATNeuronGene other)
        {
            Copy(other);
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene.</param>
        public void Copy(NEATNeuronGene gene)
        {
            NEATNeuronGene other = gene;
            Id = other.Id;
            NeuronType = other.NeuronType;
            ActivationFunction = other.ActivationFunction;
            InnovationId = other.InnovationId;
        }



        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[NEATNeuronGene: id=");
            result.Append(Id);
            result.Append(", type=");
            result.Append(NeuronType);
            result.Append("]");
            return result.ToString();
        }
    }
}
