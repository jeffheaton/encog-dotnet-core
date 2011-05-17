using System;
using System.Text;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// This class specifies how fields are to be used by the TemporalWindowCSV
    /// class.
    /// </summary>
    ///
    public class TemporalWindowField
    {
        /// <summary>
        /// The action that is to be taken on this field.
        /// </summary>
        ///
        private TemporalType action;

        /// <summary>
        /// The name of this field.
        /// </summary>
        ///
        private String name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the field to be considered.</param>
        public TemporalWindowField(String theName)
        {
            name = theName;
        }


        /// <value>the action to set</value>
        public TemporalType Action
        {
            /// <returns>the action</returns>
            get { return action; }
            /// <param name="theAction">the action to set</param>
            set { action = value; }
        }


        /// <value>Returns true, if this field is to be used as part of the input
        /// for a prediction.</value>
        public bool Input
        {
            /// <returns>Returns true, if this field is to be used as part of the input
            /// for a prediction.</returns>
            get { return ((action == TemporalType.Input) || (action == TemporalType.InputAndPredict)); }
        }


        /// <value>the lastValue to set</value>
        public String LastValue { /// <returns>the lastValue</returns>
            get; /// <param name="theLastValue">the lastValue to set</param>
            set; }


        /// <value>the name to set</value>
        public String Name
        {
            /// <returns>the name</returns>
            get { return name; }
            /// <param name="theName">the name to set</param>
            set { name = value; }
        }


        /// <value>Returns true, if this field is part of what is being predicted.</value>
        public bool Predict
        {
            /// <returns>Returns true, if this field is part of what is being predicted.</returns>
            get { return ((action == TemporalType.Predict) || (action == TemporalType.InputAndPredict)); }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(name);
            result.Append(", action=");
            result.Append(action);

            result.Append("]");
            return result.ToString();
        }
    }
}