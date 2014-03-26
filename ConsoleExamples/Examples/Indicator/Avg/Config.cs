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
using Encog.ML.Factory;

namespace Encog.Examples.Indicator.Avg
{
    public class Config
    {
        /// <summary>
        /// The maximum range (either positive or negative) that the difference between the fast and slow will be normalized to.
        /// </summary>
        public const int DiffRange = 50;

        /// <summary>
        /// The maximum range (either positive or negative) that the pip profit(or loss) will be in.
        /// </summary>
        public const int PipRange = 35;

        /// <summary>
        /// The size of a single PIP (i.e. 0.0001 for EURUSD)
        /// </summary>
        public const double PipSize = 0.0001;

        /// <summary>
        /// The size of the input window.  This is the number of previous bars to consider.
        /// </summary>
        public const int InputWindow = 3;

        /// <summary>
        /// The number of bars to look forward to determine a max profit, or loss.
        /// </summary>
        public const int PredictWindow = 10;

        /// <summary>
        /// The targeted error.  Once the training error reaches this value, training will stop.
        /// </summary>
        public const float TargetError = 0.05f;

        /// <summary>
        /// The type of method.  This is an Encog factory code.
        /// </summary>
        public const String MethodType = MLMethodFactory.TypeFeedforward;

        /// <summary>
        /// The architecture of the method.  This is an Encog factory code.
        /// </summary>
        public const String MethodArchitecture = "?:B->TANH->20:B->TANH->?";

        /// <summary>
        /// The type of training.  This is an Encog factory code.
        /// </summary>
        public const String TrainType = MLTrainFactory.TypeRPROP;

        /// <summary>
        /// The training parameters.  This is an Encog factory code.
        /// </summary>
        public const String TrainParams = "";

        /// <summary>
        /// The filename for the training data.
        /// </summary>
        public const String FilenameTrain = "training.egb";

        /// <summary>
        /// The filename to store the method to.
        /// </summary>
        public const String MethodName = "method.eg";
    }
}
