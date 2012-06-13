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