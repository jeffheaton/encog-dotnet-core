
 namespace Encog.ML.Genetic {
	
	using Encog.ML.Genetic.Genome;
	using Encog.Util.Concurrency;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
     using Encog.ML.Genetic.Genome;
	
	/// <summary>
	/// This class is used in conjunction with a thread pool. This allows the genetic
	/// algorithm to offload all of those calculations to a thread pool.
	/// </summary>
	///
	public class MateWorker : IEngineTask {
	
		/// <summary>
		/// The first child.
		/// </summary>
		///
		private readonly IGenome child1;
	
		/// <summary>
		/// The second child.
		/// </summary>
		///
		private readonly IGenome child2;
	
		/// <summary>
		/// The father.
		/// </summary>
		///
		private readonly IGenome father;
	
		/// <summary>
		/// The mother.
		/// </summary>
		///
		private readonly IGenome mother;
	
		
		/// <param name="theMother">The mother.</param>
		/// <param name="theFather">The father.</param>
		/// <param name="theChild1">The first child.</param>
		/// <param name="theChild2">The second child.</param>
		public MateWorker(IGenome theMother, IGenome theFather,
				IGenome theChild1, IGenome theChild2) {
			this.mother = theMother;
			this.father = theFather;
			this.child1 = theChild1;
			this.child2 = theChild2;
		}
	
		/// <summary>
		/// Mate the two chromosomes.
		/// </summary>
		///
		public void Run() {
			mother.Mate(father, child1, child2);
		}
	
	}
}
