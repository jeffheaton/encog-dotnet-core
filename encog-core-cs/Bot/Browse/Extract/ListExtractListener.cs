using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse.Extract
{
    /// <summary>
    /// A simple implementation of the ExtractListener interface that will listen for
    /// words and add them to a list. This allows you to quickly build a list of all
    /// of the words on a web page.
    /// </summary>
    public class ListExtractListener : IExtractListener
    {
        /// <summary>
        /// The list to extract into.
        /// </summary>
        private IList<Object> list = new List<Object>();

        /// <summary>
        /// Called when a word is found, add it to the list.
        /// </summary>
        /// <param name="obj">The word found.</param>
        public void FoundData(Object obj)
        {
            this.list.Add(obj);
        }

        /// <summary>
        /// The list of words extracted.
        /// </summary>
        public IList<Object> List
        {
            get
            {
                return this.list;
            }
        }
    }
}
