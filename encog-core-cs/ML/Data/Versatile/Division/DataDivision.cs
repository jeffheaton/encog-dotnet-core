namespace Encog.ML.Data.Versatile.Division
{
    /// <summary>
    /// A division of data inside of a versatile data set.
    /// </summary>
    public class DataDivision
    {
        /// <summary>
        /// The count of items in this partition.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The percent of items in this partition.
        /// </summary>
        public double Percent { get; private set; }

        /// <summary>
        /// The dataset that we are dividing.
        /// </summary>
        public MatrixMLDataSet Dataset { get; set; }

        /// <summary>
        /// The mask of items we are to use.
        /// </summary>
        public int[] Mask { get; private set; }

        /// <summary>
        /// Construct a division.
        /// </summary>
        /// <param name="thePercent">The desired percentage in this division.</param>
        public DataDivision(double thePercent)
        {
            Percent = thePercent;
        }

        /// <summary>
        /// Allocat space to hold the mask.
        /// </summary>
        /// <param name="theSize">The mask size.</param>
        public void AllocateMask(int theSize)
        {
            Mask = new int[theSize];
        }

    }
}
