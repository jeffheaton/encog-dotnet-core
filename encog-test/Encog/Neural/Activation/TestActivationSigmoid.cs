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
using Encog.Persist.Persistors;
using Encog.Engine.Network.Activation;

namespace encog_test.Encog.Neural.Activation
{
    [TestFixture]
    public class TestActivationSigmoid
    {


        [Test]
        public void testSigmoid()
        {
            ActivationSigmoid activation = new ActivationSigmoid();
            Assert.IsTrue(activation.HasDerivative());

            ActivationSigmoid clone = (ActivationSigmoid)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.0 };

            activation.ActivationFunction(input,0,1);

            Assert.AreEqual(0.5, input[0], 0.1);

            // test derivative, should throw an error
            input[0] = activation.DerivativeFunction(input[0]);
            Assert.AreEqual(0.25, input[0], 0.1);
        }
    }
}
