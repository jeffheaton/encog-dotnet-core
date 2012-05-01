using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// An indicator packet.
    /// </summary>
    public class IndicatorPacket
    {
        /// <summary>
        /// The command.
        /// </summary>
        private string command;

        /// <summary>
        /// The arguments.
        /// </summary>
        private string[] args;
        
        /// <summary>
        /// Construct a packet from he list of arguments.
        /// </summary>
        /// <param name="list">The argument list.</param>
        public IndicatorPacket(IList<string> list)
        {
            this.command = list[0].ToUpper();

            if (list.Count == 1)
            {
                this.args = new String[0];
            }
            else
            {
                this.args = new String[list.Count - 1];
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                this.args[i] = list[i + 1];
            }
        }

        /// <summary>
        /// The command.
        /// </summary>
        public String Command
        {
            get
            {
                return command;
            }
        }

        /// <summary>
        /// The arguments.
        /// </summary>
        public String[] Args
        {
            get
            {
                return args;
            }
        }

    }
}
