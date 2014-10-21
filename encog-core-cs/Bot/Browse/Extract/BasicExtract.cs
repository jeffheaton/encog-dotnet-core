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
        private readonly ICollection<IExtractListener> _listeners =
            new List<IExtractListener>();

        #region IExtract Members

        /// <summary>
        /// Add a listener for the extraction.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddListener(IExtractListener listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Extract from the web page and return the results as a list.
        /// </summary>
        /// <param name="page">The web page to extract from.</param>
        /// <returns>The results of the extraction as a List.</returns>
        public IList<Object> ExtractList(WebPage page)
        {
            Listeners.Clear();
            var listener = new ListExtractListener();
            AddListener(listener);
            Extract(page);
            return listener.List;
        }

        /// <summary>
        /// A list of listeners registered with this object.
        /// </summary>
        public ICollection<IExtractListener> Listeners
        {
            get { return _listeners; }
        }

        /// <summary>
        /// Remove the specified listener.
        /// </summary>
        /// <param name="listener">The listener to rmove.</param>
        public void RemoveListener(IExtractListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// Extract data from the web page.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        public abstract void Extract(WebPage page);

        #endregion

        /// <summary>
        /// Distribute an object to the listeners.
        /// </summary>
        /// <param name="obj">The object to be distributed.</param>
        public void Distribute(Object obj)
        {
            foreach (IExtractListener listener in _listeners)
            {
                listener.FoundData(obj);
            }
        }
    }
}
