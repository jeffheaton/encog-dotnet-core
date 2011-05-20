//
// Encog(tm) Unit Tests v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using Encog.Bot.Browse.Range;
using Encog.Bot.DataUnits;
using Encog.Parse.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Bot.Browse
{
    [TestClass]
    public class TestWebPageData
    {
        [TestMethod]
        public void TestSimple()
        {
            var load = new LoadWebPage(null);
            WebPage page = load.Load("a<b>b</b>c");
            Assert.AreEqual(5, page.Data.Count);

            // Index 0 (text)
            var textDu = (TextDataUnit) page.GetDataUnit(0);
            Assert.AreEqual("a", textDu.ToString());
            // Index 1 (tag)
            var tagDu = (TagDataUnit) page.GetDataUnit(1);
            Assert.AreEqual("b", tagDu.Tag.Name);
            Assert.AreEqual("<b>", tagDu.Tag.ToString());
            Assert.AreEqual(Tag.Type.BEGIN, tagDu.Tag.TagType);
            // Index 2 (text)
            textDu = (TextDataUnit) page.GetDataUnit(2);
            Assert.AreEqual("b", textDu.ToString());
            // Index 3 (tag)
            tagDu = (TagDataUnit) page.GetDataUnit(3);
            Assert.AreEqual("b", tagDu.Tag.Name);
            Assert.AreEqual(Tag.Type.END, tagDu.Tag.TagType);
            // Index 4 (text)
            textDu = (TextDataUnit) page.GetDataUnit(4);
            Assert.AreEqual("c", textDu.ToString());
        }

        [TestMethod]
        public void TestLink()
        {
            var load = new LoadWebPage(null);
            WebPage page = load.Load("<a href=\"index.html\">Link <b>1</b></a>");
            Assert.AreEqual(1, page.Contents.Count);

            DocumentRange span = page.Contents[0];
            Assert.AreEqual(0, span.Begin);
            Assert.AreEqual(5, span.End);
            Assert.IsTrue(span is Link);
            var link = (Link) span;
            Assert.AreEqual("index.html", link.Target.Original);
            Address address = link.Target;
            Assert.IsNotNull(address.ToString());
        }
    }
}
