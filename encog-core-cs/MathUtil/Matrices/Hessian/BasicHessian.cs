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
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Util;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    /// Some basic code used to calculate Hessian matrixes.
    /// </summary>
    public abstract class BasicHessian : IComputeHessian
    {
        /// <summary>
        /// The training data that provides the ideal values.
        /// </summary>
        protected IMLDataSet _training;

        /// <summary>
        /// The neural network that we would like to train.
        /// </summary>
        protected BasicNetwork _network;


        /// <summary>
        /// The sum of square error.
        /// </summary>
        protected double _sse;

        /// <summary>
        /// The gradients of the Hessian.
        /// </summary>
        protected double[] _gradients;

        /// <summary>
        /// The Hessian matrix.
        /// </summary>
        protected Matrix _hessianMatrix;

        /// <summary>
        /// The Hessian 2d array.
        /// </summary>
        protected double[][] _hessian;

        /// <summary>
        /// The flat network.
        /// </summary>
        protected FlatNetwork _flat;

        /// <inheritdoc/>
        public virtual void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {

            int weightCount = theNetwork.Structure.Flat.Weights.Length;
            _flat = theNetwork.Flat;
            _training = theTraining;
            _network = theNetwork;
            _gradients = new double[weightCount];
            _hessianMatrix = new Matrix(weightCount, weightCount);
            _hessian = _hessianMatrix.Data;
        }

        /// <inheritdoc/>
        public abstract void Compute();

        /// <inheritdoc/>
        public double[] Gradients
        {
            get
            {
                return _gradients;
            }
        }

        /// <inheritdoc/>
        public Matrix HessianMatrix
        {
            get
            {
                return _hessianMatrix;
            }
        }

        /// <inheritdoc/>
        public double[][] Hessian
        {
            get
            {
                return _hessian;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            EngineArray.Fill(_gradients, 0);
            _hessianMatrix.Clear();
        }

        /// <inheritdoc/>
        public double SSE
        {
            get
            {
                return _sse;
            }
        }

        /// <summary>
        /// Update the Hessian, sum's with what is in the Hessian already.  
        /// Call clear to clear out old Hessian.
        /// </summary>
        /// <param name="d">The values to sum into the current Hessian</param>
        public void UpdateHessian(double[] d)
        {
            // update the hessian
            int weightCount = _network.Flat.Weights.Length;
            for (int i = 0; i < weightCount; i++)
            {
                for (int j = 0; j < weightCount; j++)
                {
                    _hessian[i][j] += d[i] * d[j];
                }
            }
        }
    }
}
