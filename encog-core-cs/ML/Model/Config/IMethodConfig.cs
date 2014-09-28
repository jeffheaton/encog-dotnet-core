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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Normalizers.Strategy;

namespace Encog.ML.Model.Config
{
    /// <summary>
    /// Define normalization for a specific method.
    /// </summary>
    public interface IMethodConfig
    {
        /// <summary>
        /// The method name.
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Suggest a model architecture, based on a dataset. 
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>The model architecture.</returns>
        string SuggestModelArchitecture(VersatileMLDataSet dataset);

        /// <summary>
        /// Suggest a normalization strategy based on a dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="architecture">The architecture.</param>
        /// <returns>The strategy.</returns>
        INormalizationStrategy SuggestNormalizationStrategy(VersatileMLDataSet dataset, string architecture);

        /// <summary>
        /// Suggest a training type. 
        /// </summary>
        /// <returns>The training type.</returns>
        string SuggestTrainingType();

        /// <summary>
        ///  Suggest training arguments. 
        /// </summary>
        /// <param name="trainingType">The training type.</param>
        /// <returns>The training arguments.</returns>
        string SuggestTrainingArgs(string trainingType);

        /// <summary>
        /// Determine the needed output count.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>The needed output count.</returns>
        int DetermineOutputCount(VersatileMLDataSet dataset);
    }
}
