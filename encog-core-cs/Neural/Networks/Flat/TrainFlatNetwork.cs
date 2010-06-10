using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network single-threaded. This class is left in mainly for testing
    /// purposes. Usually, you will use TrainFlatNetworkMulti.
    /// </summary>
    public class TrainFlatNetwork
    {
        /// <summary>
        /// The error calculation method.
        /// </summary>
        private ErrorCalculation errorCalculation = new ErrorCalculation();

        /// <summary>
        /// The gradients
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// The last gradients, from the last training iteration.
        /// </summary>
        private double[] lastGradient;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        private int[] layerCounts;

        /// <summary>
        /// The deltas for each layer
        /// </summary>
        private double[] layerDelta;

        /// <summary>
        /// The layer indexes
        /// </summary>
        private int[] layerIndex;

        /// <summary>
        /// The output from each layer
        /// </summary>
        private double[] layerOutput;

        /// <summary>
        /// The network to train.
        /// </summary>
        private FlatNetwork network;

        /// <summary>
        /// The training data.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        private double[] updateValues;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        private int[] weightIndex;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// Construct a training class.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public TrainFlatNetwork(FlatNetwork network,
                INeuralDataSet training)
        {
            this.training = training;
            this.network = network;

            layerDelta = new double[network.LayerOutput.Length];
            gradients = new double[network.Weights.Length];
            updateValues = new double[network.Weights.Length];
            lastGradient = new double[network.Weights.Length];

            weights = network.Weights;
            layerIndex = network.LayerIndex;
            layerCounts = network.LayerCounts;
            weightIndex = network.WeightIndex;
            layerOutput = network.LayerOutput;

            for (int i = 0; i < updateValues.Length; i++)
            {
                updateValues[i] = ResilientPropagation.DEFAULT_INITIAL_UPDATE;
            }
        }

        /// <summary>
        /// The overall error.
        /// </summary>
        public double Error
        {
            get
            {
                return errorCalculation.Calculate();
            }
        }

        /// <summary>
        /// Perform a training iteration.
        /// </summary>
        public void Iteration()
        {
            double[] actual = new double[network.OutputCount];
            errorCalculation.Reset();

            foreach (INeuralDataPair pair in training)
            {
                double[] input = pair.Input.Data;
                double[] ideal = pair.Ideal.Data;

                network.Compute(input, actual);

                errorCalculation.UpdateError(actual, ideal);

                for (int i = 0; i < actual.Length; i++)
                {
                    layerDelta[i] = FlatNetwork.CalculateActivationDerivative(this.network.ActivationType[0],actual[i])
                      * (ideal[i] - actual[i]);  
                }

                for (int i = 0; i < layerCounts.Length - 1; i++)
                {
                    ProcessLevel(i);
                }
            }

            Learn();
        }

        /// <summary>
        /// Update the neural network weights.
        /// </summary>
        private void Learn()
        {
            for (int i = 0; i < gradients.Length; i++)
            {
                weights[i] += UpdateWeight(gradients, i);
                gradients[i] = 0;
            }
        }

        /// <summary>
        /// Process a level. 
        /// </summary>
        /// <param name="currentLevel">The level to process.</param>
        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = layerIndex[currentLevel + 1];
            int toLayerIndex = layerIndex[currentLevel];
            int fromLayerSize = layerCounts[currentLevel + 1];
            int toLayerSize = layerCounts[currentLevel];

            // clear the to-deltas
            for (int i = 0; i < fromLayerSize; i++)
            {
                layerDelta[fromLayerIndex + i] = 0;
            }

            int index = weightIndex[currentLevel] + toLayerSize;

            for (int x = 0; x < toLayerSize; x++)
            {
                for (int y = 0; y < fromLayerSize; y++)
                {
                    double value = layerOutput[fromLayerIndex + y]
                            * layerDelta[toLayerIndex + x];
                    gradients[index] += value;
                    layerDelta[fromLayerIndex + y] += weights[index]
                            * layerDelta[toLayerIndex + x];
                    index++;
                }
            }

            for (int i = 0; i < fromLayerSize; i++)
            {
                layerDelta[fromLayerIndex + i] *= FlatNetwork.CalculateActivationDerivative(
                    this.network.ActivationType[currentLevel],
                    layerOutput[fromLayerIndex+ i]);
            }
        }

        
        /// <summary>
        /// Determine the sign of the value. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value)
        {
            if (Math.Abs(value) < ResilientPropagation.DEFAULT_ZERO_TOLERANCE)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Determine the amount to change a weight by. 
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="index">The weight to adjust.</param>
        /// <returns>The amount to change this weight by.</returns>
        private double UpdateWeight(double[] gradients, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(this.gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = updateValues[index]
                        * ResilientPropagation.POSITIVE_ETA;
                delta = Math.Min(delta, ResilientPropagation.DEFAULT_MAX_STEP);
                weightChange = Sign(this.gradients[index]) * delta;
                updateValues[index] = delta;
                lastGradient[index] = this.gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = updateValues[index]
                        * ResilientPropagation.NEGATIVE_ETA;
                delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                updateValues[index] = delta;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = lastGradient[index];
                weightChange = Sign(this.gradients[index]) * delta;
                lastGradient[index] = this.gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }
    }
}
