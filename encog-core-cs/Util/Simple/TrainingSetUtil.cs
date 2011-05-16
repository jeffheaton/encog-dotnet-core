using System;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.CSV;

namespace Encog.Util.Simple
{
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
        public static MLDataSet LoadCSVTOMemory(CSVFormat format, String filename,
                                                bool headers, int inputSize, int idealSize)
        {
            MLDataSet result = new BasicMLDataSet();
            var csv = new ReadCSV(filename, headers, format);
            while (csv.Next())
            {
                MLData input = null;
                MLData ideal = null;
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

                MLDataPair pair = new BasicMLDataPair(input, ideal);
                result.Add(pair);
            }

            return result;
        }

        public static ObjectPair<double[][], double[][]> TrainingToArray(
            MLDataSet training)
        {
            var length = (int) training.Count;
            double[][] a = EngineArray.AllocateDouble2D(length, training.InputSize);
            double[][] b = EngineArray.AllocateDouble2D(length, training.IdealSize);

            int index = 0;

            foreach (MLDataPair pair  in  training)
            {
                EngineArray.ArrayCopy(pair.InputArray, a[index]);
                EngineArray.ArrayCopy(pair.IdealArray, b[index]);
                index++;
            }

            return new ObjectPair<double[][], double[][]>(a, b);
        }
    }
}