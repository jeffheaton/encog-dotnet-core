using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.MathUtil.LIBSVM;
using Encog.Persist;
using Encog.Util;

namespace Encog.ML.SVM
{
    /// <summary>
    /// Persist a SVM.
    /// </summary>
    ///
    public class PersistSVM : EncogPersistor
    {
        /// <summary>
        /// The parameter to hold the const C.
        /// </summary>
        ///
        public const String PARAM_C = "C";

        /// <summary>
        /// The parameter to hold the cache size.
        /// </summary>
        ///
        public const String PARAM_CACHE_SIZE = "cacheSize";

        /// <summary>
        /// The parameter to hold the coef0.
        /// </summary>
        ///
        public const String PARAM_COEF0 = "coef0";

        /// <summary>
        /// The parameter to hold the degree.
        /// </summary>
        ///
        public const String PARAM_DEGREE = "degree";

        /// <summary>
        /// The parameter to hold the eps.
        /// </summary>
        ///
        public const String PARAM_EPS = "eps";

        /// <summary>
        /// The parameter to hold the gamma.
        /// </summary>
        ///
        public const String PARAM_GAMMA = "gamma";

        /// <summary>
        /// The parameter to hold the kernel type.
        /// </summary>
        ///
        public const String PARAM_KERNEL_TYPE = "kernelType";

        /// <summary>
        /// The parameter to hold the number of weights.
        /// </summary>
        ///
        public const String PARAM_NUM_WEIGHT = "nrWeight";

        /// <summary>
        /// The parameter to hold the nu.
        /// </summary>
        ///
        public const String PARAM_NU = "nu";

        /// <summary>
        /// The parameter to hold the p.
        /// </summary>
        ///
        public const String PARAM_P = "p";

        /// <summary>
        /// The parameter to hold the probability.
        /// </summary>
        ///
        public const String PARAM_PROBABILITY = "probability";

        /// <summary>
        /// The parameter to hold the shrinking.
        /// </summary>
        ///
        public const String PARAM_SHRINKING = "shrinking";

        /// <summary>
        /// The parameter to hold the statIterations.
        /// </summary>
        ///
        public const String PARAM_START_ITERATIONS = "statIterations";

        /// <summary>
        /// The parameter to hold the SVM type.
        /// </summary>
        ///
        public const String PARAM_SVM_TYPE = "svmType";

        /// <summary>
        /// The paramater to hold the weight.
        /// </summary>
        ///
        public const String PARAM_WEIGHT = "weight";

        /// <summary>
        /// The parameter to hold the weight label.
        /// </summary>
        ///
        public const String PARAM_WEIGHT_LABEL = "weightLabel";

        #region EncogPersistor Members

        /// <value>The file version.</value>
        public int FileVersion
        {
            /// <returns>The file version.</returns>
            get { return 1; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public String PersistClassString
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return "SVM"; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new SupportVectorMachine();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("SVM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("SVM")
                    && section.SubSectionName.Equals("SVM-PARAM"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();
                    result.InputCount = EncogFileSection.ParseInt(params_0,
                                                                  PersistConst.INPUT_COUNT);
                    result.Params.C = EncogFileSection.ParseDouble(params_0,
                                                                   PARAM_C);
                    result.Params.cache_size = EncogFileSection.ParseDouble(
                        params_0, PARAM_CACHE_SIZE);
                    result.Params.coef0 = EncogFileSection.ParseDouble(params_0,
                                                                       PARAM_COEF0);
                    result.Params.degree = EncogFileSection.ParseInt(params_0,
                                                                     PARAM_DEGREE);
                    result.Params.eps = EncogFileSection.ParseDouble(params_0,
                                                                     PARAM_EPS);
                    result.Params.gamma = EncogFileSection.ParseDouble(params_0,
                                                                       PARAM_GAMMA);
                    result.Params.kernel_type = EncogFileSection.ParseInt(
                        params_0, PARAM_KERNEL_TYPE);
                    result.Params.nr_weight = EncogFileSection.ParseInt(
                        params_0, PARAM_NUM_WEIGHT);
                    result.Params.nu = EncogFileSection.ParseDouble(params_0,
                                                                    PARAM_NU);
                    result.Params.p = EncogFileSection.ParseDouble(params_0,
                                                                   PARAM_P);
                    result.Params.probability = EncogFileSection.ParseInt(
                        params_0, PARAM_PROBABILITY);
                    result.Params.shrinking = EncogFileSection.ParseInt(
                        params_0, PARAM_SHRINKING);
                    /*result.Params.statIterations = Encog.Persist.EncogFileSection.ParseInt(
							params_0, PersistSVM.PARAM_START_ITERATIONS);*/
                    result.Params.svm_type = EncogFileSection.ParseInt(params_0,
                                                                       PARAM_SVM_TYPE);
                    result.Params.weight = EncogFileSection.ParseDoubleArray(
                        params_0, PARAM_WEIGHT);
                    result.Params.weight_label = EncogFileSection
                        .ParseIntArray(params_0, PARAM_WEIGHT_LABEL);
                }
                else if (section.SectionName.Equals("SVM")
                         && section.SubSectionName.Equals("SVM-MODEL"))
                {
                    try
                    {
                        var rdr = new StringReader(
                            section.LinesAsString);
                        TextReader br = rdr;
                        svm_model model = svm.svm_load_model(rdr);
                        result.Model = model;
                        br.Close();
                        rdr.Close();
                    }
                    catch (IOException ex)
                    {
                        throw new PersistError(ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var svm2 = (SupportVectorMachine) obj;
            xout.AddSection("SVM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(svm2.Properties);
            xout.AddSubSection("SVM-PARAM");
            xout.WriteProperty(PersistConst.INPUT_COUNT, svm2.InputCount);
            xout.WriteProperty(PARAM_C, svm2.Params.C);
            xout.WriteProperty(PARAM_CACHE_SIZE,
                               svm2.Params.cache_size);
            xout.WriteProperty(PARAM_COEF0, svm2.Params.coef0);
            xout.WriteProperty(PARAM_DEGREE, svm2.Params.degree);
            xout.WriteProperty(PARAM_EPS, svm2.Params.eps);
            xout.WriteProperty(PARAM_GAMMA, svm2.Params.gamma);
            xout.WriteProperty(PARAM_KERNEL_TYPE,
                               svm2.Params.kernel_type);
            xout.WriteProperty(PARAM_NUM_WEIGHT,
                               svm2.Params.nr_weight);
            xout.WriteProperty(PARAM_NU, svm2.Params.nu);
            xout.WriteProperty(PARAM_P, svm2.Params.p);
            xout.WriteProperty(PARAM_PROBABILITY,
                               svm2.Params.probability);
            xout.WriteProperty(PARAM_SHRINKING,
                               svm2.Params.shrinking);
            /* xout.WriteProperty(PersistSVM.PARAM_START_ITERATIONS,
					svm2.Params.statIterations); */
            xout.WriteProperty(PARAM_SVM_TYPE, svm2.Params.svm_type);
            xout.WriteProperty(PARAM_WEIGHT, svm2.Params.weight);
            xout.WriteProperty(PARAM_WEIGHT_LABEL,
                               svm2.Params.weight_label);
            if (svm2.Model != null)
            {
                xout.AddSubSection("SVM-MODEL");
                try
                {
                    var ba = new MemoryStream();
                    var w = new StreamWriter(ba);
                    svm.svm_save_model(w, svm2.Model);
                    var enc = new ASCIIEncoding();
                    xout.Write(enc.GetString(ba.ToArray()));
                    w.Close();
                    ba.Close();
                }
                catch (IOException ex)
                {
                    throw new PersistError(ex);
                }
            }

            xout.Flush();
        }

        #endregion
    }
}