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
