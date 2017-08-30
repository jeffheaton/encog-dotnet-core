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
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
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
            get { return _codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to an Encog memory training set. 
        /// </summary>
        public IMLDataSet External2Memory()
        {
            Status.Report(0, 0, "Importing to memory");

            if (Result == null)
            {
                Result = new BasicMLDataSet();
            }

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 1.0;

            while (_codec.Read(input, ideal, ref significance))
            {
                IMLData b = null;

                IMLData a = new BasicMLData(input);

                if (_codec.IdealSize > 0)
                    b = new BasicMLData(ideal);

                IMLDataPair pair = new BasicMLDataPair(a, b);
                pair.Significance = significance;
                Result.Add(pair);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
            }

            _codec.Close();
            Status.Report(0, 0, "Done importing to memory");
            return Result;
        }
    }
}
