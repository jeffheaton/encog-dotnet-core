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
    public class PersistSVM : IEncogPersistor
    {
        /// <summary>
        /// The parameter to hold the const C.
        /// </summary>
        ///
        public const String ParamC = "C";

        /// <summary>
        /// The parameter to hold the cache size.
        /// </summary>
        ///
        public const String ParamCacheSize = "cacheSize";

        /// <summary>
        /// The parameter to hold the coef0.
        /// </summary>
        ///
        public const String ParamCoef0 = "coef0";

        /// <summary>
        /// The parameter to hold the degree.
        /// </summary>
        ///
        public const String ParamDegree = "degree";

        /// <summary>
        /// The parameter to hold the eps.
        /// </summary>
        ///
        public const String ParamEps = "eps";

        /// <summary>
        /// The parameter to hold the gamma.
        /// </summary>
        ///
        public const String ParamGamma = "gamma";

        /// <summary>
        /// The parameter to hold the kernel type.
        /// </summary>
        ///
        public const String ParamKernelType = "kernelType";

        /// <summary>
        /// The parameter to hold the number of weights.
        /// </summary>
        ///
        public const String ParamNumWeight = "nrWeight";

        /// <summary>
        /// The parameter to hold the nu.
        /// </summary>
        ///
        public const String ParamNu = "nu";

        /// <summary>
        /// The parameter to hold the p.
        /// </summary>
        ///
        public const String ParamP = "p";

        /// <summary>
        /// The parameter to hold the probability.
        /// </summary>
        ///
        public const String ParamProbability = "probability";

        /// <summary>
        /// The parameter to hold the shrinking.
        /// </summary>
        ///
        public const String ParamShrinking = "shrinking";

        /// <summary>
        /// The parameter to hold the statIterations.
        /// </summary>
        ///
        public const String ParamStartIterations = "statIterations";

        /// <summary>
        /// The parameter to hold the SVM type.
        /// </summary>
        ///
        public const String ParamSVMType = "svmType";

        /// <summary>
        /// The paramater to hold the weight.
        /// </summary>
        ///
        public const String ParamWeight = "weight";

        /// <summary>
        /// The parameter to hold the weight label.
        /// </summary>
        ///
        public const String ParamWeightLabel = "weightLabel";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(SupportVectorMachine); }
        }

        #region EncogPersistor Members

        /// <value>The file version.</value>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {

            get { return "SVM"; }
        }


        /// <inheritdoc/>
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
                    IDictionary<String, String> p = section.ParseParams();
                    result.InputCount = EncogFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                    result.Params.C = EncogFileSection.ParseDouble(p,
                                                                   ParamC);
                    result.Params.cache_size = EncogFileSection.ParseDouble(
                        p, ParamCacheSize);
                    result.Params.coef0 = EncogFileSection.ParseDouble(p,
                                                                       ParamCoef0);
                    result.Params.degree = EncogFileSection.ParseDouble(p,
                                                                     ParamDegree);
                    result.Params.eps = EncogFileSection.ParseDouble(p,
                                                                     ParamEps);
                    result.Params.gamma = EncogFileSection.ParseDouble(p,
                                                                       ParamGamma);
                    result.Params.kernel_type = EncogFileSection.ParseInt(
                        p, ParamKernelType);
                    result.Params.nr_weight = EncogFileSection.ParseInt(
                        p, ParamNumWeight);
                    result.Params.nu = EncogFileSection.ParseDouble(p,
                                                                    ParamNu);
                    result.Params.p = EncogFileSection.ParseDouble(p,
                                                                   ParamP);
                    result.Params.probability = EncogFileSection.ParseInt(
                        p, ParamProbability);
                    result.Params.shrinking = EncogFileSection.ParseInt(
                        p, ParamShrinking);
                    /*result.Params.statIterations = Encog.Persist.EncogFileSection.ParseInt(
							params_0, PersistSVM.PARAM_START_ITERATIONS);*/
                    result.Params.svm_type = EncogFileSection.ParseInt(p,
                                                                       ParamSVMType);
                    result.Params.weight = section.ParseDoubleArray(p, ParamWeight);
                    result.Params.weight_label = EncogFileSection
                        .ParseIntArray(p, ParamWeightLabel);
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

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var svm2 = (SupportVectorMachine) obj;
            xout.AddSection("SVM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(svm2.Properties);
            xout.AddSubSection("SVM-PARAM");
            xout.WriteProperty(PersistConst.InputCount, svm2.InputCount);
            xout.WriteProperty(ParamC, svm2.Params.C);
            xout.WriteProperty(ParamCacheSize,
                               svm2.Params.cache_size);
            xout.WriteProperty(ParamCoef0, svm2.Params.coef0);
            xout.WriteProperty(ParamDegree, svm2.Params.degree);
            xout.WriteProperty(ParamEps, svm2.Params.eps);
            xout.WriteProperty(ParamGamma, svm2.Params.gamma);
            xout.WriteProperty(ParamKernelType,
                               svm2.Params.kernel_type);
            xout.WriteProperty(ParamNumWeight,
                               svm2.Params.nr_weight);
            xout.WriteProperty(ParamNu, svm2.Params.nu);
            xout.WriteProperty(ParamP, svm2.Params.p);
            xout.WriteProperty(ParamProbability,
                               svm2.Params.probability);
            xout.WriteProperty(ParamShrinking,
                               svm2.Params.shrinking);
            /* xout.WriteProperty(PersistSVM.PARAM_START_ITERATIONS,
					svm2.Params.statIterations); */
            xout.WriteProperty(ParamSVMType, svm2.Params.svm_type);
            xout.WriteProperty(ParamWeight, svm2.Params.weight);
            xout.WriteProperty(ParamWeightLabel,
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
