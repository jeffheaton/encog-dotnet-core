using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Opp
{
    /// <summary>
    /// The level holder class is passed down as a tree is mutated. The level holder
    /// class is initially given the desired output of the program and tracks the
    /// desired output for each of the nodes. This allows for type-safe crossovers
    /// and mutations.
    /// </summary>
    public class LevelHolder
    {
        /// <summary>
        /// Determine if the specified child types are compatible with the parent types.
        /// </summary>
        /// <param name="parentTypes">The parent types.</param>
        /// <param name="childTypes">The child types.</param>
        /// <returns>True, if compatible.</returns>
        public static bool CompatibleTypes(IList<EPLValueType> parentTypes,
                IList<EPLValueType> childTypes)
        {
            foreach (EPLValueType childType in childTypes)
            {
                if (!parentTypes.Contains(childType))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The current level in the tree.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// The current node, or node found.  This will be the mutation or crossover point.
        /// </summary>
        public ProgramNode NodeFound { get; set; }

        /// <summary>
        /// The types we are expecting at this level.
        /// </summary>
        public IList<EPLValueType> Types { get; set; }
        
        /// <summary>
        /// Construct the level holder. 
        /// </summary>
        /// <param name="theCurrentLevel">The level to construct the holder for.</param>
        public LevelHolder(int theCurrentLevel)
        {
            CurrentLevel = theCurrentLevel;
        }

        /// <summary>
        /// Decrease the level.
        /// </summary>
        public void DecreaseLevel()
        {
            CurrentLevel--;
        }

    }
}
