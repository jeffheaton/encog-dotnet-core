// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
#if logging
using log4net;
#endif
namespace Encog.Persist.Location
{
    /// <summary>
    /// A location that allows Encog objects to be read from a resource. This
    /// location only supports read operations, so the Encog resource is usually
    /// created first as a file and then embedded in the application as a resource.
    /// </summary>
    public class ResourcePersistence : IPersistenceLocation
    {

        /// <summary>
        /// The name of the resource to read from.
        /// </summary>
        private String resource;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ResourcePersistence));
#endif

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
        public Stream CreateStream(FileMode mode)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(this.resource);            
            return stream;
        }

        /// <summary>
        /// Delete operations are not supported for resource persistence.
        /// </summary>
        public void Delete()
        {
            String str =
            "The ResourcePersistence location does not suppor delete operations.";
#if logging   
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
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
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);
        }

        /// <summary>
        /// Rename is not supported for resource persistence.
        /// </summary>
        /// <param name="toLocation">Not used.</param>
        public void RenameTo(IPersistenceLocation toLocation)
        {
            String str =
           "The ResourcePersistence location does not suppor rename operations.";
#if logging            
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PersistError(str);
        }

        /// <summary>
        /// Load a string.
        /// </summary>
        /// <returns>The loaded string.</returns>
        public String LoadString()
        {
            StringBuilder result = new StringBuilder();
            Stream istream = CreateStream(FileMode.Open);
            StreamReader sr = new StreamReader(istream);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                result.Append(line);
                result.Append("\r\n");
            }
            sr.Close();
            istream.Close();

            return result.ToString();
        }

    }
}
