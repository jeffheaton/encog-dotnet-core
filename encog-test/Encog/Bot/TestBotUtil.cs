using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Logging;
using Encog.Bot;
using NUnit.Framework;

namespace encog_test.Encog.Bot
{
    [TestFixture]
    public class TestBotUtil
    {
        [Test]
        public void testLoadPage()
        {
            Logging.StopConsoleLogging();
            // test good web site
            String str = BotUtil.LoadPage(new Uri("http://www.httprecipes.com/"));
            Assert.IsTrue(str.IndexOf("Recipes") != -1);
            // test bad website
            try
            {
                str = BotUtil.LoadPage(new Uri("http://www.httprecipes.com/sdhfuishdfui"));
                Assert.IsFalse(true);
            }
            catch (Exception )
            {

            }
        }

        [Test]
        public void testExtractFromIndex()
        {
            Logging.StopConsoleLogging();
            String html = "<b>first</b><b>second</b>";
            String str = BotUtil.ExtractFromIndex(html, "<b>", "</b>", 0, 0);
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

        [Test]
        public void testExtract()
        {
            Logging.StopConsoleLogging();
            String html = "<b>first</b><b>second</b>";
            String str = BotUtil.Extract(html, "<b>", "</b>", 0);
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
