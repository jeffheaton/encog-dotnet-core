using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.ML.Data.Market;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.ML;
using Encog.Neural.Pattern;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;

namespace CSVanalyze
{
    public class Build
    {
        #region networks builders.
        public class NetworkBuilders
        {



            /// <summary>
            /// Creates the elman network.
            /// </summary>
            /// <param name="inputsize">The inputsize.</param>
            /// <param name="outputsizes">The outputsizes.</param>
            /// <param name="hiddenLayers">The hidden layers.</param>
            /// <returns></returns>
            public static IMLMethod CreateElmanNetwork(int inputsize, int outputsizes, int hiddenLayers)
            {
                // construct an Elman type network
                ElmanPattern pattern = new ElmanPattern();
                pattern.ActivationFunction = new ActivationTANH();
                pattern.InputNeurons = inputsize;
                pattern.AddHiddenLayer(hiddenLayers);
                pattern.OutputNeurons = outputsizes;
                return pattern.Generate();
            }




            /// <summary>
            /// Creates the feedforward network.
            /// </summary>
            /// <param name="inputneurons">The inputneurons.</param>
            /// <param name="outputsneurons">The outputsneurons.</param>
            /// <param name="hiddenNeurons">The hidden neurons.</param>
            /// <returns></returns>
            public static IMLMethod CreateFeedforwardNetwork(int inputneurons, int outputsneurons, int hiddenNeurons, int hidden2)
            {
                // construct a feedforward type network
                FeedForwardPattern pattern = new FeedForwardPattern();
                pattern.ActivationFunction = new ActivationTANH();
                pattern.InputNeurons = inputneurons;
                pattern.AddHiddenLayer(hiddenNeurons);
                pattern.AddHiddenLayer(hidden2);
                pattern.OutputNeurons = outputsneurons;
                return pattern.Generate();
            }

            /// <summary>
            /// Creates the jordan network.
            /// </summary>
            /// <param name="inputneurons">The inputneurons.</param>
            /// <param name="outputsneurons">The outputsneurons.</param>
            /// <param name="hiddenNeurons">The hidden neurons.</param>
            /// <param name="hidden2">The hidden2.</param>
            /// <returns></returns>
            public static IMLMethod CreateJordanNetwork(int inputneurons, int outputsneurons, int hiddenNeurons)
            {
                // construct an Jordan type network
                var pattern = new JordanPattern
                {
                    ActivationFunction = new ActivationTANH()
                };
                pattern.InputNeurons = inputneurons;
                pattern.AddHiddenLayer(hiddenNeurons);
                pattern.OutputNeurons = outputsneurons;
                return pattern.Generate();
            }



        #endregion


        }
    }
}
