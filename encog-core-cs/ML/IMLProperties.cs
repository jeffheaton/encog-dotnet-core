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

namespace Encog.ML
{
    /// <summary>
    /// Defines a Machine Learning Method that holds properties.
    /// </summary>
    ///
    public interface IMLProperties : IMLMethod
    {
        /// <value>A map of all properties.</value>
        IDictionary<String, String> Properties { get; }


        /// <summary>
        /// Get the specified property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a double.</returns>
        double GetPropertyDouble(String name);

        /// <summary>
        /// Get the specified property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the specified property.</param>
        /// <returns>The value of the specified property.</returns>
        long GetPropertyLong(String name);

        /// <summary>
        /// Get the specified property as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        String GetPropertyString(String name);

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The value of the property.</param>
        void SetProperty(String name, double d);

        /// <summary>
        /// Set a property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="l">The value of the property.</param>
        void SetProperty(String name, long l);

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="v">The value of the property.</param>
        void SetProperty(String name, String v);

        /// <summary>
        /// Update any objeccts when a property changes.
        /// </summary>
        ///
        void UpdateProperties();
    }
}
