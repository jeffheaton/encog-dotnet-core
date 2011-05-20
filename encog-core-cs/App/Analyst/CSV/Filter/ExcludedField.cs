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
        private int _fieldNumber;

        /// <summary>
        /// The field value to filter on.
        /// </summary>
        ///
        private String _fieldValue;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theFieldNumber">The field number.</param>
        /// <param name="theFieldValue">The field value to filter on.</param>
        public ExcludedField(int theFieldNumber, String theFieldValue)
        {
            _fieldNumber = theFieldNumber;
            _fieldValue = theFieldValue;
        }


        /// <value>the fieldNumber to set</value>
        public int FieldNumber
        {
            get { return _fieldNumber; }
            set { _fieldNumber = value; }
        }


        /// <value>the fieldValue to set</value>
        public String FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
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
            result.Append(_fieldNumber);
            result.Append(", value=");
            result.Append(_fieldValue);

            result.Append("]");
            return result.ToString();
        }
    }
}