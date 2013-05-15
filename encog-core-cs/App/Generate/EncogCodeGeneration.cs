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
using Encog.ML;
using Encog.Neural.Networks;
using Encog.App.Generate.Generators;
using Encog.App.Generate.Program;
using Encog.App.Analyst;
using Encog.App.Analyst.Script.Prop;
using System.IO;
using Encog.Persist;
using Encog.App.Generate.Generators.Java;
using Encog.App.Generate.Generators.CS;
using Encog.App.Generate.Generators.MQL4;
using Encog.App.Generate.Generators.NinjaScript;
using Encog.App.Generate.Generators.JS;

namespace Encog.App.Generate
{
    /// <summary>
    /// Perform Encog code generation. Encog is capable of generating code from
    /// several different objects. This code generation will be to the specified
    /// target language.
    /// </summary>
    public class EncogCodeGeneration
    {
        /**
	 * Is the specified method supported for code generation?
	 * 
	 * @param method
	 *            The specified method.
	 * @return True, if the specified method is supported.
	 */
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

        /**
         * The target language for the code generation.
         */
        private TargetLanguage targetLanguage;

        /**
         * True if the data should be embedded.
         */
        public bool EmbedData { get; set; }

        /**
         * The language specific code generator.
         */
        private ILanguageSpecificGenerator generator;

        /**
         * The program that we are generating.
         */
        private EncogGenProgram program = new EncogGenProgram();

        /**
         * Construct the generation object.
         * 
         * @param theTargetLanguage
         *            The target language.
         */
        public EncogCodeGeneration(TargetLanguage theTargetLanguage)
        {
            this.targetLanguage = theTargetLanguage;

            switch (theTargetLanguage)
            {
                case TargetLanguage.NoGeneration:
                    throw new AnalystCodeGenerationError(
                            "No target language has been specified for code generation.");
                case TargetLanguage.Java:
                    this.generator = new GenerateEncogJava();
                    break;
                case TargetLanguage.CSharp:
                    this.generator = new GenerateCS();
                    break;
                case TargetLanguage.MQL4:
                    this.generator = new GenerateMQL4();
                    break;
                case TargetLanguage.NinjaScript:
                    this.generator = new GenerateNinjaScript();
                    break;
                case TargetLanguage.JavaScript:
                    this.generator = new GenerateEncogJavaScript();
                    break;

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

            if (this.targetLanguage == TargetLanguage.MQL4
                    || this.targetLanguage == TargetLanguage.NinjaScript)
            {
                if (!EmbedData)
                {
                    throw new AnalystCodeGenerationError(
                            "MQL4 and Ninjascript must be embedded.");
                }
            }

            if (this.generator is IProgramGenerator)
            {
                String methodID = analyst.Script.Properties.GetPropertyString(ScriptProperties.MlConfigMachineLearningFile);

                String trainingID = analyst.Script.Properties.GetPropertyString(ScriptProperties.MlConfigTrainingFile);

                FileInfo methodFile = analyst.Script.ResolveFilename(methodID);
                FileInfo trainingFile = analyst.Script.ResolveFilename(trainingID);

                Generate(methodFile, trainingFile);
            }
            else
            {
                ((ITemplateGenerator)this.generator).Generate(analyst);
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
            this.program.AddComment("Code generated by Encog v"
                    + EncogFramework.Instance.Properties[EncogFramework.EncogVersion]);
            this.program.AddComment("Generation Date: " + new DateTime().ToString());
            this.program.AddComment("Generated code may be used freely");
            this.program.AddComment("http://www.heatonresearch.com/encog");
            EncogProgramNode mainClass = this.program.CreateClass("EncogExample");

            if (this.targetLanguage == TargetLanguage.MQL4
                    || this.targetLanguage == TargetLanguage.NinjaScript)
            {
                throw new AnalystCodeGenerationError(
                        "MQL4 and Ninjascript can only be generated from Encog Analyst");
            }

            if (data != null)
            {
                mainClass.EmbedTraining(data);
                if (!(this.generator is GenerateEncogJavaScript))
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
                if (!(this.generator is GenerateEncogJavaScript))
                {
                    mainFunction.CreateFunctionCall("createTraining", "MLDataSet",
                            "training");
                }
            }
            mainFunction
                    .AddComment("Network and/or data is now loaded, you can add code to train, evaluate, etc.");

            ((IProgramGenerator)this.generator).Generate(this.program, EmbedData);
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
                IMLEncodable encodable = (IMLEncodable)EncogDirectoryPersistence
                        .LoadObject(method);
                double[] weights = new double[encodable.EncodedArrayLength()];
                encodable.EncodeToArray(weights);
                mainClass.CreateArray("WEIGHTS", weights);
            }

            return mainClass.CreateNetworkFunction("createNetwork", method);
        }

        /**
         * @return the targetLanguage
         */
        public TargetLanguage TargetLanguage
        {
            get
            {
                return this.targetLanguage;
            }
        }

        /**
         * Save the contents to a string.
         * 
         * @return The contents.
         */
        public String Save()
        {
            return this.generator.Contents;
        }

        /**
         * Save the contents to the specified file.
         * 
         * @param file
         */
        public void Save(FileInfo file)
        {
            this.generator.WriteContents(file);
        }

    }
}
