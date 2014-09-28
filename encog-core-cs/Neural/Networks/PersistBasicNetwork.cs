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
    public class PersistBasicNetwork : IEncogPersistor
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
                    IDictionary<String, String> p = section.ParseParams();

                    flat.BeginTraining = EncogFileSection.ParseInt(p,
                                                                   BasicNetwork.TagBeginTraining);
                    flat.ConnectionLimit = EncogFileSection.ParseDouble(p,
                                                                        BasicNetwork.TagConnectionLimit);
                    flat.ContextTargetOffset = EncogFileSection.ParseIntArray(
                        p, BasicNetwork.TagContextTargetOffset);
                    flat.ContextTargetSize = EncogFileSection.ParseIntArray(
                        p, BasicNetwork.TagContextTargetSize);
                    flat.EndTraining = EncogFileSection.ParseInt(p,
                                                                 BasicNetwork.TagEndTraining);
                    flat.HasContext = EncogFileSection.ParseBoolean(p,
                                                                    BasicNetwork.TagHasContext);
                    flat.InputCount = EncogFileSection.ParseInt(p,
                                                                PersistConst.InputCount);
                    flat.LayerCounts = EncogFileSection.ParseIntArray(p,
                                                                      BasicNetwork.TagLayerCounts);
                    flat.LayerFeedCounts = EncogFileSection.ParseIntArray(p,
                                                                          BasicNetwork.TagLayerFeedCounts);
                    flat.LayerContextCount = EncogFileSection.ParseIntArray(
                        p, BasicNetwork.TagLayerContextCount);
                    flat.LayerIndex = EncogFileSection.ParseIntArray(p,
                                                                     BasicNetwork.TagLayerIndex);
                    flat.LayerOutput = section.ParseDoubleArray(p,PersistConst.Output);
                    flat.LayerSums = new double[flat.LayerOutput.Length];
                    flat.OutputCount = EncogFileSection.ParseInt(p,
                                                                 PersistConst.OutputCount);
                    flat.WeightIndex = EncogFileSection.ParseIntArray(p,
                                                                      BasicNetwork.TagWeightIndex);
                    flat.Weights = section.ParseDoubleArray(p, PersistConst.Weights);
                    flat.BiasActivation = section.ParseDoubleArray(p, BasicNetwork.TagBiasActivation);
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("ACTIVATION"))
                {
                    int index = 0;

                    flat.ActivationFunctions = new IActivationFunction[flat.LayerCounts.Length];


                    foreach (String line  in  section.Lines)
                    {
                        IActivationFunction af;
                        IList<String> cols = EncogFileSection
                            .SplitColumns(line);
                        String name = ReflectionUtil.AfPath
                                      + cols[0];
                        try
                        {
                            af = (IActivationFunction) ReflectionUtil.LoadObject(name);
                        }
                        catch (TypeLoadException e)
                        {
                            throw new PersistError(e);
                        }
                        catch (TargetException e)
                        {
                            throw new PersistError(e);
                        }
                        catch (MemberAccessException e)
                        {
                            throw new PersistError(e);
                        }

                        for (int i = 0; i < af.ParamNames.Length; i++)
                        {
                            af.Params[i] = 
                                        CSVFormat.EgFormat.Parse(cols[i + 1]);
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

            xout.WriteProperty(BasicNetwork.TagBeginTraining,
                               flat.BeginTraining);
            xout.WriteProperty(BasicNetwork.TagConnectionLimit,
                               flat.ConnectionLimit);
            xout.WriteProperty(BasicNetwork.TagContextTargetOffset,
                               flat.ContextTargetOffset);
            xout.WriteProperty(BasicNetwork.TagContextTargetSize,
                               flat.ContextTargetSize);
            xout.WriteProperty(BasicNetwork.TagEndTraining, flat.EndTraining);
            xout.WriteProperty(BasicNetwork.TagHasContext, flat.HasContext);
            xout.WriteProperty(PersistConst.InputCount, flat.InputCount);
            xout.WriteProperty(BasicNetwork.TagLayerCounts, flat.LayerCounts);
            xout.WriteProperty(BasicNetwork.TagLayerFeedCounts,
                               flat.LayerFeedCounts);
            xout.WriteProperty(BasicNetwork.TagLayerContextCount,
                               flat.LayerContextCount);
            xout.WriteProperty(BasicNetwork.TagLayerIndex, flat.LayerIndex);
            xout.WriteProperty(PersistConst.Output, flat.LayerOutput);
            xout.WriteProperty(PersistConst.OutputCount, flat.OutputCount);
            xout.WriteProperty(BasicNetwork.TagWeightIndex, flat.WeightIndex);
            xout.WriteProperty(PersistConst.Weights, flat.Weights);
            xout.WriteProperty(BasicNetwork.TagBiasActivation,
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
