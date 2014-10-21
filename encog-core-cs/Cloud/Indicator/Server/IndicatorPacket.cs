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

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// An indicator packet.
    /// </summary>
    public class IndicatorPacket
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly string[] _args;

        /// <summary>
        /// The command.
        /// </summary>
        private readonly string _command;

        /// <summary>
        /// Construct a packet from he list of arguments.
        /// </summary>
        /// <param name="list">The argument list.</param>
        public IndicatorPacket(IList<string> list)
        {
            _command = list[0].ToUpper();

            _args = list.Count == 1 ? new String[0] : new String[list.Count - 1];

            for (int i = 0; i < list.Count - 1; i++)
            {
                _args[i] = list[i + 1];
            }
        }

        /// <summary>
        /// The command.
        /// </summary>
        public String Command
        {
            get { return _command; }
        }

        /// <summary>
        /// The arguments.
        /// </summary>
        public String[] Args
        {
            get { return _args; }
        }
    }
}
