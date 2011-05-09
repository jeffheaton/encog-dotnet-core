
 namespace Encog.ML {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// This interface is the base for all Encog Machine Learning methods.  It 
	/// defines very little, other than the fact that a subclass is a Machine 
	/// Learning Method.  A MLMethod is an algorithm that accepts data and 
	/// provides some sort of insight into it.  This could be a neural network, 
	/// support vector machine, clustering algorithm, or something else entirely.
	/// Many MLMethods must be trained by a MLTrain object before they are useful.
	/// </summary>
	///
	public interface MLMethod {
	}
}
