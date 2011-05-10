
 namespace Encog.ML.Genetic.Genes {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A gene that holds a single character.
	/// </summary>
	///
	public class CharGene : BasicGene {
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// The character value of the gene.
		/// </summary>
		///
		private char value_ren;
	
		/// <summary>
		/// Copy another gene to this gene.
		/// </summary>
		///
		/// <param name="gene">The source gene.</param>
		public sealed override void Copy(IGene gene) {
			this.value_ren = ((CharGene) gene).Value;
		}
	
		/// <summary>
		/// Set the value of this gene.
		/// </summary>
		///
		/// <value>The new value of this gene.</value>
		public char Value {
		
		/// <returns>The value of this gene.</returns>
		  get {
				return this.value_ren;
			}
		/// <summary>
		/// Set the value of this gene.
		/// </summary>
		///
		/// <param name="theValue">The new value of this gene.</param>
		  set {
				this.value_ren = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public sealed override  System.String ToString() {
			return "" + this.value_ren;
		}
	
	}
}
