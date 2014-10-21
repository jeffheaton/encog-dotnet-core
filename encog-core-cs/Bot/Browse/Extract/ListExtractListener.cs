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
    /// A simple implementation of the ExtractListener interface that will listen for
    /// words and add them to a list. This allows you to quickly build a list of all
    /// of the words on a web page.
    /// </summary>
    public class ListExtractListener : IExtractListener
    {
        /// <summary>
        /// The list to extract into.
        /// </summary>
        private readonly IList<Object> _list = new List<Object>();

        /// <summary>
        /// The list of words extracted.
        /// </summary>
        public IList<Object> List
        {
            get { return _list; }
        }

        #region IExtractListener Members

        /// <summary>
        /// Called when a word is found, add it to the list.
        /// </summary>
        /// <param name="obj">The word found.</param>
        public void FoundData(Object obj)
        {
            _list.Add(obj);
        }

        #endregion
    }
}
