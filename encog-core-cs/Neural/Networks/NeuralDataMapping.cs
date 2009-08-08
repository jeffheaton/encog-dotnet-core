using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// A mapping between two INeuralData classes.
    /// </summary>
    public class NeuralDataMapping
    {
        /// <summary>
        /// The source.
        /// </summary>
        private INeuralData from;

        /// <summary>
        /// The target.
        /// </summary>
        private INeuralData to;

        /// <summary>
        /// Construct an empty mapping.
        /// </summary>
        public NeuralDataMapping()
        {
            this.from = this.to = null;
        }

        /// <summary>
        /// Construct a neural data mapping.
        /// </summary>
        /// <param name="from">The source.</param>
        /// <param name="to">The target.</param>
        public NeuralDataMapping(INeuralData from, INeuralData to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// The source.
        /// </summary>
        public INeuralData From
        {
            get
            {
                return from;
            }
            set
            {
                this.from = value;
            }
        }

        /// <summary>
        /// The target.
        /// </summary>
        public INeuralData To
        {
            get
            {
                return to;
            }
            set
            {
                this.to = value;
            }
        }

        /// <summary>
        /// Copy from one mapping to another.  Deep copy.
        /// </summary>
        /// <param name="source">The source mapping.</param>
        /// <param name="target">The target mapping.</param>
        public static void Copy(NeuralDataMapping source, NeuralDataMapping target)
        {
            for (int i = 0; i < source.From.Count; i++)
            {
                target.From[i] = source.From[i];
            }

            for (int i = 0; i < source.To.Count; i++)
            {
                target.To[i] = source.To[i];
            }
        }
    }

}
