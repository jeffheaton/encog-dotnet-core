//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that represents a form, and all embedded tags.
    /// </summary>
    public class Form : DocumentRange
    {
        #region FormMethod enum

        /// <summary>
        /// The method for this form.
        /// </summary>
        public enum FormMethod
        {
            /// <summary>
            /// This form is to be POSTed.
            /// </summary>
            Post,
            /// <summary>
            /// This form is to sent using a GET.
            /// </summary>
            Get
        } ;

        #endregion

        /// <summary>
        /// Construct a form on the specified web page.
        /// </summary>
        /// <param name="source">The web page that contains this form.</param>
        public Form(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The URL to send the form to.
        /// </summary>
        public Address Action { get; set; }

        /// <summary>
        /// The method, GET or POST.
        /// </summary>
        public FormMethod Method { get; set; }

        /// <summary>
        /// Find the form input by type.
        /// </summary>
        /// <param name="type">The type of input we want.</param>
        /// <param name="index">The index to begin searching at.</param>
        /// <returns>The Input object that was found.</returns>
        public Input FindType(String type, int index)
        {
            int i = index;

            foreach (DocumentRange element in Elements)
            {
                if (element is Input)
                {
                    var input = (Input) element;
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
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Form:");
            builder.Append("method=");
            builder.Append(Method);
            builder.Append(",action=");
            builder.Append(Action);
            foreach (DocumentRange element in Elements)
            {
                builder.Append("\n\t");
                builder.Append(element.ToString());
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
