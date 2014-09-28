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
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Analyze
{
    /// <summary>
    ///     This class is used to perform an analysis of a CSV file. This will help Encog
    ///     to determine how the fields should be normalized.
    /// </summary>
    public class PerformAnalysis
    {
        /// <summary>
        ///     The file name to analyze.
        /// </summary>
        private readonly String _filename;

        /// <summary>
        ///     The format of this file.
        /// </summary>
        private readonly AnalystFileFormat _format;

        /// <summary>
        ///     True, if headers are present.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        ///     The script to use.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     The fields to analyze.
        /// </summary>
        private AnalyzedField[] _fields;

        /// <summary>
        ///     Construct the analysis object.
        /// </summary>
        /// <param name="theScript">The script to use.</param>
        /// <param name="theFilename">The name of the file to analyze.</param>
        /// <param name="theHeaders">True if headers are present.</param>
        /// <param name="theFormat">The format of the file being analyzed.</param>
        public PerformAnalysis(AnalystScript theScript,
                               String theFilename, bool theHeaders,
                               AnalystFileFormat theFormat)
        {
            _filename = theFilename;
            _headers = theHeaders;
            _format = theFormat;
            _script = theScript;
        }

        /// <summary>
        ///     Generate the header fields.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFields(ReadCSV csv)
        {
            if (_headers)
            {
                GenerateFieldsFromHeaders(csv);
            }
            else
            {
                GenerateFieldsFromCount(csv);
            }
        }

        /// <summary>
        ///     Generate the fields using counts, no headers provided.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFieldsFromCount(ReadCSV csv)
        {
            _fields = new AnalyzedField[csv.ColumnCount];
            for (int i = 0; i < _fields.Length; i++)
            {
                _fields[i] = new AnalyzedField(_script, "field:" + (i + 1));
            }
        }

        /// <summary>
        ///     Generate the fields using header values.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
        private void GenerateFieldsFromHeaders(ReadCSV csv)
        {
            var h = new CSVHeaders(csv.ColumnNames);
            _fields = new AnalyzedField[csv.ColumnCount];
            for (int i = 0; i < _fields.Length; i++)
            {
                if (i >= csv.ColumnCount)
                {
                    throw new AnalystError(
                        "CSV header count does not match column count");
                }
                _fields[i] = new AnalyzedField(_script, h.GetHeader(i));
            }
        }

        /// <summary>
        ///     Perform the analysis.
        /// </summary>
        /// <param name="target">The Encog analyst object to analyze.</param>
        public void Process(EncogAnalyst target)
        {
            int count = 0;
            CSVFormat csvFormat = ConvertStringConst
                .ConvertToCSVFormat(_format);
            var csv = new ReadCSV(_filename, _headers, csvFormat);

            // pass one, calculate the min/max
            while (csv.Next())
            {
                if (_fields == null)
                {
                    GenerateFields(csv);
                }

                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    if (_fields != null)
                    {
                        _fields[i].Analyze1(csv.Get(i));
                    }
                }
                count++;
            }

            if (count == 0)
            {
                throw new AnalystError("Can't analyze file, it is empty.");
            }

            if (_fields != null)
            {
                foreach (AnalyzedField field in _fields)
                {
                    field.CompletePass1();
                }
            }

            csv.Close();

            // pass two, standard deviation
            csv = new ReadCSV(_filename, _headers, csvFormat);
           
            while (csv.Next())
            {
                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    if (_fields != null)
                    {
                        _fields[i].Analyze2(csv.Get(i));
                    }
                }
            }


            if (_fields != null)
            {
                foreach (AnalyzedField field in _fields)
                {
                    field.CompletePass2();
                }
            }

            csv.Close();

            String str = _script.Properties.GetPropertyString(
                ScriptProperties.SetupConfigAllowedClasses) ?? "";

            bool allowInt = str.Contains("int");
            bool allowReal = str.Contains("real")
                             || str.Contains("double");
            bool allowString = str.Contains("string");


            // remove any classes that did not qualify
            foreach (AnalyzedField field  in  _fields)
            {
                if (field.Class)
                {
                    if (!allowInt && field.Integer)
                    {
                        field.Class = false;
                    }

                    if (!allowString && (!field.Integer && !field.Real))
                    {
                        field.Class = false;
                    }

                    if (!allowReal && field.Real && !field.Integer)
                    {
                        field.Class = false;
                    }
                }
            }

            // merge with existing
            if ((target.Script.Fields != null)
                && (_fields.Length == target.Script.Fields.Length))
            {
                for (int i = 0; i < _fields.Length; i++)
                {
                    // copy the old field name
                    _fields[i].Name = target.Script.Fields[i].Name;

                    if (_fields[i].Class)
                    {
                        IList<AnalystClassItem> t = _fields[i].AnalyzedClassMembers;
                        IList<AnalystClassItem> s = target.Script.Fields[i].ClassMembers;

                        if (s.Count == t.Count)
                        {
                            for (int j = 0; j < s.Count; j++)
                            {
                                if (t[j].Code.Equals(s[j].Code))
                                {
                                    t[j].Name = s[j].Name;
                                }
                            }
                        }
                    }
                }
            }

            // now copy the fields
            var df = new DataField[_fields.Length];

            for (int i_4 = 0; i_4 < df.Length; i_4++)
            {
                df[i_4] = _fields[i_4].FinalizeField();
            }

            target.Script.Fields = df;
        }

        /// <summary>
        /// </summary>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" filename=");
            result.Append(_filename);
            result.Append(", headers=");
            result.Append(_headers);
            result.Append("]");
            return result.ToString();
        }
    }
}
