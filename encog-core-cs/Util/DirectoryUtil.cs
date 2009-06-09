using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

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

        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DirectoryUtil));

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
                DirectoryUtil.LOGGER.Error("Exception", e);
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

        /// <summary>
        /// Private constructor.
        /// </summary>
        private DirectoryUtil()
        {
        }

    }

}
