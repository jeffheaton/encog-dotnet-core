using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.MathUtil.LIBSVM;

namespace Encog.Neural.Networks.Training.SVM
{
    /// <summary>
    /// Encode an Encog dataset as a SVM problem.
    /// </summary>
    public class EncodeSVMProblem
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private EncodeSVMProblem()
        {

        }

        /// <summary>
        /// Encode the Encog dataset.
        /// </summary>
        /// <param name="training">The training data.</param>
        /// <param name="outputIndex">The ideal element to use, this is necessary becase SVM's have
        /// only a single output.</param>
        /// <returns>The SVM problem.</returns>
        public static svm_problem Encode(MLDataSet training, int outputIndex)
        {
            svm_problem result = new svm_problem();

            result.l = (int)training.Count;

            result.y = new double[result.l];
            result.x = new svm_node[result.l][];

            int elementIndex = 0;

            foreach (MLDataPair pair in training)
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
    }
}
