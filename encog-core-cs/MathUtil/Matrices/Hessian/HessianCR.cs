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
using System.Threading.Tasks;
using Encog.ML.Data;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Util;
using Encog.Util.Concurrency;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    ///     Calculate the Hessian matrix using the chain rule method.
    /// </summary>
    public class HessianCR : BasicHessian, IMultiThreadable
    {
        /// <summary>
        ///     The workers.
        /// </summary>
        private ChainRuleWorker[] _workers;

        /// <summary>
        ///     The number of threads to use.
        /// </summary>
        public int ThreadCount { get; set; }


        /// <inheritdoc />
        public override void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            _training = theTraining;
            _network = theNetwork;

            _hessianMatrix = new Matrix(weightCount, weightCount);
            _hessian = _hessianMatrix.Data;

            // create worker(s)
            var determine = new DetermineWorkload(
                ThreadCount, _training.Count);

            _workers = new ChainRuleWorker[determine.ThreadCount];

            int index = 0;

            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                _workers[index++] = new ChainRuleWorker((FlatNetwork) _flat.Clone(),
                    _training.OpenAdditional(), r.Low,
                    r.High);
            }
        }

        /// <inheritdoc />
        public override void Compute()
        {
            Clear();
            double e = 0;
            int weightCount = _network.Flat.Weights.Length;

            for (int outputNeuron = 0; outputNeuron < _network.OutputCount; outputNeuron++)
            {
                // handle context
                if (_flat.HasContext)
                {
                    _workers[0].Network.ClearContext();
                }

                int neuron = outputNeuron;
                Parallel.ForEach(_workers, worker =>
                {
                    worker.OutputNeuron = neuron;
                    worker.Run();
                });


                // aggregate workers

                foreach (ChainRuleWorker worker in _workers)
                {
                    e += worker.Error;
                    for (int i = 0; i < weightCount; i++)
                    {
                        _gradients[i] += worker.Gradients[i];
                    }
                    EngineArray.ArrayAdd(Hessian, worker.Hessian);
                }
            }

            _sse = e/2;
        }
    }
}
