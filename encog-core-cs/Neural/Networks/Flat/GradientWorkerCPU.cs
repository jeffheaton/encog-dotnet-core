using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Util;
using Encog.Util.Concurrency;
using System.Diagnostics;
using Encog.Util.Time;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Worker class for the mulithreaded training of flat networks.
    /// </summary>
    public class GradientWorkerCPU : IFlatGradientWorker
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        private FlatNetwork network;

        /// <summary>
        /// The error calculation method.
        /// </summary>
        private ErrorCalculation errorCalculation = new ErrorCalculation();

        /// <summary>
        /// The actual values from the neural network.
        /// </summary>
        private double[] actual;

        /// <summary>
        /// The deltas for each layer
        /// </summary>
        private double[] layerDelta;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        private int[] layerCounts;

        /// <summary>
        /// The layer indexes
        /// </summary>
        private int[] layerIndex;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        private int[] weightIndex;

        /// <summary>
        /// The output from each layer
        /// </summary>
        private double[] layerOutput;

        /// <summary>
        /// The gradients
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        private double[] weights;

        private INeuralDataPair pair;

        private IIndexable training;
        private int low;
        private int high;
        private TrainFlatNetworkMulti owner;
        private long elapsedTime;
        private Stopwatch stopwatch;


        /// <summary>
        /// Construct a gradient worker.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="owner">The owner that is doing the training.</param>
        /// <param name="training">The training data.</param>
        /// <param name="low">The low index to use in the training data.</param>
        /// <param name="high">The high index to use in the training data.</param>
        public GradientWorkerCPU(FlatNetwork network, TrainFlatNetworkMulti owner, IIndexable training, int low, int high)
        {
            this.network = network;
            this.training = training;
            this.low = low;
            this.high = high;
            this.owner = owner;

            this.stopwatch = new Stopwatch();

            layerDelta = new double[network.LayerOutput.Length];
            gradients = new double[network.Weights.Length];
            this.actual = new double[network.OutputCount];

            weights = network.Weights;
            layerIndex = network.LayerIndex;
            layerCounts = network.LayerCounts;
            weightIndex = network.WeightIndex;
            layerOutput = network.LayerOutput;

            this.pair = BasicNeuralDataPair.CreatePair(network.InputCount, network.OutputCount);
        }


        /// <summary>
        /// Perform the gradient calculation for the specified index range.
        /// </summary>
        public void Run()
        {
            try
            {
                this.stopwatch.Reset();
                this.stopwatch.Start();
                this.errorCalculation.Reset();
                for (int i = this.low; i <= high; i++)
                {
                    this.training.GetRecord(i, this.pair);
                    Process(pair.Input.Data, pair.Ideal.Data);
                }
                double error = this.errorCalculation.Calculate();
                this.owner.Report(this.gradients, error, null);
                EncogArray.Fill(this.gradients, 0);
                this.stopwatch.Stop();
                this.elapsedTime = this.stopwatch.ElapsedTicks;
            }
            catch (Exception ex)
            {
                this.owner.Report(null, 0, ex);
            }
        }

        /// <summary>
        /// Process one training set element.
        /// </summary>
        /// <param name="input">The network input.</param>
        /// <param name="ideal">The ideal values.</param>
        private void Process(double[] input, double[] ideal)
        {
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

        /// <summary>
        /// Process one level.
        /// </summary>
        /// <param name="currentLevel">The level.</param>
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

            int index = weightIndex[currentLevel];

            // handle thresholds
            for (int i = 0; i < toLayerSize; i++)
            {
                this.gradients[index++] += layerDelta[toLayerIndex+i];
            }

            // handle weights
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
                  this.network.ActivationType[currentLevel+1],
                  layerOutput[fromLayerIndex + i]); 
            }
        }


        /// <summary>
        /// The weights for this network.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return weights;
            }
        }

        /// <summary>
        /// Elapsed time for the last iteration.
        /// </summary>
        public long ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }
        }
    }
}
