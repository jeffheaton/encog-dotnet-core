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
