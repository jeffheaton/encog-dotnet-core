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
