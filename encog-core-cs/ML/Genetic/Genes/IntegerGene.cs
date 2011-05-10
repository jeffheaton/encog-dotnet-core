
 namespace Encog.ML.Genetic.Genes {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A gene that contains an integer value.
	/// </summary>
	///
	public class IntegerGene : BasicGene {
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// The value of this gene.
		/// </summary>
		///
		private int value_ren;
	
		/// <summary>
		/// Copy another gene to this one.
		/// </summary>
		///
		/// <param name="gene">The other gene to copy.</param>
		public sealed override void Copy(IGene gene) {
			this.value_ren = ((IntegerGene) gene).Value;
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public sealed override bool Equals(Object obj) {
			if (obj  is  IntegerGene) {
				return (((IntegerGene) obj).Value == this.value_ren);
			} else {
				return false;
			}
		}
	
		/// <summary>
		/// Set the value of this gene.
		/// </summary>
		///
		/// <value>The value of this gene.</value>
		public int Value {
		
		/// <returns>The value of this gene.</returns>
		  get {
				return this.value_ren;
			}
		/// <summary>
		/// Set the value of this gene.
		/// </summary>
		///
		/// <param name="theValue">The value of this gene.</param>
		  set {
				this.value_ren = value;
			}
		}
		
	
		
		/// <returns>a hash code.</returns>
		public sealed override int GetHashCode() {
			return this.value_ren;
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
