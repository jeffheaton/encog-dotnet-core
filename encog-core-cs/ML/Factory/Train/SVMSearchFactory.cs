//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
        public const String PROPERTY_GAMMA1 = "GAMMA1";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PROPERTY_C1 = "C1";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PROPERTY_GAMMA2 = "GAMMA2";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PROPERTY_C2 = "C2";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PROPERTY_GAMMA_STEP = "GAMMASTEP";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PROPERTY_C_STEP = "CSTEP";

        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public MLTrain Create(MLMethod method,
                              MLDataSet training, String argsStr)
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
                PROPERTY_GAMMA1, false,
                SVMSearchTrain.DEFAULT_GAMMA_BEGIN);
            double cStart = holder.GetDouble(PROPERTY_C1,
                                             false, SVMSearchTrain.DEFAULT_CONST_BEGIN);
            double gammaStop = holder.GetDouble(
                PROPERTY_GAMMA2, false,
                SVMSearchTrain.DEFAULT_GAMMA_END);
            double cStop = holder.GetDouble(PROPERTY_C2,
                                            false, SVMSearchTrain.DEFAULT_CONST_END);
            double gammaStep = holder.GetDouble(
                PROPERTY_GAMMA_STEP, false,
                SVMSearchTrain.DEFAULT_GAMMA_STEP);
            double cStep = holder.GetDouble(PROPERTY_C_STEP,
                                            false, SVMSearchTrain.DEFAULT_CONST_STEP);

            var result = new SVMSearchTrain((SupportVectorMachine) method, training);

            result.GammaBegin = gammaStart;
            result.GammaEnd = gammaStop;
            result.GammaStep = gammaStep;
            result.ConstBegin = cStart;
            result.ConstEnd = cStop;
            result.ConstStep = cStep;

            return result;
        }
    }
}
