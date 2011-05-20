using System;
using System.IO;
using Encog.App.Analyst.Script.Prop;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Folded;
using Encog.ML.Factory;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Cross;
using Encog.Persist;
using Encog.Util.Logging;
using Encog.Util.Simple;
using Encog.Util.Validate;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to perform training on a machine learning method and
    /// dataset.
    /// </summary>
    ///
    public class CmdTrain : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String CommandName = "TRAIN";

        /// <summary>
        /// The number of folds, if kfold is used.
        /// </summary>
        ///
        private int _kfold;

        /// <summary>
        /// Construct the train command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdTrain(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return CommandName; }
        }

        /// <summary>
        /// Create a trainer, use cross validation if enabled.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <returns>The trainer.</returns>
        private MLTrain CreateTrainer(MLMethod method,
                                      MLDataSet trainingSet)
        {
            var factory = new MLTrainFactory();

            String type = Prop.GetPropertyString(
                ScriptProperties.MlTrainType);
            String args = Prop.GetPropertyString(
                ScriptProperties.MlTrainArguments);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "training type:" + type);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "training args:" + args);

            MLTrain train = factory.Create(method, trainingSet, type, args);

            if (_kfold > 0)
            {
                train = new CrossValidationKFold(train, _kfold);
            }

            return train;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed bool ExecuteCommand(String args)
        {
            _kfold = ObtainCross();
            MLDataSet trainingSet = ObtainTrainingSet();
            MLMethod method = ObtainMethod();
            MLTrain trainer = CreateTrainer(method, trainingSet);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning training");

            PerformTraining(trainer, method, trainingSet);

            String resourceID = Prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);
            FileInfo resourceFile = Analyst.Script.ResolveFilename(
                resourceID);
            EncogDirectoryPersistence.SaveObject(resourceFile, method);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "save to:" + resourceID);

            return Analyst.ShouldStopCommand();
        }


        /// <summary>
        /// Obtain the number of folds for cross validation.
        /// </summary>
        ///
        /// <returns>The number of folds.</returns>
        private int ObtainCross()
        {
            String cross = Prop.GetPropertyString(
                ScriptProperties.MlTrainCross);
            if (string.IsNullOrEmpty(cross))
            {
                return 0;
            }
            if (cross.ToLower().StartsWith("kfold:"))
            {
                String str = cross.Substring(6);
                try
                {
                    return Int32.Parse(str);
                }
                catch (FormatException)
                {
                    throw new AnalystError("Invalid kfold :" + str);
                }
            }
            throw new AnalystError("Unknown cross validation: " + cross);
        }

        /// <summary>
        /// Obtain the ML method.
        /// </summary>
        ///
        /// <returns>The method.</returns>
        private MLMethod ObtainMethod()
        {
            String resourceID = Prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);
            FileInfo resourceFile = Script.ResolveFilename(resourceID);

            var method = EncogDirectoryPersistence
                                        .LoadObject(resourceFile);

            if (!(method is MLMethod))
            {
                throw new AnalystError(
                    "The object to be trained must be an instance of MLMethod. "
                    + method.GetType().Name);
            }

            return (MLMethod)method;
        }

        /// <summary>
        /// Obtain the training set.
        /// </summary>
        ///
        /// <returns>The training set.</returns>
        private MLDataSet ObtainTrainingSet()
        {
            String trainingID = Prop.GetPropertyString(
                ScriptProperties.MlConfigTrainingFile);

            FileInfo trainingFile = Script.ResolveFilename(trainingID);

            MLDataSet trainingSet = EncogUtility.LoadEGB2Memory(trainingFile);

            if (_kfold > 0)
            {
                trainingSet = new FoldedDataSet(trainingSet);
            }

            return trainingSet;
        }

        /// <summary>
        /// Perform the training.
        /// </summary>
        ///
        /// <param name="train">The training method.</param>
        /// <param name="method">The ML method.</param>
        /// <param name="trainingSet">The training set.</param>
        private void PerformTraining(MLTrain train, MLMethod method,
                                     MLDataSet trainingSet)
        {
            ValidateNetwork.ValidateMethodToData(method, trainingSet);
            double targetError = Prop.GetPropertyDouble(
                ScriptProperties.MlTrainTargetError);
            Analyst.ReportTrainingBegin();
            int maxIteration = Analyst.MaxIteration;

            do
            {
                train.Iteration();
                Analyst.ReportTraining(train);
            } while ((train.Error > targetError)
                     && !Analyst.ShouldStopCommand()
                     && !train.TrainingDone
                     && ((maxIteration == -1) || (train.IterationNumber < maxIteration)));
            train.FinishTraining();

            Analyst.ReportTrainingEnd();
        }
    }
}