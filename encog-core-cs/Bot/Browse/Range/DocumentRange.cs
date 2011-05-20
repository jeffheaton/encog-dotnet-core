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
using System.Collections.Generic;
using System.Text;
using Encog.Bot.DataUnits;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// Base class that represents a document range. A document range is a collection
    /// of tags that all apply to one "concept". For example, a Form, or a Link. This
    /// allows the form to collect the elements inside the form, or a link to collect
    /// the text along with the link tag.
    /// </summary>
    public class DocumentRange
    {
        /// <summary>
        /// Sub elements of this range.
        /// </summary>
        private readonly IList<DocumentRange> _elements = new List<DocumentRange>();

        /// <summary>
        /// The source page for this range.
        /// </summary>
        private WebPage _source;

        /// <summary>
        /// Construct a document range from the specified WebPage.
        /// </summary>
        /// <param name="source">The web page that this range belongs to.</param>
        public DocumentRange(WebPage source)
        {
            _source = source;
        }

        /// <summary>
        /// The beginning of this attribute.
        /// </summary>
        public int Begin { get; set; }

        /// <summary>
        /// The HTML class attribiute for this element.
        /// </summary>
        public String ClassAttribute { get; set; }

        /// <summary>
        /// The elements of this document range. 
        /// </summary>
        public IList<DocumentRange> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// The HTML id for this element.
        /// </summary>
        public String IdAttribute { get; set; }

        /// <summary>
        /// The web page that owns this class.
        /// </summary>
        public DocumentRange Parent { get; set; }

        /// <summary>
        /// The web page that this range is owned by.
        /// </summary>
        public WebPage Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(DocumentRange element)
        {
            Elements.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// Get the text from this range.
        /// </summary>
        /// <returns>The text from this range.</returns>
        public String GetTextOnly()
        {
            var result = new StringBuilder();

            for (int i = Begin; i < End; i++)
            {
                DataUnit du = _source.Data[i];
                if (du is TextDataUnit)
                {
                    result.Append(du.ToString());
                    result.Append("\n");
                }
            }

            return result.ToString();
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return GetTextOnly();
        }
    }
}