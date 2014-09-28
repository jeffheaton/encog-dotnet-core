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

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Allows training to be continued.
    /// </summary>
    [Serializable]
    public class TrainingContinuation
    {
        /// <summary>
        /// The contents of this object.
        /// </summary>
        ///
        private readonly IDictionary<String, Object> _contents;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public TrainingContinuation()
        {
            _contents = new Dictionary<String, Object>();
        }


        /// <value>The contents.</value>
        public IDictionary<String, Object> Contents
        {
            get { return _contents; }
        }

        /// <value>the trainingType to set</value>
        public String TrainingType { 
            get; 
            set; }

        /// <summary>
        /// Get an object by name.
        /// </summary>
        ///
        /// <param name="name">The name of the object.</param>
        /// <returns>The object requested.</returns>
        public Object Get(String name)
        {
            return _contents[name];
        }


        /// <summary>
        /// Save a list of doubles.
        /// </summary>
        ///
        /// <param name="key">The key to save them under.</param>
        /// <param name="list">The list of doubles.</param>
        public void Put(String key, double[] list)
        {
            _contents[key] = list;
        }

        /// <summary>
        /// Set a value to a string.
        /// </summary>
        ///
        /// <param name="name">The value to set.</param>
        /// <param name="v">The value.</param>
        public void Set(String name, Object v)
        {
            _contents[name] = v;
        }
    }
}
