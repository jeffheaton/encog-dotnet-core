// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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
using Encog.Util.Banchmark;
using ConsoleExamples.Examples;

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
