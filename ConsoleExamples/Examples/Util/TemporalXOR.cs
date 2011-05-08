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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NeuralData;

namespace Encog.Examples.Util
{
    public class TemporalXOR
    {
        /*
   * 1 xor 0 = 1
   * 0 xor 0 = 0
   * 0 xor 1 = 1
   * 1 xor 1 = 0
   */
        public double[] SEQUENCE = { 1.0,0.0,1.0,
		0.0,0.0,0.0,
		0.0,1.0,1.0,
		1.0,1.0,0.0 };

        private double[][] input;
        private double[][] ideal;

        public MLDataSet Generate(int count)
        {
            this.input = new double[count][];
            this.ideal = new double[count][];

            for (int i = 0; i < this.input.Length; i++)
            {
                this.input[i] = new double[1];
                this.ideal[i] = new double[1];
                this.input[i][0] = SEQUENCE[i % SEQUENCE.Length];
                this.ideal[i][0] = SEQUENCE[(i + 1) % SEQUENCE.Length];
            }

            return new BasicMLDataSet(this.input, this.ideal);
        }
    }
}
