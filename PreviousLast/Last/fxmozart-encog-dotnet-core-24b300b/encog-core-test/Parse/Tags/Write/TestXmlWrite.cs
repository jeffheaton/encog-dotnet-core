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

namespace Encog.Parse.Tags.Write
{
    [TestClass]
    public class TestXmlWrite
    {
        [TestMethod]
        public void TestWrite()
        {
            var ms = new MemoryStream();
            var write = new WriteXML(ms);
            write.BeginDocument();
            write.AddAttribute("name", "value");
            write.BeginTag("tag");
            write.EndTag();
            write.EndDocument();
            ms.Close();

            var enc = new ASCIIEncoding();
            string str = enc.GetString(ms.ToArray());

            Assert.AreEqual("<tag name=\"value\"></tag>", str);
        }
    }
}
