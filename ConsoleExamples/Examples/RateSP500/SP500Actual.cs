using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Examples.RateSP500;
using Encog.Util.CSV;


namespace Encog.Examples.RateSP500
{
    public class SP500Actual
    {
        private List<InterestRate> rates = new List<InterestRate>();
        private List<FinancialSample> samples = new List<FinancialSample>();
        private int inputSize;
        private int outputSize;

        public SP500Actual(int inputSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.outputSize = outputSize;
        }

        public void calculatePercents()
        {
            double prev = -1;
            foreach (FinancialSample sample in this.samples)
            {
                if (prev != -1)
                {
                    double movement = sample.getAmount() - prev;
                    double percent = movement / prev;
                    sample.setPercent(percent);
                }
                prev = sample.getAmount();
            }
        }

        public void getInputData(int offset, double[] input)
        {
            Object[] samplesArray = this.samples.ToArray();
            // get SP500 & prime data
            for (int i = 0; i < this.inputSize; i++)
            {
                FinancialSample sample = (FinancialSample)samplesArray[offset
                       + i];
                input[i] = sample.getPercent();
                input[i + this.inputSize] = sample.getRate();
            }
        }

        public void getOutputData(int offset, double[] output)
        {
            Object[] samplesArray = this.samples.ToArray();
            for (int i = 0; i < this.outputSize; i++)
            {
                FinancialSample sample = (FinancialSample)samplesArray[offset
                       + this.inputSize + i];
                output[i] = sample.getPercent();
            }

        }

        public double getPrimeRate(DateTime date)
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

        /**
         * @return the samples
         */
        public IList<FinancialSample> getSamples()
        {
            return this.samples;
        }

        public void load(String sp500Filename, String primeFilename)
        {
            loadSP500(sp500Filename);
            loadPrime(primeFilename);
            stitchInterestRates();
            calculatePercents();
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
            ReadCSV csv = new ReadCSV(sp500Filename, true, CSVFormat.English);
            while (csv.Next())
            {
                DateTime date = csv.GetDate("date");
                double amount = csv.GetDouble("adj close");
                FinancialSample sample = new FinancialSample();
                sample.setAmount(amount);
                sample.setDate(date);
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
                double rate = getPrimeRate(sample.getDate());
                sample.setRate(rate);
            }
        }

    }
}
