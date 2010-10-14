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
using Encog.Neural.Networks;
using Encog.Neural.Networks.Pattern;
using Encog.Persist;
using System.IO;
using Encog.Engine.Network.Activation;

namespace encog_test.Neural.Persist
{
    [TestFixture]
    public class TestPersist
    {
        private BasicNetwork getRBF()
        {
            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();
            return net;
        }

        private BasicNetwork getElman()
        {
            ElmanPattern pattern = new ElmanPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            pattern.ActivationFunction = new ActivationSigmoid();
            BasicNetwork net = pattern.Generate();
            return net;
        }

        [Test]
        public void testPersist()
        {
            File.Delete("encogtest.eg");
            EncogPersistedCollection encog =
                new EncogPersistedCollection("encogtest.eg", FileMode.OpenOrCreate);
            encog.Create();
            BasicNetwork net1 = getRBF();
            BasicNetwork net2 = getElman();
            encog.Add("rbf", net1);
            encog.Add("elman", net2);

            net1 = (BasicNetwork)encog.Find("rbf");
            net2 = (BasicNetwork)encog.Find("elman");

            Assert.IsNotNull(net1);
            Assert.IsNotNull(net2);
            File.Delete("encogtest.eg");

        }
    }
}
