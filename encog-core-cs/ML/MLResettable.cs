
 namespace Encog.ML {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Defines a Machine Learning Method that can be reset to an untrained 
	/// starting point.  Most weight based machine learning methods, such
	/// as neural networks support this.  Support vector machines do not.
	/// </summary>
	///
	public interface MLResettable : MLMethod {
	
		/// <summary>
		/// Reset the weights.
		/// </summary>
		///
		void Reset();
	
		/// <summary>
		/// Reset the weights with a seed.
		/// </summary>
		///
		/// <param name="seed">The seed value.</param>
		void Reset(int seed);
	}
}
