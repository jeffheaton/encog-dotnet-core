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
