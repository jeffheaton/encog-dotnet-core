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
using System.Linq;
using Encog.Util.Obj;

namespace Encog.ML.EA.Opp
{
    /// <summary>
    ///     This class holds a list of evolutionary operators. Each operator is given a
    ///     probability weight. Based on the number of parents available a random
    ///     selection of an operator can be made based on the probability given each of
    ///     the operators.
    /// </summary>
    public class OperationList : ChooseObject<IEvolutionaryOperator>
    {
        /// <summary>
        ///     Determine the maximum number of offspring that might be produced by any
        ///     of the operators in this list.
        /// </summary>
        /// <returns>The maximum number of offspring.</returns>
        public int MaxOffspring()
        {
            return Contents.Select(holder => holder.obj.OffspringProduced).Concat(new[] {0}).Max();
        }

        /// <summary>
        ///     Determine the maximum number of parents required by any of the operators
        ///     in the list.
        /// </summary>
        /// <returns>The maximum number of parents.</returns>
        public int MaxParents()
        {
            return Contents.Select(holder => holder.obj.ParentsNeeded).Concat(new[] {int.MinValue}).Max();
        }

        /// <summary>
        ///     Pick a operator based on the number of parents available.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="maxParents">The maximum number of parents available.</param>
        /// <returns>The operator that was selected.</returns>
        public IEvolutionaryOperator PickMaxParents(Random rnd,
                                                    int maxParents)
        {
            // determine the total probability of eligible operators
            double total = Contents.Where(holder => holder.obj.ParentsNeeded <= maxParents).Sum(holder => holder.probability);

            // choose an operator
            double r = rnd.NextDouble()*total;
            double current = 0;
            foreach (var holder in Contents)
            {
                if (holder.obj.ParentsNeeded <= maxParents)
                {
                    current += holder.probability;
                    if (r < current)
                    {
                        return holder.obj;
                    }
                }
            }

            return null;
        }
    }
}
