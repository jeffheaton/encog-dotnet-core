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
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is NOT
        /// submit. This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public override bool AutoSend
        {
            get { return string.Compare(type, "submit", true) != 0; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Input:");
            builder.Append("type=");
            builder.Append(Type);
            builder.Append(",name=");
            builder.Append(Name);
            builder.Append(",value=");
            builder.Append(Value);
            builder.Append("]");
            return builder.ToString();
        }
    }
}

#endif