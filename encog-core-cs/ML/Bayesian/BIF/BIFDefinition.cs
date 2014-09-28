//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.Util.CSV;

namespace Encog.ML.Bayesian.BIF
{
    /// <summary>
    /// Holds a BIF definition.
    /// </summary>
    public class BIFDefinition
    {
        /// <summary>
        /// Given definitions.
        /// </summary>
        private readonly IList<String> _givenDefinitions = new List<String>();

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        private double[] _table;

        /// <summary>
        /// The "for" definition.
        /// </summary>
        public String ForDefinition { get; set; }

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        public double[] Table
        {
            get { return _table; }
        }

        /// <summary>
        /// The given defintions.
        /// </summary>
        public IList<String> GivenDefinitions
        {
            get { return _givenDefinitions; }
        }

        /// <summary>
        /// Set the probabilities as a string.
        /// </summary>
        /// <param name="s">A space separated string.</param>
        public void SetTable(String s)
        {
            // parse a space separated list of numbers
            String[] tok = s.Split(' ');
            IList<Double> list = new List<Double>();
            foreach (String str in tok)
            {
                // support both radix formats
                if (str.IndexOf(",") != -1)
                {
                    list.Add(CSVFormat.DecimalComma.Parse(str));
                }
                else
                {
                    list.Add(CSVFormat.DecimalComma.Parse(str));
                }
            }

            // now copy to regular array
            _table = new double[list.Count];
            for (int i = 0; i < _table.Length; i++)
            {
                _table[i] = list[i];
            }
        }

        /// <summary>
        /// Add a given.
        /// </summary>
        /// <param name="s">The given to add.</param>
        public void AddGiven(String s)
        {
            _givenDefinitions.Add(s);
        }
    }
}
