using System;

namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// A neighborhood function that uses a simple bubble. A radius is defined, and
    /// any neuron that is plus or minus that width from the winning neuron will be
    /// updated as a result of training.
    /// </summary>
    ///
    public class NeighborhoodBubble : INeighborhoodFunction
    {
        /// <summary>
        /// The radius of the bubble.
        /// </summary>
        ///
        private double radius;

        /// <summary>
        /// Create a bubble neighborhood function that will return 1.0 (full update)
        /// for any neuron that is plus or minus the width distance from the winning
        /// neuron.
        /// </summary>
        ///
        /// <param name="radius_0">bubble, is actually two times this parameter.</param>
        public NeighborhoodBubble(int radius_0)
        {
            radius = radius_0;
        }

        #region INeighborhoodFunction Members

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public double Function(int currentNeuron, int bestNeuron)
        {
            int distance = Math.Abs(bestNeuron - currentNeuron);
            if (distance <= radius)
            {
                return 1.0d;
            }
            else
            {
                return 0.0d;
            }
        }

        /// <summary>
        /// Set the radius.
        /// </summary>
        ///
        /// <value>The new radius.</value>
        public virtual double Radius
        {
            /// <returns>The radius.</returns>
            get { return radius; }
            /// <summary>
            /// Set the radius.
            /// </summary>
            ///
            /// <param name="radius_0">The new radius.</param>
            set { radius = value; }
        }

        #endregion
    }
}