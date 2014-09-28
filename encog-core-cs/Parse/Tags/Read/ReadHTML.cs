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
using System.IO;

namespace Encog.Parse.Tags.Read
{
    /// <summary>
    /// This class is designed to parse HTML documents.  It will parse the
    /// individual tags and text between the tags.
    /// </summary>
    public class ReadHTML : ReadTags
    {
        /// <summary>
        /// Construct a HTML reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        public ReadHTML(Stream istream) : base(istream)
        {
        }

        /// <summary>
        /// Parse the attribute name.
        /// </summary>
        /// <returns>The attribute name.</returns>
        protected String ParseAttributeName()
        {
            String result = base.ParseAttributeName();
            return result.ToLower();
        }
    }
}
