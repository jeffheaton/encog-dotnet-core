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
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Training.PSO
{
    /// <summary>
    /// PSO multi-treaded worker.
    /// It allows PSO to offload all of the individual 
    /// particle calculations to a separate thread.
    /// 
    /// Contributed by:
    /// Geoffroy Noel
    /// https://github.com/goffer-looney 
    /// 
    /// </summary>
    public class NeuralPSOWorker
    {
        private NeuralPSO m_neuralPSO;
        private int m_particleIndex;
        private bool m_init = false;
        
        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="neuralPSO">the training algorithm</param>
        /// <param name="particleIndex">the index of the particle in the swarm</param>
        /// <param name="init">true for an initialisation iteration </param>
        public NeuralPSOWorker(NeuralPSO neuralPSO, int particleIndex, bool init)
        {
            m_neuralPSO = neuralPSO;
            m_particleIndex = particleIndex;
            m_init = init;
        }

        /// <summary>
        /// Update the particle velocity, position and personal best.
        /// </summary>
        public void Run()
        {
            m_neuralPSO.UpdateParticle(m_particleIndex, m_init);
        }

    }
}
