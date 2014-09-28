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

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     This command is used to generate the binary EGB file from a CSV file. The
    ///     resulting file can be used for training.
    /// </summary>
    public class CmdCode : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String COMMAND_NAME = "CODE";

        /// <summary>
        ///     Construct this generate command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdCode(EncogAnalyst analyst)
            : base(analyst)
        {
        }


        /// <inheritdoc />
        public override String Name
        {
            get { return COMMAND_NAME; }
        }

        /// <inheritdoc />
        public override bool ExecuteCommand(string args)
        {
            // get filenames
            String targetID = Prop.GetPropertyString(
                ScriptProperties.CODE_CONFIG_TARGET_FILE);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get other options
            /*TargetLanguage targetLanguage = Prop.getPropertyTargetLanguage(
                    ScriptProperties.CODE_CONFIG_TARGET_LANGUAGE);
            boolean embedData = getProp().getPropertyBoolean(
                    ScriptProperties.CODE_CONFIG_EMBED_DATA);

            EncogLogging.log(EncogLogging.LEVEL_DEBUG, "Beginning code generation");
            EncogLogging.log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);
            EncogLogging.log(EncogLogging.LEVEL_DEBUG, "target language:" + targetLanguage.toString());
		
            EncogCodeGeneration code = new EncogCodeGeneration(targetLanguage);
            code.setEmbedData(embedData);
            code.generate(getAnalyst());		
            code.save(targetFile);*/
            return false;
        }
    }
}
