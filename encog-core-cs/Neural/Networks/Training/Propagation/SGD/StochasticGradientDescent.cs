using Encog.ML.Train;
using System;
using Encog.ML;
using Encog.Neural.Networks.Training.Propagation.SGD.Update;
using Encog.Neural.Flat;
using Encog.Neural.Error;
using Encog.MathUtil.Error;
using Encog.MathUtil.Randomize.Generate;
using Encog.Neural.Networks.Training.Propagation;

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

        /**
         * The gradients.
         */
        private double[] gradients;

        /**
         * The deltas for each layer.
         */
        private double[] layerDelta;

        /**
         * L1 regularization.
         */
        public double L1 { get; set; }

        /**
         * L2 regularization.
         */
        public double L2 { get; set; }

        /**
         * The update rule to use.
         */
        private IUpdateRule updateRule = new AdamUpdate();

        /**
         * The last delta values.
         */
        private double[] _lastDelta;

        /**
         * A flat neural network.
         */
        private FlatNetwork _flat;

        /**
         * The error function to use.
         */
        private IErrorFunction _errorFunction = new CrossEntropyErrorFunction();

        /**
         * The error calculation.
         */
        private ErrorCalculation errorCalculation;

        private IGenerateRandom rnd;

        private IMLMethod _method;

        public StochasticGradientDescent(TrainingImplementationType implementationType) : base(implementationType)
        {
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

        public override void Iteration()
        {

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
        
    }
}
