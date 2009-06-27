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
        private String original;

        /// <summary>
        /// The address as a URL.
        /// </summary>
        private Uri url;

        /// <summary>
        /// Construct the address from a URL.
        /// </summary>
        /// <param name="u">The URL to use.</param>
        public Address(Uri u)
        {
            this.url = u;
            this.original = u.ToString();
        }

        /// <summary>
        /// Construct a URL using a perhaps relative URL and a base URL.
        /// </summary>
        /// <param name="b">The base URL.</param>
        /// <param name="original">A full URL or a URL relative to the base.</param>
        public Address(Uri b, String original)
        {
            this.original = original;
            this.url = new Uri(b, original);

        }

        /// <summary>
        /// The original text from this URL.
        /// </summary>
        public String Original
        {
            get
            {
                return this.original;
            }
        }

        /// <summary>
        /// The URL.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }
        }

        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (this.url != null)
            {
                return this.url.ToString();
            }
            else
            {
                return this.original;
            }
        }

    }

}
