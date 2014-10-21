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
using System.Collections.Generic;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Opp;
using Encog.ML.EA.Train;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Generator;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Opp
{
    /// <summary>
    ///     Perform a type-safe subtree mutation. The mutation point is chosen randomly,
    ///     but the new tree will be generated with compatible types to the parent.
    /// </summary>
    public class SubtreeMutation : IEvolutionaryOperator
    {
        /// <summary>
        ///     The maximum depth.
        /// </summary>
        private readonly int _maxDepth;

        /// <summary>
        ///     Construct the subtree mutation object.
        /// </summary>
        /// <param name="theContext">The program context.</param>
        /// <param name="theMaxDepth">The maximum depth.</param>
        public SubtreeMutation(EncogProgramContext theContext,
                               int theMaxDepth)
        {
            Generator = new PrgGrowGenerator(theContext, theMaxDepth);
            _maxDepth = theMaxDepth;
        }

        /// <summary>
        ///     A random generator.
        /// </summary>
        public IPrgGenerator Generator { get; set; }


        /// <inheritdoc />
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            // TODO Auto-generated method stub
        }

        /// <summary>
        ///     Returns the number of offspring produced. In this case, one.
        /// </summary>
        public int OffspringProduced
        {
            get { return 1; }
        }

        /// <summary>
        ///     Returns the number of parents needed. In this case, one.
        /// </summary>
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

            IList<EPLValueType> types = new List<EPLValueType>();
            types.Add(context.Result.VariableType);
            var globalIndex = new int[1];
            globalIndex[0] = rnd.Next(result.RootNode.Count);
            FindNode(rnd, result, result.RootNode, types, globalIndex);

            offspring[0] = result;
        }

        /// <summary>
        ///     This method is called reflexivly as we iterate downward. Once we reach
        ///     the desired point (when current level drops to zero), the operation is
        ///     performed.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="result">The parent node.</param>
        /// <param name="parentNode"></param>
        /// <param name="types">The desired node</param>
        /// <param name="globalIndex">The level holder.</param>
        private void FindNode(EncogRandom rnd, EncogProgram result,
                              ProgramNode parentNode, IList<EPLValueType> types,
                              int[] globalIndex)
        {
            if (globalIndex[0] == 0)
            {
                globalIndex[0]--;

                ProgramNode newInsert = Generator.CreateNode(rnd,
                                                             result, _maxDepth, types);
                result.ReplaceNode(parentNode, newInsert);
            }
            else
            {
                globalIndex[0]--;
                for (int i = 0; i < parentNode.Template.ChildNodeCount; i++)
                {
                    ProgramNode childNode = parentNode.GetChildNode(i);
                    IList<EPLValueType> childTypes = parentNode.Template.Params[i].DetermineArgumentTypes(types);
                    FindNode(rnd, result, childNode, childTypes, globalIndex);
                }
            }
        }
    }
}
