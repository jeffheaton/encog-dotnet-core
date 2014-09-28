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
using System.IO;

namespace Encog.Persist
{
    /// <summary>
    /// This interface defines an Encog Persistor. An Encog persistor will write an
    /// Encog object to an EG file.
    /// </summary>
    ///
    public interface IEncogPersistor
    {
        /// <summary>
        /// The native type.
        /// </summary>
        Type NativeType { get;  }
        
        /// <value>Get the class string for the object.</value>
        String PersistClassString { get; }

        /// <value>Get the file version used by this persistor.</value>
        int FileVersion { get; }


        /// <summary>
        /// Read the object from an input stream.
        /// </summary>
        ///
        /// <param name="mask0">The input stream.</param>
        /// <returns>The object.</returns>
        Object Read(Stream mask0);

        /// <summary>
        /// Save the object.
        /// </summary>
        ///
        /// <param name="os">The output stream to save to.</param>
        /// <param name="obj">The object to save.</param>
        void Save(Stream os, Object obj);
    }
}
