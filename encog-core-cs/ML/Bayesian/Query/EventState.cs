using System;
using System.Text;
using Encog.Util;

namespace Encog.ML.Bayesian.Query
{
    /// <summary>
    /// Holds the state of an event during a query. This allows the event to actually
    /// hold a value, as well as an anticipated value (compareValue).
    /// </summary>
    [Serializable]
    public class EventState
    {
        /// <summary>
        /// The event that this state is connected to.
        /// </summary>
        private readonly BayesianEvent _event;

        /// <summary>
        /// The current value of this event.
        /// </summary>
        private int _value;

        /// <summary>
        /// Construct an event state for the specified event. 
        /// </summary>
        /// <param name="theEvent">The event to create a state for.</param>
        public EventState(BayesianEvent theEvent)
        {
            _event = theEvent;
            CurrentEventType = EventType.Hidden;
            IsCalculated = false;
        }

        /// <summary>
        /// Has this event been calculated yet?
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        /// The type of event that this is for the query.
        /// </summary>
        public EventType CurrentEventType { get; set; }

        /// <summary>
        /// The value that we are comparing to, for probability.
        /// </summary>
        public int CompareValue { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                IsCalculated = true;
                _value = value;
            }
        }


        /// <summary>
        /// The event.
        /// </summary>
        public BayesianEvent Event
        {
            get { return _event; }
        }


        /// <summary>
        /// Is this event satisified.
        /// </summary>
        public bool IsSatisfied
        {
            get
            {
                if (CurrentEventType == EventType.Hidden)
                {
                    throw new BayesianError(
                        "Satisfy can't be called on a hidden event.");
                }
                return Math.Abs(CompareValue - _value) < EncogFramework.DefaultDoubleEqual;
            }
        }

        /// <summary>
        /// Randomize according to the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Randomize(params int[] args)
        {
            Value = _event.Table.GenerateRandom(args);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[EventState:event=");
            result.Append(_event.ToString());
            result.Append(",type=");
            result.Append(CurrentEventType.ToString());
            result.Append(",value=");
            result.Append(Format.FormatDouble(_value, 2));
            result.Append(",compare=");
            result.Append(Format.FormatDouble(CompareValue, 2));
            result.Append(",calc=");
            result.Append(IsCalculated ? "y" : "n");
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Convert a state to a simple string. (probability expression) 
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A probability expression as a string.</returns>
        public static String ToSimpleString(EventState state)
        {
            return BayesianEvent.FormatEventName(state.Event, state.CompareValue);
        }
    }
}