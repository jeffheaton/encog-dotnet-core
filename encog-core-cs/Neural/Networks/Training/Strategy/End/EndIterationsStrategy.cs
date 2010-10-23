using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    public class EndIterationsStrategy:IEndTrainingStrategy 
    {
        	private int maxIterations;
	private int currentIteration;
	private ITrain train;
	
	public EndIterationsStrategy(int maxIterations) {
		this.maxIterations = maxIterations;
		this.currentIteration = 0;
	}
	
	
	/**
	 * {@inheritDoc}
	 */
	public bool ShouldStop() {
		return (this.currentIteration>=this.maxIterations);
	}

	/**
	 * {@inheritDoc}
	 */
	public void Init(ITrain train) {
		this.train = train;
	}

	/**
	 * {@inheritDoc}
	 */
	public void PostIteration() {
		this.currentIteration = this.train.CurrentIteration;
	}

	/**
	 * {@inheritDoc}
	 */
	public void PreIteration() {
	}
    }
}
