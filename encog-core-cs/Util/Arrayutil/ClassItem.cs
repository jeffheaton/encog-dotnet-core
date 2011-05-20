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
        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        /// <summary>
        /// Set the name of the class.
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <inheritdoc/>
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