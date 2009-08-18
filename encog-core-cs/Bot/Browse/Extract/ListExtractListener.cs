// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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

namespace Encog.Bot.Browse.Extract
{
    /// <summary>
    /// A simple implementation of the ExtractListener interface that will listen for
    /// words and add them to a list. This allows you to quickly build a list of all
    /// of the words on a web page.
    /// </summary>
    public class ListExtractListener : IExtractListener
    {
        /// <summary>
        /// The list to extract into.
        /// </summary>
        private IList<Object> list = new List<Object>();

        /// <summary>
        /// Called when a word is found, add it to the list.
        /// </summary>
        /// <param name="obj">The word found.</param>
        public void FoundData(Object obj)
        {
            this.list.Add(obj);
        }

        /// <summary>
        /// The list of words extracted.
        /// </summary>
        public IList<Object> List
        {
            get
            {
                return this.list;
            }
        }
    }
}
