
 namespace Encog.ML {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
     using Encog.Util.CSV;
	
	/// <summary>
	/// A class that provides basic property functionality for the MLProperties
	/// interface.
	/// </summary>
	///
	[Serializable]
	public abstract class BasicML : MLMethod, MLProperties {
	
		public BasicML() {
			this.properties = new Dictionary<String, String>();
		}
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// Properties about the neural network. Some NeuralLogic classes require
		/// certain properties to be set.
		/// </summary>
		///
		private readonly IDictionary<String, String> properties;
	
		
		/// <value>A map of all properties.</value>
		public IDictionary<String,String> Properties {
		
		/// <returns>A map of all properties.</returns>
		  get {
				return this.properties;
			}
		}
		
	
		/// <summary>
		/// Get the specified property as a double.
		/// </summary>
		///
		/// <param name="name">The name of the property.</param>
		/// <returns>The property as a double.</returns>
		public double GetPropertyDouble(String name) {
			return (CSVFormat.EG_FORMAT.Parse((this.properties[name])));
		}
	
		/// <summary>
		/// Get the specified property as a long.
		/// </summary>
		///
		/// <param name="name">The name of the specified property.</param>
		/// <returns>The value of the specified property.</returns>
		public long GetPropertyLong(String name) {
			return ((Int64 )Int64.Parse(this.properties[name]));
		}
	
		/// <summary>
		/// Get the specified property as a string.
		/// </summary>
		///
		/// <param name="name">The name of the property.</param>
		/// <returns>The value of the property.</returns>
		public String GetPropertyString(String name) {
			return (this.properties[name]);
		}
	
		/// <summary>
		/// Set a property as a double.
		/// </summary>
		///
		/// <param name="name">The name of the property.</param>
		/// <param name="d">The value of the property.</param>
		public void SetProperty(String name, double d) {
			this.properties[name] = CSVFormat.EG_FORMAT.Format(d, Encog.EncogFramework.DEFAULT_PRECISION);
			UpdateProperties();
		}
	
		/// <summary>
		/// Set a property as a long.
		/// </summary>
		///
		/// <param name="name">The name of the property.</param>
		/// <param name="l">The value of the property.</param>
		public void SetProperty(String name, long l) {
			this.properties[name] = "" + l;
			UpdateProperties();
		}
	
		/// <summary>
		/// Set a property as a double.
		/// </summary>
		///
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		public void SetProperty(String name, String value_ren) {
			this.properties[name] = value_ren;
			UpdateProperties();
		}
	
		public abstract void UpdateProperties();
	
	}
}
