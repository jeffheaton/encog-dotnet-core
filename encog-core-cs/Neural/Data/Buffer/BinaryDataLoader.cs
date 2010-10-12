using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Buffer.CODEC;

namespace Encog.Neural.Data.Buffer
{
    /// <summary>
    /// This class is used, together with a CODEC, to move data to/from the Encog
    /// binary training file format. The same Encog binary files can be used on all
    /// Encog platforms. CODEC's are used to import/export with other formats, such
    /// as CSV.
    /// </summary>
    public class BinaryDataLoader
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
        public BinaryDataLoader(IDataSetCODEC codec)
        {
            this.codec = codec;
            this.Status = new NullStatusReportable();
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to the Encog binary
        /// training format. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public void External2Binary(String binaryFile)
        {

            Status.Report(0, 0, "Importing to binary file: "
                    + binaryFile);

            EncogEGBFile egb = new EncogEGBFile(binaryFile);

            egb.Create(codec.InputSize, codec.IdealSize);

            double[] input = new double[this.codec.InputSize];
            double[] ideal = new double[this.codec.IdealSize];

            this.codec.PrepareRead();

            int index = 3;
            int currentRecord = 0;
            int lastUpdate = 0;

            while (codec.Read(input, ideal))
            {

                egb.Write(input);
                egb.Write(ideal);

                index += input.Length;
                index += ideal.Length;
                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    this.Status.Report(0, currentRecord, "Importing...");
                }
            }

            egb.Close();
            this.codec.Close();
            Status.Report(0, 0, "Done importing to binary file: "
                    + binaryFile);

        }

        /// <summary>
        /// Convert an Encog binary file to an external form, such as CSV. 
        /// </summary>
        /// <param name="binaryFile">THe binary file to use.</param>
        public void Binary2External(String binaryFile)
        {
            Status.Report(0, 0, "Exporting binary file: " + binaryFile);

            EncogEGBFile egb = new EncogEGBFile(binaryFile);
            egb.Open();

            this.codec.PrepareWrite(egb.NumberOfRecords, egb.InputCount,
                    egb.IdealCount);

            int inputCount = egb.InputCount;
            int idealCount = egb.IdealCount;

            double[] input = new double[inputCount];
            double[] ideal = new double[idealCount];

            int currentRecord = 0;
            int lastUpdate = 0;

            // now load the data
            for (int i = 0; i < egb.NumberOfRecords; i++)
            {

                for (int j = 0; j < inputCount; j++)
                {
                    input[j] = egb.Read();
                }

                for (int j = 0; j < idealCount; j++)
                {
                    ideal[j] = egb.Read();
                }

                this.codec.Write(input, ideal);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    this.Status.Report(egb.NumberOfRecords, currentRecord,
                            "Exporting...");
                }

            }

            egb.Close();
            this.codec.Close();
            Status.Report(0, 0, "Done exporting binary file: "
                    + binaryFile);
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
