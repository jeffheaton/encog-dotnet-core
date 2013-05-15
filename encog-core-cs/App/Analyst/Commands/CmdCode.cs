using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst.Script.Prop;
using System.IO;
using Encog.App.Generate;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to generate the binary EGB file from a CSV file. The
    /// resulting file can be used for training.
    /// </summary>
    public class CmdCode : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        public const String COMMAND_NAME = "CODE";

        /// <summary>
        /// Construct this generate command. 
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdCode(EncogAnalyst analyst)
            : base(analyst)
        {
        }


        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override String Name
        {
            get
            {
                return CmdCode.COMMAND_NAME;
            }
        }
    }
}
