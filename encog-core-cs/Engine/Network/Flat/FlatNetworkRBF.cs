/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Network.Flat
{

    using Encog.Engine.Network.Activation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Util;
    using Encog.Engine.Network.RBF;

    /// <summary>
    /// A flat network designed to handle an RBF.
    /// </summary>
    ///
    public class FlatNetworkRBF : FlatNetwork
    {
        /// <summary>
        /// The RBF's used.
        /// </summary>
        private IRadialBasisFunction[] rbf;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FlatNetworkRBF()
        {

        }

        
        /// <summary>
        /// Construct an RBF flat network. 
        /// </summary>
        /// <param name="inputCount">The number of input neurons. (also the number of dimensions)</param>
        /// <param name="hiddenCount">The number of hidden neurons.</param>
        /// <param name="outputCount">The number of output neurons.</param>
        /// <param name="rbf">The RBF's to use.</param>
        public FlatNetworkRBF(int inputCount, int hiddenCount,
                 int outputCount, IRadialBasisFunction[] rbf)
        {
            this.rbf = rbf;
            FlatLayer[] layers = new FlatLayer[3];

            double[] slope = new double[1];
            slope[0] = 1.0;

            layers[0] = new FlatLayer(new ActivationLinear(), inputCount, 0.0,
                    slope);
            layers[1] = new FlatLayer(new ActivationLinear(), hiddenCount, 1.0,
                    slope);
            layers[2] = new FlatLayer(new ActivationLinear(), outputCount, 0.0,
                    slope);

            Init(layers);
        }

        /// <summary>
        /// Clone the network. 
        /// </summary>
        /// <returns>A clone of the network.</returns>
        public override object Clone()
        {
            FlatNetworkRBF result = new FlatNetworkRBF();
            CloneFlatNetwork(result);
            result.rbf = this.rbf;
            return result;
        }

        /// <summary>
        /// Calculate the output for the given input. 
        /// </summary>
        /// <param name="x">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public override void Compute(double[] x, double[] output)
        {
            int outputIndex = this.LayerIndex[1];

            for (int i = 0; i < rbf.Length; i++)
            {
                double o = this.rbf[i].Calculate(x);
                this.LayerOutput[outputIndex + i] = o;
            }

            // now compute the output
            ComputeLayer(1);
            EngineArray.ArrayCopy(this.LayerOutput, 0, output, 0, this
                    .OutputCount);
        }
    }
}
