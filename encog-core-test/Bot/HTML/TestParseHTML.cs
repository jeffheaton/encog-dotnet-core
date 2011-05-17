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

using System.IO;
using System.Text;
using Encog.Parse.Tags;
using Encog.Parse.Tags.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Bot.HTML
{
    [TestClass]
    public class TestParseHTML
    {
        [TestMethod]
        public void TestAttributeLess()
        {
            const string html = "12<b>12</b>1";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));

            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == '1');
            Assert.IsTrue(parse.Read() == '2');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.LastTag.Name.Equals("b"));
            Assert.IsTrue(parse.LastTag.TagType == Tag.Type.BEGIN);
            Assert.IsTrue(parse.Read() == '1');
            Assert.IsTrue(parse.Read() == '2');
            Assert.IsTrue(parse.Read() == 0);
            Tag tag = parse.LastTag;
            Assert.IsTrue(tag.Name.Equals("b"));
            Assert.IsTrue(tag.TagType == Tag.Type.END);
            Assert.AreEqual(tag.ToString(), "</b>");
            Assert.IsTrue(parse.Read() == '1');
        }

        [TestMethod]
        public void TestAttributes()
        {
            const string html = "<img src=\"picture.gif\" alt=\"A Picture\">";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
            Tag tag = parse.LastTag;
            Assert.IsNotNull(tag);
            Assert.IsTrue(tag.Name.Equals("img"));
            //TestCase.assertTrue(html.equals(tag.toString()));
            Assert.IsTrue(tag.GetAttributeValue("src").Equals("picture.gif"));
            Assert.IsTrue(tag.GetAttributeValue("alt").Equals("A Picture"));
        }

        [TestMethod]
        public void TestAttributesNoDelim()
        {
            const string html = "<img src=picture.gif alt=APicture>";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
            Tag tag = parse.LastTag;
            Assert.IsNotNull(tag);
            Assert.IsTrue(tag.Name.Equals("img"));
            Assert.IsTrue(tag.GetAttributeValue("src").Equals("picture.gif"));
            Assert.IsTrue(tag.GetAttributeValue("alt").Equals("APicture"));
        }

        [TestMethod]
        public void TestBoth()
        {
            const string html = "<br/>";
            const string htmlName = "br";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
            Tag tag = parse.LastTag;
            Assert.IsNotNull(tag);
            Assert.IsTrue(tag.TagType == Tag.Type.BEGIN);
            Assert.IsTrue(tag.Name.Equals(htmlName));
            parse.ReadToTag();
            tag = parse.LastTag;
            Assert.IsNotNull(tag);
            Assert.IsTrue(tag.TagType == Tag.Type.END);
            Assert.IsTrue(tag.Name.Equals(htmlName));
        }

        [TestMethod]
        public void TestBothWithAttributes()
        {
            const string html = "<img src=\"picture.gif\" alt=\"A Picture\"/>";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
        }

        [TestMethod]
        public void TestComment()
        {
            const string html = "a<!-- Hello -->b";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 'a');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.Read() == 'b');
        }

        [TestMethod]
        public void TestScript()
        {
            const string html = "a<script>12</script>b";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 'a');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.Read() == '1');
            Assert.IsTrue(parse.Read() == '2');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.Read() == 'b');
        }

        [TestMethod]
        public void TestScript2()
        {
            const string html = "a<script>1<2</script>b<br>";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 'a');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.Read() == '1');
            Assert.IsTrue(parse.Read() == '<');
            Assert.IsTrue(parse.Read() == '2');
            Assert.IsTrue(parse.Read() == 0);
            Assert.IsTrue(parse.Read() == 'b');
            Assert.IsTrue(parse.Read() == 0);
        }

        [TestMethod]
        public void TestToString()
        {     
            const  string html = "a<img src=\"picture.gif\" alt=\"A Picture\">b";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            parse.ReadToTag();
            Assert.IsTrue(parse.ToString().IndexOf("A Picture") != -1);
        }

        [TestMethod]
        public void TestTagToString()
        {
            const string html = "<br/>";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
        }

        [TestMethod]
        public void TestSpecialCharacter()
        {
            const string html = "&lt;&gt;&#65;";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == '<');
            Assert.IsTrue(parse.Read() == '>');
            Assert.IsTrue(parse.Read() == 'A');
        }

        [TestMethod]
        public void TestSimpleAttribute()
        {
            const string html = "<!DOCTYPE \"test\">";
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(html));
            var parse = new ReadHTML(bis);
            Assert.IsTrue(parse.Read() == 0);
            Tag tag = parse.LastTag;
            Assert.AreEqual(tag.ToString(), html);
        }
    }
}