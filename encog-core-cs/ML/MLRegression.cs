
 namespace Encog.ML {
	
	using Encog.ML.Data;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Defines a Machine Learning Method that supports regression.  Regression 
	/// takes an input and produces numeric output.  Function approximation 
	/// uses regression.  Contrast this to classification, which uses the input 
	/// to assign a class.
	/// </summary>
	///
	public interface MLRegression : MLInputOutput {
	
		/// <summary>
		/// Compute regression.
		/// </summary>
		///
		/// <param name="input">The input data.</param>
		/// <returns>The output data.</returns>
		MLData Compute(MLData input);
	}
}
