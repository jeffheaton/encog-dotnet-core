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
using System.Collections.Generic;
using System.IO;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.PNN;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Neural.Pnn
{
    /// <summary>
    /// Persist a PNN.
    /// </summary>
    ///
    public class PersistBasicPNN : EncogPersistor
    {
        /// <summary>
        /// The output mode property.
        /// </summary>
        ///
        public const String PROPERTY_outputMode = "outputMode";

        #region EncogPersistor Members

        /// <summary>
        /// File version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }


        /// <summary>
        /// File version.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "BasicPNN"; }
        }


        /// <summary>
        /// Read an object.
        /// </summary>
        public Object Read(Stream mask0)
        {
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;
            var samples = new BasicMLDataSet();
            IDictionary<String, String> networkParams = null;
            PNNKernelType kernel = default(PNNKernelType) /* was: null */;
            PNNOutputMode outmodel = default(PNNOutputMode) /* was: null */;
            int inputCount = 0;
            int outputCount = 0;
            double error = 0;
            double[] sigma = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("PNN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("PNN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    inputCount = EncogFileSection.ParseInt(paras,
                                                           PersistConst.INPUT_COUNT);
                    outputCount = EncogFileSection.ParseInt(paras,
                                                            PersistConst.OUTPUT_COUNT);
                    kernel = StringToKernel(paras[PersistConst.KERNEL]);
                    outmodel = StringToOutputMode(paras[PROPERTY_outputMode]);
                    error = EncogFileSection
                        .ParseDouble(paras, PersistConst.ERROR);
                    sigma = EncogFileSection.ParseDoubleArray(paras,
                                                              PersistConst.SIGMA);
                }
                if (section.SectionName.Equals("PNN")
                    && section.SubSectionName.Equals("SAMPLES"))
                {
                    foreach (String line  in  section.Lines)
                    {
                        IList<String> cols = EncogFileSection
                            .SplitColumns(line);
                        int index = 0;
                        IMLData inputData = new BasicMLData(inputCount);
                        for (int i = 0; i < inputCount; i++)
                        {
                            inputData[i] =
                                CSVFormat.EG_FORMAT.Parse(cols[index++]);
                        }
                        IMLData idealData = new BasicMLData(inputCount);
                        for (int i_0 = 0; i_0 < outputCount; i_0++)
                        {
                            idealData[i_0] =
                                CSVFormat.EG_FORMAT.Parse(cols[index++]);
                        }
                        MLDataPair pair = new BasicMLDataPair(inputData,
                                                              idealData);
                        samples.Add(pair);
                    }
                }
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);
            if (networkParams != null)
            {
                EngineArray.PutAll(networkParams, result.Properties);
            }
            result.Samples = samples;
            result.Error = error;
            if (sigma != null)
            {
                EngineArray.ArrayCopy(sigma, result.Sigma);
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
            var pnn = (BasicPNN) obj;
            xout.AddSection("PNN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(pnn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.ERROR, pnn.Error);
            xout.WriteProperty(PersistConst.INPUT_COUNT, pnn.InputCount);
            xout.WriteProperty(PersistConst.KERNEL,
                               KernelToString(pnn.Kernel));
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, pnn.OutputCount);
            xout.WriteProperty(PROPERTY_outputMode,
                               OutputModeToString(pnn.OutputMode));
            xout.WriteProperty(PersistConst.SIGMA, pnn.Sigma);

            xout.AddSubSection("SAMPLES");

            foreach (MLDataPair pair  in  pnn.Samples)
            {
                for (int i = 0; i < pair.Input.Count; i++)
                {
                    xout.AddColumn(pair.Input[i]);
                }
                for (int i_0 = 0; i_0 < pair.Ideal.Count; i_0++)
                {
                    xout.AddColumn(pair.Ideal[i_0]);
                }
                xout.WriteLine();
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Convert a kernel type to a string.
        /// </summary>
        ///
        /// <param name="k">The kernel type.</param>
        /// <returns>The string.</returns>
        public static String KernelToString(PNNKernelType k)
        {
            switch (k)
            {
                case PNNKernelType.Gaussian:
                    return "gaussian";
                case PNNKernelType.Reciprocal:
                    return "reciprocal";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert output mode to string.
        /// </summary>
        ///
        /// <param name="mode">The output mode.</param>
        /// <returns>The string.</returns>
        public static String OutputModeToString(PNNOutputMode mode)
        {
            switch (mode)
            {
                case PNNOutputMode.Regression:
                    return "regression";
                case PNNOutputMode.Unsupervised:
                    return "unsupervised";
                case PNNOutputMode.Classification:
                    return "classification";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to a PNN kernel.
        /// </summary>
        ///
        /// <param name="k">The string.</param>
        /// <returns>The kernel.</returns>
        public static PNNKernelType StringToKernel(String k)
        {
            if (k.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                return PNNKernelType.Gaussian;
            }
            else if (k.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                return PNNKernelType.Reciprocal;
            }
            else
            {
                return default(PNNKernelType) /* was: null */;
            }
        }

        /// <summary>
        /// Convert a string to a PNN output mode.
        /// </summary>
        ///
        /// <param name="mode">The string.</param>
        /// <returns>The output ndoe.</returns>
        public static PNNOutputMode StringToOutputMode(String mode)
        {
            if (mode.Equals("regression", StringComparison.InvariantCultureIgnoreCase))
            {
                return PNNOutputMode.Regression;
            }
            else if (mode.Equals("unsupervised", StringComparison.InvariantCultureIgnoreCase))
            {
                return PNNOutputMode.Unsupervised;
            }
            else if (mode.Equals("classification", StringComparison.InvariantCultureIgnoreCase))
            {
                return PNNOutputMode.Classification;
            }
            else
            {
                return default(PNNOutputMode) /* was: null */;
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BasicPNN); }
        }
    }
}
