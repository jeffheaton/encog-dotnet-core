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
using Encog.Util.File;

namespace Encog.App.Generate.Generators.NinjaScript
{
    /// <summary>
    ///     Generate NinjaScript.
    /// </summary>
    public class GenerateNinjaScript : AbstractTemplateGenerator
    {
        public override String TemplatePath
        {
            get { return "Encog.Resources.ninja.cs"; }
        }

        public override String NullArray
        {
            get { return "null"; }
        }


        private void AddCols()
        {
            var line = new StringBuilder();
            line.Append("public readonly string[] ENCOG_COLS = {");

            bool first = true;

            foreach (DataField df in Analyst.Script.Fields)
            {
                if (string.Compare(df.Name, "time", true) != 0 && string.Compare(df.Name, "prediction", true) != 0)
                {
                    if (!first)
                    {
                        line.Append(",");
                    }

                    line.Append("\"");
                    line.Append(df.Name);
                    line.Append("\"");
                    first = false;
                }
            }

            line.Append("};");
            AddLine(line.ToString());
        }

        private void ProcessMainBlock()
        {
            EncogAnalyst analyst = Analyst;

            String processID = analyst.Script.Properties.GetPropertyString(ScriptProperties.PROCESS_CONFIG_SOURCE_FILE);

            String methodID = analyst
                .Script
                .Properties
                .GetPropertyString(ScriptProperties.MlConfigMachineLearningFile);

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
            ;
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
            }

            IndentLevel = 2;
            AddLine("#region Encog Data");
            IndentIn();
            AddNameValue("public const string EXPORT_FILENAME", "\""
                                                                + FileUtil.ToStringLiteral(processFile) + "\"");
            AddCols();

            AddNameValue("private readonly int[] _contextTargetOffset",
                         contextTargetOffset);
            AddNameValue("private readonly int[] _contextTargetSize",
                         contextTargetSize);
            AddNameValue("private const bool _hasContext", hasContext
                                                               ? "true"
                                                               : "false");
            AddNameValue("private const int _inputCount", inputCount);
            AddNameValue("private readonly int[] _layerContextCount",
                         layerContextCount);
            AddNameValue("private readonly int[] _layerCounts", layerCounts);
            AddNameValue("private readonly int[] _layerFeedCounts",
                         layerFeedCounts);
            AddNameValue("private readonly int[] _layerIndex", layerIndex);
            AddNameValue("private readonly double[] _layerOutput", layerOutput);
            AddNameValue("private readonly double[] _layerSums", layerSums);
            AddNameValue("private const int _outputCount", outputCount);
            AddNameValue("private readonly int[] _weightIndex", weightIndex);
            AddNameValue("private readonly double[] _weights", weights);
            AddNameValue("private readonly int[] _activation", activation);
            AddNameValue("private readonly double[] _p", p);
            IndentOut();
            AddLine("#endregion");
            IndentLevel = 0;
        }

        private void ProcessCalc()
        {
            AnalystField firstOutputField = null;
            int barsNeeded = Math.Abs(Analyst.DetermineMinTimeSlice());

            IndentLevel = 2;
            AddLine("if( _inputCount>0 && CurrentBar>=" + barsNeeded + " )");
            AddLine("{");
            IndentIn();
            AddLine("double[] input = new double[_inputCount];");
            AddLine("double[] output = new double[_outputCount];");

            int idx = 0;
            foreach (AnalystField field in Analyst.Script.Normalize
                                                  .NormalizedFields)
            {
                if (field.Input)
                {
                    String str;
                    DataField df = Analyst.Script
                                          .FindDataField(field.Name);

                    switch (field.Action)
                    {
                        case NormalizationAction.PassThrough:
                            str = EngineArray.Replace(df.Source, "##", "" + (-field.TimeSlice));
                            AddLine("input[" + idx + "]=" + str + ";");
                            idx++;
                            break;
                        case NormalizationAction.Normalize:
                            str = EngineArray.Replace(df.Source, "##", "" + (-field.TimeSlice));
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

            if (firstOutputField != null)
            {
                AddLine("Compute(input,output);");
                AddLine("Output.Set(DeNorm(output[0]" + ","
                        + firstOutputField.NormalizedHigh + ","
                        + firstOutputField.NormalizedLow + ","
                        + firstOutputField.ActualHigh + ","
                        + firstOutputField.ActualLow + "));");
                IndentOut();
            }

            AddLine("}");
            IndentLevel = 2;
        }

        private void ProcessObtain()
        {
            IndentLevel = 3;
            AddLine("double[] result = new double[ENCOG_COLS.Length];");

            int idx = 0;
            foreach (DataField df in Analyst.Script.Fields)
            {
                if (string.Compare(df.Name, "time", true) != 0 && string.Compare(df.Name, "prediction", true) != 0)
                {
                    String str = EngineArray.Replace(df.Source, "##", "0");
                    AddLine("result[" + idx + "]=" + str + ";");
                    idx++;
                }
            }
            AddLine("return result;");
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
            IndentLevel = 0;
        }
    }
}
