using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Bayesian.Query
{
    /// <summary>
    /// Provides basic functionality for a Bayesian query. This class is abstract,
    /// and is not used directly. Rather, other queries make use of it.
    /// </summary>
    public abstract class BasicQuery : IBayesianQuery
    {
        /// <summary>
        /// The network to be queried.
        /// </summary>
        private readonly BayesianNetwork network;

        /// <summary>
        /// A mapping of the events to event states.
        /// </summary>
        private readonly IDictionary<BayesianEvent, EventState> events = new Dictionary<BayesianEvent, EventState>();

        /// <summary>
        /// The evidence events.
        /// </summary>
        private readonly IList<BayesianEvent> evidenceEvents = new List<BayesianEvent>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicQuery()
        {
            this.network = null;
        }

        /// <summary>
        /// The outcome events.
        /// </summary>
        private readonly IList<BayesianEvent> outcomeEvents = new List<BayesianEvent>();

        /// <summary>
        /// Construct a basic query.
        /// </summary>
        /// <param name="theNetwork">The network to use for this query.</param>
        public BasicQuery(BayesianNetwork theNetwork)
        {
            this.network = theNetwork;
            FinalizeStructure();
        }

        /// <summary>
        /// Finalize the query structure.
        /// </summary>
        public void FinalizeStructure()
        {
            this.events.Clear();
            foreach (BayesianEvent e in this.network.Events)
            {
                events[e] = new EventState(e);
            }
        }

        /// <inheritdoc/>
        public BayesianNetwork Network
        {
            get
            {
                return network;
            }
        }

        /// <inheritdoc/>
        public IDictionary<BayesianEvent, EventState> Events
        {
            get
            {
                return events;
            }
        }



        /// <inheritdoc/>
        public IList<BayesianEvent> EvidenceEvents
        {
            get
            {
                return evidenceEvents;
            }
        }

        /// <inheritdoc/>
        public IList<BayesianEvent> OutcomeEvents
        {
            get
            {
                return outcomeEvents;
            }
        }

        /// <summary>
        /// Called to locate the evidence and outcome events.
        /// </summary>
        public void LocateEventTypes()
        {
            this.evidenceEvents.Clear();
            this.outcomeEvents.Clear();

            foreach (BayesianEvent e in this.network.Events)
            {
                switch (GetEventType(e))
                {
                    case EventType.Evidence:
                        this.evidenceEvents.Add(e);
                        break;
                    case EventType.Outcome:
                        this.outcomeEvents.Add(e);
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            foreach (EventState s in this.events.Values)
            {
                s.IsCalculated = false;
            }
        }


        /// <inheritdoc/>
        public void DefineEventType(BayesianEvent theEvent, EventType et)
        {
            GetEventState(theEvent).CurrentEventType = et;
        }

        /// <inheritdoc/>
        public EventState GetEventState(BayesianEvent theEvent)
        {
            if (!this.events.ContainsKey(theEvent))
                return null;
            return this.events[theEvent];
        }

        /// <inheritdoc/>
        public EventType GetEventType(BayesianEvent theEvent)
        {
            return GetEventState(theEvent).CurrentEventType;
        }

        /// <summary>
        /// Determines if the evidence events have values that satisfy the
        /// needed case. This is used for sampling.
        /// </summary>
        protected bool IsNeededEvidence
        {
            get
            {
                foreach (BayesianEvent evidenceEvent in this.evidenceEvents)
                {
                    EventState state = GetEventState(evidenceEvent);
                    if (!state.IsSatisfied)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// True, if the current state satisifies the desired outcome.
        /// </summary>
        /// <returns></returns>
        protected bool SatisfiesDesiredOutcome
        {
            get
            {
                foreach (BayesianEvent outcomeEvent in this.outcomeEvents)
                {
                    EventState state = GetEventState(outcomeEvent);
                    if (!state.IsSatisfied)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <inheritdoc/>
        public void SetEventValue(BayesianEvent theEvent, bool b)
        {
            SetEventValue(theEvent, b ? 0 : 1);
        }

        /// <inheritdoc/>
        public void SetEventValue(BayesianEvent theEvent, int d)
        {
            if (GetEventType(theEvent) == EventType.Hidden)
            {
                throw new BayesianError("You may only set the value of an evidence or outcome event.");
            }

            GetEventState(theEvent).CompareValue = d;
            GetEventState(theEvent).Value = d;
        }

        /// <inheritdoc/>
        public String Problem
        {
            get
            {

                if (this.outcomeEvents.Count == 0)
                    return "";

                StringBuilder result = new StringBuilder();
                result.Append("P(");
                bool first = true;
                foreach (BayesianEvent e in this.outcomeEvents)
                {
                    if (!first)
                    {
                        result.Append(",");
                    }
                    first = false;
                    result.Append(EventState.ToSimpleString(GetEventState(e)));
                }
                result.Append("|");

                first = true;
                foreach (BayesianEvent e in this.evidenceEvents)
                {
                    if (!first)
                    {
                        result.Append(",");
                    }
                    first = false;
                    EventState state = GetEventState(e);
                    if (state == null)
                        break;
                    result.Append(EventState.ToSimpleString(state));
                }
                result.Append(")");

                return result.ToString();
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A clone of the object.</returns>
        public virtual IBayesianQuery Clone()
        {
            return null;
        }

        /// <inheritdoc/>
        public abstract double Probability { get; }

        /// <inheritdoc/>
        public abstract void Execute();


    }
}
