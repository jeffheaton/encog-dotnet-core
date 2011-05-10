
 namespace Encog.ML.Genetic.Innovation {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	
	/// <summary>
	/// Provides basic functionality for a list of innovations.
	/// </summary>
	///
	[Serializable]
	public class BasicInnovationList : IInnovationList {
	
		public BasicInnovationList() {
			this.list = new List<IInnovation>();
		}
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// The list of innovations.
		/// </summary>
		///
		private readonly IList<IInnovation> list;
	
		/// <summary>
		/// Add an innovation.
		/// </summary>
		///
		/// <param name="innovation">The innovation to add.</param>
		public void Add(IInnovation innovation) {
			list.Add(innovation);
		}
	
		/// <summary>
		/// Get a specific innovation, by index.
		/// </summary>
		///
		/// <param name="id">The innovation index id.</param>
		/// <returns>The innovation.</returns>
		public IInnovation Get(int id) {
			return list[id];
		}
	
		
		/// <value>A list of innovations.</value>
		public IList<IInnovation> Innovations {
		
		/// <returns>A list of innovations.</returns>
		  get {
				return list;
			}
		}
		
	
	}
}
