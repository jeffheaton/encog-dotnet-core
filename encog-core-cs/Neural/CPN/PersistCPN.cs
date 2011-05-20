using System;
using System.Collections.Generic;
using System.IO;
using Encog.MathUtil.Matrices;
using Encog.Persist;
using Encog.Util;

namespace Encog.Neural.CPN
{
    /// <summary>
    /// Persist a CPN network.
    /// </summary>
    ///
    public class PersistCPN : EncogPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PROPERTY_inputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PROPERTY_instarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PROPERTY_winnerCount = "winnerCount";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(CPNNetwork); }
        }

        #region EncogPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "CPN"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            IDictionary<String, String> networkParams = null;
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;
            int inputCount = 0;
            int instarCount = 0;
            int outputCount = 0;
            int winnerCount = 0;
            Matrix m1 = null;
            Matrix m2 = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    inputCount = EncogFileSection.ParseInt(paras,
                                                           PersistConst.INPUT_COUNT);
                    instarCount = EncogFileSection.ParseInt(paras,
                                                            PersistConst.INSTAR);
                    outputCount = EncogFileSection.ParseInt(paras,
                                                            PersistConst.OUTPUT_COUNT);
                    winnerCount = EncogFileSection.ParseInt(paras,
                                                            PROPERTY_winnerCount);
                    m1 = EncogFileSection.ParseMatrix(paras,
                                                      PROPERTY_inputToInstar);
                    m2 = EncogFileSection.ParseMatrix(paras,
                                                      PROPERTY_instarToInput);
                }
            }

            var result = new CPNNetwork(inputCount, instarCount, outputCount,
                                        winnerCount);
            EngineArray.PutAll(networkParams, result.Properties);
            result.WeightsInputToInstar.Set(m1);
            result.WeightsInstarToOutstar.Set(m2);
            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var cpn = (CPNNetwork) obj;
            xout.AddSection("CPN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(cpn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.INPUT_COUNT, cpn.InputCount);
            xout.WriteProperty(PersistConst.INSTAR, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, cpn.OutputCount);
            xout.WriteProperty(PROPERTY_inputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PROPERTY_instarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PROPERTY_winnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }
}