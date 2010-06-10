using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.Parse.Tags.Write;

namespace encog_test.Encog.Parse.Tags.Write
{
    [TestFixture]
    public class TestXMLWrite
    {
        [Test]
        public void testXMLWrite()
        {
            MemoryStream ms = new MemoryStream();
            WriteXML write = new WriteXML(ms);
            write.BeginDocument();
            write.AddAttribute("name", "value");
            write.BeginTag("tag");
            write.EndTag();
            write.EndDocument();
            ms.Close();

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            String str = enc.GetString(ms.ToArray());

            Assert.AreEqual("<tag name=\"value\"></tag>", str);
        }
    }
}
