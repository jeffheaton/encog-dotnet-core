//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.MathUtil.Error;
using Encog.MathUtil.LIBSVM;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.ML.SVM.Training
{
    /// <summary>
    /// Provides training for Support Vector Machine networks.
    /// </summary>
    ///
    public class SVMTrain : BasicTraining
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
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        ///
        private readonly svm_problem problem;

        /// <summary>
        /// The const c value.
        /// </summary>
        ///
        private double c;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int fold;

        /// <summary>
        /// The gamma value.
        /// </summary>
        ///
        private double gamma;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The network to train.</param>
        /// <param name="dataSet">The training data for this network.</param>
        public SVMTrain(SupportVectorMachine method, IMLDataSet dataSet) : base(TrainingImplementationType.OnePass)
        {
            fold = 0;
            network = method;
            Training = dataSet;
            trainingDone = false;

            problem = EncodeSVMProblem.Encode(dataSet, 0);
            gamma = 1.0d/network.InputCount;
            c = 1.0d;
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the constant C.
        /// </summary>
        public double C
        {
            get { return c; }
            set { c = value; }
        }


        /// <summary>
        /// Set the number of folds.
        /// </summary>
        public int Fold
        {
            get { return fold; }
            set { fold = value; }
        }


        /// <summary>
        /// Set the gamma.
        /// </summary>
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return network; }
        }


        /// <value>The problem being trained.</value>
        public svm_problem Problem
        {
            get { return problem; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return trainingDone; }
        }

        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        ///
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private double Evaluate(svm_parameter param, svm_problem prob,
                                double[] target)
        {
            int totalCorrect = 0;

            var error = new ErrorCalculation();

            if ((param.svm_type == svm_parameter.EPSILON_SVR)
                || (param.svm_type == svm_parameter.NU_SVR))
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
                for (int i_0 = 0; i_0 < prob.l; i_0++)
                {
                    if (target[i_0] == prob.y[i_0])
                    {
                        ++totalCorrect;
                    }
                }

                return Format.HUNDRED_PERCENT*totalCorrect/prob.l;
            }
        }


        /// <summary>
        /// Perform either a train or a cross validation.  If the folds property is 
        /// greater than 1 then cross validation will be done.  Cross validation does 
        /// not produce a usable model, but it does set the error. 
        /// If you are cross validating try C and Gamma values until you have a good 
        /// error rate.  Then use those values to train, producing the final model.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            network.Params.C = c;
            network.Params.gamma = gamma;

            if (fold > 1)
            {
                // cross validate
                var target = new double[problem.l];

                svm.svm_cross_validation(problem, network.Params,
                                         fold, target);
                network.Model = null;

                Error = Evaluate(network.Params, problem, target);
            }
            else
            {
                // train
                network.Model = svm.svm_train(problem,
                                              network.Params);

                Error = network.CalculateError(Training);
            }

            trainingDone = true;
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
    }
}
