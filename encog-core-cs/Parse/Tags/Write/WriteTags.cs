// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if logging

#endif

namespace Encog.Parse.Tags.Write
{
    /// <summary>
    /// Class used to write out tags, such as XML or HTML.
    /// </summary>
    public class WriteTags
    {
        /// <summary>
        /// The output stream to write to.
        /// </summary>
        private readonly Stream output;

        /// <summary>
        /// Stack to keep track of beginning and ending tags.
        /// </summary>
        private readonly Stack<String> tagStack;

        /// <summary>
        /// The attributes for the current tag.
        /// </summary>
        private readonly IDictionary<String, String> attributes;

        /// <summary>
        /// Used to encode strings to bytes.
        /// </summary>
        private readonly StreamWriter encoder;

        /// <summary>
        /// Construct an object to write tags.
        /// </summary>
        /// <param name="output">THe output stream.</param>
        public WriteTags(Stream output)
        {
            this.output = output;
            tagStack = new Stack<String>();
            attributes = new Dictionary<String, String>();
            encoder = new StreamWriter(output);
        }

        /// <summary>
        /// Add an attribute to be written with the next tag.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(String name, String value)
        {
            attributes.Add(name, value);
        }

        /// <summary>
        /// Add CDATA to the output stream. XML allows a large block of unformatted
        /// text to be added as a CDATA tag.
        /// </summary>
        /// <param name="text">The text to add.</param>
        public void AddCDATA(String text)
        {
            var builder = new StringBuilder();
            builder.Append('<');
            builder.Append(TagConst.CDATA_BEGIN);
            builder.Append(text);
            builder.Append(TagConst.CDATA_END);
            builder.Append('>');
            try
            {
                encoder.Write(builder.ToString());
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
        }

        /// <summary>
        /// Add a property as a double. A property is a value enclosed in two tags.
        /// </summary>
        /// <param name="name">The name of the enclosing tags.</param>
        /// <param name="d">The value to store.</param>
        public void AddProperty(String name, double d)
        {
            BeginTag(name);
            AddText("" + d);
            EndTag();
        }

        /// <summary>
        /// Add a property as an integer. A property is a value enclosed in two tags.
        /// </summary>
        /// <param name="name">The name of the enclosing tags.</param>
        /// <param name="i">The value to store.</param>
        public void AddProperty(String name, int i)
        {
            AddProperty(name, "" + i);
        }

        /// <summary>
        /// Add a property as a string. A property is a value enclosed in two tags.
        /// </summary>
        /// <param name="name">The name of the enclosing tags.</param>
        /// <param name="str">The value to store.</param>
        public void AddProperty(String name, String str)
        {
            BeginTag(name);
            AddText(str);
            EndTag();
        }

        /// <summary>
        /// Add text.
        /// </summary>
        /// <param name="text">The text to add.</param>
        public void AddText(String text)
        {
            try
            {
                encoder.Write(text);
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
        }

        /// <summary>
        /// Called to begin the document.
        /// </summary>
        public void BeginDocument()
        {
        }

        /// <summary>
        /// Begin a tag with the specified name.
        /// </summary>
        /// <param name="name">The tag to begin.</param>
        public void BeginTag(String name)
        {
            var builder = new StringBuilder();
            builder.Append("<");
            builder.Append(name);
            if (attributes.Count > 0)
            {
                foreach (String key in attributes.Keys)
                {
                    String value = attributes[key];
                    builder.Append(' ');
                    builder.Append(key);
                    builder.Append('=');
                    builder.Append("\"");
                    builder.Append(value);
                    builder.Append("\"");
                }
            }
            builder.Append(">");

            try
            {
                encoder.Write(builder.ToString());
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
            attributes.Clear();
            tagStack.Push(name);
        }

        /// <summary>
        /// Close this object.
        /// </summary>
        public void Close()
        {
            try
            {
                output.Close();
            }
            catch (Exception e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// End the document.
        /// </summary>
        public void EndDocument()
        {
            encoder.Flush();
        }

        /// <summary>
        /// End the current tag.
        /// </summary>
        public void EndTag()
        {
            if (tagStack.Count < 1)
            {
                throw new ParseError(
                    "Can't create end tag, no beginning tag.");
            }
            String tag = tagStack.Pop();

            var builder = new StringBuilder();
            builder.Append("</");
            builder.Append(tag);
            builder.Append(">");

            try
            {
                encoder.Write(builder.ToString());
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
        }

        /**
	 * Write an array as a property.
	 * @param name The name of the property.
	 * @param array The array to write.
	 * @param len The length of the array to write.
	 */

        public void AddProperty(String name, double[] array, int len)
        {
            if (array != null)
            {
                var str = new StringBuilder();
                for (int i = 0; i < len; i++)
                {
                    if (i != 0)
                        str.Append(' ');
                    str.Append(array[i]);
                }
                AddProperty(name, str.ToString());
            }
        }

        /**
	 * Write an array as a property.
	 * @param name The name of the property.
	 * @param array The array to write.
	 * @param len The length of the array to write.
	 */

        public void AddProperty(String name, int[] array, int len)
        {
            if (array != null)
            {
                var str = new StringBuilder();
                for (int i = 0; i < len; i++)
                {
                    if (i != 0)
                        str.Append(' ');
                    str.Append(array[i]);
                }
                AddProperty(name, str.ToString());
            }
        }


        /// <summary>
        /// End a tag, require that we are ending the specified tag.
        /// </summary>
        /// <param name="name">The tag to be ending.</param>
        public void EndTag(String name)
        {
            if (!tagStack.Peek().Equals(name))
            {
                String str = "End tag mismatch, should be ending: "
                             + tagStack.Peek() + ", but trying to end: " + name
                             + ".";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new ParseError(str);
            }
            else
            {
                EndTag();
            }
        }
    }
}