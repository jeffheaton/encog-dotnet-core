using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.MathUtil;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Util;
using Encog.Util.CL.Kernels;

namespace Encog.Neural.Networks.Flat
{
    public class GradientWorkerGPU:IEncogTask
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        private FlatNetwork network;

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

        /// <summary>
        /// Construct a gradient worker.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="owner">The owner that is doing the training.</param>
        /// <param name="training">The training data.</param>
        /// <param name="low">The low index to use in the training data.</param>
        /// <param name="high">The high index to use in the training data.</param>
        public GradientWorkerGPU(FlatNetwork network, TrainFlatNetworkMulti owner, IIndexable training, int low, int high)
        {
            this.network = network;
            this.training = training;
            this.low = low;
            this.high = high;
            this.owner = owner;

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
            KernelNetworkTrain k = Encog.Instance.GPU.ChooseAdapter().NetworkTrain;
            k.Train(this.network, this.training, this.high, this.low);

            double e = 0;
            for (int i = low; i < high; i++)
            {
                e += k.Errors[i];
            }

            this.owner.Report(this.gradients, Math.Sqrt(e/(training.Count*training.IdealSize)));

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
    }
}
