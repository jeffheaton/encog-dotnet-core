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
using Encog.App.Analyst.Script.Prop;
using Encog.ML;
using Encog.ML.Bayesian;
using Encog.ML.Data.Buffer;
using Encog.ML.Factory;
using Encog.Persist;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     The Encog Analyst create command. This command is used to create a Machine
    ///     Learning method.
    /// </summary>
    public class CmdCreate : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "CREATE";

        /// <summary>
        ///     Construct the create command.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        public CmdCreate(EncogAnalyst theAnalyst) : base(theAnalyst)
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
            String trainingID = Prop.GetPropertyString(
                ScriptProperties.MlConfigTrainingFile);
            String resourceID = Prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);

            FileInfo trainingFile = Script.ResolveFilename(trainingID);
            FileInfo resourceFile = Script.ResolveFilename(resourceID);

            String type = Prop.GetPropertyString(
                ScriptProperties.MlConfigType);
            String arch = Prop.GetPropertyString(
                ScriptProperties.MlConfigArchitecture);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning create");
            EncogLogging.Log(EncogLogging.LevelDebug, "training file:"
                                                      + trainingID);
            EncogLogging.Log(EncogLogging.LevelDebug, "resource file:"
                                                      + resourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "type:" + type);
            EncogLogging.Log(EncogLogging.LevelDebug, "arch:" + arch);

            var egb = new EncogEGBFile(trainingFile.ToString());
            egb.Open();
            int input = egb.InputCount;
            int ideal = egb.IdealCount;
            egb.Close();

            var factory = new MLMethodFactory();
            IMLMethod obj = factory.Create(type, arch, input, ideal);

            if (obj is BayesianNetwork)
            {
                string query = Prop.GetPropertyString(ScriptProperties.MLConfigQuery);
                ((BayesianNetwork) obj).DefineClassificationStructure(query);
            }

            EncogDirectoryPersistence.SaveObject(resourceFile, obj);

            return false;
        }
    }
}
