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
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Parse.Tags.Read
{
    [TestClass]
    public class TestXMLRead
    {
        public const string XML = "<doc><a t1='text1'>a</a><b>b</b><c>c</c><d>d</d></doc>";

        [TestMethod]
        public void TestRead()
        {
            var enc = new ASCIIEncoding();
            var bis = new MemoryStream(enc.GetBytes(XML));
            var read = new ReadXML(bis);
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
