 namespace Org.Encog.Engine.Network.Rbf {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A multi-dimension RBF.
	/// </summary>
	///
	public interface RadialBasisFunction {
		/// <summary>
		/// Calculate the RBF result for the specified value.
		/// </summary>
		///
		/// <param name="x">The value to be passed into the RBF.</param>
		/// <returns>The RBF value.</returns>
		double Calculate(double[] x);
	
		/// <summary>
		/// Get the center of this RBD.
		/// </summary>
		///
		/// <param name="dimension">The dimension to get the center for.</param>
		/// <returns>The center of the RBF.</returns>
		double GetCenter(int dimension);
	
		/// <summary>
		/// Set the peak.
		/// </summary>
		double Peak {
		  get;
		  set;
		}
		
	
		/// <summary>
		/// Set the width.
		/// </summary>
		double Width {
		  get;
		  set;
		}
		
	
		
		/// <returns>The dimensions in this RBF.</returns>
		int Dimensions {
		  get;
		}
		
	
		/// <summary>
		/// Set the centers.
		/// </summary>
		double[] Centers {
		  get;
		  set;
		}
		
	}
}
