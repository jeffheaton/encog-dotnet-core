using System;
using System.Collections.Generic;
using System.IO;
using Encog.Persist;
using Encog.Util;

namespace Encog.Neural.SOM
{
    /// <summary>
    /// Persist the SOM.
    /// </summary>
    ///
    public class PersistSOM : EncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual int FileVersion
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
        public virtual String PersistClassString
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return "SOM"; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new SOMNetwork();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();
                    result.Weights = EncogFileSection.ParseMatrix(params_0,
                                                                  PersistConst.WEIGHTS);
                    result.OutputNeuronCount = EncogFileSection.ParseInt(params_0,
                                                                         PersistConst.OUTPUT_COUNT);
                    result.InputCount = EncogFileSection.ParseInt(params_0,
                                                                  PersistConst.INPUT_COUNT);
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
            var som = (SOMNetwork) obj;
            xout.AddSection("SOM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(som.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.WEIGHTS, som.Weights);
            xout.WriteProperty(PersistConst.INPUT_COUNT, som.InputCount);
            xout.WriteProperty(PersistConst.OUTPUT_COUNT, som.OutputNeuronCount);
            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(SOMNetwork); }
        }

        #endregion
    }
}