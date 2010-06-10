using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Bot.Browse;
using Encog.Bot.Browse.Range;

namespace encog_test.Encog.Bot.Browse
{
    [TestFixture]
    public class TestBrowse
    {

        [Test]
        public void testBrowse()
        {
            Browser b = new Browser();
            b.Navigate(new Uri("http://www.httprecipes.com"));
            WebPage page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
        }

        [Test]
        public void testFormGET()
        {
            Browser b = new Browser();
            b.Navigate(new Uri("http://www.httprecipes.com/1/7/get.php"));
            WebPage page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
            Form form = (Form)page.Find(typeof(Form), 0);
            Input input1 = form.FindType("text", 0);
            input1.Value = "New York";
            b.Navigate(form);
            page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
        }
    }
}
