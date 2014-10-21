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
using Encog.Util.CSV;

namespace Encog.ML
{
    /// <summary>
    /// A class that provides basic property functionality for the MLProperties
    /// interface.
    /// </summary>
    ///
    [Serializable]
    public abstract class BasicML : IMLProperties
    {

        /// <summary>
        /// Properties about the neural network. Some NeuralLogic classes require
        /// certain properties to be set.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _properties;

        /// <summary>
        /// Construct the object.
        /// </summary>
        protected BasicML()
        {
            _properties = new Dictionary<String, String>();
        }

        #region MLProperties Members

        /// <value>A map of all properties.</value>
        public IDictionary<String, String> Properties
        {
            get { return _properties; }
        }


        /// <summary>
        /// Get the specified property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a double.</returns>
        public double GetPropertyDouble(String name)
        {
            return (CSVFormat.EgFormat.Parse((_properties[name])));
        }

        /// <summary>
        /// Get the specified property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the specified property.</param>
        /// <returns>The value of the specified property.</returns>
        public long GetPropertyLong(String name)
        {
            return (Int64.Parse(_properties[name]));
        }

        /// <summary>
        /// Get the specified property as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public String GetPropertyString(String name)
        {
            if (_properties.ContainsKey(name))
            {
                return (_properties[name]);
            }
            return null;
        }

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The value of the property.</param>
        public void SetProperty(String name, double d)
        {
            _properties[name] = CSVFormat.EgFormat.Format(d, EncogFramework.DefaultPrecision);
            UpdateProperties();
        }

        /// <summary>
        /// Set a property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="l">The value of the property.</param>
        public void SetProperty(String name, long l)
        {
            _properties[name] = "" + l;
            UpdateProperties();
        }

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="v">The value of the property.</param>
        public void SetProperty(String name, String v)
        {
            _properties[name] = v;
            UpdateProperties();
        }

        /// <summary>
        /// Update from the propeties stored in the hash map.  Should be called 
        /// whenever the properties change and might need to be reloaded.
        /// </summary>
        public abstract void UpdateProperties();

        #endregion
    }
}
