using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Allows training to contune.
    /// </summary>
    public class TrainingContinuation : IEncogPersistedObject
    {
        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The description of this object.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The contents of this object.
        /// </summary>
        private IDictionary<String, Object> contents = new Dictionary<String, Object>();

        /// <summary>
        /// Obtain a persistor for this object.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        public IPersistor CreatePersistor()
        {
            return new TrainingContinuationPersistor();
        }

        /// <summary>
        /// Get the specified object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The object.</returns>
        public Object this[String key]
        {
            get
            {
                return this.contents[key];
            }
            set
            {
                this.contents[key] = value;
            }
        }

        /// <summary>
        /// The contents.
        /// </summary>
        public IDictionary<String, Object> Contents
        {
            get
            {
                return this.contents;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Not supported.</returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
