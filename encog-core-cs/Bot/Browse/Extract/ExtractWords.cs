using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Bot.DataUnits;
using Encog.Parse.Signal;
using Encog.Parse;

namespace Encog.Bot.Browse.Extract
{
    /// <summary>
    /// An extractor that is designed to extract all of the words from a web page.
    /// </summary>
    public class ExtractWords : BasicExtract
    {
        /// <summary>
        /// Used to parse the text found ont the web page.  This is how the
	    /// text is broken into words.
        /// </summary>
        private ParseText parse;

        /// <summary>
        /// Construct the extractor for words.
        /// </summary>
        public ExtractWords()
        {
            this.parse = new ParseText();
            this.parse.Load();
        }

        /// <summary>
        /// Extract words from the specified WebPage.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        public override void Extract(WebPage page)
        {

            foreach (DataUnit unit in page.Data)
            {
                if (unit is TextDataUnit)
                {
                    TextDataUnit text = (TextDataUnit)unit;
                    Signal signal = this.parse.ParseString(text.Text);
                    ICollection<Signal> list = signal.FindByType("word");
                    foreach (Signal word in list)
                    {
                        Distribute(word.ToString());
                    }
                }
            }
        }
    }
}
