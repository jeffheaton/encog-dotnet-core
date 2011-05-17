using System;
using Encog.App.Analyst.Script.Prop;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// The set command allows a script to override a property value.
    /// </summary>
    ///
    public class CmdSet : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "SET";

        /// <summary>
        /// Construct the set command with the analyst.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdSet(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override String Name
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return COMMAND_NAME; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed bool ExecuteCommand(String args)
        {
            int index = args.IndexOf('=');
            String dots = args.Substring(0, (index) - (0)).Trim();
            String value_ren = args.Substring(index + 1).Trim();

            PropertyEntry entry = PropertyConstraints.Instance
                .FindEntry(dots);

            if (entry == null)
            {
                throw new AnalystError("Unknown property: " + args.ToUpper());
            }

            // strip quotes
            if (value_ren[0] == '\"')
            {
                value_ren = value_ren.Substring(1);
            }
            if (value_ren.EndsWith("\""))
            {
                value_ren = value_ren.Substring(0, (value_ren.Length - 1) - (0));
            }

            String[] cols = dots.Split('.');
            String section = cols[0];
            String subSection = cols[1];
            String name = cols[2];

            entry.Validate(section, subSection, name, value_ren);
            Prop.SetProperty(entry.Key, value_ren);

            return false;
        }
    }
}