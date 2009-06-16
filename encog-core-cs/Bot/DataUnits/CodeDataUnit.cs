using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.DataUnits
{
    /// <summary>
    /// A data unit that holds code.
    /// </summary>
    public class CodeDataUnit : DataUnit
    {
        /// <summary>
        /// The code for this data unit.
        /// </summary>
        private String code;

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return this.code;
        }

        /// <summary>
        /// The code for this data unit.
        /// </summary>
        public String Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }
    }
}
