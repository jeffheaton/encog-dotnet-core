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
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.SVM;
using Encog.ML.SVM.Training;
using Encog.ML.Train;
using Encog.Util;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// A factory that creates SVM-search trainers.
    /// </summary>
    ///
    public class SVMSearchFactory
    {
        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma1 = "GAMMA1";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC1 = "C1";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma2 = "GAMMA2";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC2 = "C2";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGammaStep = "GAMMASTEP";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyCStep = "CSTEP";

        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new EncogError(
                    "SVM Train training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            new ParamsHolder(args);

            var holder = new ParamsHolder(args);
            double gammaStart = holder.GetDouble(
                PropertyGamma1, false,
                SVMSearchTrain.DefaultGammaBegin);
            double cStart = holder.GetDouble(PropertyC1,
                                             false, SVMSearchTrain.DefaultConstBegin);
            double gammaStop = holder.GetDouble(
                PropertyGamma2, false,
                SVMSearchTrain.DefaultGammaEnd);
            double cStop = holder.GetDouble(PropertyC2,
                                            false, SVMSearchTrain.DefaultConstEnd);
            double gammaStep = holder.GetDouble(
                PropertyGammaStep, false,
                SVMSearchTrain.DefaultGammaStep);
            double cStep = holder.GetDouble(PropertyCStep,
                                            false, SVMSearchTrain.DefaultConstStep);

            var result = new SVMSearchTrain((SupportVectorMachine) method, training)
                             {
                                 GammaBegin = gammaStart,
                                 GammaEnd = gammaStop,
                                 GammaStep = gammaStep,
                                 ConstBegin = cStart,
                                 ConstEnd = cStop,
                                 ConstStep = cStep
                             };

            return result;
        }
    }
}
