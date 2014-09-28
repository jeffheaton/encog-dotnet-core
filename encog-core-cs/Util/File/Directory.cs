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
using System.Text;

namespace Encog.Util.File
{
    /// <summary>
    /// Directory utilities.
    /// </summary>
    ///
    public static class Directory
    {
        /// <summary>
        /// Default buffer size for read/write operations.
        /// </summary>
        ///
        public const int BufferSize = 1024;

        /// <summary>
        /// Copy the specified file.
        /// </summary>
        ///
        /// <param name="source">The file to copy.</param>
        /// <param name="target">The target of the copy.</param>
        public static void CopyFile(FileInfo source, FileInfo target)
        {
            try
            {
                var buffer = new byte[BufferSize];

                // open the files before the copy
                FileStream
                    ins0 = source.OpenRead();
				target.Delete();
                FileStream xout = target.OpenWrite();

                // perform the copy
                int packetSize = 0;

                while (packetSize != -1)
                {
                    packetSize = ins0.Read(buffer, 0, buffer.Length);
                    if (packetSize != -1)
                    {
                        xout.Write(buffer, 0, packetSize);
                    }
                }

                // close the files after the copy
                ins0.Close();
                xout.Close();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Delete a directory and all children.
        /// </summary>
        ///
        /// <param name="path">The path to delete.</param>
        /// <returns>True if successful.</returns>
        public static bool DeleteDirectory(FileInfo path)
        {
            return DeleteDirectory(path);
        }

        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        ///
        /// <param name="mask0">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadStream(Stream mask0)
        {
            try
            {
                var sb = new StringBuilder(1024);
                TextReader reader = new StreamReader(mask0);

                var chars = new char[BufferSize];
                int numRead;
                while ((numRead = reader.Read(chars, 0, chars.Length)) > -1)
                {
                    sb.Append(new String(chars, 0, numRead));
                }
                reader.Close();

                return sb.ToString();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        ///
        /// <param name="filename">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadTextFile(String filename)
        {
            try
            {
                var result = new StringBuilder();
                using (var sr = new StreamReader(filename))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        result.Append(line);
                        result.Append("\r\n");
                    }
                }
                return result.ToString();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }
    }
}
