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
using System.Text;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation list.
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
    /// </summary>
    [Serializable]
    public class NEATInnovationList
    {
        /// <summary>
        /// The population.
        /// </summary>
        public NEATPopulation Population { get; set; }

        /// <summary>
        /// The list of innovations.
        /// </summary>
        private readonly IDictionary<string, NEATInnovation> _list = new Dictionary<String, NEATInnovation>();

        /// <summary>
        /// The default constructor, used mainly for persistance.
        /// </summary>
        public NEATInnovationList()
        {

        }

        /// <summary>
        /// Produce an innovation key for a neuron.
        /// </summary>
        /// <param name="id">The neuron id.</param>
        /// <returns>The newly created key.</returns>
        public static String ProduceKeyNeuron(long id)
        {
            var result = new StringBuilder();
            result.Append("n:");
            result.Append(id);
            return result.ToString();
        }

        /// <summary>
        /// Produce a key for a split neuron.
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        /// <returns></returns>
        public static String ProduceKeyNeuronSplit(long fromId, long toId)
        {
            var result = new StringBuilder();
            result.Append("ns:");
            result.Append(fromId);
            result.Append(":");
            result.Append(toId);
            return result.ToString();
        }

        /// <summary>
        /// Produce a key for a link.
        /// </summary>
        /// <param name="fromId">The from id.</param>
        /// <param name="toId">The to id.</param>
        /// <returns>The key for the link.</returns>
        public static String ProduceKeyLink(long fromId, long toId)
        {
            var result = new StringBuilder();
            result.Append("l:");
            result.Append(fromId);
            result.Append(":");
            result.Append(toId);
            return result.ToString();
        }

        /// <summary>
        /// Construct an innovation list, that includes the initial innovations.
        /// </summary>
        /// <param name="population">The population to base this innovation list on.</param>
        public NEATInnovationList(NEATPopulation population)
        {

            Population = population;

            FindInnovation(Population.AssignGeneId()); // bias

            // input neurons
            for (int i = 0; i < Population.InputCount; i++)
            {
                FindInnovation(Population.AssignGeneId());
            }

            // output neurons
            for (int i = 0; i < Population.OutputCount; i++)
            {
                FindInnovation(Population.AssignGeneId());
            }

            // connections
            for (var fromId = 0; fromId < Population.InputCount + 1; fromId++)
            {
                for (var toId = 0; toId < Population.OutputCount; toId++)
                {
                    FindInnovation(fromId, toId);
                }
            }



        }

        /// <summary>
        /// Find an innovation for a hidden neuron that split a existing link. This
        /// is the means by which hidden neurons are introduced in NEAT.
        /// </summary>
        /// <param name="fromId">The source neuron ID in the link.</param>
        /// <param name="toId">The target neuron ID in the link.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovationSplit(long fromId, long toId)
        {
            String key = ProduceKeyNeuronSplit(fromId, toId);

            lock (_list)
            {
                if (_list.ContainsKey(key))
                {
                    return _list[key];
                }
                long neuronId = Population.AssignGeneId();
                var innovation = new NEATInnovation
                    {
                        InnovationId = Population.AssignInnovationId(),
                        NeuronId = neuronId
                    };
                _list[key] = innovation;

                // create other sides of split, if needed
                FindInnovation(fromId, neuronId);
                FindInnovation(neuronId, toId);
                return innovation;
            }
        }

        /// <summary>
        /// Find an innovation for a single neuron. Single neurons were created
        /// without producing a split. This means, the only single neurons are the
        /// input, bias and output neurons.
        /// </summary>
        /// <param name="neuronId">The neuron ID to find.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovation(long neuronId)
        {
            String key = ProduceKeyNeuron(neuronId);

            lock (_list)
            {
                if (_list.ContainsKey(key))
                {
                    return _list[key];
                }
                var innovation = new NEATInnovation
                    {
                        InnovationId = Population.AssignInnovationId(),
                        NeuronId = neuronId
                    };
                _list[key] = innovation;
                return innovation;
            }
        }

        /// <summary>
        /// Find an innovation for a new link added between two existing neurons.
        /// </summary>
        /// <param name="fromId">The source neuron ID in the link.</param>
        /// <param name="toId">The target neuron ID in the link.</param>
        /// <returns>The newly created innovation, or the one that matched the search.</returns>
        public NEATInnovation FindInnovation(long fromId, long toId)
        {
            String key = ProduceKeyLink(fromId, toId);

            lock (_list)
            {
                if (_list.ContainsKey(key))
                {
                    return _list[key];
                }
                else
                {
                    NEATInnovation innovation = new NEATInnovation();
                    innovation.InnovationId = Population.AssignInnovationId();
                    innovation.NeuronId = -1;
                    _list[key] = innovation;
                    return innovation;
                }
            }
        }

        /// <summary>
        /// A list of innovations.
        /// </summary>
        public IDictionary<String, NEATInnovation> Innovations
        {
            get
            {
                return _list;
            }
        }
    }
}
