using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Temporal
{
    /// <summary>
    /// This class specifies how fields are to be used by the TemporalWindowCSV class.
    /// </summary>
    public class TemporalWindowField
    {
        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="name">The name of the field to be considered.</param>
        public TemporalWindowField(String name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns true, if this field is to be used as part of the input for a prediction.
        /// </summary>
        public bool Input
        {
            get
            {
                return( Action==TemporalType.Input || Action==TemporalType.InputAndPredict );
            }
        }

        /// <summary>
        /// Returns true, if this field is part of what is being predicted.
        /// </summary>
        public bool Predict
        {
            get
            {
                return (Action == TemporalType.Predict || Action == TemporalType.InputAndPredict);
            }
        }

        /// <summary>
        /// The action that is to be taken on this field.
        /// </summary>
        public TemporalType Action { get; set; }

        /// <summary>
        /// The name of this field.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// THe last value of this field.  Used internally.
        /// </summary>
        public String LastValue { get; set; }
    }
}
