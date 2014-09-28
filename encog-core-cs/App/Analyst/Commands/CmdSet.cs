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
using Encog.App.Analyst.Script.Prop;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     The set command allows a script to override a property value.
    /// </summary>
    public class CmdSet : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "SET";

        /// <summary>
        ///     Construct the set command with the analyst.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdSet(EncogAnalyst analyst) : base(analyst)
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
