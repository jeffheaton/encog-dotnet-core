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
using System.Text;
using Encog.App.Analyst;
using Encog.Engine.Network.Activation;
using Encog.Neural.Flat;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.File;

namespace Encog.App.Generate.Generators
{
    /// <summary>
    ///     Provides a basic implementation of a template generator.
    /// </summary>
    public abstract class AbstractTemplateGenerator : ITemplateGenerator
    {
        /// <summary>
        ///     The contents of the generated file.
        /// </summary>
        private readonly StringBuilder contents = new StringBuilder();

        /// <summary>
        ///     The Encog analyst that is being used.
        /// </summary>
        private EncogAnalyst analyst;

        /// <summary>
        ///     The current indention level.
        /// </summary>
        public int IndentLevel { get; set; }

        /// <summary>
        ///     The Encog analyst that we are using.
        /// </summary>
        public EncogAnalyst Analyst
        {
            get { return analyst; }
        }

        /// <summary>
        ///     A platform specific array set to null.
        /// </summary>
        public abstract String NullArray { get; }

        /// <summary>
        ///     Get a resource path to the template that we are using.
        /// </summary>
        public abstract String TemplatePath { get; }

        /// <summary>
        ///     Generate based on the provided Encog Analyst.
        /// </summary>
        /// <param name="theAnalyst">The Encog analyst to base this on.</param>
        public void Generate(EncogAnalyst theAnalyst)
        {
            string fileContent = ResourceLoader.LoadString(TemplatePath);
            using (var reader = new StringReader(fileContent))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("~~"))
                    {
                        ProcessToken(line.Substring(2).Trim());
                    }
                    else
                    {
                        contents.Append(line);
                        contents.Append("\n");
                    }
                }
            }
        }

        /// <summary>
        ///     The generated contents.
        /// </summary>
        public String Contents
        {
            get { return contents.ToString(); }
        }

        /// <summary>
        ///     Write the contents to the specified file.
        /// </summary>
        /// <param name="targetFile">The target file.</param>
        public void WriteContents(FileInfo targetFile)
        {
            File.WriteAllText(targetFile.ToString(), contents.ToString());
        }

        /// <summary>
        ///     Add a line, with proper indention.
        /// </summary>
        /// <param name="line">The line to add.</param>
        public void AddLine(String line)
        {
            for (int i = 0; i < IndentLevel; i++)
            {
                contents.Append("\t");
            }
            contents.Append(line);
            contents.Append("\n");
        }

        /// <summary>
        ///     Add a name value definition, as a double array.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">THe data.</param>
        public void AddNameValue(String name, double[] data)
        {
            var value = new StringBuilder();
            if (data == null)
            {
                value.Append(name);
                value.Append(" = " + NullArray + ";");
                AddLine(value.ToString());
            }
            else
            {
                ToBrokenList(value, data);
                AddNameValue(name, "{" + value + "}");
            }
        }

        /// <summary>
        ///     Add a name-value as an int.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">THe value.</param>
        public void AddNameValue(String name, int value)
        {
            AddNameValue(name, "" + value);
        }

        /// <summary>
        ///     Add a name-value array where the value is an int array.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The value.</param>
        public void AddNameValue(String name, int[] data)
        {
            var value = new StringBuilder();
            if (data == null)
            {
                value.Append(name);
                value.Append(" = " + NullArray + ";");
                AddLine(value.ToString());
            }
            else
            {
                ToBrokenList(value, data);
                AddNameValue(name, "{" + value + "}");
            }
        }

        /// <summary>
        ///     Add a name-value where a string is the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddNameValue(String name, String value)
        {
            var line = new StringBuilder();
            line.Append(name);
            line.Append(" = ");

            if (value == null)
            {
                line.Append(NullArray);
            }
            else
            {
                line.Append(value);
            }

            line.Append(";");
            AddLine(line.ToString());
        }

        /// <summary>
        ///     Create an array of activations based on a flat network.
        /// </summary>
        /// <param name="flat">The flat network.</param>
        /// <returns></returns>
        public int[] CreateActivations(FlatNetwork flat)
        {
            var result = new int[flat.ActivationFunctions.Length];
            for (int i = 0; i < flat.ActivationFunctions.Length; i++)
            {
                IActivationFunction af = flat.ActivationFunctions[i];

                if (af is ActivationLinear)
                {
                    result[i] = 0;
                }
                else if (af is ActivationTANH)
                {
                    result[i] = 1;
                }
                if (af is ActivationSigmoid)
                {
                    result[i] = 2;
                }
                if (af is ActivationElliottSymmetric)
                {
                    result[i] = 3;
                }
                if (af is ActivationElliott)
                {
                    result[i] = 4;
                }
            }

            return result;
        }

        /// <summary>
        ///     Create an array of doubles to hold the specified flat network.
        /// </summary>
        /// <param name="flat">The flat network to use as a model.</param>
        /// <returns>The new array.</returns>
        public double[] CreateParams(FlatNetwork flat)
        {
            var result = new double[flat.ActivationFunctions.Length];
            EngineArray.Fill(result, 1);
            return result;
        }

        /// <summary>
        ///     Indent to the right one.
        /// </summary>
        public void IndentIn()
        {
            IndentLevel++;
        }

        /// <summary>
        ///     Indent to the left one.
        /// </summary>
        public void IndentOut()
        {
            IndentLevel--;
        }

        /// <summary>
        ///     Process the specified token.
        /// </summary>
        /// <param name="command">The token to process.</param>
        public abstract void ProcessToken(String command);

        /// <summary>
        ///     Create an array list broken into 10 columns. This prevents a very large
        ///     array from creating a very long single line.
        /// </summary>
        /// <param name="result">The string builder to add to.</param>
        /// <param name="data">The data to convert.</param>
        public void ToBrokenList(StringBuilder result, double[] data)
        {
            int lineCount = 0;
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(',');
                }

                lineCount++;
                if (lineCount > 10)
                {
                    result.Append("\n");
                    lineCount = 0;
                }
                result.Append(CSVFormat.EgFormat.Format(data[i],
                                                        EncogFramework.DefaultPrecision));
            }
        }

        /// <summary>
        ///     Create an array list broken into 10 columns. This prevents a very large
        ///     array from creating a very long single line.
        /// </summary>
        /// <param name="result">The string builder to add to.</param>
        /// <param name="data">The data to convert.</param>
        public void ToBrokenList(StringBuilder result, int[] data)
        {
            int lineCount = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(',');
                }

                lineCount++;
                if (lineCount > 10)
                {
                    result.Append("\n");
                    lineCount = 0;
                }
                result.Append("" + data[i]);
            }
        }
    }
}
