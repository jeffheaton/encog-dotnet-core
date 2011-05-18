using System;
using System.Text;
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;
using Encog.Util.Logging;

namespace Encog.Neural.SOM.Training.Neighborhood
{
    /// <summary>
    /// This class implements competitive training, which would be used in a
    /// winner-take-all neural network, such as the self organizing map (SOM). This
    /// is an unsupervised training method, no ideal data is needed on the training
    /// set. If ideal data is provided, it will be ignored.
    /// Training is done by looping over all of the training elements and calculating
    /// a "best matching unit" (BMU). This BMU output neuron is then adjusted to
    /// better "learn" this pattern. Additionally, this training may be applied to
    /// other "nearby" output neurons. The degree to which nearby neurons are update
    /// is defined by the neighborhood function.
    /// A neighborhood function is required to determine the degree to which
    /// neighboring neurons (to the winning neuron) are updated by each training
    /// iteration.
    /// Because this is unsupervised training, calculating an error to measure
    /// progress by is difficult. The error is defined to be the "worst", or longest,
    /// Euclidean distance of any of the BMU's. This value should be minimized, as
    /// learning progresses.
    /// Because only the BMU neuron and its close neighbors are updated, you can end
    /// up with some output neurons that learn nothing. By default these neurons are
    /// not forced to win patterns that are not represented well. This spreads out
    /// the workload among all output neurons. This feature is not used by default,
    /// but can be enabled by setting the "forceWinner" property.
    /// </summary>
    ///
    public class BasicTrainSOM : BasicTraining, ILearningRate
    {
        /// <summary>
        /// Utility class used to determine the BMU.
        /// </summary>
        ///
        private readonly BestMatchingUnit bmuUtil;

        /// <summary>
        /// Holds the corrections for any matrix being trained.
        /// </summary>
        ///
        private readonly Matrix correctionMatrix;

        /// <summary>
        /// How many neurons in the input layer.
        /// </summary>
        ///
        private readonly int inputNeuronCount;

        /// <summary>
        /// The neighborhood function to use to determine to what degree a neuron
        /// should be "trained".
        /// </summary>
        ///
        private readonly INeighborhoodFunction neighborhood;

        /// <summary>
        /// The network being trained.
        /// </summary>
        ///
        private readonly SOMNetwork network;

        /// <summary>
        /// How many neurons in the output layer.
        /// </summary>
        ///
        private readonly int outputNeuronCount;

        /// <summary>
        /// This is the current autodecay radius.
        /// </summary>
        ///
        private double autoDecayRadius;

        /// <summary>
        /// This is the current autodecay learning rate.
        /// </summary>
        ///
        private double autoDecayRate;

        /// <summary>
        /// When used with autodecay, this is the ending radius.
        /// </summary>
        ///
        private double endRadius;

        /// <summary>
        /// When used with autodecay, this is the ending learning rate.
        /// </summary>
        ///
        private double endRate;

        /// <summary>
        /// True is a winner is to be forced, see class description, or forceWinners
        /// method. By default, this is true.
        /// </summary>
        ///
        private bool forceWinner;

        /// <summary>
        /// The learning rate. To what degree should changes be applied.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// The current radius.
        /// </summary>
        ///
        private double radius;

        /// <summary>
        /// When used with autodecay, this is the starting radius.
        /// </summary>
        ///
        private double startRadius;

        /// <summary>
        /// When used with autodecay, this is the starting learning rate.
        /// </summary>
        ///
        private double startRate;

        /// <summary>
        /// Create an instance of competitive training.
        /// </summary>
        ///
        /// <param name="network_0">The network to train.</param>
        /// <param name="learningRate_1">The learning rate, how much to apply per iteration.</param>
        /// <param name="training">The training set (unsupervised).</param>
        /// <param name="neighborhood_2">The neighborhood function to use.</param>
        public BasicTrainSOM(SOMNetwork network_0, double learningRate_1,
                             MLDataSet training, INeighborhoodFunction neighborhood_2)
            : base(TrainingImplementationType.Iterative)
        {
            neighborhood = neighborhood_2;
            Training = training;
            learningRate = learningRate_1;
            network = network_0;
            inputNeuronCount = network_0.InputNeuronCount;
            outputNeuronCount = network_0.OutputNeuronCount;
            forceWinner = false;
            Error = 0;

            // setup the correction matrix
            correctionMatrix = new Matrix(inputNeuronCount,
                                          outputNeuronCount);

            // create the BMU class
            bmuUtil = new BestMatchingUnit(network_0);
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <value>The input neuron count.</value>
        public int InputNeuronCount
        {
            get { return inputNeuronCount; }
        }


        /// <inheritdoc/>
        public override MLMethod Method
        {
            get { return null; }
        }


        /// <value>The network neighborhood function.</value>
        public INeighborhoodFunction Neighborhood
        {
            get { return neighborhood; }
        }


        /// <value>The output neuron count.</value>
        public int OutputNeuronCount
        {
            get { return outputNeuronCount; }
        }


        /// <summary>
        /// Determine if a winner is to be forced. See class description for more
        /// info.
        /// </summary>
        public bool ForceWinner
        {
            get { return forceWinner; }
            set { forceWinner = value; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate. This is the rate at which the weights are changed.
        /// </summary>
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }

        #endregion

        /// <summary>
        /// Loop over the synapses to be trained and apply any corrections that were
        /// determined by this training iteration.
        /// </summary>
        ///
        private void ApplyCorrection()
        {
            network.Weights.Set(correctionMatrix);
        }

        /// <summary>
        /// Should be called each iteration if autodecay is desired.
        /// </summary>
        ///
        public void AutoDecay()
        {
            if (radius > endRadius)
            {
                radius += autoDecayRadius;
            }

            if (learningRate > endRate)
            {
                learningRate += autoDecayRate;
            }
            Neighborhood.Radius = radius;
        }

        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        ///
        /// <param name="matrix">The matrix that is the target of the copy.</param>
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(Matrix matrix, int outputNeuron,
                                      MLData input)
        {
            for (int inputNeuron = 0; inputNeuron < inputNeuronCount; inputNeuron++)
            {
                matrix[inputNeuron, outputNeuron] = input[inputNeuron];
            }
        }

        /// <summary>
        /// Called to decay the learning rate and radius by the specified amount.
        /// </summary>
        ///
        /// <param name="d">The percent to decay by.</param>
        public void Decay(double d)
        {
            radius *= (1.0d - d);
            learningRate *= (1.0d - d);
        }

        /// <summary>
        /// Decay the learning rate and radius by the specified amount.
        /// </summary>
        ///
        /// <param name="decayRate">The percent to decay the learning rate by.</param>
        /// <param name="decayRadius">The percent to decay the radius by.</param>
        public void Decay(double decayRate, double decayRadius)
        {
            radius *= (1.0d - decayRadius);
            learningRate *= (1.0d - decayRate);
            Neighborhood.Radius = radius;
        }

        /// <summary>
        /// Determine the weight adjustment for a single neuron during a training
        /// iteration.
        /// </summary>
        ///
        /// <param name="weight">The starting weight.</param>
        /// <param name="input">The input to this neuron.</param>
        /// <param name="currentNeuron">The neuron who's weight is being updated.</param>
        /// <param name="bmu">The neuron that "won", the best matching unit.</param>
        /// <returns>The new weight value.</returns>
        private double DetermineNewWeight(double weight, double input,
                                          int currentNeuron, int bmu)
        {
            double newWeight = weight
                               + (neighborhood.Function(currentNeuron, bmu)
                                  *learningRate*(input - weight));
            return newWeight;
        }

        /// <summary>
        /// Force any neurons that did not win to off-load patterns from overworked
        /// neurons.
        /// </summary>
        ///
        /// <param name="won"></param>
        /// <param name="leastRepresented"></param>
        /// <param name="matrix">The synapse to modify.</param>
        /// <returns>True if a winner was forced.</returns>
        private bool ForceWinners(Matrix matrix, int[] won,
                                  MLData leastRepresented)
        {
            double maxActivation = Double.MinValue;
            int maxActivationNeuron = -1;

            MLData output = network.Compute(leastRepresented);

            // Loop over all of the output neurons. Consider any neurons that were
            // not the BMU (winner) for any pattern. Track which of these
            // non-winning neurons had the highest activation.
            for (int outputNeuron = 0; outputNeuron < won.Length; outputNeuron++)
            {
                // Only consider neurons that did not "win".
                if (won[outputNeuron] == 0)
                {
                    if ((maxActivationNeuron == -1)
                        || (output[outputNeuron] > maxActivation))
                    {
                        maxActivation = output[outputNeuron];
                        maxActivationNeuron = outputNeuron;
                    }
                }
            }

            // If a neurons was found that did not activate for any patterns, then
            // force it to "win" the least represented pattern.
            if (maxActivationNeuron != -1)
            {
                CopyInputPattern(matrix, maxActivationNeuron, leastRepresented);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            EncogLogging.Log(EncogLogging.LEVEL_INFO,
                             "Performing SOM Training iteration.");

            PreIteration();

            // Reset the BMU and begin this iteration.
            bmuUtil.Reset();
            var won = new int[outputNeuronCount];
            double leastRepresentedActivation = Double.MaxValue;
            MLData leastRepresented = null;

            // Reset the correction matrix for this synapse and iteration.
            correctionMatrix.Clear();


            // Determine the BMU foreach each training element.
            foreach (MLDataPair pair  in  Training)
            {
                MLData input = pair.Input;

                int bmu = bmuUtil.CalculateBMU(input);

                // If we are to force a winner each time, then track how many
                // times each output neuron becomes the BMU (winner).
                if (forceWinner)
                {
                    won[bmu]++;

                    // Get the "output" from the network for this pattern. This
                    // gets the activation level of the BMU.
                    MLData output = network.Compute(pair.Input);

                    // Track which training entry produces the least BMU. This
                    // pattern is the least represented by the network.
                    if (output[bmu] < leastRepresentedActivation)
                    {
                        leastRepresentedActivation = output[bmu];
                        leastRepresented = pair.Input;
                    }
                }

                Train(bmu, network.Weights, input);

                if (forceWinner)
                {
                    // force any non-winning neurons to share the burden somewhat\
                    if (!ForceWinners(network.Weights, won,
                                      leastRepresented))
                    {
                        ApplyCorrection();
                    }
                }
                else
                {
                    ApplyCorrection();
                }
            }

            // update the error
            Error = bmuUtil.WorstDistance/100.0d;

            PostIteration();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup autodecay. This will decrease the radius and learning rate from the
        /// start values to the end values.
        /// </summary>
        ///
        /// <param name="plannedIterations"></param>
        /// <param name="startRate_0">The starting learning rate.</param>
        /// <param name="endRate_1">The ending learning rate.</param>
        /// <param name="startRadius_2">The starting radius.</param>
        /// <param name="endRadius_3">The ending radius.</param>
        public void SetAutoDecay(int plannedIterations,
                                 double startRate_0, double endRate_1,
                                 double startRadius_2, double endRadius_3)
        {
            startRate = startRate_0;
            endRate = endRate_1;
            startRadius = startRadius_2;
            endRadius = endRadius_3;
            autoDecayRadius = (endRadius_3 - startRadius_2)/plannedIterations;
            autoDecayRate = (endRate_1 - startRate_0)/plannedIterations;
            SetParams(startRate, startRadius);
        }

        /// <summary>
        /// Set the learning rate and radius.
        /// </summary>
        ///
        /// <param name="rate">The new learning rate.</param>
        /// <param name="radius_0">The new radius.</param>
        public void SetParams(double rate, double radius_0)
        {
            radius = radius_0;
            learningRate = rate;
            Neighborhood.Radius = radius_0;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("Rate=");
            result.Append(Format.FormatPercent(learningRate));
            result.Append(", Radius=");
            result.Append(Format.FormatDouble(radius, 2));
            return result.ToString();
        }

        /// <summary>
        /// Train for the specified synapse and BMU.
        /// </summary>
        ///
        /// <param name="bmu">The best matching unit for this input.</param>
        /// <param name="matrix">The synapse to train.</param>
        /// <param name="input">The input to train for.</param>
        private void Train(int bmu, Matrix matrix, MLData input)
        {
            // adjust the weight for the BMU and its neighborhood
            for (int outputNeuron = 0; outputNeuron < outputNeuronCount; outputNeuron++)
            {
                TrainPattern(matrix, input, outputNeuron, bmu);
            }
        }

        /// <summary>
        /// Train for the specified pattern.
        /// </summary>
        ///
        /// <param name="matrix">The synapse to train.</param>
        /// <param name="input">The input pattern to train for.</param>
        /// <param name="current">The current output neuron being trained.</param>
        /// <param name="bmu">The best matching unit, or winning output neuron.</param>
        private void TrainPattern(Matrix matrix, MLData input,
                                  int current, int bmu)
        {
            for (int inputNeuron = 0; inputNeuron < inputNeuronCount; inputNeuron++)
            {
                double currentWeight = matrix[inputNeuron, current];
                double inputValue = input[inputNeuron];

                double newWeight = DetermineNewWeight(currentWeight,
                                                      inputValue, current, bmu);

                correctionMatrix[inputNeuron, current] = newWeight;
            }
        }

        /// <summary>
        /// Train the specified pattern. Find a winning neuron and adjust all neurons
        /// according to the neighborhood function.
        /// </summary>
        ///
        /// <param name="pattern">The pattern to train.</param>
        public void TrainPattern(MLData pattern)
        {
            MLData input = pattern;
            int bmu = bmuUtil.CalculateBMU(input);
            Train(bmu, network.Weights, input);
            ApplyCorrection();
        }
    }
}