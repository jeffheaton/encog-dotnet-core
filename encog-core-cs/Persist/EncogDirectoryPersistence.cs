using System;
using System.IO;
using System.Text;
using Encog.Util.Logging;

namespace Encog.Persist
{
    /// <summary>
    /// Handles Encog persistence for a directory. This is the usual mode where each
    /// resource is stored in a separate EG file.
    /// </summary>
    ///
    public class EncogDirectoryPersistence
    {
        /// <summary>
        /// The directory that holds the EG files.
        /// </summary>
        ///
        private readonly FileInfo parent;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="parent_0">The directory to use.</param>
        public EncogDirectoryPersistence(FileInfo parent_0)
        {
            parent = parent_0;
        }

        /// <value>The directory.</value>
        public FileInfo Parent
        {
            /// <returns>The directory.</returns>
            get { return parent; }
        }

        /// <summary>
        /// Load the specified object.
        /// </summary>
        ///
        /// <param name="file">The file to load.</param>
        /// <returns>The loaded object.</returns>
        public static Object LoadObject(FileInfo file)
        {
            FileStream fis = null;

            try
            {
                fis = file.OpenRead();
                Object result = LoadObject(fis);

                return result;
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
            finally
            {
                if (fis != null)
                {
                    try
                    {
                        fis.Close();
                    }
                    catch (IOException e)
                    {
                        EncogLogging.Log(e);
                    }
                }
            }
        }

        /// <summary>
        /// Load an object from an input stream.
        /// </summary>
        ///
        /// <param name="is">The input stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public static Object LoadObject(Stream mask0)
        {
            String header = ReadLine(mask0);
            String[] paras = header.Split(',');

            if (!"encog".Equals(paras[0]))
            {
                throw new PersistError("Not a valid EG file.");
            }

            String name = paras[1];

            EncogPersistor p = PersistorRegistry.Instance.GetPersistor(
                name);

            if (p == null)
            {
                throw new PersistError("Do not know how to read the object: "
                                       + name);
            }

            if (p.FileVersion < Int32.Parse(paras[4]))
            {
                throw new PersistError(
                    "The file you are trying to read is from a later version of Encog.  Please upgrade Encog to read this file.");
            }

            return p.Read(mask0);
        }

        /// <summary>
        /// Read a line from the input stream.
        /// </summary>
        ///
        /// <param name="is">The input stream.</param>
        /// <returns>The line read.</returns>
        private static String ReadLine(Stream mask0)
        {
            try
            {
                var result = new StringBuilder();

                char ch;

                do
                {
                    int b = mask0.ReadByte();
                    if (b == -1)
                    {
                        return result.ToString();
                    }

                    ch = (char) b;

                    if ((ch != 13) && (ch != 10))
                    {
                        result.Append(ch);
                    }
                } while (ch != 10);

                return result.ToString();
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        ///
        /// <param name="filename">The filename to save to.</param>
        /// <param name="obj">The Object to save.</param>
        public static void SaveObject(FileInfo filename, Object obj)
        {
            FileStream fos = null;

            try
            {
				filename.Delete();
                fos = filename.OpenWrite();
                SaveObject(fos, obj);
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
            finally
            {
                try
                {
                    if (fos != null)
                    {
                        fos.Close();
                    }
                }
                catch (IOException e)
                {
                    EncogLogging.Log(e);
                }
            }
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        ///
        /// <param name="os">The output stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public static void SaveObject(Stream os, Object obj)
        {
            try
            {
                EncogPersistor p = PersistorRegistry.Instance
                    .GetPersistor(obj.GetType());

                if (p == null)
                {
                    throw new PersistError("Do not know how to persist object: "
                                           + obj.GetType().Name);
                }

                os.Flush();
                var pw = new StreamWriter(os);
                DateTime now = DateTime.Now;
                pw.WriteLine("encog," + obj.GetType().Name + ",java,"
                             + EncogFramework.VERSION + "," + p.FileVersion + ","
                             + (now.Ticks/10000));
                pw.Flush();
                p.Save(os, obj);
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Get the type of an Encog object in an EG file, without the 
        /// need to read the entire file.
        /// </summary>
        ///
        /// <param name="name">The filename to read.</param>
        /// <returns>The type.</returns>
        public String GetEncogType(String name)
        {
            try
            {
                var path = new FileInfo(Path.Combine(parent.FullName, name));
                TextReader br = new StreamReader(path.OpenRead());
                String header = br.ReadLine();
                String[] paras = header.Split(',');
                br.Close();
                return paras[1];
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }


        /// <summary>
        /// Load a file from the directory that this object refers to.
        /// </summary>
        ///
        /// <param name="name">The name to load.</param>
        /// <returns>The object.</returns>
        public Object LoadFromDirectory(String name)
        {
            var path = new FileInfo(Path.Combine(parent.FullName, name));
            return LoadObject(path);
        }

        /// <summary>
        /// Save a file to the directory that this object refers to.
        /// </summary>
        ///
        /// <param name="name">The name to load.</param>
        public void SaveToDirectory(String name, Object obj)
        {
            var path = new FileInfo(Path.Combine(parent.FullName, name));
            SaveObject(path, obj);
        }
    }
}