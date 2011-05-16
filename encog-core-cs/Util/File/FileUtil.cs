using System;
using System.IO;
using System.Text;
using Encog.Bot;

namespace Encog.Util.File
{
    public class FileUtil
    {
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

        public static String GetFileExt(FileInfo file)
        {
            String fileName = file.ToString();
            int mid = fileName.LastIndexOf(".");
            if (mid == -1)
                return "";
            return fileName.Substring(mid + 1, (fileName.Length) - (mid + 1));
        }

        public static String ReadFileAsString(FileInfo filePath)
        {
            var fileData = new StringBuilder(1000);
            TextReader reader = new StreamReader(filePath.OpenWrite());
            var buf = new char[1024];
            int numRead = 0;
            while ((numRead = reader.Read(buf, 0, buf.Length)) != -1)
            {
                var readData = new string(buf, 0, numRead);
                fileData.Append(readData);
                buf = new char[1024];
            }
            reader.Close();
            return fileData.ToString();
        }

        public static String ForceExtension(String name, String ext)
        {
            String b = GetFileName(new FileInfo(name));
            return b + "." + ext;
        }

        public static void WriteFileAsString(FileInfo path, String str)
        {
            FileStream fs = path.OpenRead();
            var writer = new StreamWriter(fs);
            writer.Write(str);
            writer.Close();
            fs.Close();
        }

        public static void Copy(FileInfo source, FileInfo target)
        {
            try
            {
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

        public static void Copy(Stream mask0, Stream os)
        {
            try
            {
                var buffer = new byte[BotUtil.BUFFER_SIZE];
                int length;
                do
                {
                    length = mask0.Read(buffer, 0, buffer.Length);

                    if (length >= 0)
                    {
                        os.Write(buffer, 0, length);
                    }
                } while (length >= 0);
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        public static void CopyResource(String resource, FileInfo targetFile)
        {
            try
            {
                Stream mask0 = ResourceInputStream
                    .OpenResourceInputStream(resource);
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
    }
}