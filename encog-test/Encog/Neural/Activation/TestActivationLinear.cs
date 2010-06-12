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
using Encog.Neural.Activation;
using Encog.Persist.Persistors;
using Encog;

namespace encog_test.Encog.Neural.Activation
{
    [TestFixture]
    public class TestActivationLinear
    {
        [Test]
        public void testLinear()
        {
            ActivationLinear activation = new ActivationLinear();
            Assert.IsTrue(activation.HasDerivative);

            ActivationLinear clone = (ActivationLinear)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 1, 2, 3 };

            activation.ActivationFunction(input);

            Assert.AreEqual(1.0, input[0], 0.1);
            Assert.AreEqual(2.0, input[1], 0.1);
            Assert.AreEqual(3.0, input[2], 0.1);

            
            // test derivative, should not throw an error
            activation.DerivativeFunction(input);
   

            // test name and description
            // names and descriptions are not stored for these
            activation.Name = "name";
            activation.Description = "name";
            Assert.AreEqual(null, activation.Name);
            Assert.AreEqual(null, activation.Description);
        }
    }
}
