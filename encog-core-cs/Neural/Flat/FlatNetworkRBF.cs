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
using Encog.Engine.Network.Activation;
using Encog.MathUtil.RBF;
using Encog.Util;

namespace Encog.Neural.Flat
{
    /// <summary>
    /// A flat network designed to handle an RBF.
    /// </summary>
    ///
    [Serializable]
    public class FlatNetworkRBF : FlatNetwork
    {
        /// <summary>
        /// The RBF's used.
        /// </summary>
        ///
        private IRadialBasisFunction[] _rbf;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public FlatNetworkRBF()
        {
        }

        /// <summary>
        /// Construct an RBF flat network.
        /// </summary>
        ///
        /// <param name="inputCount">The number of input neurons. (also the number of dimensions)</param>
        /// <param name="hiddenCount">The number of hidden neurons.</param>
        /// <param name="outputCount">The number of output neurons.</param>
        /// <param name="rbf"></param>
        public FlatNetworkRBF(int inputCount, int hiddenCount,
                              int outputCount, IRadialBasisFunction[] rbf)
        {
            var layers = new FlatLayer[3];
            _rbf = rbf;

            layers[0] = new FlatLayer(new ActivationLinear(), inputCount, 0.0d);
            layers[1] = new FlatLayer(new ActivationLinear(), hiddenCount, 0.0d);
            layers[2] = new FlatLayer(new ActivationLinear(), outputCount, 0.0d);

            Init(layers);
        }

        /// <summary>
        /// Set the RBF's used.
        /// </summary>
        public IRadialBasisFunction[] RBF
        {
            get { return _rbf; }
            set { _rbf = value; }
        }

        /// <summary>
        /// Clone the network.
        /// </summary>
        ///
        /// <returns>A clone of the network.</returns>
        public override sealed Object Clone()
        {
            var result = new FlatNetworkRBF();
            CloneFlatNetwork(result);
            result._rbf = _rbf;
            return result;
        }

        /// <summary>
        /// Calculate the output for the given input.
        /// </summary>
        ///
        /// <param name="x">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public override sealed void Compute(double[] x, double[] output)
        {
            int outputIndex = LayerIndex[1];

            for (int i = 0; i < _rbf.Length; i++)
            {
                double o = _rbf[i].Calculate(x);
                LayerOutput[outputIndex + i] = o;
            }

            // now compute the output
            ComputeLayer(1);
            EngineArray.ArrayCopy(LayerOutput, 0, output, 0,
                                  OutputCount);
        }

		public override void Compute(ML.Data.IMLData input, double[] output)
		{
			if(input is ML.Data.Basic.BasicMLData)
				Compute(((ML.Data.Basic.BasicMLData)input).Data, output);
			else
			{
				// TODO: make this more efficient
				var tmp = new double[input.Count];
				input.CopyTo(tmp, 0, input.Count);
				Compute(tmp, output);
			}
		}
    }
}
