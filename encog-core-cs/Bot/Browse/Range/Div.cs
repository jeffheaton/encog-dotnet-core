using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that represents the beginning and ending DIV tag, as well as
    /// any tages embedded between them.
    /// </summary>
    public class Div : DocumentRange
    {

        /// <summary>
        /// Construct a range to hold the DIV tag.
        /// </summary>
        /// <param name="source">The web page this range was found on.</param>
        public Div(WebPage source)
            : base(source)
        {

        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[Div:class=");
            result.Append(this.ClassAttribute);
            result.Append(",id=");
            result.Append(this.IdAttribute);
            result.Append(",elements=");
            result.Append(this.Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }

}
