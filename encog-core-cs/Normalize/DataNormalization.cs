// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;
using Encog.Normalize.Input;
using Encog.Normalize.Output;
using Encog.Neural.Data.Basic;
using Encog.Neural.Data;
using Encog.MathUtil;
using Encog.Neural.NeuralData;
using Encog.Normalize.Segregate;
using Encog.Normalize.Target;
using Encog.Persist;
using Encog.Persist.Persistors.Generic;
using Encog.Util.CSV;

namespace Encog.Normalize
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
    [EGReferenceable]
    public class DataNormalization : IEncogPersistedObject
    {
        /// <summary>
        /// The input fields.
        /// </summary>
        private ICollection<IInputField> inputFields = new List<IInputField>();

        /// <summary>
        /// The output fields.
        /// </summary>
        private ICollection<IOutputField> outputFields = new List<IOutputField>();

        /// <summary>
        /// Keep a collection of all of the ReadCSV classes to support all of the
        /// distinct CSV files that are to be read.
        /// </summary>
        [EGIgnore]
        private ICollection<ReadCSV> readCSV = new List<ReadCSV>();

        /// <summary>
        /// Hold a map between the InputFieldCSV objects and the corresponding
        /// ReadCSV object. There will likely be many fields read from a single file.
        /// This allows only one ReadCSV object to need to be created per actual CSV
        /// file.
        /// </summary>
        [EGIgnore]
        private IDictionary<IInputField, ReadCSV> csvMap = new Dictionary<IInputField, ReadCSV>();

        /// <summary>
        /// For each InputFieldNeuralDataSet input field an Iterator must be kept to
        /// actually access the data. Only one Iterator should be kept per data set
        /// actually used.
        /// </summary>
        [EGIgnore]
        private ICollection<IEnumerator<INeuralDataPair>> readDataSet = new List<IEnumerator<INeuralDataPair>>();


        /**
         * Map each of the input fields to an internally-build NeuralDataFieldHolder object.
         * The NeuralDataFieldHolder object holds an Iterator, InputField and last 
         * NeuralDataPair object loaded.
         */
        [EGIgnore]
        private IDictionary<IInputField, NeuralDataFieldHolder> dataSetFieldMap = new Dictionary<IInputField, NeuralDataFieldHolder>();

        /// <summary>
        /// Map each of the NeuralDataSet Iterators to an internally-build NeuralDataFieldHolder 
        /// object. The NeuralDataFieldHolder object holds an Iterator, InputField and last 
        /// NeuralDataPair object loaded.
        /// </summary>
        [EGIgnore]
        private IDictionary<IEnumerator<INeuralDataPair>, NeuralDataFieldHolder> dataSetIteratorMap = new Dictionary<IEnumerator<INeuralDataPair>, NeuralDataFieldHolder>();

        /// <summary>
        /// Output fields can be grouped together, if the value of one output field might 
        /// affect all of the others.  This collection holds a list of all of the output 
        /// field groups.
        /// </summary>
        private ICollection<IOutputFieldGroup> groups = new List<IOutputFieldGroup>();

        /// <summary>
        /// A list of the segregators.
        /// </summary>
        private ICollection<ISegregator> segregators = new List<ISegregator>();

        /// <summary>
        /// Where the final output from the normalization is sent.
        /// </summary>
        [EGIgnore]
        private INormalizationStorage storage;

        /// <summary>
        /// The object to report the progress of the normalization to.
        /// </summary>
        [EGIgnore]
        private IStatusReportable report = new NullStatusReportable();

        /// <summary>
        /// The number of records that were found in the first pass.
        /// </summary>
        private int recordCount;

        /// <summary>
        /// The current record's index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The format to use for all CSV files.
        /// </summary>
        private CSVFormat csvFormat = CSVFormat.ENGLISH;

        /// <summary>
        /// How long has it been since the last report.  This filters so that
        /// every single record does not produce a message.
        /// </summary>
        private int lastReport;

        /// <summary>
        /// The name of this object.
        /// </summary>
        private String name;

        /// <summary>
        /// The description of this object.
        /// </summary>
        private String description;

        /// <summary>
        /// Add an input field.
        /// </summary>
        /// <param name="f">The input field to add.</param>
        public void AddInputField(IInputField f)
        {
            this.inputFields.Add(f);
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
            this.outputFields.Add(outputField);
            outputField.Ideal = ideal;
            if (outputField is OutputFieldGrouped)
            {
                OutputFieldGrouped ofg = (OutputFieldGrouped)outputField;
                this.groups.Add(ofg.Group);
            }
        }

        /// <summary>
        ///  Add a segregator.
        /// </summary>
        /// <param name="segregator">The segregator to add.</param>
        public void AddSegregator(ISegregator segregator)
        {
            this.segregators.Add(segregator);
            segregator.Init(this);
        }

        /// <summary>
        /// Called internally to allow each of the input fields to update their
        /// min/max values in the first pass.
        /// </summary>
        private void ApplyMinMax()
        {
            foreach (IInputField field in this.inputFields)
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
        public INeuralData BuildForNetworkInput(double[] data)
        {

            // feed the input fields
            int index = 0;
            foreach (IInputField field in this.inputFields)
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
            foreach (IOutputField ofield in this.outputFields)
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

            INeuralData result = new BasicNeuralData(outputCount);

            // write the value
            int outputIndex = 0;
            foreach (IOutputField ofield in this.outputFields)
            {
                if (!ofield.Ideal)
                {
                    for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                    {
                        result.Data[outputIndex++] = ofield.Calculate(sub);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// A persistor to persist this DataNormalization object. 
        /// </summary>
        /// <returns>The persistor.</returns>
        public IPersistor CreatePersistor()
        {
            return new GenericPersistor(typeof(DataNormalization));
        }

        /// <summary>
        /// Called internally to obtain the current value for an input field.
        /// </summary>
        /// <param name="field">The input field to determine.</param>
        /// <param name="index">The current index.</param>
        /// <returns>The value for this input field.</returns>
        private double DetermineInputFieldValue(IInputField field,
                 int index)
        {
            double result = 0;

            if (field is InputFieldCSV)
            {
                InputFieldCSV fieldCSV = (InputFieldCSV)field;
                ReadCSV csv = this.csvMap[field];
                result = csv.GetDouble(fieldCSV.Offset);
            }
            else if (field is InputFieldNeuralDataSet)
            {
                InputFieldNeuralDataSet neuralField = (InputFieldNeuralDataSet)field;
                NeuralDataFieldHolder holder = this.dataSetFieldMap
                       [field];
                INeuralDataPair pair = holder.Pair;
                int offset = neuralField.Offset;
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
            return result;
        }

        /// <summary>
        /// Called internally to determine all of the input field values.
        /// </summary>
        /// <param name="index">The current index.</param>
        private void DetermineInputFieldValues(int index)
        {
            foreach (IInputField field in this.inputFields)
            {
                DetermineInputFieldValue(field, index);
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
            foreach (IInputField field in this.inputFields)
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
            foreach (IOutputField field in this.outputFields)
            {
                if (field.GetType().IsInstanceOfType(clazz) || field.GetType()==clazz)
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
        private void FirstPass()
        {
            OpenCSV();
            OpenDataSet();

            this.currentIndex = -1;
            this.recordCount = 0;

            this.report.Report(0, 0, "Analyzing file");
            this.lastReport = 0;
            int index = 0;

            InitForPass();

            // loop over all of the records
            while (Next())
            {

                DetermineInputFieldValues(index);

                if (ShouldInclude())
                {
                    ApplyMinMax();
                    this.recordCount++;
                    ReportResult("First pass, analyzing file", 0, this.recordCount);
                }
                index++;
            }
        }

        /// <summary>
        /// The CSV format being used.
        /// </summary>
        public CSVFormat CSVFormatUsed
        {
            get
            {
                return this.csvFormat;
            }
            set
            {
                this.csvFormat = value;
            }
        }

        /// <summary>
        /// The description of this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The object groups.
        /// </summary>
        public ICollection<IOutputFieldGroup> Groups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// The input fields.
        /// </summary>
        public ICollection<IInputField> InputFields
        {
            get
            {
                return this.inputFields;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
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
            int result = 0;
            foreach (IOutputField field in this.outputFields)
            {
                if (!field.Ideal)
                {
                    result += field.SubfieldCount;
                }
            }
            return result;
        }

        /// <summary>
        /// The number of output fields that are used as ideal
        /// values, these will be the ideal output from the neural network.
        /// This is the output layer size for the neural network.
        /// </summary>
        /// <returns>The output layer size.</returns>
        public int GetNetworkOutputLayerSize()
        {
            int result = 0;
            foreach (IOutputField field in this.outputFields)
            {
                if (field.Ideal)
                {
                    result += field.SubfieldCount;
                }
            }
            return result;
        }

        /// <summary>
        /// The total size of all output fields.  This takes into
        /// account output fields that generate more than one value.
        /// </summary>
        /// <returns>The output field count.</returns>
        public int GetOutputFieldCount()
        {
            int result = 0;
            foreach (IOutputField field in this.outputFields)
            {
                result += field.SubfieldCount;
            }
            return result;
        }

        /// <summary>
        /// The output fields.
        /// </summary>
        public ICollection<IOutputField> OutputFields
        {
            get
            {
                return this.outputFields;
            }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get
            {
                return this.recordCount;
            }
        }

        /// <summary>
        /// The class that progress will be reported to.
        /// </summary>
        public IStatusReportable Report
        {
            get
            {
                return this.report;
            }
            set
            {
                this.report = value;
            }
        }

        /// <summary>
        /// The segregators in use.
        /// </summary>
        public ICollection<ISegregator> Segregators
        {
            get
            {
                return this.segregators;
            }
        }

        /// <summary>
        /// The place that the normalization output will be stored.
        /// </summary>
        public INormalizationStorage Storage
        {
            get
            {
                return this.storage;
            }
            set
            {
                this.storage = value;
            }
        }

        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForOutput()
        {

            // init groups
            foreach (IOutputFieldGroup group in this.groups)
            {
                group.RowInit();
            }

            // init output fields
            foreach (IOutputField field in this.outputFields)
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
            foreach (ReadCSV csv in this.readCSV)
            {
                if (!csv.Next())
                {
                    return false;
                }
            }

            // see if any of the data sets want to stop
            foreach (IEnumerator<INeuralDataPair> iterator in this.readDataSet)
            {
                if (!iterator.MoveNext())
                {
                    return false;
                }
                NeuralDataFieldHolder holder = this.dataSetIteratorMap
                       [iterator];
                INeuralDataPair pair = iterator.Current;
                holder.Pair = pair;
            }

            // see if any of the arrays want to stop
            foreach (IInputField field in this.inputFields)
            {
                if (field is IHasFixedLength)
                {
                    IHasFixedLength fl = (IHasFixedLength)field;
                    if ((this.currentIndex + 1) >= fl.Length)
                    {
                        return false;
                    }
                }
            }

            this.currentIndex++;

            return true;
        }

        /// <summary>
        /// Called internally to open the CSV file.
        /// </summary>
        private void OpenCSV()
        {
            // clear out any CSV files already there
            this.csvMap.Clear();
            this.readCSV.Clear();

            // only add each CSV once
            IDictionary<String, ReadCSV> uniqueFiles = new Dictionary<String, ReadCSV>();

            // find the unique files
            foreach (IInputField field in this.inputFields)
            {
                if (field is InputFieldCSV)
                {
                    InputFieldCSV csvField = (InputFieldCSV)field;
                    String file = csvField.File;
                    if (!uniqueFiles.ContainsKey(file))
                    {
                        ReadCSV csv = new ReadCSV(file, false,
                               this.csvFormat);
                        uniqueFiles[file] = csv;
                        this.readCSV.Add(csv);
                    }
                    this.csvMap[csvField] = uniqueFiles[file];
                }
            }
        }

        /// <summary>
        /// Open any datasets that were used by the input layer.
        /// </summary>
        private void OpenDataSet()
        {
            // clear out any data sets already there
            this.readDataSet.Clear();
            this.dataSetFieldMap.Clear();
            this.dataSetIteratorMap.Clear();

            // only add each iterator once
            IDictionary<INeuralDataSet, NeuralDataFieldHolder> uniqueSets = new Dictionary<INeuralDataSet, NeuralDataFieldHolder>();

            // find the unique files
            foreach (IInputField field in this.inputFields)
            {
                if (field is InputFieldNeuralDataSet)
                {
                    InputFieldNeuralDataSet dataSetField = (InputFieldNeuralDataSet)field;
                    INeuralDataSet dataSet = dataSetField.NeuralDataSet;
                    if (!uniqueSets.ContainsKey(dataSet))
                    {
                        IEnumerator<INeuralDataPair> iterator = dataSet
                               .GetEnumerator();
                        NeuralDataFieldHolder holder = new NeuralDataFieldHolder(
                               iterator, dataSetField);
                        uniqueSets[dataSet] = holder;
                        this.readDataSet.Add(iterator);
                    }

                    NeuralDataFieldHolder holder2 = uniqueSets[dataSet];

                    this.dataSetFieldMap[dataSetField] = holder2;
                    this.dataSetIteratorMap[holder2.GetEnumerator()] = holder2;
                }
            }
        }

        /// <summary>
        /// Call this method to begin the normalization process.  Any status 
        /// updates will be sent to the class specified in the constructor.
        /// </summary>
        public void Process()
        {
            if (TwoPassesNeeded())
            {
                FirstPass();
            }
            SecondPass();
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
            this.lastReport++;
            if (this.lastReport >= 10000)
            {
                this.report.Report(total, current, message);
                this.lastReport = 0;
            }
        }

        /// <summary>
        /// The second pass actually writes the data to the output files.
        /// </summary>
        private void SecondPass()
        {
            bool twopass = this.TwoPassesNeeded();

            // move any CSV and datasets files back to the beginning.
            OpenCSV();
            OpenDataSet();
            InitForPass();

            this.currentIndex = -1;

            // process the records
            int size = GetOutputFieldCount();
            double[] output = new double[size];

            this.storage.Open();
            this.lastReport = 0;
            int index = 0;
            int current = 0;
            while (Next())
            {
                // read the value
                foreach (IInputField field in this.inputFields)
                {
                    DetermineInputFieldValue(field, index);
                }

                if (ShouldInclude())
                {
                    // handle groups
                    InitForOutput();

                    // write the value
                    int outputIndex = 0;
                    foreach (IOutputField ofield in this.outputFields)
                    {
                        for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                        {
                            output[outputIndex++] = ofield.Calculate(sub);
                        }
                    }

                    if (twopass)
                    {
                        ReportResult("Second pass, normalizing data",
                                this.recordCount, ++current);
                    }
                    else
                    {
                        ReportResult("Processing data (single pass)",
                                this.recordCount, ++current);
                    }
                    this.storage.Write(output, 0);
                }

                index++;
            }
            this.storage.Close();

        }

        /// <summary>
        /// Should this row be included? Check the segregatprs.
        /// </summary>
        /// <returns>True if the row should be included.</returns>
        private bool ShouldInclude()
        {
            foreach (ISegregator segregator in this.segregators)
            {
                if (!segregator.ShouldInclude())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns>Not implemented.</returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForPass()
        {

            // init segregators
            foreach (ISegregator segregator in this.segregators)
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
            foreach (IOutputField field in this.outputFields)
            {
                if (field is IRequireTwoPass)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
