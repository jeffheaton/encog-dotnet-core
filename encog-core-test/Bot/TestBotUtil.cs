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