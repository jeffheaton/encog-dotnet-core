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
#region

using System;
using System.Collections.Generic;
using Encog.Util.CSV;

#endregion

namespace Encog.Examples.RateSP500
{
    public class SP500Actual
    {
        private readonly int inputSize;
        private readonly int outputSize;
        private readonly List<InterestRate> rates = new List<InterestRate>();
        private readonly List<FinancialSample> samples = new List<FinancialSample>();

        public SP500Actual(int inputSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.outputSize = outputSize;
        }

        public void calculatePercents()
        {
            double prev = -1;
            foreach (FinancialSample sample in samples)
            {
                if (prev != -1)
                {
                    double movement = sample.getAmount() - prev;
                    double percent = movement/prev;
                    sample.setPercent(percent);
                }
                prev = sample.getAmount();
            }
        }

        public void getInputData(int offset, double[] input)
        {
            Object[] samplesArray = samples.ToArray();
            // get SP500 & prime data
            for (int i = 0; i < inputSize; i++)
            {
                var sample = (FinancialSample) samplesArray[offset
                                                            + i];
                input[i] = sample.getPercent();
                input[i + inputSize] = sample.getRate();
            }
        }

        public void getOutputData(int offset, double[] output)
        {
            Object[] samplesArray = samples.ToArray();
            for (int i = 0; i < outputSize; i++)
            {
                var sample = (FinancialSample) samplesArray[offset
                                                            + inputSize + i];
                output[i] = sample.getPercent();
            }
        }

        public double getPrimeRate(DateTime date)
        {
            double currentRate = 0;

            foreach (InterestRate rate in rates)
            {
                if (rate.getEffectiveDate().CompareTo(date) > 0)
                {
                    return currentRate;
                }
                currentRate = rate.getRate();
            }
            return currentRate;
        }

        /**
         * @return the samples
         */

        public IList<FinancialSample> getSamples()
        {
            return samples;
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
            var csv = new ReadCSV(primeFilename, true, CSVFormat.English);

            while (csv.Next())
            {
                var date = csv.GetDate("date");
                double rate = csv.GetDouble("prime");
                var ir = new InterestRate(date, rate);
                rates.Add(ir);
            }

            csv.Close();
            rates.Sort();
        }

        public void loadSP500(String sp500Filename)
        {
            var csv = new ReadCSV(sp500Filename, true, CSVFormat.English);
            while (csv.Next())
            {
                var date = csv.GetDate("date");
                double amount = csv.GetDouble("adj close");
                var sample = new FinancialSample();
                sample.setAmount(amount);
                sample.setDate(date);
                samples.Add(sample);
            }
            csv.Close();
            samples.Sort();
        }

        public int size()
        {
            return samples.Count;
        }

        public void stitchInterestRates()
        {
            foreach (FinancialSample sample in samples)
            {
                double rate = getPrimeRate(sample.getDate());
                sample.setRate(rate);
            }
        }
    }
}
