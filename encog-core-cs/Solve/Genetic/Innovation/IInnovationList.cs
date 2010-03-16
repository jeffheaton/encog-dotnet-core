using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    public interface IInnovationList
    {
        /**
	 * Add an innovation.
	 * @param innovation The innovation added.
	 */
        void add(IInnovation innovation);

        /**
         * Get the innovation specified by index.
         * @param id The index.
         * @return The innovation.
         */
        IInnovation get(int id);

        /**
         * @return A list of innovations.
         * @return The innovation list.
         */
        List<IInnovation> getInnovations();
    }
}
