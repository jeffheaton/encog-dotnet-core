//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.Util.Data
{
    public sealed class GenerationUtil
    {
        public delegate double EncogFunction(double x);

        /// <summary>
        /// Private constructor.
        /// </summary>
        private GenerationUtil()
        {
        }

        public static IMLDataSet GenerateSingleDataRange(EncogFunction task, double start, double stop, double step)
        {
            BasicMLDataSet result = new BasicMLDataSet();
            double current = start;


            while (current <= stop)
            {
                BasicMLData input = new BasicMLData(1);
                input[0] = current;
                BasicMLData ideal = new BasicMLData(1);
                ideal[0] = task(current);
                result.Add(input, ideal);
                current += step;
            }

            return result;
        }
    }
}
