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
using System;
using System.Collections.Generic;
using System.Text;
using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data;
using Encog.ML.Data.Cross;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Division;
using Encog.ML.Factory;
using Encog.ML.Model.Config;
using Encog.ML.Train;
using Encog.ML.Train.Strategy.End;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.ML.Model
{
    /// <summary>
    ///     Encog model is designed to allow you to easily swap between different model
    ///     types and automatically normalize data.  It is designed to work with a
    ///     VersatileMLDataSet only.
    /// </summary>
    public class EncogModel
    {
        /// <summary>
        ///     The dataset to use.
        /// </summary>
        private readonly VersatileMLDataSet _dataset;

        /// <summary>
        ///     The input features.
        /// </summary>
        private readonly IList<ColumnDefinition> _inputFeatures = new List<ColumnDefinition>();

        /// <summary>
        ///     The standard configrations for each method type.
        /// </summary>
        private readonly IDictionary<string, IMethodConfig> _methodConfigurations =
            new Dictionary<string, IMethodConfig>();

        /// <summary>
        ///     The predicted features.
        /// </summary>
        private readonly IList<ColumnDefinition> _predictedFeatures = new List<ColumnDefinition>();

        /// <summary>
        ///     The current method configuration, determined by the selected model.
        /// </summary>
        private IMethodConfig _config;

        /// <summary>
        ///     The method arguments for the selected method.
        /// </summary>
        private string _methodArgs;

        /// <summary>
        ///     The selected method type.
        /// </summary>
        private string _methodType;

        /// <summary>
        ///     The training arguments for the selected training type.
        /// </summary>
        private string _trainingArgs;

        /// <summary>
        ///     The selected training type.
        /// </summary>
        private string _trainingType;

        /// <summary>
        /// The cross validated score.
        /// </summary>
        public double CrossValidatedScore { get; set; }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public EncogModel()
        {
            Report = new NullStatusReportable();
        }

        /// <summary>
        ///     Construct a model for the specified dataset.
        /// </summary>
        /// <param name="theDataset">The dataset.</param>
        public EncogModel(VersatileMLDataSet theDataset) : this()
        {
            _dataset = theDataset;
            _methodConfigurations[MLMethodFactory.TypeFeedforward] = new FeedforwardConfig();
            _methodConfigurations[MLMethodFactory.TypeSVM] = new SVMConfig();
            _methodConfigurations[MLMethodFactory.TypeRbfnetwork] = new RBFNetworkConfig();
            _methodConfigurations[MLMethodFactory.TypeNEAT] = new NEATConfig();
            _methodConfigurations[MLMethodFactory.TypePNN] = new PNNConfig();
        }

        /// <summary>
        ///     The training dataset.
        /// </summary>
        public MatrixMLDataSet TrainingDataset { get; set; }

        /// <summary>
        ///     The validation dataset.
        /// </summary>
        public MatrixMLDataSet ValidationDataset { get; set; }

        /// <summary>
        ///     The report.
        /// </summary>
        public IStatusReportable Report { get; set; }

        /// <summary>
        ///     The data set.
        /// </summary>
        public VersatileMLDataSet Dataset
        {
            get { return _dataset; }
        }

        /// <summary>
        ///     The input features.
        /// </summary>
        public IList<ColumnDefinition> InputFeatures
        {
            get { return _inputFeatures; }
        }

        /// <summary>
        ///     The predicted features.
        /// </summary>
        public IList<ColumnDefinition> PredictedFeatures
        {
            get { return _predictedFeatures; }
        }

        /// <summary>
        ///     The method configurations.
        /// </summary>
        public IDictionary<String, IMethodConfig> MethodConfigurations
        {
            get { return _methodConfigurations; }
        }

        /// <summary>
        ///     Specify a validation set to hold back.
        /// </summary>
        /// <param name="validationPercent">The percent to use for validation.</param>
        /// <param name="shuffle">True to shuffle.</param>
        /// <param name="seed">The seed for random generation.</param>
        public void HoldBackValidation(double validationPercent, bool shuffle,
            int seed)
        {
            IList<DataDivision> dataDivisionList = new List<DataDivision>();
            dataDivisionList.Add(new DataDivision(1.0 - validationPercent)); // Training
            dataDivisionList.Add(new DataDivision(validationPercent)); // Validation
            _dataset.Divide(dataDivisionList, shuffle,
                new MersenneTwisterGenerateRandom((uint) seed));
            TrainingDataset = dataDivisionList[0].Dataset;
            ValidationDataset = dataDivisionList[1].Dataset;
        }

        /// <summary>
        ///     Fit the model using cross validation.
        /// </summary>
        /// <param name="k">The number of folds total.</param>
        /// <param name="foldNum">The current fold.</param>
        /// <param name="fold">The current fold.</param>
        private void FitFold(int k, int foldNum, DataFold fold)
        {
            IMLMethod method = CreateMethod();
            IMLTrain train = CreateTrainer(method, fold.Training);

            if (train.ImplementationType == TrainingImplementationType.Iterative)
            {
                var earlyStop = new SimpleEarlyStoppingStrategy(
                    fold.Validation);
                train.AddStrategy(earlyStop);

                var line = new StringBuilder();
                while (!train.TrainingDone)
                {
                    train.Iteration();
                    line.Length = 0;
                    line.Append("Fold #");
                    line.Append(foldNum);
                    line.Append("/");
                    line.Append(k);
                    line.Append(": Iteration #");
                    line.Append(train.IterationNumber);
                    line.Append(", Training Error: ");
                    line.Append(Format.FormatDouble(train.Error, 8));
                    line.Append(", Validation Error: ");
                    line.Append(Format.FormatDouble(earlyStop.ValidationError,
                        8));
                    Report.Report(k, foldNum, line.ToString());
                }
                fold.Score = earlyStop.ValidationError;
                fold.Method = method;
            }
            else if (train.ImplementationType == TrainingImplementationType.OnePass)
            {
                train.Iteration();
                double validationError = CalculateError(method,
                    fold.Validation);
                Report.Report(k, k,
                    "Trained, Training Error: " + train.Error
                    + ", Validatoin Error: " + validationError);
                fold.Score = validationError;
                fold.Method = method;
            }
            else
            {
                throw new EncogError("Unsupported training type for EncogModel: "
                                     + train.ImplementationType);
            }
        }

        /// <summary>
        ///     Calculate the error for the given method and dataset.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="data">The data to use.</param>
        /// <returns>The error.</returns>
        public double CalculateError(IMLMethod method, IMLDataSet data)
        {
            if (_dataset.NormHelper.OutputColumns.Count == 1)
            {
                ColumnDefinition cd = _dataset.NormHelper
                    .OutputColumns[0];
                if (cd.DataType == ColumnType.Nominal)
                {
                    return EncogUtility.CalculateClassificationError(
                        (IMLClassification) method, data);
                }
            }

            return EncogUtility.CalculateRegressionError((IMLRegression) method,
                data);
        }

        /// <summary>
        ///     Create a trainer.
        /// </summary>
        /// <param name="method">The method to train.</param>
        /// <param name="dataset">The dataset.</param>
        /// <returns>The trainer.</returns>
        private IMLTrain CreateTrainer(IMLMethod method, IMLDataSet dataset)
        {
            if (_trainingType == null)
            {
                throw new EncogError(
                    "Please call selectTraining first to choose how to train.");
            }
            var trainFactory = new MLTrainFactory();
            IMLTrain train = trainFactory.Create(method, dataset, _trainingType,
                _trainingArgs);
            return train;
        }

        /// <summary>
        ///     Crossvalidate and fit.
        /// </summary>
        /// <param name="k">The number of folds.</param>
        /// <param name="shuffle">True if we should shuffle.</param>
        /// <returns>The trained method.</returns>
        public IMLMethod Crossvalidate(int k, bool shuffle)
        {
            var cross = new KFoldCrossvalidation(
                TrainingDataset, k);
            cross.Process(shuffle);

            int foldNumber = 0;
            foreach (DataFold fold in cross.Folds)
            {
                foldNumber++;
                Report.Report(k, foldNumber, "Fold #" + foldNumber);
                FitFold(k, foldNumber, fold);
            }

            double sum = 0;
            double bestScore = Double.PositiveInfinity;
            IMLMethod bestMethod = null;
            foreach (DataFold fold in cross.Folds)
            {
                sum += fold.Score;
                if (fold.Score < bestScore)
                {
                    bestScore = fold.Score;
                    bestMethod = fold.Method;
                }
            }
            sum = sum/cross.Folds.Count;
            Report.Report(k, k, "Cross-validated score:" + sum);
            CrossValidatedScore = sum;
            return bestMethod;
        }

       

        /// <summary>
        ///     Create the selected method.
        /// </summary>
        /// <returns>The created method.</returns>
        public IMLMethod CreateMethod()
        {
            if (_methodType == null)
            {
                throw new EncogError(
                    "Please call selectMethod first to choose what type of method you wish to use.");
            }
            var methodFactory = new MLMethodFactory();
            IMLMethod method = methodFactory.Create(_methodType, _methodArgs, _dataset
                .NormHelper.CalculateNormalizedInputCount(), _config
                    .DetermineOutputCount(_dataset));
            return method;
        }

        /// <summary>
        ///     Select the method to create.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="methodType">The method type.</param>
        public void SelectMethod(VersatileMLDataSet dataset, String methodType)
        {
            if (!_methodConfigurations.ContainsKey(methodType))
            {
                throw new EncogError("Don't know how to autoconfig method: "
                                     + methodType);
            }

            _config = _methodConfigurations[methodType];
            _methodType = methodType;
            _methodArgs = _config.SuggestModelArchitecture(dataset);
            dataset.NormHelper.NormStrategy =
                _config.SuggestNormalizationStrategy(dataset, _methodArgs);
        }

        /// <summary>
        ///     Select the method to use.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="methodType">The type of method.</param>
        /// <param name="methodArgs">The method arguments.</param>
        /// <param name="trainingType">The training type.</param>
        /// <param name="trainingArgs">The training arguments.</param>
        public void SelectMethod(VersatileMLDataSet dataset, String methodType,
            String methodArgs, String trainingType, String trainingArgs)
        {
            if (!_methodConfigurations.ContainsKey(methodType))
            {
                throw new EncogError("Don't know how to autoconfig method: "
                                     + methodType);
            }


            _config = _methodConfigurations[methodType];
            _methodType = methodType;
            _methodArgs = methodArgs;
            dataset.NormHelper.NormStrategy =
                _methodConfigurations[methodType]
                    .SuggestNormalizationStrategy(dataset, methodArgs);
        }

        /// <summary>
        ///     Select the training type.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public void SelectTrainingType(VersatileMLDataSet dataset)
        {
            if (_methodType == null)
            {
                throw new EncogError(
                    "Please select your training method, before your training type.");
            }
            IMethodConfig config = _methodConfigurations[_methodType];
            SelectTraining(dataset, config.SuggestTrainingType(),
                config.SuggestTrainingArgs(_trainingType));
        }

        /// <summary>
        ///     Select the training to use.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="trainingType">The type of training.</param>
        /// <param name="trainingArgs">The training arguments.</param>
        public void SelectTraining(VersatileMLDataSet dataset, String trainingType,
            String trainingArgs)
        {
            if (_methodType == null)
            {
                throw new EncogError(
                    "Please select your training method, before your training type.");
            }

            _trainingType = trainingType;
            _trainingArgs = trainingArgs;
        }
    }
}
