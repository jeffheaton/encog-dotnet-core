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

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     Analyst command that allows all properties to be reset to what they were
    ///     originally loaded from the Encog EGA file.
    /// </summary>
    public class CmdReset : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "RESET";

        /// <summary>
        ///     Construct the reset command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdReset(EncogAnalyst analyst) : base(analyst)
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
            IDictionary<String, String> revertedData = Analyst.RevertData;
            Script.Properties.PerformRevert(revertedData);
            return false;
        }
    }
}
