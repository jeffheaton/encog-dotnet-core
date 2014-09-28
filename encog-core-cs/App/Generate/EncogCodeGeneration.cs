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
using Encog.App.Analyst;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Generate.Generators;
using Encog.App.Generate.Generators.CS;
using Encog.App.Generate.Generators.JS;
using Encog.App.Generate.Generators.Java;
using Encog.App.Generate.Generators.MQL4;
using Encog.App.Generate.Generators.NinjaScript;
using Encog.App.Generate.Program;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Persist;

namespace Encog.App.Generate
{
    /// <summary>
    ///     Perform Encog code generation. Encog is capable of generating code from
    ///     several different objects. This code generation will be to the specified
    ///     target language.
    /// </summary>
    public class EncogCodeGeneration
    {
        /**
         * The language specific code generator.
         */
        private readonly ILanguageSpecificGenerator generator;

        /**
         * The program that we are generating.
         */
        private readonly EncogGenProgram program = new EncogGenProgram();

        /// <summary>
        ///     The target language for the code generation.
        /// </summary>
        private readonly TargetLanguage targetLanguage;

        /**
         * Construct the generation object.
         * 
         * @param theTargetLanguage
         *            The target language.
         */

        public EncogCodeGeneration(TargetLanguage theTargetLanguage)
        {
            targetLanguage = theTargetLanguage;

            switch (theTargetLanguage)
            {
                case TargetLanguage.NoGeneration:
                    throw new AnalystCodeGenerationError(
                        "No target language has been specified for code generation.");
                case TargetLanguage.Java:
                    generator = new GenerateEncogJava();
                    break;
                case TargetLanguage.CSharp:
                    generator = new GenerateCS();
                    break;
                case TargetLanguage.MQL4:
                    generator = new GenerateMQL4();
                    break;
                case TargetLanguage.NinjaScript:
                    generator = new GenerateNinjaScript();
                    break;
                case TargetLanguage.JavaScript:
                    generator = new GenerateEncogJavaScript();
                    break;
            }
        }

        public bool EmbedData { get; set; }

        public TargetLanguage TargetLanguage
        {
            get { return targetLanguage; }
        }

        /// <summary>
        ///     Is the specified method supported for code generation?
        /// </summary>
        /// <param name="method">The specified method.</param>
        /// <returns>True, if the specified method is supported.</returns>
        public static bool IsSupported(IMLMethod method)
        {
            if (method is BasicNetwork)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///     Get the extension fot the specified language.
        /// </summary>
        /// <param name="lang">The specified language.</param>
        /// <returns></returns>
        public static string GetExtension(TargetLanguage lang)
        {
            if (lang == TargetLanguage.Java)
            {
                return "java";
            }
            else if (lang == TargetLanguage.JavaScript)
            {
                return "html";
            }
            else if (lang == TargetLanguage.CSharp)
            {
                return "cs";
            }
            else if (lang == TargetLanguage.MQL4)
            {
                return "mql4";
            }
            else if (lang == TargetLanguage.NinjaScript)
            {
                return "cs";
            }
            else
            {
                return "txt";
            }
        }

        /**
         * Generate the code from Encog Analyst.
         * 
         * @param analyst
         *            The Encog Analyst object to use for code generation.
         */

        public void Generate(EncogAnalyst analyst)
        {
            if (targetLanguage == TargetLanguage.MQL4
                || targetLanguage == TargetLanguage.NinjaScript)
            {
                if (!EmbedData)
                {
                    throw new AnalystCodeGenerationError(
                        "MQL4 and Ninjascript must be embedded.");
                }
            }

            if (generator is IProgramGenerator)
            {
                String methodID =
                    analyst.Script.Properties.GetPropertyString(ScriptProperties.MlConfigMachineLearningFile);

                String trainingID = analyst.Script.Properties.GetPropertyString(ScriptProperties.MlConfigTrainingFile);

                FileInfo methodFile = analyst.Script.ResolveFilename(methodID);
                FileInfo trainingFile = analyst.Script.ResolveFilename(trainingID);

                Generate(methodFile, trainingFile);
            }
            else
            {
                ((ITemplateGenerator) generator).Generate(analyst);
            }
        }

        /**
         * Generate from a method and data.
         * 
         * @param method
         *            The machine learning method to generate from.
         * @param data
         *            The data to use perform generation.
         */

        public void Generate(FileInfo method, FileInfo data)
        {
            EncogProgramNode createNetworkFunction = null;
            program.AddComment("Code generated by Encog v"
                               + EncogFramework.Instance.Properties[EncogFramework.EncogVersion]);
            program.AddComment("Generation Date: " + new DateTime().ToString());
            program.AddComment("Generated code may be used freely");
            program.AddComment("http://www.heatonresearch.com/encog");
            EncogProgramNode mainClass = program.CreateClass("EncogExample");

            if (targetLanguage == TargetLanguage.MQL4
                || targetLanguage == TargetLanguage.NinjaScript)
            {
                throw new AnalystCodeGenerationError(
                    "MQL4 and Ninjascript can only be generated from Encog Analyst");
            }

            if (data != null)
            {
                mainClass.EmbedTraining(data);
                if (!(generator is GenerateEncogJavaScript))
                {
                    mainClass.GenerateLoadTraining(data);
                }
            }

            if (method != null)
            {
                createNetworkFunction = GenerateForMethod(mainClass, method);
            }

            EncogProgramNode mainFunction = mainClass.CreateMainFunction();

            if (createNetworkFunction != null)
            {
                mainFunction.CreateFunctionCall(createNetworkFunction, "MLMethod",
                                                "method");
            }

            if (data != null)
            {
                if (!(generator is GenerateEncogJavaScript))
                {
                    mainFunction.CreateFunctionCall("createTraining", "MLDataSet",
                                                    "training");
                }
            }
            mainFunction
                .AddComment("Network and/or data is now loaded, you can add code to train, evaluate, etc.");

            ((IProgramGenerator) generator).Generate(program, EmbedData);
        }

        /**
         * GEnerate from a machine learning method.
         * 
         * @param mainClass
         *            The main class.
         * @param method
         *            The filename of the method.
         * @return The newly created node.
         */

        private EncogProgramNode GenerateForMethod(
            EncogProgramNode mainClass, FileInfo method)
        {
            if (EmbedData)
            {
                var encodable = (IMLEncodable) EncogDirectoryPersistence
                                                   .LoadObject(method);
                var weights = new double[encodable.EncodedArrayLength()];
                encodable.EncodeToArray(weights);
                mainClass.CreateArray("WEIGHTS", weights);
            }

            return mainClass.CreateNetworkFunction("createNetwork", method);
        }

        /**
         * @return the targetLanguage
         */

        /**
         * Save the contents to a string.
         * 
         * @return The contents.
         */

        public String Save()
        {
            return generator.Contents;
        }

        /**
         * Save the contents to the specified file.
         * 
         * @param file
         */

        public void Save(FileInfo file)
        {
            generator.WriteContents(file);
        }
    }
}
