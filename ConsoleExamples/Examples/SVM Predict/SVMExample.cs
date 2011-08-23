using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;

namespace Encog.Examples.SVM_Predict
{
    class SVMExample :IExample
    {

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SVMExample),
                    "SVM",
                    "Makes a small SVM random trainer.",
                    "Makes a SVM network and trains against random inputs." +
                    "\nYou can do SVM random [numberofInputs] [OutputSize]")
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

            if (String.Compare(app.Args[0], "random", true) == 0)
            {
                if (app.Args.Length > 1)
                {
                    SVM_Predict.CreateSVMNetWork.RandomTrainerMethod(Convert.ToInt16(app.Args[1]), Convert.ToInt16(app.Args[2]));

                }
                else
                {
                    Console.WriteLine(@"You didn't input enough args in your request, will default to 3000 inputs , and 50 prediction size");
                    SVM_Predict.CreateSVMNetWork.RandomTrainerMethod(3000, 50);
                    MakeAPause();
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
