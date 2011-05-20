//
// Encog(tm) Console Examples v3.0 - .Net Version
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Banchmark;
using ConsoleExamples.Examples;
using Encog.Engine;

namespace Encog.Examples.Benchmark
{
    public class EncogBenchmarkExample : IExample, IStatusReportable
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(EncogBenchmarkExample),
                    "benchmark",
                    "Perform an Encog benchmark.",
                    "Return a number to show how fast Encog executes on this machine.  The lower the number, the better.");
                return info;
            }
        }

        public void Report(int total, int current, String message)
        {
            Console.WriteLine(current + " of " + total + ":" + message);

        }

        public void Execute(IExampleInterface app)
        {
            EncogBenchmark mark = new EncogBenchmark(this);
            Console.WriteLine("Benchmark result: " + mark.Process());
        }
    }
}
