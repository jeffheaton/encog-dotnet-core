using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Classify
{
    /// <summary>
    /// A class item.
    /// </summary>
    public class ClassItem
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The index of the class.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="index">The index of the class.</param>
        public ClassItem(String name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}
