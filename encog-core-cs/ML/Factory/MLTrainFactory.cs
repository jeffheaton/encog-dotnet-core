//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.ML.Factory.Train;
using Encog.ML.Train;

namespace Encog.ML.Factory
{
    /// <summary>
    /// This factory is used to create trainers for machine learning methods.
    /// </summary>
    ///
    public class MLTrainFactory
    {
        /// <summary>
        /// String constant for RPROP training.
        /// </summary>
        ///
        public const String TYPE_RPROP = "rprop";

        /// <summary>
        /// String constant for backprop training.
        /// </summary>
        ///
        public const String TYPE_BACKPROP = "backprop";

        /// <summary>
        /// String constant for SCG training.
        /// </summary>
        ///
        public const String TYPE_SCG = "scg";

        /// <summary>
        /// String constant for LMA training.
        /// </summary>
        ///
        public const String TYPE_LMA = "lma";

        /// <summary>
        /// String constant for SVM training.
        /// </summary>
        ///
        public const String TYPE_SVM = "svm-train";

        /// <summary>
        /// String constant for SVM-Search training.
        /// </summary>
        ///
        public const String TYPE_SVM_SEARCH = "svm-search";

        /// <summary>
        /// String constant for SOM-Neighborhood training.
        /// </summary>
        ///
        public const String TYPE_SOM_NEIGHBORHOOD = "som-neighborhood";

        /// <summary>
        /// String constant for SOM-Cluster training.
        /// </summary>
        ///
        public const String TYPE_SOM_CLUSTER = "som-cluster";

        /// <summary>
        /// Property for learning rate.
        /// </summary>
        ///
        public const String PROPERTY_LEARNING_RATE = "LR";

        /// <summary>
        /// Property for momentum.
        /// </summary>
        ///
        public const String PROPERTY_LEARNING_MOMENTUM = "MOM";

        /// <summary>
        /// Property for init update.
        /// </summary>
        ///
        public const String PROPERTY_INITIAL_UPDATE = "INIT_UPDATE";

        /// <summary>
        /// Property for max step.
        /// </summary>
        ///
        public const String PROPERTY_MAX_STEP = "MAX_STEP";

        /// <summary>
        /// Property for bayes reg.
        /// </summary>
        ///
        public const String PROPERTY_BAYESIAN_REGULARIZATION = "BAYES_REG";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PROPERTY_GAMMA = "GAMMA";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PROPERTY_C = "C";

        /// <summary>
        /// Property for neighborhood.
        /// </summary>
        ///
        public const String PROPERTY_PROPERTY_NEIGHBORHOOD = "NEIGHBORHOOD";

        /// <summary>
        /// Property for iterations.
        /// </summary>
        ///
        public const String PROPERTY_ITERATIONS = "ITERATIONS";

        /// <summary>
        /// Property for starting learning rate.
        /// </summary>
        ///
        public const String PROPERTY_START_LEARNING_RATE = "START_LR";

        /// <summary>
        /// Property for ending learning rate.
        /// </summary>
        ///
        public const String PROPERTY_END_LEARNING_RATE = "END_LR";

        /// <summary>
        /// Property for starting radius.
        /// </summary>
        ///
        public const String PROPERTY_START_RADIUS = "START_RADIUS";

        /// <summary>
        /// Property for ending radius.
        /// </summary>
        ///
        public const String PROPERTY_END_RADIUS = "END_RADIUS";

        /// <summary>
        /// Property for neighborhood.
        /// </summary>
        ///
        public const String PROPERTY_NEIGHBORHOOD = "NEIGHBORHOOD";

        /// <summary>
        /// Property for rbf type.
        /// </summary>
        ///
        public const String PROPERTY_RBF_TYPE = "RBF_TYPE";

        /// <summary>
        /// Property for dimensions.
        /// </summary>
        ///
        public const String PROPERTY_DIMENSIONS = "DIM";

        /// <summary>
        /// The number of cycles.
        /// </summary>
        ///
        public const String CYCLES = "cycles";

        /// <summary>
        /// The starting temperature.
        /// </summary>
        ///
        public const String PROPERTY_TEMPERATURE_START = "startTemp";

        /// <summary>
        /// The ending temperature.
        /// </summary>
        ///
        public const String PROPERTY_TEMPERATURE_STOP = "stopTemp";

        /// <summary>
        /// Use simulated annealing.
        /// </summary>
        ///
        public const String TYPE_ANNEAL = "anneal";

        /// <summary>
        /// Population size.
        /// </summary>
        ///
        public const String PROPERTY_POPULATION_SIZE = "population";

        /// <summary>
        /// Percent to mutate.
        /// </summary>
        ///
        public const String PROPERTY_MUTATION = "mutate";

        /// <summary>
        /// Percent to mate.
        /// </summary>
        ///
        public const String PROPERTY_MATE = "mate";

        /// <summary>
        /// Genetic training.
        /// </summary>
        ///
        public const String TYPE_GENETIC = "genetic";

        /// <summary>
        /// Manhattan training.
        /// </summary>
        ///
        public const String TYPE_MANHATTAN = "manhattan";

        /// <summary>
        /// RBF-SVD training.
        /// </summary>
        ///
        public const String TYPE_SVD = "rbf-svd";

        /// <summary>
        /// PNN training.
        /// </summary>
        ///
        public const String TYPE_PNN = "pnn";

        /// <summary>
        /// The factory for simulated annealing.
        /// </summary>
        ///
        private readonly AnnealFactory annealFactory;

        /// <summary>
        /// The factory for backprop.
        /// </summary>
        ///
        private readonly BackPropFactory backpropFactory;

        /// <summary>
        /// The factory for genetic.
        /// </summary>
        ///
        private readonly GeneticFactory geneticFactory;

        /// <summary>
        /// The factory for LMA.
        /// </summary>
        ///
        private readonly LMAFactory lmaFactory;

        /// <summary>
        /// The factory for Manhattan networks.
        /// </summary>
        ///
        private readonly ManhattanFactory manhattanFactory;

        /// <summary>
        /// The factory for neighborhood SOM.
        /// </summary>
        ///
        private readonly NeighborhoodSOMFactory neighborhoodFactory;

        /// <summary>
        /// Factory for PNN.
        /// </summary>
        ///
        private readonly PNNTrainFactory pnnFactory;

        /// <summary>
        /// The factory for RPROP.
        /// </summary>
        ///
        private readonly RPROPFactory rpropFactory;

        /// <summary>
        /// The factory for SCG.
        /// </summary>
        ///
        private readonly SCGFactory scgFactory;

        /// <summary>
        /// The factory for SOM cluster.
        /// </summary>
        ///
        private readonly ClusterSOMFactory somClusterFactory;

        /// <summary>
        /// Factory for SVD.
        /// </summary>
        ///
        private readonly RBFSVDFactory svdFactory;

        /// <summary>
        /// THe factory for basic SVM.
        /// </summary>
        ///
        private readonly SVMFactory svmFactory;

        /// <summary>
        /// The factory for SVM-Search.
        /// </summary>
        ///
        private readonly SVMSearchFactory svmSearchFactory;

        /// <summary>
        /// Construct the boject.
        /// </summary>
        public MLTrainFactory()
        {
            backpropFactory = new BackPropFactory();
            lmaFactory = new LMAFactory();
            rpropFactory = new RPROPFactory();
            svmFactory = new SVMFactory();
            svmSearchFactory = new SVMSearchFactory();
            scgFactory = new SCGFactory();
            annealFactory = new AnnealFactory();
            neighborhoodFactory = new NeighborhoodSOMFactory();
            somClusterFactory = new ClusterSOMFactory();
            geneticFactory = new GeneticFactory();
            manhattanFactory = new ManhattanFactory();
            svdFactory = new RBFSVDFactory();
            pnnFactory = new PNNTrainFactory();
        }

        /// <summary>
        /// Create a trainer.
        /// </summary>
        ///
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="type">Type type of trainer.</param>
        /// <param name="args">The training args.</param>
        /// <returns>The new training method.</returns>
        public MLTrain Create(IMLMethod method,
                              IMLDataSet training, String type, String args)
        {
            String args2 = args;

            if (args2 == null)
            {
                args2 = "";
            }

            if (TYPE_RPROP.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return rpropFactory.Create(method, training, args2);
            }
            else if (TYPE_BACKPROP.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return backpropFactory.Create(method, training, args2);
            }
            else if (TYPE_SCG.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return scgFactory.Create(method, training, args2);
            }
            else if (TYPE_LMA.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return lmaFactory.Create(method, training, args2);
            }
            else if (TYPE_SVM.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return svmFactory.Create(method, training, args2);
            }
            else if (TYPE_SVM_SEARCH.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return svmSearchFactory.Create(method, training, args2);
            }
            else if (TYPE_SOM_NEIGHBORHOOD.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return neighborhoodFactory.Create(method, training, args2);
            }
            else if (TYPE_ANNEAL.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return annealFactory.Create(method, training, args2);
            }
            else if (TYPE_GENETIC.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return geneticFactory.Create(method, training, args2);
            }
            else if (TYPE_SOM_CLUSTER.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return somClusterFactory.Create(method, training, args2);
            }
            else if (TYPE_MANHATTAN.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return manhattanFactory.Create(method, training, args2);
            }
            else if (TYPE_SVD.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return svdFactory.Create(method, training, args2);
            }
            else if (TYPE_PNN.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return pnnFactory.Create(method, training, args2);
            }
            else
            {
                throw new EncogError("Unknown training type: " + type);
            }
        }
    }
}
