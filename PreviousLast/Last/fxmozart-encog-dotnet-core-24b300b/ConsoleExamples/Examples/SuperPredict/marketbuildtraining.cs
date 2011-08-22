using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.NetworkUtil;
using Encog.Util.Simple;
using NUnit.Framework;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;

namespace Encog.Examples.SuperPredict
{

    public class MarketBuildTraining
    {
        public static void Generate()
        {




            GetStuffs();


            //EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Examples.RangeCalculators.RangeConfig.TRAINING_FILE), market);

            //// create a network



            //Console.WriteLine(@"Built training file , and saved it to:" + dataDir.DirectoryName);

          //// train the neural network
            ////  EncogUtility.TrainConsole(network, trainingSet, RangeConfig.TRAINING_MINUTES);
            //NetworkBuilders.DualNetworksTrainer(market.InputNeuronCount, market.IdealSize,
            //                                    Examples.RangeCalculators.RangeConfig.HIDDEN1_COUNT, Examples.RangeCalculators.RangeConfig.HIDDEN2_COUNT, market,
            //                                    Examples.RangeCalculators.RangeConfig.NETWORK_FILE);


        }

   

        public static void GetStuffs()
        {
            string fileName = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
            List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open",1200);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High",1200);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low",1200);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close",1200);

            double[] Opensd = new double[Opens.Count];
            double[] Closed = new double[Close.Count];
            double[] Highd = new double[High.Count];
            double[] Lowd = new double[Low.Count];
            Encog.Util.Arrayutil.NormalizeArray ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();



            List<double> NormedOpened = SuperUtils.CopydoubleArrayToList(ArrayNormalizer.Process(Opens.ToArray()));
            ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();
            List<double> NormedHigh = SuperUtils.CopydoubleArrayToList(ArrayNormalizer.Process(High.ToArray()));
            ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();
            List<double> NormedLow = SuperUtils.CopydoubleArrayToList(ArrayNormalizer.Process(Low.ToArray()));
            ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();
            List<double> NormedClose = SuperUtils.CopydoubleArrayToList(ArrayNormalizer.Process(Close.ToArray()));
            ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();




            double[][] idealdb = CreateIdealOrInput(Closed.Length, NormedClose.ToArray(), NormedClose.ToArray(), NormedClose.ToArray(), NormedClose.ToArray());
            double[][] inputsz = CreateIdealOrInput(Closed.Length, NormedHigh.ToArray(), NormedLow.ToArray(), NormedClose.ToArray(), NormedOpened.ToArray());
            var x = GenerateTraining(inputsz, idealdb);

           // BasicNetwork mynetwork = (BasicNetwork) CreateElmanNetwork(4, 1, 6);
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 4));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationLinear(), false, 4));
            network.Structure.FinalizeStructure();
            network.Reset();


            EncogUtility.TrainConsole(network, x, 1);

        }


        /// <summary>
        /// Creates the elman network.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="outputs">The outputs.</param>
        /// <param name="hiddenlayers">The hiddenlayers.</param>
        /// <returns></returns>
       private static IMLMethod CreateElmanNetwork(int inputs, int outputs, int hiddenlayers)
        {
            // construct an Elman type network
            var pattern = new ElmanPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = inputs;
            pattern.AddHiddenLayer(hiddenlayers);
           pattern.OutputNeurons = outputs;
            return pattern.Generate();
        }

        public static TemporalMLDataSet AddToTraining(int inputsize, int outputsize,IActivationFunction fc, bool input, bool predict , TemporalDataDescription.Type aType ,double[] Arraydouble)
        {
            TemporalMLDataSet result = new TemporalMLDataSet(inputsize, outputsize);
            TemporalDataDescription desc = new TemporalDataDescription(fc,aType, input,predict);
            result.AddDescription(desc);
            for (int year = 0; year < Arraydouble.Length; year++)
            {
                TemporalPoint point = new TemporalPoint(1);
                point.Sequence = year;
                point.Data[0] = Arraydouble[year];
                result.Points.Add(point);
            }
        
            return result;
        }





        public static double [][] CreateIdealOrInput(int nbofSeCondDimendsion ,params object[] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, nbofSeCondDimendsion);
            int i = 0, k = 0;
                foreach (double[] doubles in inputs)
                {
                    foreach (double d in doubles)
                    {
                        result[i][k] = d;
                        k++;
                    }
                    if (i < inputs.Length -1)
                    {
                        i++;
                        k = 0;
                    }
                }
            return result;
        }

        public IMLDataSet Generate(int count, double [][]input,double [][]ideal)
        {
            return new BasicMLDataSet(input, ideal);
        }
        public static IMLDataSet GenerateTraining(double[][] input, double[][] ideal)
        {
            var result = new BasicMLDataSet(input, ideal);
            return result;
        }

     
        public static double [] CreateNormalizatione(double [] input)
        {
            double [] results = NetworkUtility.NormalizeThisArray(input);
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