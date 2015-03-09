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
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Analyze
{
    /// <summary>
    ///     This class represents a field that the Encog Analyst is in the process of
    ///     analyzing. This class is used to track statistical information on the field
    ///     that will help the Encog analyst determine what type of field this is, and
    ///     how to normalize it.
    /// </summary>
    public class AnalyzedField : DataField
    {
        /// <summary>
        ///     A mapping between the class names that the class items.
        /// </summary>
        private readonly IDictionary<String, AnalystClassItem> _classMap;

        /// <summary>
        ///     The analyst script that the results are saved to.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     The total for standard deviation calculation.
        /// </summary>
        private double _devTotal;

        /// <summary>
        ///     The number of instances of this field.
        /// </summary>
        private int _instances;

        /// <summary>
        ///     Tge sum of all values of this field.
        /// </summary>
        private double _total;

        /// <summary>
        /// The numeric format to use.
        /// </summary>
        private CSVFormat _fmt;


        /// <summary>
        ///     Construct an analyzed field.
        /// </summary>
        /// <param name="theScript">The script being analyzed.</param>
        /// <param name="name">The name of the field.</param>
        public AnalyzedField(AnalystScript theScript, String name) : base(name)
        {
            _classMap = new Dictionary<String, AnalystClassItem>();
            _instances = 0;
            _script = theScript;
            _fmt = _script.DetermineFormat();
        }

        /// <summary>
        ///     Get the class members.
        /// </summary>
        public IList<AnalystClassItem> AnalyzedClassMembers
        {
            get
            {
                List<string> sorted = _classMap.Keys.ToList();

                sorted.Sort();

                return sorted.Select(str => _classMap[str]).ToList();
            }
        }

        /// <summary>
        ///     Perform a pass one analysis of this field.
        /// </summary>
        /// <param name="str">The current value.</param>
        public void Analyze1(String v)
        {
            bool accountedFor = false;
            string str = v.Trim();

            if (str.Trim().Length == 0 || str.Equals("?"))
            {
                Complete = false;
                return;
            }

            _instances++;

            if (Real)
            {
                try
                {
                    double d = _fmt.Parse(str);
                    Max = Math.Max(d, Max);
                    Min = Math.Min(d, Min);
                    _total += d;
                    accountedFor = true;
                }
                catch (FormatException)
                {
                    Real = false;
                    if (!Integer)
                    {
                        Max = 0;
                        Min = 0;
                        StandardDeviation = 0;
                    }
                }
            }

            if (Integer)
            {
                try
                {
                    int i = Int32.Parse(str);
                    Max = Math.Max(i, Max);
                    Min = Math.Min(i, Min);
                    if (!accountedFor)
                    {
                        _total += i;
                    }
                }
                catch (FormatException)
                {
                    Integer = false;
                    if (!Real)
                    {
                        Max = 0;
                        Min = 0;
                        StandardDeviation = 0;
                    }
                }
            }

            if (Class)
            {
                AnalystClassItem item;

                // is this a new class?
                if (!_classMap.ContainsKey(str))
                {
                    item = new AnalystClassItem(str, str, 1);
                    _classMap[str] = item;

                    // do we have too many different classes?
                    int max = _script.Properties.GetPropertyInt(
                        ScriptProperties.SetupConfigMaxClassCount);
                    if (_classMap.Count > max)
                    {
                        Class = false;
                    }
                }
                else
                {
                    item = _classMap[str];
                    item.IncreaseCount();
                }
            }
        }

        /// <summary>
        ///     Perform a pass two analysis of this field.
        /// </summary>
        /// <param name="str">The current value.</param>
        public void Analyze2(String str)
        {
            if (str.Trim().Length == 0)
            {
                return;
            }

            if (Real || Integer)
            {
                if (!str.Equals("") && !str.Equals("?"))
                {
                    double d = _fmt.Parse(str) - Mean;
                    _devTotal += d*d;
                }
            }
        }

        /// <summary>
        ///     Complete pass 1.
        /// </summary>
        public void CompletePass1()
        {
            _devTotal = 0;

            if (_instances == 0)
            {
                Mean = 0;
            }
            else
            {
                Mean = _total/_instances;
            }
        }

        /// <summary>
        ///     Complete pass 2.
        /// </summary>
        public void CompletePass2()
        {
            StandardDeviation = Math.Sqrt(_devTotal/_instances);
        }

        /// <summary>
        ///     Finalize the field, and create a DataField.
        /// </summary>
        /// <returns>The new DataField.</returns>
        public DataField FinalizeField()
        {
            var result = new DataField(Name)
                {
                    Name = Name,
                    Min = Min,
                    Max = Max,
                    Mean = Mean,
                    StandardDeviation = StandardDeviation,
                    Integer = Integer,
                    Real = Real,
                    Class = Class,
                    Complete = Complete
                };

            // if max and min are the same, we are dealing with a zero-sized range,
            // which will cause other issues.  This is caused by ever number in the
            // column having exactly (or nearly exactly) the same value.  Provide a
            // small range around that value so that every value in this column normalizes
            // to the midpoint of the desired normalization range, typically 0 or 0.5.
            if (Math.Abs(Max - Min) < EncogFramework.DefaultDoubleEqual)
            {
                result.Min = Min - 0.0001;
                result.Max = Min + 0.0001;
            }

            result.ClassMembers.Clear();

            if (result.Class)
            {
                IList<AnalystClassItem> list = AnalyzedClassMembers;
                foreach (AnalystClassItem item in list)
                {
                    result.ClassMembers.Add(item);
                }
            }

            return result;
        }


        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" total=");
            result.Append(_total);
            result.Append(", instances=");
            result.Append(_instances);
            result.Append("]");
            return result.ToString();
        }
    }
}
