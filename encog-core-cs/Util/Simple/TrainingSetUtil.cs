//
// Encog(tm) Core v3.0 - .Net Version
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.CSV;

namespace Encog.Util.Simple
{
    /// <summary>
    /// Provides some utilities for training sets.
    /// </summary>
    public class TrainingSetUtil
    {
        /// <summary>
        /// Load a CSV file into a memory dataset.  
        /// </summary>
        ///
        /// <param name="format">The CSV format to use.</param>
        /// <param name="filename">The filename to load.</param>
        /// <param name="headers">True if there is a header line.</param>
        /// <param name="inputSize">The input size.  Input always comes first in a file.</param>
        /// <param name="idealSize">The ideal size, 0 for unsupervised.</param>
        /// <returns>A NeuralDataSet that holds the contents of the CSV file.</returns>
        public static IMLDataSet LoadCSVTOMemory(CSVFormat format, String filename,
                                                bool headers, int inputSize, int idealSize)
        {
            IMLDataSet result = new BasicMLDataSet();
            var csv = new ReadCSV(filename, headers, format);
            while (csv.Next())
            {
                IMLData input = null;
                IMLData ideal = null;
                int index = 0;

                input = new BasicMLData(inputSize);
                for (int i = 0; i < inputSize; i++)
                {
                    double d = csv.GetDouble(index++);
                    input[i] = d;
                }

                if (idealSize > 0)
                {
                    ideal = new BasicMLData(idealSize);
                    for (int i_0 = 0; i_0 < idealSize; i_0++)
                    {
                        double d_1 = csv.GetDouble(index++);
                        ideal[i_0] = d_1;
                    }
                }

                IMLDataPair pair = new BasicMLDataPair(input, ideal);
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Convert a training set to an array.
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public static ObjectPair<double[][], double[][]> TrainingToArray(
            IMLDataSet training)
        {
            var length = (int) training.Count;
            double[][] a = EngineArray.AllocateDouble2D(length, training.InputSize);
            double[][] b = EngineArray.AllocateDouble2D(length, training.IdealSize);

            int index = 0;

            foreach (IMLDataPair pair  in  training)
            {
                EngineArray.ArrayCopy(pair.InputArray, a[index]);
                EngineArray.ArrayCopy(pair.IdealArray, b[index]);
                index++;
            }

            return new ObjectPair<double[][], double[][]>(a, b);
        }
    }
}
