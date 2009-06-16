using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that specifies a span tag, and any embedded tags.
    /// </summary>
    public class Span : DocumentRange
    {

        /// <summary>
        /// Construct a span range from the specified web page.
        /// </summary>
        /// <param name="source">The source web page.</param>
        public Span(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string. 
        /// </summary>
        /// <returns>This object as a string. </returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[Span:class=");
            result.Append(ClassAttribute);
            result.Append(",id=");
            result.Append(IdAttribute);
            result.Append(",elements=");
            result.Append(Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }

}
