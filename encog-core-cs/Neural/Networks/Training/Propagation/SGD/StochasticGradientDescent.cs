using Encog.ML.Train;
using System;
using Encog.ML;
using Encog.Neural.Networks.Training.Propagation.SGD.Update;
using Encog.Neural.Flat;
using Encog.Neural.Error;
using Encog.MathUtil.Error;
using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data;
using Encog.Util;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Networks.Training.Propagation.SGD
{


    public class StochasticGradientDescent : BasicTraining, IMomentum,
        ILearningRate
    {
        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// The momentum.
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// The gradients.
        /// </summary>
        private double[] _gradients;

        /// <summary>
        /// The deltas for each layer.
        /// </summary>
        private double[] _layerDelta;

        /// <summary>
        /// L1 regularization.
        /// </summary>
        public double L1 { get; set; }

        /// <summary>
        /// L2 regularization.
        /// </summary>
        public double L2 { get; set; }

        /// <summary>
        /// The update rule to use.
        /// </summary>
        public IUpdateRule UpdateRule { get; set; }

        /// <summary>
        /// The last delta values.
        /// </summary>
        private double[] _lastDelta;

        /// <summary>
        /// A flat neural network.
        /// </summary>
        private FlatNetwork _flat;

        /// <summary>
        /// The error function to use.
        /// </summary>
        private IErrorFunction _errorFunction = new CrossEntropyErrorFunction();

        /// <summary>
        /// The error calculation.
        /// </summary>
        private ErrorCalculation _errorCalculation;

        private IGenerateRandom _rnd;

        private IMLMethod _method;


        public StochasticGradientDescent(IContainsFlat network,
                                 IMLDataSet training) :
            this(network, training, new MersenneTwisterGenerateRandom())
        {

        }



        public StochasticGradientDescent(IContainsFlat network,
            IMLDataSet training, IGenerateRandom theRandom) :
            base(TrainingImplementationType.Iterative)
        {
            Training = training;
            UpdateRule = new AdamUpdate();

            if (!(training is BatchDataSet))
            {
                BatchSize = 25;
            }

            _method = network;
            _flat = network.Flat;
            _layerDelta = new double[_flat.LayerOutput.Length];
            _gradients = new double[_flat.Weights.Length];
            _errorCalculation = new ErrorCalculation();
            _rnd = theRandom;
            LearningRate = 0.001;
            Momentum = 0.9;
        }

        public int BatchSize
        {
            get
            {
                if (Training is BatchDataSet)
                {
                    return ((BatchDataSet)Training).BatchSize;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (Training is BatchDataSet)
                {
                    ((BatchDataSet)Training).BatchSize = value;
                }
                else
                {
                    BatchDataSet batchSet = new BatchDataSet(Training, _rnd);
                    batchSet.BatchSize = value;
                    Training = batchSet;
                }
            }
        }

        public void Process(IMLDataPair pair)
        {
            _errorCalculation = new ErrorCalculation();

            double[] actual = new double[_flat.OutputCount];

            _flat.Compute(pair.Input, actual);

            _errorCalculation.UpdateError(actual, pair.Ideal, pair.Significance);

            // Calculate error for the output layer.
            _errorFunction.CalculateError(
                    _flat.ActivationFunctions[0], _flat.LayerSums, _flat.LayerOutput,
                    pair.Ideal, actual, _layerDelta, 0,
                    pair.Significance);

            // Apply regularization, if requested.
            if (L1 > EncogFramework.DefaultDoubleEqual
                    || L2 > EncogFramework.DefaultDoubleEqual)
            {
                double[] lp = new double[2];
                CalculateRegularizationPenalty(lp);
                for (int i = 0; i < actual.Length; i++)
                {
                    double p = (lp[0] * L1) + (lp[1] * L2);
                    _layerDelta[i] += p;
                }
            }

            // Propagate backwards (chain rule from calculus).
            for (int i = _flat.BeginTraining; i < _flat
                    .EndTraining; i++)
            {
                ProcessLevel(i);
            }
        }

        public void Update()
        {
            if (IterationNumber == 0)
            {
                UpdateRule.Init(this);
            }

            PreIteration();

            UpdateRule.Update(_gradients, _flat.Weights);
            Error = _errorCalculation.Calculate();

            PostIteration();

            EngineArray.Fill(_gradients, 0);
            _errorCalculation.Reset();

            if (Training is BatchDataSet)
            {
                ((BatchDataSet)Training).Advance();
            }
        }

        public void ResetError()
        {
            _errorCalculation.Reset();
        }

        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = _flat.LayerIndex[currentLevel + 1];
            int toLayerIndex = _flat.LayerIndex[currentLevel];
            int fromLayerSize = _flat.LayerCounts[currentLevel + 1];
            int toLayerSize = _flat.LayerFeedCounts[currentLevel];
            double dropoutRate = 0;

            int index = _flat.WeightIndex[currentLevel];
            IActivationFunction activation = _flat
                    .ActivationFunctions[currentLevel];

            // handle weights
            // array references are made method local to avoid one indirection
            double[] layerDelta = _layerDelta;
            double[] weights = _flat.Weights;
            double[] gradients = _gradients;
            double[] layerOutput = _flat.LayerOutput;
            double[] layerSums = _flat.LayerSums;
            int yi = fromLayerIndex;
            for (int y = 0; y < fromLayerSize; y++)
            {
                double output = layerOutput[yi];
                double sum = 0;

                int wi = index + y;
                int loopEnd = toLayerIndex + toLayerSize;

                for (int xi = toLayerIndex; xi < loopEnd; xi++, wi += fromLayerSize)
                {
                    gradients[wi] += output * layerDelta[xi];
                    sum += weights[wi] * layerDelta[xi];
                }
                layerDelta[yi] = sum
                        * (activation.DerivativeFunction(layerSums[yi], layerOutput[yi]));

                yi++;
            }
        }

        public override void Iteration()
        {

            for (int i = 0; i < Training.Count; i++)
            {
                Process(Training[i]);
            }

            if (IterationNumber == 0)
            {
                UpdateRule.Init(this);
            }

            PreIteration();

            Update();
            PostIteration();

            if (Training is BatchDataSet)
            {
                ((BatchDataSet)Training).Advance();
            }
        }



        public override bool CanContinue
        {
            get { return false; }
        }

        public override IMLMethod Method
        {
            get
            {
                return _method;
            }
        }


        public override TrainingContinuation Pause()
        {
            throw new NotImplementedException();
        }

        public override void Resume(TrainingContinuation state)
        {
            throw new NotImplementedException();
        }

        public FlatNetwork Flat
        {
            get
            {
                return _flat;
            }
        }

        public void CalculateRegularizationPenalty(double[] l)
        {
            for (int i = 0; i < _flat.LayerCounts.Length - 1; i++)
            {
                LayerRegularizationPenalty(i, l);
            }
        }

        public void LayerRegularizationPenalty(int fromLayer, double[] l)
        {
            int fromCount = _flat.GetLayerTotalNeuronCount(fromLayer);
            int toCount = _flat.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double w = _flat.GetWeight(fromLayer, fromNeuron, toNeuron);
                    l[0] += Math.Abs(w);
                    l[1] += w * w;
                }
            }
        }
    }
}
