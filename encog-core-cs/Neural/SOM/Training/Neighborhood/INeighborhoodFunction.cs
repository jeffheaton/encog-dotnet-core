namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// Defines how a neighborhood function should work in competitive training. This
    /// is most often used in the training process for a self-organizing map. This
    /// function determines to what degree the training should take place on a
    /// neuron, based on its proximity to the "winning" neuron.
    /// </summary>
    ///
    public interface INeighborhoodFunction
    {
        /// <summary>
        /// Set the radius.
        /// </summary>
        double Radius { 
            get;
            set; }

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        double Function(int currentNeuron, int bestNeuron);
    }
}