using System;
using System.Collections.Generic;
using System.IO;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// Persist the Hopfield network.
    /// </summary>
    ///
    public class PersistHopfield : EncogPersistor
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
            get { return typeof (HopfieldNetwork).Name; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new HopfieldNetwork();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("HOPFIELD")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("HOPFIELD")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();
                    result.Weights = NumberList.FromList(CSVFormat.EG_FORMAT,
                                                         (params_0[PersistConst.WEIGHTS]));
                    result.SetCurrentState(NumberList.FromList(CSVFormat.EG_FORMAT,
                                                               (params_0[PersistConst.OUTPUT])));
                    result.NeuronCount = EncogFileSection.ParseInt(params_0,
                                                                   PersistConst.NEURON_COUNT);
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
            var hopfield = (HopfieldNetwork) obj;
            xout.AddSection("HOPFIELD");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(hopfield.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.WEIGHTS, hopfield.Weights);
            xout.WriteProperty(PersistConst.OUTPUT, hopfield.CurrentState.Data);
            xout.WriteProperty(PersistConst.NEURON_COUNT, hopfield.NeuronCount);
            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(HopfieldNetwork); }
        }

        #endregion
    }
}