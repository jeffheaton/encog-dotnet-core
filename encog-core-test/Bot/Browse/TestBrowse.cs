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
using Encog.Bot.Browse.Range;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Bot.Browse
{
    [TestClass]
    public class TestBrowse
    {
        [TestMethod]
        public void TestBrowser()
        {
            var b = new Browser();
            b.Navigate(new Uri("http://www.httprecipes.com"));
            WebPage page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
        }

        [TestMethod]
        public void TestFormGET()
        {
            var b = new Browser();
            b.Navigate(new Uri("http://www.httprecipes.com/1/7/get.php"));
            WebPage page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
            var form = (Form) page.Find(typeof (Form), 0);
            Input input1 = form.FindType("text", 0);
            input1.Value = "New York";
            b.Navigate(form);
            page = b.CurrentPage;
            Assert.IsTrue(page.Title.GetTextOnly().IndexOf("HTTP") != -1);
        }
    }
}
