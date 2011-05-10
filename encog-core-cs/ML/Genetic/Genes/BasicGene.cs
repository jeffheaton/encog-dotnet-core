
 namespace Encog.ML.Genetic.Genes {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	
	/// <summary>
	/// Implements the basic functionality for a gene. This is an abstract class.
	/// </summary>
	///
	[Serializable]
	public abstract class BasicGene : IGene {
	
		public BasicGene() {
			this.enabled = true;
			this.id = -1;
			this.innovationId = -1;
		}
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// Is this gene enabled?
		/// </summary>
		///
		private bool enabled;
	
		/// <summary>
		/// ID of this gene, -1 for unassigned.
		/// </summary>
		///
		private long id;
	
		/// <summary>
		/// Innovation ID, -1 for unassigned.
		/// </summary>
		///
		private long innovationId;
	
		/// <summary>
		/// 
		/// </summary>
		///
		public int CompareTo(IGene o) {
			return ((int) (InnovationId - o.InnovationId));
		}
	
		/// <summary>
		/// Set the id for this gene.
		/// </summary>
		///
		/// <value>The id for this gene.</value>
		public long Id {
		
		/// <returns>The id of this gene.</returns>
		  get {
				return id;
			}
		/// <summary>
		/// Set the id for this gene.
		/// </summary>
		///
		/// <param name="i">The id for this gene.</param>
		  set {
				this.id = value;
			}
		}
		
	
		/// <summary>
		/// Set the innovation id for this gene.
		/// </summary>
		///
		/// <value>The innovation id for this gene.</value>
		public long InnovationId {
		
		/// <returns>The innovation id of this gene.</returns>
		  get {
				return innovationId;
			}
		/// <summary>
		/// Set the innovation id for this gene.
		/// </summary>
		///
		/// <param name="theInnovationID">The innovation id for this gene.</param>
		  set {
				innovationId = value;
			}
		}
		
	
		
		/// <value>True, if this gene is enabled.</value>
		public bool Enabled {
		
		/// <returns>True, if this gene is enabled.</returns>
		  get {
				return enabled;
			}
		
		/// <param name="e">True, if this gene is enabled.</param>
		  set {
				enabled = value;
			}
		}
		
	
		/// <summary>
		/// from Encog.ml.genetic.genes.Gene
		/// </summary>
		///
		public abstract void Copy(Encog.ML.Genetic.Genes.IGene gene);
	
	}
}
