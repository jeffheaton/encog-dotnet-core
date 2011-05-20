using System;
using System.Collections.Generic;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// Analyst command that allows all properties to be reset to what they were
    /// originally loaded from the Encog EGA file.
    /// </summary>
    ///
    public class CmdReset : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "RESET";

        /// <summary>
        /// Construct the reset command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdReset(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return COMMAND_NAME; }
        }

        /// <inheritdoc/>
        public override sealed bool ExecuteCommand(String args)
        {
            IDictionary<String, String> revertedData = Analyst.RevertData;
            Script.Properties.PerformRevert(revertedData);
            return false;
        }
    }
}