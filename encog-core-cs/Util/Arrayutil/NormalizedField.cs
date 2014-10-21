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
using Encog.App.Quant;
using Encog.MathUtil;
using Encog.Util.CSV;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// This object holds the normalization stats for a column. This includes the
    /// actual and desired high-low range for this column.
    /// </summary>
    ///
    public class NormalizedField
    {
        /// <summary>
        /// The list of classes.
        /// </summary>
        ///
        private readonly IList<ClassItem> _classes;

        /// <summary>
        /// Allows the index of a field to be looked up.
        /// </summary>
        ///
        private readonly IDictionary<String, Int32> _lookup;

        /// <summary>
        /// The action that should be taken on this column.
        /// </summary>
        ///
        private NormalizationAction _action;

        /// <summary>
        /// The actual high from the sample data.
        /// </summary>
        ///
        private double _actualHigh;

        /// <summary>
        /// The actual low from the sample data.
        /// </summary>
        ///
        private double _actualLow;

        /// <summary>
        /// If equilateral classification is used, this is the Equilateral object.
        /// </summary>
        ///
        private Equilateral _eq;

        /// <summary>
        /// The name of this column.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// The desired normalized high.
        /// </summary>
        ///
        private double _normalizedHigh;

        /// <summary>
        /// The desired normalized low from the sample data.
        /// </summary>
        ///
        private double _normalizedLow;

        /// <summary>
        /// Construct the object with a range of 1 and -1.
        /// </summary>
        ///
        public NormalizedField() : this(1, -1)
        {
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theNormalizedHigh">The normalized high.</param>
        /// <param name="theNormalizedLow">The normalized low.</param>
        public NormalizedField(double theNormalizedHigh,
                               double theNormalizedLow)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _normalizedHigh = theNormalizedHigh;
            _normalizedLow = theNormalizedLow;
            _actualHigh = Double.MinValue;
            _actualLow = Double.MaxValue;
            _action = NormalizationAction.Normalize;
        }

        /// <summary>
        /// Construct an object.
        /// </summary>
        ///
        /// <param name="theAction">The desired action.</param>
        /// <param name="theName">The name of this column.</param>
        public NormalizedField(NormalizationAction theAction,
                               String theName) : this(theAction, theName, 0, 0, 0, 0)
        {
        }

        /// <summary>
        /// Construct the field, with no defaults.
        /// </summary>
        ///
        /// <param name="theAction">The normalization action to take.</param>
        /// <param name="theName">The name of this field.</param>
        /// <param name="ahigh">The actual high.</param>
        /// <param name="alow">The actual low.</param>
        /// <param name="nhigh">The normalized high.</param>
        /// <param name="nlow">The normalized low.</param>
        public NormalizedField(NormalizationAction theAction,
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
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the field.</param>
        /// <param name="theAction">The action of the field.</param>
        /// <param name="high">The high end of the range for the field.</param>
        /// <param name="low">The low end of the range for the field.</param>
        public NormalizedField(String theName,
                               NormalizationAction theAction, double high,
                               double low)
        {
            _classes = new List<ClassItem>();
            _lookup = new Dictionary<String, Int32>();
            _name = theName;
            _action = theAction;
            _normalizedHigh = high;
            _normalizedLow = low;
        }

        /// <summary>
        /// Set the action for the field.
        /// </summary>
        public NormalizationAction Action
        {
            get { return _action; }
            set { _action = value; }
        }


        /// <summary>
        /// Set the actual high for the field.
        /// </summary>
        public double ActualHigh
        {
            get { return _actualHigh; }
            set { _actualHigh = value; }
        }


        /// <summary>
        /// Set the actual low for the field.
        /// </summary>
        public double ActualLow
        {
            get { return _actualLow; }
            set { _actualLow = value; }
        }


        /// <value>A list of any classes in this field.</value>
        public IList<ClassItem> Classes
        {
            get { return _classes; }
        }


        /// <value>Returns the number of columns needed for this classification. The
        /// number of columns needed will vary, depending on the
        /// classification method used.</value>
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


        /// <value>The equilateral object used by this class, null if none.</value>
        public Equilateral Eq
        {
            get { return _eq; }
        }


        /// <summary>
        /// Set the name of the field.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// Set the normalized high for the field.
        /// </summary>
        public double NormalizedHigh
        {
            get { return _normalizedHigh; }
            set { _normalizedHigh = value; }
        }


        /// <summary>
        /// Set the normalized low for the field.
        /// </summary>
        public double NormalizedLow
        {
            get { return _normalizedLow; }
            set { _normalizedLow = value; }
        }


        /// <value>Is this field a classify field.</value>
        public bool Classify
        {
            get
            {         
                return (_action == NormalizationAction.Equilateral)
                       || (_action == NormalizationAction.OneOf)
                       || (_action == NormalizationAction.SingleField);
            }
        }

        /// <summary>
        /// Analyze the specified value. Adjust min/max as needed. Usually used only
        /// internally.
        /// </summary>
        ///
        /// <param name="d">The value to analyze.</param>
        public void Analyze(double d)
        {
            _actualHigh = Math.Max(_actualHigh, d);
            _actualLow = Math.Min(_actualLow, d);
        }

        /// <summary>
        /// Denormalize the specified value.
        /// </summary>
        ///
        /// <param name="v">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double DeNormalize(double v)
        {
            double result = ((_actualLow - _actualHigh)*v
                             - _normalizedHigh*_actualLow + _actualHigh
                             *_normalizedLow)
                            /(_normalizedLow - _normalizedHigh);
            return result;
        }

        /// <summary>
        /// Determine what class the specified data belongs to.
        /// </summary>
        ///
        /// <param name="data">The data to analyze.</param>
        /// <returns>The class the data belongs to.</returns>
        public ClassItem DetermineClass(double[] data)
        {
            int resultIndex;

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
                    throw new QuantError("Unknown action: " + _action);
            }

            return _classes[resultIndex];
        }

        /// <summary>
        /// Encode the headers used by this field.
        /// </summary>
        ///
        /// <returns>A string containing a comma separated list with the headers.</returns>
        public String EncodeHeaders()
        {
            var line = new StringBuilder();
            switch (_action)
            {
                case NormalizationAction.SingleField:
                    BasicFile.AppendSeparator(line, CSVFormat.EgFormat);
                    line.Append('\"');
                    line.Append(_name);
                    line.Append('\"');
                    break;
                case NormalizationAction.Equilateral:
                    for (int i = 0; i < _classes.Count - 1; i++)
                    {
                        BasicFile.AppendSeparator(line, CSVFormat.EgFormat);
                        line.Append('\"');
                        line.Append(_name);
                        line.Append('-');
                        line.Append(i);
                        line.Append('\"');
                    }
                    break;
                case NormalizationAction.OneOf:
                    for (int i = 0; i < _classes.Count; i++)
                    {
                        BasicFile.AppendSeparator(line, CSVFormat.EgFormat);
                        line.Append('\"');
                        line.Append(_name);
                        line.Append('-');
                        line.Append(i);
                        line.Append('\"');
                    }
                    break;
                default:
                    return null;
            }
            return line.ToString();
        }

        /// <summary>
        /// Encode a single field.
        /// </summary>
        ///
        /// <param name="classNumber">The class number to encode.</param>
        /// <returns>The encoded columns.</returns>
        public String EncodeSingleField(int classNumber)
        {
            var result = new StringBuilder();
            result.Append(classNumber);
            return result.ToString();
        }

        /// <summary>
        /// Fix normalized fields that have a single value for the min/max. Separate
        /// them by 2 units.
        /// </summary>
        ///
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
        /// Init any internal structures.
        /// </summary>
        ///
        public void Init()
        {
            if (_action == NormalizationAction.Equilateral)
            {
                if (_classes.Count < Equilateral.MinEq)
                {
                    throw new QuantError("There must be at least three classes "
                                         + "to make use of equilateral normalization.");
                }

                _eq = new Equilateral(_classes.Count, _normalizedHigh,
                                     _normalizedLow);
            }

            // build lookup map
            foreach (ClassItem t in _classes)
            {
                _lookup[t.Name] = t.Index;
            }
        }


        /// <summary>
        /// Lookup the specified field.
        /// </summary>
        ///
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
        /// Make a field to hold a class.  Use a numeric range for class items.
        /// </summary>
        ///
        /// <param name="theAction">The action to take.</param>
        /// <param name="classFrom">The beginning class item.</param>
        /// <param name="classTo">The ending class item.</param>
        /// <param name="high">The output high value.</param>
        /// <param name="low">The output low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              int classFrom, int classTo, int high,
                              int low)
        {
            if ((theAction != NormalizationAction.Equilateral)
                && (theAction != NormalizationAction.OneOf)
                && (theAction != NormalizationAction.SingleField))
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
        /// Create a field that will be used to hold a class.
        /// </summary>
        ///
        /// <param name="theAction">The action for this field.</param>
        /// <param name="cls">The class items.</param>
        /// <param name="high">The output high value.</param>
        /// <param name="low">The output low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              String[] cls, double high, double low)
        {
            if ((theAction != NormalizationAction.Equilateral)
                && (theAction != NormalizationAction.OneOf)
                && (theAction != NormalizationAction.SingleField))
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
                _classes.Insert(i, new ClassItem(cls[i], i));
            }
        }

        /// <summary>
        /// Make this a pass-through field.
        /// </summary>
        ///
        public void MakePassThrough()
        {
            _normalizedHigh = 0;
            _normalizedLow = 0;
            _actualHigh = 0;
            _actualLow = 0;
            _action = NormalizationAction.PassThrough;
        }

        /// <summary>
        /// Normalize the specified value.
        /// </summary>
        /// <param name="v">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double Normalize(double v)
        {
            return ((v - _actualLow)/(_actualHigh - _actualLow))
                   *(_normalizedHigh - _normalizedLow)
                   + _normalizedLow;
        }

        /// <inheritdoc/>
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
    }
}
