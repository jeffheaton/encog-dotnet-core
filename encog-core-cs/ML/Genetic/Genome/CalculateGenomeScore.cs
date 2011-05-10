
 namespace Encog.ML.Genetic.Genome {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Genetic Algorithms need a class to calculate the score.
	/// </summary>
	///
	public interface CalculateGenomeScore {
		/// <summary>
		/// Calculate this genome's score.
		/// </summary>
		///
		/// <param name="genome">The genome.</param>
		/// <returns>The score.</returns>
		double CalculateScore(IGenome genome);
	
		
		/// <returns>True if the goal is to minimize the score.</returns>
		bool ShouldMinimize();
	}
}
