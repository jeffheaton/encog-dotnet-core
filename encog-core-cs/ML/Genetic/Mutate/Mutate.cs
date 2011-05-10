
 namespace Encog.ML.Genetic.Mutate {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
     using Encog.ML.Genetic.Genome;
	
	/// <summary>
	/// Defines how a chromosome is mutated.
	/// </summary>
	///
	public interface IMutate {
	
		/// <summary>
		/// Perform a mutation on the specified chromosome.
		/// </summary>
		///
		/// <param name="chromosome">The chromosome to mutate.</param>
		void PerformMutation(Chromosome chromosome);
	
	}
}
