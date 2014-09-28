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
using System.Linq;
using Encog.ML.Data;
using Encog.Neural.Data.Basic;
using Encog.Util.CSV;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Segregate;
using Encog.Util.Normalize.Target;

namespace Encog.Util.Normalize
{
    /// <summary>
    /// This class is used to normalize both input and ideal data for neural
    /// networks. This class can accept input from a variety of sources and output to
    /// a variety of targets. Normalization is a process by which input data is
    /// normalized so that it falls in specific ranges. Neural networks typically
    /// require input to be in the range of 0 to 1, or -1 to 1, depending on how the
    /// network is structured.
    /// 
    /// The normalize class is typically given for different types of objects to tell
    /// it how to process data.
    /// 
    /// Input Fields:
    /// 
    /// Input fields specify the raw data that will be read by the Normalize class.
    /// Input fields are added to the Normalize class by calling addInputField
    /// method. Input fields must implement the InputField interface. There are a
    /// number of different input fields provided. Input data can be read from
    /// several different sources. For example, you can read the "neural network
    /// input" data from one CSV file and the "ideal neural network output" from
    /// another.
    /// 
    /// 
    /// Output Fields:
    /// 
    /// The output fields are used to specify the final output from the Normalize
    /// class. The output fields specify both the "neural network input" and "ideal
    /// output". The output fields are flagged as either input our ideal. The output
    /// fields are not necessarily one-to-one with the input fields. For example,
    /// several input fields may combine to produce a single output field. Further
    /// some input fields may be used only to segregate data, whereas other input
    /// fields may be ignored all together. The type of output field that you specify
    /// determines the type of processing that will be done on that field. An
    /// OutputField is added by calling the addOutputField method.
    /// 
    /// 
    /// Segregators:
    /// 
    /// Segregators are used generally for two related purposes. First, segregators
    /// can be used to exclude rows of data based on certain input values. Perhaps
    /// the data includes several classes of data, and you only want to train on one
    /// class. Secondly, segregators can be used to segregate data into training and
    /// evaluation sets. You may choose to use 80% of your data for training and 20%
    /// for evaluation. A segregator is added by calling the addSegregator method.
    /// 
    /// 
    /// Target Storage:
    /// 
    /// The data created by the Normalization class must be stored somewhere. The
    /// storage targets allow this to be specified. The output can be sent to a CSV
    /// file, a NeuralDataSet, or any other target supported by a
    /// NormalizationStorage derived class. The target is specified by calling the
    /// setTarget method.
    /// 
    /// The normalization process can take some time.  The progress can be reported
    /// to a StatusReportable object.
    /// 
    /// The normalization is a two pass process.  The first pass counts the number
    /// of records and computes important statistics that will be used to 
    /// normalize the output.  The second pass actually performs the normalization
    /// and writes to the target.  Both passes are performed when the process
    /// method is called.
    /// </summary>
    [Serializable]
    public class DataNormalization
    {
        /// <summary>
        /// Hold a map between the InputFieldCSV objects and the corresponding
        /// ReadCSV object. There will likely be many fields read from a single file.
        /// This allows only one ReadCSV object to need to be created per actual CSV
        /// file.
        /// </summary>
        [NonSerialized]
        private IDictionary<IInputField, ReadCSV> _csvMap;


        /// <summary>
        /// Map each of the input fields to an internally-build NeuralDataFieldHolder object.
        /// The NeuralDataFieldHolder object holds an Iterator, InputField and last 
        /// NeuralDataPair object loaded.
        /// </summary>
        [NonSerialized]
        private IDictionary<IInputField, MLDataFieldHolder> _dataSetFieldMap;

        /// <summary>
        /// Map each of the NeuralDataSet Iterators to an internally-build NeuralDataFieldHolder 
        /// object. The NeuralDataFieldHolder object holds an Iterator, InputField and last 
        /// NeuralDataPair object loaded.
        /// </summary>
        [NonSerialized]
        private IDictionary<IEnumerator<IMLDataPair>, MLDataFieldHolder> _dataSetIteratorMap;

        /// <summary>
        /// Output fields can be grouped together, if the value of one output field might 
        /// affect all of the others.  This collection holds a list of all of the output 
        /// field groups.
        /// </summary>
        private readonly IList<IOutputFieldGroup> _groups = new List<IOutputFieldGroup>();

        /// <summary>
        /// The input fields.
        /// </summary>
        private readonly IList<IInputField> _inputFields = new List<IInputField>();

        /// <summary>
        /// The output fields.
        /// </summary>
        private readonly IList<IOutputField> _outputFields = new List<IOutputField>();

        /// <summary>
        /// Keep a collection of all of the ReadCSV classes to support all of the
        /// distinct CSV files that are to be read.
        /// </summary>
        [NonSerialized]
        private ICollection<ReadCSV> _readCSV;

        /// <summary>
        /// For each InputFieldNeuralDataSet input field an Iterator must be kept to
        /// actually access the data. Only one Iterator should be kept per data set
        /// actually used.
        /// </summary>
        [NonSerialized]
        private ICollection<IEnumerator<IMLDataPair>> _readDataSet;

        /// <summary>
        /// A list of the segregators.
        /// </summary>
        private readonly IList<ISegregator> _segregators = new List<ISegregator>();

        /// <summary>
        /// The format to use for all CSV files.
        /// </summary>
        private CSVFormat _csvFormat = CSVFormat.English;

        /// <summary>
        /// The current record's index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// How long has it been since the last report.  This filters so that
        /// every single record does not produce a message.
        /// </summary>
        private int _lastReport;

        /// <summary>
        /// The number of records that were found in the first pass.
        /// </summary>
        private int _recordCount;

        /// <summary>
        /// The object to report the progress of the normalization to.
        /// </summary>
        [NonSerialized]
        private IStatusReportable _report = new NullStatusReportable();

        /// <summary>
        /// Where the final output from the normalization is sent.
        /// </summary>
        private INormalizationStorage _storage;

        /// <summary>
        /// The CSV format being used.
        /// </summary>
        public CSVFormat CSVFormatUsed
        {
            get { return _csvFormat; }
            set { _csvFormat = value; }
        }


        /// <summary>
        /// The object groups.
        /// </summary>
        public IList<IOutputFieldGroup> Groups
        {
            get { return _groups; }
        }

        /// <summary>
        /// The input fields.
        /// </summary>
        public IList<IInputField> InputFields
        {
            get { return _inputFields; }
        }

        /// <summary>
        /// The output fields.
        /// </summary>
        public IList<IOutputField> OutputFields
        {
            get { return _outputFields; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return _recordCount; }
        }

        /// <summary>
        /// The class that progress will be reported to.
        /// </summary>
        public IStatusReportable Report
        {
            get { return _report; }
            set { _report = value; }
        }

        /// <summary>
        /// The segregators in use.
        /// </summary>
        public IList<ISegregator> Segregators
        {
            get { return _segregators; }
        }

        /// <summary>
        /// The place that the normalization output will be stored.
        /// </summary>
        public INormalizationStorage Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        /// <summary>
        /// Add an input field.
        /// </summary>
        /// <param name="f">The input field to add.</param>
        public void AddInputField(IInputField f)
        {
            _inputFields.Add(f);
        }

        /// <summary>
        ///  Add an output field.  This output field will be added as a 
        /// "neural network input field", not an "ideal output field".
        /// </summary>
        /// <param name="outputField">The output field to add.</param>
        public void AddOutputField(IOutputField outputField)
        {
            AddOutputField(outputField, false);
        }

        /// <summary>
        /// Add a field and allow it to be specified as an "ideal output field".
        /// An "ideal" field is the expected output that the neural network is
        /// training towards.
        /// </summary>
        /// <param name="outputField">The output field.</param>
        /// <param name="ideal">True if this is an ideal field.</param>
        public void AddOutputField(IOutputField outputField,
                                   bool ideal)
        {
            _outputFields.Add(outputField);
            outputField.Ideal = ideal;
            if (outputField is OutputFieldGrouped)
            {
                var ofg = (OutputFieldGrouped)outputField;
                _groups.Add(ofg.Group);
            }
        }

        /// <summary>
        ///  Add a segregator.
        /// </summary>
        /// <param name="segregator">The segregator to add.</param>
        public void AddSegregator(ISegregator segregator)
        {
            _segregators.Add(segregator);
            segregator.Init(this);
        }

        /// <summary>
        /// Called internally to allow each of the input fields to update their
        /// min/max values in the first pass.
        /// </summary>
        private void ApplyMinMax()
        {
            foreach (IInputField field in _inputFields)
            {
                double value = field.CurrentValue;
                field.ApplyMinMax(value);
            }
        }

        /// <summary>
        /// Build "input data for a neural network" based on the input values
        /// provided.  This allows  input for a neural network to be normalized.
        /// This is typically used when data is to be presented to a trained
        /// neural network.
        /// </summary>
        /// <param name="data">The input values to be normalized.</param>
        /// <returns>The data to be sent to the neural network.</returns>
        public IMLData BuildForNetworkInput(double[] data)
        {
            // feed the input fields
            int index = 0;
            foreach (IInputField field in _inputFields)
            {
                if (field.UsedForNetworkInput)
                {
                    if (index >= data.Length)
                    {
                        throw new NormalizationError(
                            "Can't build data, input fields used for neural input, must match provided data("
                            + data.Length + ").");
                    }
                    field.CurrentValue = data[index++];
                }
            }

            // count the output fields
            int outputCount = 0;
            foreach (IOutputField ofield in _outputFields)
            {
                if (!ofield.Ideal)
                {
                    for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                    {
                        outputCount++;
                    }
                }
            }

            // process the output fields

            InitForOutput();

            var result = new BasicNeuralData(outputCount);

            // write the value
            int outputIndex = 0;
            foreach (IOutputField ofield in _outputFields)
            {
                if (!ofield.Ideal)
                {
                    for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                    {
                        result[outputIndex++] = ofield.Calculate(sub);
                    }
                }
            }

            return result;
        }



        private void DetermineInputFieldValue(IInputField field, int index, bool headers)
        {
            double result;

            if (field is InputFieldCSV)
            {
                var fieldCSV = (InputFieldCSV)field;
                ReadCSV csv = _csvMap[field];
                result = csv.GetDouble(fieldCSV.ColumnName);

            }
            else if (field is InputFieldMLDataSet)
            {
                var mlField = (InputFieldMLDataSet)field;
                MLDataFieldHolder holder = _dataSetFieldMap
                    [field];
                IMLDataPair pair = holder.Pair;
                int offset = mlField.Offset;
                if (offset < pair.Input.Count)
                {
                    result = pair.Input[offset];
                }
                else
                {
                    offset -= pair.Input.Count;
                    result = pair.Ideal[offset];
                }
            }
            else
            {
                result = field.GetValue(index);
            }

            field.CurrentValue = result;
            return;
        }
        /// <summary>
        /// Called internally to obtain the current value for an input field.
        /// </summary>
        /// <param name="field">The input field to determine.</param>
        /// <param name="index">The current index.</param>
        /// <returns>The value for this input field.</returns>
        private void DetermineInputFieldValue(IInputField field, int index)
        {
            double result;

            if (field is InputFieldCSV)
            {
                var fieldCSV = (InputFieldCSV)field;
                ReadCSV csv = _csvMap[field];
                result = csv.GetDouble(fieldCSV.Offset);

            }
            else if (field is InputFieldMLDataSet)
            {
                var mlField = (InputFieldMLDataSet)field;
                MLDataFieldHolder holder = _dataSetFieldMap
                    [field];
                IMLDataPair pair = holder.Pair;
                int offset = mlField.Offset;
                if (offset < pair.Input.Count)
                {
                    result = pair.Input[offset];
                }
                else
                {
                    offset -= pair.Input.Count;
                    result = pair.Ideal[offset];
                }
            }
            else
            {
                result = field.GetValue(index);
            }

            field.CurrentValue = result;
            return;
        }

        /// <summary>
        /// Called internally to determine all of the input field values.
        /// </summary>
        /// <param name="index">The current index.</param>
        private void DetermineInputFieldValues(int index)
        {
            foreach (IInputField field in _inputFields)
            {
                DetermineInputFieldValue(field, index);
            }
        }


        /// <summary>
        /// Called internally to determine all of the input field values.
        /// </summary>
        /// <param name="index">The current index.</param>
        /// <param name="headers">if set to <c>true</c> [headers].</param>
        private void DetermineInputFieldValues(int index, bool headers)
        {
            foreach (IInputField field in _inputFields)
            {
                DetermineInputFieldValue(field, index, headers);
            }
        }


        /// <summary>
        /// Find an input field by its class.
        /// </summary>
        /// <param name="clazz">The input field class type you are looking for.</param>
        /// <param name="count">The instance of the input field needed, 0 for the first.</param>
        /// <returns>The input field if found, otherwise null.</returns>
        public IInputField FindInputField(Type clazz, int count)
        {
            int i = 0;
            foreach (IInputField field in _inputFields)
            {
                if (field.GetType().IsInstanceOfType(clazz))
                {
                    if (i == count)
                    {
                        return field;
                    }
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an output field by its class.
        /// </summary>
        /// <param name="clazz">The output field class type you are looking for.</param>
        /// <param name="count">The instance of the output field needed, 0 for the first.</param>
        /// <returns>The output field if found, otherwise null.</returns>
        public IOutputField FindOutputField(Type clazz, int count)
        {
            int i = 0;
            foreach (IOutputField field in _outputFields)
            {
                if (field.GetType().IsInstanceOfType(clazz) || field.GetType() == clazz)
                {
                    if (i == count)
                    {
                        return field;
                    }
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// First pass, count everything, establish min/max.
        /// </summary>
        private void FirstPass(bool headers)
        {
            OpenCSV(headers);
            OpenDataSet();

            _currentIndex = -1;
            _recordCount = 0;

            if (_report != null)
            {
                _report.Report(0, 0, "Analyzing file");
            }
            _lastReport = 0;
            int index = 0;

            InitForPass();

            // loop over all of the records
            while (Next())
            {
                DetermineInputFieldValues(index, headers);

                if (ShouldInclude())
                {
                    ApplyMinMax();
                    _recordCount++;
                    ReportResult("First pass, analyzing file", 0, _recordCount);
                }
                index++;
            }
        }




        /// <summary>
        /// First pass, count everything, establish min/max.
        /// This version doesn't read column names in csvinputfields.
        /// </summary>
        private void FirstPass()
        {
            OpenCSV();
            OpenDataSet();

            _currentIndex = -1;
            _recordCount = 0;

            if (_report != null)
            {
                _report.Report(0, 0, "Analyzing file");
            }
            _lastReport = 0;
            int index = 0;

            InitForPass();

            // loop over all of the records
            while (Next())
            {
                DetermineInputFieldValues(index);

                if (ShouldInclude())
                {
                    ApplyMinMax();
                    _recordCount++;
                    ReportResult("First pass, analyzing file", 0, _recordCount);
                }
                index++;
            }
        }





        /// <summary>
        /// Calculate the number of output fields that are not used as ideal
        /// values, these will be the input to the neural network.
        /// This is the input layer size for the neural network.
        /// </summary>
        /// <returns>The input layer size.</returns>
        public int GetNetworkInputLayerSize()
        {
            return _outputFields.Where(field => !field.Ideal).Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// The number of output fields that are used as ideal
        /// values, these will be the ideal output from the neural network.
        /// This is the output layer size for the neural network.
        /// </summary>
        /// <returns>The output layer size.</returns>
        public int GetNetworkOutputLayerSize()
        {
            return _outputFields.Where(field => field.Ideal).Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// The total size of all output fields.  This takes into
        /// account output fields that generate more than one value.
        /// </summary>
        /// <returns>The output field count.</returns>
        public int GetOutputFieldCount()
        {
            return _outputFields.Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForOutput()
        {
            // init groups
            foreach (IOutputFieldGroup group in _groups)
            {
                group.RowInit();
            }

            // init output fields
            foreach (IOutputField field in _outputFields)
            {
                field.RowInit();
            }
        }

        /// <summary>
        /// Called internally to advance to the next row.
        /// </summary>
        /// <returns>True if there are more rows to reed.</returns>
        private bool Next()
        {
            // see if any of the CSV readers want to stop
            if (_readCSV.Any(csv => !csv.Next()))
            {
                return false;
            }

            // see if any of the data sets want to stop
            foreach (var iterator in _readDataSet)
            {
                if (!iterator.MoveNext()) // are we sure that we intended for every other item here? an explanation would be helpful
                {
                    return false;
                }
                MLDataFieldHolder holder = _dataSetIteratorMap
                    [iterator];
                IMLDataPair pair = iterator.Current;
                holder.Pair = pair;
            }

            // see if any of the arrays want to stop
            if (_inputFields.OfType<IHasFixedLength>().Any(fl => (_currentIndex + 1) >= fl.Length))
            {
                return false;
            }

            _currentIndex++;

            return true;
        }

        /// <summary>
        /// Called internally to open the CSV file.
        /// </summary>
        private void OpenCSV()
        {
            // clear out any CSV files already there
            _csvMap.Clear();
            _readCSV.Clear();

            // only add each CSV once
            IDictionary<String, ReadCSV> uniqueFiles = new Dictionary<String, ReadCSV>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldCSV)
                {
                    var csvField = (InputFieldCSV)field;
                    String file = csvField.File;
                    if (!uniqueFiles.ContainsKey(file))
                    {
                        var csv = new ReadCSV(file, false,
                                              _csvFormat);
                        uniqueFiles[file] = csv;
                        _readCSV.Add(csv);
                    }
                    _csvMap[csvField] = uniqueFiles[file];
                }
            }
        }

        /// <summary>
        /// Called internally to open the CSV file with header.
        /// </summary>
        private void OpenCSV(bool headers)
        {
            // clear out any CSV files already there
            _csvMap.Clear();
            _readCSV.Clear();

            // only add each CSV once
            IDictionary<String, ReadCSV> uniqueFiles = new Dictionary<String, ReadCSV>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldCSV)
                {
                    var csvField = (InputFieldCSV)field;
                    String file = csvField.File;
                    if (!uniqueFiles.ContainsKey(file))
                    {
                        var csv = new ReadCSV(file, headers,
                                              _csvFormat);
                        uniqueFiles[file] = csv;
                        _readCSV.Add(csv);
                    }
                    _csvMap[csvField] = uniqueFiles[file];
                }
            }
        }


        /// <summary>
        /// Open any datasets that were used by the input layer.
        /// </summary>
        private void OpenDataSet()
        {
            // clear out any data sets already there
            _readDataSet.Clear();
            _dataSetFieldMap.Clear();
            _dataSetIteratorMap.Clear();

            // only add each iterator once
            IDictionary<IMLDataSet, MLDataFieldHolder> uniqueSets = new Dictionary<IMLDataSet, MLDataFieldHolder>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldMLDataSet)
                {
                    var dataSetField = (InputFieldMLDataSet)field;
                    IMLDataSet dataSet = dataSetField.NeuralDataSet;
                    if (!uniqueSets.ContainsKey(dataSet))
                    {
                        IEnumerator<IMLDataPair> iterator = dataSet
                            .GetEnumerator();
                        var holder = new MLDataFieldHolder(
                            iterator, dataSetField);
                        uniqueSets[dataSet] = holder;
                        _readDataSet.Add(iterator);
                    }

                    MLDataFieldHolder holder2 = uniqueSets[dataSet];

                    _dataSetFieldMap[dataSetField] = holder2;
                    _dataSetIteratorMap[holder2.GetEnumerator()] = holder2;
                }
            }
        }

        /// <summary>
        /// Call this method to begin the normalization process.  Any status 
        /// updates will be sent to the class specified in the constructor.
        /// </summary>
        public void Process()
        {
            Init();
            if (TwoPassesNeeded())
            {
                FirstPass();
            }
            SecondPass();

            // clean up
            foreach (var csv in _readCSV)
            {
                csv.Close();
            }
        }
        /// <summary>
        /// Call this method to begin the normalization process.  Any status 
        /// updates will be sent to the class specified in the constructor.
        /// this version uses headers.
        /// </summary>
        public void Process(bool headers)
        {
            Init();
            if (TwoPassesNeeded())
            {
                FirstPass(headers);
            }
            SecondPass(headers);
        }
        /// <summary>
        /// Report on the current progress.
        /// </summary>
        /// <param name="message">The message to report.</param>
        /// <param name="total">The total number of records to process, 0 for unknown.</param>
        /// <param name="current"> The current record.</param>
        private void ReportResult(String message, int total,
                                  int current)
        {
            // count the records, report status
            _lastReport++;
            if (_lastReport >= 10000)
            {
                _report.Report(total, current, message);
                _lastReport = 0;
            }
        }

        /// <summary>
        /// The second pass actually writes the data to the output files.
        /// </summary>
        private void SecondPass()
        {
            bool twopass = TwoPassesNeeded();

            // move any CSV and datasets files back to the beginning.
            OpenCSV();
            OpenDataSet();
            InitForPass();

            _currentIndex = -1;

            // process the records
            int size = GetOutputFieldCount();
            var output = new double[size];

            _storage.Open();
            _lastReport = 0;
            int index = 0;
            int current = 0;
            while (Next())
            {
                // read the value
                foreach (IInputField field in _inputFields)
                {
                    DetermineInputFieldValue(field, index);
                }

                if (ShouldInclude())
                {
                    // handle groups
                    InitForOutput();

                    // write the value
                    int outputIndex = 0;
                    foreach (IOutputField ofield in _outputFields)
                    {
                        for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                        {
                            output[outputIndex++] = ofield.Calculate(sub);
                        }
                    }

                    ReportResult(twopass ? "Second pass, normalizing data" : "Processing data (single pass)",
                                 _recordCount, ++current);
                    _storage.Write(output, 0);
                }

                index++;
            }
            _storage.Close();
        }




        /// <summary>
        /// The second pass actually writes the data to the output files.
        /// </summary>
        private void SecondPass(bool headers)
        {
            bool twopass = TwoPassesNeeded();

            // move any CSV and datasets files back to the beginning.
            OpenCSV(headers);
            OpenDataSet();
            InitForPass();

            _currentIndex = -1;

            // process the records
            int size = GetOutputFieldCount();
            var output = new double[size];

            _storage.Open();
            _lastReport = 0;
            int index = 0;
            int current = 0;
            while (Next())
            {
                // read the value
                foreach (IInputField field in _inputFields)
                {
                    DetermineInputFieldValue(field, index, headers);
                }

                if (ShouldInclude())
                {
                    // handle groups
                    InitForOutput();

                    // write the value
                    int outputIndex = 0;
                    foreach (IOutputField ofield in _outputFields)
                    {
                        for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                        {
                            output[outputIndex++] = ofield.Calculate(sub);
                        }
                    }

                    ReportResult(twopass ? "Second pass, normalizing data" : "Processing data (single pass)",
                                 _recordCount, ++current);
                    _storage.Write(output, 0);
                }

                index++;
            }
            _storage.Close();
        }


        /// <summary>
        /// Should this row be included? Check the segregatprs.
        /// </summary>
        /// <returns>True if the row should be included.</returns>
        private bool ShouldInclude()
        {
            return _segregators.All(segregator => segregator.ShouldInclude());
        }


        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForPass()
        {
            // init segregators
            foreach (ISegregator segregator in _segregators)
            {
                segregator.PassInit();
            }
        }

        /// <summary>
        /// Determine if two passes will be needed.
        /// </summary>
        /// <returns>True if two passes will be needed.</returns>
        public bool TwoPassesNeeded()
        {
            return _outputFields.OfType<IRequireTwoPass>().Any();
        }

        private void Init()
        {
            _csvMap = new Dictionary<IInputField, ReadCSV>();
            _dataSetFieldMap = new Dictionary<IInputField, MLDataFieldHolder>();
            _dataSetIteratorMap = new Dictionary<IEnumerator<IMLDataPair>, MLDataFieldHolder>();
            _readCSV = new List<ReadCSV>();
            _readDataSet = new List<IEnumerator<IMLDataPair>>();


        }
    }
}
