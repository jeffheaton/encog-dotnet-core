using System;
using System.Collections.Generic;
using System.IO;
using Encog.Persist;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// Persist the Boltzmann machine.
    /// </summary>
    ///
    public class PersistBoltzmann : EncogPersistor
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
            get { return "BoltzmannMachine"; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new BoltzmannMachine();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BOLTZMANN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BOLTZMANN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> params_0 = section.ParseParams();
                    result.Weights = NumberList.FromList(CSVFormat.EG_FORMAT,
                                                         (params_0[PersistConst.WEIGHTS]));
                    result.SetCurrentState(NumberList.FromList(CSVFormat.EG_FORMAT,
                                                               (params_0[PersistConst.OUTPUT])));
                    result.NeuronCount = EncogFileSection.ParseInt(params_0,
                                                                   PersistConst.NEURON_COUNT);

                    result.Threshold = NumberList.FromList(CSVFormat.EG_FORMAT,
                                                           (params_0[PersistConst.THRESHOLDS]));
                    result.AnnealCycles = EncogFileSection.ParseInt(params_0,
                                                                    BoltzmannMachine.ANNEAL_CYCLES);
                    result.RunCycles = EncogFileSection.ParseInt(params_0,
                                                                 BoltzmannMachine.RUN_CYCLES);
                    result.Temperature = EncogFileSection.ParseDouble(params_0,
                                                                      PersistConst.TEMPERATURE);
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
            var boltz = (BoltzmannMachine) obj;
            xout.AddSection("BOLTZMANN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(boltz.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.WEIGHTS, boltz.Weights);
            xout.WriteProperty(PersistConst.OUTPUT, boltz.CurrentState.Data);
            xout.WriteProperty(PersistConst.NEURON_COUNT, boltz.NeuronCount);

            xout.WriteProperty(PersistConst.THRESHOLDS, boltz.Threshold);
            xout.WriteProperty(BoltzmannMachine.ANNEAL_CYCLES,
                               boltz.AnnealCycles);
            xout.WriteProperty(BoltzmannMachine.RUN_CYCLES, boltz.RunCycles);
            xout.WriteProperty(PersistConst.TEMPERATURE, boltz.Temperature);

            xout.Flush();
        }

        #endregion
    }
}