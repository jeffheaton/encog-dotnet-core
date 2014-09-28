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
using System.Text;

namespace Encog.Bot.Browse.Range
{
    /// <summary>
    /// A document range that specifies a span tag, and any embedded tags.
    /// </summary>
    public class Span : DocumentRange
    {
        /// <summary>
        /// Construct a span range from the specified web page.
        /// </summary>
        /// <param name="source">The source web page.</param>
        public Span(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string. 
        /// </summary>
        /// <returns>This object as a string. </returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Span:class=");
            result.Append(ClassAttribute);
            result.Append(",id=");
            result.Append(IdAttribute);
            result.Append(",elements=");
            result.Append(Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }
}
