using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.ML.Factory;
using Encog.Util.Arrayutil;
using Encog.Util.File;

namespace Encog.App.Analyst.Wizard
{
    /// <summary>
    /// The Encog Analyst Wizard can be used to create Encog Analyst script files
    /// from a CSV file. This class is typically used by the Encog Workbench, but it
    /// can easily be used from any program to create a starting point for an Encog
    /// Analyst Script.
    /// Several items must be provided to the wizard.
    /// Desired Machine Learning Method: This is the machine learning method that you
    /// would like the wizard to use. This might be a neural network, SVM or other
    /// supported method.
    /// Normalization Range: This is the range that the data should be normalized
    /// into. Some machine learning methods perform better with different ranges. The
    /// two ranges supported by the wizard are -1 to 1 and 0 to 1.
    /// Goal: What are we trying to accomplish. Is this a classification, regression
    /// or autoassociation problem.
    /// </summary>
    ///
    public class AnalystWizard
    {
        /// <summary>
        /// The default training percent.
        /// </summary>
        ///
        public const int DefaultTrainPercent = 75;

        /// <summary>
        /// The default evaluation percent.
        /// </summary>
        ///
        public const int DefaultEvalPercent = 25;

        /// <summary>
        /// The default training error.
        /// </summary>
        ///
        public const double DefaultTrainError = 0.01d;

        /// <summary>
        /// The raw file.
        /// </summary>
        ///
        public const String FileRaw = "FILE_RAW";

        /// <summary>
        /// The normalized file.
        /// </summary>
        ///
        public const String FileNormalize = "FILE_NORMALIZE";

        /// <summary>
        /// The randomized file.
        /// </summary>
        ///
        public const String FileRandom = "FILE_RANDOMIZE";

        /// <summary>
        /// The training file.
        /// </summary>
        ///
        public const String FileTrain = "FILE_TRAIN";

        /// <summary>
        /// The evaluation file.
        /// </summary>
        ///
        public const String FileEval = "FILE_EVAL";

        /// <summary>
        /// The eval file normalization file.
        /// </summary>
        ///
        public const String FileEvalNorm = "FILE_EVAL_NORM";

        /// <summary>
        /// The training set.
        /// </summary>
        ///
        public const String FileTrainset = "FILE_TRAINSET";

        /// <summary>
        /// The machine learning file.
        /// </summary>
        ///
        public const String FileMl = "FILE_ML";

        /// <summary>
        /// The output file.
        /// </summary>
        ///
        public const String FileOutput = "FILE_OUTPUT";

        /// <summary>
        /// The balanced file.
        /// </summary>
        ///
        public const String FileBalance = "FILE_BALANCE";

        /// <summary>
        /// The clustered file.
        /// </summary>
        ///
        public const String FileCluster = "FILE_CLUSTER";

        /// <summary>
        /// The analyst.
        /// </summary>
        ///
        private readonly EncogAnalyst _analyst;

        /// <summary>
        /// The analyst script.
        /// </summary>
        ///
        private readonly AnalystScript _script;

        /// <summary>
        /// Are we using single-field(direct) classification.
        /// </summary>
        ///
        private bool _directClassification;

        /// <summary>
        /// The balance filename.
        /// </summary>
        ///
        private String _filenameBalance;

        /// <summary>
        /// The cluster filename.
        /// </summary>
        ///
        private String _filenameCluster;

        /// <summary>
        /// The evaluation filename.
        /// </summary>
        ///
        private String _filenameEval;

        /// <summary>
        /// The normalization eval file name.
        /// </summary>
        ///
        private String _filenameEvalNorm;

        /// <summary>
        /// The machine learning file name.
        /// </summary>
        ///
        private String _filenameMl;

        /// <summary>
        /// The normalized filename.
        /// </summary>
        ///
        private String _filenameNorm;

        /// <summary>
        /// The output filename.
        /// </summary>
        ///
        private String _filenameOutput;

        /// <summary>
        /// The random file name.
        /// </summary>
        ///
        private String _filenameRandom;

        /// <summary>
        /// The raw filename.
        /// </summary>
        ///
        private String _filenameRaw;

        /// <summary>
        /// The training filename.
        /// </summary>
        ///
        private String _filenameTrain;

        /// <summary>
        /// The training set filename.
        /// </summary>
        ///
        private String _filenameTrainSet;

        /// <summary>
        /// The analyst goal.
        /// </summary>
        ///
        private AnalystGoal _goal;

        /// <summary>
        /// Should the target field be included int he input, if we are doing 
        /// time-series.
        /// </summary>
        ///
        private bool _includeTargetField;

        /// <summary>
        /// The size of the lag window, if we are doing time-series.
        /// </summary>
        ///
        private int _lagWindowSize;

        /// <summary>
        /// The size of the lead window, if we are doing time-series.
        /// </summary>
        ///
        private int _leadWindowSize;

        /// <summary>
        /// The machine learning method that we will be using.
        /// </summary>
        ///
        private WizardMethodType _methodType;

        /// <summary>
        /// The normalization range.
        /// </summary>
        ///
        private NormalizeRange _range;

        /// <summary>
        /// The target field, or "" to detect.
        /// </summary>
        ///
        private String _targetField;

        /// <summary>
        /// True if the balance command should be generated.
        /// </summary>
        ///
        private bool _taskBalance;

        /// <summary>
        /// True if the cluster command should be generated.
        /// </summary>
        ///
        private bool _taskCluster;

        /// <summary>
        /// True if the normalize command should be generated.
        /// </summary>
        ///
        private bool _taskNormalize;

        /// <summary>
        /// True if the randomize command should be generated.
        /// </summary>
        ///
        private bool _taskRandomize;

        /// <summary>
        /// True if the segregate command should be generated.
        /// </summary>
        ///
        private bool _taskSegregate;

        /// <summary>
        /// True if we are doing time-series.
        /// </summary>
        ///
        private bool _timeSeries;

        /// <summary>
        /// Construct the analyst wizard.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        public AnalystWizard(EncogAnalyst theAnalyst)
        {
            _directClassification = false;
            _taskSegregate = true;
            _taskRandomize = true;
            _taskNormalize = true;
            _taskBalance = false;
            _taskCluster = true;
            _range = NormalizeRange.NegOne2One;
            _analyst = theAnalyst;
            _script = _analyst.Script;
            _methodType = WizardMethodType.FeedForward;
            _targetField = "";
            _goal = AnalystGoal.Classification;
            _leadWindowSize = 0;
            _lagWindowSize = 0;
            _includeTargetField = false;
        }

        /// <summary>
        /// Set the goal.
        /// </summary>
        public AnalystGoal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }


        /// <value>the lagWindowSize to set</value>
        public int LagWindowSize
        {
            get { return _lagWindowSize; }
            set { _lagWindowSize = value; }
        }


        /// <value>the leadWindowSize to set</value>
        public int LeadWindowSize
        {
            get { return _leadWindowSize; }
            set { _leadWindowSize = value; }
        }


        /// <value>the methodType to set</value>
        public WizardMethodType MethodType
        {
            get { return _methodType; }
            set { _methodType = value; }
        }


        /// <value>the range to set</value>
        public NormalizeRange Range
        {
            get { return _range; }
            set { _range = value; }
        }


        /// <summary>
        /// Set the target field.
        /// </summary>
        ///
        /// <value>The target field.</value>
        public String TargetField
        {
            get { return _targetField; }
            set { _targetField = value; }
        }


        /// <value>the includeTargetField to set</value>
        public bool IncludeTargetField
        {
            get { return _includeTargetField; }
            set { _includeTargetField = value; }
        }


        /// <value>the taskBalance to set</value>
        public bool TaskBalance
        {
            get { return _taskBalance; }
            set { _taskBalance = value; }
        }


        /// <value>the taskCluster to set</value>
        public bool TaskCluster
        {
            get { return _taskCluster; }
            set { _taskCluster = value; }
        }


        /// <value>the taskNormalize to set</value>
        public bool TaskNormalize
        {
            get { return _taskNormalize; }
            set { _taskNormalize = value; }
        }


        /// <value>the taskRandomize to set</value>
        public bool TaskRandomize
        {
            get { return _taskRandomize; }
            set { _taskRandomize = value; }
        }


        /// <value>the taskSegregate to set</value>
        public bool TaskSegregate
        {
            get { return _taskSegregate; }
            set { _taskSegregate = value; }
        }

        /// <summary>
        /// Create a "set" command to add to a task.
        /// </summary>
        ///
        /// <param name="setTarget">The target.</param>
        /// <param name="setSource">The source.</param>
        /// <returns>The "set" command.</returns>
        private static String CreateSet(String setTarget, String setSource)
        {
            var result = new StringBuilder();
            result.Append("set ");
            result.Append(ScriptProperties.ToDots(setTarget));
            result.Append("=\"");
            result.Append(setSource);
            result.Append("\"");
            return result.ToString();
        }

        /// <summary>
        /// Determine the type of classification used.
        /// </summary>
        ///
        private void DetermineClassification()
        {
            _directClassification = false;

            if ((_methodType == WizardMethodType.SVM)
                || (_methodType == WizardMethodType.SOM))
            {
                _directClassification = true;
            }
        }

        /// <summary>
        /// Determine the target field.
        /// </summary>
        ///
        private void DetermineTargetField()
        {
            IList<AnalystField> fields = _script.Normalize.NormalizedFields;

            if (_targetField.Trim().Length == 0)
            {
                bool success = false;

                if (_goal == AnalystGoal.Classification)
                {
                    // first try to the last classify field
                    foreach (AnalystField field  in  fields)
                    {
                        DataField df = _script.FindDataField(field.Name);
                        if (field.Action.IsClassify() && df.Class)
                        {
                            _targetField = field.Name;
                            success = true;
                        }
                    }
                }
                else
                {
                    // otherwise, just return the last regression field
                    foreach (AnalystField field  in  fields)
                    {
                        DataField df = _script.FindDataField(field.Name);
                        if (!df.Class && (df.Real || df.Integer))
                        {
                            _targetField = field.Name;
                            success = true;
                        }
                    }
                }

                if (!success)
                {
                    throw new AnalystError(
                        "Can't determine target field automatically, "
                        + "please specify one.\nThis can also happen if you "
                        + "specified the wrong file format.");
                }
            }
            else
            {
                if (_script.FindDataField(_targetField) == null)
                {
                    throw new AnalystError("Invalid target field: "
                                           + _targetField);
                }
            }

            _script.Properties.SetProperty(
                ScriptProperties.DataConfigGoal, _goal);

            if (!_timeSeries && _taskBalance)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.BalanceConfigBalanceField,
                    _targetField);
                DataField field = _analyst.Script.FindDataField(
                    _targetField);
                if ((field != null) && field.Class)
                {
                    int countPer = field.MinClassCount;
                    _script.Properties.SetProperty(
                        ScriptProperties.BalanceConfigCountPer, countPer);
                }
            }

            // now that the target field has been determined, set the analyst fields
            AnalystField af = null;

            foreach (AnalystField field  in  _analyst.Script.Normalize.NormalizedFields)
            {
                if ((field.Action != NormalizationAction.Ignore)
                    && field.Name.Equals(_targetField, StringComparison.InvariantCultureIgnoreCase))
                {
                    if ((af == null) || (af.TimeSlice < field.TimeSlice))
                    {
                        af = field;
                    }
                }
            }

            if (af != null)
            {
                af.Output = true;
            }

            // set the clusters count
            if (_taskCluster)
            {
                if ((_targetField.Length == 0)
                    || (_goal != AnalystGoal.Classification))
                {
                    _script.Properties.SetProperty(
                        ScriptProperties.ClusterConfigClusters, 2);
                }
                else
                {
                    DataField tf = _script
                        .FindDataField(_targetField);
                    _script.Properties.SetProperty(
                        ScriptProperties.ClusterConfigClusters,
                        tf.ClassMembers.Count);
                }
            }
        }

        /// <summary>
        /// Expand the time-series fields.
        /// </summary>
        ///
        private void ExpandTimeSlices()
        {
            IList<AnalystField> oldList = _script.Normalize.NormalizedFields;
            IList<AnalystField> newList = new List<AnalystField>();


            // generate the inputs foreach the new list
            foreach (AnalystField field  in  oldList)
            {
                if (!field.Ignored)
                {
                    if (_includeTargetField || field.Input)
                    {
                        for (int i = 0; i < _lagWindowSize; i++)
                        {
                            var newField = new AnalystField(field) {TimeSlice = -i, Output = false};
                            newList.Add(newField);
                        }
                    }
                }
                else
                {
                    newList.Add(field);
                }
            }


            // generate the outputs foreach the new list
            foreach (AnalystField field  in  oldList)
            {
                if (!field.Ignored)
                {
                    if (field.Output)
                    {
                        for (int i = 1; i <= _leadWindowSize; i++)
                        {
                            var newField = new AnalystField(field) {TimeSlice = i};
                            newList.Add(newField);
                        }
                    }
                }
            }


            // generate the ignores foreach the new list
            foreach (AnalystField field  in  oldList)
            {
                if (field.Ignored)
                {
                    newList.Add(field);
                }
            }

            // swap back in
            oldList.Clear();
            foreach (AnalystField item in oldList)
            {
                oldList.Add(item);
            }
        }

        /// <summary>
        /// Generate a feed forward machine learning method.
        /// </summary>
        ///
        /// <param name="inputColumns">The input column count.</param>
        private void GenerateFeedForward(int inputColumns)
        {
            var hidden = (int) ((inputColumns)*1.5d);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TYPE_FEEDFORWARD);

            if (_range == NormalizeRange.NegOne2One)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.MlConfigArchitecture,
                    "?:B->TANH->" + hidden + ":B->TANH->?");
            }
            else
            {
                _script.Properties.SetProperty(
                    ScriptProperties.MlConfigArchitecture,
                    "?:B->SIGMOID->" + hidden + ":B->SIGMOID->?");
            }

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                          "rprop");
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, DefaultTrainError);
        }

        /// <summary>
        /// Generate filenames.
        /// </summary>
        ///
        /// <param name="rawFile">The raw filename.</param>
        private void GenerateFilenames(FileInfo rawFile)
        {
            _filenameRaw = rawFile.Name;
            _filenameNorm = FileUtil.AddFilenameBase(rawFile, "_norm").Name;
            _filenameRandom = FileUtil.AddFilenameBase(rawFile, "_random").Name;
            _filenameTrain = FileUtil.AddFilenameBase(rawFile, "_train").Name;
            _filenameEval = FileUtil.AddFilenameBase(rawFile, "_eval").Name;
            _filenameEvalNorm = FileUtil.AddFilenameBase(rawFile, "_eval_norm").Name;
            _filenameTrainSet = FileUtil.ForceExtension(_filenameTrain,
                                                       "egb");
            _filenameMl = FileUtil.ForceExtension(_filenameTrain, "eg");
            _filenameOutput = FileUtil.AddFilenameBase(rawFile, "_output").Name;
            _filenameBalance = FileUtil.AddFilenameBase(rawFile, "_balance").Name;
            _filenameCluster = FileUtil.AddFilenameBase(rawFile, "_cluster").Name;

            ScriptProperties p = _script.Properties;

            p.SetFilename(FileRaw, _filenameRaw);
            if (_taskNormalize)
            {
                p.SetFilename(FileNormalize, _filenameNorm);
            }

            if (_taskRandomize)
            {
                p.SetFilename(FileRandom, _filenameRandom);
            }

            if (_taskCluster)
            {
                p.SetFilename(FileCluster, _filenameCluster);
            }

            if (_taskSegregate)
            {
                p.SetFilename(FileTrain, _filenameTrain);
                p.SetFilename(FileEval, _filenameEval);
                p.SetFilename(FileEvalNorm, _filenameEvalNorm);
            }

            if (_taskBalance)
            {
                p.SetFilename(FileBalance, _filenameBalance);
            }

            p.SetFilename(FileTrainset, _filenameTrainSet);
            p.SetFilename(FileMl, _filenameMl);
            p.SetFilename(FileOutput, _filenameOutput);
        }

        /// <summary>
        /// Generate the generate task.
        /// </summary>
        ///
        private void GenerateGenerate()
        {
            DetermineTargetField();

            if (_targetField == null)
            {
                throw new AnalystError(
                    "Failed to find normalized version of target field: "
                    + _targetField);
            }

            int inputColumns = _script.Normalize
                .CalculateInputColumns();
            int idealColumns = _script.Normalize
                .CalculateOutputColumns();

            switch (_methodType)
            {
                case WizardMethodType.FeedForward:
                    GenerateFeedForward(inputColumns);
                    break;
                case WizardMethodType.SVM:
                    GenerateSVM();
                    break;
                case WizardMethodType.RBF:
                    GenerateRBF(inputColumns, idealColumns);
                    break;
                case WizardMethodType.SOM:
                    GenerateSOM();
                    break;
                default:
                    throw new AnalystError("Unknown method type");
            }
        }

        /// <summary>
        /// Generate the normalized fields.
        /// </summary>
        ///
        private void GenerateNormalizedFields()
        {
            IList<AnalystField> norm = _script.Normalize.NormalizedFields;
            norm.Clear();
            DataField[] dataFields = _script.Fields;

            for (int i = 0; i < _script.Fields.Length; i++)
            {
                DataField f = dataFields[i];

                NormalizationAction action;
                bool isLast = i == _script.Fields.Length - 1;

                if ((f.Integer || f.Real) && !f.Class)
                {
                    action = NormalizationAction.Normalize;
                    AnalystField af = _range == NormalizeRange.NegOne2One ? new AnalystField(f.Name, action, 1, -1) : new AnalystField(f.Name, action, 1, 0);
                    norm.Add(af);
                    af.ActualHigh = f.Max;
                    af.ActualLow = f.Min;
                }
                else if (f.Class)
                {
                    if (isLast && _directClassification)
                    {
                        action = NormalizationAction.SingleField;
                    }
                    else if (f.ClassMembers.Count > 2)
                    {
                        action = NormalizationAction.Equilateral;
                    }
                    else
                    {
                        action = NormalizationAction.OneOf;
                    }

                    norm.Add(_range == NormalizeRange.NegOne2One
                                 ? new AnalystField(f.Name, action, 1, -1)
                                 : new AnalystField(f.Name, action, 1, 0));
                }
                else
                {
                    action = NormalizationAction.Ignore;
                    norm.Add(new AnalystField(action, f.Name));
                }
            }

            _script.Normalize.Init(_script);
        }

        /// <summary>
        /// Generate a RBF machine learning method.
        /// </summary>
        ///
        /// <param name="inputColumns">The number of input columns.</param>
        /// <param name="outputColumns">The number of output columns.</param>
        private void GenerateRBF(int inputColumns, int outputColumns)
        {
            var hidden = (int) ((inputColumns)*1.5d);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TYPE_RBFNETWORK);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture,
                "?->GAUSSIAN(c=" + hidden + ")->?");

            _script.Properties.SetProperty(
                ScriptProperties.MlTrainType, outputColumns > 1 ? "rprop" : "svd");

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                          DefaultTrainError);
        }

        /// <summary>
        /// Generate the segregate task.
        /// </summary>
        ///
        private void GenerateSegregate()
        {
            if (_taskSegregate)
            {
                var array = new AnalystSegregateTarget[2];
                array[0] = new AnalystSegregateTarget(FileTrain,
                                                      DefaultTrainPercent);
                array[1] = new AnalystSegregateTarget(FileEval,
                                                      DefaultEvalPercent);
                _script.Segregate.SegregateTargets = array;
            }
            else
            {
                var array = new AnalystSegregateTarget[0];
                _script.Segregate.SegregateTargets = array;
            }
        }

        /// <summary>
        /// Generate the settings.
        /// </summary>
        ///
        private void GenerateSettings()
        {
            // starting point
            string target = FileRaw;
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile, target);

            // randomize
            if (!_timeSeries && _taskRandomize)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.RandomizeConfigSourceFile,
                    FileRaw);
                target = FileRandom;
                _script.Properties.SetProperty(
                    ScriptProperties.RandomizeConfigTargetFile, target);
            }

            // balance
            if (!_timeSeries && _taskBalance)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.BalanceConfigSourceFile, target);
                target = FileBalance;
                _script.Properties.SetProperty(
                    ScriptProperties.BalanceConfigTargetFile, target);
            }

            // segregate
            if (_taskSegregate)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.SegregateConfigSourceFile, target);
                target = FileTrain;
            }

            // normalize
            if (_taskNormalize)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.NormalizeConfigSourceFile, target);
                target = FileNormalize;
                _script.Properties.SetProperty(
                    ScriptProperties.NormalizeConfigTargetFile, target);
            }

            string evalSource = _taskSegregate ? FileEval : target;

            // cluster
            if (_taskCluster)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.ClusterConfigSourceFile, evalSource);
                _script.Properties.SetProperty(
                    ScriptProperties.ClusterConfigTargetFile,
                    FileCluster);
                _script.Properties.SetProperty(
                    ScriptProperties.ClusterConfigType, "kmeans");
            }

            // generate
            _script.Properties.SetProperty(
                ScriptProperties.GenerateConfigSourceFile, target);
            _script.Properties.SetProperty(
                ScriptProperties.GenerateConfigTargetFile,
                FileTrainset);

            // ML
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigTrainingFile,
                FileTrainset);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigMachineLearningFile,
                FileMl);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigOutputFile,
                FileOutput);

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigEvalFile, evalSource);

            // other
            _script.Properties.SetProperty(
                ScriptProperties.SetupConfigCSVFormat,
                AnalystFileFormat.DecpntComma);
        }

        /// <summary>
        /// Generate a SOM machine learning method.
        /// </summary>
        private void GenerateSOM()
        {
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TYPE_SOM);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, "?->?");

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                          MLTrainFactory.TYPE_SOM_NEIGHBORHOOD);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainArguments,
                "ITERATIONS=1000,NEIGHBORHOOD=rbf1d,RBF_TYPE=gaussian");

            // ScriptProperties.ML_TRAIN_arguments
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, DefaultTrainError);
        }

        /// <summary>
        /// Generate a SVM machine learning method.
        /// </summary>
        private void GenerateSVM()
        {
            var arch = new StringBuilder();
            arch.Append("?->");
            arch.Append(_goal == AnalystGoal.Classification ? "C" : "R");
            arch.Append("(type=new,kernel=rbf)->?");

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TYPE_SVM);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, arch.ToString());

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                          MLTrainFactory.TYPE_SVM_SEARCH);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, DefaultTrainError);
        }

        /// <summary>
        /// Generate the tasks.
        /// </summary>
        ///
        private void GenerateTasks()
        {
            var task1 = new AnalystTask(EncogAnalyst.TaskFull);
            if (!_timeSeries && _taskRandomize)
            {
                task1.Lines.Add("randomize");
            }

            if (!_timeSeries && _taskBalance)
            {
                task1.Lines.Add("balance");
            }

            if (_taskSegregate)
            {
                task1.Lines.Add("segregate");
            }

            if (_taskNormalize)
            {
                task1.Lines.Add("normalize");
            }

            task1.Lines.Add("generate");
            task1.Lines.Add("create");
            task1.Lines.Add("train");
            task1.Lines.Add("evaluate");

            var task2 = new AnalystTask("task-generate");
            if (!_timeSeries && _taskRandomize)
            {
                task2.Lines.Add("randomize");
            }

            if (_taskSegregate)
            {
                task2.Lines.Add("segregate");
            }
            if (_taskNormalize)
            {
                task2.Lines.Add("normalize");
            }
            task2.Lines.Add("generate");

            var task3 = new AnalystTask("task-evaluate-raw");
            task3.Lines.Add(CreateSet(ScriptProperties.MlConfigEvalFile,
                                      FileEvalNorm));
            task3.Lines.Add(CreateSet(ScriptProperties.NormalizeConfigSourceFile,
                                      FileEval));
            task3.Lines.Add(CreateSet(ScriptProperties.NormalizeConfigTargetFile,
                                      FileEvalNorm));
            task3.Lines.Add("normalize");
            task3.Lines.Add("evaluate-raw");

            var task4 = new AnalystTask("task-create");
            task4.Lines.Add("create");

            var task5 = new AnalystTask("task-train");
            task5.Lines.Add("train");

            var task6 = new AnalystTask("task-evaluate");
            task6.Lines.Add("evaluate");

            var task7 = new AnalystTask("task-cluster");
            task7.Lines.Add("cluster");

            _script.AddTask(task1);
            _script.AddTask(task2);
            _script.AddTask(task3);
            _script.AddTask(task4);
            _script.AddTask(task5);
            _script.AddTask(task6);
            _script.AddTask(task7);
        }


        /// <summary>
        /// Reanalyze column ranges.
        /// </summary>
        ///
        public void Reanalyze()
        {
            String rawID = _script.Properties.GetPropertyFile(
                ScriptProperties.HeaderDatasourceRawFile);

            FileInfo rawFilename = _analyst.Script
                .ResolveFilename(rawID);

            _analyst.Analyze(
                rawFilename,
                _script.Properties.GetPropertyBoolean(
                    ScriptProperties.SetupConfigInputHeaders),
                _script.Properties.GetPropertyFormat(
                    ScriptProperties.SetupConfigCSVFormat));
        }

        /// <summary>
        /// Analyze a file.
        /// </summary>
        ///
        /// <param name="analyzeFile">The file to analyze.</param>
        /// <param name="b">True if there are headers.</param>
        /// <param name="format">The file format.</param>
        public void Wizard(FileInfo analyzeFile, bool b,
                           AnalystFileFormat format)
        {
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceFormat, format);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceHeaders, b);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile, analyzeFile);

            _timeSeries = ((_lagWindowSize > 0) || (_leadWindowSize > 0));

            DetermineClassification();
            GenerateFilenames(analyzeFile);
            GenerateSettings();
            _analyst.Analyze(analyzeFile, b, format);
            GenerateNormalizedFields();
            GenerateSegregate();

            GenerateGenerate();

            GenerateTasks();
            if (_timeSeries && (_lagWindowSize > 0)
                && (_leadWindowSize > 0))
            {
                ExpandTimeSlices();
            }
        }

        /// <summary>
        /// Analyze a file at the specified URL.
        /// </summary>
        ///
        /// <param name="url">The URL to analyze.</param>
        /// <param name="saveFile">The save file.</param>
        /// <param name="analyzeFile">The Encog analyst file.</param>
        /// <param name="b">True if there are headers.</param>
        /// <param name="format">The file format.</param>
        public void Wizard(Uri url, FileInfo saveFile,
                           FileInfo analyzeFile, bool b,
                           AnalystFileFormat format)
        {
            _script.BasePath = saveFile.DirectoryName;

            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceFile, url);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceFormat, format);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceHeaders, b);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile, analyzeFile);

            GenerateFilenames(analyzeFile);
            GenerateSettings();
            _analyst.Download();

            Wizard(analyzeFile, b, format);
        }
    }
}