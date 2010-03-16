using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    public interface IInnovation
    {
        /**
	 * @return The innovation id.
	 */
        long getInnovationID();

        /**
         * Set the innovation id.
         * @param innovationID The innovation id.
         */
        void setInnovationID(long innovationID);

    }
}
