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

namespace Encog.Neural.SOM
{
    /// <summary>
    /// Persist the SOM.
    /// </summary>
    ///
    public class PersistSOM : IEncogPersistor
    {
        /**
	 * {@inheritDoc}
	 */

        #region IEncogPersistor Members

        public int FileVersion
        {
            get { return 1; }
        }

        /**
	 * {@inheritDoc}
	 */

        public String PersistClassString
        {
            get { return "SOMNetwork"; }
        }


        /**
	 * {@inheritDoc}
	 */

        public Object Read(Stream istream)
        {
            var result = new SOMNetwork();
            var reader = new EncogReadHelper(istream);
            EncogFileSection section;

            while ((section = reader.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    EngineArray.PutAll(p, result.Properties);
                }
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    result.Weights = EncogFileSection.ParseMatrix(p,
                                                                  PersistConst.Weights)
                        ;
                }
            }

            return result;
        }

        /**
	 * {@inheritDoc}
	 */

        public void Save(Stream os, Object obj)
        {
            var writer = new EncogWriteHelper(os);
            var som = (SOMNetwork) obj;
            writer.AddSection("SOM");
            writer.AddSubSection("PARAMS");
            writer.AddProperties(som.Properties);
            writer.AddSubSection("NETWORK");
            writer.WriteProperty(PersistConst.Weights, som.Weights);
            writer.WriteProperty(PersistConst.InputCount, som.InputCount);
            writer.WriteProperty(PersistConst.OutputCount, som.OutputCount);
            writer.Flush();
        }


        public Type NativeType
        {
            get { return typeof (SOMNetwork); }
        }

        #endregion
    }
}
