using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Training;
using System.IO;
using Encog.Neural.NeuralData.CSV;
using Encog.Neural.Data.Buffer;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.SCG;

namespace Encog.Util.Simple
{
    /// <summary>
    /// General utility class for Encog.  Provides for some common Encog procedures.
    /// </summary>
    public class EncogUtility
    {
        
	/**
	 * Private constructor.
	 */
	private EncogUtility() {
		
	}
	
	/**
	 * Convert a CSV file to a binary training file.
	 * @param csvFile The CSV file.
	 * @param binFile The binary file.
	 * @param inputCount The number of input values.
	 * @param outputCount The number of output values.
	 * @param headers True, if there are headers on the3 CSV.
	 */
	public static void convertCSV2Binary( String csvFile,
			 String binFile,  int inputCount,  int outputCount,
			 bool headers) {
        
		File.Delete(binFile);
		 CSVNeuralDataSet csv = new CSVNeuralDataSet(csvFile.ToString(),
				inputCount, outputCount, false);
		 BufferedNeuralDataSet buffer = new BufferedNeuralDataSet(binFile);
		buffer.BeginLoad(50, 6);
		foreach (INeuralDataPair pair in csv) {
			buffer.Add(pair);
		}
		buffer.EndLoad();
	}

	/**
	 * Evaluate the network and display (to the console) the output for every
	 * value in the training set. Displays ideal and actual.
	 * 
	 * @param network
	 *            The network to evaluate.
	 * @param training
	 *            The training set to evaluate.
	 */
	public static void evaluate( BasicNetwork network,
			 INeuralDataSet training) {
		foreach ( INeuralDataPair pair in training) {
			 INeuralData output = network.Compute(pair.Input);
			Console.WriteLine("Input="
					+ EncogUtility.formatNeuralData(pair.Input)
					+ ", Actual=" + EncogUtility.formatNeuralData(output)
					+ ", Ideal="
					+ EncogUtility.formatNeuralData(pair.Ideal));

		}
	}

	/**
	 * Format neural data as a list of numbers.
	 * @param data The neural data to format.
	 * @return The formatted neural data.
	 */
	private static String formatNeuralData( INeuralData data) {
		 StringBuilder result = new StringBuilder();
		for (int i = 0; i < data.Count; i++) {
			if (i != 0) {
				result.Append(',');
			}
			result.Append(Format.FormatDouble(data[i], 4));
		}
		return result.ToString();
	}

	/**
	 * Create a simple feedforward neural network.
	 * 
	 * @param input
	 *            The number of input neurons.
	 * @param hidden1
	 *            The number of hidden layer 1 neurons.
	 * @param hidden2
	 *            The number of hidden layer 2 neurons.
	 * @param output
	 *            The number of output neurons.
	 * @param tanh
	 *            True to use hyperbolic tangent activation function, false to
	 *            use the sigmoid activation function.
	 * @return
	 * 			The neural network.
	 */
	public static BasicNetwork simpleFeedForward( int input,
			 int hidden1,  int hidden2,  int output,
			 bool tanh) {
		 FeedForwardPattern pattern = new FeedForwardPattern();
		pattern.InputNeurons = input;
		pattern.OutputNeurons = output;
		if (tanh) {
			pattern.ActivationFunction = new ActivationTANH();
		} else {
			pattern.ActivationFunction = new ActivationSigmoid();
		}

		if (hidden1 > 0) {
			pattern.AddHiddenLayer(hidden1);
		}
		if (hidden2 > 0) {
			pattern.AddHiddenLayer(hidden2);
		}

		 BasicNetwork network = pattern.Generate();
		network.Reset();
		return network;
	}

	/**
	 * Train the neural network, using SCG training, and output status to the
	 * console.
	 * 
	 * @param network
	 *            The network to train.
	 * @param trainingSet
	 *            The training set.
	 * @param minutes
	 *            The number of minutes to train for.
	 */
	public static void trainConsole( BasicNetwork network,
			 INeuralDataSet trainingSet,  int minutes) {
		 Propagation train = new ScaledConjugateGradient(network,
				trainingSet);
		train.NumThreads = 0;
		EncogUtility.trainConsole(train, network, trainingSet, minutes);
	}

	/**
	 * Train the network, using the specified training algorithm, and send the
	 * output to the console.
	 * 
	 * @param train
	 *            The training method to use.
	 * @param network
	 *            The network to train.
	 * @param trainingSet
	 *            The training set.
	 * @param minutes
	 *            The number of minutes to train for.
	 */
	public static void trainConsole( ITrain train,
			 BasicNetwork network,  INeuralDataSet trainingSet,
			 int minutes) {

		int epoch = 1;
		long remaining;

		Console.WriteLine("Beginning training...");
		long start = Environment.TickCount;
		do {
			train.Iteration();

			long current = Environment.TickCount ;
			long elapsed = (current - start) / 10000;// mili
            elapsed /= 1000; // second
			remaining = minutes - elapsed / 60;

			Console.WriteLine("Iteration #" + Format.FormatInteger(epoch)
					+ " Error:" + Format.FormatPercent(train.Error)
					+ " elapsed time = " + Format.FormatTimeSpan((int) elapsed)
					+ " time left = "
					+ Format.FormatTimeSpan((int) remaining * 60));
			epoch++;
		} while (remaining > 0);
	}

	/**
	 * Train using SCG and display progress to a dialog box.
	 * @param network The network to train.
	 * @param trainingSet The training set to use.
	 */
	public static void trainDialog( BasicNetwork network,
			 INeuralDataSet trainingSet) {
		 Propagation train = new ScaledConjugateGradient(network,
				trainingSet);
		train.NumThreads = 0;
		EncogUtility.trainDialog(train, network, trainingSet);
	}

	/**
	 * Train, using the specified training method, display progress to a dialog
	 * box.
	 * 
	 * @param train
	 *            The training method to use.
	 * @param network
	 *            The network to train.
	 * @param trainingSet
	 *            The training set to use.
	 */
	public static void trainDialog(ITrain train,
			BasicNetwork network, INeuralDataSet trainingSet) {

		/*int epoch = 1;
		TrainingDialog dialog = new TrainingDialog();
		dialog.setVisible(true);

		long start = System.currentTimeMillis();
		do {
			train.iteration();

			long current = System.currentTimeMillis();
			long elapsed = (current - start) / 1000;// seconds
			dialog.setIterations(epoch);
			dialog.setError(train.getError());
			dialog.setTime((int) elapsed);
			epoch++;
		} while (!dialog.shouldStop());
		dialog.dispose();*/
	}

	/**
	 * Train the network, to a specific error, send the output to the console.
	 * @param network The network to train.
	 * @param trainingSet The training set to use.
	 * @param error The error level to train to.
	 */
	public static void trainToError( BasicNetwork network,
			 INeuralDataSet trainingSet,  double error) {
		/* Propagation train = new ScaledConjugateGradient(network,
				trainingSet);
		train.setNumThreads(0);
		EncogUtility.trainToError(train, network, trainingSet, error);*/
	}

	/**
	 * Train to a specific error, using the specified training method, send the
	 * output to the console.
	 * 
	 * @param train
	 *            The training method.
	 * @param network
	 *            The network to train.
	 * @param trainingSet
	 *            The training set to use.
	 * @param error
	 *            The desired error level.
	 */
	public static void trainToError(ITrain train,
			BasicNetwork network, INeuralDataSet trainingSet,
			double error) {

		int epoch = 1;

		Console.WriteLine("Beginning training...");

		do {
			train.Iteration();

			Console.WriteLine("Iteration #" + Format.FormatInteger(epoch)
					+ " Error:" + Format.FormatPercent(train.Error)
					+ " Target Error: " + Format.FormatPercent(error));
			epoch++;
		} while (train.Error > error);
	}
    }
}
