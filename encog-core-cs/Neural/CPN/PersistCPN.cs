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
using Encog.MathUtil.Matrices;
using Encog.Persist;
using Encog.Util;

namespace Encog.Neural.CPN
{
    /// <summary>
    /// Persist a CPN network.
    /// </summary>
    ///
    public class PersistCPN : IEncogPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PropertyInputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PropertyInstarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PropertyWinnerCount = "winnerCount";

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
                                                           PersistConst.InputCount);
                    instarCount = EncogFileSection.ParseInt(paras,
                                                            PersistConst.Instar);
                    outputCount = EncogFileSection.ParseInt(paras,
                                                            PersistConst.OutputCount);
                    winnerCount = EncogFileSection.ParseInt(paras,
                                                            PropertyWinnerCount);
                    m1 = EncogFileSection.ParseMatrix(paras,
                                                      PropertyInputToInstar);
                    m2 = EncogFileSection.ParseMatrix(paras,
                                                      PropertyInstarToInput);
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

            xout.WriteProperty(PersistConst.InputCount, cpn.InputCount);
            xout.WriteProperty(PersistConst.Instar, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OutputCount, cpn.OutputCount);
            xout.WriteProperty(PropertyInputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PropertyInstarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PropertyWinnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }
}
