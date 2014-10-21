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
using System.Text;
using Encog.Util;

namespace Encog.ML.Bayesian.Query.Sample
{
    /// <summary>
    /// A sampling query allows probabilistic queries on a Bayesian network. Sampling
    /// works by actually simulating the probabilities using a random number
    /// generator. A sample size must be specified. The higher the sample size, the
    /// more accurate the probability will be. However, the higher the sampling size,
    /// the longer it takes to run the query.
    /// 
    /// An enumeration query is more precise than the sampling query. However, the
    /// enumeration query will become slow as the size of the Bayesian network grows.
    /// Sampling can often be used for a quick estimation of a probability.
    /// </summary>
    [Serializable]
    public class SamplingQuery : BasicQuery
    {
        /// <summary>
        /// The default sample size.
        /// </summary>
        public const int DefaultSampleSize = 100000;

        /// <summary>
        /// The number of samples that matched the result the query is looking for.
        /// </summary>
        private int _goodSamples;

        /// <summary>
        /// The total number of samples generated. This should match sampleSize at
        /// the end of a query.
        /// </summary>
        private int _totalSamples;

        /// <summary>
        /// The number of usable samples. This is the set size for the average
        /// probability.
        /// </summary>
        private int _usableSamples;

        /// <summary>
        /// Construct a sampling query. 
        /// </summary>
        /// <param name="theNetwork">The network that will be queried.</param>
        public SamplingQuery(BayesianNetwork theNetwork)
            : base(theNetwork)
        {
            SampleSize = DefaultSampleSize;
        }

        /// <summary>
        /// The sample size.
        /// </summary>
        public int SampleSize { get; set; }

        /// <inheritdoc/>
        public override double Probability
        {
            get { return _goodSamples/(double) _usableSamples; }
        }

        /// <summary>
        /// Obtain the arguments for an event. 
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>The arguments for that event, based on the other event values.</returns>
        private int[] ObtainArgs(BayesianEvent e)
        {
            var result = new int[e.Parents.Count];

            int index = 0;
            foreach (BayesianEvent parentEvent in e.Parents)
            {
                EventState state = GetEventState(parentEvent);
                if (!state.IsCalculated)
                    return null;
                result[index++] = state.Value;
            }
            return result;
        }

        /// <summary>
        /// Set all events to random values, based on their probabilities. 
        /// </summary>
        /// <param name="eventState">The event state.</param>
        private void RandomizeEvents(EventState eventState)
        {
            // first, has this event already been randomized
            if (!eventState.IsCalculated)
            {
                // next, see if we can randomize the event passed
                int[] args = ObtainArgs(eventState.Event);
                if (args != null)
                {
                    eventState.Randomize(args);
                }
            }

            // randomize children
            foreach (BayesianEvent childEvent in eventState.Event.Children)
            {
                RandomizeEvents(GetEventState(childEvent));
            }
        }

        /// <summary>
        /// The number of events that are still uncalculated.
        /// </summary>
        /// <returns>The uncalculated count.</returns>
        private int CountUnCalculated()
        {
            return Events.Values.Count(state => !state.IsCalculated);
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            LocateEventTypes();
            _usableSamples = 0;
            _goodSamples = 0;
            _totalSamples = 0;

            for (int i = 0; i < SampleSize; i++)
            {
                Reset();

                int lastUncalculated = int.MaxValue;
                int uncalculated;
                do
                {
                    foreach (EventState state in Events.Values)
                    {
                        RandomizeEvents(state);
                    }
                    uncalculated = CountUnCalculated();
                    if (uncalculated == lastUncalculated)
                    {
                        throw new BayesianError(
                            "Unable to calculate all nodes in the graph.");
                    }
                    lastUncalculated = uncalculated;
                } while (uncalculated > 0);

                // System.out.println("Sample:\n" + this.dumpCurrentState());
                _totalSamples++;
                if (IsNeededEvidence)
                {
                    _usableSamples++;
                    if (SatisfiesDesiredOutcome)
                    {
                        _goodSamples++;
                    }
                }
            }
        }

        /// <summary>
        /// The current state as a string.
        /// </summary>
        /// <returns>The state.</returns>
        public String DumpCurrentState()
        {
            var result = new StringBuilder();
            foreach (EventState state in Events.Values)
            {
                result.Append(state.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns></returns>
        public override IBayesianQuery Clone()
        {
            return new SamplingQuery(Network);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[SamplingQuery: ");
            result.Append(Problem);
            result.Append("=");
            result.Append(Format.FormatPercent(Probability));
            result.Append(" ;good/usable=");
            result.Append(Format.FormatInteger(_goodSamples));
            result.Append("/");
            result.Append(Format.FormatInteger(_usableSamples));
            result.Append(";totalSamples=");
            result.Append(Format.FormatInteger(_totalSamples));
            return result.ToString();
        }
    }
}
