using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Exceptions;

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
        private Random rnd;

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
                                child.BirthGeneration = this.train.CurrentIteration;

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
