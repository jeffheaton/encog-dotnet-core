using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Parse.Tags
{
    /// <summary>
    /// Constants to use while parsing the tags.
    /// </summary>
    public class TagConst
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private TagConst()
        {
        }

        /// <summary>
        /// The beginning of a comment.
        /// </summary>
        public const String COMMENT_BEGIN = "!--";

        /// <summary>
        /// The end of a comment.
        /// </summary>
        public const String COMMENT_END = "-->";

        /// <summary>
        /// The beginning of a CDATA section.
        /// </summary>
        public const String CDATA_BEGIN = "![CDATA[";

        /// <summary>
        /// The end of a CDATA section.
        /// </summary>
        public const String CDATA_END = "]]";
    }
}
