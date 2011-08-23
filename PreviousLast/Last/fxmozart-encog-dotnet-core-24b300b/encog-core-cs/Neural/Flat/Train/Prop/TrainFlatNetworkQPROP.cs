using Encog.ML.Data;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a flat network, using QuickPropagation.  
    /// Code in this file is based on work done by 
    /// 
    /// An Empirical Study of Learning Speed in Back-Propagation Networks" (Scott E. Fahlman, 1988)
    /// </summary>
    public class TrainFlatNetworkQPROP : TrainFlatNetworkProp
    {
        /// <summary>
        /// This factor times the current weight is added to the slope 
        /// at the start of each output epoch. Keeps weights from growing 
        /// too big.
        /// </summary>
        public double Decay { get; set; }

        /// <summary>
        /// Used to scale for the size of the training set.
        /// </summary>
        public double EPS { get; set; }

        /// <summary>
        /// The last deltas.
        /// </summary>
        public double[] LastDelta { get; set; }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Controls the amount of linear gradient descent 
        /// to use in updating output weights.
        /// </summary>
        public double OutputEpsilon { get; set; }

        /// <summary>
        /// Used in computing whether the proposed step is 
        /// too large.  Related to learningRate.
        /// </summary>
        public double Shrink { get; set; }

        /// <summary>
        /// Construct a QPROP trainer for flat networks.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="theLearningRate">The learning rate.  2 is a good suggestion as 
        ///          a learning rate to start with.  If it fails to converge, 
        ///          then drop it.  Just like backprop, except QPROP can 
        ///            take higher learning rates.</param>
        public TrainFlatNetworkQPROP(FlatNetwork network,
                                     IMLDataSet training, double theLearningRate) : base(network, training)
        {
            LearningRate = theLearningRate;
            LastDelta = new double[Network.Weights.Length];
            Decay = 0.0001d;
            OutputEpsilon = 0.35;
        }

        /// <summary>
        /// Called to init the QPROP.
        /// </summary>
        public override void InitOthers()
        {
            EPS = OutputEpsilon/Training.Count;
            Shrink = LearningRate/(1.0 + LearningRate);
        }

        /// <summary>
        /// Update a weight.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index)
        {
            double w = Network.Weights[index];
            double d = LastDelta[index];
            double s = -Gradients[index] + Decay*w;
            double p = -lastGradient[index];
            double nextStep = 0.0;

            // The step must always be in direction opposite to the slope.
            if (d < 0.0)
            {
                // If last step was negative...
                if (s > 0.0)
                {
                    // Add in linear term if current slope is still positive.
                    nextStep -= EPS*s;
                }
                // If current slope is close to or larger than prev slope...
                if (s >= (Shrink*p))
                {
                    // Take maximum size negative step.
                    nextStep += LearningRate*d;
                }
                else
                {
                    // Else, use quadratic estimate.
                    nextStep += d*s/(p - s);
                }
            }
            else if (d > 0.0)
            {
                // If last step was positive...
                if (s < 0.0)
                {
                    // Add in linear term if current slope is still negative.
                    nextStep -= EPS*s;
                }
                // If current slope is close to or more neg than prev slope...
                if (s <= (Shrink*p))
                {
                    // Take maximum size negative step.
                    nextStep += LearningRate*d;
                }
                else
                {
                    // Else, use quadratic estimate.
                    nextStep += d*s/(p - s);
                }
            }
            else
            {
                // Last step was zero, so use only linear term. 
                nextStep -= EPS * s;
            }

            // update global data arrays
            LastDelta[index] = nextStep;
            LastGradient[index] = gradients[index];

            return nextStep;
        }
    }
}