
 namespace Encog.ML.Genetic {
	
	using Encog;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// An error raised by the genetic algorithm.
	/// </summary>
	///
	[Serializable]
	public class GeneticError : EncogError {
	
		private const long serialVersionUID = -5557732297908150500L;
	
		/// <summary>
		/// Construct a message exception.
		/// </summary>
		///
		/// <param name="msg">The exception message.</param>
		public GeneticError(String msg) : base(msg) {
		}
	
		/// <summary>
		/// Construct an exception that holds another exception.
		/// </summary>
		///
		/// <param name="msg">A message.</param>
		/// <param name="t">The other exception.</param>
		public GeneticError(String msg, Exception t) : base(msg, t) {
		}
	
		/// <summary>
		/// Construct an exception that holds another exception.
		/// </summary>
		///
		/// <param name="t">The other exception.</param>
		public GeneticError(Exception t) : base(t) {
		}
	
	}
}
