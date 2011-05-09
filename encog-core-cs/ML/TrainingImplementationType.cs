
 namespace Encog.ML {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Specifies the type of training that an object provides.
	/// </summary>
	///
	public enum TrainingImplementationType {
		/// <summary>
		/// Iterative - Each iteration attempts to improve the machine 
		/// learning method.
		/// </summary>
		///
		Iterative,
	
		/// <summary>
		/// Background - Training continues in the background until it is
		/// either finished or is stopped.
		/// </summary>
		///
		Background,
	
		/// <summary>
		/// Single Pass - Only one iteration is necessary.
		/// </summary>
		///
		OnePass
	}
}
