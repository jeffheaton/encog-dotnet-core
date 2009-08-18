// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.

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
