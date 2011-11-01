using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CSVanalyze.Properties;
using Encog.ML.Data.FXDataSet;
using Encog.Neural.Networks;
using Encog.ML.Data;
using Encog.ML.Data.Market;
using Encog.Util;

using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
using QuickCSV = Encog.Util.NetworkUtil.QuickCSVUtils;
using TrainingHelpers = Encog.Util.NetworkUtil.TrainerHelper;
using System.Collections;

namespace CSVanalyze
{
    class Program
    {


        static Dictionary<string, BasicNetwork> NetworkHolderDictionnary = new Dictionary<string, BasicNetwork>();
        static Dictionary<string, FXDataSet> MarketTrainingsDictionary = new Dictionary<string, FXDataSet>();

        private static List<EvaluationResults> EvaluationResultList = new List<EvaluationResults>();

        static void Main(string[] args)
        {

            //MT4 CSV format is being used.
            DateTimeCSVFormat = "yyyy.MM.dd HH:mm:ss";
            try
            {
                ParseArgs(args);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message + " Stack :" + ex.StackTrace.ToString() + " Source:" + ex.Source);
            }
            finally
            {
                Console.WriteLine(".............End Press a key .............");
                Console.ReadKey();
                Environment.Exit(0);
            }          
        }


#region console parser

        private static void ParseArgs(string[] args)
        {
           
            if (args.Count() < 1)
            {
                HelpOutput();
                return;
            }

            switch (args[0])
            {
                case "Generate":
                    GenerateParser(ref args);
                    break;
                //case "Train":
                //    TrainParser();
                //    break;
                case "Eval":
                    DoEvaluator(args[1]);
                    break;
                case "prune":
                    break;
                case "ViewSettings":
                    SettingsViewAndEdit(args);
                    break;
                case "EditSettings":
                    SettingsViewAndEdit(args);
                    break;
                default:
                    break;
            }
        }


        #region Everything related to parsing args and training networks
      /*
        private static void TrainParser()
        {

            //Lets load all our training files.
            MarketMLDataSet Set = (MarketMLDataSet) SuperUtils.LoadTrainingMarketDataSet(ExecutingDirectory, Settings.Default.TrainingOpen);
            //Lets add this dataset to the dataset dictionnary.
            MarketTrainingsDictionary.Add("Open", Set);
            //we use the same Dataset object so we save a bit of memory.
            Set = (MarketMLDataSet) SuperUtils.LoadTrainingMarketDataSet(ExecutingDirectory, Settings.Default.TrainingHigh);
            MarketTrainingsDictionary.Add("High", Set);
            //we use the same Dataset object so we save a bit of memory.
            Set = (MarketMLDataSet) SuperUtils.LoadTrainingMarketDataSet(ExecutingDirectory, Settings.Default.TrainingLow);
            MarketTrainingsDictionary.Add("Low", Set);
            //we use the same Dataset object so we save a bit of memory.
            Set = (MarketMLDataSet) SuperUtils.LoadTrainingMarketDataSet(ExecutingDirectory, Settings.Default.TrainingClose);
            MarketTrainingsDictionary.Add("Close", Set);
            
            
            //Lets create the networks.
            BasicNetwork elmhan = (BasicNetwork)Build.NetworkBuilders.CreateElmanNetwork(Settings.Default.InputSize * Set.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1);
            BasicNetwork FeedForward = (BasicNetwork)Build.NetworkBuilders.CreateFeedforwardNetwork(Settings.Default.InputSize * Set.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1, Settings.Default.Hidden2);
            BasicNetwork JordanNetwork = (BasicNetwork)Build.NetworkBuilders.CreateJordanNetwork(Settings.Default.InputSize * Set.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1);
            //lets add the networks to our dictionary
            NetworkHolderDictionnary.Add("Elman", elmhan);
            NetworkHolderDictionnary.Add("Feed", FeedForward);
            NetworkHolderDictionnary.Add("Jordan", JordanNetwork);
            //Lets add all the types we will work with.
            List<MarketDataType> TypeUsed = new List<MarketDataType>();
            AddTypes(TypeUsed);
            //Lets check if the training files can be found.
            CheckTrainingExistence();

          

            //we have all the market training, now lets train our networks.

            foreach  (string networkstring  in NetworkHolderDictionnary.Keys)
            {
                //Lets get the current file we will save this network on.
                string currentfile = "Network" + networkstring + "Open.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["Open"], false);

                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);


            }



        }
        */
        /// <summary>
        /// Checks the training file existence.
        /// If the files dont exists an error message will be outputted to the console.
        /// </summary>
        private static void CheckTrainingExistence()
        {
            if (!File.Exists(Settings.Default.TrainingOpen) || !File.Exists(Settings.Default.TrainingHigh) || !File.Exists(Settings.Default.TrainingClose) || !File.Exists(Settings.Default.TrainingLow))
            {
                Console.WriteLine("It seems the training files can't be found, you should run the training generator by using the command line: Generate [file]");
                return;
            }
        }









        #endregion


        #region everything related to parsing args to create training files.

        private static void GenerateParser(ref string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You need to input a csv file to create the datasets");
                Console.WriteLine("Help : Generate [File]");               
                return;

            }
            if (File.Exists(args[1]))
                DoGenerate(args[1]);
            else
            {
                Console.WriteLine("The specified file : " + args[1] + " doesnt seem to exist, you must specify a valid csv file");
                Console.WriteLine("Help : Generate [File]");                
            }
        }

        #endregion


#endregion


        #region command calls


        private static void DoEvaluator(string argumentfile)
        {
            //if (!CheckNetWorkFileExistance())
            //    return;
            if (!File.Exists(argumentfile))
            {
                Console.WriteLine(@"couldnt find the file you specified to make an evaluation");
                return;
            }

            //we have the elman network files...Lets start doing some evals.

            //first lets grab a month of data , and we can use it as our evaluation.

            //Lets create the networks.
            
             
            //Lets get the current file we will save this network on.
            //elman networks
            string Elman = "Elman";

             string currentfileOpen = "Network" + Elman + "Open.eg";
            BasicNetwork elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Elman+"Open", elmhannetwork);

            currentfileOpen =  "Network" + Elman + "Close.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Elman+"Close", elmhannetwork);

            currentfileOpen = "Network" + Elman + "High.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Elman+"High", elmhannetwork);

            currentfileOpen = "Network" + Elman + "Low.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Elman+"Low", elmhannetwork);



            //feed networks
            string Feed = "Feed";

            currentfileOpen = "Network" + Feed + "Open.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Feed + "Open", elmhannetwork);

            currentfileOpen = "Network" + Feed + "Close.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Feed + "Close", elmhannetwork);

            currentfileOpen = "Network" + Feed + "High.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Feed + "High", elmhannetwork);

            currentfileOpen = "Network" + Feed + "Low.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Feed + "Low", elmhannetwork);


            //jordan networks.

            string Jordan = "Jordan";

            currentfileOpen = "Network" + Jordan + "Open.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Jordan + "Open", elmhannetwork);

            currentfileOpen = "Network" + Jordan + "Close.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Jordan + "Close", elmhannetwork);

            currentfileOpen = "Network" + Jordan + "High.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Jordan + "High", elmhannetwork);

            currentfileOpen = "Network" + Jordan + "Low.eg";
            elmhannetwork = SuperUtils.LoadNetwork(ExecutingDirectory, currentfileOpen);
            NetworkHolderDictionnary.Add(Jordan + "Low", elmhannetwork);



            //lets grab data for evaluation.
            //Lets add all the types we will work with.
            List<MarketDataType> TypeUsed = new List<MarketDataType>();
            AddTypes(TypeUsed);

            DateTime FromDate = Settings.Default.EvalStartDate;
            DateTime ToDate = Settings.Default.EvalEndDate;
            //Lets load data and save it in a file ready for re use later.
            FXDataSet SetOpen = Loader.GrabEvaluationData(argumentfile, TypeUsed, "Open", true,FromDate,ToDate);
            MarketTrainingsDictionary.Add("Open", SetOpen);

            FXDataSet SetClose = Loader.GrabEvaluationData(argumentfile, TypeUsed, "Close", true, FromDate, ToDate);
            MarketTrainingsDictionary.Add("Close", SetClose);

            FXDataSet SetHigh = Loader.GrabEvaluationData(argumentfile, TypeUsed, "High", true, FromDate, ToDate);
            MarketTrainingsDictionary.Add("High", SetHigh);

            FXDataSet SetLow = Loader.GrabEvaluationData(argumentfile, TypeUsed, "Low", true, FromDate, ToDate);
      
            MarketTrainingsDictionary.Add("Low", SetLow);

            

            //now lets start evaluating this network ..
            //we have all the market training, now lets train our networks.
      
            foreach (KeyValuePair<string, FXDataSet> keyValuePair in MarketTrainingsDictionary)
            {
               EvaluateNetwork(keyValuePair.Value,NetworkHolderDictionnary[Elman+keyValuePair.Key], keyValuePair.Key, "Elman "+keyValuePair.Key);

               EvaluateNetwork(keyValuePair.Value, NetworkHolderDictionnary[Feed + keyValuePair.Key], keyValuePair.Key, Feed +" "+ keyValuePair.Key);

               EvaluateNetwork(keyValuePair.Value, NetworkHolderDictionnary[Jordan + keyValuePair.Key], keyValuePair.Key, Jordan+" " + keyValuePair.Key);
            }

            if (Settings.Default.OutputsAll)
            foreach (EvaluationResults resultse in EvaluationResultList)
            {
                Console.WriteLine(@"Network :" + resultse.NetworkName + @" on data set:" + resultse.DataSetName +
                                  @" Percent correct : " + resultse.PercentCorrent +
                                  @" Accuracy :" + resultse.CorrentRight + @" \ " + resultse.Counts);
            }

            int index = 0;

         

            foreach (PredictedAndOutsputs Analysisobject in outputs)
            {



                if (Analysisobject.NetworkName.Contains("Jordan"))
                {

                    double x = GetPredictedPriceFromPreviousPriceAndPrediction(Analysisobject.PredictedPercentage,
                                                                               Analysisobject.PreviousPrice);

                    Direction actualDirection = DetermineDirection(Analysisobject.ActualPercentage);
                    Direction predictDirection = DetermineDirection(GetAveragePercentage(Analysisobject.Sequence, Analysisobject.DatasetName));

                    if (actualDirection == predictDirection)
                        CountRights++;
                    CountTotal++;


                    if (logs++ < 30)
                    {

                        double PredictedOpen = GetAveragePriceForSequence(Analysisobject.Sequence, "Open");
                        double PredictedLow = GetAveragePriceForSequence(Analysisobject.Sequence, "Low");
                        double PredictedClose = GetAveragePriceForSequence(Analysisobject.Sequence, "Close");
                        double PredictedHigh = GetAveragePriceForSequence(Analysisobject.Sequence, "High");
                        


                        Console.WriteLine(" \nPredicted Average percentage Move:" +
                                          GetAveragePercentage(Analysisobject.Sequence, Analysisobject.DatasetName) +
                                          " \nActual direction :" + Analysisobject.ActualDirection 
                                          + " \nActual Price :" + Analysisobject.ActualPrice 
                                          + " \nPrevious Price :" + Analysisobject.PreviousPrice 
                                          + " \nAverage Price of predictions:" +GetAveragePriceForSequence(Analysisobject.Sequence, Analysisobject.DatasetName)
                                          + "\nPredicted High:"+PredictedHigh 
                                          + " \nPredicted Low:"+PredictedLow
                                          +" \nPredicted Open"+PredictedOpen 
                                          + " \nPredicted Close:"+PredictedClose
                                          
                                          
                                          
                                          );}

                    else if (logs > 30&& logs < 32)
                    {
                        foreach (EvaluationResults resultse in EvaluationResultList)
                        {
                            Console.WriteLine(@"Network :" + resultse.NetworkName + @" on data set:" +
                                              resultse.DataSetName +
                                              @" Percent correct : " + resultse.PercentCorrent *100+
                                              @" Accuracy :" + resultse.CorrentRight *100 + @" \ " + resultse.Counts);
                        }
                    }
                }



            }

            //var query = from valeurs in outputs.NetworksDirections.Values
            //            from networknames in outputs.NetworksDirections.Keys
            //            where valeurs.ActualDirection == valeurs.PredictedDirection
            //            where valeurs.
            //            where valeurs.NetworkName.Equals("Elman")


            Console.WriteLine(" Average Is :" + CountRights + " On count Total :" + CountTotal);

        }

        private static int logs = 0;
        private static int CountRights = 0;
        private static int CountTotal;
        public static double GetAveragePriceForSequence(UInt64 sequence, string trainingName)
        {
            UInt64 prevSequence = 0;
            var Seqs = from xys in outputs
                       where xys.Sequence == sequence
                       where xys.DatasetName == trainingName
                       select (xys);
            double y = Seqs.Average(x => x.PredictedPrice);
            Seqs.AsQueryable();
            //Seqs.Dump();
            return y;
        }


        public static double GetAveragePercentage(UInt64 sequence, string trainingName)
        {
            UInt64 prevSequence = 0;
            var Seqs = from xys in outputs
                       where xys.Sequence == sequence
                       where xys.DatasetName == trainingName
                       select (xys);
            double y = Seqs.Average(x => x.PredictedPercentage);
            Seqs.AsQueryable();
            //Seqs.Dump();




            return y;
        }

        public static string DateTimeCSVFormat { get; set;}
        public static double GetPrice(int CurrentIndex, string WhichTraining)
        {
            //we know our starting date by the eval starting date.

            //QuickCSV.QuickParseCSVForDate(file, datetofind, DateTimeCSVFormat,0);
           return MarketTrainingsDictionary[WhichTraining].Points[CurrentIndex].Data[3];

        }
        public static double GetPreviousPrice(int CurrentIndex, string WhichTraining)
        {
            //we know our starting date by the eval starting date.

            //Lets check if we are above zero index and below the max number of points in our dataset.
            if (CurrentIndex - 1 >= 0 && CurrentIndex - 1 < MarketTrainingsDictionary[WhichTraining].Points.Count)
                return MarketTrainingsDictionary[WhichTraining].Points[CurrentIndex - 1].Data[3];
            else return 0;
        }

        public static UInt64 GetSequence(int CurrentIndex, string WhichTraining)
        {
            //we know our starting date by the eval starting date.

            //Lets check if we are above zero index and below the max number of points in our dataset.
            if (CurrentIndex - 1 >= 0 && CurrentIndex - 1 < MarketTrainingsDictionary[WhichTraining].Points.Count)
                return MarketTrainingsDictionary[WhichTraining].Points[CurrentIndex - 1].Sequence;
            else return 0;
        }


        public static double GetPredictedPriceFromPreviousPriceAndPrediction(double predictedDoublePercent, double ActualPreviousPrice)
        {
            double MakeDouble = ActualPreviousPrice*predictedDoublePercent + ActualPreviousPrice;
            return MakeDouble;

        }
        public static DateTime GetDate(int CurrentIndex, string WhichTraining)
        {
            
            DateTime curDate = new DateTime(Convert.ToInt64(MarketTrainingsDictionary[WhichTraining].Points[CurrentIndex].Sequence));
            return curDate;

        }
     
        static  List<PredictedAndOutsputs> outputs = new List<PredictedAndOutsputs>();
        /// <summary>
        /// Evaluates the network with the specified data
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="network">The network.</param>
        /// <param name="what">The name of the data set.</param>
        /// <param name="whatnetwork">The name of the network being evaluated.</param>
        private static void EvaluateNetwork(IEnumerable<IMLDataPair> data, BasicNetwork network, string what, string whatnetwork)
        {

            
            int count = 0;
            int correct = 0;
            EvaluationResults res = new EvaluationResults();
            foreach (IMLDataPair pair in data)
            {
                PredictedAndOutsputs dirobject = new PredictedAndOutsputs();
                IMLData input = pair.Input;
                IMLData actualData = pair.Ideal;
                IMLData predictData = network.Compute(input);

                double actual = actualData[0];
                double predict = predictData[0];
                double diff = Math.Abs(predict - actual);

                Direction actualDirection = DetermineDirection(actual);
                Direction predictDirection = DetermineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;

                count++;

                if(Settings.Default.OutputsAll)
                Console.WriteLine(whatnetwork +@" Minutes " + count + @":actual="
                                  + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"("
                                  + predictDirection + @")" + @",diff=" + diff + @" on dataset:"+ what);



                dirobject.ActualPrice = GetPrice(count, what);
                dirobject.PreviousPrice = GetPreviousPrice(count, what);
                dirobject.ActualDate = GetDate(count, what);
                dirobject.ActualDirection = actualDirection;
                dirobject.DatasetName = what;
                dirobject.NetworkName = whatnetwork;
                dirobject.PredictedPercentage = predict;
                dirobject.ActualPercentage = actual;
                dirobject.PredictedDirection = predictDirection;
                dirobject.Sequence = GetSequence(count, what);
                dirobject.PredictedPrice = dirobject.PreviousPrice + (dirobject.PreviousPrice * predict);
                outputs.Add(dirobject);

            }
            double percent = correct / (double)count;
            if (Settings.Default.OutputsAll)
            Console.WriteLine(@"Direction correct:" + correct + @"/" + count +@" On data set:"+what);
            if (Settings.Default.OutputsAll)
            Console.WriteLine(@"Directional Accuracy:" + Format.FormatPercent(percent));


           
            res.CorrentRight = correct;
            res.Counts = count;
            res.DataSetName = what;
            res.NetworkName = whatnetwork;
            res.PercentCorrent = percent;
            EvaluationResultList.Add(res);
           
        }

        #region analysis
        public class EvaluationResults
        {
           public double PercentCorrent { get; set; }
           public double CorrentRight { get; set; }
           public int Counts { get; set; }
           public string NetworkName { get; set; }
           public string DataSetName { get; set; }
           public List<DirectionalAnalysis> ActualForecastedDirections = new List<DirectionalAnalysis>();
        }



        public class DirectionalAnalysis
        {
            public Hashtable NetworksDirections = new Hashtable();
        }
        
        
        /// <summary>
        /// this class holds the predicted directions of all networks and the actual direction.
        /// Makes this easy to check all the predicted network values versus the actual prediction. 
        /// </summary>
        public class PredictedAndOutsputs
        {

            public Direction PredictedDirection { get; set;}
            public Direction ActualDirection { get; set; }
            public DateTime ActualDate { get; set; }
            public double ActualPrice { get; set; }
            public double PredictedPrice { get; set; }
            public string NetworkName { get; set; }
            public string DatasetName { get; set; }
            public double PreviousPrice { get; set; }
            public double PredictedPercentage { get; set; }
            public double ActualPercentage { get; set;}
            public UInt64 Sequence { get; set; }
        }
        #endregion





        private static bool CheckNetWorkFileExistance()
        {
            if (!File.Exists(Settings.Default.NetworkElmanClose) || !File.Exists(Settings.Default.NetworkElmanHigh) || !File.Exists(Settings.Default.NetworkElmanLow) || !File.Exists(Settings.Default.NetworkElmanOpen))
            {
                Console.WriteLine(@"It seems the elman network files can't be found, you should run the training generator by using the command line: Generate [file]");
                return false;
            }
            return true;
        }



        #region file generations
        private static void DoGenerate(string file)
        {

            //Lets add all the types we will work with.
            List<MarketDataType> TypeUsed = new List<MarketDataType>();
            AddTypes(TypeUsed);
            //Lets load data and save it in a file ready for re use later.
            FXDataSet SetOpen = Loader.GrabData(file, TypeUsed, "Open", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingOpen, SetOpen);
            MarketTrainingsDictionary.Add("Open", SetOpen);

            FXDataSet SetClose = Loader.GrabData(file, TypeUsed, "Close", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingClose, SetClose);
            MarketTrainingsDictionary.Add("Close", SetClose);

            FXDataSet SetHigh = Loader.GrabData(file, TypeUsed, "High", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingHigh, SetHigh);
            MarketTrainingsDictionary.Add("High", SetHigh);

            FXDataSet SetLow = Loader.GrabData(file, TypeUsed, "Low", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingLow, SetLow);
            MarketTrainingsDictionary.Add("Low", SetLow);


            //Meta trader files have no headers...and are as below.
            //2000.09.27,00:00,0.60350,0.60380,0.60280,0.60330,95





            //Lets create the networks.
            BasicNetwork elmhan = (BasicNetwork)Build.NetworkBuilders.CreateElmanNetwork((int)SetOpen.InputWindowSize * SetOpen.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1);
            
            BasicNetwork FeedForward = (BasicNetwork)Build.NetworkBuilders.CreateFeedforwardNetwork((int)SetOpen.InputWindowSize*SetOpen.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1, Settings.Default.Hidden2);
            BasicNetwork JordanNetwork = (BasicNetwork)Build.NetworkBuilders.CreateJordanNetwork((int)SetOpen.InputWindowSize* SetOpen.Descriptions.Count, Settings.Default.OutPutSize, Settings.Default.Hidden1);


            ////lets add the networks to our dictionary
            NetworkHolderDictionnary.Add("Elman", elmhan);
            NetworkHolderDictionnary.Add("Feed", FeedForward);
            NetworkHolderDictionnary.Add("Jordan", JordanNetwork);
            ////Lets add all the types we will work with.
            
            ////Lets check if the training files can be found.
            //CheckTrainingExistence();



            //we have all the market training, now lets train our networks.

            foreach (string networkstring in NetworkHolderDictionnary.Keys)
            {
                //Lets get the current file we will save this network on.
                string currentfile = "Network" + networkstring + "Open.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["Open"], false);
                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);
            }

            foreach (string networkstring in NetworkHolderDictionnary.Keys)
            {
                //Lets get the current file we will save this network on.
                string currentfile = "Network" + networkstring + "High.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["High"], false);
                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);
            }

            foreach (string networkstring in NetworkHolderDictionnary.Keys)
            {
                //Lets get the current file we will save this network on.
                string currentfile = "Network" + networkstring + "Low.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["Low"], false);
                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);
            }

            foreach (string networkstring in NetworkHolderDictionnary.Keys)
            {
                //Lets get the current file we will save this network on.
                string currentfile = "Network" + networkstring + "Close.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["Close"], false);
                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);
            }

        }
        #endregion



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

        public static double GrabPriceFromDate(DateTime theDate)
        {


            return 0;
        }

        #endregion
        #region add market data types
        
        /// <summary>
        /// Adds the marketdatatypes used by the neural network.
        /// </summary>
        /// <param name="TypeUsed">The type used.</param>
        private static void AddTypes(List<MarketDataType> TypeUsed)
        {
            if (!TypeUsed.Contains(MarketDataType.Close))
                TypeUsed.Add(MarketDataType.Close);
            if (!TypeUsed.Contains(MarketDataType.High))
            TypeUsed.Add(MarketDataType.High);
            if (!TypeUsed.Contains(MarketDataType.Low))
            TypeUsed.Add(MarketDataType.Low);
            if (!TypeUsed.Contains(MarketDataType.Open))
            TypeUsed.Add(MarketDataType.Open);
        }

        #endregion

#endregion



        #region help string ouput.
        private static void HelpOutput()
        {
            HelpStrings();
        }


        #region helps string output
        /// <summary>
        /// Reads a text file and outputs it to the console as the help screen.
        /// </summary>
        /// <returns></returns>
        static void HelpStrings()
        {

            StringBuilder bl = new StringBuilder();
            FileInfo dataDir = new FileInfo(ExecutingDirectory);

            using (StreamReader rd = new StreamReader(dataDir.Directory.ToString() + "\\Debug\\HelpFile.txt"))
            {
                bl.Append(rd.ReadToEnd());
            }
            Console.WriteLine(bl.ToString());
        }

        public static string ExecutingDirectory
        {
            get
            {
                string assemblyLaunch = System.Reflection.Assembly.GetCallingAssembly().Location;
                return Path.GetDirectoryName(assemblyLaunch);
            }
        }
        #endregion
        #endregion

        #region settings
        static void SettingsViewAndEdit(string[] args)
        {
            if (args[0].Equals("ViewSettings"))
            {
                SetSettings.ViewSettings();
               
               

            }
            if (args[0].Equals("EditSettings"))
            {
                Console.WriteLine("You can edit settings by typing :");
                Console.WriteLine(" From , To, Input , Output , Hidden1 , Hidden2, EvalStart, EvalEnd , and the new value");
                if (args.Length < 2)
                {
                    Console.WriteLine("You need to supply 2 values : Editsettings [setting] [value]");
                    return;
                }


                if (args[1].Length > 0 && args[2].Length > 0)
                    SetSettings.EditASetting(args[1], args[2]);
               

            }
        }
        #endregion

       

 
    }
}


/*
    public static class LinqDumps
    {
        public static T Dump<T>(this T o)
        {
            var localUrl = Path.GetTempFileName() + ".html";
            using (var writer = LINQPad.Util.CreateXhtmlWriter(true))
            {
                writer.Write(o);
                File.WriteAllText(localUrl, writer.ToString());
            }
            Process.Start(localUrl);
            return o;
        }
    }
 */