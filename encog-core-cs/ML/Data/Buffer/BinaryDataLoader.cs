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
using Encog.ML.Data.Buffer.CODEC;

namespace Encog.ML.Data.Buffer
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
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public BinaryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        public IStatusReportable Status { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return _codec; }
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

            var egb = new EncogEGBFile(binaryFile);

            egb.Create(_codec.InputSize, _codec.IdealSize);

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 0;

            while (_codec.Read(input, ideal, ref significance))
            {
                egb.Write(input);
                egb.Write(ideal);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
                egb.Write(significance);
            }

            egb.Close();
            _codec.Close();
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

            var egb = new EncogEGBFile(binaryFile);
            egb.Open();

            _codec.PrepareWrite(egb.NumberOfRecords, egb.InputCount,
                               egb.IdealCount);

            int inputCount = egb.InputCount;
            int idealCount = egb.IdealCount;

            var input = new double[inputCount];
            var ideal = new double[idealCount];

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

                double significance = egb.Read();

                _codec.Write(input, ideal, significance);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(egb.NumberOfRecords, currentRecord,
                                  "Exporting...");
                }
            }

            egb.Close();
            _codec.Close();
            Status.Report(0, 0, "Done exporting binary file: "
                                + binaryFile);
        }
    }
}
