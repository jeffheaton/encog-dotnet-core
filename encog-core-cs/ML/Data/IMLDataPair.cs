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
using Encog.Util.KMeans;

namespace Encog.ML.Data
{
    /// <summary>
    /// A neural data pair holds both the input and ideal data.  If this
    /// is an unsupervised data element, then only input is provided.
    /// </summary>
    public interface IMLDataPair : ICloneable, ICentroidFactory<IMLDataPair>
    {
        /// <summary>
        /// The input that the neural network.
        /// </summary>
        IMLData Input { get; }

        /// <summary>
        /// The ideal data that the neural network should produce
        /// for the specified input.
        /// </summary>
        IMLData Ideal { get; }

        /// <summary>
        /// True if this training pair is supervised.  That is, it has 
        /// both input and ideal data.
        /// </summary>
        bool Supervised { get; }

        /// <summary>
        /// The significance of this training element.
        /// </summary>
        double Significance { get; set; }
    }
}
