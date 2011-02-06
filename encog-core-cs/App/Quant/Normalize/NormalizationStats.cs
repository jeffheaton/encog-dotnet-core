using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.App.Quant.Normalize
{
    /// <summary>
    /// Holds a group of normalized field stats.  This object can be persisted in an EG file.
    /// </summary>
    public class NormalizationStats : BasicPersistedObject
    {
        /// <summary>
        /// Get the normalization stats for the specified column.
        /// </summary>
        /// <param name="index">The index of the column to obtain the stats for.</param>
        /// <returns></returns>
        public NormalizedFieldStats this[int index]
        {
            get
            {
                return this.stats[index];
            }
            set
            {
                this.stats[index] = value;
            }
        }

        /// <summary>
        /// Get the number of columns.
        /// </summary>
        public int Count
        {
            get
            {
                return this.stats.Length;
            }
        }

        /// <summary>
        /// Access the normalized column data.
        /// </summary>
        public NormalizedFieldStats[] Data
        {
            get
            {
                return this.stats;
            }
            set
            {
                this.stats = value;
            }
        }

        /// <summary>
        /// The stats for each of the columns.
        /// </summary>
        private NormalizedFieldStats[] stats;

        /// <summary>
        /// Create a new object.
        /// </summary>
        /// <param name="count">The number of columns.</param>
        public NormalizationStats(int count)
        {
            this.stats = new NormalizedFieldStats[count];
        }

        /// <inheritdoc/>
        public override IPersistor CreatePersistor()
        {
            return new NormalizationStatsPersistor();
        }

        public void FixSingleValue()
        {
            foreach (NormalizedFieldStats stat in this.stats)
            {
                stat.FixSingleValue();
            }
        }
    }
}
