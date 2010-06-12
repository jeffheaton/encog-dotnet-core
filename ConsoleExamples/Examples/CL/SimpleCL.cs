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
using ConsoleExamples.Examples;
using Encog.Util.CL;
using Encog.Util.CL.Kernels;

namespace Encog.Examples.CL
{
    public class SimpleCL: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(SimpleCL),
                    "opencl-simple",
                    "Simple OpenCL example.",
                    "Simple OpenCL example that does a vector add.");
                return info;
            }
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            try
            {
                Encog.Instance.InitCL();

                EncogCLDevice device = Encog.Instance.CL.Devices[0];
                KernelVectorAdd k = Encog.Instance.CL.Devices[0].Platform.VectorAdd;
                k.Compile();

                double[] a = { 1, 2, 3, 4 };
                double[] b = { 5, 6, 7, 8 };
                double[] c = k.Add(device, a, b);

                for (int i = 0; i < a.Length; i++)
                {
                    Console.WriteLine(a[i] + " + " + b[i] + " = " + c[i]);
                }
            }
            catch (EncogCLError ex)
            {
                Console.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
