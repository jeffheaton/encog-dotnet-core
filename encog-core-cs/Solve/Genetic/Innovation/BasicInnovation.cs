using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Solve.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for an innovation.
    /// </summary>
    public class BasicInnovation: IInnovation
    {
        /// <summary>
        /// The innovation id.
        /// </summary>
        [EGAttribute]
        private long innovationID;

        /// <summary>
        /// The innovation id.
        /// </summary>
        public long InnovationID 
        {
            get
            {
                return this.innovationID;
            }
            set
            {
                this.innovationID = value;
            }
        }
    }
}
