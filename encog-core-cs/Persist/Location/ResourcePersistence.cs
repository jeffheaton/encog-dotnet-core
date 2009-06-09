using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using System.Resources;

namespace Encog.Persist.Location
{
    /// <summary>
    /// A location that allows Encog objects to be read from a resource. This
    /// location only supports read operations, so the Encog resource is usually
    /// created first as a file and then embedded in the application as a resource.
    /// </summary>
    public class ResourcePersistence : PersistenceLocation
    {

        /// <summary>
        /// The name of the resource to read from.
        /// </summary>
        private String resource;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ResourcePersistence));

        /// <summary>
        /// Construct a location to read from the specified resource. 
        /// </summary>
        /// <param name="resource">The resource to read from.</param>
        public ResourcePersistence(String resource)
        {
            this.resource = resource;
        }

        /// <summary>
        /// Create a stream to read the resource.
        /// </summary>
        /// <returns>A stream.</returns>
        public Stream CreateStream()
        {
            String filePath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            ResourceReader reader = new ResourceReader(filePath);
            string type;
            byte[] data;
            reader.GetResourceData(this.resource,out type,out data);
            return new MemoryStream(data);
        }

        /// <summary>
        /// Delete operations are not supported for resource persistence.
        /// </summary>
        public void Delete()
        {
            String str =
            "The ResourcePersistence location does not suppor delete operations.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
            throw new PersistError(str);

        }

        /// <summary>
        /// Exist is not supported for resource persistence.
        /// </summary>
        /// <returns>Nothing.</returns>
        public bool Exists()
        {
            String str =
            "The ResourcePersistence location does not suppor exists.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
            throw new PersistError(str);
        }

        /// <summary>
        /// Rename is not supported for resource persistence.
        /// </summary>
        /// <param name="toLocation">Not used.</param>
        public void RenameTo(PersistenceLocation toLocation)
        {
            String str =
           "The ResourcePersistence location does not suppor rename operations.";
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
            throw new PersistError(str);
        }

    }
}
