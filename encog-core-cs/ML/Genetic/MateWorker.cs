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
using Encog.ML.Genetic.Genome;
using Encog.Util.Concurrency;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// This class is used in conjunction with a thread pool. This allows the genetic
    /// algorithm to offload all of those calculations to a thread pool.
    /// </summary>
    ///
    public class MateWorker : IEngineTask
    {
        /// <summary>
        /// The first child.
        /// </summary>
        ///
        private readonly IGenome _child1;

        /// <summary>
        /// The second child.
        /// </summary>
        ///
        private readonly IGenome _child2;

        /// <summary>
        /// The father.
        /// </summary>
        ///
        private readonly IGenome _father;

        /// <summary>
        /// The mother.
        /// </summary>
        ///
        private readonly IGenome _mother;


        /// <param name="theMother">The mother.</param>
        /// <param name="theFather">The father.</param>
        /// <param name="theChild1">The first child.</param>
        /// <param name="theChild2">The second child.</param>
        public MateWorker(IGenome theMother, IGenome theFather,
                          IGenome theChild1, IGenome theChild2)
        {
            _mother = theMother;
            _father = theFather;
            _child1 = theChild1;
            _child2 = theChild2;
        }

        #region IEngineTask Members

        /// <summary>
        /// Mate the two chromosomes.
        /// </summary>
        ///
        public void Run()
        {
            _mother.Mate(_father, _child1, _child2);
        }

        #endregion
    }
}
