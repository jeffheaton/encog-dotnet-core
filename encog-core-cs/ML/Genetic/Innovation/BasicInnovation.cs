
 namespace Encog.ML.Genetic.Innovation {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Provides basic functionality for an innovation.
	/// </summary>
	///
	public class BasicInnovation : IInnovation {
	
		/// <summary>
		/// The innovation id.
		/// </summary>
		///
		private long innovationID;
	
		/// <summary>
		/// Set the innovation id.
		/// </summary>
		///
		/// <value>The innovation id.</value>
		public long InnovationID {
		
		/// <returns>The innovation ID.</returns>
		  get {
				return innovationID;
			}
		/// <summary>
		/// Set the innovation id.
		/// </summary>
		///
		/// <param name="theInnovationID">The innovation id.</param>
		  set {
				this.innovationID = value;
			}
		}
		
	
	}
}
