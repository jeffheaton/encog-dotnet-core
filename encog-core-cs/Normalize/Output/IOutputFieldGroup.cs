using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// Output fields can be grouped together if they are calculated together.
    /// This interface defines how a field group works.
    /// </summary>
    public interface IOutputFieldGroup
    {
        /// <summary>
        /// Add an output field to the group.
        /// </summary>
        /// <param name="field">The field to add.</param>
        void AddField(OutputFieldGrouped field);

        /// <summary>
        /// All of the output fields in this group.
        /// </summary>
        ICollection<OutputFieldGrouped> GroupedFields { get; }

        /// <summary>
        /// Init the group for a new row.
        /// </summary>
        void RowInit();
    }
}
