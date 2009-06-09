using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

namespace Encog.Persist.Location
{
    /// <summary>
    /// A persistence location based on a file.
    /// </summary>
    public class FilePersistence : PersistenceLocation
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        /// <summary>
        /// The file to persist to/from.
        /// </summary>
        private String file;

        /// <summary>
        /// The mode this streamed is opened
        /// </summary>
        private FileMode mode;

        /// <summary>
        /// Construct a persistance location based on a file.
        /// </summary>
        /// <param name="file">The file to use.</param>
        /// <param name="mode">The mode to open this file in.</param>
        public FilePersistence(String file, FileMode mode)
        {
            this.file = file;
            this.mode = mode;
        }

        /// <summary>
        /// Create a stream to a access the file.
        /// </summary>
        /// <returns>A new InputStream for this file.</returns>
        public Stream CreateStream()
        {
            try
            {
                return new FileStream(this.file, this.mode);
            }
            catch (IOException e)
            {
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
                throw new PersistError(e);
            }
        }

        /// <summary>
        /// Attempt to delete the file.
        /// </summary>
        public void Delete()
        {
            File.Delete(this.file);
        }

        /// <summary>
        /// Does the file exist?
        /// </summary>
        /// <returns>True if the file exists.</returns>
        public bool Exists()
        {
            return File.Exists(this.file);
        }

        /// <summary>
        /// The file this location is based on.
        /// </summary>
        public String FileName
        {
            get
            {
                return this.file;
            }
        }

        /// <summary>
        /// Rename this file to a different location.
        /// </summary>
        /// <param name="toLocation">What to rename to.</param>
        public void RenameTo(PersistenceLocation toLocation)
        {
            if (!(toLocation is FilePersistence))
            {
                String str =
                   "Can only rename from one FilePersistence location to another";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PersistError(str);
            }

            String toFile = ((FilePersistence)toLocation).FileName;

            File.Move(this.file, toFile);
        }
    }
}
