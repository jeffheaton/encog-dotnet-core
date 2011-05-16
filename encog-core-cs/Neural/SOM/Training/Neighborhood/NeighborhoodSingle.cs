namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// A very simple neighborhood function that will return 1.0 (full effect) for
    /// the winning neuron, and 0.0 (no change) for everything else.
    /// </summary>
    ///
    public class NeighborhoodSingle : INeighborhoodFunction
    {
        #region INeighborhoodFunction Members

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public virtual double Function(int currentNeuron, int bestNeuron)
        {
            if (currentNeuron == bestNeuron)
            {
                return 1.0d;
            }
            else
            {
                return 0.0d;
            }
        }

        /// <summary>
        /// Set the radius.  This type does not use a radius, so this has no effect.
        /// </summary>
        ///
        /// <value>The radius.</value>
        public virtual double Radius
        {
            /// <summary>
            /// The radius for this neighborhood function is always 1.
            /// </summary>
            ///
            /// <returns>The radius.</returns>
            get { return 1; }
            /// <summary>
            /// Set the radius.  This type does not use a radius, so this has no effect.
            /// </summary>
            ///
            /// <param name="radius">The radius.</param>
            set
            {
                // no effect on this type
            }
        }

        #endregion
    }
}