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
using System.IO;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.ML.Factory;
using Encog.Util.CSV;
using Encog.Util.Logging;
using Encog.Util.Simple;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     This command is used to generate the binary EGB file from a CSV file. The
    ///     resulting file can be used for training.
    /// </summary>
    public class CmdGenerate : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "GENERATE";

        /// <summary>
        ///     Construct this generate command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdGenerate(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc />
        public override String Name
        {
            get { return CommandName; }
        }

        /// <summary>
        ///     Determine the ideal fields.
        /// </summary>
        /// <param name="headerList">The headers.</param>
        /// <returns>The indexes of the ideal fields.</returns>
        private int[] DetermineIdealFields(CSVHeaders headerList)
        {
            int[] result;
            String type = Prop.GetPropertyString(
                ScriptProperties.MlConfigType);

            // is it non-supervised?
            if (type.Equals(MLMethodFactory.TypeSOM))
            {
                result = new int[0];
                return result;
            }

            IList<Int32> fields = new List<Int32>();

            for (int currentIndex = 0; currentIndex < headerList.Size(); currentIndex++)
            {
                String baseName = headerList.GetBaseHeader(currentIndex);
                int slice = headerList.GetSlice(currentIndex);
                AnalystField field = Analyst.Script
                                            .FindNormalizedField(baseName, slice);

                if (field != null && field.Output)
                {
                    fields.Add(currentIndex);
                }
            }

            // allocate result array
            result = new int[fields.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (fields[i]);
            }

            return result;
        }

        /// <summary>
        ///     Determine the input fields.
        /// </summary>
        /// <param name="headerList">The headers.</param>
        /// <returns>The indexes of the input fields.</returns>
        private int[] DetermineInputFields(CSVHeaders headerList)
        {
            IList<Int32> fields = new List<Int32>();

            for (int currentIndex = 0; currentIndex < headerList.Size(); currentIndex++)
            {
                String baseName = headerList.GetBaseHeader(currentIndex);
                int slice = headerList.GetSlice(currentIndex);
                AnalystField field = Analyst.Script
                                            .FindNormalizedField(baseName, slice);

                if (field != null && field.Input)
                {
                    fields.Add(currentIndex);
                }
            }

            // allocate result array
            var result = new int[fields.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (fields[i]);
            }

            return result;
        }

        /// <summary>
        /// </summary>
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                ScriptProperties.GenerateConfigSourceFile);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.GenerateConfigTargetFile);
            CSVFormat format = Analyst.Script.DetermineFormat();

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning generate");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // mark generated
            Script.MarkGenerated(targetID);

            // read file
            bool headers = Script.ExpectInputHeaders(sourceID);
            var headerList = new CSVHeaders(sourceFile, headers,
                                            format);

            int[] input = DetermineInputFields(headerList);
            int[] ideal = DetermineIdealFields(headerList);

            EncogUtility.ConvertCSV2Binary(sourceFile, format, targetFile, input,
                                           ideal, headers);
            return false;
        }
    }
}
