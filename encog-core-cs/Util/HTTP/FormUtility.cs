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
using System.Web;
using Encog.MathUtil;

namespace Encog.Util.HTTP
{
    /// <summary>
    /// FormUtility: This class is used to construct responses to
    /// HTML forms. The class supports both standard HTML forms,
    /// as well as multipart forms.
    /// </summary>
    public class FormUtility
    {
        /// <summary>
        /// The boundary used for a multipart post. This field is
        /// null if this is not a multipart form and has a value if
        /// this is a multipart form.
        /// </summary>
        private readonly String _boundary;

        /// <summary>
        /// The stream to output the encoded form to.
        /// </summary>
        private readonly Stream _os;

        /// <summary>
        /// The text writer to use.
        /// </summary>
        private readonly TextWriter _writer;

        /// <summary>
        /// Keep track of if we're on the first form element.
        /// </summary>
        private bool _first;

        /// <summary>
        /// Prepare to access either a regular, or multipart, form.
        /// </summary>
        /// <param name="os">The stream to output to.</param>
        /// <param name="boundary">The boundary to be used, or null if this is
        /// not a multipart form.</param>
        public FormUtility(Stream os, String boundary)
        {
            _os = os;
            _writer = new StreamWriter(os);
            _boundary = boundary;
            _first = true;
        }

        /// <summary>
        /// Generate a boundary for a multipart form.
        /// </summary>
        /// <returns>The boundary.</returns>
        public static String GetBoundary()
        {
            return "---------------------------" + RandomString() + RandomString()
                   + RandomString();
        }

        /// <summary>
        /// Encode the specified string. This encodes all special
        /// characters.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static String Encode(String str)
        {
            //return HttpUtility.HtmlEncode(str);
            return str;
        }

        /// <summary>
        /// Generate a random string, of a specified length. This
        /// is used to generate the multipart boundary.
        /// </summary>
        /// <returns>A random string.</returns>
        protected static String RandomString()
        {
            return "" + ThreadSafeRandom.NextDouble();
        }

        /// <summary>
        /// Add a file to a multipart form.  Default mime type to
        /// application/octet-stream.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="file">The file to attach.</param>
        public void AddFile(String name, String file)
        {
            AddFile(name, file, "application/octet-stream");
        }

        /// <summary>
        /// Add a file to a multipart form.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="file">he file to attach.</param>
        /// <param name="type">The mime type</param>
        public void AddFile(String name, String file, String type)
        {
            if (_boundary != null)
            {
                Boundary();
                WriteName(name);
                Write("; filename=\"");
                Write(file);
                Write("\"");
                Newline();
                Write("Content-Type: ");

                Writeln(type);
                Newline();

                var buf = new byte[8192];
                int nread;

                _writer.Flush();
                _os.Flush();

                Stream istream = new FileStream(file, FileMode.Open);
                while ((nread = istream.Read(buf, 0, buf.Length)) > 0)
                {
                    _os.Write(buf, 0, nread);
                }

                _os.Flush();
                Newline();
            }
        }

        /// <summary>
        /// Add a regular text field to either a regular or
        /// multipart form.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="v">The value of the field.</param>
        public void Add(String name, String v)
        {
            if (_boundary != null)
            {
                Boundary();
                WriteName(name);
                Newline();
                Newline();
                Writeln(v);
            }
            else
            {
                if (!_first)
                {
                    Write("&");
                }
                Write(Encode(name));
                Write("=");
                Write(Encode(v));
            }
            _first = false;
        }

        /// <summary>
        /// Complete the building of the form.
        /// </summary>
        public void Complete()
        {
            if (_boundary != null)
            {
                Boundary();
                Writeln("--");
                _os.Flush();
            }
        }

        /// <summary>
        /// Generate a multipart form boundary.
        /// </summary>
        private void Boundary()
        {
            Write("--");
            Write(_boundary);
        }

        /// <summary>
        /// Create a new line by displaying a carriage return and
        /// linefeed.
        /// </summary>
        private void Newline()
        {
            Write("\r\n");
        }

        /// <summary>
        /// Write the specified string, without a carriage return
        /// and line feed.
        /// </summary>
        /// <param name="str">The string to write.</param>
        private void Write(String str)
        {
            _writer.Write(str);
            _writer.Flush();
        }

        /// <summary>
        /// Write the name element for a multipart post.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        private void WriteName(String name)
        {
            Newline();
            Write("Content-Disposition: form-data; name=\"");
            Write(name);
            Write("\"");
        }

        /// <summary>
        /// Write a string, with a carriage return and linefeed.
        /// </summary>
        /// <param name="str">The string to write.</param>
        protected void Writeln(String str)
        {
            Write(str);
            Newline();
        }
    }
}
