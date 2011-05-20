using Encog.ML.Data;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a flat network, using backpropagation.
    /// </summary>
    ///
    public class TrainFlatNetworkBackPropagation : TrainFlatNetworkProp
    {
        /// <summary>
        /// The last delta values.
        /// </summary>
        ///
        private double[] lastDelta;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// The momentum.
        /// </summary>
        ///
        private double momentum;

        /// <summary>
        /// Construct a backprop trainer for flat networks.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="theLearningRate">The learning rate.</param>
        /// <param name="theMomentum">The momentum.</param>
        public TrainFlatNetworkBackPropagation(FlatNetwork network,
                                               MLDataSet training, double theLearningRate,
                                               double theMomentum) : base(network, training)
        {
            momentum = theMomentum;
            learningRate = theLearningRate;
            lastDelta = new double[network.Weights.Length];
        }

        /// <summary>
        /// Set the last delta.
        /// </summary>
        public double[] LastDelta
        {
            get { return lastDelta; }
            set { lastDelta = value; }
        }


        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }


        /// <summary>
        /// Set the momentum.
        /// </summary>
        public double Momentum
        {
            get { return momentum; }
            set { momentum = value; }
        }


        /// <summary>
        /// Update a weight.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            double delta = (gradients[index]*learningRate)
                           + (lastDelta[index]*momentum);
            lastDelta[index] = delta;
            return delta;
        }
    }
}