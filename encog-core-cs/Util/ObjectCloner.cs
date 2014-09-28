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
using System.Runtime.Serialization.Formatters.Binary;

namespace Encog.Util
{
    /// <summary>
    /// A simple Object cloner that uses serialization. Actually works really well
    /// for the somewhat complex nature of BasicNetwork. Performs a deep copy without
    /// all the headache of programming a custom clone.
    /// 
    /// From a Java example at:
    /// 
    /// http://www.javaworld.com/javaworld/javatips/jw-javatip76.html?page=2
    /// 
    /// </summary>
    public class ObjectCloner
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ObjectCloner()
        {
        }

        /// <summary>
        /// Perform a deep copy.
        /// </summary>
        /// <param name="oldObj">The old object.</param>
        /// <returns>The new object.</returns>
        public static Object DeepCopy(Object oldObj)
        {
            var formatter = new BinaryFormatter();

            using (var memory = new MemoryStream())
            {
                try
                {
                    // serialize and pass the object
                    formatter.Serialize(memory, oldObj);
                    memory.Flush();
                    memory.Position = 0;

                    // return the new object
                    return formatter.Deserialize(memory);
                }
                catch (Exception e)
                {
                    throw new EncogError(e);
                }
            }            
        }
    }
}
