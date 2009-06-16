using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that represents a form, and all embedded tags.
    /// </summary>
    public class Form : DocumentRange
    {

        /// <summary>
        /// The method for this form.
        /// </summary>
        public enum FormMethod
        {
            /// <summary>
            /// This form is to be POSTed.
            /// </summary>
            POST,
            /// <summary>
            /// This form is to sent using a GET.
            /// </summary>
            GET
        };

        /// <summary>
        /// The address that the form will be sent to.
        /// </summary>
        private Address action;

        /// <summary>
        /// The means by which the form will be sent.
        /// </summary>
        private FormMethod method;

        /// <summary>
        /// Construct a form on the specified web page.
        /// </summary>
        /// <param name="source">The web page that contains this form.</param>
        public Form(WebPage source)
            : base(source)
        {

        }

        /// <summary>
        /// Find the form input by type.
        /// </summary>
        /// <param name="type">The type of input we want.</param>
        /// <param name="index">The index to begin searching at.</param>
        /// <returns>The Input object that was found.</returns>
        public Input FindType(String type, int index)
        {
            int i = index;

            foreach (DocumentRange element in this.Elements)
            {
                if (element is Input)
                {
                    Input input = (Input)element;
                    if (String.Compare(input.Type, type, true) == 0)
                    {
                        if (i <= 0)
                        {
                            return input;
                        }
                        i--;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The URL to send the form to.
        /// </summary>
        public Address Action
        {
            get
            {
                return this.action;
            }
            set
            {
                this.action = value;
            }
        }

        public FormMethod Method
        {
            get
            {
                return this.method;
            }
            set
            {
                this.method = value;
            }
        }


        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[Form:");
            builder.Append("method=");
            builder.Append(this.Method);
            builder.Append(",action=");
            builder.Append(this.Action);
            foreach (DocumentRange element in this.Elements)
            {
                builder.Append("\n\t");
                builder.Append(element.ToString());
            }
            builder.Append("]");
            return builder.ToString();
        }

    }

}
