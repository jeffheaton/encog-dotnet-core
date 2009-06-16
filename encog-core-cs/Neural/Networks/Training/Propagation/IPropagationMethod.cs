using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Defines the specifics to one of the propagation methods. The individual ways
    /// that each of the propagation methods uses to modify the weight and] threshold
    /// matrix are defined here.
    /// </summary>
    public interface IPropagationMethod
    {
        /// <summary>
        /// Calculate the error between these two levels.
        /// </summary>
        /// <param name="output">The output to the "to level".</param>
        /// <param name="fromLevel">The from level.</param>
        /// <param name="toLevel">The target level.</param>
        void CalculateError(NeuralOutputHolder output,
                 PropagationLevel fromLevel,
                 PropagationLevel toLevel);

        /// <summary>
        /// Init with the specified propagation object.
        /// </summary>
        /// <param name="propagation">The propagation object that this method will
        /// be used with.</param>
        void Init(Propagation propagation);

        /// <summary>
        /// Apply the accumulated deltas and learn.
        /// </summary>
        void Learn();

    }

}
