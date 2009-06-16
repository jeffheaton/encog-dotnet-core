using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.DataUnits
{
    /// <summary>
    /// A data unit that holds text.
    /// </summary>
    public class TextDataUnit : DataUnit
    {
        /// <summary>
        /// The text for this data unit.
        /// </summary>
        private String text;

        /// <summary>
        /// The text for this data unit.
        /// </summary>
        public String Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return this.text;
        }
    }
}
