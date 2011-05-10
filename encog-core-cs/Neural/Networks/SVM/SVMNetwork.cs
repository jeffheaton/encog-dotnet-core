using Encog.MathUtil.LIBSVM;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Persist;

namespace Encog.Neural.Networks.SVM
{
    /// <summary>
    /// This is a network that is backed by one or more Support Vector Machines
    /// (SVM). It is designed to function very similarly to an Encog neural network,
    /// and is largely interchangeable with an Encog neural network.
    /// 
    /// The support vector machine supports several types. Regression is used when
    /// you want the network to predict a value, given the input. Function
    /// approximation is a good example of regression. Classification is used when
    /// you want the SVM to group the input data into one or more classes.
    /// 
    /// Support Vector Machines typically have a single output. Neural networks can
    /// have multiple output neurons. To get around this issue, this class will
    /// create multiple SVM's if there is more than one output specified.
    /// 
    /// Because a SVM is trained quite differently from a neural network, none of the
    /// neural network training classes will work. This class must be trained using
    /// SVMTrain.
    /// </summary>
    public class SVMNetwork : BasicNetwork
    {
        /// <summary>
        /// The SVM's to use, one for each output.
        /// </summary>
        private svm_model[] models;

        /// <summary>
        /// The parameters for each of the SVM's.
        /// </summary>
        private svm_parameter[] parameters;

        /// <summary>
        /// The input count.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The output count.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The kernel type.
        /// </summary>
        private KernelType kernelType;

        /// <summary>
        /// The SVM type.
        /// </summary>
        private SVMType svmType;


        /// <summary>
        /// Construct a SVM network. 
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="svmType">The type of SVM.</param>
        /// <param name="kernelType">The SVM kernal type.</param>
        public SVMNetwork(int inputCount, int outputCount, SVMType svmType,
                KernelType kernelType)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;
            this.kernelType = kernelType;
            this.svmType = svmType;

            models = new svm_model[outputCount];
            parameters = new svm_parameter[outputCount];

            for (int i = 0; i < outputCount; i++)
            {
                parameters[i] = new svm_parameter();

                switch (svmType)
                {
                    case SVMType.SupportVectorClassification:
                        parameters[i].svm_type = svm_parameter.C_SVC;
                        break;
                    case SVMType.NewSupportVectorClassification:
                        parameters[i].svm_type = svm_parameter.NU_SVC;
                        break;
                    case SVMType.SupportVectorOneClass:
                        parameters[i].svm_type = svm_parameter.ONE_CLASS;
                        break;
                    case SVMType.EpsilonSupportVectorRegression:
                        parameters[i].svm_type = svm_parameter.EPSILON_SVR;
                        break;
                    case SVMType.NewSupportVectorRegression:
                        parameters[i].svm_type = svm_parameter.NU_SVR;
                        break;
                }

                switch (kernelType)
                {
                    case KernelType.Linear:
                        parameters[i].kernel_type = svm_parameter.LINEAR;
                        break;
                    case KernelType.Poly:
                        parameters[i].kernel_type = svm_parameter.POLY;
                        break;
                    case KernelType.RadialBasisFunction:
                        parameters[i].kernel_type = svm_parameter.RBF;
                        break;
                    case KernelType.Sigmoid:
                        parameters[i].kernel_type = svm_parameter.SIGMOID;
                        break;
                }

                parameters[i].kernel_type = svm_parameter.RBF;
                parameters[i].degree = 3;
                parameters[i].coef0 = 0;
                parameters[i].nu = 0.5;
                parameters[i].cache_size = 100;
                parameters[i].C = 1;
                parameters[i].eps = 1e-3;
                parameters[i].p = 0.1;
                parameters[i].shrinking = 1;
                parameters[i].probability = 0;
                parameters[i].nr_weight = 0;
                parameters[i].weight_label = new int[0];
                parameters[i].weight = new double[0];
                parameters[i].gamma = 1.0 / inputCount;
            }
        }

        /// <summary>
        /// Construct an SVM network. For regression it will use an epsilon support
        /// vector. Both types will use an RBF kernel.
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="regression">True if this network is used for regression.</param>
        public SVMNetwork(int inputCount, int outputCount, bool regression) :
            this(inputCount, outputCount,
                    regression ? SVMType.EpsilonSupportVectorRegression
                            : SVMType.SupportVectorClassification,
                    KernelType.RadialBasisFunction)
        {

        }


        /// <summary>
        /// Compute the output for the given input.
        /// </summary>
        /// <param name="input">The input to the SVM.</param>
        /// <returns>The results from the SVM.</returns>
        public override MLData Compute(MLData input)
        {
            MLData result = new BasicMLData(this.outputCount);

            svm_node[] formattedInput = MakeSparse(input);

            for (int i = 0; i < this.outputCount; i++)
            {
                double d = svm.svm_predict(this.models[i], formattedInput);
                result[i] = d;
            }
            return result;
        }

        /// <summary>
        /// Compute the output for the given input.
        /// </summary>
        /// <param name="input">The input to the SVM.</param>
        /// <param name="useHolder">The output holder to use.</param>
        /// <returns>The results from the SVM.</returns>
        public override MLData Compute(MLData input,
                NeuralOutputHolder useHolder)
        {

            useHolder.Output = Compute(input);
            return useHolder.Output;
        }


        /// <summary>
        /// Convert regular Encog NeuralData into the "sparse" data needed by an SVM. 
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>The SVM sparse data.</returns>
        public svm_node[] MakeSparse(MLData data)
        {
            svm_node[] result = new svm_node[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                result[i] = new svm_node();
                result[i].index = i + 1;
                result[i].value_Renamed = data[i];
            }

            return result;
        }


        /// <summary>
        /// Create a persistor for this object.
        /// </summary>
        /// <returns>The newly created persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return null;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public override int InputCount
        {
            get
            {
                return this.inputCount;
            }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public override int OutputCount
        {
            get
            {
                return this.outputCount;
            }
        }

        /// <summary>
        /// The SVM models for each output.
        /// </summary>
        public svm_model[] Models
        {
            get
            {
                return models;
            }
        }

        /// <summary>
        /// The SVM params for each of the outputs.
        /// </summary>
        public svm_parameter[] Params
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// The SVM kernel type.
        /// </summary>
        public KernelType KernelTypeUsed
        {
            get
            {
                return kernelType;
            }
        }

        /// <summary>
        /// The type of SVM in use.
        /// </summary>
        public SVMType SVMTypeUsed
        {
            get
            {
                return svmType;
            }
        }

    }
}
