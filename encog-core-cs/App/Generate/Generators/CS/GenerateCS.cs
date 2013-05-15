//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using Encog.App.Generate.Program;
using System.IO;
using Encog.ML;
using Encog.Persist;
using Encog.Util.Simple;
using Encog.ML.Data;
using Encog.Util.CSV;

namespace Encog.App.Generate.Generators.CS
{
    /// <summary>
    /// Generate C# code.
    /// </summary>
    public class GenerateCS : AbstractGenerator
    {
        private bool embed;

        private void EmbedNetwork(EncogProgramNode node)
        {
            AddBreak();

            FileInfo methodFile = (FileInfo)node.Args[0].Value;

            IMLMethod method = (IMLMethod)EncogDirectoryPersistence
                    .LoadObject(methodFile);

            if (!(method is IMLFactory))
            {
                throw new EncogError("Code generation not yet supported for: "
                        + method.GetType().Name);
            }

            IMLFactory factoryMethod = (IMLFactory)method;

            string methodName = factoryMethod.FactoryType;
            string methodArchitecture = factoryMethod.FactoryArchitecture;

            // header
            AddInclude("Encog.ML");
            AddInclude("Encog.Persist");

            StringBuilder line = new StringBuilder();
            line.Append("public static IMLMethod ");
            line.Append(node.Name);
            line.Append("()");
            AddLine(line.ToString());
            IndentLine("{");

            // create factory
            line.Length = 0;
            AddInclude("Encog.ML.Factory");
            line.Append("MLMethodFactory methodFactory = new MLMethodFactory();");
            AddLine(line.ToString());

            // factory create
            line.Length = 0;
            line.Append("IMLMethod result = ");

            line.Append("methodFactory.Create(");
            line.Append("\"");
            line.Append(methodName);
            line.Append("\"");
            line.Append(",");
            line.Append("\"");
            line.Append(methodArchitecture);
            line.Append("\"");
            line.Append(", 0, 0);");
            AddLine(line.ToString());

            line.Length = 0;
            AddInclude("Encog.ML");
            line.Append("((IMLEncodable)result).DecodeFromArray(WEIGHTS);");
            AddLine(line.ToString());

            // return
            AddLine("return result;");

            UnIndentLine("}");
        }

        private void EmbedTraining(EncogProgramNode node)
        {

            FileInfo dataFile = (FileInfo)node.Args[0].Value;
            IMLDataSet data = EncogUtility.LoadEGB2Memory(dataFile);

            // generate the input data

            IndentLine("public static readonly double[][] INPUT_DATA = {");
            foreach (IMLDataPair pair in data)
            {
                IMLData item = pair.Input;

                StringBuilder line = new StringBuilder();

                NumberList.ToList(CSVFormat.EgFormat, line, item);
                line.Insert(0, "new double[] { ");
                line.Append(" },");
                AddLine(line.ToString());
            }
            UnIndentLine("};");

            AddBreak();

            // generate the ideal data

            IndentLine("public static readonly double[][] IDEAL_DATA = {");
            foreach (IMLDataPair pair in data)
            {
                IMLData item = pair.Ideal;

                StringBuilder line = new StringBuilder();

                NumberList.ToList(CSVFormat.EgFormat, line, item);
                line.Insert(0, "new double[] { ");
                line.Append(" },");
                AddLine(line.ToString());
            }
            UnIndentLine("};");
        }

        public override void Generate(EncogGenProgram program, bool shouldEmbed)
        {
            this.embed = shouldEmbed;
            AddLine("namespace EncogGenerated");
            IndentLine("{");
            GenerateForChildren(program);
            GenerateImports(program);
            UnIndentLine("}");
        }

        private void GenerateArrayInit(EncogProgramNode node)
        {
            StringBuilder line = new StringBuilder();
            line.Append("public static readonly double[] ");
            line.Append(node.Name);
            line.Append(" = {");
            IndentLine(line.ToString());

            double[] a = (double[])node.Args[0].Value;

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

            UnIndentLine("};");
        }

        private void GenerateClass(EncogProgramNode node)
        {
            AddBreak();
            AddLine("public class " + node.Name);
            IndentLine("{");
            GenerateForChildren(node);
            UnIndentLine("}");
        }

        private void GenerateComment(EncogProgramNode commentNode)
        {
            AddLine("// " + commentNode.Name);
        }

        private void GenerateConst(EncogProgramNode node)
        {
            StringBuilder line = new StringBuilder();
            line.Append("public static readonly ");
            line.Append(node.Args[1].Value);
            line.Append(" ");
            line.Append(node.Name);
            line.Append(" = \"");
            line.Append(node.Args[0].Value);
            line.Append("\";");

            AddLine(line.ToString());
        }

        private void GenerateCreateNetwork(EncogProgramNode node)
        {
            if (this.embed)
            {
                EmbedNetwork(node);
            }
            else
            {
                LinkNetwork(node);
            }
        }

        private void GenerateEmbedTraining(EncogProgramNode node)
        {
            if (this.embed)
            {
                EmbedTraining(node);
            }
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

            StringBuilder line = new StringBuilder();
            line.Append("public static void ");
            line.Append(node.Name);
            line.Append("() {");
            IndentLine(line.ToString());

            GenerateForChildren(node);
            UnIndentLine("}");
        }

        private void GenerateFunctionCall(EncogProgramNode node)
        {
            AddBreak();
            StringBuilder line = new StringBuilder();
            if (node.Args[0].Value.ToString().Length > 0)
            {
                String objType = node.Args[0].Value.ToString();

                if (objType.Equals("MLMethod"))
                {
                    objType = "IMLMethod";
                }
                else if (objType.Equals("MLDataSet"))
                {
                    objType = "IMLDataSet";
                }

                line.Append(objType);
                line.Append(" ");
                line.Append(node.Args[1].Value.ToString());
                line.Append(" = ");
            }

            line.Append(node.Name);
            line.Append("();");
            AddLine(line.ToString());
        }

        private void GenerateImports(EncogGenProgram program)
        {
            StringBuilder imports = new StringBuilder();
            foreach (String str in Includes)
            {
                imports.Append("using ");
                imports.Append(str);
                imports.Append(";\n");
            }

            imports.Append("\n");

            AddToBeginning(imports.ToString());

        }

        private void GenerateLoadTraining(EncogProgramNode node)
        {
            AddBreak();

            FileInfo methodFile = (FileInfo)node.Args[0].Value;

            StringBuilder line = new StringBuilder();
            line.Append("public static MLDataSet createTraining() {");
            IndentLine(line.ToString());

            line.Length = 0;
            AddInclude("Encog.ML.Data");

            if (this.embed)
            {
                line.Append("IMLDataSet result = new BasicMLDataSet(INPUT_DATA,IDEAL_DATA);");
            }
            else
            {
                AddInclude("Encog.Util.Simple");
                line.Append("IMLDataSet result = EncogUtility.LoadEGB2Memory(new File(\"");
                line.Append(methodFile.ToString());
                line.Append("\"));");
            }

            AddLine(line.ToString());

            // return
            AddLine("return result;");

            UnIndentLine("}");
        }

        private void GenerateMainFunction(EncogProgramNode node)
        {
            AddBreak();
            AddLine("static void Main(string[] args)");
            IndentLine("{");
            GenerateForChildren(node);
            UnIndentLine("}");
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
                    GenerateCreateNetwork(node);
                    break;
                case NodeType.InitArray:
                    GenerateArrayInit(node);
                    break;
                case NodeType.EmbedTraining:
                    GenerateEmbedTraining(node);
                    break;
                case NodeType.LoadTraining:
                    GenerateLoadTraining(node);
                    break;
            }
        }

        private void LinkNetwork(EncogProgramNode node)
        {
            AddBreak();

            FileInfo methodFile = (FileInfo)node.Args[0].Value;

            AddInclude("Encog.ML");
            StringBuilder line = new StringBuilder();
            line.Append("public static IMLMethod ");
            line.Append(node.Name);
            line.Append("()");
            AddLine(line.ToString());
            IndentLine("{");

            line.Length = 0;
            line.Append("IMLMethod result = (IMLMethod)EncogDirectoryPersistence.LoadObject(new File(\"");
            line.Append(methodFile.ToString());
            line.Append("\"));");
            AddLine(line.ToString());

            // return
            AddLine("return result;");

            UnIndentLine("}");
        }
    }
}
