using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Factory.Train;
using Encog.ML.Factory;
using Encog.ML.Train;
using Encog.ML;
using Encog.ML.Data;
using Encog.Engine.Network.Activation;

namespace Encog.Plugin.SystemPlugin
{
    /// <summary>
    /// Create the system training methods.
    /// </summary>
    public class SystemTrainingPlugin : IEncogPluginService1
    {
        /// <summary>
        /// The factory for backprop.
        /// </summary>
        private BackPropFactory backpropFactory = new BackPropFactory();

        /// <summary>
        /// The factory for LMA.
        /// </summary>
        private LMAFactory lmaFactory = new LMAFactory();

        /// <summary>
        /// The factory for RPROP.
        /// </summary>
        private RPROPFactory rpropFactory = new RPROPFactory();

        /// <summary>
        /// The factory for basic SVM.
        /// </summary>
        private SVMFactory svmFactory = new SVMFactory();

        /// <summary>
        /// The factory for SVM-Search.
        /// </summary>
        private SVMSearchFactory svmSearchFactory = new SVMSearchFactory();

        /// <summary>
        /// The factory for SCG.
        /// </summary>
        private SCGFactory scgFactory = new SCGFactory();

        /// <summary>
        /// The factory for simulated annealing.
        /// </summary>
        private AnnealFactory annealFactory = new AnnealFactory();

        /// <summary>
        /// The factory for neighborhood SOM.
        /// </summary>
        private NeighborhoodSOMFactory neighborhoodFactory
            = new NeighborhoodSOMFactory();

        /// <summary>
        /// The factory for SOM cluster.
        /// </summary>
        private ClusterSOMFactory somClusterFactory = new ClusterSOMFactory();

        /// <summary>
        /// The factory for genetic.
        /// </summary>
        private GeneticFactory geneticFactory = new GeneticFactory();

        /// <summary>
        /// The factory for Manhattan networks.
        /// </summary>
        private ManhattanFactory manhattanFactory = new ManhattanFactory();

        /// <summary>
        /// Factory for SVD.
        /// </summary>
        private RBFSVDFactory svdFactory = new RBFSVDFactory();

        /// <summary>
        /// Factory for PNN.
        /// </summary>
        private PNNTrainFactory pnnFactory = new PNNTrainFactory();

        //private QuickPropFactory qpropFactory = new QuickPropFactory(); 

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
            get
            {
                return "HRI-System-Training";
            }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get
            {
                return 1;
            }
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
                return this.rpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeBackprop, type) == 0)
            {
                return this.backpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSCG, type) == 0)
            {
                return this.scgFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeLma, type) == 0)
            {
                return this.lmaFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSVM, type) == 0)
            {
                return this.svmFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSVMSearch, type) == 0)
            {
                return this.svmSearchFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMNeighborhood, type) == 0)
            {
                return this.neighborhoodFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeAnneal, type) == 0)
            {
                return this.annealFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeGenetic, type) == 0)
            {
                return this.geneticFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMCluster, type) == 0)
            {
                return this.somClusterFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeManhattan, type) == 0)
            {
                return this.manhattanFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSvd, type) == 0)
            {
                return this.svdFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypePNN, type) == 0)
            {
                return this.pnnFactory.Create(method, training, args2);
            } /*else if (MLTrainFactory.TYPE_QPROP.equalsIgnoreCase(type)) {
			return this.qpropFactory.Create(method, training, args2);
		}*/ else
            {
                throw new EncogError("Unknown training type: " + type);
            }
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get
            {
                return EncogPluginBaseConst.SERVICE_TYPE_GENERAL;
            }
        }
    }
}
