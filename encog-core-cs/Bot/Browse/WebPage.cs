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
using Encog.Bot.Browse.Range;
using Encog.Bot.DataUnits;

namespace Encog.Bot.Browse
{
    /// <summary>
    /// Holds a web page that was loaded by the Browse class. Web pages are made
    /// up of DataUnits and contents, which are ranges of data units.  The data
    /// units are basically tags and blocks of text.  The contents collection uses
    /// DocumentRange objects to assign meatning to the lower level DataObjects.
    /// </summary>
    public class WebPage
    {
        /// <summary>
        /// The contents of this page, builds upon the list of DataUnits.
        /// </summary>
        private readonly IList<DocumentRange> contents = new List<DocumentRange>();

        /// <summary>
        /// The data units that make up this page.
        /// </summary>
        private readonly IList<DataUnit> data = new List<DataUnit>();

        /// <summary>
        /// The title of this HTML page.
        /// </summary>
        private DocumentRange title;

        /// <summary>
        /// The contents in a list collection.
        /// </summary>
        public IList<DocumentRange> Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// The data units in a list collection.
        /// </summary>
        public IList<DataUnit> Data
        {
            get { return data; }
        }

        /// <summary>
        /// The title of this document.
        /// </summary>
        public DocumentRange Title
        {
            get { return title; }
            set
            {
                title = value;
                title.Source = this;
            }
        }

        /// <summary>
        /// Add to the content collection.
        /// </summary>
        /// <param name="span">The range to add to the collection.</param>
        public void AddContent(DocumentRange span)
        {
            span.Source = this;
            contents.Add(span);
        }

        /// <summary>
        /// Add a data unit to the collection.
        /// </summary>
        /// <param name="unit">The data unit to load.</param>
        public void AddDataUnit(DataUnit unit)
        {
            data.Add(unit);
        }

        /// <summary>
        /// Find the specified DocumentRange subclass in the contents list.
        /// </summary>
        /// <param name="c">The class type to search for.</param>
        /// <param name="index">The index to search from.</param>
        /// <returns>The document range that was found.</returns>
        public DocumentRange Find(Type c, int index)
        {
            int i = index;
            foreach (DocumentRange span in Contents)
            {
                if (span.GetType() == c)
                {
                    if (i <= 0)
                    {
                        return span;
                    }
                    i--;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the link that contains the specified string.
        /// </summary>
        /// <param name="str">The string to search for.</param>
        /// <returns>The link found.</returns>
        public Link FindLink(String str)
        {
            foreach (DocumentRange span in Contents)
            {
                if (span is Link)
                {
                    var link = (Link) span;
                    if (link.GetTextOnly().Equals(str))
                    {
                        return link;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the number of data items in this collection.
        /// </summary>
        /// <returns>The size of the data unit.</returns>
        public int getDataSize()
        {
            return data.Count;
        }

        /// <summary>
        /// Get the DataUnit unit at the specified index.
        /// </summary>
        /// <param name="i">The index to use.</param>
        /// <returns>The DataUnit found at the specified index.</returns>
        public DataUnit GetDataUnit(int i)
        {
            return data[i];
        }


        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            foreach (DocumentRange span in Contents)
            {
                result.Append(span.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}