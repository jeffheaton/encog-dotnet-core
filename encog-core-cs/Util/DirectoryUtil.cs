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
#if logging

#endif

namespace Encog.Util
{
    /// <summary>
    /// Directory utilities.
    /// </summary>
    public static class DirectoryUtil
    {
        /// <summary>
        /// Default buffer size for read/write operations.
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// Copy the specified file.
        /// </summary>
        /// <param name="source">The file to copy.</param>
        /// <param name="target">The target of the copy.</param>
        public static void CopyFile(String source, String target)
        {
            try
            {
                var buffer = new byte[BufferSize];

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
        /// Read the entire contents of a stream into a string.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadStream(Stream istream)
        {
            try
            {
                var sb = new StringBuilder(1024);

                var chars = new byte[BufferSize];
                while ((istream.Read(chars, 0, chars.Length)) > -1)
                {
                    string s = Encoding.ASCII.GetString(chars);
                    sb.Append(s);
                }

                return sb.ToString();
            }
            catch (IOException e)
            {
#if logging
                LOGGER.Error("Exception", e);
#endif
                throw new EncogError(e);
            }
        }

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
    }
}
