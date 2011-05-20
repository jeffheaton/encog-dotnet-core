using System;
using System.Text;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    /// Holds a class item for the script. Some columns in a CSV are classes. This
    /// object holds the possible class types.
    /// </summary>
    ///
    public class AnalystClassItem : IComparable<AnalystClassItem>
    {
        /// <summary>
        /// THe code for the class item.
        /// </summary>
        ///
        private String _code;

        /// <summary>
        /// THe count.
        /// </summary>
        ///
        private int _count;

        /// <summary>
        /// The name for the class item.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct a class item.
        /// </summary>
        ///
        /// <param name="theCode">The code, this is how it is in the CSV.</param>
        /// <param name="theName"></param>
        /// <param name="theCount">The count.</param>
        public AnalystClassItem(String theCode, String theName,
                                int theCount)
        {
            _code = theCode;
            _name = theName;
            _count = theCount;
        }


        /// <value>the code to set</value>
        public String Code
        {
            get { return _code; }
            set { _code = value; }
        }


        /// <value>The count.</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #region IComparable<AnalystClassItem> Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CompareTo(AnalystClassItem o)
        {
            return String.CompareOrdinal(_code, o.Code);
        }

        #endregion

        /// <summary>
        /// Increase the count.
        /// </summary>
        ///
        public void IncreaseCount()
        {
            _count++;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", code=");
            result.Append(_code);
            result.Append("]");
            return result.ToString();
        }
    }
}