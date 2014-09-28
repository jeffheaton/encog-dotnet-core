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
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// A dataset based on a SQL query. This is not a memory based dataset, so it can
    /// handle very large datasets without a memory issue. and can handle very large
    /// datasets.
    /// </summary>
    public class SQLMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLMLDataSet(String sql, int inputSize,
                            int idealSize, String connectString)
        {
            IDataSetCODEC codec = new SQLCODEC(sql, inputSize, idealSize, connectString);
            var load = new MemoryDataLoader(codec) {Result = this};
            load.External2Memory();
        }
    }
}
