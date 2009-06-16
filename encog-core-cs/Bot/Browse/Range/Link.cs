using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that represents a hyperlink, and any embedded tags and text.
    /// </summary>
    public class Link : DocumentRange
    {

        /// <summary>
        /// The target address for this link.
        /// </summary>
        private Address target;

        /// <summary>
        /// Construct a link from the specified web page.
        /// </summary>
        /// <param name="source">The web page this link is from.</param>
        public Link(WebPage source)
            : base(source)
        {

        }

        /// <summary>
        /// The target of this link.
        /// </summary>
        public Address Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        /// <summary>
        /// Set the target of this link.
        /// </summary>
        /// <param name="target">The link target.</param>
        public void setTarget(Address target)
        {
            this.target = target;
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[Link:");
            result.Append(this.target);
            result.Append("|");
            result.Append(this.GetTextOnly());
            result.Append("]");
            return result.ToString();
        }

    }

}
