using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

namespace Encog.Parse.Tags.Read
{
    public class ReadXML : ReadTags
    {
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ReadXML));

        /// <summary>
        /// Construct an XML reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        public ReadXML(Stream istream)
            : base(istream)
        {
        }

        /// <summary>
        /// Advance until the specified tag is found.
        /// </summary>
        /// <param name="name">The name of the tag we are looking for.</param>
        /// <param name="beginTag">True if this is a begin tage, false otherwise.</param>
        /// <returns>True if the tag was found.</returns>
        public bool FindTag(String name, bool beginTag)
        {
            while (ReadToTag())
            {
                if (beginTag)
                {
                    if (this.LastTag.Name.Equals(name)
                            && (this.LastTag.TagType == Tag.Type.BEGIN))
                    {
                        return true;
                    }
                }
                else
                {
                    if (this.LastTag.Name.Equals(name)
                            && (this.LastTag.TagType == Tag.Type.END))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Read an integer that is contained between the current position, and the
        /// next tag.
        /// </summary>
        /// <returns>The integer that was found.</returns>
        public int ReadIntToTag()
        {
            try
            {
                String str = ReadTextToTag();
                return int.Parse(str);
            }
            catch (Exception e)
            {
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
                throw new ParseError(e);
            }
        }

        /// <summary>
        /// Read all property data until an end tag, which corrisponds to the current
        /// tag, is found. The properties found will be returned in a map.
        /// </summary>
        /// <returns>The properties found.</returns>
        public IDictionary<String, String> readPropertyBlock()
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            String endingBlock = this.LastTag.Name;

            while (ReadToTag())
            {
                if (this.LastTag.Name.Equals(endingBlock)
                        && (this.LastTag.TagType == Tag.Type.END))
                {
                    break;
                }
                String name = this.LastTag.Name;
                String value = ReadTextToTag().Trim();
                result[name] = value;
            }

            return result;
        }

        /// <summary>
        /// Read all text between the current position and the next tag.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public String ReadTextToTag()
        {
            StringBuilder result = new StringBuilder();
            bool done = false;

            while (!done)
            {
                int ch = Read();
                if ((ch == -1) || (ch == 0))
                {
                    done = true;
                }
                else
                {
                    result.Append((char)ch);
                }

            }
            return result.ToString();
        }
    }
}
