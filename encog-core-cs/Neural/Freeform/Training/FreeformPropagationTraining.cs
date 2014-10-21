//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.MathUtil.Error;

namespace Encog.Neural.Freeform.Training
{
    /// <summary>
    /// Provides basic propagation functions to other trainers.
    /// </summary>
    [Serializable]
    public abstract class FreeformPropagationTraining : BasicTraining
    {
        /// <summary>
        /// The constant to use to fix the flat spot problem.
        /// </summary>
	public static double FlatSpotConst = 0.1;
	
	/// <summary>
    /// The network that we are training.
	/// </summary>
	private readonly FreeformNetwork _network;
	
	/// <summary>
    /// The training set to use.
	/// </summary>
	private readonly IMLDataSet _training;
	
	/// <summary>
    /// The number of iterations.
	/// </summary>
    private int _iterationCount = 0;	

	/// <summary>
        /// The error at the beginning of the last iteration.
	/// </summary>
	public override double Error { get; set; }
	
	/// <summary>
    /// The neurons that have been visited.
	/// </summary>
	private readonly HashSet<IFreeformNeuron> _visited = new HashSet<IFreeformNeuron>();
	
	/// <summary>
    /// Are we fixing the flat spot problem?  (default = true)
	/// </summary>
	public bool FixFlatSopt { get; set; }
	
	/// <summary>
	/// The batch size. Specify 1 for pure online training. Specify 0 for pure
	/// batch training (complete training set in one batch). Otherwise specify
	/// the batch size for batch training.
	/// </summary>
	public int BatchSize { get; set; }

	/// <summary>
    /// Don't use this constructor, it is for serialization only.
	/// </summary>
	public FreeformPropagationTraining(): base(TrainingImplementationType.Iterative) {
		_network = null;
		_training = null;
	    FixFlatSopt = true;
	}
	
	/// <summary>
	/// Construct the trainer.
	/// </summary>
	/// <param name="theNetwork">The network to train.</param>
	/// <param name="theTraining">The training data.</param>
	public FreeformPropagationTraining(FreeformNetwork theNetwork,
			IMLDataSet theTraining):
            base(TrainingImplementationType.Iterative)
    {
		
		_network = theNetwork;
		_training = theTraining;
	    FixFlatSopt = true;
    }


	/// <summary>
    /// Calculate the gradient for a neuron.
	/// </summary>
    /// <param name="toNeuron">The neuron to calculate for.</param>
	private void CalculateNeuronGradient(IFreeformNeuron toNeuron) {

		// Only calculate if layer has inputs, because we've already handled the
		// output
		// neurons, this means a hidden layer.
		if (toNeuron.InputSummation != null) {

			// between the layer deltas between toNeuron and the neurons that
			// feed toNeuron.
			// also calculate all inbound gradeints to toNeuron
			foreach (IFreeformConnection connection in toNeuron
					.InputSummation.List) {

				// calculate the gradient
				double gradient = connection.Source.Activation
						* toNeuron.GetTempTraining(0);
				connection.AddTempTraining(0, gradient);

				// calculate the next layer delta
				IFreeformNeuron fromNeuron = connection.Source;
				double sum = fromNeuron.Outputs.Sum(toConnection => toConnection.Target.GetTempTraining(0)*toConnection.Weight);
					    double neuronOutput = fromNeuron.Activation;
				double neuronSum = fromNeuron.Sum;
				double deriv = toNeuron.InputSummation
						.ActivationFunction
						.DerivativeFunction(neuronSum, neuronOutput);

				if (FixFlatSopt
						&& (toNeuron.InputSummation
								.ActivationFunction is ActivationSigmoid)) {
					deriv += FlatSpotConst;
				}

				double layerDelta = sum * deriv;
				fromNeuron.SetTempTraining(0, layerDelta);
			}

			// recurse to the next level
			foreach (IFreeformConnection connection in toNeuron
					.InputSummation.List) {
				IFreeformNeuron fromNeuron = connection.Source;
				CalculateNeuronGradient(fromNeuron);
			}

		}

	}

	/// <summary>
	/// Calculate the output delta for a neuron, given its difference.
    /// Only used for output neurons.
	/// </summary>
	/// <param name="neuron">The neuron.</param>
	/// <param name="diff">The difference.</param>
	private void CalculateOutputDelta(IFreeformNeuron neuron,
			double diff) {
		double neuronOutput = neuron.Activation;
		double neuronSum = neuron.InputSummation.Sum;
		double deriv = neuron.InputSummation.ActivationFunction
				.DerivativeFunction(neuronSum, neuronOutput);
		if (FixFlatSopt
				&& (neuron.InputSummation.ActivationFunction is ActivationSigmoid)) {
			deriv += FlatSpotConst;
		}
		double layerDelta = deriv * diff;
		neuron.SetTempTraining(0, layerDelta);
	}

	/// <inheritdoc/>
	public override bool CanContinue {
	    get
	    {
	        return false;
	    }
	}

    /// <inheritdoc/>
	public override void FinishTraining() {
		_network.TempTrainingClear();
	}


    /// <inheritdoc/>
	public override TrainingImplementationType ImplementationType {
	    get
	    {
	        return TrainingImplementationType.Iterative;
	    }
	}

    /// <inheritdoc/>
	public override IMLMethod Method {
	    get
	    {
	        return _network;
	    }
	}

    /// <inheritdoc/>
	public override IMLDataSet Training {
	    get
	    {
	        return _training;
	    }
	}


    /// <inheritdoc/>
	public override void Iteration() {
		PreIteration();
		_iterationCount++;
		_network.ClearContext();
		
		if (BatchSize == 0) {
			ProcessPureBatch();
		} else {
			ProcessBatches();
		}
		
		PostIteration();
	}

    /// <inheritdoc/>
	public override void Iteration(int count) {
		for (int i = 0; i < count; i++) {
			Iteration();
		}

	}
	
	/// <summary>
    /// Process training for pure batch mode (one single batch).
	/// </summary>
	protected void ProcessPureBatch() {
		var errorCalc = new ErrorCalculation();
		_visited.Clear();

		foreach (IMLDataPair pair in _training) {
			var input = pair.Input;
			var ideal = pair.Ideal;
			var actual = _network.Compute(input);
			var sig = pair.Significance;

			errorCalc.UpdateError(actual, ideal, sig);

			for (int i = 0; i < _network.OutputCount; i++) {
				var diff = (ideal[i] - actual[i])
						* sig;
				IFreeformNeuron neuron = _network.OutputLayer.Neurons[i];
				CalculateOutputDelta(neuron, diff);
				CalculateNeuronGradient(neuron);
			}
		}

		// Set the overall error.
		Error = errorCalc.Calculate();
		
		// Learn for all data.
		Learn();		
	}
	
	/// <summary>
    /// Process training batches.
	/// </summary>
	protected void ProcessBatches() {
		int lastLearn = 0;
		var errorCalc = new ErrorCalculation();
		_visited.Clear();

		foreach (IMLDataPair pair in _training) {
			var input = pair.Input;
			var ideal = pair.Ideal;
			var actual = _network.Compute(input);
			var sig = pair.Significance;

			errorCalc.UpdateError(actual, ideal, sig);

			for (int i = 0; i < _network.OutputCount; i++) {
				double diff = (ideal[i] - actual[i])
						* sig;
				IFreeformNeuron neuron = _network.OutputLayer.Neurons[i];
				CalculateOutputDelta(neuron, diff);
				CalculateNeuronGradient(neuron);
			}
			
			// Are we at the end of a batch.
			lastLearn++;
			if( lastLearn>=BatchSize ) {
				lastLearn = 0;
				Learn();	
			}
		}
		
		// Handle any remaining data.
		if( lastLearn>0 ) {
			Learn();
		}

		// Set the overall error.
		Error = errorCalc.Calculate();
		
	}
	
	/// <summary>
    /// Learn for the entire network.
	/// </summary>
	protected void Learn() {
		_network.PerformConnectionTask(c=>{
				LearnConnection(c);
				c.SetTempTraining(0, 0);
			}
		);
	}

	/// <summary>
    /// Learn for a single connection.
	/// </summary>
    /// <param name="connection">The connection to learn from.</param>
	protected abstract void LearnConnection(IFreeformConnection connection);

    }
}
