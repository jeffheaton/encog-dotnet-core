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
    /// Provides the basic interface that any extractor must support. An extractor is
    /// a class that is capable of extracting certain types of data from web data.
    /// For example, the ExtractWords extractor is used to extract all of the words
    /// from a web page.
    /// </summary>
    public interface IExtract
    {
        /// <summary>
        /// A list of listeners registered with this object.
        /// </summary>
        ICollection<IExtractListener> Listeners { get; }

        /// <summary>
        /// Add a listener for the extraction.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        void AddListener(IExtractListener listener);

        /// <summary>
        /// Extract data from the web page.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        void Extract(WebPage page);

        /// <summary>
        /// Extract from the web page and return the results as a list.
        /// </summary>
        /// <param name="page">The web page to extract from.</param>
        /// <returns>The results of the extraction as a List.</returns>
        IList<Object> ExtractList(WebPage page);

        /// <summary>
        /// Remove the specified listener.
        /// </summary>
        /// <param name="listener">The listener to rmove.</param>
        void RemoveListener(IExtractListener listener);
    }
}
