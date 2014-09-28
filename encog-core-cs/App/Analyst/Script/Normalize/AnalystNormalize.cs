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
using System.Linq;
using System.Text;
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Script.Prop;
using Encog.Util.Arrayutil;

namespace Encog.App.Analyst.Script.Normalize
{
    /// <summary>
    ///     This class holds information about the fields that the Encog Analyst will
    ///     normalize.
    /// </summary>
    public class AnalystNormalize
    {
        /// <summary>
        ///     The normalized fields.  These fields define the order and format
        ///     that data will be presented to the ML method.
        /// </summary>
        private readonly IList<AnalystField> _normalizedFields;

        /// <summary>
        ///     The parent script.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public AnalystNormalize(AnalystScript script)
        {
            _normalizedFields = new List<AnalystField>();
            _script = script;
        }

        /// <value>the normalizedFields</value>
        public IList<AnalystField> NormalizedFields
        {
            get { return _normalizedFields; }
        }

        /// <summary>
        ///     The missing values handler.
        /// </summary>
        public IHandleMissingValues MissingValues
        {
            get
            {
                String type = _script.Properties.GetPropertyString(
                    ScriptProperties.NormalizeMissingValues);

                if (type.Equals("DiscardMissing"))
                {
                    return new DiscardMissing();
                }
                else if (type.Equals("MeanAndModeMissing"))
                {
                    return new MeanAndModeMissing();
                }
                else if (type.Equals("NegateMissing"))
                {
                    return new NegateMissing();
                }
                else
                {
                    return new DiscardMissing();
                }
            }
            set
            {
                _script.Properties.SetProperty(
                    ScriptProperties.NormalizeMissingValues, value.GetType().Name);
            }
        }


        /// <returns>Calculate the input columns.</returns>
        public int CalculateInputColumns()
        {
            return _normalizedFields.Where(field => field.Input).Sum(field => field.ColumnsNeeded);
        }

        /// <summary>
        ///     Calculate the output columns.
        /// </summary>
        /// <returns>The output columns.</returns>
        public int CalculateOutputColumns()
        {
            return _normalizedFields.Where(field => field.Output).Sum(field => field.ColumnsNeeded);
        }


        /// <returns>Count the active fields.</returns>
        public int CountActiveFields()
        {
            int result = 0;

            foreach (AnalystField field  in  _normalizedFields)
            {
                if (field.Action != NormalizationAction.Ignore)
                {
                    result++;
                }
            }
            return result;
        }


        /// <summary>
        ///     Init the normalized fields.
        /// </summary>
        /// <param name="script">The script.</param>
        public void Init(AnalystScript script)
        {
            if (_normalizedFields == null)
            {
                return;
            }


            foreach (AnalystField norm  in  _normalizedFields)
            {
                DataField f = script.FindDataField(norm.Name);

                if (f == null)
                {
                    throw new AnalystError("Normalize specifies unknown field: "
                                           + norm.Name);
                }

                if (norm.Action == NormalizationAction.Normalize)
                {
                    norm.ActualHigh = f.Max;
                    norm.ActualLow = f.Min;
                }

                if ((norm.Action == NormalizationAction.Equilateral)
                    || (norm.Action == NormalizationAction.OneOf)
                    || (norm.Action == NormalizationAction.SingleField))
                {
                    int index = 0;

                    foreach (AnalystClassItem item  in  f.ClassMembers)
                    {
                        norm.Classes.Add(new ClassItem(item.Name, index++));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(": ");
            if (_normalizedFields != null)
            {
                result.Append(_normalizedFields);
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
