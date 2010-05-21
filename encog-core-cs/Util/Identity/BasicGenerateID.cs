using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Util.Identity
{
    /// <summary>
    /// Generate unique id's.  ID's start at 1.
    /// </summary>
    public class BasicGenerateID : IGenerateID
    {
        /// <summary>
        /// The current id to generate.  This is the next id returned.
        /// </summary>
        [@EGAttribute]
        private long currentID;

        /// <summary>
        /// Construct an ID generator.
        /// </summary>
        public BasicGenerateID()
        {
            this.currentID = 1;
        }

        /// <summary>
        /// Generate a unique id.
        /// </summary>
        /// <returns>The unique id.</returns>
        public long Generate()
        {
            lock (this)
            {
                return this.currentID++;
            }
        }
    }
}
