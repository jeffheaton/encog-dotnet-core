using System;
using System.Collections.Generic;

namespace Encog.ML.Bayesian.Query
{
    /// <summary>
    /// A Bayesian query. This is used to query a Bayesian network and determine a
    /// the probability of an output, given some input. The input is called evidence,
    /// and the output is the outcome. This results in a final probability of the
    /// output being what you specified.
    /// 
    /// You can easily change the events between evidence and outcome, this allows
    /// the Bayesian network to be queried in nearly any way.  It is also possible to
    /// omit missing evidence to handle missing data.
    /// </summary>
    public interface IBayesianQuery
    {
        /// <summary>
        /// The Bayesian network that we are using this query for.
        /// </summary>
        BayesianNetwork Network { get; }

        /// <summary>
        /// A mapping of events to event states.
        /// </summary>
        IDictionary<BayesianEvent, EventState> Events { get; }

        /// <summary>
        /// The evidence events (inputs).
        /// </summary>
        IList<BayesianEvent> EvidenceEvents { get; }

        /// <summary>
        /// The outcome events (outputs).
        /// </summary>
        IList<BayesianEvent> OutcomeEvents { get; }

        /// <summary>
        /// Return a string that represents this query as a probability "problem".
        /// </summary>
        String Problem { get; }

        /// <summary>
        /// Obtains the probability after execute has been called.
        /// </summary>
        double Probability { get; }

        /// <summary>
        /// Reset all event types back to hidden.
        /// </summary>
        void Reset();

        /// <summary>
        /// Define an event type to be either hidden(default), evidence(input) or
        /// outcome (output).
        /// </summary>
        /// <param name="theEvent">The event to define.</param>
        /// <param name="et">The new event type.         */</param>
        void DefineEventType(BayesianEvent theEvent, EventType et);

        /// <summary>
        /// Get the event state for a given event.
        /// </summary>
        /// <param name="theEvent">The event to get the state for.</param>
        /// <returns>The event state.</returns>
        EventState GetEventState(BayesianEvent theEvent);

        ///<summary>
        /// Get the event type.
        ///</summary>
        ///<param name="theEvent">The event to check.</param>
        ///<returns>The current event type for this event.</returns>
        EventType GetEventType(BayesianEvent theEvent);

        /// <summary>
        /// Set the event value to a boolean.
        /// </summary>
        /// <param name="theEvent">The event.</param>
        /// <param name="b">The value.</param>
        void SetEventValue(BayesianEvent theEvent, bool b);

        /// <summary>
        /// Set the event value as a class item.
        /// </summary>
        /// <param name="theEvent">The event to set.</param>
        /// <param name="d">An integer class item.</param>
        void SetEventValue(BayesianEvent theEvent, int d);

        /// <summary>
        /// Execute the query.
        /// </summary>
        void Execute();

        /// <summary>
        /// Finalize the structure, once all events and dependencies are in place.
        /// </summary>
        void FinalizeStructure();

        /// <summary>
        /// Called to locate the evidence and outcome events.
        /// </summary>
        void LocateEventTypes();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        IBayesianQuery Clone();
    }
}