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

namespace Encog.Util.Normalize.Output
{
    /// <summary>
    /// Provides very basic functionality that other output field groups
    /// will use.  Mainly provides the list of fields that are grouped.
    /// </summary>
    [Serializable]
    public abstract class BasicOutputFieldGroup : IOutputFieldGroup
    {
        /// <summary>
        /// The fields in this group.
        /// </summary>
        private readonly ICollection<OutputFieldGrouped> _fields = new List<OutputFieldGrouped>();

        #region IOutputFieldGroup Members

        /// <summary>
        /// Add a field to this group.
        /// </summary>
        /// <param name="field">The field to add to the group.</param>
        public void AddField(OutputFieldGrouped field)
        {
            _fields.Add(field);
        }

        /// <summary>
        /// The list of grouped fields.
        /// </summary>
        public ICollection<OutputFieldGrouped> GroupedFields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Init for a new row.
        /// </summary>
        public abstract void RowInit();

        #endregion
    }
}
