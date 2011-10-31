using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSVanalyze.Properties;
using Encog.Neural.Networks;
using Encog.ML.Data;
using Encog.ML.Data.Market;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
using QuickCSV = Encog.Util.NetworkUtil.QuickCSVUtils;
using TrainingHelpers = Encog.Util.NetworkUtil.TrainerHelper;

namespace CSVanalyze
{
    class Program
    {


        static Dictionary<string, BasicNetwork> NetworkHolderDictionnary = new Dictionary<string, BasicNetwork>();
        static Dictionary<string, MarketMLDataSet> MarketTrainingsDictionary = new Dictionary<string, MarketMLDataSet>();

        static void Main(string[] args)
        {

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
                    DoEvaluator();
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


        private static void DoEvaluator()
        {
            throw new NotImplementedException();
        }

        private static void DoGenerate(string file)
        {

            //Lets add all the types we will work with.
            List<MarketDataType> TypeUsed = new List<MarketDataType>();
            AddTypes(TypeUsed);
            //Lets load data and save it in a file ready for re use later.
            MarketMLDataSet SetOpen = Loader.GrabData(file, TypeUsed, "Open", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingOpen, SetOpen);
            MarketTrainingsDictionary.Add("Open", SetOpen);

            MarketMLDataSet SetClose = Loader.GrabData(file, TypeUsed, "Close", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingClose, SetClose);
            MarketTrainingsDictionary.Add("Close", SetClose);

            MarketMLDataSet SetHigh = Loader.GrabData(file, TypeUsed, "High", true);
            SuperUtils.SaveTraining(ExecutingDirectory, Settings.Default.TrainingHigh, SetHigh);
            MarketTrainingsDictionary.Add("High", SetHigh);

            MarketMLDataSet SetLow = Loader.GrabData(file, TypeUsed, "Low", true);
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
                string currentfile = "Network" + networkstring + "Low.eg";
                BasicNetwork nets = Training.TrainNetwork(networkstring, NetworkHolderDictionnary[networkstring], MarketTrainingsDictionary["Low"], false);
                SuperUtils.SaveNetwork(ExecutingDirectory, currentfile, nets);
            }

        }



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
