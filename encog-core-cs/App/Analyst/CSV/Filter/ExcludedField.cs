using System;
using System.Text;

namespace Encog.App.Analyst.CSV.Filter
{
    /// <summary>
    /// Used internally to track excluded fields from the FilterCSV.
    /// </summary>
    ///
    public class ExcludedField
    {
        /// <summary>
        /// The field number.
        /// </summary>
        ///
        private int fieldNumber;

        /// <summary>
        /// The field value to filter on.
        /// </summary>
        ///
        private String fieldValue;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theFieldNumber">The field number.</param>
        /// <param name="theFieldValue">The field value to filter on.</param>
        public ExcludedField(int theFieldNumber, String theFieldValue)
        {
            fieldNumber = theFieldNumber;
            fieldValue = theFieldValue;
        }


        /// <value>the fieldNumber to set</value>
        public int FieldNumber
        {
            get { return fieldNumber; }
            set { fieldNumber = value; }
        }


        /// <value>the fieldValue to set</value>
        public String FieldValue
        {
            get { return fieldValue; }
            set { fieldValue = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" fieldNumber=");
            result.Append(fieldNumber);
            result.Append(", value=");
            result.Append(fieldValue);

            result.Append("]");
            return result.ToString();
        }
    }
}