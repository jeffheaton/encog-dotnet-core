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
        private readonly Stream _output;

        /// <summary>
        /// Stack to keep track of beginning and ending tags.
        /// </summary>
        private readonly Stack<String> _tagStack;

        /// <summary>
        /// The attributes for the current tag.
        /// </summary>
        private readonly IDictionary<String, String> _attributes;

        /// <summary>
        /// Used to encode strings to bytes.
        /// </summary>
        private readonly StreamWriter _encoder;

        /// <summary>
        /// Construct an object to write tags.
        /// </summary>
        /// <param name="output">THe output stream.</param>
        public WriteTags(Stream output)
        {
            _output = output;
            _tagStack = new Stack<String>();
            _attributes = new Dictionary<String, String>();
            _encoder = new StreamWriter(output);
        }

        /// <summary>
        /// Add an attribute to be written with the next tag.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="v">The value of the attribute.</param>
        public void AddAttribute(String name, String v)
        {
            _attributes.Add(name, v);
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
            builder.Append(TagConst.CDATABegin);
            builder.Append(text);
            builder.Append(TagConst.CDATAEnd);
            builder.Append('>');
            try
            {
                _encoder.Write(builder.ToString());
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
                _encoder.Write(text);
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
            if (_attributes.Count > 0)
            {
                foreach (String key in _attributes.Keys)
                {
                    String value = _attributes[key];
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
                _encoder.Write(builder.ToString());
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
            _attributes.Clear();
            _tagStack.Push(name);
        }

        /// <summary>
        /// Close this object.
        /// </summary>
        public void Close()
        {
            try
            {
                _output.Close();
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
            _encoder.Flush();
        }

        /// <summary>
        /// End the current tag.
        /// </summary>
        public void EndTag()
        {
            if (_tagStack.Count < 1)
            {
                throw new ParseError(
                    "Can't create end tag, no beginning tag.");
            }
            String tag = _tagStack.Pop();

            var builder = new StringBuilder();
            builder.Append("</");
            builder.Append(tag);
            builder.Append(">");

            try
            {
                _encoder.Write(builder.ToString());
            }
            catch (IOException e)
            {
                throw new ParseError(e);
            }
        }
        
        /// <summary>
        /// Write an array as a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="len">The length of the array to write.</param>
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

        /// <summary>
        /// Write an array as a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="array">The array to write.</param>
        /// <param name="len">The length of the array to write.</param>
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
            if (!_tagStack.Peek().Equals(name))
            {
                String str = "End tag mismatch, should be ending: "
                             + _tagStack.Peek() + ", but trying to end: " + name
                             + ".";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new ParseError(str);
            }
            EndTag();
        }
    }
}
