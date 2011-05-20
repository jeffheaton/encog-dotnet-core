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
    public class PersistTrainingContinuation : EncogPersistor
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
                            double[] list = EncogFileSection
                                .ParseDoubleArray(paras, key);
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