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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Factory;
using Encog.ML.Factory.Train;
using Encog.ML.Train;

namespace Encog.Plugin.SystemPlugin
{
    /// <summary>
    /// Create the system training methods.
    /// </summary>
    public class SystemTrainingPlugin : IEncogPluginService1
    {
        /// <summary>
        /// The factory for simulated annealing.
        /// </summary>
        private readonly AnnealFactory annealFactory = new AnnealFactory();

        /// <summary>
        /// The factory for backprop.
        /// </summary>
        private readonly BackPropFactory backpropFactory = new BackPropFactory();

        /// <summary>
        /// The factory for K2
        /// </summary>
        private readonly TrainBayesianFactory bayesianFactory = new TrainBayesianFactory();

        /// <summary>
        /// The factory for genetic.
        /// </summary>
        private readonly GeneticFactory geneticFactory = new GeneticFactory();

        /// <summary>
        /// The factory for LMA.
        /// </summary>
        private readonly LMAFactory lmaFactory = new LMAFactory();

        /// <summary>
        /// The factory for Manhattan networks.
        /// </summary>
        private readonly ManhattanFactory manhattanFactory = new ManhattanFactory();

        /// <summary>
        /// The factory for neighborhood SOM.
        /// </summary>
        private readonly NeighborhoodSOMFactory neighborhoodFactory
            = new NeighborhoodSOMFactory();

        /// <summary>
        /// Nelder Mead Factory.
        /// </summary>
        private readonly NelderMeadFactory nmFactory = new NelderMeadFactory();

        /// <summary>
        /// Factory for PNN.
        /// </summary>
        private readonly PNNTrainFactory pnnFactory = new PNNTrainFactory();

        /// <summary>
        /// PSO training factory.
        /// </summary>
        private readonly PSOFactory psoFactory = new PSOFactory();

        /// <summary>
        /// Factory for quick prop.
        /// </summary>
        private readonly QuickPropFactory qpropFactory = new QuickPropFactory();

        /// <summary>
        /// The factory for RPROP.
        /// </summary>
        private readonly RPROPFactory rpropFactory = new RPROPFactory();

        /// <summary>
        /// The factory for SCG.
        /// </summary>
        private readonly SCGFactory scgFactory = new SCGFactory();

        /// <summary>
        /// The factory for SOM cluster.
        /// </summary>
        private readonly ClusterSOMFactory somClusterFactory = new ClusterSOMFactory();

        /// <summary>
        /// Factory for SVD.
        /// </summary>
        private readonly RBFSVDFactory svdFactory = new RBFSVDFactory();

        /// <summary>
        /// The factory for basic SVM.
        /// </summary>
        private readonly SVMFactory svmFactory = new SVMFactory();

        /// <summary>
        /// The factory for SVM-Search.
        /// </summary>
        private readonly SVMSearchFactory svmSearchFactory = new SVMSearchFactory();

        private readonly NEATGAFactory neatGAFactory = new NEATGAFactory();

        private readonly EPLGAFactory eplTrainFctory = new EPLGAFactory();

        #region IEncogPluginService1 Members

        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in training " +
                       "methods for Encog.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get { return "HRI-System-Training"; }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get { return 1; }
        }


        /// <summary>
        /// This plugin does not support activation functions, so it will 
        /// always return null. 
        /// </summary>
        /// <param name="name">Not used.</param>
        /// <returns>The activation function.</returns>
        public IActivationFunction CreateActivationFunction(String name)
        {
            return null;
        }

        public IMLMethod CreateMethod(String methodType, String architecture,
                                      int input, int output)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public IMLTrain CreateTraining(IMLMethod method, IMLDataSet training,
                                       String type, String args)
        {
            String args2 = args;

            if (args2 == null)
            {
                args2 = "";
            }

            if (String.Compare(MLTrainFactory.TypeRPROP, type) == 0)
            {
                return rpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeBackprop, type) == 0)
            {
                return backpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSCG, type) == 0)
            {
                return scgFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeLma, type) == 0)
            {
                return lmaFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSVM, type) == 0)
            {
                return svmFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSVMSearch, type) == 0)
            {
                return svmSearchFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMNeighborhood, type) == 0)
            {
                return neighborhoodFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeAnneal, type) == 0)
            {
                return annealFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeGenetic, type) == 0)
            {
                return geneticFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMCluster, type) == 0)
            {
                return somClusterFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeManhattan, type) == 0)
            {
                return manhattanFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSvd, type) == 0)
            {
                return svdFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypePNN, type) == 0)
            {
                return pnnFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeQPROP, type) == 0)
            {
                return qpropFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeBayesian.Equals(type))
            {
                return bayesianFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeNelderMead.Equals(type))
            {
                return nmFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypePSO.Equals(type))
            {
                return psoFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeNEATGA.Equals(type))
            {
                return this.neatGAFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeEPLGA.Equals(type))
            {
                return this.eplTrainFctory.Create(method, training, args2);
            }
            else
            {
                throw new EncogError("Unknown training type: " + type);
            }
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get { return EncogPluginBaseConst.SERVICE_TYPE_GENERAL; }
        }

        #endregion
    }
}
