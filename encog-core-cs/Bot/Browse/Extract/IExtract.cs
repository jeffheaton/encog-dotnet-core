using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Extract
{
    /// <summary>
    /// Provides the basic interface that any extractor must support. An extractor is
    /// a class that is capable of extracting certain types of data from web data.
    /// For example, the ExtractWords extractor is used to extract all of the words
    /// from a web page.
    /// </summary>
    public interface IExtract
    {
        /// <summary>
        /// Add a listener for the extraction.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        void AddListener(IExtractListener listener);

        /// <summary>
        /// Extract data from the web page.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        void Extract(WebPage page);

        /// <summary>
        /// Extract from the web page and return the results as a list.
        /// </summary>
        /// <param name="page">The web page to extract from.</param>
        /// <returns>The results of the extraction as a List.</returns>
        IList<Object> ExtractList(WebPage page);

        /// <summary>
        /// A list of listeners registered with this object.
        /// </summary>
        ICollection<IExtractListener> Listeners{ get; }

        /// <summary>
        /// Remove the specified listener.
        /// </summary>
        /// <param name="listener">The listener to rmove.</param>
        void RemoveListener(IExtractListener listener);
    }
}
