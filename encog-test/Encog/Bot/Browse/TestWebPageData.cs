// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Bot.Browse;
using NUnit.Framework;
using Encog.Bot.DataUnits;
using Encog.Parse.Tags;
using Encog.Bot.Browse.Range;

namespace encog_test.Encog.Bot.Browse
{
    [TestFixture]
    public class TestWebPageData
    {

        [Test]
        public void testSimple()
        {
            LoadWebPage load = new LoadWebPage(null);
            WebPage page = load.Load("a<b>b</b>c");
            Assert.AreEqual(5, page.Data.Count);

            TextDataUnit textDU;
            TagDataUnit tagDU;

            // Index 0 (text)
            textDU = (TextDataUnit)page.GetDataUnit(0);
            Assert.AreEqual("a", textDU.ToString());
            // Index 1 (tag)
            tagDU = (TagDataUnit)page.GetDataUnit(1);
            Assert.AreEqual("b", tagDU.Tag.Name);
            Assert.AreEqual("<b>", tagDU.Tag.ToString());
            Assert.AreEqual(Tag.Type.BEGIN, tagDU.Tag.TagType);
            // Index 2 (text)
            textDU = (TextDataUnit)page.GetDataUnit(2);
            Assert.AreEqual("b", textDU.ToString());
            // Index 3 (tag)
            tagDU = (TagDataUnit)page.GetDataUnit(3);
            Assert.AreEqual("b", tagDU.Tag.Name);
            Assert.AreEqual(Tag.Type.END, tagDU.Tag.TagType);
            // Index 4 (text)
            textDU = (TextDataUnit)page.GetDataUnit(4);
            Assert.AreEqual("c", textDU.ToString());
        }

        [Test]
        public void testLink()
        {
            LoadWebPage load = new LoadWebPage(null);
            WebPage page = load.Load("<a href=\"index.html\">Link <b>1</b></a>");
            Assert.AreEqual(1, page.Contents.Count);

            DocumentRange span = page.Contents[0];
            Assert.AreEqual(0, span.Begin);
            Assert.AreEqual(5, span.End);
            Assert.IsTrue(span is Link);
            Link link = (Link)span;
            Assert.AreEqual("index.html", link.Target.Original.ToString());
            Address address = link.Target;
            Assert.IsNotNull(address.ToString());

        }
    }
}
