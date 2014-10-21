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
using System.Text;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// This class specifies how fields are to be used by the TemporalWindowCSV
    /// class.
    /// </summary>
    ///
    public class TemporalWindowField
    {
        /// <summary>
        /// The action that is to be taken on this field.
        /// </summary>
        ///
        private TemporalType _action;

        /// <summary>
        /// The name of this field.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the field to be considered.</param>
        public TemporalWindowField(String theName)
        {
            _name = theName;
        }


        /// <value>the action to set</value>
        public TemporalType Action
        {
            get { return _action; }
            set { _action = value; }
        }


        /// <value>Returns true, if this field is to be used as part of the input
        /// for a prediction.</value>
        public bool Input
        {
            get { return ((_action == TemporalType.Input) || (_action == TemporalType.InputAndPredict)); }
        }


        /// <value>the lastValue to set</value>
        public String LastValue { get; set; }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <value>Returns true, if this field is part of what is being predicted.</value>
        public bool Predict
        {
            get { return ((_action == TemporalType.Predict) || (_action == TemporalType.InputAndPredict)); }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", action=");
            result.Append(_action);

            result.Append("]");
            return result.ToString();
        }
    }
}
