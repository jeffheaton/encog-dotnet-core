//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Text;
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
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
        private readonly BestMatchingUnit _bmuUtil;

        /// <summary>
        /// Holds the corrections for any matrix being trained.
        /// </summary>
        private readonly Matrix _correctionMatrix;

        /// <summary>
        /// How many neurons in the input layer.
        /// </summary>
        private readonly int _inputNeuronCount;

        /// <summary>
        /// The neighborhood function to use to determine to what degree a neuron
        /// should be "trained".
        /// </summary>
        private readonly INeighborhoodFunction _neighborhood;

        /// <summary>
        /// The network being trained.
        /// </summary>
        private readonly SOMNetwork _network;

        /// <summary>
        /// How many neurons in the output layer.
        /// </summary>
        private readonly int _outputNeuronCount;

        /// <summary>
        /// This is the current autodecay radius.
        /// </summary>
        private double _autoDecayRadius;

        /// <summary>
        /// This is the current autodecay learning rate.
        /// </summary>
        private double _autoDecayRate;

        /// <summary>
        /// When used with autodecay, this is the ending radius.
        /// </summary>
        private double _endRadius;

        /// <summary>
        /// When used with autodecay, this is the ending learning rate.
        /// </summary>
        private double _endRate;

        /// <summary>
        /// The current radius.
        /// </summary>
        private double _radius;

        /// <summary>
        /// When used with autodecay, this is the starting radius.
        /// </summary>
        private double _startRadius;

        /// <summary>
        /// When used with autodecay, this is the starting learning rate.
        /// </summary>
        private double _startRate;

        /// <summary>
        /// Create an instance of competitive training.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="learningRate">The learning rate, how much to apply per iteration.</param>
        /// <param name="training">The training set (unsupervised).</param>
        /// <param name="neighborhood">The neighborhood function to use.</param>
        public BasicTrainSOM(SOMNetwork network, double learningRate,
                             IMLDataSet training, INeighborhoodFunction neighborhood)
            : base(TrainingImplementationType.Iterative)
        {
            _neighborhood = neighborhood;
            Training = training;
            LearningRate = learningRate;
            _network = network;
            _inputNeuronCount = network.InputCount;
            _outputNeuronCount = network.OutputCount;
            ForceWinner = false;

            // setup the correction matrix
            _correctionMatrix = new Matrix(_outputNeuronCount, _inputNeuronCount);

            // create the BMU class
            _bmuUtil = new BestMatchingUnit(network);
        }

        /// <summary>
        /// True is a winner is to be forced, see class description, or forceWinners
        /// method. By default, this is true.
        /// </summary>
        public bool ForceWinner { get; set; }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// The input neuron count.
        /// </summary>
        public int InputNeuronCount
        {
            get { return _inputNeuronCount; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// The network neighborhood function.
        /// </summary>
        public INeighborhoodFunction Neighborhood
        {
            get { return _neighborhood; }
        }

        /// <summary>
        /// The output neuron count.
        /// </summary>
        public int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
        }

        #region ILearningRate Members

        /// <summary>
        /// The learning rate. To what degree should changes be applied.
        /// </summary>
        public double LearningRate { get; set; }

        #endregion

        /// <summary>
        /// Loop over the synapses to be trained and apply any corrections that were
        /// determined by this training iteration.
        /// </summary>
        private void ApplyCorrection()
        {
            _network.Weights.Set(_correctionMatrix);
        }

        /// <summary>
        /// Should be called each iteration if autodecay is desired.
        /// </summary>
        public void AutoDecay()
        {
            if (_radius > _endRadius)
            {
                _radius += _autoDecayRadius;
            }

            if (LearningRate > _endRate)
            {
                LearningRate += _autoDecayRate;
            }
            _neighborhood.Radius = _radius;
        }


        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        /// <param name="matrix">The matrix that is the target of the copy.</param>
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(Matrix matrix, int outputNeuron,
                                      IMLData input)
        {
            for (int inputNeuron = 0; inputNeuron < _inputNeuronCount; inputNeuron++)
            {
                matrix.Data[outputNeuron][inputNeuron] = input[inputNeuron];
            }
        }

        /// <summary>
        /// Called to decay the learning rate and radius by the specified amount.
        /// </summary>
        /// <param name="d">The percent to decay by.</param>
        public void Decay(double d)
        {
            _radius *= (1.0 - d);
            LearningRate *= (1.0 - d);
        }

        /// <summary>
        /// Decay the learning rate and radius by the specified amount.
        /// </summary>
        /// <param name="decayRate">The percent to decay the learning rate by.</param>
        /// <param name="decayRadius">The percent to decay the radius by.</param>
        public void Decay(double decayRate, double decayRadius)
        {
            _radius *= (1.0 - decayRadius);
            LearningRate *= (1.0 - decayRate);
            _neighborhood.Radius = _radius;
        }

        /// <summary>
        /// Determine the weight adjustment for a single neuron during a training
        /// iteration.
        /// </summary>
        /// <param name="weight">The starting weight.</param>
        /// <param name="input">The input to this neuron.</param>
        /// <param name="currentNeuron">The neuron who's weight is being updated.</param>
        /// <param name="bmu">The neuron that "won", the best matching unit.</param>
        /// <returns>The new weight value.</returns>
        private double DetermineNewWeight(double weight, double input,
                                          int currentNeuron, int bmu)
        {
            double newWeight = weight
                               + (_neighborhood.Function(currentNeuron, bmu)
                                  *LearningRate*(input - weight));
            return newWeight;
        }

        /// <summary>
        /// Force any neurons that did not win to off-load patterns from overworked
        /// neurons.
        /// </summary>
        /// <param name="matrix">The synapse to modify.</param>
        /// <param name="won">An array that specifies how many times each output neuron has "won".</param>
        /// <param name="leastRepresented">The training pattern that is the least represented by this neural network.</param>
        /// <returns>True if a winner was forced.</returns>
        private bool ForceWinners(Matrix matrix, int[] won,
                                  IMLData leastRepresented)
        {
            double maxActivation = Double.NegativeInfinity;
            int maxActivationNeuron = -1;

            IMLData output = Compute(_network, leastRepresented);

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
            return false;
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            EncogLogging.Log(EncogLogging.LevelInfo,
                             "Performing SOM Training iteration.");

            PreIteration();

            // Reset the BMU and begin this iteration.
            _bmuUtil.Reset();
            var won = new int[_outputNeuronCount];
            double leastRepresentedActivation = Double.PositiveInfinity;
            IMLData leastRepresented = null;

            // Reset the correction matrix for this synapse and iteration.
            _correctionMatrix.Clear();

            // Determine the BMU for each training element.
            foreach (IMLDataPair pair in Training)
            {
                IMLData input = pair.Input;

                int bmu = _bmuUtil.CalculateBMU(input);
                won[bmu]++;

                // If we are to force a winner each time, then track how many
                // times each output neuron becomes the BMU (winner).
                if (ForceWinner)
                {
                    // Get the "output" from the network for this pattern. This
                    // gets the activation level of the BMU.
                    IMLData output = Compute(_network, pair.Input);

                    // Track which training entry produces the least BMU. This
                    // pattern is the least represented by the network.
                    if (output[bmu] < leastRepresentedActivation)
                    {
                        leastRepresentedActivation = output[bmu];
                        leastRepresented = pair.Input;
                    }
                }

                Train(bmu, _network.Weights, input);

                if (ForceWinner)
                {
                    // force any non-winning neurons to share the burden somewhat\
                    if (!ForceWinners(_network.Weights, won,
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
            Error = _bmuUtil.WorstDistance/100.0;

            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup autodecay. This will decrease the radius and learning rate from the
        /// start values to the end values.
        /// </summary>
        /// <param name="plannedIterations">The number of iterations that are planned. This allows the
        /// decay rate to be determined.</param>
        /// <param name="startRate">The starting learning rate.</param>
        /// <param name="endRate">The ending learning rate.</param>
        /// <param name="startRadius">The starting radius.</param>
        /// <param name="endRadius">The ending radius.</param>
        public void SetAutoDecay(int plannedIterations,
                                 double startRate, double endRate,
                                 double startRadius, double endRadius)
        {
            _startRate = startRate;
            _endRate = endRate;
            _startRadius = startRadius;
            _endRadius = endRadius;
            _autoDecayRadius = (endRadius - startRadius)/plannedIterations;
            _autoDecayRate = (endRate - startRate)/plannedIterations;
            SetParams(_startRate, _startRadius);
        }

        /// <summary>
        /// Set the learning rate and radius.
        /// </summary>
        /// <param name="rate">The new learning rate.</param>
        /// <param name="radius">The new radius.</param>
        public void SetParams(double rate, double radius)
        {
            _radius = radius;
            LearningRate = rate;
            _neighborhood.Radius = radius;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("Rate=");
            result.Append(Format.FormatPercent(LearningRate));
            result.Append(", Radius=");
            result.Append(Format.FormatDouble(_radius, 2));
            return result.ToString();
        }

        /// <summary>
        /// Train for the specified synapse and BMU.
        /// </summary>
        /// <param name="bmu">The best matching unit for this input.</param>
        /// <param name="matrix">The synapse to train.</param>
        /// <param name="input">The input to train for.</param>
        private void Train(int bmu, Matrix matrix, IMLData input)
        {
            // adjust the weight for the BMU and its neighborhood
            for (int outputNeuron = 0; outputNeuron < _outputNeuronCount; outputNeuron++)
            {
                TrainPattern(matrix, input, outputNeuron, bmu);
            }
        }

        /// <summary>
        /// Train for the specified pattern.
        /// </summary>
        /// <param name="matrix">The synapse to train.</param>
        /// <param name="input">The input pattern to train for.</param>
        /// <param name="current">The current output neuron being trained.</param>
        /// <param name="bmu">The best matching unit, or winning output neuron.</param>
        private void TrainPattern(Matrix matrix, IMLData input,
                                  int current, int bmu)
        {
            for (int inputNeuron = 0; inputNeuron < _inputNeuronCount; inputNeuron++)
            {
                double currentWeight = matrix.Data[current][inputNeuron];
                double inputValue = input[inputNeuron];

                double newWeight = DetermineNewWeight(currentWeight,
                                                      inputValue, current, bmu);

                _correctionMatrix.Data[current][inputNeuron] = newWeight;
            }
        }

        /// <summary>
        /// Train the specified pattern. Find a winning neuron and adjust all neurons
        /// according to the neighborhood function.
        /// </summary>
        /// <param name="pattern">The pattern to train.</param>
        public void TrainPattern(IMLData pattern)
        {
            IMLData input = pattern;
            int bmu = _bmuUtil.CalculateBMU(input);
            Train(bmu, _network.Weights, input);
            ApplyCorrection();
        }

        /// <summary>
        /// Calculate the output of the SOM, for each output neuron.  Typically,
        /// you will use the classify method instead of calling this method.
        /// </summary>
        /// <param name="som">The SOM to use.</param>
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        private static IMLData Compute(SOMNetwork som, IMLData input)
        {
            var result = new BasicMLData(som.OutputCount);

            for (int i = 0; i < som.OutputCount; i++)
            {
                Matrix optr = som.Weights.GetRow(i);
                Matrix inputMatrix = Matrix.CreateRowMatrix(input);
                result[i] = MatrixMath.DotProduct(inputMatrix, optr);
            }

            return result;
        }
    }
}
