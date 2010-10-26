using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks.SVM;
using Encog.MathUtil.LIBSVM;
using Encog.Util;
using Encog.Parse.Tags.Write;
using Encog.Engine.Util;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// Persist a SVM network.
    /// </summary>
    public class SVMNetworkPersistor : IPersistor
    {
        /// <summary>
        /// Constants for the SVM types.
        /// </summary>
        public static readonly String[] svm_type_table = { "c_svc", "nu_svc",
			"one_class", "epsilon_svr", "nu_svr", };

        /// <summary>
        /// Constants for the kernel types.
        /// </summary>
        public static readonly String[] kernel_type_table = { "linear", "polynomial",
			"rbf", "sigmoid", "precomputed" };

        /// <summary>
        /// The input tag.
        /// </summary>
        public const String TAG_INPUT = "input";

        /// <summary>
        /// The output tag.
        /// </summary>
        public const String TAG_OUTPUT = "output";

        /// <summary>
        /// The models tag.
        /// </summary>
        public const String TAG_MODELS = "models";

        /// <summary>
        /// The data tag.
        /// </summary>
        public const String TAG_DATA = "Data";

        /// <summary>
        /// The row tag.
        /// </summary>
        public const String TAG_ROW = "Row";

        /// <summary>
        /// The model tag.
        /// </summary>
        public const String TAG_MODEL = "Model";

        /// <summary>
        /// The type of SVM this is.
        /// </summary>
        public const String TAG_TYPE_SVM = "typeSVM";

        /// <summary>
        /// The type of kernel to use.
        /// </summary>
        public const String TAG_TYPE_KERNEL = "typeKernel";

        /// <summary>
        /// The degree to use.
        /// </summary>
        public const String TAG_DEGREE = "degree";

        /// <summary>
        /// The gamma to use.
        /// </summary>
        public const String TAG_GAMMA = "gamma";

        /// <summary>
        /// The coefficient.
        /// </summary>
        public const String TAG_COEF0 = "coef0";

        /// <summary>
        /// The number of classes.
        /// </summary>
        public const String TAG_NUMCLASS = "numClass";

        /// <summary>
        /// The total number of cases.
        /// </summary>
        public const String TAG_TOTALSV = "totalSV";

        /// <summary>
        /// The rho to use.
        /// </summary>
        public const String TAG_RHO = "rho";

        /// <summary>
        /// The labels.
        /// </summary>
        public const String TAG_LABEL = "label";

        /// <summary>
        /// The A-probability.
        /// </summary>
        public const String TAG_PROB_A = "probA";

        /// <summary>
        /// The B-probability.
        /// </summary>
        public const String TAG_PROB_B = "probB";

        /// <summary>
        /// The number of support vectors.
        /// </summary>
        public const String TAG_NSV = "nSV";

        
        /// <summary>
        /// Load the SVM network. 
        /// </summary>
        /// <param name="xmlin">Where to read it from.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            SVMNetwork result = null;
            int input = -1, output = -1;

            String name = xmlin.LastTag.Attributes[
                    EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlin.LastTag.Attributes[
                    EncogPersistedCollection.ATTRIBUTE_DESCRIPTION];

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(SVMNetworkPersistor.TAG_INPUT, true))
                {
                    input = int.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_OUTPUT, true))
                {
                    output = int.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_MODELS, true))
                {
                    result = new SVMNetwork(input, output, false);
                    HandleModels(xmlin, result);
                }
                else if (xmlin.IsIt(EncogPersistedCollection.TYPE_SVM, false))
                {
                    break;
                }
            }

            result.Name = name;
            result.Description = description;
            return result;
        }

        
        /// <summary>
        /// Load the models. 
        /// </summary>
        /// <param name="xmlin">Where to read the models from.</param>
        /// <param name="network">Where the models are read into.</param>
        private void HandleModels(ReadXML xmlin, SVMNetwork network)
        {

            int index = 0;
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(SVMNetworkPersistor.TAG_MODEL, true))
                {
                    svm_parameter param = new svm_parameter();
                    svm_model model = new svm_model();
                    model.param = param;
                    network.Models[index] = model;
                    HandleModel(xmlin, network.Models[index]);
                    index++;
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_MODELS, false))
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Handle a model. 
        /// </summary>
        /// <param name="xmlin">Where to read the model from.</param>
        /// <param name="model">Where to load the model into.</param>
        private void HandleModel(ReadXML xmlin, svm_model model)
        {
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(SVMNetworkPersistor.TAG_TYPE_SVM, true))
                {
                    int i = EngineArray.FindStringInArray(
                            SVMNetworkPersistor.svm_type_table, xmlin.ReadTextToTag());
                    model.param.svm_type = i;
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_DEGREE, true))
                {
                    model.param.degree = int.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_GAMMA, true))
                {
                    model.param.gamma = double.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_COEF0, true))
                {
                    model.param.coef0 = double.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_NUMCLASS, true))
                {
                    model.nr_class = int.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_TOTALSV, true))
                {
                    model.l = int.Parse(xmlin.ReadTextToTag());
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_RHO, true))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.rho = new double[n];
                    String[] st = xmlin.ReadTextToTag().Split(',');
                    for (int i = 0; i < n; i++)
                        model.rho[i] = double.Parse(st[i]);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_LABEL, true))
                {
                    int n = model.nr_class;
                    model.label = new int[n];
                    String[] st = xmlin.ReadTextToTag().Split(',');
                    for (int i = 0; i < n; i++)
                        model.label[i] = int.Parse(st[i]);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_PROB_A, true))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probA = new double[n];
                    String[] st = xmlin.ReadTextToTag().Split(',');
                    for (int i = 0; i < n; i++)
                        model.probA[i] = Double.Parse(st[i]);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_PROB_B, true))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probB = new double[n];
                    String[] st = xmlin.ReadTextToTag().Split(',');
                    for (int i = 0; i < n; i++)
                        model.probB[i] = Double.Parse(st[i]);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_NSV, true))
                {
                    int n = model.nr_class;
                    model.nSV = new int[n];
                    String[] st = xmlin.ReadTextToTag().Split(',');
                    for (int i = 0; i < n; i++)
                        model.nSV[i] = int.Parse(st[i]);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_TYPE_KERNEL, true))
                {
                    int i = EngineArray.FindStringInArray(
                            SVMNetworkPersistor.kernel_type_table, xmlin
                                    .ReadTextToTag());
                    model.param.kernel_type = i;
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_DATA, true))
                {
                    HandleData(xmlin, model);
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_MODEL, false))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Load the data from a model.
        /// </summary>
        /// <param name="xmlin">Where to read the data from.</param>
        /// <param name="model">The model to load data into.</param>
        private void HandleData(ReadXML xmlin, svm_model model)
        {
            int i = 0;
            int m = model.nr_class - 1;
            int l = model.l;

            model.sv_coef = EngineArray.AllocateDouble2D(m, l);
            model.SV = new svm_node[l][];

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(SVMNetworkPersistor.TAG_ROW, true))
                {
                    String line = xmlin.ReadTextToTag();

                    String[] st = xmlin.ReadTextToTag().Split(',');

                    for (int k = 0; k < m; k++)
                        model.sv_coef[k][i] = Double.Parse(st[i]);
                    int n = st.Length / 2;
                    model.SV[i] = new svm_node[n];
                    int idx = 0;
                    for (int j = 0; j < n; j++)
                    {
                        model.SV[i][j] = new svm_node();
                        model.SV[i][j].index = int.Parse(st[idx++]);
                        model.SV[i][j].value_Renamed = Double.Parse(st[idx++]);
                    }
                    i++;
                }
                else if (xmlin.IsIt(SVMNetworkPersistor.TAG_DATA, false))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Save a model.
        /// </summary>
        /// <param name="xmlout">Where to save a model to.</param>
        /// <param name="model">The model to save to.</param>
        public static void SaveModel(WriteXML xmlout, svm_model model)
        {
            if (model != null)
            {
                xmlout.BeginTag(SVMNetworkPersistor.TAG_MODEL);

                svm_parameter param = model.param;

                xmlout.AddProperty(SVMNetworkPersistor.TAG_TYPE_SVM,
                        svm_type_table[param.svm_type]);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_TYPE_KERNEL,
                        kernel_type_table[param.kernel_type]);

                if (param.kernel_type == svm_parameter.POLY)
                {
                    xmlout.AddProperty(SVMNetworkPersistor.TAG_DEGREE, param.degree);
                }

                if (param.kernel_type == svm_parameter.POLY
                        || param.kernel_type == svm_parameter.RBF
                        || param.kernel_type == svm_parameter.SIGMOID)
                {
                    xmlout.AddProperty(SVMNetworkPersistor.TAG_GAMMA, param.gamma);
                }

                if (param.kernel_type == svm_parameter.POLY
                        || param.kernel_type == svm_parameter.SIGMOID)
                {
                    xmlout.AddProperty(SVMNetworkPersistor.TAG_COEF0, param.coef0);
                }

                int nr_class = model.nr_class;
                int l = model.l;

                xmlout.AddProperty(SVMNetworkPersistor.TAG_NUMCLASS, nr_class);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_TOTALSV, l);

                xmlout.AddProperty(SVMNetworkPersistor.TAG_RHO, model.rho, nr_class
                        * (nr_class - 1) / 2);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_LABEL, model.label,
                        nr_class);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_PROB_A, model.probA,
                        nr_class * (nr_class - 1) / 2);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_PROB_B, model.probB,
                        nr_class * (nr_class - 1) / 2);
                xmlout.AddProperty(SVMNetworkPersistor.TAG_NSV, model.nSV, nr_class);

                xmlout.BeginTag(SVMNetworkPersistor.TAG_DATA);

                double[][] sv_coef = model.sv_coef;
                svm_node[][] SV = model.SV;

                StringBuilder line = new StringBuilder();
                for (int i = 0; i < l; i++)
                {
                    line.Length = 0;
                    for (int j = 0; j < nr_class - 1; j++)
                        line.Append(sv_coef[j][i] + " ");

                    svm_node[] p = SV[i];
                    //if (param.kernel_type == svm_parameter.PRECOMPUTED)
                    //{
                    //  line.Append("0:" + (int) (p[0].value));
                    //}
                    //else
                    for (int j = 0; j < p.Length; j++)
                        line.Append(p[j].index + ":" + p[j].value_Renamed + " ");
                    xmlout.AddProperty(SVMNetworkPersistor.TAG_ROW, line.ToString());
                }

                xmlout.EndTag();
                xmlout.EndTag();

            }
        }

      
        /// <summary>
        /// Save a SVMNetwork. 
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">Where to save it to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {
            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_SVM, xmlout,
                    obj, true);
            SVMNetwork net = (SVMNetwork)obj;

            xmlout.AddProperty(SVMNetworkPersistor.TAG_INPUT, net.InputCount);
            xmlout.AddProperty(SVMNetworkPersistor.TAG_OUTPUT, net.OutputCount);
            xmlout.BeginTag(SVMNetworkPersistor.TAG_MODELS);
            for (int i = 0; i < net.Models.Length; i++)
            {
                SaveModel(xmlout, net.Models[i]);
            }
            xmlout.EndTag();
            xmlout.EndTag();
        }

    }
}
