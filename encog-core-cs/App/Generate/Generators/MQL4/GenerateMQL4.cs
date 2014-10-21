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
using System.IO;
using System.Text;
using Encog.App.Analyst;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.ML;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util;
using Encog.Util.Arrayutil;

namespace Encog.App.Generate.Generators.MQL4
{
    /// <summary>
    ///     Generate MQL4.
    /// </summary>
    public class GenerateMQL4 : AbstractTemplateGenerator
    {
        public override String NullArray
        {
            get { return "{-1}"; }
        }

        public override String TemplatePath
        {
            get { return "Encog.Resources.mt4.mql4"; }
        }

        private void ProcessCalc()
        {
            AnalystField firstOutputField = null;
            int barsNeeded = Math.Abs(Analyst.DetermineMinTimeSlice());

            int inputCount = Analyst.DetermineInputCount();
            int outputCount = Analyst.DetermineOutputCount();

            IndentLevel = 2;
            AddLine("if( _inputCount>0 && Bars>=" + barsNeeded + " )");
            AddLine("{");
            IndentIn();
            AddLine("double input[" + inputCount + "];");
            AddLine("double output[" + outputCount + "];");

            int idx = 0;
            foreach (AnalystField field in Analyst.Script.Normalize
                                                  .NormalizedFields)
            {
                if (field.Input)
                {
                    DataField df = Analyst.Script.FindDataField(field.Name);
                    String str;

                    switch (field.Action)
                    {
                        case NormalizationAction.PassThrough:
                            str = EngineArray.Replace(df.Source, "##", "pos+"
                                                                       + (-field.TimeSlice));
                            AddLine("input[" + idx + "]=" + str + ";");
                            idx++;
                            break;
                        case NormalizationAction.Normalize:
                            str = EngineArray.Replace(df.Source, "##", "pos+"
                                                                       + (-field.TimeSlice));
                            AddLine("input[" + idx + "]=Norm(" + str + ","
                                    + field.NormalizedHigh + ","
                                    + field.NormalizedLow + ","
                                    + field.ActualHigh + ","
                                    + field.ActualLow + ");");
                            idx++;
                            break;
                        case NormalizationAction.Ignore:
                            break;
                        default:
                            throw new AnalystCodeGenerationError(
                                "Can't generate Ninjascript code, unsupported normalizatoin action: "
                                + field.Action.ToString());
                    }
                }
                if (field.Output)
                {
                    if (firstOutputField == null)
                    {
                        firstOutputField = field;
                    }
                }
            }

            if (firstOutputField == null)
            {
                throw new AnalystCodeGenerationError(
                    "Could not find an output field.");
            }

            AddLine("Compute(input,output);");

            AddLine("ExtMapBuffer1[pos] = DeNorm(output[0]" + ","
                    + firstOutputField.NormalizedHigh + ","
                    + firstOutputField.NormalizedLow + ","
                    + firstOutputField.ActualHigh + ","
                    + firstOutputField.ActualLow + ");");
            IndentOut();
            AddLine("}");
            IndentLevel = 2;
        }

        private void ProcessHeaders()
        {
            DataField[] fields = Analyst.Script.Fields;

            var line = new StringBuilder();
            line.Append("FileWrite(iHandle");

            foreach (DataField df in fields)
            {
                line.Append(",");
                line.Append("\"");
                line.Append(df.Name);
                line.Append("\"");
            }

            line.Append(");");
            AddLine(line.ToString());
        }

        private void ProcessMainBlock()
        {
            EncogAnalyst analyst = Analyst;

            String processID = analyst.Script.Properties
                                      .GetPropertyString(ScriptProperties.PROCESS_CONFIG_SOURCE_FILE);

            String methodID = analyst
                .Script
                .Properties
                .GetPropertyString(
                    ScriptProperties.MlConfigMachineLearningFile);

            FileInfo methodFile = analyst.Script.ResolveFilename(methodID);

            FileInfo processFile = analyst.Script.ResolveFilename(processID);

            IMLMethod method = null;
            int[] contextTargetOffset = null;
            int[] contextTargetSize = null;
            bool hasContext = false;
            int inputCount = 0;
            int[] layerContextCount = null;
            int[] layerCounts = null;
            int[] layerFeedCounts = null;
            int[] layerIndex = null;
            double[] layerOutput = null;
            double[] layerSums = null;
            int outputCount = 0;
            int[] weightIndex = null;
            double[] weights = null;
            int neuronCount = 0;
            int layerCount = 0;
            int[] activation = null;
            double[] p = null;

            if (methodFile.Exists)
            {
                method = (IMLMethod) EncogDirectoryPersistence
                                         .LoadObject(methodFile);
                FlatNetwork flat = ((BasicNetwork) method).Flat;

                contextTargetOffset = flat.ContextTargetOffset;
                contextTargetSize = flat.ContextTargetSize;
                hasContext = flat.HasContext;
                inputCount = flat.InputCount;
                layerContextCount = flat.LayerContextCount;
                layerCounts = flat.LayerCounts;
                layerFeedCounts = flat.LayerFeedCounts;
                layerIndex = flat.LayerIndex;
                layerOutput = flat.LayerOutput;
                layerSums = flat.LayerSums;
                outputCount = flat.OutputCount;
                weightIndex = flat.WeightIndex;
                weights = flat.Weights;
                activation = CreateActivations(flat);
                p = CreateParams(flat);
                neuronCount = flat.LayerOutput.Length;
                layerCount = flat.LayerCounts.Length;
            }

            IndentLevel = 2;
            IndentIn();
            AddNameValue("string EXPORT_FILENAME", "\"" + processFile
                                                   + "\"");

            AddNameValue("int _neuronCount", neuronCount);
            AddNameValue("int _layerCount", layerCount);
            AddNameValue("int _contextTargetOffset[]", contextTargetOffset);
            AddNameValue("int _contextTargetSize[]", contextTargetSize);
            AddNameValue("bool _hasContext", hasContext ? "true" : "false");
            AddNameValue("int _inputCount", inputCount);
            AddNameValue("int _layerContextCount[]", layerContextCount);
            AddNameValue("int _layerCounts[]", layerCounts);
            AddNameValue("int _layerFeedCounts[]", layerFeedCounts);
            AddNameValue("int _layerIndex[]", layerIndex);
            AddNameValue("double _layerOutput[]", layerOutput);
            AddNameValue("double _layerSums[]", layerSums);
            AddNameValue("int _outputCount", outputCount);
            AddNameValue("int _weightIndex[]", weightIndex);
            AddNameValue("double _weights[]", weights);
            AddNameValue("int _activation[]", activation);
            AddNameValue("double _p[]", p);

            IndentOut();
            IndentLevel = 0;
        }

        private void ProcessObtain()
        {
            IndentLevel = 3;

            AddLine("FileWrite(iHandle, when,");

            DataField[] fields = Analyst.Script.Fields;
            String lastLine = null;
            foreach (DataField field in fields)
            {
                DataField df = field;
                if (string.Compare(df.Name, "time", true) != 0
                    && string.Compare(df.Name, "prediction", true) != 0)
                {
                    String str = EngineArray.Replace(df.Source, "##",
                                                     "pos");
                    if (lastLine != null)
                    {
                        AddLine(lastLine + ",");
                    }
                    lastLine = str;
                }
            }

            if (lastLine != null)
            {
                AddLine(lastLine);
            }
            AddLine(");");
            IndentLevel = 0;
        }

        public override void ProcessToken(String command)
        {
            if (string.Compare(command, "MAIN-BLOCK", true) == 0)
            {
                ProcessMainBlock();
            }
            else if (command.Equals("CALC"))
            {
                ProcessCalc();
            }
            else if (command.Equals("OBTAIN"))
            {
                ProcessObtain();
            }
            else if (command.Equals("HEADERS"))
            {
                ProcessHeaders();
            }
            IndentLevel = 0;
        }
    }
}
