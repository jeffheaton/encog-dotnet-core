using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.ML.Bayesian.BIF
{
    /// <summary>
    /// Holds a BIF definition.
    /// </summary>
    public class BIFDefinition
    {
        /// <summary>
        /// The "for" definition.
        /// </summary>
        public String ForDefinition { get; set; }

        /// <summary>
        /// Given definitions.
        /// </summary>
        private IList<String> givenDefinitions = new List<String>();

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        private double[] table;

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        public double[] Table
        {
            get
            {
                return table;
            }
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
            this.table = new double[list.Count];
            for (int i = 0; i < this.table.Length; i++)
            {
                this.table[i] = list[i];
            }
        }

        /// <summary>
        /// The given defintions.
        /// </summary>
        public IList<String> GivenDefinitions
        {
            get
            {
                return givenDefinitions;
            }
        }

        /// <summary>
        /// Add a given.
        /// </summary>
        /// <param name="s">The given to add.</param>
        public void AddGiven(String s)
        {
            this.givenDefinitions.Add(s);

        }

    }
}
