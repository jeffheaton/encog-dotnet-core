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

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Persist the training continuation.
    /// </summary>
    ///
    public class PersistTrainingContinuation : IEncogPersistor
    {
        #region EncogPersistor Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof (TrainingContinuation); }
        }

        /// <inheritdoc/>
        public virtual String PersistClassString
        {
            get { return "TrainingContinuation"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new TrainingContinuation();
            var ins0 = new EncogReadHelper(mask0);
            EncogFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CONT")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key  in  paras.Keys)
                    {
                        if (key.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.TrainingType = paras[key];
                        }
                        else
                        {
                            double[] list = section.ParseDoubleArray(paras, key);
                            result.Put(key, list);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new EncogWriteHelper(os);
            var cont = (TrainingContinuation) obj;
            xout.AddSection("CONT");
            xout.AddSubSection("PARAMS");
            xout.WriteProperty("type", cont.TrainingType);

            foreach (String key  in  cont.Contents.Keys)
            {
                var list = (double[]) cont.Get(key);
                xout.WriteProperty(key, list);
            }
            xout.Flush();
        }

        #endregion
    }
}
