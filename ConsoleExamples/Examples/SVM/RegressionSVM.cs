using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.SVM;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.SVM.Training;
using ConsoleExamples.Examples;

namespace Encog.Examples.SVM
{
    public class RegressionSVM : IExample
    {
        /// <summary>
        /// Input for function.
        /// </summary>
        public static double[][] RegressionInput = {
            new[] {0.0},
            new[] {0.1},
            new[] {0.2},
            new[] {0.3},
            new[] {0.4},
            new[] {0.5},
            new[] {0.6},
            new[] {0.7},
            new[] {0.8},
            new[] {0.9}
            };

        /// <summary>
        /// Ideal output.
        /// </summary>
        public static double[][] RegressionIdeal = {
            new[] {0.9},
            new[] {0.8},
            new[] {0.7},
            new[] {0.6},
            new[] {0.5},
            new[] {0.4},
            new[] {0.3},
            new[] {0.2},
            new[] {0.1},
            new[] {0.0}
        };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(RegressionSVM),
                    "svmreg",
                    "This example shows how to preform simple regression with a SVM.",
                    "SVM Regression.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            var svm = new SupportVectorMachine(1,true); // 1 input, & true for regression

            // create training data
            IMLDataSet trainingSet = new BasicMLDataSet(RegressionInput, RegressionIdeal);

            // train the SVM
            IMLTrain train = new SVMSearchTrain(svm, trainingSet);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01);

            // test the SVM
            Console.WriteLine(@"SVM Results:");
            foreach (IMLDataPair pair in trainingSet)
            {
                IMLData output = svm.Compute(pair.Input);
                Console.WriteLine(pair.Input[0] 
                                  + @", actual=" + output[0] + @",ideal=" + pair.Ideal[0]);
            }
        }
    }
}
