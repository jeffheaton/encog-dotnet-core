using Encog.App.Analyst.CSV.Basic;
using Encog.ML.Data.Basic;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    /// Holds input data and the CSV row for a cluster item.
    /// </summary>
    ///
    public class ClusterRow : BasicMLDataPair
    {

        /// <summary>
        /// The loaded row of data.
        /// </summary>
        ///
        private readonly LoadedRow row;

        /// <summary>
        /// Construct the cluster row.
        /// </summary>
        ///
        /// <param name="input">The input data.</param>
        /// <param name="theRow">The CSV row.</param>
        public ClusterRow(double[] input, LoadedRow theRow) : base(new BasicMLData(input))
        {
            row = theRow;
        }


        /// <value>the row</value>
        public LoadedRow Row
        {
            /// <returns>the row</returns>
            get { return row; }
        }
    }
}