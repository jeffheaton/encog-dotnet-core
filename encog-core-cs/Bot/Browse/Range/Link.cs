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

using System;
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
        private Address _target;

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
            get { return _target; }
            set { _target = value; }
        }
        
        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Link:");
            result.Append(_target);
            result.Append("|");
            result.Append(GetTextOnly());
            result.Append("]");
            return result.ToString();
        }
    }
}