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
using Encog.ML.Data;
using Encog.ML.Factory.Train;
using Encog.ML.Train;
using Encog.Plugin;

namespace Encog.ML.Factory
{
    /// <summary>
    /// This factory is used to create trainers for machine learning methods.
    /// </summary>
    ///
    public class MLTrainFactory
    {
        /// <summary>
        /// The maximum number of parents for K2.
        /// </summary>
        public const string PropertyMaxParents = "MAXPARENTS";


        ///<summary>
        /// The number of particles.
        ///</summary>
        public const String PropertyParticles = "PARTICLES";

        /// <summary>
        /// Nelder Mead training for Bayesian.
        /// </summary>
        public const String TypeNelderMead = "nm";

        /// <summary>
        /// K2 training for Bayesian.
        /// </summary>
        public const String TypeBayesian = "bayesian";

        /// <summary>
        /// PSO training.
        /// </summary>
        public const String TypePSO = "pso";

        /// <summary>
        /// String constant for RPROP training.
        /// </summary>
        ///
        public const String TypeRPROP = "rprop";

        /// <summary>
        /// String constant for RPROP training.
        /// </summary>
        ///
        public const String TypeQPROP = "qprop";

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
        /// String constant for LMA training.
        /// </summary>
        public const String TypeNEATGA = "neat-ga";

        /// <summary>
        /// String constant for LMA training.
        /// </summary>
        public const String TypeEPLGA = "epl-ga";

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
        /// Construct the boject.
        /// </summary>
        public MLTrainFactory()
        {
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
            foreach (EncogPluginBase plugin in EncogFramework.Instance.Plugins)
            {
                if (plugin is IEncogPluginService1)
                {
                    IMLTrain result = ((IEncogPluginService1)plugin).CreateTraining(
                            method, training, type, args);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            throw new EncogError("Unknown training type: " + type);
        }
    }
}
