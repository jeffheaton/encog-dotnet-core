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
using Encog.MathUtil;
using Encog.ML.Bayesian.Query.Enumeration;
using Encog.ML.Data;

namespace Encog.ML.Bayesian.Training.Search.k2
{
    /// <summary>
    /// Search for optimal Bayes structure with K2.
    /// </summary>
    public class SearchK2 : IBayesSearch
    {
        /// <summary>
        /// The node ordering.
        /// </summary>
        private readonly IList<BayesianEvent> _nodeOrdering = new List<BayesianEvent>();

        /// <summary>
        /// The data to use.
        /// </summary>
        private IMLDataSet _data;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _index = -1;

        /// <summary>
        /// The last calculated value for p.
        /// </summary>
        private double _lastCalculatedP;

        /// <summary>
        /// The network to optimize.
        /// </summary>
        private BayesianNetwork _network;

        /// <summary>
        /// The trainer being used.
        /// </summary>
        private TrainBayesian _train;

        #region IBayesSearch Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            _network = theNetwork;
            _data = theData;
            _train = theTrainer;
            OrderNodes();
            _index = -1;
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            if (_index == -1)
            {
                OrderNodes();
            }
            else
            {
                BayesianEvent e = _nodeOrdering[_index];
                double oldP = CalculateG(_network, e, e.Parents);

                while (e.Parents.Count < _train.MaximumParents)
                {
                    BayesianEvent z = FindZ(e, _index, oldP);
                    if (z != null)
                    {
                        _network.CreateDependency(z, e);
                        oldP = _lastCalculatedP;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _index++;
            return (_index < _data.InputSize);
        }

        #endregion

        /// <summary>
        /// Basically the goal here is to get the classification target, if it exists,
        /// to go first. This will greatly enhance K2's effectiveness.
        /// </summary>
        private void OrderNodes()
        {
            _nodeOrdering.Clear();

            // is there a classification target?
            if (_network.ClassificationTarget != -1)
            {
                _nodeOrdering.Add(_network.ClassificationTargetEvent);
            }


            // now add the others
            foreach (BayesianEvent e in _network.Events)
            {
                if (!_nodeOrdering.Contains(e))
                {
                    _nodeOrdering.Add(e);
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
                BayesianEvent trialParent = _nodeOrdering[i];
                IList<BayesianEvent> parents = new List<BayesianEvent>();
                parents.CopyTo(e.Parents.ToArray(), 0);
                parents.Add(trialParent);
                //System.out.println("Calculating adding " + trialParent.toString() + " to " + event.toString());
                _lastCalculatedP = CalculateG(_network, e, parents);
                //System.out.println("lastP:" + this.lastCalculatedP);
                //System.out.println("old:" + old);
                if (_lastCalculatedP > old && _lastCalculatedP > maxChildP)
                {
                    result = trialParent;
                    maxChildP = _lastCalculatedP;
                    //System.out.println("Current best is: " + result.toString());
                }
            }

            _lastCalculatedP = maxChildP;
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

            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

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

            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

                bool reject = false;

                for (int i = 0; i < parentInstance.Length; i++)
                {
                    BayesianEvent parentEvent = parents[i];
                    int parentIndex = network.GetEventIndex(parentEvent);
                    if (parentInstance[i] != (d[parentIndex]))
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

            var args = new int[parents.Count];

            do
            {
                double n = EncogMath.Factorial(r - 1);
                double d = EncogMath.Factorial(CalculateN(network, e,
                                                          parents, args) + r - 1);
                double p1 = n/d;

                double p2 = 1;
                for (int k = 0; k < e.Choices.Count; k++)
                {
                    p2 *= EncogMath.Factorial(CalculateN(network, e, parents, args, k));
                }

                result *= p1*p2;
            } while (EnumerationQuery.Roll(parents, args));

            return result;
        }
    }
}
