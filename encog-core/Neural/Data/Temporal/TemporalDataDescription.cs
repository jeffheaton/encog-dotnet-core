using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Activation;

namespace Encog.Neural.Data.Temporal
{
    public class TemporalDataDescription
    {

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Data in its raw, unmodified form.
            /// </summary>
            RAW,
            /// <summary>
            /// The percent change.
            /// </summary>
            PERCENT_CHANGE,
            /// <summary>
            /// The difference change.
            /// </summary>
            DELTA_CHANGE,
        }

        /// <summary>
        /// The lowest allowed number.
        /// </summary>
        private double low;

        /// <summary>
        /// The highest allowed number.
        /// </summary>
        private double high;

        /// <summary>
        /// Is this data item used for input to prediction?
        /// </summary>
        private bool input;

        /// <summary>
        /// Should this data item be predicted?
        /// </summary>
        private bool predict;

        /// <summary>
        /// What type of data is requested?
        /// </summary>
        private Type type;

        /// <summary>
        /// What is the index of this data item in relation to the others.
        /// </summary>
        private int index;

        public double Low
        {
            get
            {
                return this.low;
            }
            set
            {
                this.low = value;
            }
        }

        public double High
        {
            get
            {
                return this.high;
            }
            set
            {
                this.high = value;
            }
        }

        public bool IsInput
        {
            get
            {
                return this.input;
            }
            set
            {
                this.input = value;
            }
        }

        public bool IsPredict
        {
            get
            {
                return this.predict;
            }
            set
            {
                this.predict = value;
            }
        }

        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        public Type DescriptionType
        {
            get
            {
                return this.type;
            }
        }

        public IActivationFunction ActivationFunction
        {
            get
            {
                return this.activationFunction;
            }
        }

        /// <summary>
        /// Should an activation function be used?
        /// </summary>
        private IActivationFunction activationFunction;

        /// <summary>
        /// Construct a data description item. Set both low and high to zero for
        /// unbounded.
        /// </summary>
        /// <param name="activationFunction">What activation function should be used?</param>
        /// <param name="low">What is the lowest allowed value.</param>
        /// <param name="high">What is the highest allowed value.</param>
        /// <param name="type">What type of data is this.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                 double low, double high, Type type,
                 bool input, bool predict)
        {
            this.low = low;
            this.type = type;
            this.high = high;
            this.input = input;
            this.predict = predict;
            this.activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a data description with an activation function, but no range.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                 Type type, bool input, bool predict)
            : this(activationFunction, 0, 0, type, input, predict)
        {

        }

        /// <summary>
        /// Construct a data description with no activation function or range.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(Type type, bool input,
                 bool predict)
            : this(null, 0, 0, type, input, predict)
        {

        }
    }
}
