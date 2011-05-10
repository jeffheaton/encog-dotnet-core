
 namespace Encog.ML.Genetic.Genes {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A gene that contains a floating point value.
	/// </summary>
	///
	public class DoubleGene : BasicGene {
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// The value of this gene.
		/// </summary>
		///
		private double value_ren;
	
		/// <summary>
		/// Copy another gene to this one.
		/// </summary>
		///
		/// <param name="gene">The other gene to copy.</param>
		public sealed override void Copy(IGene gene) {
			value_ren = ((DoubleGene) gene).Value;
	
		}
	
		/// <summary>
		/// Set the value of the gene.
		/// </summary>
		///
		/// <value>The gene's value.</value>
		public double Value {
		
		/// <returns>The gene value.</returns>
		  get {
				return value_ren;
			}
		/// <summary>
		/// Set the value of the gene.
		/// </summary>
		///
		/// <param name="theValue">The gene's value.</param>
		  set {
				this.value_ren = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public sealed override  System.String ToString() {
			return "" + value_ren;
		}
	
	}
}
