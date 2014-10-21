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
using Encog.MathUtil.RBF;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Neural.RBF;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Neural.Rbf
{
    /// <summary>
    /// Persist a RBF network.
    /// </summary>
    ///
    public class PersistRBFNetwork : IEncogPersistor
    {
        #region EncogPersistor Members

        /// <inheritdoc/>
        public virtual int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public virtual String PersistClassString
        {
            get { return "RBFNetwork"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new RBFNetwork();
            var flat = (FlatNetworkRBF) result.Flat;

            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("RBF-NETWORK")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("RBF-NETWORK")
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
                    flat.LayerContextCount = EncogFileSection.ParseIntArray(p, BasicNetwork.TagLayerContextCount);
                    flat.LayerIndex = EncogFileSection.ParseIntArray(p,
                                                                     BasicNetwork.TagLayerIndex);
                    flat.LayerOutput = section.ParseDoubleArray(p,
                                                                         PersistConst.Output);
                    flat.LayerSums = new double[flat.LayerOutput.Length];
                    flat.OutputCount = EncogFileSection.ParseInt(p,PersistConst.OutputCount);
                    flat.WeightIndex = EncogFileSection.ParseIntArray(p,
                                                                      BasicNetwork.TagWeightIndex);
                    flat.Weights = section.ParseDoubleArray(p,
                                                                     PersistConst.Weights);
                    flat.BiasActivation = section.ParseDoubleArray(p, BasicNetwork.TagBiasActivation);
                }
                else if (section.SectionName.Equals("RBF-NETWORK")
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
                        catch (Exception e)
                        {
                            throw new PersistError(e);
                        }
                        for (int i = 0; i < af.ParamNames.Length; i++)
                        {
                            af.Params[i] = CSVFormat.EgFormat.Parse(cols[i + 1]);
                        }

                        flat.ActivationFunctions[index++] = af;
                    }
                }
                else if (section.SectionName.Equals("RBF-NETWORK")
                         && section.SubSectionName.Equals("RBF"))
                {
                    int index = 0;

                    int hiddenCount = flat.LayerCounts[1];
                    int inputCount = flat.LayerCounts[2];

                    flat.RBF = new IRadialBasisFunction[hiddenCount];


                    foreach (String line  in  section.Lines)
                    {
                        IRadialBasisFunction rbf;
                        IList<String> cols = EncogFileSection
                            .SplitColumns(line);
                        String name = ReflectionUtil.RBFPath + cols[0];
                        try
                        {
                            rbf = (IRadialBasisFunction) ReflectionUtil.LoadObject(name);
                        }
                        catch (TypeLoadException ex)
                        {
                            throw new PersistError(ex);
                        }
                        catch (TargetException ex)
                        {
                            throw new PersistError(ex);
                        }
                        catch (MemberAccessException ex)
                        {
                            throw new PersistError(ex);
                        }

                        rbf.Width = CSVFormat.EgFormat.Parse(cols[1]);
                        rbf.Peak = CSVFormat.EgFormat.Parse(cols[2]);
                        rbf.Centers = new double[inputCount];

                        for (int i = 0; i < inputCount; i++)
                        {
                            rbf.Centers[i] = CSVFormat.EgFormat.Parse(cols[i + 3]);
                        }

                        flat.RBF[index++] = rbf;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var net = (RBFNetwork) obj;
            var flat = (FlatNetworkRBF) net.Flat;
            xout.AddSection("RBF-NETWORK");
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
                foreach (double t in af.Params)
                {
                    xout.AddColumn(t);
                }
                xout.WriteLine();
            }
            xout.AddSubSection("RBF");

            foreach (IRadialBasisFunction rbf  in  flat.RBF)
            {
                xout.AddColumn(rbf.GetType().Name);
                xout.AddColumn(rbf.Width);
                xout.AddColumn(rbf.Peak);
                foreach (double t in rbf.Centers)
                {
                    xout.AddColumn(t);
                }
                xout.WriteLine();
            }

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(RBFNetwork); }
        }

        #endregion
    }
}
