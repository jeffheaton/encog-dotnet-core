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
using Encog.ML.EA.Codec;
using Encog.ML;
using Encog.ML.EA.Genome;
using Encog.Neural.NEAT;
using Encog.Neural.HyperNEAT.Substrate;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Genetic;

namespace Encog.Neural.HyperNEAT
{
    public class HyperNEATCODEC : IGeneticCODEC
    {
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }


        public HyperNEATCODEC()
        {
            MinWeight = 0.2;
            MaxWeight = 5.0;
        }

        /// <inheritdoc/>
        public IMLMethod Decode(IGenome genome)
        {
            NEATPopulation pop = (NEATPopulation)genome.Population;
            Substrate.Substrate substrate = pop.CurrentSubstrate;
            return Decode(pop, substrate, genome);
        }

        /// <inheritdoc/>
        public IMLMethod Decode(NEATPopulation pop, Substrate.Substrate substrate,
                IGenome genome)
        {
            // obtain the CPPN
            NEATCODEC neatCodec = new NEATCODEC();
            NEATNetwork cppn = (NEATNetwork)neatCodec.Decode(genome);

            List<NEATLink> linkList = new List<NEATLink>();

            IActivationFunction[] afs = new IActivationFunction[substrate.NodeCount];

            IActivationFunction af = new ActivationSteepenedSigmoid();
            // all activation functions are the same
            for (int i = 0; i < afs.Length; i++)
            {
                afs[i] = af;
            }

            double c = this.MaxWeight / (1.0 - this.MinWeight);
            BasicMLData input = new BasicMLData(cppn.InputCount);

            // First create all of the non-bias links.
            foreach (SubstrateLink link in substrate.Links)
            {
                SubstrateNode source = link.Source;
                SubstrateNode target = link.Target;

                int index = 0;
                foreach (double d in source.Location)
                {
                    input.Data[index++] = d;
                }
                foreach (double d in target.Location)
                {
                    input.Data[index++] = d;
                }
                IMLData output = cppn.Compute(input);

                double weight = output[0];
                if (Math.Abs(weight) > this.MinWeight)
                {
                    weight = (Math.Abs(weight) - this.MinWeight) * c
                            * Math.Sign(weight);
                    linkList.Add(new NEATLink(source.ID, target.ID,
                            weight));
                }
            }

            // now create biased links
            input.Clear();
            int d2 = substrate.Dimensions;
            IList<SubstrateNode> biasedNodes = substrate.GetBiasedNodes();
            foreach (SubstrateNode target in biasedNodes)
            {
                for (int i = 0; i < d2; i++)
                {
                    input.Data[d2 + i] = target.Location[i];
                }

                IMLData output = cppn.Compute(input);

                double biasWeight = output[1];
                if (Math.Abs(biasWeight) > this.MinWeight)
                {
                    biasWeight = (Math.Abs(biasWeight) - this.MinWeight) * c
                            * Math.Sign(biasWeight);
                    linkList.Add(new NEATLink(0, target.ID, biasWeight));
                }
            }

            // check for invalid neural network
            if (linkList.Count == 0)
            {
                return null;
            }

            linkList.Sort();

            NEATNetwork network = new NEATNetwork(substrate.InputCount,
                    substrate.OutputCount, linkList, afs);

            network.ActivationCycles = substrate.ActivationCycles;
            return network;

        }


        /// <inheritdoc/>
        public IGenome Encode(IMLMethod phenotype)
        {
            throw new GeneticError(
                    "Encoding of a HyperNEAT network is not supported.");
        }

    }
}
