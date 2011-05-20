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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Bot
{
    [TestClass]
    public class TestBotUtil
    {
        [TestMethod]
        public void TestLoadPage()
        {
            // test good web site
            string str = BotUtil.LoadPage(new Uri("http://www.httprecipes.com/"));
            Assert.IsTrue(str.IndexOf("Recipes") != -1);
            // test bad website
            try
            {
                BotUtil.LoadPage(new Uri("http://www.httprecipes.com/sdhfuishdfui"));
                Assert.IsFalse(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestExtractFromIndex()
        {
            const string html = "<b>first</b><b>second</b>";
            string str = BotUtil.ExtractFromIndex(html, "<b>", "</b>", 0, 0);
            Assert.AreEqual("first", str);
            str = BotUtil.ExtractFromIndex(html, "<b>", "</b>", 1, 0);
            Assert.AreEqual("second", str);
            str = BotUtil.ExtractFromIndex(html, "<b>", "</b>", 0, 2);
            Assert.AreEqual("second", str);

            str = BotUtil.ExtractFromIndex(html, "bad", "</b>", 0, 0);
            Assert.IsNull(str);
            str = BotUtil.ExtractFromIndex(html, "<b>", "bad", 0, 0);
            Assert.IsNull(str);
        }

        [TestMethod]
        public void TestExtract()
        {
            const string html = "<b>first</b><b>second</b>";
            string str = BotUtil.Extract(html, "<b>", "</b>", 0);
            Assert.AreEqual("first", str);
            str = BotUtil.Extract(html, "<b>", "</b>", 2);
            Assert.AreEqual("second", str);
            str = BotUtil.Extract(html, "bad", "</b>", 0);
            Assert.IsNull(str);
            str = BotUtil.Extract(html, "<b>", "bad", 0);
            Assert.IsNull(str);
        }
    }
}
