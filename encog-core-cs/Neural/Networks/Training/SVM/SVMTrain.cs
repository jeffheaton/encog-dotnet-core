using System;
using Encog.MathUtil.Error;
using Encog.ML.Data;
using Encog.MathUtil.LIBSVM;
using Encog.ML.SVM;

namespace Encog.Neural.Networks.Training.SVM
{
    /// <summary>
    /// Provides training for Support Vector Machine networks.
    /// </summary>
    public class SVMTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        public const double DEFAULT_CONST_BEGIN = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        public const double DEFAULT_CONST_END = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        public const double DEFAULT_CONST_STEP = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        public const double DEFAULT_GAMMA_BEGIN = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        public const double DEFAULT_GAMMA_END = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        public const double DEFAULT_GAMMA_STEP = 1;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        private SVMNetwork network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        private svm_problem[] problem;

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int Fold { get; set; }


        /// <summary>
        /// The beginning value for C.
        /// </summary>
        public double ConstBegin { get; set; }

        /// <summary>
        /// The step value for C.
        /// </summary>
        public double ConstStep { get; set; }

        /// <summary>
        /// The ending value for C.
        /// </summary>
        public double ConstEnd { get; set; }

        /// <summary>
        /// The beginning value for gamma.
        /// </summary>
        public double GammaBegin { get; set; }

        /// <summary>
        /// The ending value for gamma.
        /// </summary>
        public double GammaEnd { get; set; }

        /// <summary>
        /// The step value for gamma.
        /// </summary>
        public double GammaStep { get; set; }

        /// <summary>
        /// The best values found for C.
        /// </summary>
        private double[] bestConst;

        /// <summary>
        /// The best values found for gamma.
        /// </summary>
        private double[] bestGamma;

        /// <summary>
        /// The best error.
        /// </summary>
        private double[] bestError;

        /// <summary>
        /// The current C.
        /// </summary>
        private double[] currentConst;

        /// <summary>
        /// The current gamma.
        /// </summary>
        private double[] currentGamma;

        /// <summary>
        /// Is the network setup.
        /// </summary>
        private bool isSetup;

        /// <summary>
        /// Is the training done.
        /// </summary>
        private bool trainingDone;

        
        /// <summary>
        /// Construct a trainer for an SVM network. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data for this network.</param>
        public SVMTrain(BasicNetwork network, MLDataSet training)
        {
            this.network = (SVMNetwork)network;
            this.Training = training;
            this.isSetup = false;
            this.trainingDone = false;
            this.Fold = 5;
 
            this.ConstBegin = DEFAULT_CONST_BEGIN;
            this.ConstStep = DEFAULT_CONST_END;
            this.ConstEnd = DEFAULT_CONST_STEP;
            this.GammaBegin = DEFAULT_GAMMA_BEGIN;
            this.GammaEnd = DEFAULT_GAMMA_END;
            this.GammaStep = DEFAULT_GAMMA_STEP;

            this.problem = new svm_problem[this.network.OutputCount];

            for (int i = 0; i < this.network.OutputCount; i++)
            {                
                this.problem[i] = EncodeSVMProblem.Encode(training, i);
            }
        }

        /// <summary>
        /// Quickly train all outputs with a C of 1.0 and a gamma equal to 1/(num inputs).
        /// </summary>
        public void Train()
        {
            double gamma = 1.0 / this.network.InputCount;
            double c = 1.0;

            for (int i = 0; i < network.OutputCount; i++)
                Train(i, gamma, c);
        }

        /// <summary>
        /// Quickly train one output with the specified gamma and C.
        /// </summary>
        /// <param name="index">The output to train.</param>
        /// <param name="gamma">The gamma to train with.</param>
        /// <param name="c">The C to train with.</param>
        public void Train(int index, double gamma, double c)
        {
            network.Params[index].C = c;

            if (gamma > EncogFramework.DEFAULT_DOUBLE_EQUAL)
            {
                network.Params[index].gamma = 1.0 / this.network.InputCount;
            }
            else
            {
                network.Params[index].gamma = gamma;
            }

            network.Models[index] = svm.svm_train(problem[index], network
                    .Params[index]);
        }


        /// <summary>
        /// Cross validate and check the specified index/gamma.
        /// </summary>
        /// <param name="index">The output index to cross validate.</param>
        /// <param name="gamma">The gamma to check.</param>
        /// <param name="c">The C to check.</param>
        /// <returns>The calculated error.</returns>
        public double CrossValidate(int index, double gamma, double c)
        {

            double[] target = new double[this.problem[0].l];

            network.Params[index].C = c;
            network.Params[index].gamma = gamma;
            svm.svm_cross_validation(problem[index], network.Params[index], Fold,
                    target);
            return Evaluate(network.Params[index], problem[index], target);
        }


        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private double Evaluate(svm_parameter param, svm_problem prob,
                double[] target)
        {
            int total_correct = 0;

            ErrorCalculation error = new ErrorCalculation();

            if (param.svm_type == svm_parameter.EPSILON_SVR
                    || param.svm_type == svm_parameter.NU_SVR)
            {
                for (int i = 0; i < prob.l; i++)
                {
                    double ideal = prob.y[i];
                    double actual = target[i];
                    error.UpdateError(actual, ideal);
                }
                return error.Calculate();
            }
            else
            {
                for (int i = 0; i < prob.l; i++)
                    if (target[i] == prob.y[i])
                        ++total_correct;

                return 100.0 * total_correct / prob.l;
            }
        }

        /// <summary>
        /// Setup to train the SVM.
        /// </summary>
        private void Setup()
        {
            this.currentConst = new double[this.network.OutputCount];
            this.currentGamma = new double[this.network.OutputCount];
            this.bestConst = new double[this.network.OutputCount];
            this.bestGamma = new double[this.network.OutputCount];
            this.bestError = new double[this.network.OutputCount];


            for (int i = 0; i < this.network.OutputCount; i++)
            {
                this.currentConst[i] = this.ConstBegin;
                this.currentGamma[i] = this.GammaBegin;
                this.bestError[i] = Double.PositiveInfinity;
            }
            this.isSetup = true;
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            if (!trainingDone)
            {
                if (!isSetup)
                    Setup();

                PreIteration();

                if (network.KernelTypeUsed == KernelType.RadialBasisFunction)
                {

                    double totalError = 0;

                    for (int i = 0; i < this.network.OutputCount; i++)
                    {
                        double e = this.CrossValidate(i, this.currentGamma[i],
                                currentConst[i]);

                        if (e < bestError[i])
                        {
                            this.bestConst[i] = this.currentConst[i];
                            this.bestGamma[i] = this.currentGamma[i];
                            this.bestError[i] = e;
                        }

                        this.currentConst[i] += this.ConstStep;
                        if (this.currentConst[i] > this.ConstEnd)
                        {
                            this.currentConst[i] = this.ConstBegin;
                            this.currentGamma[i] += this.GammaStep;
                            if (this.currentGamma[i] > this.GammaEnd)
                                this.trainingDone = true;
                        }

                        totalError += this.bestError[i];
                    }

                    this.Error = (totalError / this.network.OutputCount);
                }
                else
                {
                    Train();
                }

                PostIteration();
            }
        }

        /// <summary>
        /// The problem being trained.
        /// </summary>
        public svm_problem[] Problem
        {
            get
            {
                return problem;
            }
        }


        /// <summary>
        /// Called to finish training.
        /// </summary>
        public override void FinishTraining()
        {
            base.FinishTraining();
            for (int i = 0; i < network.OutputCount; i++)
            {
                Train(i, this.bestGamma[i], this.bestConst[i]);
            }
        }

        /// <summary>
        /// The trained network.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// True if the training is done.
        /// </summary>
        public bool IsTrainingDone
        {
            get
            {
                return this.trainingDone;
            }
        }

        
        /// <summary>
        /// Quickly train the network with a fixed gamma and C.
        /// </summary>
        /// <param name="gamma">The gamma to use.</param>
        /// <param name="c">The C to use.</param>
        public void Train(double gamma, double c)
        {
            for (int i = 0; i < this.network.OutputCount; i++)
            {
                Train(i, gamma, c);
            }

        }
    }
}
