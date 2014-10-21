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

namespace Encog.Bot.Browse
{
    /// <summary>
    /// A URL address. Holds both the URL object, as well as original text.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The original text from the address.
        /// </summary>
        private readonly String _original;

        /// <summary>
        /// The address as a URL.
        /// </summary>
        private readonly Uri _url;

        /// <summary>
        /// Construct the address from a URL.
        /// </summary>
        /// <param name="u">The URL to use.</param>
        public Address(Uri u)
        {
            _url = u;
            _original = u.ToString();
        }

        /// <summary>
        /// Construct a URL using a perhaps relative URL and a base URL.
        /// </summary>
        /// <param name="b">The base URL.</param>
        /// <param name="original">A full URL or a URL relative to the base.</param>
        public Address(Uri b, String original)
        {
            _original = original;
            _url = b == null ? new Uri(new Uri("http://localhost/"), original) : new Uri(b, original);
        }

        /// <summary>
        /// The original text from this URL.
        /// </summary>
        public String Original
        {
            get { return _original; }
        }

        /// <summary>
        /// The URL.
        /// </summary>
        public Uri Url
        {
            get { return _url; }
        }

        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return _url != null ? _url.ToString() : _original;
        }
    }
}
