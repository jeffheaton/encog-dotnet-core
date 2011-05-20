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
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }


        /// <summary>
        /// The class string.
        /// </summary>
        ///
        public virtual String PersistClassString
        {
            get { return typeof (HopfieldNetwork).Name; }
        }


        /// <summary>
        /// Read a an object.
        /// </summary>
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
