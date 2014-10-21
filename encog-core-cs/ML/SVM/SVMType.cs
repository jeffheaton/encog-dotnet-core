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
namespace Encog.ML.SVM
{
    /// <summary>
    /// Supports both class and new support vector calculations, as well as one-class
    /// distribution.
    /// For more information about the two "new" support vector types, as well as the
    /// one-class SVM, refer to the following articles.
    /// B. Sch?lkopf, A. Smola, R. Williamson, and P. L. Bartlett. New support vector
    /// algorithms. Neural Computation, 12, 2000, 1207-1245.
    /// B. Sch?lkopf, J. Platt, J. Shawe-Taylor, A. J. Smola, and R. C. Williamson.
    /// Estimating the support of a high-dimensional distribution. Neural
    /// Computation, 13, 2001, 1443-1471.
    /// </summary>
    ///
    public enum SVMType
    {
        /// <summary>
        /// Support vector for classification.
        /// </summary>
        ///
        SupportVectorClassification,

        /// <summary>
        /// New support vector for classification. For more information see the
        /// citations in the class header.
        /// </summary>
        ///
        NewSupportVectorClassification,

        /// <summary>
        /// One class distribution estimation.
        /// </summary>
        ///
        SupportVectorOneClass,

        /// <summary>
        /// Support vector for regression. Use Epsilon.
        /// </summary>
        ///
        EpsilonSupportVectorRegression,

        /// <summary>
        /// A "new" support vector machine for regression. For more information see
        /// the citations in the class header.
        /// </summary>
        ///
        NewSupportVectorRegression
    }
}
