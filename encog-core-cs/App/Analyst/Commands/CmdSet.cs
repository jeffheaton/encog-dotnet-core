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
        public const String CommandName = "SET";

        /// <summary>
        /// Construct the set command with the analyst.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdSet(EncogAnalyst analyst) : base(analyst)
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
            int index = args.IndexOf('=');
            String dots = args.Substring(0, (index) - (0)).Trim();
            String v = args.Substring(index + 1).Trim();

            PropertyEntry entry = PropertyConstraints.Instance
                .FindEntry(dots);

            if (entry == null)
            {
                throw new AnalystError("Unknown property: " + args.ToUpper());
            }

            // strip quotes
            if (v[0] == '\"')
            {
                v = v.Substring(1);
            }
            if (v.EndsWith("\""))
            {
                v = v.Substring(0, (v.Length - 1) - (0));
            }

            String[] cols = dots.Split('.');
            String section = cols[0];
            String subSection = cols[1];
            String name = cols[2];

            entry.Validate(section, subSection, name, v);
            Prop.SetProperty(entry.Key, v);

            return false;
        }
    }
}