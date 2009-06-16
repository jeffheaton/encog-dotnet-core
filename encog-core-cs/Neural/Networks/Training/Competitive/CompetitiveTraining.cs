using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Synapse;
using log4net;
using Encog.Neural.Data;
using Encog.Neural.Networks.Training.Competitive.Neighborhood;
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Competitive
{
    /// <summary>
    /// This class implements competitive training, which would be used in a
    /// winner-take-all neural network, such as the self organizing map (SOM). This
    /// is an unsupervised training method, no ideal data is needed on the training
    /// set. If ideal data is provided, it will be ignored.
    /// 
    /// Training is done by looping over all of the training elements and calculating
    /// a "best matching unit" (BMU). This BMU output neuron is then adjusted to
    /// better "learn" this pattern. Additionally, this training may be applied to
    /// othr "nearby" output neurons. The degree to which nearby neurons are update
    /// is defined by the neighborhood function.
    /// 
    /// A neighborhood function is required to determine the degree to which
    /// neighboring neurons (to the winning neuron) are updated by each training
    /// iteration.
    /// 
    /// Because this is unsupervised training, calculating an error to measure
    /// progress by is difficult. The error is defined to be the "worst", or longest,
    /// Euclidean distance of any of the BMU's. This value should be minimized, as
    /// learning progresses.
    /// 
    /// Because only the BMU neuron and its close neighbors are updated, you can end
    /// up with some output neurons that learn nothing. By default these neurons are
    /// forced to win patterns that are not represented well. This spreads out the
    /// workload among all output neurons. This feature is used by default, but can
    /// be disabled by setting the "forceWinner" property.
    /// </summary>
    public class CompetitiveTraining : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The neighborhood function to use to determine to what degree a neuron
        /// should be "trained".
        /// </summary>
        private INeighborhoodFunction neighborhood;

        /// <summary>
        /// The learning rate. To what degree should changes be applied.
        /// </summary>
        private double learningRate;

        /// <summary>
        /// The network being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The input layer.
        /// </summary>
        private ILayer inputLayer;

        /// <summary>
        /// The output layer.
        /// </summary>
        private ILayer outputLayer;

        /// <summary>
        /// A collection of the synapses being modified.
        /// </summary>
        private ICollection<ISynapse> synapses;

        /// <summary>
        /// How many neurons in the input layer.
        /// </summary>
        private int inputNeuronCount;

        /// <summary>
        /// How many neurons in the output layer.
        /// </summary>
        private int outputNeuronCount;

        /// <summary>
        /// Utility class used to determine the BMU.
        /// </summary>
        private BestMatchingUnit bmuUtil;

        /// <summary>
        /// Holds the corrections for any matrix being trained.
        /// </summary>
        private IDictionary<ISynapse, Matrix.Matrix> correctionMatrix
            = new Dictionary<ISynapse, Matrix.Matrix>();

        /// <summary>
        /// True is a winner is to be forced, see class description, or forceWinners
        /// method. By default, this is true.
        /// </summary>
        private bool forceWinner;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(CompetitiveTraining));

        /// <summary>
        /// Create an instance of competitive training.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="learningRate">The learning rate, how much to apply per iteration.</param>
        /// <param name="training">The training set (unsupervised).</param>
        /// <param name="neighborhood">The neighborhood function to use.</param>
        public CompetitiveTraining(BasicNetwork network,
                 double learningRate, INeuralDataSet training,
                 INeighborhoodFunction neighborhood)
        {
            this.neighborhood = neighborhood;
            this.Training = training;
            this.learningRate = learningRate;
            this.network = network;
            this.inputLayer = network.InputLayer;
            this.outputLayer = network.OutputLayer;
            this.synapses = network.Structure.GetPreviousSynapses(
                    this.outputLayer);
            this.inputNeuronCount = this.inputLayer.NeuronCount;
            this.outputNeuronCount = this.outputLayer.NeuronCount;
            this.forceWinner = true;
            this.Error = 0;

            // setup the correction matrix
            foreach (ISynapse synapse in this.synapses)
            {
                Matrix.Matrix matrix = new Matrix.Matrix(synapse.WeightMatrix.Rows,
                       synapse.WeightMatrix.Cols);
                this.correctionMatrix.Add(synapse, matrix);
            }

            // create the BMU class
            this.bmuUtil = new BestMatchingUnit(this);
        }

        /// <summary>
        /// Loop over the synapses to be trained and apply any corrections that were
        /// determined by this training iteration.
        /// </summary>
        private void ApplyCorrection()
        {
            foreach (KeyValuePair<ISynapse, Matrix.Matrix> entry in this.correctionMatrix)
            {
                entry.Key.WeightMatrix.Add(entry.Value);
            }
        }

        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        /// <param name="synapse">The synapse that is the target of the copy.</param>
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(ISynapse synapse,
                 int outputNeuron, INeuralData input)
        {
            for (int inputNeuron = 0; inputNeuron < this.inputNeuronCount;
                inputNeuron++)
            {
                synapse.WeightMatrix[inputNeuron, outputNeuron] =
                        input[inputNeuron];
            }
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
        private double DetermineWeightAdjustment(double weight,
                 double input, int currentNeuron, int bmu)
        {

            double delta = this.neighborhood.Function(currentNeuron, bmu)
                   * this.learningRate * (input - weight);

            return delta;
        }

        /// <summary>
        /// Force any neurons that did not win to off-load patterns from overworked
        /// neurons.
        /// </summary>
        /// <param name="synapse">The synapse.</param>
        /// <param name="won">An array that specifies how many times each output neuron has "won".</param>
        /// <param name="leastRepresented">The training pattern that is the least represented by this neural network.</param>
        /// <returns>True if a winner was forced.</returns>
        private bool ForceWinners(ISynapse synapse, int[] won,
                 INeuralData leastRepresented)
        {

            double maxActivation = double.MinValue;
            int maxActivationNeuron = -1;

            INeuralData output = this.network.Compute(leastRepresented);

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
                CopyInputPattern(synapse, maxActivationNeuron, leastRepresented);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The input neuron count.
        /// </summary>
        public int InputNeuronCount
        {
            get
            {
                return this.inputNeuronCount;
            }
        }

        /// <summary>
        /// The learning rate. This was set when the object was created.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return this.learningRate;
            }
            set
            {
                this.learningRate = value;
            }
        }

        /// <summary>
        /// The network neighborhood function.
        /// </summary>
        public INeighborhoodFunction Neighborhood
        {
            get
            {
                return this.neighborhood;
            }
        }

        /// <summary>
        /// The network being trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The output neuron count.
        /// </summary>
        public int OutputNeuronCount
        {
            get
            {
                return this.outputNeuronCount;
            }
        }

        /// <summary>
        /// Is a winner to be forced of neurons that do not learn. See class
        /// description for more info.
        /// </summary>
        public bool ForceWinner
        {
            get
            {
                return this.forceWinner;
            }
            set
            {
                this.forceWinner = value;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("Performing Competitive Training iteration.");
            }

            PreIteration();

            // Reset the BMU and begin this iteration.
            this.bmuUtil.Reset();
            int[] won = new int[this.outputNeuronCount];
            double leastRepresentedActivation = Double.MaxValue;
            INeuralData leastRepresented = null;

            // The synapses are processed parallel to each other.
            foreach (ISynapse synapse in this.synapses)
            {

                // Reset the correction matrix for this synapse and iteration.
                Matrix.Matrix correction = this.correctionMatrix[synapse];
                correction.Clear();

                // Determine the BMU for each training element.
                foreach (INeuralDataPair pair in this.Training)
                {

                    INeuralData input = pair.Input;

                    int bmu = this.bmuUtil.CalculateBMU(synapse, input);

                    // If we are to force a winner each time, then track how many
                    // times each output neuron becomes the BMU (winner).
                    if (this.forceWinner)
                    {
                        won[bmu]++;

                        // Get the "output" from the network for this pattern. This
                        // gets the activation level of the BMU.
                        INeuralData output = this.network.Compute(pair
                               .Input);

                        // Track which training entry produces the least BMU. This
                        // pattern is the least represented by the network.
                        if (output[bmu] < leastRepresentedActivation)
                        {
                            leastRepresentedActivation = output[bmu];
                            leastRepresented = pair.Input;
                        }
                    }

                    Train(bmu, synapse, input);

                }

                if (this.forceWinner)
                {
                    // force any non-winning neurons to share the burden somewhat\
                    if (!ForceWinners(synapse, won, leastRepresented))
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
            this.Error = this.bmuUtil.WorstDistance;

            PostIteration();
        }

        /// <summary>
        /// Train for the specified synapse and BMU.
        /// </summary>
        /// <param name="bmu">The best matching unit for this input.</param>
        /// <param name="synapse">The synapse to train.</param>
        /// <param name="input">The input to train for.</param>
        private void Train(int bmu, ISynapse synapse,
                 INeuralData input)
        {
            // adjust the weight for the BMU and its neighborhood
            for (int outputNeuron = 0; outputNeuron < this.outputNeuronCount;
                outputNeuron++)
            {
                TrainPattern(synapse, input, outputNeuron, bmu);
            }
        }

        /// <summary>
        /// Train for the specified pattern.
        /// </summary>
        /// <param name="synapse">The synapse to train.</param>
        /// <param name="input">The input pattern to train for.</param>
        /// <param name="current">The current output neuron being trained.</param>
        /// <param name="bmu">The best matching unit, or winning output neuron.</param>
        private void TrainPattern(ISynapse synapse, INeuralData input,
                 int current, int bmu)
        {

            Matrix.Matrix correction = this.correctionMatrix[synapse];

            for (int inputNeuron = 0; inputNeuron < this.inputNeuronCount;
                inputNeuron++)
            {

                double currentWeight = synapse.WeightMatrix[inputNeuron,
                       current];
                double inputValue = input[inputNeuron];

                double newWeight = DetermineWeightAdjustment(currentWeight,
                       inputValue, current, bmu);

                correction.Add(inputNeuron, current, newWeight);
            }
        }

    }

}
