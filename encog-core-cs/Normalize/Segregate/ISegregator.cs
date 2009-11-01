using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Segregate
{
    /// <summary>
    /// Segregators are used to exclude certain rows. You may want to exclude rows to
    /// create training and validation sets. You may also simply wish to exclude some
    /// rows because they do not apply to what you are currently training for.
    /// </summary>
    public interface ISegregator
    {
        /// <summary>
        /// The normalization object that is being used with this segregator.
        /// </summary>
        DataNormalization Owner { get; }

        /// <summary>
        /// Setup this object to use the specified normalization object.
        /// </summary>
        /// <param name="normalization">The normalization object to use.</param>
        void Init(DataNormalization normalization);

        /// <summary>
        /// Should this row be included, according to this segregator.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        bool ShouldInclude();
    }
}
