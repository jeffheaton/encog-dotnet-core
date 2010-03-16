using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for a list of innovations.
    /// </summary>
    public class BasicInnovationList: IInnovationList
    {
        /// <summary>
        /// The list of innovations.
        /// </summary>
        private IList<IInnovation> list = new List<IInnovation>();

        /// <summary>
        /// The innovations.
        /// </summary>
        public IList<IInnovation> Innovations 
        {
            get
            {
                return this.list;
            }
        }
    }
}
