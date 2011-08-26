//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.Util.Concurrency;
using Encog.Neural.Networks;
using Encog.ML.Data;
using Encog.Neural.Flat;

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
        private int numThreads;

        /// <summary>
        /// The workers.
        /// </summary>
        private ChainRuleWorker[] workers;


        /// <inheritdoc/>
        public override void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {

            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            this.training = theTraining;
            this.network = theNetwork;

            this.hessianMatrix = new Matrix(weightCount, weightCount);
            this.hessian = this.hessianMatrix.Data;

            // create worker(s)
            DetermineWorkload determine = new DetermineWorkload(
                    this.numThreads, (int)this.training.Count);

            this.workers = new ChainRuleWorker[determine.ThreadCount];

            int index = 0;

            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                this.workers[index++] = new ChainRuleWorker((FlatNetwork)this.flat.Clone(),
                        this.training.OpenAdditional(), r.Low,
                        r.High);
            }

        }

        /// <inheritdoc/>
        public override void Compute()
        {
            Clear();
            double e = 0;
            int weightCount = this.network.Flat.Weights.Length;

            for (int outputNeuron = 0; outputNeuron < this.network.OutputCount; outputNeuron++)
            {

                // handle context
                if (this.flat.HasContext)
                {
                    this.workers[0].Network.ClearContext();
                }

                if (this.workers.Length > 1)
                {

                    TaskGroup group = EngineConcurrency.Instance.CreateTaskGroup();

                    foreach (ChainRuleWorker worker in this.workers)
                    {
                        worker.OutputNeuron = outputNeuron;
                        EngineConcurrency.Instance.ProcessTask(worker, group);
                    }

                    group.WaitForComplete();
                }
                else
                {
                    this.workers[0].OutputNeuron = outputNeuron;
                    this.workers[0].Run();
                }

                // aggregate workers

                foreach (ChainRuleWorker worker in this.workers)
                {
                    e += worker.Error;
                    for (int i = 0; i < weightCount; i++)
                    {
                        this.gradients[i] += worker.Gradients[i];
                    }
                    UpdateHessian(worker.Derivative);
                }
            }

            sse = e / 2;
        }

        /// <summary>
        /// Set the number of threads. Specify zero to tell Encog to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        public int ThreadCount
        {
            get
            {
                return this.numThreads;
            }
            set
            {
                this.numThreads = value;
            }
        }
    }
}
