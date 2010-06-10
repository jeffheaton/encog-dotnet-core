using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.Parse.Tags.Read;

namespace encog_test.Encog.Parse.Tags.Read
{
    [TestFixture]
    public class TestXMLRead
    {
        public const String XML = "<doc><a t1='text1'>a</a><b>b</b><c>c</c><d>d</d></doc>";

        [Test]
        public void testRead()
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            MemoryStream bis = new MemoryStream(enc.GetBytes(XML));
            ReadXML read = new ReadXML(bis);
            Assert.AreEqual(0, read.Read());
            Assert.IsTrue(read.IsIt("doc", true));
            Assert.AreEqual(0, read.Read());
            Assert.IsTrue(read.IsIt("a", true));
            Assert.AreEqual('a', read.Read());
            Assert.AreEqual(0, read.Read());
            Assert.IsTrue(read.IsIt("a", false));
            bis.Close();
        }
    }
}
