using System;
using System.Collections.Generic;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Allows training to be continued.
    /// </summary>
    ///
    public class TrainingContinuation
    {
        /// <summary>
        /// The contents of this object.
        /// </summary>
        ///
        private readonly IDictionary<String, Object> contents;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public TrainingContinuation()
        {
            contents = new Dictionary<String, Object>();
        }


        /// <value>The contents.</value>
        public IDictionary<String, Object> Contents
        {
            /// <returns>The contents.</returns>
            get { return contents; }
        }

        /// <value>the trainingType to set</value>
        public String TrainingType { /// <returns>the trainingType</returns>
            get; /// <param name="trainingType_0">the trainingType to set</param>
            set; }

        /// <summary>
        /// Get an object by name.
        /// </summary>
        ///
        /// <param name="name">The name of the object.</param>
        /// <returns>The object requested.</returns>
        public Object Get(String name)
        {
            return contents[name];
        }


        /// <summary>
        /// Save a list of doubles.
        /// </summary>
        ///
        /// <param name="key">The key to save them under.</param>
        /// <param name="list">The list of doubles.</param>
        public void Put(String key, double[] list)
        {
            contents[key] = list;
        }

        /// <summary>
        /// Set a value to a string.
        /// </summary>
        ///
        /// <param name="name">The value to set.</param>
        /// <param name="v">The value.</param>
        public void Set(String name, Object v)
        {
            contents[name] = v;
        }
    }
}