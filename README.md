![Encog Machine Learning Framework](http://www.heatonresearch.com/images/encog128.png)

**Note: The name "Encog Core" causes some confusion with Microsoft .Net Core. This version of Encog has nothing to do with Microsoft's .Net Core release.  I am tempted to change the name of the Repo and NuGet, but that would be a massive [breaking change](https://en.wiktionary.org/wiki/breaking_change) at this point.**

Encog Machine Learning Framework
--------------------------------
Encog is a pure-Java/C# machine learning framework that I created back in 2008 to support genetic programming, NEAT/HyperNEAT, and other neural network technologies.  Originally, Encog was created to support research for my master’s degree and early books.  The neural network aspects of Encog proved popular, and Encog was used by a number of people and is cited by [952 academic papers in Google Scholar](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C26&q=encog&btnG=). I created Encog at a time when there were not so many well developed frameworks, such as [TensorFlow](https://medium.com/tensorflow/standardizing-on-keras-guidance-on-high-level-apis-in-tensorflow-2-0-bad2b04c819a), [Keras](https://keras.io/), [DeepLearning4J](https://deeplearning4j.org/), and many others (these are the frameworks I work with the most these days for neural networks).   

Encog continues to be developed (and bugs fixed) for the types of models not covered by the large frameworks and to provide a pure non-GPU Java/C# implementation of several classic neural networks.  Because it is pure Java, the source code for Encog can be much simpler to adapt for cases where you want to implement the neural network yourself from scratch. Some of the less mainstream technologies supported by Encog include [NEAT](https://en.wikipedia.org/wiki/Neuroevolution_of_augmenting_topologies), [HyperNEAT](https://en.wikipedia.org/wiki/HyperNEAT), and [Genetic Programming](https://en.wikipedia.org/wiki/Genetic_programming). Encog has minimal support for computer vision.  Computer vision is a fascinating topic, but just has never been a research interest of mine.

Encog supports a variety of advanced algorithms, as well as support classes to normalize and process data. Machine learning algorithms such as Support Vector Machines, Neural Networks, Bayesian Networks, Hidden Markov Models, Genetic Programming and Genetic Algorithms are supported. Most Encog training algorithms are multi-threaded and scale well to multicore hardware.

Encog continues to be developed, and is used in my own research, for areas that I need Java and are not covered by Keras.  However, for larger-scale cutting edge work, where I do not need to implement the technology from scratch, I make use of Keras/TensorFlow for my own work.

Simple C# XOR Example in Encog
--------------------------------

```c#
using System;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Examples.XOR
{
    public class XORHelloWorld : IExample
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XORInput = {
            new[] {0.0, 0.0},
            new[] {1.0, 0.0},
            new[] {0.0, 1.0},
            new[] {1.0, 1.0}
        };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal = {
            new[] {0.0},
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}
        };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(XORHelloWorld),
                    "xor",
                    "Simple XOR with backprop, no factories or helper functions.",
                    "This example shows how to train an XOR with no factories or helper functions.");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            // create training data
            IMLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);

            // train the neural network
            IMLTrain train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01);

            // test the neural network
            Console.WriteLine(@"Neural Network Results:");
            foreach (IMLDataPair pair in trainingSet)
            {
                IMLData output = network.Compute(pair.Input);
                Console.WriteLine(pair.Input[0] + @"," + pair.Input[1]
                                  + @", actual=" + output[0] + @",ideal=" + pair.Ideal[0]);
            }
        }

        #endregion
    }
}
```
