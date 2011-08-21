using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Util.CSV;
using Encog.Util.File;
using Encog.Util.Simple;
using NUnit.Framework;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;

namespace Encog.Examples.RangeAndPredictions
{

    public class MarketBuildTraining
    {
        public static void Generate(string fileName)
        {




            GetStuffs();


            //EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Examples.RangeCalculators.RangeConfig.TRAINING_FILE), market);

            //// create a network



            //Console.WriteLine(@"Built training file , and saved it to:" + dataDir.DirectoryName);

            ////BasicNetwork network =
            ////    (BasicNetwork) MarketBuildTraining.NetworkBuilders.CreateFeedforwardNetwork(RangeConfig.INPUT_WINDOW,
            ////                                                                                RangeConfig.PREDICT_WINDOW,
            ////                                                                                RangeConfig.HIDDEN1_COUNT,
            ////                                                                                RangeConfig.HIDDEN2_COUNT);
            //// training file




            //// train the neural network
            ////  EncogUtility.TrainConsole(network, trainingSet, RangeConfig.TRAINING_MINUTES);
            //NetworkBuilders.DualNetworksTrainer(market.InputNeuronCount, market.IdealSize,
            //                                    Examples.RangeCalculators.RangeConfig.HIDDEN1_COUNT, Examples.RangeCalculators.RangeConfig.HIDDEN2_COUNT, market,
            //                                    Examples.RangeCalculators.RangeConfig.NETWORK_FILE);


        }

   
        [Test]
        public static void GetStuffs()
        {
            string fileName = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
            List<double> Opens = SuperUtils.QuickParseCSV(fileName, "Open");
            List<double> High = SuperUtils.QuickParseCSV(fileName, "High");
            List<double> Low = SuperUtils.QuickParseCSV(fileName, "Low");
            List<double> Close = SuperUtils.QuickParseCSV(fileName, "Close");

            double[] Opensd = new double[Opens.Count];
            double[] Closed = new double[Close.Count];
            double[] Highd = new double[High.Count];
            double[] Lowd = new double[Low.Count];




            Opensd= CreateNormalizatione(Opensd.ToArray());
            Highd = CreateNormalizatione(High.ToArray());
            Closed = CreateNormalizatione(Close.ToArray());
            Lowd = CreateNormalizatione(Low.ToArray());



        }
        public IMLDataSet CreateSetFromInputs(double [][] input,int nbofInputs)
        {
            

        }

        public IMLDataSet GenerateTraining(double[][] input, double[][] ideal)
        {
            IMLDataSet result = new BasicMLDataSet(input, ideal);
            return result;
        }



        public static double [] CreateNormalizatione(double [] input)
        {
            double [] results = SuperUtils.NormalizeThisArray(input);
            return results;
        }
        public class PredictionInputs
        {
            private int _inputsize = 0;
            private int _outputsize = 0;
            private List<InterestRate> rates = new List<InterestRate>();
            private List<FinancialSample> samples = new List<FinancialSample>();
            private int inputSize;
            private int outputSize;
            public PredictionInputs(int inputSize, int outputSize)
            {
                this._inputsize = inputSize;
                this._outputsize = outputSize;
            }

            public void GetInputData(int offset, double[] input)
            {
                Object[] samplesArray = samples.ToArray();
                // get SP500 & prime data
                for (int i = 0; i < this._inputsize; i++)
                {
                    FinancialSample sample = (FinancialSample)samplesArray[offset + i];
                    input[i] = sample.GetPercent();
                    input[i + this._outputsize] = sample.GetRate();
                }
            }
            public double GetPrimeRate(DateTime date)
            {
                double currentRate = 0;
                foreach (InterestRate rate in this.rates)
                {
                    if (rate.getEffectiveDate().CompareTo(date) > 0)
                    {
                        return currentRate;
                    }
                    else
                    {
                        currentRate = rate.getRate();
                    }
                }
                return currentRate;
            }



            public IList<FinancialSample> GetSamples()
            {
                return this.samples;
            }
            public void Load(String sp500Filename,
            String primeFilename)
            {
                loadSP500(sp500Filename);
                loadPrime(primeFilename);
                stitchInterestRates();
                //CalculatePercents();
            }
            public void loadPrime(String primeFilename)
            {
                ReadCSV csv = new ReadCSV(primeFilename,true,CSVFormat.English);

                while (csv.Next())
                {
                    DateTime date = csv.GetDate("date");
                    double rate = csv.GetDouble("prime");
                    InterestRate ir = new InterestRate(date, rate);
                    this.rates.Add(ir);
                }

                csv.Close();
                this.rates.Sort();
            }

            public void loadSP500(String sp500Filename)
            {
                ReadCSV csv = new ReadCSV(sp500Filename,true,CSVFormat.English);
                while (csv.Next())
                {
                    DateTime date = csv.GetDate("date");
                    double amount = csv.GetDouble("adj close");
                    FinancialSample sample = new FinancialSample();
                    sample.SetAmount(amount);
                    sample.SetDate(date);
                    this.samples.Add(sample);
                }
                csv.Close();
                this.samples.Sort();
            }

            public int size()
            {
                return this.samples.Count;
            }

            public void stitchInterestRates()
            {
                foreach (FinancialSample sample in this.samples)
                {
                    double rate = GetPrimeRate(sample.GetDate());
                    sample.SetRate(rate);
                }
            }


            public class FinancialSample : IComparable<FinancialSample>
            {
                private double amount;
                private double rate;
                private DateTime date;
                private double percent;
                public int CompareTo(FinancialSample other)
                {
                    return GetDate().CompareTo(other.GetDate());
                }
                public double GetAmount()
                {
                    return this.amount;
                }
                public DateTime GetDate()
                {
                    return this.date;
                }
                public double GetPercent()
                {
                    return this.percent;
                }
                public double GetRate()
                {
                    return this.rate;
                }
                public void SetAmount(double amount)
                {
                    this.amount = amount;
                }
                public void SetDate(DateTime date)
                {
                    this.date = date;
                }
                public void SetPercent(double percent)
                {
                    this.percent = percent;
                }
                public void SetRate(double rate)
                {
                    this.rate = rate;
                }
                override public String ToString()
                {
                    StringBuilder result = new StringBuilder();
                    result.Append(ReadCSV.ParseDate(date.ToShortDateString(), "yyyy/mm/dd"));
                    result.Append(", Amount: ");
                    result.Append(this.amount);
                    result.Append(", Prime Rate: ");
                    result.Append(this.rate);
                    result.Append(", Percent from Previous: ");
                    result.Append(this.percent.ToString("N2"));
                    return result.ToString();

                }

            }
        }

    }

}