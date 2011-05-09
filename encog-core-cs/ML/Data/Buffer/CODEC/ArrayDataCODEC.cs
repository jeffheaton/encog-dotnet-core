using Encog.Util;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC used for arrays.
    /// </summary>
    public class ArrayDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The current index.
        /// </summary>
        private int index;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The input array.
        /// </summary>
        private double[][] input;

        /// <summary>
        /// The ideal array.
        /// </summary>
        private double[][] ideal;
        
        /// <summary>
        /// Construct an array CODEC. 
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="ideal">The ideal array.</param>
        public ArrayDataCODEC(double[][] input, double[][] ideal)
        {
            this.input = input;
            this.ideal = ideal;
            this.inputSize = input[0].Length;
            this.idealSize = ideal[0].Length;
            this.index = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArrayDataCODEC()
        {
        }

        /// <inheritdoc/>
        public int InputSize
        {
            get
            {
                return inputSize;
            }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get
            {
                return idealSize;
            }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal)
        {
            if (index >= this.input.Length)
            {
                return false;
            }
            else
            {
                EngineArray.ArrayCopy(this.input[index], input);
                EngineArray.ArrayCopy(this.ideal[index], ideal);
                index++;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal)
        {
            EngineArray.ArrayCopy(input, this.input[index]);
            EngineArray.ArrayCopy(ideal, this.ideal[index]);
            index++;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
            int inputSize, int idealSize)
        {
            this.input = EngineArray.AllocateDouble2D(recordCount, inputSize);
            this.ideal = EngineArray.AllocateDouble2D(recordCount, idealSize);
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.index = 0;
        }

        /// <inheritdoc/>
        public double[][] Input
        {
            get
            {
                return input;
            }
        }

        /// <inheritdoc/>
        public double[][] Ideal
        {
            get
            {
                return ideal;
            }
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {

        }

        /// <inheritdoc/>
        public void Close()
        {

        }
    }
}
