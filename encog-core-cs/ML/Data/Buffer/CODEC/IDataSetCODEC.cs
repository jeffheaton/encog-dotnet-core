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
namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC is used to encode and decode data. The DataSetCODEC is designed to
    /// move data to/from the Encog binary training file format, used by
    /// BufferedMlDataSet. CODECs are provided for such items as CSV files,
    /// arrays and many other sources.
    /// </summary>
    public interface IDataSetCODEC
    {
        /// <summary>
        /// The size of the input data.
        /// </summary>
        int InputSize { get; }

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        int IdealSize { get; }

        /// <summary>
        /// Read one record of data from an external source.
        /// </summary>
        /// <param name="input">The input data array.</param>
        /// <param name="ideal">The ideal data array.</param>
        /// <param name="significance">The signficance. (by reff)</param>
        /// <returns>True, if there is more data to be read.</returns>
        bool Read(double[] input, double[] ideal, ref double significance);

        /// <summary>
        /// Write one record of data to an external destination. 
        /// </summary>
        /// <param name="input">The input data array.</param>
        /// <param name="ideal">The ideal data array.</param>
        /// <returns>True, if there is more data to be read.</returns>
        void Write(double[] input, double[] ideal, double significance);

        /// <summary>
        /// Prepare to write to an external data destination. 
        /// </summary>
        /// <param name="recordCount">The total record count, that will be written.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        void PrepareWrite(int recordCount, int inputSize, int idealSize);

        /// <summary>
        /// Prepare to read from an external data source.
        /// </summary>
        void PrepareRead();

        /// <summary>
        /// Close any open files.
        /// </summary>
        void Close();
    }
}
