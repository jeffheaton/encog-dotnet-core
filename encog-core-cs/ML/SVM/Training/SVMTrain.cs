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
using Encog.MathUtil.Error;
using Encog.MathUtil.LIBSVM;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;
using Encog.Util.Logging;

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
        public const double DefaultConstBegin = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        ///
        private readonly svm_problem _problem;

        /// <summary>
        /// The const c value.
        /// </summary>
        ///
        private double _c;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The gamma value.
        /// </summary>
        ///
        private double _gamma;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The network to train.</param>
        /// <param name="dataSet">The training data for this network.</param>
        public SVMTrain(SupportVectorMachine method, IMLDataSet dataSet) : base(TrainingImplementationType.OnePass)
        {
            _fold = 0;
            _network = method;
            Training = dataSet;
            _trainingDone = false;

            _problem = EncodeSVMProblem.Encode(dataSet, 0);
            _gamma = 1.0d/_network.InputCount;
            _c = 1.0d;
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
            get { return _c; }
            set
            {
                if (value <= 0 || value < EncogFramework.DefaultDoubleEqual)
                {
                    throw new EncogError("SVM training cannot use a c value less than zero.");
                }

                _c = value;
            }
        }


        /// <summary>
        /// Set the number of folds.
        /// </summary>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <summary>
        /// Set the gamma.
        /// </summary>
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                if (value <= 0 || value < EncogFramework.DefaultDoubleEqual)
                {
                    throw new EncogError("SVM training cannot use a gamma value less than zero.");
                }
                _gamma = value;
            }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>The problem being trained.</value>
        public svm_problem Problem
        {
            get { return _problem; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        ///
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private static double Evaluate(svm_parameter param, svm_problem prob,
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
            for (int i = 0; i < prob.l; i++)
            {
                if (target[i] == prob.y[i])
                {
                    ++totalCorrect;
                }
            }

            return Format.HundredPercent*totalCorrect/prob.l;
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
            _network.Params.C = _c;
            _network.Params.gamma = _gamma;

            EncogLogging.Log(EncogLogging.LevelInfo, "Training with parameters C = " + _c + ", gamma = " + _gamma);

            if (_fold > 1)
            {
                // cross validate
                var target = new double[_problem.l];

                svm.svm_cross_validation(_problem, _network.Params,
                                         _fold, target);
                _network.Model = null;

                Error = Evaluate(_network.Params, _problem, target);
            }
            else
            {
                // train
                _network.Model = svm.svm_train(_problem,
                                              _network.Params);

                Error = _network.CalculateError(Training);
            }

            _trainingDone = true;
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
