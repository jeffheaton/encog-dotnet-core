using System;
using Encog.MathUtil.LIBSVM;
using Encog.ML.Data;

namespace Encog.ML.SVM.Training
{
    /// <summary>
    /// Encode an Encog dataset as a SVM problem.
    /// </summary>
    ///
    public sealed class EncodeSVMProblem
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private EncodeSVMProblem()
        {
        }

        /// <summary>
        /// Encode the Encog dataset.
        /// </summary>
        ///
        /// <param name="training">The training data.</param>
        /// <param name="outputIndex"></param>
        /// <returns>The SVM problem.</returns>
        public static svm_problem Encode(MLDataSet training,
                                         int outputIndex)
        {
            try
            {
                var result = new svm_problem();

                result.l = (int) training.Count;

                result.y = new double[result.l];
                result.x = new svm_node[result.l][];
                for (int i = 0; i < result.l; i++)
                {
                    result.x[i] = new svm_node[training.InputSize];
                }

                int elementIndex = 0;


                foreach (MLDataPair pair  in  training)
                {
                    MLData input = pair.Input;
                    MLData output = pair.Ideal;
                    result.x[elementIndex] = new svm_node[input.Count];

                    for (int i = 0; i < input.Count; i++)
                    {
                        result.x[elementIndex][i] = new svm_node();
                        result.x[elementIndex][i].index = i + 1;
                        result.x[elementIndex][i].value_Renamed = input[i];
                    }

                    result.y[elementIndex] = output[outputIndex];

                    elementIndex++;
                }

                return result;
            }
            catch (OutOfMemoryException e)
            {
                throw new EncogError("SVM Model - Out of Memory");
            }
        }
    }
}