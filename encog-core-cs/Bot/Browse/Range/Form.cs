// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT
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

#endif