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
using Encog.App.Analyst.CSV.Basic;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Process
{
    public class ProcessExtension
    {
        public const String EXTENSION_DATA_NAME = "ENCOG-ANALYST-PROCESS";

        // add field
        public static IProgramExtensionTemplate OPCODE_FIELD = new BasicTemplate(
            BasicTemplate.NoPrec, "field({s}{i}):{s}",
            NodeType.Function, true, 0,
            (actual) =>
                {
                    var pe = (ProcessExtension) actual.Owner.GetExtraData(EXTENSION_DATA_NAME);
                    string fieldName = actual.GetChildNode(0).Evaluate().ToStringValue();
                    int fieldIndex = (int) actual.GetChildNode(1).Evaluate().ToFloatValue()
                                     + pe.BackwardWindowSize;
                    String value = pe.GetField(fieldName, fieldIndex);
                    return new ExpressionValue(value);
                }, null, null);


        // add fieldmax
        public static IProgramExtensionTemplate OPCODE_FIELDMAX = new BasicTemplate(
            BasicTemplate.NoPrec, "fieldmax({s}{i}{i}):{f}",
            NodeType.Function, true, 0,
            (actual) =>
                {
                    var pe = (ProcessExtension) actual.Owner.GetExtraData(EXTENSION_DATA_NAME);
                    String fieldName = actual.GetChildNode(0).Evaluate().ToStringValue();
                    var startIndex = (int) actual.GetChildNode(1).Evaluate().ToIntValue();
                    var stopIndex = (int) actual.GetChildNode(2).Evaluate().ToIntValue();
                    double value = double.NegativeInfinity;

                    for (int i = startIndex; i <= stopIndex; i++)
                    {
                        String str = pe.GetField(fieldName, pe.BackwardWindowSize
                                                            + i);
                        double d = pe.format.Parse(str);
                        value = Math.Max(d, value);
                    }
                    return new ExpressionValue(value);
                }, null, null);

        // add fieldmaxpip
        public static IProgramExtensionTemplate OPCODE_FIELDMAXPIP = new BasicTemplate(
            BasicTemplate.NoPrec, "fieldmaxpip({s}{i}{i}):{f}",
            NodeType.Function, true, 0,
            (actual) =>
                {
                    var pe = (ProcessExtension) actual.Owner.GetExtraData(EXTENSION_DATA_NAME);
                    String fieldName = actual.GetChildNode(0).Evaluate()
                                             .ToStringValue();
                    var startIndex = (int) actual.GetChildNode(1).Evaluate()
                                                 .ToIntValue();
                    var stopIndex = (int) actual.GetChildNode(2).Evaluate()
                                                .ToIntValue();
                    int value = int.MinValue;

                    String str = pe.GetField(fieldName, pe.BackwardWindowSize);
                    double quoteNow = pe.Format.Parse(str);

                    for (int i = startIndex; i <= stopIndex; i++)
                    {
                        str = pe.GetField(fieldName, pe.BackwardWindowSize + i);
                        double d = pe.Format.Parse(str) - quoteNow;
                        d /= 0.0001;
                        d = Math.Round(d);
                        value = Math.Max((int) d, value);
                    }

                    return new ExpressionValue(value);
                }, null, null);

        private readonly IList<LoadedRow> data = new List<LoadedRow>();
        private readonly CSVFormat format;
        private readonly IDictionary<string, int> map = new Dictionary<string, int>();
        private int backwardWindowSize;
        private int forwardWindowSize;
        private int totalWindowSize;


        /**
         * Add opcodes to the Encog resource registry.
         */

        public ProcessExtension()
        {
            EncogOpcodeRegistry.Instance.Add(OPCODE_FIELD);
            EncogOpcodeRegistry.Instance.Add(OPCODE_FIELDMAX);
            EncogOpcodeRegistry.Instance.Add(OPCODE_FIELDMAXPIP);
        }

        public ProcessExtension(CSVFormat theFormat)
        {
            format = theFormat;
        }

        public int ForwardWindowSize
        {
            get { return forwardWindowSize; }
        }

        public int BackwardWindowSize
        {
            get { return backwardWindowSize; }
        }

        public int TotalWindowSize
        {
            get { return totalWindowSize; }
        }

        public CSVFormat Format
        {
            get { return format; }
        }

        public String GetField(String fieldName, int fieldIndex)
        {
            if (!map.ContainsKey(fieldName))
            {
                throw new AnalystError("Unknown input field: " + fieldName);
            }

            int idx = map[fieldName];

            if (fieldIndex >= data.Count || fieldIndex < 0)
            {
                throw new AnalystError(
                    "The specified temporal index "
                    + fieldIndex
                    + " is out of bounds.  You should probably increase the forward window size.");
            }

            return data[fieldIndex].Data[idx];
        }

        public void LoadRow(LoadedRow row)
        {
            data.Insert(0, row);
            if (data.Count > totalWindowSize)
            {
                data.RemoveAt(data.Count - 1);
            }
        }

        public void Init(ReadCSV csv, int theBackwardWindowSize,
                         int theForwardWindowSize)
        {
            forwardWindowSize = theForwardWindowSize;
            backwardWindowSize = theBackwardWindowSize;
            totalWindowSize = forwardWindowSize + backwardWindowSize
                              + 1;

            int i = 0;
            foreach (string name in csv.ColumnNames)
            {
                map[name] = i++;
            }
        }

        public bool IsDataReady()
        {
            return data.Count >= totalWindowSize;
        }

        public void register(FunctionFactory functions)
        {
            functions.AddExtension(OPCODE_FIELD);
            functions.AddExtension(OPCODE_FIELDMAX);
            functions.AddExtension(OPCODE_FIELDMAXPIP);
        }
    }
}
