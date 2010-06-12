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
using Encog.Neural.Data.Basic;
using NUnit.Framework;
using encog_test.Neural.Networks;
using Encog.Util;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks;
using System.IO;

namespace encog_test.Util
{
    [TestFixture]
    class TestSerializeObject
    {
        [Test]
        public void testSerializeXOR()
        {
            BasicNeuralDataSet set = new BasicNeuralDataSet(XOR.XOR_INPUT,
                    XOR.XOR_IDEAL);
            SerializeObject.Save("encog1.ser", set);
            set = (BasicNeuralDataSet)SerializeObject.Load("encog1.ser");
            XOR.TestXORDataSet(set);
            File.Delete("encog1.ser");
        }

        [Test]
        public void testSerializeNetwork()
        {
            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();

            SerializeObject.Save("encog2.ser", net);
            net = (BasicNetwork)SerializeObject.Load("encog2.ser");
            Assert.AreEqual(3, net.Structure.Layers.Count);
            File.Delete("encog2.ser");
        }


        [Test]
        public void testSerializeNetwork2()
        {
            ElmanPattern pattern = new ElmanPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();

            SerializeObject.Save("encog3.ser", net);
            net = (BasicNetwork)SerializeObject.Load("encog3.ser");
            Assert.AreEqual(4, net.Structure.Layers.Count);
            File.Delete("encog3.ser");

        }
    }
}
