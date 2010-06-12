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
using encog_test.Neural.Networks;
using Encog.Neural.Data.XML;

namespace encog_test.Data.XML
{
    [TestFixture]
    public class TestXMLNeuralData
    {
        public const String FILENAME = "test.xml";

        private void GenerateXML()
        {
            TextWriter ps = new StreamWriter(TestXMLNeuralData.FILENAME);
            ps.WriteLine("<DataSet>");
            ps.WriteLine("<pair><input><value>0</value><value>0</value></input><ideal><value>0</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>1</value><value>0</value></input><ideal><value>1</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>0</value><value>1</value></input><ideal><value>1</value></ideal></pair>");
            ps.WriteLine("<pair><input><value>1</value><value>1</value></input><ideal><value>0</value></ideal></pair>");
            ps.WriteLine("</DataSet>");
            ps.Close();
        }

        [Test]
        public void XMLNeuralData()
        {
            GenerateXML();
            XMLNeuralDataSet set = new XMLNeuralDataSet(
                    TestXMLNeuralData.FILENAME,2,1,
                    "pair",
                    "input",
                    "ideal",
                    "value");

            Assert.IsTrue(set.InputSize == 2);
            Assert.IsTrue(set.IdealSize == 1);
            XOR.TestXORDataSet(set);

        }
    }
}
