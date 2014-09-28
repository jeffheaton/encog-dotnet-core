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
using System.Linq;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Opp
{
    /// <summary>
    ///     The level holder class is passed down as a tree is mutated. The level holder
    ///     class is initially given the desired output of the program and tracks the
    ///     desired output for each of the nodes. This allows for type-safe crossovers
    ///     and mutations.
    /// </summary>
    public class LevelHolder
    {
        /// <summary>
        ///     Construct the level holder.
        /// </summary>
        /// <param name="theCurrentLevel">The level to construct the holder for.</param>
        public LevelHolder(int theCurrentLevel)
        {
            CurrentLevel = theCurrentLevel;
        }

        /// <summary>
        ///     The current level in the tree.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        ///     The current node, or node found.  This will be the mutation or crossover point.
        /// </summary>
        public ProgramNode NodeFound { get; set; }

        /// <summary>
        ///     The types we are expecting at this level.
        /// </summary>
        public IList<EPLValueType> Types { get; set; }

        /// <summary>
        ///     Determine if the specified child types are compatible with the parent types.
        /// </summary>
        /// <param name="parentTypes">The parent types.</param>
        /// <param name="childTypes">The child types.</param>
        /// <returns>True, if compatible.</returns>
        public static bool CompatibleTypes(IList<EPLValueType> parentTypes,
                                           IList<EPLValueType> childTypes)
        {
            return childTypes.All(parentTypes.Contains);
        }

        /// <summary>
        ///     Decrease the level.
        /// </summary>
        public void DecreaseLevel()
        {
            CurrentLevel--;
        }
    }
}
