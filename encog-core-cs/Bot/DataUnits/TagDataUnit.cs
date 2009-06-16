using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags;

namespace Encog.Bot.DataUnits
{
    /// <summary>
    /// A data unit that holds a tag.
    /// </summary>
    public class TagDataUnit : DataUnit
    {

        /// <summary>
        /// The tag for this data unit.
        /// </summary>
        private Tag tag;

        /// <summary>
        /// The tag that this data unit is based on.
        /// </summary>
        public Tag Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }
    }

}
