using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Extract
{
    /// <summary>
    /// The ExtractListener interface defines a class that can receive extraction
    /// events as an extraction process occurs.
    /// </summary>
    public interface IExtractListener
    {
        /// <summary>
        /// Notify that some data has been extracted.
        /// </summary>
        /// <param name="obj">The data that was extracted.</param>
        void FoundData(Object obj);
    }
}
