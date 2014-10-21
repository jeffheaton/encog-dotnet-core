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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Opp;
using Encog.ML.EA.Train;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.Tree;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Opp
{
    /// <summary>
    ///     Mutate the constant nodes of an Encog program. This mutation only changes
    ///     values and does not alter the structure.
    /// </summary>
    public class ConstMutation : IEvolutionaryOperator
    {
        /// <summary>
        ///     The frequency that constant nodes are mutated with.
        /// </summary>
        private readonly double _frequency;

        /// <summary>
        ///     The sigma value used to generate gaussian random numbers.
        /// </summary>
        private readonly double _sigma;

        /// <summary>
        ///     Construct a const mutator.
        /// </summary>
        /// <param name="theContext">The program context.</param>
        /// <param name="theFrequency">The frequency of mutation.</param>
        /// <param name="theSigma">The sigma to use for mutation.</param>
        public ConstMutation(EncogProgramContext theContext,
                             double theFrequency, double theSigma)
        {
            _frequency = theFrequency;
            _sigma = theSigma;
        }

        /// <inheritdoc />
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
        }

        /// <inheritdoc />
        public int OffspringProduced
        {
            get { return 1; }
        }

        /// <inheritdoc />
        public int ParentsNeeded
        {
            get { return 1; }
        }

        /// <inheritdoc />
        public void PerformOperation(EncogRandom rnd, IGenome[] parents,
                                     int parentIndex, IGenome[] offspring,
                                     int offspringIndex)
        {
            var program = (EncogProgram) parents[0];
            EncogProgramContext context = program.Context;
            EncogProgram result = context.CloneProgram(program);
            MutateNode(rnd, result.RootNode);
            offspring[0] = result;
        }

        /// <summary>
        ///     Called for each node in the progrmam. If this is a const node, then
        ///     mutate it according to the frequency and sigma specified.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="node">The node to mutate.</param>
        private void MutateNode(EncogRandom rnd, ProgramNode node)
        {
            if (node.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
            {
                if (rnd.NextDouble() < _frequency)
                {
                    ExpressionValue v = node.Data[0];
                    if (v.IsFloat)
                    {
                        double adj = rnd.NextGaussian()*_sigma;
                        node.Data[0] = new ExpressionValue(v.ToFloatValue()
                                                           + adj);
                    }
                }
            }

            foreach (ITreeNode n in node.ChildNodes)
            {
                var childNode = (ProgramNode) n;
                MutateNode(rnd, childNode);
            }
        }
    }
}
