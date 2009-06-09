using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.Parse.Tags.Write
{
    /// <summary>
    /// Contains specifics to writing XML.
    /// </summary>
    public class WriteXML : WriteTags
    {
        /// <summary>
        /// Construct an object to write an XML file.
        /// </summary>
        /// <param name="os">The output stream.</param>
        public WriteXML(Stream os): base(os)
        {
        }
    }
}
