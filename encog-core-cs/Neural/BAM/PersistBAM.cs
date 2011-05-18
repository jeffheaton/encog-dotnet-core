using System;
using System.Collections.Generic;
using System.IO;
using Encog.Persist;
using Encog.Util;

namespace Encog.Neural.BAM
{
    /// <summary>
    /// Persist the BAM network.
    /// </summary>
    ///
    public class PersistBAM : EncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int FileVersion
        {
            /// <summary>
            /// 
            /// </summary>
            ///
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
            get { return "BAM"; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new BAMNetwork();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BAM")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();

                    result.F1Count = EncogFileSection.ParseInt(params_0,
                                                               PersistConst.PROPERTY_F1_COUNT);
                    result.F2Count = EncogFileSection.ParseInt(params_0,
                                                               PersistConst.PROPERTY_F2_COUNT);
                    result.WeightsF1toF2 = EncogFileSection.ParseMatrix(params_0,
                                                                        PersistConst.PROPERTY_WEIGHTS_F1_F2);
                    result.WeightsF2toF1 = EncogFileSection.ParseMatrix(params_0,
                                                                        PersistConst.PROPERTY_WEIGHTS_F2_F1);
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
            var bam = (BAMNetwork) obj;
            xout.AddSection("BAM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(bam.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.PROPERTY_F1_COUNT, bam.F1Count);
            xout.WriteProperty(PersistConst.PROPERTY_F2_COUNT, bam.F2Count);
            xout.WriteProperty(PersistConst.PROPERTY_WEIGHTS_F1_F2,
                               bam.WeightsF1toF2);
            xout.WriteProperty(PersistConst.PROPERTY_WEIGHTS_F2_F1,
                               bam.WeightsF2toF1);

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BAMNetwork); }
        }

        #endregion
    }
}