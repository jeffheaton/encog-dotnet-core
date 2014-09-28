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
using Encog.Bot;

namespace Encog.Util.File
{
    /// <summary>
    /// Contains several utilities for working with files.
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// Add, or replace a filename base.  A filename base is between an underbar
        /// and the . for the extension.  For example: "myfile_raw.csv", the base is
        /// "raw".
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static FileInfo AddFilenameBase(FileInfo filename, String bs)
        {
            String f = GetFileName(filename);
            String ext = GetFileExt(filename);

            int idx1 = f.LastIndexOf('_');
            int idx2 = f.LastIndexOf(Path.PathSeparator);

            bool remove = false;

            if (idx1 != -1)
            {
                if (idx2 == -1)
                {
                    remove = true;
                }
                else
                {
                    remove = idx1 > idx2;
                }
            }

            if (remove)
            {
                f = f.Substring(0, (idx1) - (0));
            }

            return new FileInfo(f + bs + "." + ext);
        }

        /// <summary>
        /// Get the filename, without extension.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <returns>The file name.</returns>
        public static String GetFileName(FileInfo file)
        {
            String fileName = file.ToString();
            int mid = fileName.LastIndexOf(".");
            if (mid == -1)
            {
                return fileName;
            }
            return fileName.Substring(0, (mid) - (0));
        }

        /// <summary>
        /// Get the file extension.
        /// </summary>
        /// <param name="file">The base file.</param>
        /// <returns>The extension.</returns>
        public static String GetFileExt(FileInfo file)
        {
            String fileName = file.ToString();
            int mid = fileName.LastIndexOf(".");
            if (mid == -1)
                return "";
            return fileName.Substring(mid + 1, (fileName.Length) - (mid + 1));
        }

        /// <summary>
        /// Read a file into a string.
        /// </summary>
        /// <param name="filePath">The file to read.</param>
        /// <returns>The contents of the file.</returns>
        public static String ReadFileAsString(FileInfo filePath)
        {
            var fileData = new StringBuilder(1000);
            TextReader reader = new StreamReader(filePath.OpenRead());
            var buf = new char[1024];
            int numRead;
            while ((numRead = reader.Read(buf, 0, buf.Length)) != -1)
            {
                var readData = new string(buf, 0, numRead);
                fileData.Append(readData);
                buf = new char[1024];
            }
            reader.Close();
            return fileData.ToString();
        }

        /// <summary>
        /// Change a file's extension.
        /// </summary>
        /// <param name="name">The filename to change.</param>
        /// <param name="ext">The new extension.</param>
        /// <returns></returns>
        public static String ForceExtension(String name, String ext)
        {
            String b = GetFileName(new FileInfo(name));
            return b + "." + ext;
        }

        /// <summary>
        /// Write a string to a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="str"></param>
        public static void WriteFileAsString(FileInfo path, String str)
        {
            FileStream fs = path.Create();
            var writer = new StreamWriter(fs);
            writer.Write(str);
            writer.Close();
            fs.Close();
        }

        /// <summary>
        /// Copy from one file to another.
        /// </summary>
        /// <param name="source">The source file.</param>
        /// <param name="target">The target file.</param>
        public static void Copy(FileInfo source, FileInfo target)
        {
            try
            {
                target.Delete();
                FileStream fos = target.OpenWrite();
                Stream mask0 = source.OpenRead();

                Copy(mask0, fos);

                fos.Close();
                mask0.Close();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Copy from one stream to another.
        /// </summary>
        /// <param name="mask0">The source.</param>
        /// <param name="os">The target.</param>
        public static void Copy(Stream mask0, Stream os)
        {
            try
            {
                var buffer = new byte[BotUtil.BufferSize];
                int length;
                do
                {
                    length = mask0.Read(buffer, 0, buffer.Length);

                    if (length > 0)
                    {
                        os.Write(buffer, 0, length);
                    }
                } while (length > 0);
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        /// <summary>
        /// Copy a resource to the file system.
        /// </summary>
        /// <param name="resource">The resource to copy.</param>
        /// <param name="targetFile">There to copy the resource to.</param>
        public static void CopyResource(String resource, FileInfo targetFile)
        {
            try
            {
                Stream mask0 = ResourceLoader.CreateStream(resource);
                targetFile.Delete();
                Stream os = targetFile.OpenWrite();
                Copy(mask0, os);
                mask0.Close();
                os.Close();
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        /// <summary>
        /// Combine a path with a filename.
        /// </summary>
        /// <param name="dir">The path to combine.</param>
        /// <param name="f">The filename to combine.</param>
        /// <returns>The resulting path.</returns>
        public static FileInfo CombinePath(FileInfo dir, string f)
        {
            string s = dir.ToString();
            return new FileInfo(Path.Combine(s, f));
        }

        /// <summary>
        /// Copy a stream from one 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static int CopyStream(Stream input, Stream output)
        {
            int result = 0;

            var buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return result;
                output.Write(buffer, 0, read);
                result += read;
            }
        }



        /// <summary>
        ///Move a file or directory to a destination path.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        /// <returns>true if we were able to move the file or directory.</returns>
        public static bool MoveFileOrDirectory(string fromPath, string toPath)
        {
            if (System.IO.File.Exists(fromPath))
            {
                System.IO.File.Move(fromPath, toPath);
                return true;
            }
            if (System.IO.Directory.Exists(fromPath))
            {
                System.IO.Directory.Move(fromPath, toPath);
                return true;
            }
            return false;
        }

        public static String ToStringLiteral(FileInfo processFile)
        {
            String str = processFile.ToString();
            StringBuilder result = new StringBuilder();

            foreach (char ch in str)
            {
                if (ch == '\\')
                {
                    result.Append("\\\\");
                }
                else
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }
    }
}
