using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Factory.Method;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Factory;
using Encog.ML.Data;
using Encog.ML.Train;

namespace Encog.Plugin.SystemPlugin
{
    public class SystemMethodsPlugin : EncogPluginBase
    {
        /// <summary>
        /// A factory used to create feedforward neural networks.
        /// </summary>
        private FeedforwardFactory feedforwardFactory
            = new FeedforwardFactory();

        /// <summary>
        /// A factory used to create support vector machines.
        /// </summary>
        private SVMFactory svmFactory = new SVMFactory();

        /// <summary>
        /// A factory used to create RBF networks.
        /// </summary>
        private RBFNetworkFactory rbfFactory = new RBFNetworkFactory();

        /// <summary>
        /// The factory for PNN's.
        /// </summary>
        private PNNFactory pnnFactory = new PNNFactory();

        /// <summary>
        /// A factory used to create SOM's.
        /// </summary>
        private SOMFactory somFactory = new SOMFactory();


        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in machine " +
                        "learning methods for Encog.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get
            {
                return "HRI-System-Methods";
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


        /// <inheritdoc/>
        public IActivationFunction createActivationFunction(String name)
        {
            return null;
        }

        /// <inheritdoc/>
        public IMLMethod createMethod(String methodType, String architecture,
                int input, int output)
        {
            if (MLMethodFactory.TypeFeedforward.Equals(methodType))
            {
                return this.feedforwardFactory.Create(architecture, input, output);
            }
            else if (MLMethodFactory.TypeRbfnetwork.Equals(methodType))
            {
                return this.rbfFactory.Create(architecture, input, output);
            }
            else if (MLMethodFactory.TypeSVM.Equals(methodType))
            {
                return this.svmFactory.Create(architecture, input, output);
            }
            else if (MLMethodFactory.TypeSVM.Equals(methodType))
            {
                return this.somFactory.Create(architecture, input, output);
            }
            else if (MLMethodFactory.TypePNN.Equals(methodType))
            {
                return this.pnnFactory.Create(architecture, input, output);
            }
            throw new EncogError("Unknown method type: " + methodType);
        }

        /// <inheritdoc/>
        public IMLTrain createTraining(IMLMethod method, IMLDataSet training,
                String type, String args)
        {
            return null;
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
