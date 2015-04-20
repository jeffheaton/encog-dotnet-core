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
using System.IO;
using System.Text;
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.ML;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Process;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Script.Task;
using Encog.App.Generate;
using Encog.ML.Factory;
using Encog.ML.Prg;
using Encog.ML.Prg.Ext;
using Encog.Neural.NEAT;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Analyst.Wizard
{
    /// <summary>
    ///     The Encog Analyst Wizard can be used to create Encog Analyst script files
    ///     from a CSV file. This class is typically used by the Encog Workbench, but it
    ///     can easily be used from any program to create a starting point for an Encog
    ///     Analyst Script.
    ///     Several items must be provided to the wizard.
    ///     Desired Machine Learning Method: This is the machine learning method that you
    ///     would like the wizard to use. This might be a neural network, SVM or other
    ///     supported method.
    ///     Normalization Range: This is the range that the data should be normalized
    ///     into. Some machine learning methods perform better with different ranges. The
    ///     two ranges supported by the wizard are -1 to 1 and 0 to 1.
    ///     Goal: What are we trying to accomplish. Is this a classification, regression
    ///     or autoassociation problem.
    /// </summary>
    public class AnalystWizard
    {
        /// <summary>
        ///     The default training percent.
        /// </summary>
        public const int DefaultTrainPercent = 75;

        /// <summary>
        ///     The default evaluation percent.
        /// </summary>
        public const int DefaultEvalPercent = 25;

        /// <summary>
        ///     The default training error.
        /// </summary>
        public const double DefaultTrainError = 0.05d;

        /// <summary>
        ///     The raw file.
        /// </summary>
        public const String FileRaw = "FILE_RAW";

        /// <summary>
        ///     The normalized file.
        /// </summary>
        public const String FileNormalize = "FILE_NORMALIZE";

        /// <summary>
        ///     The randomized file.
        /// </summary>
        public const String FileRandom = "FILE_RANDOMIZE";

        /// <summary>
        ///     The training file.
        /// </summary>
        public const String FileTrain = "FILE_TRAIN";

        /// <summary>
        ///     The evaluation file.
        /// </summary>
        public const String FileEval = "FILE_EVAL";

        /// <summary>
        ///     The eval file normalization file.
        /// </summary>
        public const String FileEvalNorm = "FILE_EVAL_NORM";

        /// <summary>
        ///     The training set.
        /// </summary>
        public const String FileTrainset = "FILE_TRAINSET";

        /// <summary>
        ///     The machine learning file.
        /// </summary>
        public const String FileMl = "FILE_ML";

        /// <summary>
        ///     The output file.
        /// </summary>
        public const String FileOutput = "FILE_OUTPUT";

        /// <summary>
        ///     The balanced file.
        /// </summary>
        public const String FileBalance = "FILE_BALANCE";

        /// <summary>
        ///     The clustered file.
        /// </summary>
        public const String FileCluster = "FILE_CLUSTER";

        /// <summary>
        ///     The code file.
        /// </summary>
        public const String FileCode = "FILE_CODE";

        /// <summary>
        ///     The preprocess file.
        /// </summary>
        public const String FilePre = "FILE_PROCESSED";

        /// <summary>
        ///     The analyst.
        /// </summary>
        private readonly EncogAnalyst _analyst;

        /// <summary>
        ///     The analyst script.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     Should code data be embedded.
        /// </summary>
        private bool _codeEmbedData;

        /// <summary>
        ///     The target language for code generation.
        /// </summary>
        private TargetLanguage _codeTargetLanguage = TargetLanguage.NoGeneration;

        /// <summary>
        ///     Are we using single-field(direct) classification.
        /// </summary>
        private bool _directClassification;

        /// <summary>
        ///     The balance filename.
        /// </summary>
        private String _filenameBalance;

        /// <summary>
        ///     The cluster filename.
        /// </summary>
        private String _filenameCluster;

        /// <summary>
        ///     The code filename.
        /// </summary>
        private string _filenameCode;

        /// <summary>
        ///     The evaluation filename.
        /// </summary>
        private String _filenameEval;

        /// <summary>
        ///     The normalization eval file name.
        /// </summary>
        private String _filenameEvalNorm;

        /// <summary>
        ///     The machine learning file name.
        /// </summary>
        private String _filenameMl;

        /// <summary>
        ///     The normalized filename.
        /// </summary>
        private String _filenameNorm;

        /// <summary>
        ///     The output filename.
        /// </summary>
        private String _filenameOutput;

        /// <summary>
        ///     The process filename.
        /// </summary>
        private string _filenameProcess;

        /// <summary>
        ///     The random file name.
        /// </summary>
        private String _filenameRandom;

        /// <summary>
        ///     The raw filename.
        /// </summary>
        private String _filenameRaw;

        /// <summary>
        ///     The training filename.
        /// </summary>
        private String _filenameTrain;

        /// <summary>
        ///     The training set filename.
        /// </summary>
        private String _filenameTrainSet;

        /// <summary>
        ///     The format in use.
        /// </summary>
        private AnalystFileFormat _format;

        /// <summary>
        ///     The analyst goal.
        /// </summary>
        private AnalystGoal _goal;

        /// <summary>
        ///     Should the target field be included int he input, if we are doing
        ///     time-series.
        /// </summary>
        private bool _includeTargetField;

        /// <summary>
        ///     The size of the lag window, if we are doing time-series.
        /// </summary>
        private int _lagWindowSize;

        /// <summary>
        ///     The size of the lead window, if we are doing time-series.
        /// </summary>
        private int _leadWindowSize;

        /// <summary>
        ///     The machine learning method that we will be using.
        /// </summary>
        private WizardMethodType _methodType;

        /// <summary>
        ///     How to handle missing values.
        /// </summary>
        private IHandleMissingValues _missing;

        /// <summary>
        ///     Should we preprocess.
        /// </summary>
        private bool _preprocess;

        /// <summary>
        ///     The normalization range.
        /// </summary>
        private NormalizeRange _range;

        /// <summary>
        ///     The target field, or null to detect.
        /// </summary>
        private AnalystField _targetField;

        /// <summary>
        ///     True if the balance command should be generated.
        /// </summary>
        private bool _taskBalance;

        /// <summary>
        ///     True if the cluster command should be generated.
        /// </summary>
        private bool _taskCluster;

        /// <summary>
        ///     True if the normalize command should be generated.
        /// </summary>
        private bool _taskNormalize;

        /// <summary>
        ///     True if the randomize command should be generated.
        /// </summary>
        private bool _taskRandomize;

        /// <summary>
        ///     True if the segregate command should be generated.
        /// </summary>
        private bool _taskSegregate;

        /// <summary>
        ///     True if we are doing time-series.
        /// </summary>
        private bool _timeSeries;

        /// <summary>
        ///     Construct the analyst wizard.
        /// </summary>
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
            TargetFieldName = "";
            _goal = AnalystGoal.Classification;
            _leadWindowSize = 0;
            _lagWindowSize = 0;
            _includeTargetField = false;
            _missing = new DiscardMissing();
            MaxError = DefaultTrainError;
            NaiveBayes = false;
        }

        /// <summary>
        ///     The number of evidence segments to use when mapping continuous values to Bayes.
        /// </summary>
        public int EvidenceSegements { get; set; }

        public bool NaiveBayes { get; set; }

        /// <summary>
        ///     The maximum allowed training error.
        /// </summary>
        public double MaxError { get; set; }

        /// <summary>
        ///     The String name of the target field.
        /// </summary>
        public String TargetFieldName { get; set; }

        /// <summary>
        ///     Set the goal.
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
        ///     Set the target field.
        /// </summary>
        /// <value>The target field.</value>
        public AnalystField TargetField
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
        ///     How should missing values be handled.
        /// </summary>
        public IHandleMissingValues Missing
        {
            get { return _missing; }
            set { _missing = value; }
        }

        /// <summary>
        ///     The target language for code generation.
        /// </summary>
        public TargetLanguage CodeTargetLanguage
        {
            get { return _codeTargetLanguage; }
            set { _codeTargetLanguage = value; }
        }

        /// <summary>
        ///     Should code data be embedded.
        /// </summary>
        public bool CodeEmbedData
        {
            get { return _codeEmbedData; }
            set { _codeEmbedData = value; }
        }

        /// <summary>
        ///     Should we preprocess.
        /// </summary>
        public bool Preprocess
        {
            get { return _preprocess; }
            set { _preprocess = value; }
        }

        /// <summary>
        ///     Create a "set" command to add to a task.
        /// </summary>
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
        ///     Determine the type of classification used.
        /// </summary>
        private void DetermineClassification()
        {
            _directClassification = false;

            if ((_methodType == WizardMethodType.SVM)
                || (_methodType == WizardMethodType.SOM)
                || (_methodType == WizardMethodType.PNN))
            {
                _directClassification = true;
            }
        }

        /// <summary>
        ///     Determine the target field.
        /// </summary>
        private void DetermineTargetField()
        {
            IList<AnalystField> fields = _script.Normalize.NormalizedFields;

            if (string.IsNullOrEmpty(TargetFieldName))
            {
                bool success = false;

                if (Goal == AnalystGoal.Classification)
                {
                    // first try to the last classify field
                    foreach (AnalystField field in fields)
                    {
                        DataField df = _script.FindDataField(field.Name);
                        if (field.Action.IsClassify() && df.Class)
                        {
                            TargetField = field;
                            success = true;
                        }
                    }
                }
                else
                {
                    // otherwise, just return the last regression field
                    foreach (AnalystField field in fields)
                    {
                        DataField df = _script.FindDataField(field.Name);
                        if (!df.Class && (df.Real || df.Integer))
                        {
                            TargetField = field;
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
                TargetField = _script.FindAnalystField(TargetFieldName);
                if (TargetField == null)
                {
                    throw new AnalystError("Invalid target field: "
                                           + TargetFieldName);
                }
            }

            _script.Properties.SetProperty(
                ScriptProperties.DataConfigGoal, _goal);

            if (!_timeSeries && _taskBalance)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.BalanceConfigBalanceField,
                    TargetField.Name);
                DataField field = _analyst.Script.FindDataField(
                    _targetField.Name);
                if ((field != null) && field.Class)
                {
                    int countPer = field.MinClassCount;
                    _script.Properties.SetProperty(
                        ScriptProperties.BalanceConfigCountPer, countPer);
                }
            }

            // determine output field
            if (_methodType != WizardMethodType.BayesianNetwork)
            {
                // now that the target field has been determined, set the analyst fields
                AnalystField af = null;
                foreach (AnalystField field in _analyst.Script.Normalize.NormalizedFields)
                {
                    if ((field.Action != NormalizationAction.Ignore)
                        && field == _targetField)
                    {
                        if ((af == null)
                            || (af.TimeSlice < field.TimeSlice))
                        {
                            af = field;
                        }
                    }
                }

                if (af != null)
                {
                    af.Output = (true);
                }
            }

            // set the clusters count
            if (_taskCluster)
            {
                if ((TargetField == null)
                    || (_goal != AnalystGoal.Classification))
                {
                    _script.Properties.SetProperty(
                        ScriptProperties.ClusterConfigClusters, 2);
                }
                else
                {
                    DataField tf = _script.FindDataField(TargetField.Name);
                    _script.Properties.SetProperty(
                        ScriptProperties.ClusterConfigClusters,
                        tf.ClassMembers.Count);
                }
            }
        }

        /// <summary>
        ///     Expand the time-series fields.
        /// </summary>
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
            foreach (AnalystField item in newList)
            {
                oldList.Add(item);
            }
        }

        /// <summary>
        ///     Generate a feed forward machine learning method.
        /// </summary>
        /// <param name="inputColumns">The input column count.</param>
        private void GenerateFeedForward(int inputColumns)
        {
            var hidden = (int) ((inputColumns)*1.5d);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TypeFeedforward);

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
                ScriptProperties.MlTrainTargetError, MaxError);
        }

        /// <summary>
        ///     Generate a Bayesian network machine learning method.
        /// </summary>
        /// <param name="inputColumns">The input column count.</param>
        /// <param name="outputColumns">The output column count.</param>
        private void GenerateBayesian(int inputColumns,
                                      int outputColumns)
        {
            int segment = EvidenceSegements;

            if (!_targetField.Classify)
            {
                throw new AnalystError("Bayesian networks cannot be used for regression.");
            }

            var a = new StringBuilder();
            foreach (DataField field in _analyst.Script.Fields)
            {
                a.Append("P(");
                a.Append(field.Name);

                // handle actual class members
                if (field.ClassMembers.Count > 0)
                {
                    a.Append("[");
                    bool first = true;
                    foreach (AnalystClassItem item in field.ClassMembers)
                    {
                        if (!first)
                        {
                            a.Append(",");
                        }
                        a.Append(item.Code);
                        first = false;
                    }

                    // append a "fake" member, if there is only one
                    if (field.ClassMembers.Count == 1)
                    {
                        a.Append(",Other0");
                    }

                    a.Append("]");
                }
                else
                {
                    a.Append("[");
                    // handle ranges
                    double size = Math.Abs(field.Max - field.Min);
                    double per = size/segment;

                    if (size < EncogFramework.DefaultDoubleEqual)
                    {
                        double low = field.Min - 0.0001;
                        double hi = field.Min + 0.0001;
                        a.Append("BELOW: " + (low - 100) + " to " + hi + ",");
                        a.Append("Type0: " + low + " to " + hi + ",");
                        a.Append("ABOVE: " + hi + " to " + (hi + 100));
                    }
                    else
                    {
                        bool first = true;
                        for (int i = 0; i < segment; i++)
                        {
                            if (!first)
                            {
                                a.Append(",");
                            }
                            double low = field.Min + (per*i);
                            double hi = i == (segment - 1)
                                            ? (field.Max)
                                            : (low + per);
                            a.Append("Type");
                            a.Append(i);
                            a.Append(":");
                            a.Append(CSVFormat.EgFormat.Format(low, 16));
                            a.Append(" to ");
                            a.Append(CSVFormat.EgFormat.Format(hi, 16));
                            first = false;
                        }
                    }
                    a.Append("]");
                }

                a.Append(") ");
            }

            var q = new StringBuilder();
            q.Append("P(");
            q.Append(_targetField.Name);
            q.Append("|");
            bool first2 = true;
            foreach (DataField field in _analyst.Script.Fields)
            {
                if (!field.Name.Equals(_targetField.Name))
                {
                    if (!first2)
                    {
                        q.Append(",");
                    }
                    q.Append(field.Name);
                    first2 = false;
                }
            }
            q.Append(")");

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TypeBayesian);

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, a.ToString());

            _script.Properties.SetProperty(
                ScriptProperties.MLConfigQuery, q.ToString());

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           "bayesian");

            if (NaiveBayes)
            {
                _script.Properties.SetProperty(
                    ScriptProperties.MlTrainArguments,
                    "maxParents=1,estimator=simple,search=none,init=naive");
            }
            else
            {
                _script.Properties.SetProperty(
                    ScriptProperties.MlTrainArguments,
                    "maxParents=1,estimator=simple,search=k2,init=naive");
            }

            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);
        }

        /// <summary>
        ///     Generate a NEAT population method.
        /// </summary>
        /// <param name="inputColumns">The input column count.</param>
        /// <param name="outputColumns">The output column count.</param>
        private void GenerateNEAT(int inputColumns,
                                  int outputColumns)
        {
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TypeNEAT);

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture,
                "cycles=" + NEATPopulation.DefaultCycles);

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           MLTrainFactory.TypeNEATGA);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);
        }

        /// <summary>
        ///     Generate a EPL population method.
        /// </summary>
        /// <param name="inputColumns">The input column count.</param>
        /// <param name="outputColumns">The output column count.</param>
        private void generateEPL(int inputColumns,
                                 int outputColumns)
        {
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TypeEPL);
            String vars = "";

            if (inputColumns > 26)
            {
                throw new EncogError("More than 26 input variables is not supported for EPL.");
            }
            else if (inputColumns <= 3)
            {
                var temp = new StringBuilder();
                for (int i = 0; i < inputColumns; i++)
                {
                    if (temp.Length > 0)
                    {
                        temp.Append(',');
                    }
                    temp.Append((char) ('x' + i));
                }
                vars = temp.ToString();
            }
            else
            {
                var temp = new StringBuilder();
                for (int i = 0; i < inputColumns; i++)
                {
                    if (temp.Length > 0)
                    {
                        temp.Append(',');
                    }
                    temp.Append((char) ('a' + i));
                }
                vars = temp.ToString();
            }

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture,
                "cycles=" + NEATPopulation.DefaultCycles + ",vars=\"" + vars + "\"");

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           MLTrainFactory.TypeEPLGA);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);

            // add in the opcodes
            var context = new EncogProgramContext();

            if (Goal == AnalystGoal.Regression)
            {
                StandardExtensions.CreateNumericOperators(context);
            }
            else
            {
                StandardExtensions.CreateNumericOperators(context);
                StandardExtensions.CreateBooleanOperators(context);
            }
            foreach (IProgramExtensionTemplate temp in context.Functions.OpCodes)
            {
                _script.Opcodes.Add(new ScriptOpcode(temp));
            }
        }

        /// <summary>
        ///     Generate a PNN machine learning method.
        /// </summary>
        /// <param name="inputColumns">The number of input columns.</param>
        /// <param name="outputColumns">The number of ideal columns.</param>
        private void GeneratePNN(int inputColumns, int outputColumns)
        {
            var arch = new StringBuilder();
            arch.Append("?->");
            if (_goal == AnalystGoal.Classification)
            {
                arch.Append("C");
            }
            else
            {
                arch.Append("R");
            }
            arch.Append("(kernel=gaussian)->");
            arch.Append(_targetField.Classes.Count);

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TypePNN);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, arch.ToString());

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           MLTrainFactory.TypePNN);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);
        }


        /// <summary>
        ///     Generate filenames.
        /// </summary>
        /// <param name="rawFile">The raw filename.</param>
        private void GenerateFilenames(FileInfo rawFile)
        {
            if (_preprocess)
            {
                _filenameProcess = FileUtil.AddFilenameBase(rawFile, "_process").Name;
            }

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

            _filenameCode = FileUtil.ForceExtension(
                FileUtil.AddFilenameBase(rawFile, "_code").Name,
                EncogCodeGeneration.GetExtension(_codeTargetLanguage));

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

            if (_codeTargetLanguage != TargetLanguage.NoGeneration)
            {
                p.SetFilename(FileCode, _filenameCode);
            }

            p.SetFilename(FileTrainset, _filenameTrainSet);
            p.SetFilename(FileMl, _filenameMl);
            p.SetFilename(FileOutput, _filenameOutput);
        }

        /// <summary>
        ///     Generate the generate task.
        /// </summary>
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
                case WizardMethodType.PNN:
                    GeneratePNN(inputColumns, idealColumns);
                    break;
                case WizardMethodType.BayesianNetwork:
                    GenerateBayesian(inputColumns, idealColumns);
                    break;
                case WizardMethodType.NEAT:
                    GenerateNEAT(inputColumns, idealColumns);
                    break;
                case WizardMethodType.EPL:
                    generateEPL(inputColumns, idealColumns);
                    break;
                default:
                    throw new AnalystError("Unknown method type");
            }
        }

        /// <summary>
        ///     Generate the normalized fields.
        /// </summary>
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


                if (_methodType == WizardMethodType.BayesianNetwork)
                {
                    AnalystField af;
                    if (f.Class)
                    {
                        af = new AnalystField(f.Name,
                                              NormalizationAction.SingleField, 0, 0);
                    }
                    else
                    {
                        af = new AnalystField(f.Name,
                                              NormalizationAction.PassThrough, 0, 0);
                    }
                    norm.Add(af);
                }
                else if ((f.Integer || f.Real) && !f.Class)
                {
                    action = NormalizationAction.Normalize;
                    AnalystField af = _range == NormalizeRange.NegOne2One
                                          ? new AnalystField(f.Name, action, 1, -1)
                                          : new AnalystField(f.Name, action, 1, 0);
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
        ///     Generate a RBF machine learning method.
        /// </summary>
        /// <param name="inputColumns">The number of input columns.</param>
        /// <param name="outputColumns">The number of output columns.</param>
        private void GenerateRBF(int inputColumns, int outputColumns)
        {
            var hidden = (int) ((inputColumns)*1.5d);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType,
                MLMethodFactory.TypeRbfnetwork);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture,
                "?->GAUSSIAN(c=" + hidden + ")->?");

            _script.Properties.SetProperty(
                ScriptProperties.MlTrainType, outputColumns > 1 ? "rprop" : "svd");

            _script.Properties.SetProperty(ScriptProperties.MlTrainTargetError,
                                           MaxError);
        }

        /// <summary>
        ///     Generate the segregate task.
        /// </summary>
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
        ///     Generate the settings.
        /// </summary>
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
                _script.Normalize.MissingValues = _missing;
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
                _format);
        }

        /// <summary>
        ///     Generate a SOM machine learning method.
        /// </summary>
        private void GenerateSOM()
        {
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TypeSOM);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, "?->" + _targetField.Classes.Count);

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           MLTrainFactory.TypeSOMNeighborhood);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainArguments,
                "ITERATIONS=1000,NEIGHBORHOOD=rbf1d,RBF_TYPE=gaussian");

            // ScriptProperties.ML_TRAIN_arguments
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);
        }

        /// <summary>
        ///     Generate a SVM machine learning method.
        /// </summary>
        private void GenerateSVM()
        {
            var arch = new StringBuilder();
            arch.Append("?->");
            arch.Append(_goal == AnalystGoal.Classification ? "C" : "R");
            arch.Append("(type=new,kernel=rbf)->?");

            _script.Properties.SetProperty(
                ScriptProperties.MlConfigType, MLMethodFactory.TypeSVM);
            _script.Properties.SetProperty(
                ScriptProperties.MlConfigArchitecture, arch.ToString());

            _script.Properties.SetProperty(ScriptProperties.MlTrainType,
                                           MLTrainFactory.TypeSVMSearch);
            _script.Properties.SetProperty(
                ScriptProperties.MlTrainTargetError, MaxError);
        }

        /// <summary>
        ///     Generate the tasks.
        /// </summary>
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

            if (_codeTargetLanguage != TargetLanguage.NoGeneration)
            {
                task1.Lines.Add("code");
            }


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

            var task8 = new AnalystTask("task-code");
            task8.Lines.Add("code");

            AnalystTask task9 = null;
            if (_preprocess)
            {
                task9 = new AnalystTask("task-preprocess");
                task9.Lines.Add("process");
            }

            _script.AddTask(task1);
            _script.AddTask(task2);
            _script.AddTask(task3);
            _script.AddTask(task4);
            _script.AddTask(task5);
            _script.AddTask(task6);
            _script.AddTask(task7);
            _script.AddTask(task8);

            if (task9 != null)
            {
                _script.AddTask(task9);
            }
        }


        /// <summary>
        ///     Reanalyze column ranges.
        /// </summary>
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
        ///     Analyze a file.
        /// </summary>
        /// <param name="analyzeFile">The file to analyze.</param>
        /// <param name="b">True if there are headers.</param>
        /// <param name="format">The file format.</param>
        public void Wizard(FileInfo analyzeFile, bool b,
                           AnalystFileFormat format)
        {
            _script.BasePath = analyzeFile.DirectoryName;
            _format = format;
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
        ///     Analyze a file at the specified URL.
        /// </summary>
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
            _format = format;

            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceFile, url);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceHeaders, b);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile, analyzeFile);

            GenerateFilenames(analyzeFile);
            GenerateSettings();
            _analyst.Download();

            Wizard(analyzeFile, b, format);
        }

        public void WizardRealTime(IList<SourceElement> sourceData, FileInfo csvFile,
                                   int backwardWindow, int forwardWindow, PredictionType prediction,
                                   String predictField)
        {
            Preprocess = true;
            _script.BasePath = csvFile.DirectoryName;
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceSourceHeaders, true);
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile, csvFile);
            _script.Properties.SetProperty(
                ScriptProperties.SetupConfigInputHeaders, true);

            LagWindowSize = backwardWindow;
            LeadWindowSize = 1;
            _timeSeries = true;
            _format = AnalystFileFormat.DecpntComma;
            MethodType = WizardMethodType.FeedForward;
            _includeTargetField = false;
            TargetFieldName = "prediction";
            Missing = new DiscardMissing();

            Goal = AnalystGoal.Regression;
            Range = NormalizeRange.NegOne2One;
            TaskNormalize = true;
            TaskRandomize = false;
            TaskSegregate = true;
            TaskBalance = false;
            TaskCluster = false;
            MaxError = 0.05;
            CodeEmbedData = true;

            DetermineClassification();
            GenerateFilenames(csvFile);
            GenerateSettings();
            GenerateSourceData(sourceData);
            GenerateNormalizedFields();

            // if there is a time field, then ignore it
            AnalystField timeField = _script.FindAnalystField("time");
            if (timeField != null)
            {
                timeField.Action = NormalizationAction.Ignore;
            }

            GenerateSegregate();
            GenerateGenerate();
            GenerateProcess(backwardWindow, forwardWindow, prediction, predictField);
            GenerateCode();

            // override raw_file to be the processed file
            _script.Properties.SetProperty(
                ScriptProperties.HeaderDatasourceRawFile,
                FilePre);

            GenerateTasks();
            if (_timeSeries && (_lagWindowSize > 0)
                && (_leadWindowSize > 0))
            {
                ExpandTimeSlices();
            }
        }

        private void GenerateSourceData(IList<SourceElement> sourceData)
        {
            var fields = new DataField[sourceData.Count + 1];
            int index = 0;

            foreach (SourceElement element in sourceData)
            {
                var df = new DataField(element.Name);
                df.Source = element.Source;
                df.Integer = false;
                df.Class = false;
                df.Max = 100000;
                df.Mean = 0;
                df.Min = -100000;
                df.StandardDeviation = 0;
                fields[index++] = df;
            }

            // now add the prediction
            var df2 = new DataField("prediction");
            df2.Source = "prediction";
            df2.Integer = false;
            df2.Class = false;
            df2.Max = 100000;
            df2.Min = -100000;
            df2.Mean = 0;
            df2.StandardDeviation = 0;
            fields[index++] = df2;

            _script.Fields = fields;
        }

        private void GenerateProcess(int backwardWindow, int forwardWindow, PredictionType prediction,
                                     String predictField)
        {
            _script.Properties.SetProperty(
                ScriptProperties.PROCESS_CONFIG_BACKWARD_SIZE, backwardWindow);
            _script.Properties.SetProperty(
                ScriptProperties.PROCESS_CONFIG_FORWARD_SIZE, forwardWindow);

            IList<ProcessField> fields = _script.Process.Fields;
            fields.Clear();
            foreach (DataField df in _script.Fields)
            {
                if (string.Compare(df.Name, "prediction", true) == 0)
                {
                    continue;
                }

                var command = new StringBuilder();

                if (string.Compare(df.Name, "time", true) == 0)
                {
                    command.Append("cint(field(\"");
                    command.Append(df.Name);
                    command.Append("\",0");
                    command.Append("))");
                    fields.Add(new ProcessField(df.Name, command.ToString()));
                }
                else
                {
                    command.Append("cfloat(field(\"");
                    command.Append(df.Name);
                    command.Append("\",0");
                    command.Append("))");
                    fields.Add(new ProcessField(df.Name, command.ToString()));
                }
            }

            var c = new StringBuilder();

            switch (prediction)
            {
                case PredictionType.fieldmax:
                    c.Append("fieldmax(\"");
                    c.Append(predictField);
                    c.Append("\",");
                    c.Append(-forwardWindow);
                    c.Append(",");
                    c.Append(-1);
                    c.Append(")");
                    break;
                case PredictionType.fieldmaxpip:
                    c.Append("fieldmaxpip(\"");
                    c.Append(predictField);
                    c.Append("\",");
                    c.Append(-forwardWindow);
                    c.Append(",");
                    c.Append(-1);
                    c.Append(")");
                    break;
            }

            fields.Add(new ProcessField("prediction", c.ToString()));
        }

        private void GenerateCode()
        {
            _script.Properties.SetProperty(
                ScriptProperties.CODE_CONFIG_EMBED_DATA, _codeEmbedData);
            _script.Properties.SetProperty(
                ScriptProperties.CODE_CONFIG_TARGET_LANGUAGE,
                _codeTargetLanguage);
            _script.Properties.SetProperty(
                ScriptProperties.CODE_CONFIG_TARGET_FILE,
                FileCode);
        }
    }
}
