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
        public const String TypeRPROP = "rprop";

        /// <summary>
        /// String constant for backprop training.
        /// </summary>
        ///
        public const String TypeBackprop = "backprop";

        /// <summary>
        /// String constant for SCG training.
        /// </summary>
        ///
        public const String TypeSCG = "scg";

        /// <summary>
        /// String constant for LMA training.
        /// </summary>
        ///
        public const String TypeLma = "lma";

        /// <summary>
        /// String constant for SVM training.
        /// </summary>
        ///
        public const String TypeSVM = "svm-train";

        /// <summary>
        /// String constant for SVM-Search training.
        /// </summary>
        ///
        public const String TypeSVMSearch = "svm-search";

        /// <summary>
        /// String constant for SOM-Neighborhood training.
        /// </summary>
        ///
        public const String TypeSOMNeighborhood = "som-neighborhood";

        /// <summary>
        /// String constant for SOM-Cluster training.
        /// </summary>
        ///
        public const String TypeSOMCluster = "som-cluster";

        /// <summary>
        /// Property for learning rate.
        /// </summary>
        ///
        public const String PropertyLearningRate = "LR";

        /// <summary>
        /// Property for momentum.
        /// </summary>
        ///
        public const String PropertyLearningMomentum = "MOM";

        /// <summary>
        /// Property for init update.
        /// </summary>
        ///
        public const String PropertyInitialUpdate = "INIT_UPDATE";

        /// <summary>
        /// Property for max step.
        /// </summary>
        ///
        public const String PropertyMaxStep = "MAX_STEP";

        /// <summary>
        /// Property for bayes reg.
        /// </summary>
        ///
        public const String PropertyBayesianRegularization = "BAYES_REG";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma = "GAMMA";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC = "C";

        /// <summary>
        /// Property for neighborhood.
        /// </summary>
        ///
        public const String PropertyPropertyNeighborhood = "NEIGHBORHOOD";

        /// <summary>
        /// Property for iterations.
        /// </summary>
        ///
        public const String PropertyIterations = "ITERATIONS";

        /// <summary>
        /// Property for starting learning rate.
        /// </summary>
        ///
        public const String PropertyStartLearningRate = "START_LR";

        /// <summary>
        /// Property for ending learning rate.
        /// </summary>
        ///
        public const String PropertyEndLearningRate = "END_LR";

        /// <summary>
        /// Property for starting radius.
        /// </summary>
        ///
        public const String PropertyStartRadius = "START_RADIUS";

        /// <summary>
        /// Property for ending radius.
        /// </summary>
        ///
        public const String PropertyEndRadius = "END_RADIUS";

        /// <summary>
        /// Property for neighborhood.
        /// </summary>
        ///
        public const String PropertyNeighborhood = "NEIGHBORHOOD";

        /// <summary>
        /// Property for rbf type.
        /// </summary>
        ///
        public const String PropertyRBFType = "RBF_TYPE";

        /// <summary>
        /// Property for dimensions.
        /// </summary>
        ///
        public const String PropertyDimensions = "DIM";

        /// <summary>
        /// The number of cycles.
        /// </summary>
        ///
        public const String Cycles = "cycles";

        /// <summary>
        /// The starting temperature.
        /// </summary>
        ///
        public const String PropertyTemperatureStart = "startTemp";

        /// <summary>
        /// The ending temperature.
        /// </summary>
        ///
        public const String PropertyTemperatureStop = "stopTemp";

        /// <summary>
        /// Use simulated annealing.
        /// </summary>
        ///
        public const String TypeAnneal = "anneal";

        /// <summary>
        /// Population size.
        /// </summary>
        ///
        public const String PropertyPopulationSize = "population";

        /// <summary>
        /// Percent to mutate.
        /// </summary>
        ///
        public const String PropertyMutation = "mutate";

        /// <summary>
        /// Percent to mate.
        /// </summary>
        ///
        public const String PropertyMate = "mate";

        /// <summary>
        /// Genetic training.
        /// </summary>
        ///
        public const String TypeGenetic = "genetic";

        /// <summary>
        /// Manhattan training.
        /// </summary>
        ///
        public const String TypeManhattan = "manhattan";

        /// <summary>
        /// RBF-SVD training.
        /// </summary>
        ///
        public const String TypeSvd = "rbf-svd";

        /// <summary>
        /// PNN training.
        /// </summary>
        ///
        public const String TypePNN = "pnn";

        /// <summary>
        /// The factory for simulated annealing.
        /// </summary>
        ///
        private readonly AnnealFactory _annealFactory;

        /// <summary>
        /// The factory for backprop.
        /// </summary>
        ///
        private readonly BackPropFactory _backpropFactory;

        /// <summary>
        /// The factory for genetic.
        /// </summary>
        ///
        private readonly GeneticFactory _geneticFactory;

        /// <summary>
        /// The factory for LMA.
        /// </summary>
        ///
        private readonly LMAFactory _lmaFactory;

        /// <summary>
        /// The factory for Manhattan networks.
        /// </summary>
        ///
        private readonly ManhattanFactory _manhattanFactory;

        /// <summary>
        /// The factory for neighborhood SOM.
        /// </summary>
        ///
        private readonly NeighborhoodSOMFactory _neighborhoodFactory;

        /// <summary>
        /// Factory for PNN.
        /// </summary>
        ///
        private readonly PNNTrainFactory _pnnFactory;

        /// <summary>
        /// The factory for RPROP.
        /// </summary>
        ///
        private readonly RPROPFactory _rpropFactory;

        /// <summary>
        /// The factory for SCG.
        /// </summary>
        ///
        private readonly SCGFactory _scgFactory;

        /// <summary>
        /// The factory for SOM cluster.
        /// </summary>
        ///
        private readonly ClusterSOMFactory _somClusterFactory;

        /// <summary>
        /// Factory for SVD.
        /// </summary>
        ///
        private readonly RBFSVDFactory _svdFactory;

        /// <summary>
        /// THe factory for basic SVM.
        /// </summary>
        ///
        private readonly SVMFactory _svmFactory;

        /// <summary>
        /// The factory for SVM-Search.
        /// </summary>
        ///
        private readonly SVMSearchFactory _svmSearchFactory;

        /// <summary>
        /// Construct the boject.
        /// </summary>
        public MLTrainFactory()
        {
            _backpropFactory = new BackPropFactory();
            _lmaFactory = new LMAFactory();
            _rpropFactory = new RPROPFactory();
            _svmFactory = new SVMFactory();
            _svmSearchFactory = new SVMSearchFactory();
            _scgFactory = new SCGFactory();
            _annealFactory = new AnnealFactory();
            _neighborhoodFactory = new NeighborhoodSOMFactory();
            _somClusterFactory = new ClusterSOMFactory();
            _geneticFactory = new GeneticFactory();
            _manhattanFactory = new ManhattanFactory();
            _svdFactory = new RBFSVDFactory();
            _pnnFactory = new PNNTrainFactory();
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
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String type, String args)
        {
            String args2 = args ?? "";

            if (TypeRPROP.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _rpropFactory.Create(method, training, args2);
            }
            if (TypeBackprop.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _backpropFactory.Create(method, training, args2);
            }
            if (TypeSCG.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _scgFactory.Create(method, training, args2);
            }
            if (TypeLma.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _lmaFactory.Create(method, training, args2);
            }
            if (TypeSVM.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _svmFactory.Create(method, training, args2);
            }
            if (TypeSVMSearch.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _svmSearchFactory.Create(method, training, args2);
            }
            if (TypeSOMNeighborhood.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _neighborhoodFactory.Create(method, training, args2);
            }
            if (TypeAnneal.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _annealFactory.Create(method, training, args2);
            }
            if (TypeGenetic.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _geneticFactory.Create(method, training, args2);
            }
            if (TypeSOMCluster.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _somClusterFactory.Create(method, training, args2);
            }
            if (TypeManhattan.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _manhattanFactory.Create(method, training, args2);
            }
            if (TypeSvd.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _svdFactory.Create(method, training, args2);
            }
            if (TypePNN.Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                return _pnnFactory.Create(method, training, args2);
            }
            
            throw new EncogError("Unknown training type: " + type);
        }
    }
}
