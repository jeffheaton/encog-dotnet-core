using System;
using System.Text;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// A class item.
    /// </summary>
    ///
    public class ClassItem
    {
        /// <summary>
        /// The index of the class.
        /// </summary>
        ///
        private int index;

        /// <summary>
        /// The name of the class.
        /// </summary>
        ///
        private String name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the class.</param>
        /// <param name="theIndex">The index of the class.</param>
        public ClassItem(String theName, int theIndex)
        {
            name = theName;
            index = theIndex;
        }

        /// <summary>
        /// Set the index of the class.
        /// </summary>
        ///
        /// <value>The index of the class.</value>
        public int Index
        {
            /// <returns>The index of the class.</returns>
            get { return index; }
            /// <summary>
            /// Set the index of the class.
            /// </summary>
            ///
            /// <param name="theIndex">The index of the class.</param>
            set { index = value; }
        }


        /// <summary>
        /// Set the name of the class.
        /// </summary>
        ///
        /// <value>The name of the class.</value>
        public String Name
        {
            /// <returns>The name of the class.</returns>
            get { return name; }
            /// <summary>
            /// Set the name of the class.
            /// </summary>
            ///
            /// <param name="theName">The name of the class.</param>
            set { name = value; }
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
            result.Append(", index=");
            result.Append(index);

            result.Append("]");
            return result.ToString();
        }
    }
}