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
using System.Reflection;
using Encog.Engine.Network.Activation;
using Encog.Neural.Flat;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Persist a basic network.
    /// </summary>
    ///
    public class PersistBasicNetwork : EncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }


        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "BasicNetwork"; }
        }


        /// <summary>
        /// Read an object.
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new BasicNetwork();
            var flat = new FlatNetwork();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BASIC")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BASIC")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();

                    flat.BeginTraining = EncogFileSection.ParseInt(params_0,
                                                                   BasicNetwork.TAG_BEGIN_TRAINING);
                    flat.ConnectionLimit = EncogFileSection.ParseDouble(params_0,
                                                                        BasicNetwork.TAG_CONNECTION_LIMIT);
                    flat.ContextTargetOffset = EncogFileSection.ParseIntArray(
                        params_0, BasicNetwork.TAG_CONTEXT_TARGET_OFFSET);
                    flat.ContextTargetSize = EncogFileSection.ParseIntArray(
                        params_0, BasicNetwork.TAG_CONTEXT_TARGET_SIZE);
                    flat.EndTraining = EncogFileSection.ParseInt(params_0,
                                                                 BasicNetwork.TAG_END_TRAINING);
                    flat.HasContext = EncogFileSection.ParseBoolean(params_0,
                                                                    BasicNetwork.TAG_HAS_CONTEXT);
                    flat.InputCount = EncogFileSection.ParseInt(params_0,
                                                                PersistConst.INPUT_COUNT);
                    flat.LayerCounts = EncogFileSection.ParseIntArray(params_0,
                                                                      BasicNetwork.TAG_LAYER_COUNTS);
                    flat.LayerFeedCounts = EncogFileSection.ParseIntArray(params_0,
                                                                          BasicNetwork.TAG_LAYER_FEED_COUNTS);
                    flat.LayerContextCount = EncogFileSection.ParseIntArray(
                        params_0, BasicNetwork.TAG_LAYER_CONTEXT_COUNT);
                    flat.LayerIndex = EncogFileSection.ParseIntArray(params_0,
                                                                     BasicNetwork.TAG_LAYER_INDEX);
                    flat.LayerOutput = EncogFileSection.ParseDoubleArray(params_0,
                                                                         PersistConst.OUTPUT);
                    flat.OutputCount = EncogFileSection.ParseInt(params_0,
                                                                 PersistConst.OUTPUT_COUNT);
                    flat.WeightIndex = EncogFileSection.ParseIntArray(params_0,
                                                                      BasicNetwork.TAG_WEIGHT_INDEX);
                    flat.Weights = EncogFileSection.ParseDoubleArray(params_0,
                                                                     PersistConst.WEIGHTS);
                    flat.BiasActivation = EncogFileSection.ParseDoubleArray(
                        params_0, BasicNetwork.TAG_BIAS_ACTIVATION);
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("ACTIVATION"))
                {
                    int index = 0;

                    flat.ActivationFunctions = new IActivationFunction[flat.LayerCounts.Length];


                    foreach (String line  in  section.Lines)
                    {
                        IActivationFunction af = null;
                        IList<String> cols = EncogFileSection
                            .SplitColumns(line);
                        String name = ReflectionUtil.AF_PATH
                                      + cols[0];
                        try
                        {
                            af = (IActivationFunction) ReflectionUtil.LoadObject(name);
                        }
                        catch (TypeLoadException e)
                        {
                            throw new PersistError(e);
                        }
                        catch (TargetException e_1)
                        {
                            throw new PersistError(e_1);
                        }
                        catch (MemberAccessException e_2)
                        {
                            throw new PersistError(e_2);
                        }

                        for (int i = 0; i < af.ParamNames.Length; i++)
                        {
                            af.Params[i] = 
                                        CSVFormat.EG_FORMAT.Parse(cols[i + 1]);
                        }

                        flat.ActivationFunctions[index++] = af;
                    }
                }
            }

            result.Structure.Flat = flat;

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var net = (BasicNetwork) obj;
            FlatNetwork flat = net.Structure.Flat;
            xout.AddSection("BASIC");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(net.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(BasicNetwork.TAG_BEGIN_TRAINING,
                               flat.BeginTraining);
            xout.WriteProperty(BasicNetwork.TAG_CONNECTION_LIMIT,
                               flat.ConnectionLimit);
            xout.WriteProperty(BasicNetwork.TAG_CONTEXT_TARGET_OFFSET,
                               flat.ContextTargetOffset);
            xout.WriteProperty(BasicNetwork.TAG_CONTEXT_TARGET_SIZE,
                               flat.ContextTargetSize);
            xout.WriteProperty(BasicNetwork.TAG_END_TRAINING, flat.EndTraining);
            xout.WriteProperty(BasicNetwork.TAG_HAS_CONTEXT, flat.HasContext);
            xout.WriteProperty(PersistConst.INPUT_COUNT, flat.InputCount);
            xout.WriteProperty(BasicNetwork.TAG_LAYER_COUNTS, flat.LayerCounts);
            xout.WriteProperty(BasicNetwork.TAG_LAYER_FEED_COUNTS,
                               flat.LayerFeedCounts);
            xout.WriteProperty(BasicNetwork.TAG_LAYER_CONTEXT_COUNT,
                               flat.LayerContextCount);
            xout.WriteProperty(BasicNetwork.TAG_LAYER_INDEX, flat.LayerIndex);
            xout.WriteProperty(PersistConst.OUTPUT, flat.LayerOutput);
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, flat.OutputCount);
            xout.WriteProperty(BasicNetwork.TAG_WEIGHT_INDEX, flat.WeightIndex);
            xout.WriteProperty(PersistConst.WEIGHTS, flat.Weights);
            xout.WriteProperty(BasicNetwork.TAG_BIAS_ACTIVATION,
                               flat.BiasActivation);
            xout.AddSubSection("ACTIVATION");

            foreach (IActivationFunction af  in  flat.ActivationFunctions)
            {
                xout.AddColumn(af.GetType().Name);
                for (int i = 0; i < af.Params.Length; i++)
                {
                    xout.AddColumn(af.Params[i]);
                }
                xout.WriteLine();
            }

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BasicNetwork); }
        }

        #endregion
    }
}
