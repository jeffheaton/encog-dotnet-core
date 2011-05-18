using System;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.SVM.Training
{
    /// <summary>
    /// Provides training for Support Vector Machine networks.
    /// </summary>
    ///
    public class SVMSearchTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DEFAULT_CONST_BEGIN = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DEFAULT_CONST_END = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DEFAULT_CONST_STEP = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DEFAULT_GAMMA_BEGIN = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DEFAULT_GAMMA_END = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DEFAULT_GAMMA_STEP = 1;

        /// <summary>
        /// The internal training object, used for the search.
        /// </summary>
        ///
        private readonly SVMTrain internalTrain;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine network;

        /// <summary>
        /// The best values found for C.
        /// </summary>
        ///
        private double bestConst;

        /// <summary>
        /// The best error.
        /// </summary>
        ///
        private double bestError;

        /// <summary>
        /// The best values found for gamma.
        /// </summary>
        ///
        private double bestGamma;

        /// <summary>
        /// The beginning value for C.
        /// </summary>
        ///
        private double constBegin;

        /// <summary>
        /// The ending value for C.
        /// </summary>
        ///
        private double constEnd;

        /// <summary>
        /// The step value for C.
        /// </summary>
        ///
        private double constStep;

        /// <summary>
        /// The current C.
        /// </summary>
        ///
        private double currentConst;

        /// <summary>
        /// The current gamma.
        /// </summary>
        ///
        private double currentGamma;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int fold;

        /// <summary>
        /// The beginning value for gamma.
        /// </summary>
        ///
        private double gammaBegin;

        /// <summary>
        /// The ending value for gamma.
        /// </summary>
        ///
        private double gammaEnd;

        /// <summary>
        /// The step value for gamma.
        /// </summary>
        ///
        private double gammaStep;

        /// <summary>
        /// Is the network setup.
        /// </summary>
        ///
        private bool isSetup;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data for this network.</param>
        public SVMSearchTrain(SupportVectorMachine method, MLDataSet training)
            : base(TrainingImplementationType.Iterative)
        {
            fold = 0;
            constBegin = DEFAULT_CONST_BEGIN;
            constStep = DEFAULT_CONST_STEP;
            constEnd = DEFAULT_CONST_END;
            gammaBegin = DEFAULT_GAMMA_BEGIN;
            gammaEnd = DEFAULT_GAMMA_END;
            gammaStep = DEFAULT_GAMMA_STEP;
            network = method;
            Training = training;
            isSetup = false;
            trainingDone = false;

            internalTrain = new SVMTrain(network, training);
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the constBegin to set</value>
        public double ConstBegin
        {
            get { return constBegin; }
            set { constBegin = value; }
        }


        /// <value>the constEnd to set</value>
        public double ConstEnd
        {
            get { return constEnd; }
            set { constEnd = value; }
        }


        /// <value>the constStep to set</value>
        public double ConstStep
        {
            get { return constStep; }
            set { constStep = value; }
        }


        /// <value>the fold to set</value>
        public int Fold
        {
            get { return fold; }
            set { fold = value; }
        }


        /// <value>the gammaBegin to set</value>
        public double GammaBegin
        {
            get { return gammaBegin; }
            set { gammaBegin = value; }
        }


        /// <value>the gammaEnd to set.</value>
        public double GammaEnd
        {
            get { return gammaEnd; }
            set { gammaEnd = value; }
        }


        /// <value>the gammaStep to set</value>
        public double GammaStep
        {
            get { return gammaStep; }
            set { gammaStep = value; }
        }


        /// <inheritdoc/>
        public override MLMethod Method
        {
            get { return network; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return trainingDone; }
        }

        /// <inheritdoc/>
        public override sealed void FinishTraining()
        {
            internalTrain.Gamma = bestGamma;
            internalTrain.C = bestConst;
            internalTrain.Iteration();
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override sealed void Iteration()
        {
            if (!trainingDone)
            {
                if (!isSetup)
                {
                    Setup();
                }

                PreIteration();

                internalTrain.Fold = fold;

                if (network.KernelType == KernelType.RadialBasisFunction)
                {
                    internalTrain.Gamma = currentGamma;
                    internalTrain.C = currentConst;
                    internalTrain.Iteration();
                    double e = internalTrain.Error;

                    //System.out.println(this.currentGamma + "," + this.currentConst
                    //		+ "," + e);

                    // new best error?
                    if (!Double.IsNaN(e))
                    {
                        if (e < bestError)
                        {
                            bestConst = currentConst;
                            bestGamma = currentGamma;
                            bestError = e;
                        }
                    }

                    // advance
                    currentConst += constStep;
                    if (currentConst > constEnd)
                    {
                        currentConst = constBegin;
                        currentGamma += gammaStep;
                        if (currentGamma > gammaEnd)
                        {
                            trainingDone = true;
                        }
                    }

                    Error = bestError;
                }
                else
                {
                    internalTrain.Gamma = currentGamma;
                    internalTrain.C = currentConst;
                    internalTrain.Iteration();
                }

                PostIteration();
            }
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup to train the SVM.
        /// </summary>
        ///
        private void Setup()
        {
            currentConst = constBegin;
            currentGamma = gammaBegin;
            bestError = Double.PositiveInfinity;
            isSetup = true;
        }
    }
}