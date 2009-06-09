using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

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
        /// A random number generator.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Used to convert strings into bytes.
        /// </summary>
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

        /// <summary>
        /// Generate a boundary for a multipart form.
        /// </summary>
        /// <returns>The boundary.</returns>
        public static String getBoundary()
        {
            return "---------------------------" + RandomString() + RandomString()
                + RandomString();
        }

        /// <summary>
        /// The boundary used for a multipart post. This field is
        /// null if this is not a multipart form and has a value if
        /// this is a multipart form.
        /// </summary>
        private String boundary;

        /// <summary>
        /// The stream to output the encoded form to.
        /// </summary>
        private Stream os;

        /// <summary>
        /// Keep track of if we're on the first form element.
        /// </summary>
        private bool first;

        /// <summary>
        /// Encode the specified string. This encodes all special
        /// characters.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        private static String Encode(String str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Generate a random string, of a specified length. This
        /// is used to generate the multipart boundary.
        /// </summary>
        /// <returns>A random string.</returns>
        protected static String RandomString()
        {
            return "" + random.NextDouble();
        }

        /// <summary>
        /// Prepare to access either a regular, or multipart, form.
        /// </summary>
        /// <param name="os">The stream to output to.</param>
        /// <param name="boundary">The boundary to be used, or null if this is
        /// not a multipart form.</param>
        public FormUtility(Stream os, String boundary)
        {
            this.os = os;
            this.boundary = boundary;
            this.first = true;
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
            if (this.boundary != null)
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

                byte[] buf = new byte[8192];
                int nread;

                Stream istream = new FileStream(file, FileMode.Open);
                while ((nread = istream.Read(buf, 0, buf.Length)) > 0)
                {
                    this.os.Write(buf, 0, nread);
                }

                Newline();
            }
        }

        /// <summary>
        /// Add a regular text field to either a regular or
        /// multipart form.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        public void Add(String name, String value)
        {
            if (this.boundary != null)
            {
                Boundary();
                WriteName(name);
                Newline();
                Newline();
                Writeln(value);
            }
            else
            {
                if (!this.first)
                {
                    Write("&");
                }
                Write(Encode(name));
                Write("=");
                Write(Encode(value));
            }
            this.first = false;
        }

        /// <summary>
        /// Complete the building of the form.
        /// </summary>
        public void Complete()
        {
            if (this.boundary != null)
            {
                Boundary();
                Writeln("--");
                this.os.Flush();
            }
        }

        /// <summary>
        /// Generate a multipart form boundary.
        /// </summary>
        private void Boundary()
        {
            Write("--");
            Write(this.boundary);
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

            byte[] buffer = encoding.GetBytes(str);
            this.os.Write(buffer, 0, buffer.Length);
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

