using System;
using System.IO;
using Encog.App.Analyst.Script.Prop;
using Encog.ML;
using Encog.ML.Data.Buffer;
using Encog.ML.Factory;
using Encog.Persist;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// The Encog Analyst create command. This command is used to create a Machine
    /// Learning method.
    /// </summary>
    ///
    public class CmdCreate : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String CommandName = "CREATE";

        /// <summary>
        /// Construct the create command.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        public CmdCreate(EncogAnalyst theAnalyst) : base(theAnalyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return CommandName; }
        }

        /// <inheritdoc/>
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

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning create");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "training file:"
                                                       + trainingID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "resource file:"
                                                       + resourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "type:" + type);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "arch:" + arch);

            var egb = new EncogEGBFile(trainingFile.ToString());
            egb.Open();
            int input = egb.InputCount;
            int ideal = egb.IdealCount;
            egb.Close();

            var factory = new MLMethodFactory();
            MLMethod obj = factory.Create(type, arch, input, ideal);

            EncogDirectoryPersistence.SaveObject(resourceFile, obj);

            return false;
        }
    }
}