// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using log4net;
using Encog.Persist.Location;
using Encog.Persist.Persistors;

namespace Encog.Parse.Recognize
{
    /// <summary>
    /// Allows templates to be specified for the parser.
    /// </summary>
    public class ParseTemplate : IEncogPersistedObject
    {

        private ICollection<Recognize> recognizers = new List<Recognize>();
        private String name;
        private String description;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FilePersistence));

        /// <summary>
        /// Add a recognizer.
        /// </summary>
        /// <param name="recognize">The recognizer to add.</param>
        public void AddRecognizer(Recognize recognize)
        {
            recognizers.Add(recognize);
        }

        /// <summary>
        /// Create a recognizer.
        /// </summary>
        /// <param name="name">The name of the recognizer.</param>
        /// <returns>The recognizer that was created.</returns>
        public Recognize CreateRecognizer(String name)
        {
            Recognize result = new Recognize(name);
            AddRecognizer(result);
            return result;
        }

        /// <summary>
        /// The recognizers to use.
        /// </summary>
        public ICollection<Recognize> Recognizers
        {
            get
            {
                return recognizers;
            }
        }

        /// <summary>
        /// Create a persistor to load or save this object.
        /// </summary>
        /// <returns>The Encog persistor.</returns>
        public IPersistor CreatePersistor()
        {
            return new ParseTemplatePersistor();
        }

        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }


        /// <summary>
        /// The description.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }


        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Object Clone()
        {
            ParseTemplate result = new ParseTemplate();
            result.Name = this.Name;
            result.Description = this.Description;
            return result;
        }
    }
}
