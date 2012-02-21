using System;
using System.Collections.Generic;
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
    public class SamplingQuery : BasicQuery
    {
        /// <summary>
        /// The default sample size.
        /// </summary>
        public const int DEFAULT_SAMPLE_SIZE = 100000;

        /// <summary>
        /// The sample size.
        /// </summary>
        public int SampleSize { get; set; }

        /// <summary>
        /// The number of usable samples. This is the set size for the average
        /// probability.
        /// </summary>
        private int usableSamples;

        /// <summary>
        /// The number of samples that matched the result the query is looking for.
        /// </summary>
        private int goodSamples;

        /// <summary>
        /// The total number of samples generated. This should match sampleSize at
        /// the end of a query.
        /// </summary>
        private int totalSamples;

        /// <summary>
        /// Construct a sampling query. 
        /// </summary>
        /// <param name="theNetwork">The network that will be queried.</param>
        public SamplingQuery(BayesianNetwork theNetwork)
            : base(theNetwork)
        {
            SampleSize = DEFAULT_SAMPLE_SIZE;
        }

        /// <summary>
        /// Obtain the arguments for an event. 
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>The arguments for that event, based on the other event values.</returns>
        private int[] ObtainArgs(BayesianEvent e)
        {
            int[] result = new int[e.Parents.Count];

            int index = 0;
            foreach (BayesianEvent parentEvent in e.Parents)
            {
                EventState state = this.GetEventState(parentEvent);
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
            int result = 0;
            foreach (EventState state in Events.Values)
            {
                if (!state.IsCalculated)
                    result++;
            }
            return result;
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            LocateEventTypes();
            this.usableSamples = 0;
            this.goodSamples = 0;
            this.totalSamples = 0;

            for (int i = 0; i < this.SampleSize; i++)
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
                this.totalSamples++;
                if (IsNeededEvidence)
                {
                    this.usableSamples++;
                    if (SatisfiesDesiredOutcome)
                    {
                        this.goodSamples++;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override double Probability
        {
            get
            {
                return (double)this.goodSamples / (double)this.usableSamples;
            }
        }

        /// <summary>
        /// The current state as a string.
        /// </summary>
        /// <returns>The state.</returns>
        public String DumpCurrentState()
        {
            StringBuilder result = new StringBuilder();
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
            return new SamplingQuery(this.Network);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[SamplingQuery: ");
            result.Append(Problem);
            result.Append("=");
            result.Append(Format.FormatPercent(Probability));
            result.Append(" ;good/usable=");
            result.Append(Format.FormatInteger(this.goodSamples));
            result.Append("/");
            result.Append(Format.FormatInteger(this.usableSamples));
            result.Append(";totalSamples=");
            result.Append(Format.FormatInteger(this.totalSamples));
            return result.ToString();
        }
    }
}
