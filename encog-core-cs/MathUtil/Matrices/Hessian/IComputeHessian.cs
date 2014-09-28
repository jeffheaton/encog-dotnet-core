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
using Encog.Neural.Networks;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    ///  Compute (estimate) the Hessian matrix. The Hessian matrix is a matrix of the second
    /// derivatives of the neural network. This is a square matrix with rows and columns
    /// equal to the number of weights in the neural network.
    /// 
    /// A Hessian matrix is useful for several neural network functions.  It is also used
    /// by the Levenberg Marquardt training method. 
    /// 
    /// http://en.wikipedia.org/wiki/Hessian_matrix
    /// </summary>
    public interface IComputeHessian
    {
        /// <summary>
        /// The gradeints. 
        /// </summary>
        double[] Gradients { get; }

        /// <summary>
        /// The sum of squares error over all of the training elements.
        /// </summary>
        double SSE { get; }

        /// <summary>
        /// The Hessian matrix.
        /// </summary>
        Matrix HessianMatrix { get; }

        /// <summary>
        /// Get the Hessian as a 2d array.
        /// </summary>
        double[][] Hessian { get; }

        /// <summary>
        /// Init the class.  
        /// </summary>
        /// <param name="theNetwork">The neural network to train.</param>
        /// <param name="theTraining">The training set to train with.</param>
        void Init(BasicNetwork theNetwork, IMLDataSet theTraining);

        /// <summary>
        /// Compute the Hessian.
        /// </summary>
        void Compute();

        /// <summary>
        /// Clear the Hessian and gradients.
        /// </summary>
        void Clear();
    }
}
