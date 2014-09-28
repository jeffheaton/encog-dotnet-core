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
using Encog.ML.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Used to map one neural data object to another. Useful for a BAM network.
    /// </summary>
    ///
    public class NeuralDataMapping
    {
        /// <summary>
        /// Construct the neural data mapping class, with null values.
        /// </summary>
        ///
        public NeuralDataMapping()
        {
            From = null;
            To = null;
        }

        /// <summary>
        /// Construct the neural data mapping class with the specified values.
        /// </summary>
        ///
        /// <param name="f">The source data.</param>
        /// <param name="t">The target data.</param>
		public NeuralDataMapping(IMLDataModifiable f, IMLDataModifiable t)
        {
            From = f;
            To = t;
        }

        /// <summary>
        /// Set the from data.
        /// </summary>
        ///
        /// <value>The from data.</value>
		public IMLDataModifiable From
		{ 
            get;
            set; 
		}

        /// <summary>
        /// Set the target data.
        /// </summary>
        ///
        /// <value>The target data.</value>
		public IMLDataModifiable To
		{ 
            get; 
            set; 
		}

        /// <summary>
        /// Copy from one object to the other.
        /// </summary>
        ///
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public static void Copy(NeuralDataMapping source,
                                NeuralDataMapping target)
        {
            for (int i = 0; i < source.From.Count; i++)
            {
                target.From[i] = source.From[i];
            }

            for (int i_0 = 0; i_0 < source.To.Count; i_0++)
            {
                target.To[i_0] = source.To[i_0];
            }
        }
    }
}
