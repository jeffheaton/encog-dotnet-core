using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.App.Quant.Normalize
{
    public class NormalizationStats : BasicPersistedObject
    {
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

        public int Count
        {
            get
            {
                return this.stats.Length;
            }
        }

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

        private NormalizedFieldStats[] stats;

        public NormalizationStats(int count)
        {
            this.stats = new NormalizedFieldStats[count];
        }

        /// <inheritdoc/>
        public override IPersistor CreatePersistor()
        {
            return new NormalizationStatsPersistor();
        }
    }
}
