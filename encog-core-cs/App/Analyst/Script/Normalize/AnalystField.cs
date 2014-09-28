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
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.ML.Data;
using Encog.MathUtil;
using Encog.Util;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script.Normalize
{
    /// <summary>
    ///     Holds a field to be analyzed.
    /// </summary>
    public class AnalystField
    {
        /// <summary>
        ///     Minimum classes for encode using equilateral.
        /// </summary>
        public const int MinEqClasses = 3;

        /// <summary>
        ///     The list of classes.
        /// </summary>
        private readonly IList<ClassItem> _classes;

        /// <summary>
        ///     Allows the index of a field to be looked up.
        /// </summary>
        private readonly IDictionary<String, Int32> _lookup;

        /// <summary>
        ///     The action that should be taken on this column.
        /// </summary>
        private NormalizationAction _action;

        /// <summary>
        ///     The actual high from the sample data.
        /// </summary>
        private double _actualHigh;

        /// <summary>
        ///     The actual low from the sample data.
        /// </summary>
        private double _actualLow;

        /// <summary>
        ///     If equilateral classification is used, this is the Equilateral object.
        /// </summary>
        private Equilateral _eq;

        /// <summary>
        ///     The name of this column.
        /// </summary>
        private String _name;

        /// <summary>
        ///     The desired normalized high.
        /// </summary>
        private double _normalizedHigh;

        /// <summary>
        ///     The desired normalized low from the sample data.
        /// </summary>
        private double _normalizedLow;

        /// <summary>
        ///     True, if this is an output field.
        /// </summary>
        private bool _output;

        /// <summary>
        ///     The time slice number.
        /// </summary>
        private int _timeSlice;

        /// <summary>
        ///     Construct the object with a range of 1 and -1.
        /// </summary>
        public AnalystField() : this(1, -1)
        {
        }

        /// <summary>
        ///     Construct an analyst field.  Works like a C++ copy constructor.
        /// </summary>
        /// <param name="field">The field to clone.</param>
        public AnalystField(AnalystField field)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _actualHigh = field._actualHigh;
            _actualLow = field._actualLow;
            _normalizedHigh = field._normalizedHigh;
            _normalizedLow = field._normalizedLow;
            _action = field._action;
            _name = field._name;
            _output = field._output;
            _timeSlice = field._timeSlice;
            FixSingleValue();
        }

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="theNormalizedHigh">The normalized high.</param>
        /// <param name="theNormalizedLow">The normalized low.</param>
        public AnalystField(double theNormalizedHigh,
                            double theNormalizedLow)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _normalizedHigh = theNormalizedHigh;
            _normalizedLow = theNormalizedLow;
            _actualHigh = Double.MinValue;
            _actualLow = Double.MaxValue;
            _action = NormalizationAction.Normalize;
            FixSingleValue();
        }

        /// <summary>
        ///     Construct an object.
        /// </summary>
        /// <param name="theAction">The desired action.</param>
        /// <param name="theName">The name of this column.</param>
        public AnalystField(NormalizationAction theAction,
                            String theName) : this(theAction, theName, 0, 0, 0, 0)
        {
        }

        /// <summary>
        ///     Construct the field, with no defaults.
        /// </summary>
        /// <param name="theAction">The normalization action to take.</param>
        /// <param name="theName">The name of this field.</param>
        /// <param name="ahigh">The actual high.</param>
        /// <param name="alow">The actual low.</param>
        /// <param name="nhigh">The normalized high.</param>
        /// <param name="nlow">The normalized low.</param>
        public AnalystField(NormalizationAction theAction,
                            String theName, double ahigh, double alow,
                            double nhigh, double nlow)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _action = theAction;
            _actualHigh = ahigh;
            _actualLow = alow;
            _normalizedHigh = nhigh;
            _normalizedLow = nlow;
            _name = theName;
            FixSingleValue();
        }

        /// <summary>
        ///     Construct an analyst field to use.
        /// </summary>
        /// <param name="theName">The name of the field.</param>
        /// <param name="theAction">The action to use.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public AnalystField(String theName,
                            NormalizationAction theAction, double high,
                            double low)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _name = theName;
            _action = theAction;
            _normalizedHigh = high;
            _normalizedLow = low;
            FixSingleValue();
        }

        /// <summary>
        ///     Set the theAction for the field.
        /// </summary>
        public NormalizationAction Action
        {
            get { return _action; }
            set { _action = value; }
        }


        /// <summary>
        ///     Set the actual high for the field.
        /// </summary>
        public double ActualHigh
        {
            get { return _actualHigh; }
            set { _actualHigh = value; }
        }


        /// <summary>
        ///     Set the actual low for the field.
        /// </summary>
        public double ActualLow
        {
            get { return _actualLow; }
            set { _actualLow = value; }
        }


        /// <value>The classes.</value>
        public IList<ClassItem> Classes
        {
            get { return _classes; }
        }


        /// <value>
        ///     Returns the number of columns needed for this classification. The
        ///     number of columns needed will vary, depending on the
        ///     classification method used.
        /// </value>
        public int ColumnsNeeded
        {
            get
            {
                switch (_action)
                {
                    case NormalizationAction.Ignore:
                        return 0;
                    case NormalizationAction.Equilateral:
                        return _classes.Count - 1;
                    case NormalizationAction.OneOf:
                        return _classes.Count;
                    default:
                        return 1;
                }
            }
        }


        /// <value>The equilateral utility.</value>
        public Equilateral Eq
        {
            get { return _eq; }
        }


        /// <summary>
        ///     Set the name of the field.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        ///     Set the normalized high for the field.
        /// </summary>
        public double NormalizedHigh
        {
            get { return _normalizedHigh; }
            set { _normalizedHigh = value; }
        }


        /// <summary>
        ///     Set the normalized low for the field.
        /// </summary>
        /// <value>The normalized low for the field.</value>
        public double NormalizedLow
        {
            get { return _normalizedLow; }
            set { _normalizedLow = value; }
        }


        /// <value>the timeSlice to set</value>
        public int TimeSlice
        {
            get { return _timeSlice; }
            set { _timeSlice = value; }
        }


        /// <value>True if this field is classification.</value>
        public bool Classify
        {
            get
            {
                return (_action == NormalizationAction.Equilateral)
                       || (_action == NormalizationAction.OneOf)
                       || (_action == NormalizationAction.SingleField);
            }
        }


        /// <value>Is this field ignored.</value>
        public bool Ignored
        {
            get { return _action == NormalizationAction.Ignore; }
        }


        /// <value>Is this field input.</value>
        public bool Input
        {
            get { return !_output; }
        }


        /// <summary>
        ///     Set if this is an output field.
        /// </summary>
        public bool Output
        {
            get { return _output; }
            set { _output = value; }
        }

        /// <summary>
        ///     Add headings for a raw file.
        /// </summary>
        /// <param name="line">The line to write the raw headings to.</param>
        /// <param name="prefix">The prefix to place.</param>
        /// <param name="format">The format to use.</param>
        public void AddRawHeadings(StringBuilder line,
                                   String prefix, CSVFormat format)
        {
            int subFields = ColumnsNeeded;

            for (int i = 0; i < subFields; i++)
            {
                String str = CSVHeaders.TagColumn(_name, i,
                                                  _timeSlice, subFields > 1);
                BasicFile.AppendSeparator(line, format);
                line.Append('\"');
                if (prefix != null)
                {
                    line.Append(prefix);
                }
                line.Append(str);
                line.Append('\"');
            }
        }

        /// <summary>
        ///     Analyze the specified value. Adjust min/max as needed. Usually used only
        ///     internally.
        /// </summary>
        /// <param name="d">The value to analyze.</param>
        public void Analyze(double d)
        {
            _actualHigh = Math.Max(_actualHigh, d);
            _actualLow = Math.Min(_actualLow, d);
        }

        /// <summary>
        ///     Denormalize the specified value.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double DeNormalize(double v)
        {
            double result = ((_actualLow - _actualHigh)*v
                             - _normalizedHigh*_actualLow + _actualHigh
                             *_normalizedLow)
                            /(_normalizedLow - _normalizedHigh);

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                return ((NormalizedHigh - NormalizedLow)/2) + NormalizedLow;
            }

            return result;
        }

        /// <summary>
        ///     Determine what class the specified data belongs to.
        /// </summary>
        /// <param name="data">The data to analyze.</param>
        /// <returns>The class the data belongs to.</returns>
        public ClassItem DetermineClass(double[] data)
        {
            int resultIndex = 0;

            switch (_action)
            {
                case NormalizationAction.Equilateral:
                    resultIndex = _eq.Decode(data);
                    break;
                case NormalizationAction.OneOf:
                    resultIndex = EngineArray.IndexOfLargest(data);
                    break;
                case NormalizationAction.SingleField:
                    resultIndex = (int) data[0];
                    break;
                default:
                    throw new AnalystError("Unknown action: " + _action);
            }

            return _classes[resultIndex];
        }

        /// <summary>
        ///     Determine the class using part of an array.
        /// </summary>
        /// <param name="pos">The position to begin.</param>
        /// <param name="data">The array to check.</param>
        /// <returns>The class item.</returns>
        public ClassItem DetermineClass(int pos, IMLData data)
        {
            int resultIndex = 0;
            var d = new double[ColumnsNeeded];
            for (int i = 0; i < d.Length; i++)
                d[i] = data[pos + i];

            switch (_action)
            {
                case NormalizationAction.Equilateral:
                    resultIndex = _eq.Decode(d);
                    break;
                case NormalizationAction.OneOf:
                    resultIndex = EngineArray.IndexOfLargest(d);
                    break;
                case NormalizationAction.SingleField:
                    resultIndex = (int) d[0];
                    break;
                default:
                    throw new AnalystError("Invalid action: " + _action);
            }

            if (resultIndex < 0)
            {
                return null;
            }

            return _classes[resultIndex];
        }

        /// <summary>
        ///     Encode the class.
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded class.</returns>
        public double[] Encode(int classNumber)
        {
            switch (_action)
            {
                case NormalizationAction.OneOf:
                    return EncodeOneOf(classNumber);
                case NormalizationAction.Equilateral:
                    return EncodeEquilateral(classNumber);
                case NormalizationAction.SingleField:
                    return EncodeSingleField(classNumber);
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Encode the string to numeric form.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>The numeric form.</returns>
        public double[] Encode(String str)
        {
            int classNumber = Lookup(str);
            if (classNumber == -1)
            {
                try
                {
                    classNumber = Int32.Parse(str);
                }
                catch (FormatException)
                {
                    throw new QuantError("Can't determine class for: " + str);
                }
            }
            return Encode(classNumber);
        }

        /// <summary>
        ///     Perform an equilateral encode.
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The class to encode.</returns>
        public double[] EncodeEquilateral(int classNumber)
        {
            return _eq.Encode(classNumber);
        }

        /// <summary>
        ///     Perform the encoding for "one of".
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded columns.</returns>
        private double[] EncodeOneOf(int classNumber)
        {
            var result = new double[ColumnsNeeded];

            for (int i = 0; i < _classes.Count; i++)
            {
                if (i == classNumber)
                {
                    result[i] = _normalizedHigh;
                }
                else
                {
                    result[i] = _normalizedLow;
                }
            }
            return result;
        }

        /// <summary>
        ///     Encode a single field.
        /// </summary>
        /// <param name="classNumber">The class number to encode.</param>
        /// <returns>The encoded columns.</returns>
        private double[] EncodeSingleField(int classNumber)
        {
            var d = new double[1];
            d[0] = classNumber;
            return d;
        }

        /// <summary>
        ///     Fix normalized fields that have a single value for the min/max. Separate
        ///     them by 2 units.
        /// </summary>
        public void FixSingleValue()
        {
            if (_action == NormalizationAction.Normalize)
            {
                if (Math.Abs(_actualHigh - _actualLow) < EncogFramework.DefaultDoubleEqual)
                {
                    _actualHigh += 1;
                    _actualLow -= 1;
                }
            }
        }

        /// <summary>
        ///     Init any internal structures.
        /// </summary>
        public void Init()
        {
            if (_action == NormalizationAction.Equilateral)
            {
                if (_classes.Count < MinEqClasses)
                {
                    throw new QuantError(
                        "There must be at least three classes to make "
                        + "use of equilateral normalization.");
                }

                _eq = new Equilateral(_classes.Count, _normalizedHigh,
                                      _normalizedLow);
            }

            // build lookup map
            for (int i = 0; i < _classes.Count; i++)
            {
                _lookup[_classes[i].Name] = _classes[i].Index;
            }
        }


        /// <summary>
        ///     Lookup the specified field.
        /// </summary>
        /// <param name="str">The name of the field to lookup.</param>
        /// <returns>The index of the field, or -1 if not found.</returns>
        public int Lookup(String str)
        {
            if (!_lookup.ContainsKey(str))
            {
                return -1;
            }
            return _lookup[str];
        }

        /// <summary>
        ///     Make the classes based on numbers.
        /// </summary>
        /// <param name="theAction">The action.</param>
        /// <param name="classFrom">The starting class.</param>
        /// <param name="classTo">The ending class.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              int classFrom, int classTo, int high,
                              int low)
        {
            if ((_action != NormalizationAction.Equilateral)
                && (_action != NormalizationAction.OneOf)
                && (_action != NormalizationAction.SingleField))
            {
                throw new QuantError("Unsupported normalization type");
            }

            _action = theAction;
            _classes.Clear();
            _normalizedHigh = high;
            _normalizedLow = low;
            _actualHigh = 0;
            _actualLow = 0;

            int index = 0;
            for (int i = classFrom; i < classTo; i++)
            {
                _classes.Add(new ClassItem("" + i, index++));
            }
        }

        /// <summary>
        ///     Make the classes using names.
        /// </summary>
        /// <param name="theAction">The action to use.</param>
        /// <param name="cls">The class names.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              String[] cls, double high, double low)
        {
            if ((_action != NormalizationAction.Equilateral)
                && (_action != NormalizationAction.OneOf)
                && (_action != NormalizationAction.SingleField))
            {
                throw new QuantError("Unsupported normalization type");
            }

            _action = theAction;
            _classes.Clear();
            _normalizedHigh = high;
            _normalizedLow = low;
            _actualHigh = 0;
            _actualLow = 0;

            for (int i = 0; i < cls.Length; i++)
            {
                _classes.Add(new ClassItem(cls[i], i));
            }
        }

        /// <summary>
        ///     Make this a pass-through field.
        /// </summary>
        public void MakePassThrough()
        {
            _normalizedHigh = 0;
            _normalizedLow = 0;
            _actualHigh = 0;
            _actualLow = 0;
            _action = NormalizationAction.PassThrough;
        }

        /// <summary>
        ///     Normalize the specified value.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double Normalize(double v)
        {
            double result = ((v - _actualLow)/(_actualHigh - _actualLow))
                            *(_normalizedHigh - _normalizedLow)
                            + _normalizedLow;

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                return ((NormalizedHigh - NormalizedLow)/2) + NormalizedLow;
            }

            return result;
        }

        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", actualHigh=");
            result.Append(_actualHigh);
            result.Append(", actualLow=");
            result.Append(_actualLow);

            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     Determine the mode, this is the class item that has the most instances.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        /// <returns>The mode.</returns>
        public int DetermineMode(EncogAnalyst analyst)
        {
            if (!Classify)
            {
                throw new AnalystError("Can only calculate the mode for a class.");
            }

            DataField df = analyst.Script.FindDataField(Name);
            AnalystClassItem m = null;
            int result = 0;
            int idx = 0;
            foreach (AnalystClassItem item in df.ClassMembers)
            {
                if (m == null || m.Count < item.Count)
                {
                    m = item;
                    result = idx;
                }
                idx++;
            }

            return result;
        }
    }
}
