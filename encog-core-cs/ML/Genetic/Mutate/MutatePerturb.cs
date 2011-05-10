
 namespace Encog.ML.Genetic.Mutate {
	
	using Encog.ML.Genetic.Genes;
	using Encog.ML.Genetic.Genome;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A simple mutation based on random numbers.
	/// </summary>
	///
	public class MutatePerturb : IMutate {
	
		/// <summary>
		/// The amount to perturb by.
		/// </summary>
		///
		private readonly double perturbAmount;
	
		/// <summary>
		/// Construct a perturb mutation.
		/// </summary>
		///
		/// <param name="thePerturbAmount">The amount to mutate by(percent).</param>
		public MutatePerturb(double thePerturbAmount) {
			this.perturbAmount = thePerturbAmount;
		}
	
		/// <summary>
		/// Perform a perturb mutation on the specified chromosome.
		/// </summary>
		///
		/// <param name="chromosome">The chromosome to mutate.</param>
		public void PerformMutation(Chromosome chromosome) {
			/* foreach */
			foreach (IGene gene  in  chromosome.Genes) {
				if (gene  is  DoubleGene) {
					DoubleGene doubleGene = (DoubleGene) gene;
					double value_ren = doubleGene.Value;
					value_ren += (perturbAmount - ((new Random()).Next() * perturbAmount * 2));
					doubleGene.Value = value_ren;
				}
			}
		}
	}
}
