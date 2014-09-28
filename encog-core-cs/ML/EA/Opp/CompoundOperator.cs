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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.ML.EA.Opp
{
    /// <summary>
    ///     A compound operator randomly chooses sub-operators to perform the actual
    ///     operation. Each of the sub-operators can be provided with a weighting.
    /// </summary>
    public class CompoundOperator : IEvolutionaryOperator
    {
        /// <summary>
        ///     The sub-operators that make up this compound operator.
        /// </summary>
        private readonly OperationList _components = new OperationList();

        /// <summary>
        ///     The owner of this operator.
        /// </summary>
        private IEvolutionaryAlgorithm _owner;

        /// <summary>
        ///     The components.
        /// </summary>
        public OperationList Components
        {
            get { return _components; }
        }

        /// <summary>
        ///     The owner.
        /// </summary>
        public IEvolutionaryAlgorithm Owner
        {
            get { return _owner; }
        }

        /// <inheritdoc />
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            _owner = theOwner;
            foreach (var obj in _components.Contents)
            {
                obj.obj.Init(theOwner);
            }
        }

        /// <inheritdoc />
        public int OffspringProduced
        {
            get { return _components.MaxOffspring(); }
        }

        /// <inheritdoc />
        public int ParentsNeeded
        {
            get { return _components.MaxOffspring(); }
        }

        /// <inheritdoc />
        public void PerformOperation(EncogRandom rnd, IGenome[] parents,
                                     int parentIndex, IGenome[] offspring,
                                     int offspringIndex)
        {
            IEvolutionaryOperator opp = _components.Pick(rnd);
            opp.PerformOperation(rnd, parents, parentIndex, offspring,
                                 offspringIndex);
        }
    }
}
