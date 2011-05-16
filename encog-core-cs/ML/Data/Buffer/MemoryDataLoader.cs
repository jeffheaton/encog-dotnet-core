using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer.CODEC;

namespace Encog.ML.Data.Buffer
{
    /// <summary>
    /// This class is used, together with a CODEC, load training data from some 
    /// external file into an Encog memory-based training set.  
    /// </summary>
    public class MemoryDataLoader
    {
        /// <summary>
        /// The CODEC to use.
        /// </summary>
        private readonly IDataSetCODEC codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            this.codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        private IStatusReportable Status { get; set; }

        /// <summary>
        /// The dataset to load to.
        /// </summary>
        public BasicMLDataSet Result { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to an Encog memory training set. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public MLDataSet External2Memory()
        {
            Status.Report(0, 0, "Importing to memory");

            if (Result == null)
            {
                Result = new BasicMLDataSet();
            }

            var input = new double[codec.InputSize];
            var ideal = new double[codec.IdealSize];

            codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;

            while (codec.Read(input, ideal))
            {
                MLData a = null, b = null;

                a = new BasicMLData(input);

                if (codec.IdealSize > 0)
                    b = new BasicMLData(ideal);

                MLDataPair pair = new BasicMLDataPair(a, b);
                Result.Add(pair);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
            }

            codec.Close();
            Status.Report(0, 0, "Done importing to memory");
            return Result;
        }
    }
}