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
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Opp
{
    /// <summary>
    ///     Perform a type-safe subtree crossover. The crossover points will be chosen
    ///     randomly but must be type-safe. The first parent will be cloned to produce
    ///     the child. The tree formed from the crossover point of the second child will
    ///     be copied and grafted into the parent's clone and its crossover point.
    /// </summary>
    public class SubtreeCrossover : IEvolutionaryOperator
    {
        /// <inheritdoc />
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            
        }

        /// <summary>
        ///     Returns the number of offspring produced. In this case, one.
        /// </summary>
        public int OffspringProduced
        {
            get { return 1; }
        }

        /// <summary>
        ///     Returns the number of parents needed. In this case, two.
        /// </summary>
        public int ParentsNeeded
        {
            get { return 2; }
        }

        /// <inheritdoc />
        public void PerformOperation(EncogRandom rnd, IGenome[] parents,
                                     int parentIndex, IGenome[] offspring,
                                     int offspringIndex)
        {
            var parent1 = (EncogProgram) parents[0];
            var parent2 = (EncogProgram) parents[1];
            offspring[0] = null;

            EncogProgramContext context = parent1.Context;
            int size1 = parent1.RootNode.Count;
            int size2 = parent2.RootNode.Count;

            bool done = false;
            int tries = 100;

            while (!done)
            {
                int p1Index = rnd.Next(size1);
                int p2Index = rnd.Next(size2);

                var holder1 = new LevelHolder(p1Index);
                var holder2 = new LevelHolder(p2Index);

                IList<EPLValueType> types = new List<EPLValueType>();
                types.Add(context.Result.VariableType);

                FindNode(rnd, parent1.RootNode, types, holder1);
                FindNode(rnd, parent2.RootNode, types, holder2);

                if (LevelHolder.CompatibleTypes(holder1.Types,
                                                holder2.Types))
                {
                    EncogProgram result = context.CloneProgram(parent1);
                    ProgramNode resultNode = parent1.FindNode(p1Index);
                    ProgramNode p2Node = parent2.FindNode(p2Index);
                    ProgramNode newInsert = context.CloneBranch(result,
                                                                p2Node);
                    result.ReplaceNode(resultNode, newInsert);
                    offspring[0] = result;
                    done = true;
                }
                else
                {
                    tries--;
                    if (tries < 0)
                    {
                        done = true;
                    }
                }
            }
        }

        /// <summary>
        ///     This method is called reflexivly as we iterate downward. Once we reach
        ///     the desired point (when current level drops to zero), the operation is
        ///     performed.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="types">The desired node.</param>
        /// <param name="holder">The level holder.</param>
        private void FindNode(EncogRandom rnd, ProgramNode parentNode,
                              IList<EPLValueType> types, LevelHolder holder)
        {
            if (holder.CurrentLevel == 0)
            {
                holder.DecreaseLevel();
                holder.Types = types;
                holder.NodeFound = parentNode;
            }
            else
            {
                holder.DecreaseLevel();
                for (int i = 0; i < parentNode.Template.ChildNodeCount; i++)
                {
                    ProgramNode childNode = parentNode.GetChildNode(i);
                    IList<EPLValueType> childTypes = parentNode.Template
                                                               .Params[i].DetermineArgumentTypes(types);
                    FindNode(rnd, childNode, childTypes, holder);
                }
            }
        }
    }
}
