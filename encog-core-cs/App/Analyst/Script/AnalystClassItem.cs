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
        private String code;

        /// <summary>
        /// THe count.
        /// </summary>
        ///
        private int count;

        /// <summary>
        /// The name for the class item.
        /// </summary>
        ///
        private String name;

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
            code = theCode;
            name = theName;
            count = theCount;
        }


        /// <value>the code to set</value>
        public String Code
        {
            /// <returns>the code</returns>
            get { return code; }
            /// <param name="theCode">the code to set</param>
            set { code = value; }
        }


        /// <value>The count.</value>
        public int Count
        {
            /// <returns>The count.</returns>
            get { return count; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            /// <returns>the name</returns>
            get { return name; }
            /// <param name="theName">the name to set</param>
            set { name = value; }
        }

        #region IComparable<AnalystClassItem> Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CompareTo(AnalystClassItem o)
        {
            return String.CompareOrdinal(code, o.Code);
        }

        #endregion

        /// <summary>
        /// Increase the count.
        /// </summary>
        ///
        public void IncreaseCount()
        {
            count++;
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
            result.Append(", code=");
            result.Append(code);
            result.Append("]");
            return result.ToString();
        }
    }
}