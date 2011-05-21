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

namespace Encog.ML.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for a list of innovations.
    /// </summary>
    ///
    [Serializable]
    public class BasicInnovationList : IInnovationList
    {
        /// <summary>
        /// The list of innovations.
        /// </summary>
        ///
        private readonly IList<IInnovation> _list;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BasicInnovationList()
        {
            _list = new List<IInnovation>();
        }

        #region IInnovationList Members

        /// <summary>
        /// Add an innovation.
        /// </summary>
        ///
        /// <param name="innovation">The innovation to add.</param>
        public void Add(IInnovation innovation)
        {
            _list.Add(innovation);
        }

        /// <summary>
        /// Get a specific innovation, by index.
        /// </summary>
        ///
        /// <param name="id">The innovation index id.</param>
        /// <returns>The innovation.</returns>
        public IInnovation Get(int id)
        {
            return _list[id];
        }


        /// <value>A list of innovations.</value>
        public IList<IInnovation> Innovations
        {
            get { return _list; }
        }

        #endregion
    }
}
