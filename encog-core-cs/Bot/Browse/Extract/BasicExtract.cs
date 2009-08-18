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
    /// Implements the basic functionality that most extractors will need to
    /// implement. Mostly this involves maintaining a collection of the extraction
    /// listeners that will receive events as the extraction occurs.
    /// </summary>
    public abstract class BasicExtract : IExtract
    {
        /// <summary>
        /// The classes registered as listeners for the extraction.
        /// </summary>
        private ICollection<IExtractListener> listeners =
            new List<IExtractListener>();

        /// <summary>
        /// Add a listener for the extraction.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddListener(IExtractListener listener)
        {
            this.listeners.Add(listener);

        }

        /// <summary>
        /// Distribute an object to the listeners.
        /// </summary>
        /// <param name="obj">The object to be distributed.</param>
        public void Distribute(Object obj)
        {
            foreach (IExtractListener listener in this.listeners)
            {
                listener.FoundData(obj);
            }
        }

        /// <summary>
        /// Extract from the web page and return the results as a list.
        /// </summary>
        /// <param name="page">The web page to extract from.</param>
        /// <returns>The results of the extraction as a List.</returns>
        public IList<Object> ExtractList(WebPage page)
        {
            this.Listeners.Clear();
            ListExtractListener listener = new ListExtractListener();
            AddListener(listener);
            Extract(page);
            return listener.List;
        }

        /// <summary>
        /// A list of listeners registered with this object.
        /// </summary>
        public ICollection<IExtractListener> Listeners
        {
            get
            {
                return this.listeners;
            }
        }

        /// <summary>
        /// Remove the specified listener.
        /// </summary>
        /// <param name="listener">The listener to rmove.</param>
        public void RemoveListener(IExtractListener listener)
        {
            this.listeners.Remove(listener);

        }

        /// <summary>
        /// Extract data from the web page.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        public abstract void Extract(WebPage page);

    }
}
