using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.Parse.Tags.Read
{
    /// <summary>
    /// This class is designed to parse HTML documents.  It will parse the
    /// individual tags and text between the tags.
    /// </summary>
    public class ReadHTML: ReadTags
    {
        /// <summary>
        /// Construct a HTML reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        public ReadHTML(Stream istream) : base(istream)
        {
        }
    }
}
