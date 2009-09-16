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
using System.IO;
#if logging
using log4net;
#endif
namespace Encog.Util
{
    /// <summary>
    /// Directory utilities.
    /// </summary>
    public sealed class DirectoryUtil
    {

        /// <summary>
        /// Default buffer size for read/write operations.
        /// </summary>
        public const int BUFFER_SIZE = 1024;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DirectoryUtil));
#endif

        /// <summary>
        /// Copy the specified file.
        /// </summary>
        /// <param name="source">The file to copy.</param>
        /// <param name="target">The target of the copy.</param>
        public static void CopyFile(String source, String target)
        {
            try
            {
                byte[] buffer = new byte[DirectoryUtil.BUFFER_SIZE];

                // open the files before the copy
                Stream inFile = new FileStream(source, FileMode.Open);
                Stream outFile = new FileStream(target, FileMode.OpenOrCreate);

                // perform the copy
                int packetSize = 0;

                while (packetSize != -1)
                {
                    packetSize = inFile.Read(buffer, 0, buffer.Length);
                    if (packetSize != -1)
                    {
                        outFile.Write(buffer, 0, packetSize);
                    }
                }

                // close the files after the copy
                inFile.Close();
                outFile.Close();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Delete a directory and all children.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        public static void DeleteDirectory(String path)
        {
            if (File.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                foreach (DirectoryInfo element in dir.GetDirectories())
                {
                    DirectoryUtil.DeleteDirectory(element.FullName);

                }
            }
            File.Delete(path);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadStream(Stream istream)
        {
            try
            {
                StringBuilder sb = new StringBuilder(1024);

                byte[] chars = new byte[BUFFER_SIZE];
                int numRead = 0;
                while ((numRead = istream.Read(chars, 0, chars.Length)) > -1)
                {
                    string s = System.Text.ASCIIEncoding.ASCII.GetString(chars);
                    sb.Append(s);
                }

                return sb.ToString();
            }
            catch (IOException e)
            {
#if logging
                DirectoryUtil.LOGGER.Error("Exception", e);
#endif
                throw new EncogError(e);
            }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        /// <param name="filename">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadTextFile(String filename)
        {
            Stream stream = new FileStream(filename, FileMode.Open);
            String result = ReadStream(stream);
            stream.Close();
            return result;
        }
#endif
        /// <summary>
        /// Private constructor.
        /// </summary>
        private DirectoryUtil()
        {
        }

    }

}
