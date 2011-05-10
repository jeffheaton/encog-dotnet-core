
 namespace Encog.ML.Anneal {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Simulated annealing is a common training method. This class implements a
	/// simulated annealing algorithm that can be used both for neural networks, as
	/// well as more general cases. This class is abstract, so a more specialized
	/// simulated annealing subclass will need to be created for each intended use.
	/// This book demonstrates how to use the simulated annealing algorithm to train
	/// feedforward neural networks, as well as find a solution to the traveling
	/// salesman problem.
	/// The name and inspiration come from annealing in metallurgy, a technique
	/// involving heating and controlled cooling of a material to increase the size
	/// of its crystals and reduce their defects. The heat causes the atoms to become
	/// unstuck from their initial positions (a local minimum of the internal energy)
	/// and wander randomly through states of higher energy; the slow cooling gives
	/// them more chances of finding configurations with lower internal energy than
	/// the initial one.
	/// </summary>
	///
	/// <typeparam name="UNIT_TYPE">What type of data makes up the solution.</typeparam>
	public abstract class SimulatedAnnealing<UNIT_TYPE> {
	
		public SimulatedAnnealing() {
			this.shouldMinimize = true;
		}
	
		/// <summary>
		/// The starting temperature.
		/// </summary>
		///
		private double startTemperature;
		/// <summary>
		/// The ending temperature.
		/// </summary>
		///
		private double stopTemperature;
		/// <summary>
		/// The number of cycles that will be used.
		/// </summary>
		///
		private int cycles;
		/// <summary>
		/// The current score.
		/// </summary>
		///
		private double score;
	
		/// <summary>
		/// The current temperature.
		/// </summary>
		///
		private double temperature;
	
		/// <summary>
		/// Should the score be minimized.
		/// </summary>
		///
		private bool shouldMinimize;
	
		/// <summary>
		/// Subclasses should provide a method that evaluates the score for the
		/// current solution. Those solutions with a lower score are better.
		/// </summary>
		///
		/// <returns>Return the score.</returns>
		public abstract double CalculateScore();
	
		/// <summary>
		/// Subclasses must provide access to an array that makes up the solution.
		/// </summary>
		///
		/// <value>An array that makes up the solution.</value>
		public abstract UNIT_TYPE[] Array {
		/// <summary>
		/// Subclasses must provide access to an array that makes up the solution.
		/// </summary>
		///
		/// <returns>An array that makes up the solution.</returns>
		  get;
		}
		
	
		/// <summary>
		/// Get a copy of the array.
		/// </summary>
		///
		/// <value>A copy of the array.</value>
		public abstract UNIT_TYPE[] ArrayCopy {
		/// <summary>
		/// Get a copy of the array.
		/// </summary>
		///
		/// <returns>A copy of the array.</returns>
		  get;
		}
		
	
		
		/// <value>the cycles to set</value>
		public int Cycles {
		
		/// <returns>the cycles</returns>
		  get {
				return this.cycles;
			}
		
		/// <param name="theCycles">the cycles to set</param>
		  set {
				this.cycles = value;
			}
		}
		
	
		/// <summary>
		/// Set the score.
		/// </summary>
		///
		/// <value>The score to set.</value>
		public double Score {
		
		/// <returns>the globalScore</returns>
		  get {
				return this.score;
			}
		/// <summary>
		/// Set the score.
		/// </summary>
		///
		/// <param name="theScore">The score to set.</param>
		  set {
				this.score = value;
			}
		}
		
	
		
		/// <value>the startTemperature to set</value>
		public double StartTemperature {
		
		/// <returns>the startTemperature</returns>
		  get {
				return this.startTemperature;
			}
		
		/// <param name="theStartTemperature">the startTemperature to set</param>
		  set {
				this.startTemperature = value;
			}
		}
		
	
		
		/// <value>the stopTemperature to set</value>
		public double StopTemperature {
		
		/// <returns>the stopTemperature</returns>
		  get {
				return this.stopTemperature;
			}
		
		/// <param name="theStopTemperature">the stopTemperature to set</param>
		  set {
				this.stopTemperature = value;
			}
		}
		
	
		
		/// <value>the temperature to set</value>
		public double Temperature {
		
		/// <returns>the temperature</returns>
		  get {
				return this.temperature;
			}
		
		/// <param name="theTemperature">the temperature to set</param>
		  set {
				this.temperature = value;
			}
		}
		
	
		/// <summary>
		/// Should the score be minimized.
		/// </summary>
		///
		/// <value>True if the score should be minimized.</value>
		public bool ShouldMinimize {
		
		/// <returns>True if the score should be minimized.</returns>
		  get {
				return this.shouldMinimize;
			}
		/// <summary>
		/// Should the score be minimized.
		/// </summary>
		///
		/// <param name="theShouldMinimize">True if the score should be minimized.</param>
		  set {
				this.shouldMinimize = value;
			}
		}
		
	
		/// <summary>
		/// Called to perform one cycle of the annealing process.
		/// </summary>
		///
		public void Iteration() {
			UNIT_TYPE[] bestArray;
	
			Score = CalculateScore();
			bestArray = this.ArrayCopy;
	
			this.temperature = this.StartTemperature;
	
			for (int i = 0; i < this.cycles; i++) {
				double curScore;
				Randomize();
				curScore = CalculateScore();
	
				if (this.shouldMinimize) {
					if (curScore < Score) {
						bestArray = this.ArrayCopy;
						Score = curScore;
					}
				} else {
					if (curScore > Score) {
						bestArray = this.ArrayCopy;
						Score = curScore;
					}
				}
	
				this.PutArray(bestArray);
				double ratio = Math.Exp(Math.Log(StopTemperature
												/ StartTemperature)
									/ (Cycles - 1));
				this.temperature *= ratio;
			}
		}
	
		/// <summary>
		/// Store the array.
		/// </summary>
		///
		/// <param name="array">The array to be stored.</param>
		public abstract void PutArray(UNIT_TYPE[] array);
	
		/// <summary>
		/// Randomize the weight matrix.
		/// </summary>
		///
		public abstract void Randomize();
	
	}
}
