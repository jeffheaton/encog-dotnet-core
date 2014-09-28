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
using System.Text;
using Encog.ML.Bayesian.Table;
using Encog.Util;

namespace Encog.ML.Bayesian.Query.Enumeration
{
    /// <summary>
    /// An enumeration query allows probabilistic queries on a Bayesian network.
    /// Enumeration works by calculating every combination of hidden nodes and using
    /// total probability. This results in an accurate deterministic probability.
    /// However, enumeration can be slow for large Bayesian networks. For a quick
    /// estimate of probability the sampling query can be used.
    /// </summary>
    [Serializable]
    public class EnumerationQuery : BasicQuery
    {
        /// <summary>
        /// The events that we will enumerate over.
        /// </summary>
        private readonly IList<EventState> _enumerationEvents = new List<EventState>();

        /// <summary>
        /// The calculated probability.
        /// </summary>
        private double _probability;

        /// <summary>
        /// Construct the enumeration query.
        /// </summary>
        /// <param name="theNetwork">The Bayesian network to query.</param>
        public EnumerationQuery(BayesianNetwork theNetwork)
            : base(theNetwork)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EnumerationQuery()
        {
        }

        /// <inheritdoc/>
        public override double Probability
        {
            get { return _probability; }
        }

        /// <summary>
        /// Reset the enumeration events. Always reset the hidden events. Optionally
        /// reset the evidence and outcome.
        /// </summary>
        /// <param name="includeEvidence">True if the evidence is to be reset.</param>
        /// <param name="includeOutcome">True if the outcome is to be reset.</param>
        public void ResetEnumeration(bool includeEvidence, bool includeOutcome)
        {
            _enumerationEvents.Clear();

            foreach (EventState state in Events.Values)
            {
                if (state.CurrentEventType == EventType.Hidden)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else if (includeEvidence
                         && state.CurrentEventType == EventType.Evidence)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else if (includeOutcome
                         && state.CurrentEventType == EventType.Outcome)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else
                {
                    state.Value = state.CompareValue;
                }
            }
        }

        /// <summary>
        /// Roll the enumeration events forward by one.
        /// </summary>
        /// <returns>False if there are no more values to roll into, which means we're
        /// done.</returns>
        public bool Forward()
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (_enumerationEvents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {
                EventState state = _enumerationEvents[currentIndex];
                int v = state.Value;
                v++;
                if (v >= state.Event.Choices.Count)
                {
                    state.Value = 0;
                }
                else
                {
                    state.Value = v;
                    break;
                }

                currentIndex++;

                if (currentIndex >= _enumerationEvents.Count)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// Obtain the arguments for an event.
        /// </summary>
        /// <param name="theEvent">The event.</param>
        /// <returns>The arguments.</returns>
        private int[] ObtainArgs(BayesianEvent theEvent)
        {
            var result = new int[theEvent.Parents.Count];

            int index = 0;
            foreach (BayesianEvent parentEvent in theEvent.Parents)
            {
                EventState state = GetEventState(parentEvent);
                result[index++] = state.Value;
            }
            return result;
        }

        /// <summary>
        /// Calculate the probability for a state.
        /// </summary>
        /// <param name="state">The state to calculate.</param>
        /// <returns>The probability.</returns>
        private double CalculateProbability(EventState state)
        {
            int[] args = ObtainArgs(state.Event);

            foreach (TableLine line in state.Event.Table.Lines)
            {
                if (line.CompareArgs(args))
                {
                    if (Math.Abs(line.Result - state.Value) < EncogFramework.DefaultDoubleEqual)
                    {
                        return line.Probability;
                    }
                }
            }

            throw new BayesianError("Could not determine the probability for "
                                    + state);
        }

        /// <summary>
        /// Perform a single enumeration. 
        /// </summary>
        /// <returns>The result.</returns>
        private double PerformEnumeration()
        {
            double result = 0;

            do
            {
                bool first = true;
                double prob = 0;
                foreach (EventState state in Events.Values)
                {
                    if (first)
                    {
                        prob = CalculateProbability(state);
                        first = false;
                    }
                    else
                    {
                        prob *= CalculateProbability(state);
                    }
                }
                result += prob;
            } while (Forward());
            return result;
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            LocateEventTypes();
            ResetEnumeration(false, false);
            double numerator = PerformEnumeration();
            ResetEnumeration(false, true);
            double denominator = PerformEnumeration();
            _probability = numerator/denominator;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[SamplingQuery: ");
            result.Append(Problem);
            result.Append("=");
            result.Append(Format.FormatPercent(Probability));
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Roll the enumeration events forward by one.
        /// </summary>
        /// <param name="enumerationEvents">The events to roll.</param>
        /// <param name="args">The arguments to roll.</param>
        /// <returns>False if there are no more values to roll into, which means we're
        ///         done.</returns>
        public static bool Roll(IList<BayesianEvent> enumerationEvents, int[] args)
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (enumerationEvents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {
                BayesianEvent e = enumerationEvents[currentIndex];
                int v = args[currentIndex];
                v++;
                if (v >= e.Choices.Count)
                {
                    args[currentIndex] = 0;
                }
                else
                {
                    args[currentIndex] = v;
                    break;
                }

                currentIndex++;

                if (currentIndex >= args.Length)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// A clone of this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override IBayesianQuery Clone()
        {
            return new EnumerationQuery(Network);
        }
    }
}
