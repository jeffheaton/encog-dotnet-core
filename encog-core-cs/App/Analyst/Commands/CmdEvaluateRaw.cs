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
using Encog.App.Analyst.CSV;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.ML;
using Encog.Persist;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     This class is used to evaluate a machine learning method. Evaluation data is
    ///     provided and the ideal and actual responses from the machine learning method
    ///     are written to a file.
    /// </summary>
    public class CmdEvaluateRaw : Cmd
    {
        /// <summary>
        ///     The name of the command.
        /// </summary>
        public const String CommandName = "EVALUATE-RAW";

        /// <summary>
        ///     Construct an evaluate raw command.
        /// </summary>
        /// <param name="analyst">The analyst object to use.</param>
        public CmdEvaluateRaw(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc />
        public override String Name
        {
            get { return CommandName; }
        }

        /// <inheritdoc />
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String evalID = Prop.GetPropertyString(
                ScriptProperties.MlConfigEvalFile);
            String resourceID = Prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);

            String outputID = Prop.GetPropertyString(
                ScriptProperties.MlConfigOutputFile);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning evaluate raw");
            EncogLogging.Log(EncogLogging.LevelDebug, "evaluate file:" + evalID);
            EncogLogging.Log(EncogLogging.LevelDebug, "resource file:"
                                                      + resourceID);

            FileInfo evalFile = Script.ResolveFilename(evalID);
            FileInfo resourceFile = Script.ResolveFilename(resourceID);

            FileInfo outputFile = Analyst.Script.ResolveFilename(
                outputID);

            var m = (IMLMethod) EncogDirectoryPersistence.LoadObject(resourceFile);

            if (!(m is IMLRegression))
            {
                throw new AnalystError("The evaluate raw command can only be used with regression.");
            }

            var method = (IMLRegression) m;

            bool headers = Script.ExpectInputHeaders(evalID);

            var eval = new AnalystEvaluateRawCSV {Script = Script};
            Analyst.CurrentQuantTask = eval;
            eval.Report = new AnalystReportBridge(Analyst);
            eval.Analyze(Analyst, evalFile, headers, Prop
                                                         .GetPropertyCSVFormat(ScriptProperties.SetupConfigCSVFormat));
            eval.Process(outputFile, method);
            Analyst.CurrentQuantTask = null;
            return eval.ShouldStop();
        }
    }
}
