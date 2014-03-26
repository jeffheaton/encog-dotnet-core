//
// Encog(tm) Core v3.2 - .Net Version
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
using System.Reflection;
using Encog.Examples;

namespace ConsoleExamples.Examples
{
    public class ExampleInfo : IComparable<ExampleInfo>
    {
        public ExampleInfo(Type type, String command, String title, String description)
        {
            ExampleType = type;
            Command = command;
            Title = title;
            Description = description;
        }

        public String Command { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public Type ExampleType { get; set; }

        #region IComparable<ExampleInfo> Members

        public int CompareTo(ExampleInfo other)
        {
            return Command.CompareTo(other.Command);
        }

        #endregion

        public IExample CreateInstance()
        {
            var result = (IExample) Assembly.GetExecutingAssembly().CreateInstance(ExampleType.FullName);
            return result;
        }
    }
}
