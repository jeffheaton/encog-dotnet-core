//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;

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

        public double[] SEQUENCE = {
                                       1.0, 0.0, 1.0,
                                       0.0, 0.0, 0.0,
                                       0.0, 1.0, 1.0,
                                       1.0, 1.0, 0.0
                                   };

        private double[][] ideal;
        private double[][] input;

        public IMLDataSet Generate(int count)
        {
            input = new double[count][];
            ideal = new double[count][];

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new double[1];
                ideal[i] = new double[1];
                this.input[i][0] = this.SEQUENCE[i % this.SEQUENCE.Length];
                this.ideal[i][0] = this.SEQUENCE[(i + 1) % this.SEQUENCE.Length];
            }

            return new BasicMLDataSet(input, ideal);
        }
    }
}
