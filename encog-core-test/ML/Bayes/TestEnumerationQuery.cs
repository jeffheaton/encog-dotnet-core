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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Query.Enumeration;

namespace Encog.ML.Bayes
{
    [TestClass]
    public class TestEnumerationQuery
    {
        private void TestPercent(double d, int target)
        {
            if (((int)d) >= (target - 2) && ((int)d) <= (target + 2))
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void TestEnumeration1()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent b = network.CreateEvent("b");

            network.CreateDependency(a, b);
            network.FinalizeStructure();
            a.Table.AddLine(0.5, true); // P(A) = 0.5
            b.Table.AddLine(0.2, true, true); // p(b|a) = 0.2
            b.Table.AddLine(0.8, true, false);// p(b|~a) = 0.8		
            network.Validate();

            EnumerationQuery query = new EnumerationQuery(network);
            query.DefineEventType(a, EventType.Evidence);
            query.DefineEventType(b, EventType.Outcome);
            query.SetEventValue(b, true);
            query.SetEventValue(a, true);
            query.Execute();
            TestPercent(query.Probability, 20);
        }

        [TestMethod]
        public void TestEnumeration2()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent x1 = network.CreateEvent("x1");
            BayesianEvent x2 = network.CreateEvent("x2");
            BayesianEvent x3 = network.CreateEvent("x3");

            network.CreateDependency(a, x1, x2, x3);
            network.FinalizeStructure();

            a.Table.AddLine(0.5, true); // P(A) = 0.5
            x1.Table.AddLine(0.2, true, true); // p(x1|a) = 0.2
            x1.Table.AddLine(0.6, true, false);// p(x1|~a) = 0.6
            x2.Table.AddLine(0.2, true, true); // p(x2|a) = 0.2
            x2.Table.AddLine(0.6, true, false);// p(x2|~a) = 0.6
            x3.Table.AddLine(0.2, true, true); // p(x3|a) = 0.2
            x3.Table.AddLine(0.6, true, false);// p(x3|~a) = 0.6
            network.Validate();

            EnumerationQuery query = new EnumerationQuery(network);
            query.DefineEventType(x1, EventType.Evidence);
            query.DefineEventType(x2, EventType.Evidence);
            query.DefineEventType(x3, EventType.Evidence);
            query.DefineEventType(a, EventType.Outcome);
            query.SetEventValue(a, true);
            query.SetEventValue(x1, true);
            query.SetEventValue(x2, true);
            query.SetEventValue(x3, false);
            query.Execute();
            TestPercent(query.Probability, 18);
        }

        [TestMethod]
        public void TestEnumeration3()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent x1 = network.CreateEvent("x1");
            BayesianEvent x2 = network.CreateEvent("x2");
            BayesianEvent x3 = network.CreateEvent("x3");

            network.CreateDependency(a, x1, x2, x3);
            network.FinalizeStructure();

            a.Table.AddLine(0.5, true); // P(A) = 0.5
            x1.Table.AddLine(0.2, true, true); // p(x1|a) = 0.2
            x1.Table.AddLine(0.6, true, false);// p(x1|~a) = 0.6
            x2.Table.AddLine(0.2, true, true); // p(x2|a) = 0.2
            x2.Table.AddLine(0.6, true, false);// p(x2|~a) = 0.6
            x3.Table.AddLine(0.2, true, true); // p(x3|a) = 0.2
            x3.Table.AddLine(0.6, true, false);// p(x3|~a) = 0.6
            network.Validate();

            EnumerationQuery query = new EnumerationQuery(network);
            query.DefineEventType(x1, EventType.Evidence);
            query.DefineEventType(x3, EventType.Outcome);
            query.SetEventValue(x1, true);
            query.SetEventValue(x3, true);
            query.Execute();
            TestPercent(query.Probability, 50);
        }
    }
}
