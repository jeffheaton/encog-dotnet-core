using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// Provides very basic functionality that other output field groups
    /// will use.  Mainly provides the list of fields that are grouped.
    /// </summary>
    public abstract class BasicOutputFieldGroup : IOutputFieldGroup
    {
        /// <summary>
        /// The fields in this group.
        /// </summary>
        [EGReference]
        private ICollection<OutputFieldGrouped> fields = new List<OutputFieldGrouped>();

        /// <summary>
        /// Add a field to this group.
        /// </summary>
        /// <param name="field">The field to add to the group.</param>
        public void AddField(OutputFieldGrouped field)
        {
            this.fields.Add(field);
        }

        /// <summary>
        /// The list of grouped fields.
        /// </summary>
        public ICollection<OutputFieldGrouped> GroupedFields
        {
            get
            {
                return this.fields;
            }
        }

        /// <summary>
        /// Init for a new row.
        /// </summary>
        public abstract void RowInit();
    }
}
