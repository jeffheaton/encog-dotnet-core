using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Encog.Engine.Network.Activation;
using Encog.Fuzzy;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.NetworkUtil;
using Encog.Util.Simple;
using ErrorViewerForm;
using Point = Encog.Fuzzy.Core.Point;

namespace Encog.Examples.Analyzer
{
    public static class CreateEval
    {


        private static double Angle;

        #region create an evaluation set from a file and train it
// ReSharper disable UnusedMember.Local
        private static void CreateEvaluationSet()
// ReSharper restore UnusedMember.Local
        {


            string file = "DB!EURUSD.Bar.Time.600.csv";
            List<double> Opens = QuickCSVUtils.QuickParseCSV(file, "Open", 1200, 1200);
            List<double> High = QuickCSVUtils.QuickParseCSV(file, "High", 1200, 1200);
            List<double> Low = QuickCSVUtils.QuickParseCSV(file, "Low", 1200, 1200);
            List<double> Close = QuickCSVUtils.QuickParseCSV(file, "Close", 1200, 1200);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(file, 5, 1200, 1200);
         

            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());

            TemporalMLDataSet superTemportal = TrainerHelper.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            IMLDataPair aPairInput = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), 100, 1);
            IMLDataPair aPairInput3 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput2 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), 100, 1);
            IMLDataPair aPairInput4 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), 100, 1);
            IMLDataPair aPairInput5 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), 100, 1);
            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);
            listData.Add((aPairInput5));


            var minitrainning = new BasicMLDataSet(listData);

            var network = (BasicNetwork)CreateElmanNetwork(100, 1);
            double normalCorrectRate = EvaluateNetworks(network, minitrainning);

            double temporalErrorRate = EvaluateNetworks(network, superTemportal);

            Console.WriteLine(@"Percent Correct with normal Data Set:" + normalCorrectRate + @" Percent Correct with temporal Dataset:" +
                      temporalErrorRate);





            Console.WriteLine(@"Paused , Press a key to continue to evaluation");
            Console.ReadKey();
        }
        public static double EvaluateNetworks(BasicNetwork network, BasicMLDataSet set)
        {
            int count = 0;
            int correct = 0;
            foreach (IMLDataPair pair in set)
            {
                IMLData input = pair.Input;
                IMLData actualData = pair.Ideal;
                IMLData predictData = network.Compute(input);

                double actual = actualData[0];
                double predict = predictData[0];
                double diff = Math.Abs(predict - actual);

               Direction  actualDirection = DetermineDirection(actual);
               Direction predictDirection = DetermineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;
                count++;
                Console.WriteLine(@"Number" + @"count" + @": actual=" + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"(" + predictDirection + @")" + @",diff=" + diff);

                DoInference((float)predict);
            }
            double percent = correct / (double)count;
            Console.WriteLine(@"Direction correct:" + correct + @"/" + count);
            Console.WriteLine(@"Directional Accuracy:" + Format.FormatPercent(percent));

           

            return percent;
        }
        #endregion

        // Run one epoch of the Fuzzy Inference System 
        private static void DoInference(float outputs)
        {
            // Setting inputs
          //  IS.SetInput("Steel", Convert.ToSingle(outputs));

            //IS.SetInput("LeftDistance", Convert.ToSingle(outputs));
            //IS.SetInput("FrontalDistance", Convert.ToSingle(outputs));

            // Setting outputs
            try
            {
                // inference section

                // setting inputs
                IS.SetInput("NearFarDistance", outputs);
                FuzzyOutput fuzzyOutput = IS.ExecuteInference("Percentage");

                // showing the fuzzy output
                foreach (FuzzyOutput.OutputConstraint oc in fuzzyOutput.OutputList)
                {
                    Console.WriteLine("Network is :"+oc.Label + @" by " + oc.FiringStrength.ToString());
                }

                //LinguisticVariable rs =  IS.GetLinguisticVariable("Steel") ; //.NumericInput = 12;
                //rs.NumericInput = outputs * 100;

                //LinguisticVariable rs1 = IS.GetLinguisticVariable("Stove"); //.NumericInput = 12;
                //rs1.NumericInput = outputs * 100;
                //float membercold =   rs.GetLabel("Cold").GetMembership(20);
                //float memberhot =  rs.GetLabel("Hot").GetMembership(50);
                //// testing the firing strength
              
                //float first = IS.GetRule("Test1").EvaluateFiringStrength();


                //Console.WriteLine("Firing Strenth:" + first  + " membership cold:"+ membercold + " memberhot:"+memberhot);

                //float membershitHot =   rs.GetLabelMembership("Hot", outputs);

                //double NewAngle = IS.Evaluate("Angle");
                //Console.WriteLine(NewAngle.ToString("Steel: ##0.#0"));
                //Angle += NewAngle;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :"+ex.Message);
            }
        }

        #region Direction enum

        public enum Direction
        {
            Up,
            Down
        } ;



        public static Direction DetermineDirection(double d)
        {
            return d < 0 ? Direction.Down : Direction.Up;
        }

        #endregion


        public static InferenceSystem IS;

        // Hardcode initializing the Fuzzy Inference System
        public static void InitFuzzyEngine()
        {


            //// linguistic labels (fuzzy sets) that compose the distances
            //FuzzySet fsNear = new FuzzySet("Near", new TrapezoidalFunction(0.15F, 0.50F, TrapezoidalFunction.EdgeType.Right));

            //// creating an array of points representing a typical trapezoidal function /-\
            //Point[] points = new Point[4];
            //// point where membership starts to rise
            //points[0] = new Point(-1, 0);
            //// maximum membership (1) reached at the second point 
            //points[1] = new Point(00, -0.01f);
            //// membership starts to fall at the third point 
            //points[2] = new Point(30, 1);
            //// membership gets to zero at the last point 
            //points[3] = new Point(40, 0);
            //// creating the instance

          
            TrapezoidalFunction functionc = new TrapezoidalFunction(-1, 0, 0, 0);
            FuzzySet fsCooli = new FuzzySet("UnderZero", functionc);
            TrapezoidalFunction functionjustnegative = new TrapezoidalFunction(-0.2f, 0, 0, 0);
            FuzzySet fsjustneg = new FuzzySet("JustNegative", functionjustnegative);
            TrapezoidalFunction functionFlat = new TrapezoidalFunction(-0.1f, 1, 0, 0);
            FuzzySet fsFlat = new FuzzySet("Flat", functionc);
            TrapezoidalFunction functionb = new TrapezoidalFunction(0f, 0f, 1f, 1f);
            FuzzySet fsAboveZero = new FuzzySet("AboveZero", functionb);


           // assigning coordinates in the constructor
            Point p1 = new Point( -1,0 );
            // creating a point and assigning coordinates later
            Point p2;
            p2.X = 0;
            p2.Y = 1;
            // calculating distance between two points
            Console.WriteLine(p1.DistanceTo(p2));


            //PiecewiseLinearFunction funclinear = new PiecewiseLinearFunction(p1);

            TrapezoidalFunction functionSmallPositive = new TrapezoidalFunction(0.01f, 0f, 0.05f, 0.06f);
            FuzzySet fsJustPositive = new FuzzySet("JustPositive", functionb);


            //// creating the instance
             SingletonFunction membershipFunction1 = new SingletonFunction(-1);


            FuzzySet MysetTests = new FuzzySet("Negative", membershipFunction1);

            //// getting membership for several points
            //for (int i = -1; i < 3; i++)
            //    Console.WriteLine("For -1:"+membershipFunction1.GetMembership(i));

            //// creating the instance
            //SingletonFunction membershipFunction2 = new SingletonFunction(1);
            //// getting membership for several points
            //for (int i = -1; i < 3; i++)
            //    Console.WriteLine("For 1 :"+membershipFunction2.GetMembership(i));

                //// create the linguistic labels (fuzzy sets) that compose the temperature 
                //TrapezoidalFunction FunNear = new TrapezoidalFunction(0.1f, 1.5f, TrapezoidalFunction.EdgeType.Right);
                //FuzzySet fsNear = new FuzzySet("Near", FunNear);
            
                //FuzzySet fsFar = new FuzzySet("Far", funcFar);
                // front distance (input)
                LinguisticVariable lvFront = new LinguisticVariable( "NearFarDistance",-1, 1 );


            lvFront.AddLabel(fsCooli);
                lvFront.AddLabel( fsAboveZero );
            lvFront.AddLabel(fsjustneg);
            lvFront.AddLabel(fsFlat);
            lvFront.AddLabel(fsJustPositive);
                // linguistic labels (fuzzy sets) that compose the angle

                // linguistic labels (fuzzy sets) that compose the angle
                //FuzzySet fsnegative = new FuzzySet("Negative", new TrapezoidalFunction(-1F, -1, -1F, -1F));
                //FuzzySet fsZero = new FuzzySet( "Zero",new TrapezoidalFunction( -1F, 0.5F, 0.5F, 1F ) );
                //FuzzySet fsLP = new FuzzySet( "LittlePositive",new TrapezoidalFunction( 0.5F, 0.1F, 0.2F, 0.25F ) );
                //FuzzySet fsP = new FuzzySet( "Positive",new TrapezoidalFunction( 0.2F, 0.25F, 0.35F, 0.40F ) );
                //FuzzySet fsVP = new FuzzySet( "VeryPositive",new TrapezoidalFunction( 0.35F, 0.40F, TrapezoidalFunction.EdgeType.Left ));

                // creating the instance
                SingletonFunction membershipFunction = new SingletonFunction(-1);
                // getting membership for several points
                for (int i = -2; i < 1; i++)
                    Console.WriteLine(membershipFunction.GetMembership(i));

                // create the linguistic labels (fuzzy sets) that compose the temperature 
                TrapezoidalFunction function1 = new TrapezoidalFunction(-1, 0.5f, 0, 0.5f);
                FuzzySet fsCold = new FuzzySet("Negative", function1);
                TrapezoidalFunction function2 = new TrapezoidalFunction(0f, 0f, 1f, 0f);
                FuzzySet fsCool = new FuzzySet("Positive", function2);
                TrapezoidalFunction function3 = new TrapezoidalFunction(-0.1f, 0f, 1f, 0f);
                FuzzySet fsCrap = new FuzzySet("NextToFlat", function3);
                TrapezoidalFunction function4 = new TrapezoidalFunction(0.5f, 0f, 0.5f, 0f);
                FuzzySet Nearpositive = new FuzzySet("NearPositive", function3);
                //TrapezoidalFunction function3 = new TrapezoidalFunction(2f, 2.5f, 3.0f, 3.5f);
                //FuzzySet fsWarm = new FuzzySet("Positive", function3);
                //TrapezoidalFunction function4 = new TrapezoidalFunction(3.0f, 3.5f, TrapezoidalFunction.EdgeType.Left);
                //FuzzySet fsHot = new FuzzySet("VeryPositive", function4);




                // angle
                LinguisticVariable lvAngle = new LinguisticVariable( "Percentage", -1, 1);
                lvAngle.AddLabel( fsCold );
                lvAngle.AddLabel( fsCool );
            lvAngle.AddLabel(fsCrap);
            lvAngle.AddLabel(fsFlat);
            lvAngle.AddLabel(Nearpositive);
            lvAngle.AddLabel(fsAboveZero);
                // the database
                Database fuzzyDB = new Database( );
                fuzzyDB.AddVariable( lvFront );
                fuzzyDB.AddVariable( lvAngle );

                // creating the inference system
                IS = new InferenceSystem( fuzzyDB, new CentroidDefuzzifier( 1000 ) );

                // going straight
                IS.NewRule("Rule 1", "IF NearFarDistance IS UnderZero THEN Percentage IS Negative");
                // turning left
                IS.NewRule("Rule 2", "IF NearFarDistance IS AboveZero THEN Percentage IS AboveZero");

                IS.NewRule("Rule 3", "IF NearFarDistance IS JustNegative THEN Percentage IS NextToFlat");

                IS.NewRule("Rule 4", "IF NearFarDistance IS JustPositive THEN Percentage IS NearPositive");
             

        }
        public static double TrainNetworks(BasicNetwork network, IMLDataSet minis)
        {
            Backpropagation trainMain = new Backpropagation(network, minis,0.0001,0.6);
            //set the number of threads below.
            trainMain.ThreadCount = 0;
            // train the neural network
            ICalculateScore score = new TrainingSetScore(minis);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
           // IMLTrain trainMain = new Backpropagation(network, minis, 0.0001, 0.01);
            
            StopTrainingStrategy stop = new StopTrainingStrategy(0.0001, 200);
            trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            //prune strategy not in GIT!...Removing it.
            //PruneStrategy strategypruning = new PruneStrategy(0.91d, 0.001d, 10, network,minis, 0, 20);
            //trainMain.AddStrategy(strategypruning);

         //   EncogUtility.TrainConsole(trainMain,network,minis, 15.2);

            List<double> IterationsErrors = new List<double>();
            Form1 forms = new Form1();

            //ErrorViewerForm.Form1 formError = new ErrorViewerForm.Form1();
            forms.chart1.Series.Clear();

            forms.chart1.Series.Add("Error Rates");
            forms.chart1.Series.Add("Error Improvements");
            List<double> iMRPOVEMENTS = new List<double>();

            double prevError = 0;
            var sw = new Stopwatch();
            sw.Start();
            while (!stop.ShouldStop())
            {
                prevError = trainMain.Error;
                trainMain.Iteration();
                IterationsErrors.Add(trainMain.Error);

                
             
                Console.WriteLine(@"Iteration #:" + trainMain.IterationNumber + @" Error:" + trainMain.Error + @" Genetic Iteration:" + trainAlt.IterationNumber);
            }
            sw.Stop();
            forms.chart1.Series["Error Rates"].Points.DataBindY(IterationsErrors);
            forms.chart1.Series[0].ToolTip = "F5";
            forms.chart1.Series[0].ChartType = SeriesChartType.FastLine;
            forms.chart1.Series[0].Color = Color.Yellow;
            forms.chart1.Invalidate();

            //forms.chart1.Series["Error Improvements"].Points.DataBindY(iMRPOVEMENTS);
            //forms.chart1.Series[1].ToolTip = "F5";
            //forms.chart1.Series[1].ChartType = SeriesChartType.FastLine;
            //forms.chart1.Series[1].Color = Color.Peru;
            //forms.chart1.Invalidate();
            forms.ShowDialog();
           


            Console.WriteLine(@"Total elapsed time in seconds:" + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Seconds);

            return trainMain.Error;
        }

        public static BasicMLDataSet CreateEvaluationSetAndLoad(string @fileName, int startLine, int HowMany, int WindowSize, int outputsize)
        {
            List<double> Opens = QuickCSVUtils.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = QuickCSVUtils.QuickParseCSV(fileName, "High", startLine, HowMany);
            // List<double> Low = QuickCSVUtils.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = QuickCSVUtils.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(fileName, 5, startLine, HowMany);
            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());
            IMLDataPair aPairInput = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput3 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput2 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput4 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput5 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), WindowSize, outputsize);
            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);
            listData.Add((aPairInput5));


            var minitrainning = new BasicMLDataSet(listData);
            return minitrainning;
        }

        public static TemporalMLDataSet GenerateATemporalSet(string @fileName, int startLine, int HowMany, ulong WindowSize, int outputsize)
        {
            List<double> Opens = QuickCSVUtils.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = QuickCSVUtils.QuickParseCSV(fileName, "High", startLine, HowMany);
            List<double> Low = QuickCSVUtils.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = QuickCSVUtils.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(fileName, 5, startLine, HowMany);

            return TrainerHelper.GenerateTrainingWithPercentChangeOnSerie((ulong)WindowSize, (ulong)outputsize, Opens.ToArray(), Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());
        }


        public static IMLMethod CreateElmanNetwork(int inputsize, int outputsize)
        {
            // construct an Elman type network
            ElmanPattern pattern = new ElmanPattern();

            pattern.ActivationFunction = new ActivationTANH();
            pattern.InputNeurons = inputsize;
            
            pattern.AddHiddenLayer(0);
            pattern.OutputNeurons = outputsize;
            return pattern.Generate();
            
        }

        


    }
}
