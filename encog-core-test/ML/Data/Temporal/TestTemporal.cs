//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Temporal
{
    [TestClass]
    public class TestTemporal
    {
        [TestMethod]
        public void BasicTemporal()
        {
            var temporal = new TemporalMLDataSet(5, 1);
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, false, true));
            for (int i = 0; i < 10; i++)
            {
                TemporalPoint tp = temporal.CreatePoint(i);
                tp[0] = 1.0 + (i*3);
                tp[1] = 2.0 + (i*3);
                tp[2] = 3.0 + (i*3);
            }

            temporal.Generate();

            Assert.AreEqual(10, temporal.InputNeuronCount);
            Assert.AreEqual(1, temporal.OutputNeuronCount);
            Assert.AreEqual(10, temporal.CalculateActualSetSize());

            IEnumerator<IMLDataPair> itr = temporal.GetEnumerator();
            itr.MoveNext();
            // set 0
            IMLDataPair pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(1.0, pair.Input[0]);
            Assert.AreEqual(2.0, pair.Input[1]);
            Assert.AreEqual(4.0, pair.Input[2]);
            Assert.AreEqual(5.0, pair.Input[3]);
            Assert.AreEqual(7.0, pair.Input[4]);
            Assert.AreEqual(8.0, pair.Input[5]);
            Assert.AreEqual(10.0, pair.Input[6]);
            Assert.AreEqual(11.0, pair.Input[7]);
            Assert.AreEqual(13.0, pair.Input[8]);
            Assert.AreEqual(14.0, pair.Input[9]);
            Assert.AreEqual(18.0, pair.Ideal[0]);

            // set 1
            itr.MoveNext();
            pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(4.0, pair.Input[0]);
            Assert.AreEqual(5.0, pair.Input[1]);
            Assert.AreEqual(7.0, pair.Input[2]);
            Assert.AreEqual(8.0, pair.Input[3]);
            Assert.AreEqual(10.0, pair.Input[4]);
            Assert.AreEqual(11.0, pair.Input[5]);
            Assert.AreEqual(13.0, pair.Input[6]);
            Assert.AreEqual(14.0, pair.Input[7]);
            Assert.AreEqual(16.0, pair.Input[8]);
            Assert.AreEqual(17.0, pair.Input[9]);
            Assert.AreEqual(21.0, pair.Ideal[0]);

            // set 2
            itr.MoveNext();
            pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(7.0, pair.Input[0]);
            Assert.AreEqual(8.0, pair.Input[1]);
            Assert.AreEqual(10.0, pair.Input[2]);
            Assert.AreEqual(11.0, pair.Input[3]);
            Assert.AreEqual(13.0, pair.Input[4]);
            Assert.AreEqual(14.0, pair.Input[5]);
            Assert.AreEqual(16.0, pair.Input[6]);
            Assert.AreEqual(17.0, pair.Input[7]);
            Assert.AreEqual(19.0, pair.Input[8]);
            Assert.AreEqual(20.0, pair.Input[9]);
            Assert.AreEqual(24.0, pair.Ideal[0]);

            // set 3
            itr.MoveNext();
            pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(10.0, pair.Input[0]);
            Assert.AreEqual(11.0, pair.Input[1]);
            Assert.AreEqual(13.0, pair.Input[2]);
            Assert.AreEqual(14.0, pair.Input[3]);
            Assert.AreEqual(16.0, pair.Input[4]);
            Assert.AreEqual(17.0, pair.Input[5]);
            Assert.AreEqual(19.0, pair.Input[6]);
            Assert.AreEqual(20.0, pair.Input[7]);
            Assert.AreEqual(22.0, pair.Input[8]);
            Assert.AreEqual(23.0, pair.Input[9]);
            Assert.AreEqual(27.0, pair.Ideal[0]);
        }

        [TestMethod]
        public void HiLowTemporal()
        {
            var temporal = new TemporalMLDataSet(5, 1);
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, false, true));
            for (int i = 0; i < 10; i++)
            {
                TemporalPoint tp = temporal.CreatePoint(i);
                tp[0] = 1.0 + (i*3);
                tp[1] = 2.0 + (i*3);
                tp[2] = 3.0 + (i*3);
            }

            temporal.HighSequence = 8;
            temporal.LowSequence = 2;
            temporal.Generate();

            Assert.AreEqual(10, temporal.InputNeuronCount);
            Assert.AreEqual(1, temporal.OutputNeuronCount);
            Assert.AreEqual(7, temporal.CalculateActualSetSize());

            IEnumerator<IMLDataPair> itr = temporal.GetEnumerator();
            itr.MoveNext();
            // set 0
            IMLDataPair pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(7.0, pair.Input[0]);
            Assert.AreEqual(8.0, pair.Input[1]);
            Assert.AreEqual(10.0, pair.Input[2]);
            Assert.AreEqual(11.0, pair.Input[3]);
            Assert.AreEqual(13.0, pair.Input[4]);
            Assert.AreEqual(14.0, pair.Input[5]);
            Assert.AreEqual(16.0, pair.Input[6]);
            Assert.AreEqual(17.0, pair.Input[7]);
            Assert.AreEqual(19.0, pair.Input[8]);
            Assert.AreEqual(20.0, pair.Input[9]);
            Assert.AreEqual(24.0, pair.Ideal[0]);
        }

        [TestMethod]
        public void FormatTemporal()
        {
            var temporal = new TemporalMLDataSet(5, 1);
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.DeltaChange, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.PercentChange, true, false));
            temporal.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, false, true));
            for (int i = 0; i < 10; i++)
            {
                TemporalPoint tp = temporal.CreatePoint(i);
                tp[0] = 1.0 + (i*3);
                tp[1] = 2.0 + (i*3);
                tp[2] = 3.0 + (i*3);
            }

            temporal.Generate();

            IEnumerator<IMLDataPair> itr = temporal.GetEnumerator();
            itr.MoveNext();
            // set 0
            IMLDataPair pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(3.0, pair.Input[0]);
            Assert.AreEqual(1.5, pair.Input[1]);
            Assert.AreEqual(3.0, pair.Input[2]);
            Assert.AreEqual(0.6, pair.Input[3]);
            Assert.AreEqual(3.0, pair.Input[4]);
            Assert.AreEqual(0.375, pair.Input[5]);
            Assert.AreEqual(3.0, pair.Input[6]);
            Assert.AreEqual(0.25, Math.Round(pair.Input[7]*4.0)/4.0);
            Assert.AreEqual(3.0, pair.Input[8]);
            Assert.AreEqual(0.25, Math.Round(pair.Input[9]*4.0)/4.0);
            Assert.AreEqual(18.0, pair.Ideal[0]);
        }

        [TestMethod]
        public void ActivationTemporal()
        {
            var temporal = new TemporalMLDataSet(5, 1);
            temporal.AddDescription(new TemporalDataDescription(new ActivationTANH(), TemporalDataDescription.Type.Raw,
                                                                true, false));
            temporal.AddDescription(new TemporalDataDescription(new ActivationTANH(), TemporalDataDescription.Type.Raw,
                                                                true, false));
            temporal.AddDescription(new TemporalDataDescription(new ActivationTANH(), TemporalDataDescription.Type.Raw,
                                                                false, true));
            for (int i = 0; i < 10; i++)
            {
                TemporalPoint tp = temporal.CreatePoint(i);
                tp[0] = 1.0 + (i*3);
                tp[1] = 2.0 + (i*3);
                tp[2] = 3.0 + (i*3);
            }

            temporal.Generate();

            IEnumerator<IMLDataPair> itr = temporal.GetEnumerator();

            // set 0
            itr.MoveNext();
            IMLDataPair pair = itr.Current;
            Assert.AreEqual(10, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);
            Assert.AreEqual(0.75, Math.Round(pair.Input[0]*4.0)/4.0);
            Assert.AreEqual(1.0, Math.Round(pair.Input[1]*4.0)/4.0);
            Assert.AreEqual(1.0, Math.Round(pair.Input[2]*4.0)/4.0);
            Assert.AreEqual(1.0, Math.Round(pair.Input[3]*4.0)/4.0);
        }
    }
}
