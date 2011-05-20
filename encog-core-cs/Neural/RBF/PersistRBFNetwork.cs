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
    public class PersistRBFNetwork : EncogPersistor
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
                else if (section.SectionName.Equals("RBF-NETWORK")
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
                        catch (Exception e)
                        {
                            throw new PersistError(e);
                        }
                        for (int i = 0; i < af.ParamNames.Length; i++)
                        {
                            af.SetParam(i,
                                        CSVFormat.EG_FORMAT.Parse(cols[i + 1]));
                        }

                        flat.ActivationFunctions[index++] = af;
                    }
                }
                else if (section.SectionName.Equals("RBF-NETWORK")
                         && section.SubSectionName.Equals("RBF"))
                {
                    int index_3 = 0;

                    int hiddenCount = flat.LayerCounts[1];
                    int inputCount = flat.LayerCounts[2];

                    flat.RBF = new IRadialBasisFunction[hiddenCount];


                    foreach (String line_4  in  section.Lines)
                    {
                        IRadialBasisFunction rbf = null;
                        IList<String> cols_5 = EncogFileSection
                            .SplitColumns(line_4);
                        String name_6 = ReflectionUtil.RBF_PATH + cols_5[0];
                        try
                        {
                            rbf = (IRadialBasisFunction) ReflectionUtil.LoadObject(name_6);
                        }
                        catch (TypeLoadException e_8)
                        {
                            throw new PersistError(e_8);
                        }
                        catch (TargetException e_9)
                        {
                            throw new PersistError(e_9);
                        }
                        catch (MemberAccessException e_10)
                        {
                            throw new PersistError(e_10);
                        }

                        rbf.Width = CSVFormat.EG_FORMAT.Parse(cols_5[1]);
                        rbf.Peak = CSVFormat.EG_FORMAT.Parse(cols_5[2]);
                        rbf.Centers = new double[inputCount];

                        for (int i_11 = 0; i_11 < inputCount; i_11++)
                        {
                            rbf.Centers[i_11] = CSVFormat.EG_FORMAT.Parse(cols_5[i_11 + 3]);
                        }

                        flat.RBF[index_3++] = rbf;
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
            xout.AddSubSection("RBF");

            foreach (IRadialBasisFunction rbf  in  flat.RBF)
            {
                xout.AddColumn(rbf.GetType().Name);
                xout.AddColumn(rbf.Width);
                xout.AddColumn(rbf.Peak);
                for (int i_0 = 0; i_0 < rbf.Centers.Length; i_0++)
                {
                    xout.AddColumn(rbf.Centers[i_0]);
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