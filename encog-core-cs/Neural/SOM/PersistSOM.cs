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

namespace Encog.Neural.SOM
{
    /// <summary>
    /// Persist the SOM.
    /// </summary>
    ///
    public class PersistSOM : IEncogPersistor
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
            get { return "SOM"; }
        }


        /// <inheritdoc/>
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
                    IDictionary<String, String> p = section.ParseParams();
                    result.Weights = EncogFileSection.ParseMatrix(p,
                                                                  PersistConst.Weights);
                    result.OutputCount = EncogFileSection.ParseInt(p,
                                                                         PersistConst.OutputCount);
                    result.InputCount = EncogFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var som = (SOMNetwork) obj;
            xout.AddSection("SOM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(som.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.Weights, som.Weights);
            xout.WriteProperty(PersistConst.InputCount, som.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, som.OutputCount);
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
