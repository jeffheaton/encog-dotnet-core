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
using Encog.App.Analyst.Script.Normalize;
using Encog.Util;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    ///     A utility used to breat data into time-series lead and lag.
    /// </summary>
    public class TimeSeriesUtil
    {
        /// <summary>
        ///     The analyst to use.
        /// </summary>
        private readonly EncogAnalyst _analyst;

        /// <summary>
        ///     The buffer to hold the time-series data.
        /// </summary>
        private readonly IList<double[]> _buffer;

        /// <summary>
        ///     The heading map.
        /// </summary>
        private readonly IDictionary<String, Int32> _headingMap;

        /// <summary>
        ///     The input size.
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        ///     The lag depth.
        /// </summary>
        private readonly int _lagDepth;

        /// <summary>
        ///     The lead depth.
        /// </summary>
        private readonly int _leadDepth;

        /// <summary>
        ///     The output size.
        /// </summary>
        private readonly int _outputSize;

        /// <summary>
        ///     The total depth.
        /// </summary>
        private readonly int _totalDepth;

        /// <summary>
        ///     Construct the time-series utility.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="includeOutput">Should output fields be included.</param>
        /// <param name="headings">The column headings.</param>
        public TimeSeriesUtil(EncogAnalyst theAnalyst, bool includeOutput,
                              IEnumerable<string> headings)
        {
            _buffer = new List<double[]>();
            _headingMap = new Dictionary<String, Int32>();
            _analyst = theAnalyst;
            _lagDepth = _analyst.LagDepth;
            _leadDepth = _analyst.LeadDepth;
            _totalDepth = _lagDepth + _leadDepth + 1;
            _inputSize = includeOutput ? _analyst.DetermineTotalColumns() : _analyst.DetermineTotalInputFieldCount();
            _outputSize = _analyst.DetermineInputCount()
                          + _analyst.DetermineOutputCount();

            int headingIndex = 0;

            foreach (String column  in  headings)
            {
                _headingMap[column.ToUpper()] = headingIndex++;
            }
        }


        /// <value>the analyst</value>
        public EncogAnalyst Analyst
        {
            get { return _analyst; }
        }


        /// <value>the buffer</value>
        public IList<double[]> Buffer
        {
            get { return _buffer; }
        }


        /// <value>the headingMap</value>
        public IDictionary<String, Int32> HeadingMap
        {
            get { return _headingMap; }
        }


        /// <value>the inputSize</value>
        public int InputSize
        {
            get { return _inputSize; }
        }


        /// <value>the lagDepth</value>
        public int LagDepth
        {
            get { return _lagDepth; }
        }


        /// <value>the leadDepth</value>
        public int LeadDepth
        {
            get { return _leadDepth; }
        }


        /// <value>the outputSize</value>
        public int OutputSize
        {
            get { return _outputSize; }
        }


        /// <value>the totalDepth</value>
        public int TotalDepth
        {
            get { return _totalDepth; }
        }


        /// <summary>
        ///     Process a row.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public double[] Process(double[] input)
        {
            if (input.Length != _inputSize)
            {
                throw new AnalystError("Invalid input size: " + input.Length
                                       + ", should be " + _inputSize);
            }

            _buffer.Insert(0, EngineArray.ArrayCopy(input));

            // are we ready yet?
            if (_buffer.Count < _totalDepth)
            {
                return null;
            }

            // create output
            var output = new double[_outputSize];

            int outputIndex = 0;

            foreach (AnalystField field  in  _analyst.Script.Normalize.NormalizedFields)
            {
                if (!field.Ignored)
                {
                    if (!_headingMap.ContainsKey(field.Name.ToUpper()))
                    {
                        throw new AnalystError("Undefined field: "
                                               + field.Name);
                    }
                    int headingIndex = _headingMap[field.Name.ToUpper()];
                    int timeslice = TranslateTimeSlice(field.TimeSlice);
                    double[] row = _buffer[timeslice];
                    double d = row[headingIndex];
                    output[outputIndex++] = d;
                }
            }

            // keep the buffer at a good size
            while (_buffer.Count > _totalDepth)
            {
                _buffer.RemoveAt(_buffer.Count - 1);
            }

            return output;
        }

        /// <summary>
        ///     Translate a timeslice from a pos/neg number to a displacement
        ///     into the buffer.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The translated displacement.</returns>
        private int TranslateTimeSlice(int index)
        {
            return Math.Abs(index - _leadDepth);
        }
    }
}
