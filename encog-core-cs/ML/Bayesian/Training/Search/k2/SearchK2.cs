using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.MathUtil;
using Encog.ML.Bayesian.Query.Enumeration;

namespace Encog.ML.Bayesian.Training.Search.k2
{
    /// <summary>
    /// Search for optimal Bayes structure with K2.
    /// </summary>
    public class SearchK2 : IBayesSearch
    {
        /// <summary>
        /// The data to use.
        /// </summary>
        private IMLDataSet data;

        /// <summary>
        /// The network to optimize.
        /// </summary>
        private BayesianNetwork network;

        /// <summary>
        /// The trainer being used.
        /// </summary>
        private TrainBayesian train;

        /// <summary>
        /// The last calculated value for p.
        /// </summary>
        private double lastCalculatedP;

        /// <summary>
        /// The node ordering.
        /// </summary>
        private IList<BayesianEvent> nodeOrdering = new List<BayesianEvent>();

        /// <summary>
        /// The current index.
        /// </summary>
        private int index = -1;

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            this.network = theNetwork;
            this.data = theData;
            this.train = theTrainer;
            OrderNodes();
            this.index = -1;
        }

        /// <summary>
        /// Basically the goal here is to get the classification target, if it exists,
        /// to go first. This will greatly enhance K2's effectiveness.
        /// </summary>
        private void OrderNodes()
        {
            this.nodeOrdering.Clear();

            // is there a classification target?
            if (this.network.ClassificationTarget != -1)
            {
                this.nodeOrdering.Add(this.network.ClassificationTargetEvent);
            }


            // now add the others
            foreach (BayesianEvent e in this.network.Events)
            {
                if (!this.nodeOrdering.Contains(e))
                {
                    this.nodeOrdering.Add(e);
                }
            }
        }
        
        /// <summary>
        /// Find the value for z.
        /// </summary>
        /// <param name="e">The event that we are clauclating for.</param>
        /// <param name="n">The value for n.</param>
        /// <param name="old">The old value.</param>
        /// <returns>The new value for z.</returns>
        private BayesianEvent FindZ(BayesianEvent e, int n, double old)
        {
            BayesianEvent result = null;
            double maxChildP = double.NegativeInfinity;
            //System.out.println("Finding parent for: " + event.toString());
            for (int i = 0; i < n; i++)
            {
                BayesianEvent trialParent = this.nodeOrdering[i];
                IList<BayesianEvent> parents = new List<BayesianEvent>();
                parents.CopyTo(e.Parents.ToArray<BayesianEvent>(), 0);
                parents.Add(trialParent);
                //System.out.println("Calculating adding " + trialParent.toString() + " to " + event.toString());
                this.lastCalculatedP = this.CalculateG(network, e, parents);
                //System.out.println("lastP:" + this.lastCalculatedP);
                //System.out.println("old:" + old);
                if (this.lastCalculatedP > old && this.lastCalculatedP > maxChildP)
                {
                    result = trialParent;
                    maxChildP = this.lastCalculatedP;
                    //System.out.println("Current best is: " + result.toString());
                }
            }

            this.lastCalculatedP = maxChildP;
            return result;
        }


        /// <summary>
        /// Calculate the value N, which is the number of cases, from the training data, where the
        /// desiredValue matches the training data.  Only cases where the parents match the specifed
        /// parent instance are considered.
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event we are calculating for. (variable i)</param>
        /// <param name="parents">The parents of the specified event we are considering.</param>
        /// <param name="parentInstance">The parent instance we are looking for.</param>
        /// <param name="desiredValue">The desired value.</param>
        /// <returns>The value N. </returns>
        public int CalculateN(BayesianNetwork network, BayesianEvent e,
                IList<BayesianEvent> parents, int[] parentInstance, int desiredValue)
        {
            int result = 0;
            int eventIndex = network.GetEventIndex(e);

            foreach (IMLDataPair pair in this.data)
            {
                int[] d = this.network.DetermineClasses(pair.Input);

                if (d[eventIndex] == desiredValue)
                {
                    bool reject = false;

                    for (int i = 0; i < parentInstance.Length; i++)
                    {
                        BayesianEvent parentEvent = parents[i];
                        int parentIndex = network.GetEventIndex(parentEvent);
                        if (parentInstance[i] != d[parentIndex])
                        {
                            reject = true;
                            break;
                        }
                    }

                    if (!reject)
                    {
                        result++;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Calculate the value N, which is the number of cases, from the training data, where the
        /// desiredValue matches the training data.  Only cases where the parents match the specifed
        /// parent instance are considered.
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event we are calculating for. (variable i)</param>
        /// <param name="parents">The parents of the specified event we are considering.</param>
        /// <param name="parentInstance">The parent instance we are looking for.</param>
        /// <returns>The value N. </returns>
        public int CalculateN(BayesianNetwork network, BayesianEvent e,
                IList<BayesianEvent> parents, int[] parentInstance)
        {
            int result = 0;

            foreach (IMLDataPair pair in this.data)
            {
                int[] d = this.network.DetermineClasses(pair.Input);

                bool reject = false;

                for (int i = 0; i < parentInstance.Length; i++)
                {
                    BayesianEvent parentEvent = parents[i];
                    int parentIndex = network.GetEventIndex(parentEvent);
                    if (parentInstance[i] != ((int)d[parentIndex]))
                    {
                        reject = true;
                        break;
                    }
                }

                if (!reject)
                {
                    result++;
                }
            }
            return result;
        }

        
        /// <summary>
        /// Calculate G. 
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event to calculate for.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The value for G.</returns>
        public double CalculateG(BayesianNetwork network,
                BayesianEvent e, IList<BayesianEvent> parents)
        {
            double result = 1.0;
            int r = e.Choices.Count;

            int[] args = new int[parents.Count];

            do
            {
                double n = EncogMath.Factorial(r - 1);
                double d = EncogMath.Factorial(CalculateN(network, e,
                        parents, args) + r - 1);
                double p1 = n / d;

                double p2 = 1;
                for (int k = 0; k < e.Choices.Count; k++)
                {
                    p2 *= EncogMath.Factorial(CalculateN(network, e, parents, args, k));
                }

                result *= p1 * p2;
            } while (EnumerationQuery.Roll(parents, args));

            return result;
        }


        /// <inheritdoc/>
        public bool Iteration()
        {

            if (index == -1)
            {
                OrderNodes();
            }
            else
            {
                BayesianEvent e = this.nodeOrdering[index];
                double oldP = this.CalculateG(network, e, e.Parents);

                while (e.Parents.Count < this.train.MaximumParents)
                {
                    BayesianEvent z = FindZ(e, index, oldP);
                    if (z != null)
                    {
                        this.network.CreateDependency(z, e);
                        oldP = this.lastCalculatedP;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            index++;
            return (index < this.data.InputSize);
        }

    }
}
