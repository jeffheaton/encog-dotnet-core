using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Buffer.CODEC;
using Encog.Engine;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Data.Buffer
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
        private IDataSetCODEC codec;

        /// <summary>
        /// Used to report the status.
        /// </summary>
        private IStatusReportable Status { get; set; }

        /// <summary>
        /// The dataset to load to.
        /// </summary>
        public BasicMLDataSet Result { get; set; }

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            this.codec = codec;
            this.Status = new NullStatusReportable();
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to an Encog memory training set. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public MLDataSet External2Memory()
        {

            Status.Report(0, 0, "Importing to memory");

            if (this.Result == null)
            {
                Result = new BasicMLDataSet();
            }

            double[] input = new double[this.codec.InputSize];
            double[] ideal = new double[this.codec.IdealSize];

            this.codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;

            while (codec.Read(input, ideal))
            {
                MLData a = null, b = null;

                a = new BasicMLData(input);

                if( codec.IdealSize>0 )
                    b = new BasicMLData(ideal);

                MLDataPair pair = new BasicMLDataPair(a,b);
                Result.Add(pair);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    this.Status.Report(0, currentRecord, "Importing...");
                }
            }

            this.codec.Close();
            Status.Report(0, 0, "Done importing to memory");
            return Result;
        }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get
            {
                return codec;
            }
        }

    }
}
