using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace Encog.Persist.Location
{
    /**
     * Create a location based on a Stream.
     */
    public class StreamPersistence : IPersistenceLocation
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));
#endif
        /// <summary>
        /// The stream to persist to/from.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// Construct a persistance location based on a stream.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public StreamPersistence(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// The stream to use.
        /// </summary>
        /// <returns>A new InputStream for this file.</returns>
        public Stream CreateStream(FileMode mode)
        {
            return this.stream;
        }

        /// <summary>
        /// Attempt to delete the file.  Will fail for this location type.
        /// </summary>
        public void Delete()
        {
        }

        /// <summary>
        /// Does the file exist?  This is always true for this location type.
        /// </summary>
        /// <returns>True if the file exists.</returns>
        public bool Exists()
        {
            return true;
        }


        /// <summary>
        /// Rename this file to a different location.
        /// </summary>
        /// <param name="toLocation">What to rename to.</param>
        public void RenameTo(IPersistenceLocation toLocation)
        {
                String str =
                   "Rename is not supported on this location type.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PersistError(str);
 
        }
    }
}
