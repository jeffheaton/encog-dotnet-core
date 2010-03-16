using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic
{
    public class MateWorker: IEncogTask
    {
        
	/**
	 * The first child.
	 */
	private  IGenome child1;

	/**
	 * The second child.
	 */
	private IGenome child2;

	/**
	 * The father.
	 */
	private IGenome father;

	/**
	 * The mother.
	 */
	private IGenome mother;

	/**
	 * 
	 * @param mother
	 *            The mother.
	 * @param father
	 *            The father.
	 * @param child1
	 *            The first child.
	 * @param child2
	 *            The second child.
	 */
	public MateWorker(IGenome mother, IGenome father,
			IGenome child1, IGenome child2) {
		this.mother = mother;
		this.father = father;
		this.child1 = child1;
		this.child2 = child2;
	}

	/**
	 * Mate the two chromosomes.
	 */
	public void Run() {
		mother.Mate(father, child1, child2);
	}
    }
}
