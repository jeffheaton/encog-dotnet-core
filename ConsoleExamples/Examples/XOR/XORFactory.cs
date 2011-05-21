//
// Encog(tm) Console Examples v3.0 - .Net Version
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
using ConsoleExamples.Examples;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Factory;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Util.Simple;

namespace Encog.Examples.XOR
{
    /// <summary>
    /// This example shows how to use the Encog machine learning factory to 
    /// generate a number of machine learning methods and training techniques 
    /// to learn the XOR operator.
    /// </summary>
    public class XORFactory : IExample
    {
        public const string METHOD_FEEDFORWARD_A = "?:B->SIGMOID->4:B->SIGMOID->?";
        public const string METHOD_BIASLESS_A = "?->SIGMOID->4->SIGMOID->?";
        public const string METHOD_SVMC_A = "?->C->?";
        public const string METHOD_SVMR_A = "?->R->?";
        public const string METHOD_RBF_A = "?->gaussian(c=4)->?";
        public const string METHOD_PNNC_A = "?->C(kernel=gaussian)->?";
        public const string METHOD_PNNR_A = "?->R(kernel=gaussian)->?";

        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XORInput = {
                                                new double[2] {0.0, 0.0},
                                                new double[2] {1.0, 0.0},
                                                new double[2] {0.0, 1.0},
                                                new double[2] {1.0, 1.0}
                                            };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal = {
                                                new double[1] {0.0},
                                                new double[1] {1.0},
                                                new double[1] {1.0},
                                                new double[1] {0.0}
                                            };


        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (XORFactory),
                    "xor-factory",
                    "Use XOR with many different training and network types.",
                    "This example shows many neural network types and training methods used with XOR.");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length > 0)
            {
                Run(app.Args[0]);
            }
            else
            {
                Usage();
            }
        }

        #endregion

        /**
	 * Demonstrate a feedforward network with RPROP.
	 */

        public void xorRPROP()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeRPROP,
                "", 1);
        }

        /**
         * Demonstrate a feedforward biasless network with RPROP.
         */

        public void xorBiasless()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_BIASLESS_A,
                MLTrainFactory.TypeRPROP,
                "", 1);
        }

        /**
         * Demonstrate a feedforward network with backpropagation.
         */

        public void xorBackProp()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeBackprop,
                "", 1);
        }

        /**
         * Demonstrate a SVM-classify.
         */

        public void xorSVMClassify()
        {
            Process(
                MLMethodFactory.TypeSVM,
                METHOD_SVMC_A,
                MLTrainFactory.TypeSVM,
                "", 1);
        }

        /**
         * Demonstrate a SVM-regression.
         */

        public void xorSVMRegression()
        {
            Process(
                MLMethodFactory.TypeSVM,
                METHOD_SVMR_A,
                MLTrainFactory.TypeSVM,
                "", 1);
        }

        /**
         * Demonstrate a SVM-regression search.
         */

        public void xorSVMSearchRegression()
        {
            Process(
                MLMethodFactory.TypeSVM,
                METHOD_SVMR_A,
                MLTrainFactory.TypeSVMSearch,
                "", 1);
        }

        /**
         * Demonstrate a XOR annealing.
         */

        public void xorAnneal()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeAnneal,
                "", 1);
        }

        /**
         * Demonstrate a XOR genetic.
         */

        public void xorGenetic()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeGenetic,
                "", 1);
        }

        /**
         * Demonstrate a XOR LMA.
         */

        public void xorLMA()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeLma,
                "", 1);
        }

        /**
         * Demonstrate a XOR LMA.
         */

        public void xorManhattan()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeManhattan,
                "lr=0.0001", 1);
        }

        /**
         * Demonstrate a XOR SCG.
         */

        public void xorSCG()
        {
            Process(
                MLMethodFactory.TypeFeedforward,
                METHOD_FEEDFORWARD_A,
                MLTrainFactory.TypeSCG,
                "", 1);
        }

        /**
         * Demonstrate a XOR RBF.
         */

        public void xorRBF()
        {
            Process(
                MLMethodFactory.TypeRbfnetwork,
                METHOD_RBF_A,
                MLTrainFactory.TypeRPROP,
                "", 1);
        }

        /**
         * Demonstrate a XOR RBF.
         */

        public void xorSVD()
        {
            Process(
                MLMethodFactory.TypeRbfnetwork,
                METHOD_RBF_A,
                MLTrainFactory.TypeSvd,
                "", 1);
        }

        /**
         * Demonstrate a XOR RBF.
         */

        public void xorPNNC()
        {
            Process(
                MLMethodFactory.TypePNN,
                METHOD_PNNC_A,
                MLTrainFactory.TypePNN,
                "", 2);
        }

        /**
         * Demonstrate a XOR RBF.
         */

        public void xorPNNr()
        {
            Process(
                MLMethodFactory.TypePNN,
                METHOD_PNNR_A,
                MLTrainFactory.TypePNN,
                "", 1);
        }

        public void Process(String methodName, String methodArchitecture, String trainerName, String trainerArgs,
                            int outputNeurons)
        {
            // first, create the machine learning method
            var methodFactory = new MLMethodFactory();
            IMLMethod method = methodFactory.Create(methodName, methodArchitecture, 2, outputNeurons);

            // second, create the data set		
            IMLDataSet dataSet = new BasicMLDataSet(XORInput, XORIdeal);

            // third, create the trainer
            var trainFactory = new MLTrainFactory();
            IMLTrain train = trainFactory.Create(method, dataSet, trainerName, trainerArgs);
            // reset if improve is less than 1% over 5 cycles
            if (method is IMLResettable && !(train is ManhattanPropagation))
            {
                train.AddStrategy(new RequiredImprovementStrategy(50));
            }

            // fourth, train and evaluate.
            EncogUtility.TrainToError(train, 0.01);
            EncogUtility.Evaluate((IMLRegression) method, dataSet);

            // finally, write out what we did
            Console.WriteLine(@"Machine Learning Type: " + methodName);
            Console.WriteLine(@"Machine Learning Architecture: " + methodArchitecture);

            Console.WriteLine(@"Training Method: " + trainerName);
            Console.WriteLine(@"Training Args: " + trainerArgs);
        }

        /// <summary>
        /// Display usage information.
        /// </summary>
        public void Usage()
        {
            Console.WriteLine(
@"Usage:
XORFactory [mode]

Where mode is one of:

backprop - Feedforward biased with backpropagation
rprop - Feedforward biased with resilient propagation
biasless - Feedforward biasless with resilient propagation
svm-c - Support Vector Machine for classification
svm-r - Support Vector Machine for regression
svm-search-r - Support Vector Machine for classification, search training
anneal - Feedforward biased with annealing
genetic - Feedforward biased with genetic
lma - Feedforward biased with Levenberg Marquardt
manhattan - Feedforward biased with Manhattan Update Rule
scg - Feedforward biased with Scaled Conjugate Gradient
rbf - RBF Network with Resilient propagation
svd - RBF Network with SVD
pnn-c PNN for Classification
pnn-r PNN for Regression");

        }

        /// <summary>
        /// Run the program in the specific mode.
        /// </summary>
        /// <param name="mode">The mode to use.</param>
        public void Run(string mode)
        {
            if (string.Compare(mode, "backprop", true) == 0)
            {
                xorBackProp();
            }
            else if (string.Compare(mode, "rprop", true) == 0)
            {
                xorRPROP();
            }
            else if (string.Compare(mode, "biasless", true) == 0)
            {
                xorBiasless();
            }
            else if (string.Compare(mode, "svm-c", true) == 0)
            {
                xorSVMClassify();
            }
            else if (string.Compare(mode, "svm-r", true) == 0)
            {
                xorSVMRegression();
            }
            else if (string.Compare(mode, "svm-search-r", true) == 0)
            {
                xorSVMSearchRegression();
            }
            else if (string.Compare(mode, "anneal", true) == 0)
            {
                xorAnneal();
            }
            else if (string.Compare(mode, "genetic", true) == 0)
            {
                xorGenetic();
            }
            else if (string.Compare(mode, "lma", true) == 0)
            {
                xorLMA();
            }
            else if (string.Compare(mode, "manhattan", true) == 0)
            {
                xorManhattan();
            }
            else if (string.Compare(mode, "scg", true) == 0)
            {
                xorSCG();
            }
            else if (string.Compare(mode, "rbf", true) == 0)
            {
                xorRBF();
            }
            else if (string.Compare(mode, "svd", true) == 0)
            {
                xorSVD();
            }
            else if (string.Compare(mode, "pnn-c", true) == 0)
            {
                xorPNNC();
            }
            else if (string.Compare(mode, "pnn-r", true) == 0)
            {
                this.xorPNNr();
            }
            else
            {
                Usage();
            }
        }
    }
}
