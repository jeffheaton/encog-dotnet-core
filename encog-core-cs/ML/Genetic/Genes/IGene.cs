
 namespace Encog.ML.Genetic.Genes {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Describes a gene. A gene is the smallest piece of genetic information in
	/// Encog.
	/// </summary>
	///
	public interface IGene : IComparable<IGene> {
	
		/// <summary>
		/// Copy another gene to this one.
		/// </summary>
		///
		/// <param name="gene">The other gene to copy.</param>
		void Copy(IGene gene);
	
		/// <summary>
		/// Get the ID of this gene, -1 for undefined.
		/// </summary>
		///
		/// <value>The ID of this gene.</value>
		long Id {
		/// <summary>
		/// Get the ID of this gene, -1 for undefined.
		/// </summary>
		///
		/// <returns>The ID of this gene.</returns>
		  get;
		}
		
	
		
		/// <value>The innovation ID of this gene.</value>
		long InnovationId {
		
		/// <returns>The innovation ID of this gene.</returns>
		  get;
		}
		
	
		/// <summary>
		/// Determine if this gene is enabled.
		/// </summary>
		///
		/// <value>True if this gene is enabled.</value>
		bool Enabled {
		
		/// <returns>True, if this gene is enabled.</returns>
		  get;
		/// <summary>
		/// Determine if this gene is enabled.
		/// </summary>
		///
		/// <param name="e">True if this gene is enabled.</param>
		  set;
		}
		
	}
}
