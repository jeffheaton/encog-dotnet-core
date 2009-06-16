using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A form element that represents for input for text.  These are of the
    /// form name=value.
    /// </summary>
    public class Input : FormElement
    {

        /// <summary>
        /// The type of input element that this is.
        /// </summary>
        private String type;

        /// <summary>
        /// Construct this Input element.
        /// </summary>
        /// <param name="source">The source for this input ent.</param>
        public Input(WebPage source)
            : base(source)
        {

        }

        /// <summary>
        /// The type of this input.
        /// </summary>
        public String Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is NOT
        /// submit. This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public override bool AutoSend
        {
            get
            {
                return string.Compare(this.type, "submit", true) != 0;
            }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[Input:");
            builder.Append("type=");
            builder.Append(this.Type);
            builder.Append(",name=");
            builder.Append(this.Name);
            builder.Append(",value=");
            builder.Append(this.Value);
            builder.Append("]");
            return builder.ToString();
        }
    }

}
