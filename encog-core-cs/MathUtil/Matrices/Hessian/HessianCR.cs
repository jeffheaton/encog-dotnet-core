//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using Encog.Util.Concurrency;
using System.Threading.Tasks;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    /// Calculate the Hessian matrix using the chain rule method. 
    /// </summary>
    public class HessianCR : BasicHessian, IMultiThreadable
    {
        /// <summary>
        /// The number of threads to use.
        /// </summary>
        private int _numThreads;

        /// <summary>
        /// The workers.
        /// </summary>
        private ChainRuleWorker[] _workers;

        #region IMultiThreadable Members

        /// <summary>
        /// Set the number of threads. Specify zero to tell Encog to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        public int ThreadCount
        {
            get { return _numThreads; }
            set { _numThreads = value; }
        }

        #endregion

        /// <inheritdoc/>
        public override void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            training = theTraining;
            network = theNetwork;

            hessianMatrix = new Matrix(weightCount, weightCount);
            hessian = hessianMatrix.Data;

            // create worker(s)
            var determine = new DetermineWorkload(
                _numThreads, (int) training.Count);

            _workers = new ChainRuleWorker[determine.ThreadCount];

            int index = 0;

            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                _workers[index++] = new ChainRuleWorker((FlatNetwork) flat.Clone(),
                                                       training.OpenAdditional(), r.Low,
                                                       r.High);
            }
        }

        /// <inheritdoc/>
        public override void Compute()
        {
            Clear();
            double e = 0;
            int weightCount = network.Flat.Weights.Length;

            for (int outputNeuron = 0; outputNeuron < network.OutputCount; outputNeuron++)
            {
                // handle context
                if (flat.HasContext)
                {
                    _workers[0].Network.ClearContext();
                }

                if (_workers.Length > 1)
                {
                    Parallel.ForEach(_workers, worker => 
                    {
                        worker.Run();
                        worker.OutputNeuron = outputNeuron;
                    });                   
                }
                else
                {
                    _workers[0].OutputNeuron = outputNeuron;
                    _workers[0].Run();
                }

                // aggregate workers

                foreach (ChainRuleWorker worker in _workers)
                {
                    e += worker.Error;
                    for (int i = 0; i < weightCount; i++)
                    {
                        gradients[i] += worker.Gradients[i];
                    }

                    EngineArray.ArrayAdd(Hessian, worker.Hessian);
                }
            }

            sse = e/2;
        }
    }
}
