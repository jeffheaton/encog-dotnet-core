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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.ML.Factory;

namespace Encog.Examples.SVM_Predict
{
    class SVMExample :IExample
    {
        public const string METHOD_FEEDFORWARD_A = "?:B->SIGMOID->4:B->SIGMOID->?";
        public const string METHOD_BIASLESS_A = "?->SIGMOID->4->SIGMOID->?";
        public const string METHOD_SVMC_A = "?->C->?";
        public const string METHOD_SVMR_A = "?->R->?";
        public const string METHOD_RBF_A = "?->gaussian(c=4)->?";
        public const string METHOD_PNNC_A = "?->C(kernel=gaussian)->?";
        public const string METHOD_PNNR_A = "?->R(kernel=gaussian)->?";



        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SVMExample),
                    "SVM",
                    "Random Makes a small SVM random trainer.",
                    "Makes a SVM network and trains against random inputs." +
                    "\nYou can do SVM random [numberofInputs] [OutputSize]"+
                    "\n SVM [sunspot] analyzes the sunspots and predicts the sunspots"+
                    "SVM [svm] uses the super vector machines to predict random numbers")
                ;
                return info;
            }

        }

        #region IExample Members
        private IExampleInterface app;
        public void Execute(IExampleInterface app)
        {
            this.app = app;
            FileInfo dataDir = new FileInfo(Environment.CurrentDirectory);
            if (String.Compare(app.Args[0], "sun", true) == 0)
            {

                SVMPredictSunSpots.PredictSunspotSVM.run();
                MakeAPause();
                app.Exit();
            }
            if (String.Compare(app.Args[0], "svm", true) == 0)
            {
                
               CreateSVMNetWork.Process(
               MLMethodFactory.TypeSVM,
               METHOD_SVMR_A,
               MLTrainFactory.TypeSVM,
               "", 1);
            }
            if (String.Compare(app.Args[0], "random", true) == 0)
            {
                if (app.Args.Length > 1)
                {
                    SVM_Predict.CreateSVMNetWork.RandomTrainerMethod(Convert.ToInt16(app.Args[1]), Convert.ToInt16(app.Args[2]));
                    MakeAPause();
                    app.Exit();
                }
                else
                {
                    Console.WriteLine(@"You didn't input enough args in your request, will default to 3000 inputs , and 50 prediction size");
                    SVM_Predict.CreateSVMNetWork.RandomTrainerMethod(3000, 1);
                    MakeAPause();
                    app.Exit();
                }

            }
        }

        private static void MakeAPause()
        {
            Console.WriteLine("Press a key to continue ..");
            Console.ReadKey();
        }


        #endregion
    }
}
