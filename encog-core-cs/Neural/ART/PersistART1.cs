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

namespace Encog.Neural.ART
{
    /// <summary>
    /// Persist an ART1 network.  
    /// </summary>
    ///
    [Serializable]
    public class PersistART1 : IEncogPersistor
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
            get { return "ART1"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new ART1();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("ART1")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("ART1")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.A1 = EncogFileSection.ParseDouble(p,
                                                             BasicART.PropertyA1);
                    result.B1 = EncogFileSection.ParseDouble(p,
                                                             BasicART.PropertyB1);
                    result.C1 = EncogFileSection.ParseDouble(p,
                                                             BasicART.PropertyC1);
                    result.D1 = EncogFileSection.ParseDouble(p,
                                                             BasicART.PropertyD1);
                    result.F1Count = EncogFileSection.ParseInt(p,
                                                               PersistConst.PropertyF1Count);
                    result.F2Count = EncogFileSection.ParseInt(p,
                                                               PersistConst.PropertyF2Count);
                    result.NoWinner = EncogFileSection.ParseInt(p,
                                                                BasicART.PropertyNoWinner);
                    result.L = EncogFileSection
                        .ParseDouble(p, BasicART.PropertyL);
                    result.Vigilance = EncogFileSection.ParseDouble(p,
                                                                    BasicART.PropertyVigilance);
                    result.WeightsF1ToF2 = EncogFileSection.ParseMatrix(p,
                                                                        PersistConst.PropertyWeightsF1F2);
                    result.WeightsF2ToF1 = EncogFileSection.ParseMatrix(p,
                                                                        PersistConst.PropertyWeightsF2F1);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var art1 = (ART1) obj;
            xout.AddSection("ART1");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(art1.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(BasicART.PropertyA1, art1.A1);
            xout.WriteProperty(BasicART.PropertyB1, art1.B1);
            xout.WriteProperty(BasicART.PropertyC1, art1.C1);
            xout.WriteProperty(BasicART.PropertyD1, art1.D1);
            xout.WriteProperty(PersistConst.PropertyF1Count, art1.F1Count);
            xout.WriteProperty(PersistConst.PropertyF2Count, art1.F2Count);
            xout.WriteProperty(BasicART.PropertyNoWinner, art1.NoWinner);
            xout.WriteProperty(BasicART.PropertyL, art1.L);
            xout.WriteProperty(BasicART.PropertyVigilance, art1.Vigilance);
            xout.WriteProperty(PersistConst.PropertyWeightsF1F2,
                               art1.WeightsF1ToF2);
            xout.WriteProperty(PersistConst.PropertyWeightsF2F1,
                               art1.WeightsF2ToF1);

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(ART1); }
        }

        #endregion
    }
}
