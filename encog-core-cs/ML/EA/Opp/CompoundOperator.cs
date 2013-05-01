using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Train;
using Encog.Util.Obj;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Opp
{
    /// <summary>
    /// A compound operator randomly chooses sub-operators to perform the actual
    /// operation. Each of the sub-operators can be provided with a weighting.
    /// </summary>
    public class CompoundOperator : IEvolutionaryOperator
    {
        /**
	 * The owner of this operator.
	 */
	private IEvolutionaryAlgorithm owner;

	/**
	 * The sub-operators that make up this compound operator.
	 */
	private OperationList components = new OperationList();

	/**
	 * @return the components
	 */
	public OperationList getComponents() {
		return this.components;
	}

	/**
	 * @return the owner
	 */
	public IEvolutionaryAlgorithm getOwner() {
		return this.owner;
	}

	/**
	 * {@inheritDoc}
	 */
	public void Init(IEvolutionaryAlgorithm theOwner) {
		this.owner = theOwner;
		foreach (ObjectHolder<IEvolutionaryOperator> obj in this.components.Contents) {
			obj.obj.Init(theOwner);
		}
	}

	/**
	 * {@inheritDoc}
	 */
    public int OffspringProduced
    {
        get
        {
            return this.components.MaxOffspring();
        }
	}

	/**
	 * {@inheritDoc}
	 */
    public int ParentsNeeded
    {
        get
        {
            return this.components.MaxOffspring();
        }
	}

	/**
	 * {@inheritDoc}
	 */
	public void PerformOperation(Random rnd, IGenome[] parents,
			int parentIndex, IGenome[] offspring,
			int offspringIndex) {
		IEvolutionaryOperator opp = this.components.Pick(rnd);
		opp.PerformOperation(rnd, parents, parentIndex, offspring,
				offspringIndex);
	}
    }
}
