using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Opp.Selection;
using Encog.Util.Obj;

namespace Encog.ML.EA.Opp
{
    /// <summary>
    /// This class holds a list of evolutionary operators. Each operator is given a
    /// probability weight. Based on the number of parents available a random
    /// selection of an operator can be made based on the probability given each of
    /// the operators.
    /// </summary>
    public class OperationList : ChooseObject<IEvolutionaryOperator>
    {
        /// <summary>
        /// Determine the maximum number of offspring that might be produced by any
        /// of the operators in this list.
        /// </summary>
        /// <returns>The maximum number of offspring.</returns>
        public int MaxOffspring()
        {
            int result = 0;
            foreach (ObjectHolder<IEvolutionaryOperator> holder in Contents)
            {
                result = Math.Max(result, holder.obj.OffspringProduced);
            }
            return result;
        }

        /// <summary>
        /// Determine the maximum number of parents required by any of the operators
        /// in the list.
        /// </summary>
        /// <returns>The maximum number of parents.</returns>
        public int MaxParents()
        {
            int result = int.MinValue;
            foreach (ObjectHolder<IEvolutionaryOperator> holder in Contents)
            {
                result = Math.Max(result, holder.obj.ParentsNeeded);
            }
            return result;
        }

        /// <summary>
        /// Pick a operator based on the number of parents available.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="maxParents">The maximum number of parents available.</param>
        /// <returns>The operator that was selected.</returns>
        public IEvolutionaryOperator PickMaxParents(Random rnd,
                int maxParents)
        {

            // determine the total probability of eligible operators
            double total = 0;
            foreach (ObjectHolder<IEvolutionaryOperator> holder in Contents)
            {
                if (holder.obj.ParentsNeeded <= maxParents)
                {
                    total += holder.probability;
                }
            }

            // choose an operator
            double r = rnd.NextDouble() * total;
            double current = 0;
            foreach (ObjectHolder<IEvolutionaryOperator> holder in Contents)
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
