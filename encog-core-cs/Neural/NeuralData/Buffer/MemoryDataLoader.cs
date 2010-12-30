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
    /// This class is used, together with a CODEC, to move data to/from the Encog
    /// binary training file format. The same Encog binary files can be used on all
    /// Encog platforms. CODEC's are used to import/export with other formats, such
    /// as CSV.
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
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            this.codec = codec;
            this.Status = new NullStatusReportable();
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to the Encog binary
        /// training format. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public INeuralDataSet External2Memory()
        {

            Status.Report(0, 0, "Importing to memory");

            BasicNeuralDataSet result = new BasicNeuralDataSet();

            double[] input = new double[this.codec.InputSize];
            double[] ideal = new double[this.codec.IdealSize];

            this.codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;

            while (codec.Read(input, ideal))
            {
                INeuralData a = null, b = null;

                a = new BasicNeuralData(input);

                if( codec.IdealSize>0 )
                    b = new BasicNeuralData(ideal);

                INeuralDataPair pair = new BasicNeuralDataPair(a,b);
                result.Add(pair);

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
            return result;
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
