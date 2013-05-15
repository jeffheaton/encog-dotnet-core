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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Exceptions;
using Encog.MathUtil.Randomize;

namespace Encog.ML.EA.Train
{
    /// <summary>
    /// A worker thread for an Evolutionary Algorithm.
    /// </summary>
    public class EAWorker
    {
        /// <summary>
        /// The species being processed.
        /// </summary>
        private ISpecies species;

        /// <summary>
        /// The parent genomes.
        /// </summary>
        private IGenome[] parents;

        /// <summary>
        /// The children genomes.
        /// </summary>
        private IGenome[] children;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private EncogRandom rnd;

        /// <summary>
        /// The parent object.
        /// </summary>
        private BasicEA train;
        
        /// <summary>
        /// Construct the EA worker.
        /// </summary>
        /// <param name="theTrain">The trainer.</param>
        /// <param name="theSpecies">The species.</param>
        public EAWorker(BasicEA theTrain, ISpecies theSpecies)
        {
            this.train = theTrain;
            this.species = theSpecies;
            this.rnd = this.train.RandomNumberFactory.Factor();

            this.parents = new IGenome[this.train.Operators.MaxParents()];
            this.children = new IGenome[this.train.Operators.MaxOffspring()];
        }

        /// <summary>
        /// Choose a parent.
        /// </summary>
        /// <returns>The chosen parent.</returns>
        private IGenome ChooseParent()
        {
            int idx = this.train.Selection.PerformSelection(this.rnd,
                    this.species);
            return this.species.Members[idx];
        }

        /// <summary>
        /// Perform one operation.
        /// </summary>
        public void PerformTask()
        {
            bool success = false;
            int tries = this.train.MaxOperationErrors;
            do
            {
                try
                {
                    // choose an evolutionary operation (i.e. crossover or a type of
                    // mutation) to use
                    IEvolutionaryOperator opp = this.train.Operators
                            .PickMaxParents(this.rnd,
                                    this.species.Members.Count);

                    this.children[0] = null;

                    // prepare for either sexual or asexual reproduction either way,
                    // we need at least one parent, which is the first parent.
                    //
                    // Chose the first parent, there must be at least one genome in
                    // this species
                    this.parents[0] = ChooseParent();

                    // if the number of individuals in this species is only
                    // one then we can only clone and perhaps mutate, otherwise use
                    // the crossover probability to determine if we are to use
                    // sexual reproduction.
                    if (opp.ParentsNeeded > 1)
                    {

                        int numAttempts = 5;

                        this.parents[1] = ChooseParent();
                        while (this.parents[0] == this.parents[1]
                                && numAttempts-- > 0)
                        {
                            this.parents[1] = ChooseParent();
                        }

                        // success, perform crossover
                        if (this.parents[0] != this.parents[1])
                        {
                            opp.PerformOperation(this.rnd, this.parents, 0,
                                    this.children, 0);
                        }
                    }
                    else
                    {
                        // clone a child (asexual reproduction)
                        opp.PerformOperation(this.rnd, this.parents, 0,
                                this.children, 0);
                        this.children[0].Population = this.parents[0].Population;
                    }

                    // process the new child
                    foreach (IGenome child in this.children)
                    {
                        if (child != null)
                        {
                            child.Population = this.parents[0].Population;
                            if (this.train.Rules.IsValid(child))
                            {
                                child.BirthGeneration = this.train.IterationNumber;

                                this.train.CalculateScore(child);
                                if (!this.train.AddChild(child))
                                {
                                    return;
                                }
                                success = true;
                            }
                        }
                    }
                }
                catch (EARuntimeError e)
                {
                    tries--;
                    if (tries < 0)
                    {
                        throw new EncogError(
                                "Could not perform a successful genetic operaton after "
                                        + this.train.MaxOperationErrors
                                        + " tries.");
                    }
                }
                catch (Exception t)
                {
                    if (!this.train.IgnoreExceptions)
                    {
                        throw (t);
                    }
                }

            } while (!success);
        }
    }
}
