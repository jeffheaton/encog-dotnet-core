
 namespace Encog.ML {
	
	using Encog.ML.Data;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Defines Machine Learning Method that can calculate an error based on a 
	/// data set.
	/// </summary>
	///
	public interface MLError : MLMethod {
		/// <summary>
		/// Calculate the error of the ML method, given a dataset.
		/// </summary>
		///
		/// <param name="data">The dataset.</param>
		/// <returns>The error.</returns>
		double CalculateError(MLDataSet data);
	}
}
