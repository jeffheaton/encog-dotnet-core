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
