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
using Encog.App.Generate.Program;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.CSV;
using Encog.Util.Simple;

namespace Encog.App.Generate.Generators.JS
{
    /// <summary>
    ///     Generate JavaScript.
    /// </summary>
    public class GenerateEncogJavaScript : AbstractGenerator
    {
        private void EmbedNetwork(EncogProgramNode node)
        {
            AddBreak();

            var methodFile = (FileInfo) node.Args[0].Value;

            var method = (IMLMethod) EncogDirectoryPersistence
                                         .LoadObject(methodFile);

            if (!(method is IMLFactory))
            {
                throw new EncogError("Code generation not yet supported for: "
                                     + method.GetType().Name);
            }

            FlatNetwork flat = ((IContainsFlat) method).Flat;

            // header
            var line = new StringBuilder();
            line.Append("public static MLMethod ");
            line.Append(node.Name);
            line.Append("() {");
            IndentLine(line.ToString());

            // create factory
            line.Length = 0;

            AddLine("var network = ENCOG.BasicNetwork.create( null );");
            AddLine("network.inputCount = " + flat.InputCount + ";");
            AddLine("network.outputCount = " + flat.OutputCount + ";");
            AddLine("network.layerCounts = "
                    + ToSingleLineArray(flat.LayerCounts) + ";");
            AddLine("network.layerContextCount = "
                    + ToSingleLineArray(flat.LayerContextCount) + ";");
            AddLine("network.weightIndex = "
                    + ToSingleLineArray(flat.WeightIndex) + ";");
            AddLine("network.layerIndex = "
                    + ToSingleLineArray(flat.LayerIndex) + ";");
            AddLine("network.activationFunctions = "
                    + ToSingleLineArray(flat.ActivationFunctions) + ";");
            AddLine("network.layerFeedCounts = "
                    + ToSingleLineArray(flat.LayerFeedCounts) + ";");
            AddLine("network.contextTargetOffset = "
                    + ToSingleLineArray(flat.ContextTargetOffset) + ";");
            AddLine("network.contextTargetSize = "
                    + ToSingleLineArray(flat.ContextTargetSize) + ";");
            AddLine("network.biasActivation = "
                    + ToSingleLineArray(flat.BiasActivation) + ";");
            AddLine("network.beginTraining = " + flat.BeginTraining + ";");
            AddLine("network.endTraining=" + flat.EndTraining + ";");
            AddLine("network.weights = WEIGHTS;");
            AddLine("network.layerOutput = "
                    + ToSingleLineArray(flat.LayerOutput) + ";");
            AddLine("network.layerSums = " + ToSingleLineArray(flat.LayerSums)
                    + ";");

            // return
            AddLine("return network;");

            UnIndentLine("}");
        }

        private void EmbedTraining(EncogProgramNode node)
        {
            var dataFile = (FileInfo) node.Args[0].Value;
            IMLDataSet data = EncogUtility.LoadEGB2Memory(dataFile);

            // generate the input data

            IndentLine("var INPUT_DATA = [");
            foreach (IMLDataPair pair in data)
            {
                IMLData item = pair.Input;

                var line = new StringBuilder();

                NumberList.ToList(CSVFormat.EgFormat, line, item);
                line.Insert(0, "[ ");
                line.Append(" ],");
                AddLine(line.ToString());
            }
            UnIndentLine("];");

            AddBreak();

            // generate the ideal data

            IndentLine("var IDEAL_DATA = [");
            foreach (IMLDataPair pair in data)
            {
                IMLData item = pair.Ideal;

                var line = new StringBuilder();

                NumberList.ToList(CSVFormat.EgFormat, line, item);
                line.Insert(0, "[ ");
                line.Append(" ],");
                AddLine(line.ToString());
            }
            UnIndentLine("];");
        }

        public override void Generate(EncogGenProgram program, bool shouldEmbed)
        {
            if (!shouldEmbed)
            {
                throw new AnalystCodeGenerationError(
                    "Must embed when generating Javascript");
            }
            GenerateForChildren(program);
        }

        private void GenerateArrayInit(EncogProgramNode node)
        {
            var line = new StringBuilder();
            line.Append("var ");
            line.Append(node.Name);
            line.Append(" = [");
            IndentLine(line.ToString());

            var a = (double[]) node.Args[0].Value;

            line.Length = 0;

            int lineCount = 0;
            for (int i = 0; i < a.Length; i++)
            {
                line.Append(CSVFormat.EgFormat.Format(a[i],
                                                      EncogFramework.DefaultPrecision));
                if (i < (a.Length - 1))
                {
                    line.Append(",");
                }

                lineCount++;
                if (lineCount >= 10)
                {
                    AddLine(line.ToString());
                    line.Length = 0;
                    lineCount = 0;
                }
            }

            if (line.Length > 0)
            {
                AddLine(line.ToString());
                line.Length = 0;
            }

            UnIndentLine("];");
        }

        private void GenerateClass(EncogProgramNode node)
        {
            AddBreak();

            AddLine("<!DOCTYPE html>");
            AddLine("<html>");
            AddLine("<head>");
            AddLine("<title>Encog Generated Javascript</title>");
            AddLine("</head>");
            AddLine("<body>");
            AddLine("<script src=\"../encog.js\"></script>");
            AddLine("<script src=\"../encog-widget.js\"></script>");
            AddLine("<pre>");
            AddLine("<script type=\"text/javascript\">");

            GenerateForChildren(node);

            AddLine("</script>");
            AddLine(
                "<noscript>Your browser does not support JavaScript! Note: if you are trying to view this in Encog Workbench, right-click file and choose \"Open as Text\".</noscript>");
            AddLine("</pre>");
            AddLine("</body>");
            AddLine("</html>");
        }

        private void GenerateComment(EncogProgramNode commentNode)
        {
            AddLine("// " + commentNode.Name);
        }

        private void GenerateConst(EncogProgramNode node)
        {
            var line = new StringBuilder();
            line.Append("var ");
            line.Append(node.Name);
            line.Append(" = \"");
            line.Append(node.Args[0].Value);
            line.Append("\";");

            AddLine(line.ToString());
        }

        private void GenerateForChildren(EncogTreeNode parent)
        {
            foreach (EncogProgramNode node in parent.Children)
            {
                GenerateNode(node);
            }
        }

        private void GenerateFunction(EncogProgramNode node)
        {
            AddBreak();

            var line = new StringBuilder();
            line.Append("function ");
            line.Append(node.Name);
            line.Append("() {");
            IndentLine(line.ToString());

            GenerateForChildren(node);
            UnIndentLine("}");
        }

        private void GenerateFunctionCall(EncogProgramNode node)
        {
            AddBreak();
            var line = new StringBuilder();
            if (node.Args[0].Value.ToString().Length > 0)
            {
                line.Append("var ");
                line.Append(node.Args[1].Value);
                line.Append(" = ");
            }

            line.Append(node.Name);
            line.Append("();");
            AddLine(line.ToString());
        }

        private void GenerateMainFunction(EncogProgramNode node)
        {
            AddBreak();
            GenerateForChildren(node);
        }

        private void GenerateNode(EncogProgramNode node)
        {
            switch (node.Type)
            {
                case NodeType.Comment:
                    GenerateComment(node);
                    break;
                case NodeType.Class:
                    GenerateClass(node);
                    break;
                case NodeType.MainFunction:
                    GenerateMainFunction(node);
                    break;
                case NodeType.Const:
                    GenerateConst(node);
                    break;
                case NodeType.StaticFunction:
                    GenerateFunction(node);
                    break;
                case NodeType.FunctionCall:
                    GenerateFunctionCall(node);
                    break;
                case NodeType.CreateNetwork:
                    EmbedNetwork(node);
                    break;
                case NodeType.InitArray:
                    GenerateArrayInit(node);
                    break;
                case NodeType.EmbedTraining:
                    EmbedTraining(node);
                    break;
            }
        }

        private String ToSingleLineArray(
            IActivationFunction[] activationFunctions)
        {
            var result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < activationFunctions.Length; i++)
            {
                if (i > 0)
                {
                    result.Append(',');
                }

                IActivationFunction af = activationFunctions[i];
                if (af is ActivationSigmoid)
                {
                    result.Append("ENCOG.ActivationSigmoid.create()");
                }
                else if (af is ActivationTANH)
                {
                    result.Append("ENCOG.ActivationTANH.create()");
                }
                else if (af is ActivationLinear)
                {
                    result.Append("ENCOG.ActivationLinear.create()");
                }
                else if (af is ActivationElliott)
                {
                    result.Append("ENCOG.ActivationElliott.create()");
                }
                else if (af is ActivationElliott)
                {
                    result.Append("ENCOG.ActivationElliott.create()");
                }
                else
                {
                    throw new AnalystCodeGenerationError(
                        "Unsupported activatoin function for code generation: "
                        + af.GetType().Name);
                }
            }
            result.Append(']');
            return result.ToString();
        }

        private String ToSingleLineArray(double[] d)
        {
            var line = new StringBuilder();
            line.Append("[");
            for (int i = 0; i < d.Length; i++)
            {
                line.Append(CSVFormat.EgFormat.Format(d[i],
                                                      EncogFramework.DefaultPrecision));
                if (i < (d.Length - 1))
                {
                    line.Append(",");
                }
            }
            line.Append("]");
            return line.ToString();
        }

        private String ToSingleLineArray(int[] d)
        {
            var line = new StringBuilder();
            line.Append("[");
            for (int i = 0; i < d.Length; i++)
            {
                line.Append(d[i]);
                if (i < (d.Length - 1))
                {
                    line.Append(",");
                }
            }
            line.Append("]");
            return line.ToString();
        }
    }
}
