using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Bot.Browse;
using Encog.Bot.Browse.Extract;

namespace encog_test.Encog.Bot.Browse.Extract
{
    [TestFixture]
    public class TestExtract
    {
        [Test]
        public void testWordExtract()
        {
            Browser b = new Browser();
            b.Navigate(new Uri("http://www.httprecipes.com"));
            WebPage page = b.CurrentPage;
            ExtractWords extract = new ExtractWords();
            IList<Object> list = extract.ExtractList(page);
            Assert.IsTrue(list.Count > 5);
        }
    }
}
