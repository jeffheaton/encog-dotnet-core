
 namespace Encog.ML {
	
	using Encog.ML.Data;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// This interface defines a MLMethod that is used for classification.  
	/// Classification defines the output to be a class.  A MLMethod that uses 
	/// classification is attempting to use the input to place items into 
	/// classes.  It is assumed that an item will only be in one single class.  
	/// If an item can be in multiple classes, one option is to create additional 
	/// classes that represent the compound classes.
	/// </summary>
	///
	public interface MLClassification : MLInputOutput {
	
		/// <summary>
		/// Classify the input into a group.
		/// </summary>
		///
		/// <param name="input">The input data to classify.</param>
		/// <returns>The group that the data was classified into.</returns>
		int Classify(MLData input);
	}
}
