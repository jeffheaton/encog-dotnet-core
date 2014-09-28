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
using Encog.Persist;
using Encog.Util;

namespace Encog.Neural.BAM
{
    /// <summary>
    /// Persist the BAM network.
    /// </summary>
    ///
    public class PersistBAM : IEncogPersistor
    {
        #region EncogPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "BAM"; }
        }


        /// <inheritdoc/>
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
                    IDictionary<String, String> p = section.ParseParams();

                    result.F1Count = EncogFileSection.ParseInt(p,
                                                               PersistConst.PropertyF1Count);
                    result.F2Count = EncogFileSection.ParseInt(p,
                                                               PersistConst.PropertyF2Count);
                    result.WeightsF1ToF2 = EncogFileSection.ParseMatrix(p,PersistConst.PropertyWeightsF1F2);
                    result.WeightsF2ToF1 = EncogFileSection.ParseMatrix(p,PersistConst.PropertyWeightsF2F1);
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

            xout.WriteProperty(PersistConst.PropertyF1Count, bam.F1Count);
            xout.WriteProperty(PersistConst.PropertyF2Count, bam.F2Count);
            xout.WriteProperty(PersistConst.PropertyWeightsF1F2,
                               bam.WeightsF1ToF2);
            xout.WriteProperty(PersistConst.PropertyWeightsF2F1,
                               bam.WeightsF2ToF1);

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
