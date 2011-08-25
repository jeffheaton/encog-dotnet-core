using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Encog.Examples.RangeandMarket;
using Encog.MathUtil.LIBSVM;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Factory;
using Encog.ML.SVM;
using Encog.ML.SVM.Training;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Util.Simple;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
namespace Encog.Examples.SVM_Predict
{
    class CreateSVMNetWork
    {

        private static SupportVectorMachine Create(IMLDataSet theset, int inputs)
        {
            IMLDataSet training = new BasicMLDataSet(theset);
            SupportVectorMachine result = new SupportVectorMachine(inputs, SVMType.EpsilonSupportVectorRegression, KernelType.Sigmoid);
            SVMTrain train = new SVMTrain(result, training);
            train.Iteration();
            return result;
        }

        /// <summary>
        /// Makes the inputs by randomizing with encog randomize , the normal random from net framework library.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double[] MakeInputs(int number)
        {
            Random rdn = new Random();
            Encog.MathUtil.Randomize.RangeRandomizer encogRnd = new Encog.MathUtil.Randomize.RangeRandomizer(-1, 1);
            double[] x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = encogRnd.Randomize((rdn.NextDouble()));

            }
            return x;
        }
        /// <summary>
        /// Trains a random trainer.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="predictWindow">The predict window.</param>
        public static void RandomTrainerMethod(int inputs, int predictWindow)
        {
            double[] firstinput = MakeInputs(inputs);
            double[] SecondInput = MakeInputs(inputs);
            double[] ThirdInputs = MakeInputs(inputs);
            double[] FourthInputs = MakeInputs(inputs);
            var pair = SuperUtils.ProcessPair(firstinput, FourthInputs, inputs, predictWindow);
            var pair2 = SuperUtils.ProcessPair(SecondInput, FourthInputs, inputs, predictWindow);
            var pair3 = SuperUtils.ProcessPair(ThirdInputs, FourthInputs, inputs, predictWindow);
            var pair4 = SuperUtils.ProcessPair(FourthInputs, FourthInputs, inputs, predictWindow);
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            SuperSet.Add(pair);
            SuperSet.Add(pair2);
            SuperSet.Add(pair3);
            SuperSet.Add(pair4);

            SupportVectorMachine machine = Create(SuperSet, inputs);
            SVMTrain train = new SVMTrain(machine, SuperSet);

            ///  var network = (BasicNetwork)CreateEval.CreateElmanNetwork(SuperSet.InputSize, SuperSet.IdealSize);
            //double error = CreateEval.TrainNetworks(machine, SuperSet);

            TrainSVM(train, machine);

            //Lets create an evaluation.
            // Console.WriteLine(@"Last error rate on random trainer:" + error);
        }


        private static BasicMLDataSet MakeAsets(int inputs , int predictWindow)
        {
             double[] firstinput = MakeInputs(inputs);
            double[] SecondInput = MakeInputs(inputs);
            double[] ThirdInputs = MakeInputs(inputs);
            double[] FourthInputs = MakeInputs(inputs);
            var pair = SuperUtils.ProcessPair(firstinput, FourthInputs, inputs, predictWindow);
            var pair2 = SuperUtils.ProcessPair(SecondInput, FourthInputs, inputs, predictWindow);
            var pair3 = SuperUtils.ProcessPair(ThirdInputs, FourthInputs, inputs, predictWindow);
            var pair4 = SuperUtils.ProcessPair(FourthInputs, FourthInputs, inputs, predictWindow);
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            SuperSet.Add(pair);
            SuperSet.Add(pair2);
            SuperSet.Add(pair3);
            SuperSet.Add(pair4);

            return SuperSet;
        }



        public static double TrainSVM(SVMTrain train, SupportVectorMachine machine)
        {
           
            StopTrainingStrategy stop = new StopTrainingStrategy(0.0001, 200);
            train.AddStrategy(stop);
            var sw = new Stopwatch();
            sw.Start();
            while (!stop.ShouldStop())
            {
                train.PreIteration();
                
                train.Iteration();
                train.PostIteration();
                Console.WriteLine(@"Iteration #:" + train.IterationNumber + @" Error:" + train.Error +" Gamma:"+train.Gamma);
            }
            sw.Stop();
            Console.WriteLine(@"SVM Trained in :" + sw.ElapsedMilliseconds);
            return train.Error;
        }


        public static void Process(String methodName, String methodArchitecture, String trainerName, String trainerArgs,
                            int outputNeurons)
        {
            // first, create the machine learning method
            var methodFactory = new MLMethodFactory();
            IMLMethod method = methodFactory.Create(methodName, methodArchitecture, 2, 1);
            // second, create the data set		
            IMLDataSet dataSet = MakeAsets(3000, outputNeurons);
            // third, create the trainer
            var trainFactory = new MLTrainFactory();
            IMLTrain train = trainFactory.Create(method, dataSet, trainerName, trainerArgs);
            // reset if improve is less than 1% over 5 cycles
            if (method is IMLResettable && !(train is ManhattanPropagation))
            {
                train.AddStrategy(new RequiredImprovementStrategy(500));
            }
            // fourth, train and evaluate.
            EncogUtility.TrainToError(train, 0.01);
            EncogUtility.Evaluate((IMLRegression)method, dataSet);

            // finally, write out what we did
            Console.WriteLine(@"Machine Learning Type: " + methodName);
            Console.WriteLine(@"Machine Learning Architecture: " + methodArchitecture);
            Console.WriteLine(@"Training Method: " + trainerName);
            Console.WriteLine(@"Training Args: " + trainerArgs);}
    }
}
