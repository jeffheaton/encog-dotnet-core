// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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


using System;
using System.Collections.Generic;
using System.Linq;
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
        /// The beginning index for this range.
        /// </summary>
        private int begin;

        /// <summary>
        /// The ending index for this range.
        /// </summary>
        private int end;

        /// <summary>
        /// The source page for this range.
        /// </summary>
        private WebPage source;

        /// <summary>
        /// The id attribute, on the source tag.  Useful for DIV tags.
        /// </summary>
        private String idAttribute;

        /// <summary>
        /// The class attribute. on the source tag.
        /// </summary>
        private String classAttribute;

        /// <summary>
        /// Sub elements of this range.
        /// </summary>
        private IList<DocumentRange> elements = new List<DocumentRange>();

        /// <summary>
        /// The parent to this range, or null if top.
        /// </summary>
        private DocumentRange parent;

        /// <summary>
        /// Construct a document range from the specified WebPage.
        /// </summary>
        /// <param name="source">The web page that this range belongs to.</param>
        public DocumentRange(WebPage source)
        {
            this.source = source;
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(DocumentRange element)
        {
            this.Elements.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// The beginning of this attribute.
        /// </summary>
        public int Begin
        {
            get
            {
                return this.begin;
            }
            set
            {
                this.begin = value;
            }
        }

        /// <summary>
        /// The HTML class attribiute for this element.
        /// </summary>
        public String ClassAttribute
        {
            get
            {
                return this.classAttribute;
            }
            set
            {
                this.classAttribute = value;
            }
        }

        /// <summary>
        /// The elements of this document range. 
        /// </summary>
        public IList<DocumentRange> Elements
        {
            get
            {
                return this.elements;
            }
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.end = value;
            }
        }

        /// <summary>
        /// The HTML id for this element.
        /// </summary>
        public String IdAttribute
        {
            get
            {
                return this.idAttribute;
            }
            set
            {
                this.idAttribute = value;
            }
        }

        /// <summary>
        /// The web page that owns this class.
        /// </summary>
        public DocumentRange Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// The web page that this range is owned by.
        /// </summary>
        public WebPage Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        /// <summary>
        /// Get the text from this range.
        /// </summary>
        /// <returns>The text from this range.</returns>
        public String GetTextOnly()
        {
            StringBuilder result = new StringBuilder();

            for (int i = this.Begin; i < this.End; i++)
            {
                DataUnit du = this.source.Data[i];
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
