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
using Encog.MathUtil.RBF;
using Encog.ML.Data;
using Encog.ML.Factory.Parse;
using Encog.ML.SVM;
using Encog.ML.Train;
using Encog.Neural.SOM;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.Util;
using Encog.Util.CSV;

namespace Encog.ML.Factory.Train
{
    /// <summary>
    /// Train an SOM network with a neighborhood method.
    /// </summary>
    ///
    public class NeighborhoodSOMFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new EncogError(
                    "Neighborhood training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            String neighborhoodStr = holder.GetString(
                MLTrainFactory.PropertyNeighborhood, false, "rbf");
            String rbfTypeStr = holder.GetString(
                MLTrainFactory.PropertyRBFType, false, "gaussian");

            RBFEnum t;

            if (rbfTypeStr.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfTypeStr.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfTypeStr.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfTypeStr.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            INeighborhoodFunction nf = null;

            if (neighborhoodStr.Equals("bubble", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodBubble(1);
            }
            else if (neighborhoodStr.Equals("rbf", StringComparison.InvariantCultureIgnoreCase))
            {
                String str = holder.GetString(
                    MLTrainFactory.PropertyDimensions, true, null);
                int[] size = NumberList.FromListInt(CSVFormat.EgFormat, str);
                nf = new NeighborhoodRBF(size, t);
            }
            else if (neighborhoodStr.Equals("rbf1d", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodRBF1D(t);
            }
            if (neighborhoodStr.Equals("single", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodSingle();
            }

            var result = new BasicTrainSOM((SOMNetwork) method,
                                           learningRate, training, nf);

            if (args.ContainsKey(MLTrainFactory.PropertyIterations))
            {
                int plannedIterations = holder.GetInt(
                    MLTrainFactory.PropertyIterations, false, 1000);
                double startRate = holder.GetDouble(
                    MLTrainFactory.PropertyStartLearningRate, false, 0.05d);
                double endRate = holder.GetDouble(
                    MLTrainFactory.PropertyEndLearningRate, false, 0.05d);
                double startRadius = holder.GetDouble(
                    MLTrainFactory.PropertyStartRadius, false, 10);
                double endRadius = holder.GetDouble(
                    MLTrainFactory.PropertyEndRadius, false, 1);
                result.SetAutoDecay(plannedIterations, startRate, endRate,
                                    startRadius, endRadius);
            }

            return result;
        }
    }
}
