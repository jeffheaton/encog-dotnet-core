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
using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Neural.Networks.Training.Simple
{
    /// <summary>
    /// Train an ADALINE neural network.
    /// </summary>
    ///
    public class TrainAdaline : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// The training data to use.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// Construct an ADALINE trainer.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learningRate">The learning rate.</param>
        public TrainAdaline(BasicNetwork network, IMLDataSet training,
                            double learningRate) : base(TrainingImplementationType.Iterative)
        {
            if (network.LayerCount > 2)
            {
                throw new NeuralNetworkError(
                    "An ADALINE network only has two layers.");
            }
            _network = network;

            _training = training;
            _learningRate = learningRate;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        /// <inheritdoc/>
        public override sealed void Iteration()
        {
            var errorCalculation = new ErrorCalculation();


            foreach (IMLDataPair pair  in  _training)
            {
                // calculate the error
                IMLData output = _network.Compute(pair.Input);

                for (int currentAdaline = 0; currentAdaline < output.Count; currentAdaline++)
                {
                    double diff = pair.Ideal[currentAdaline]
                                  - output[currentAdaline];

                    // weights
                    for (int i = 0; i <= _network.InputCount; i++)
                    {
                        double input;

                        if (i == _network.InputCount)
                        {
                            input = 1.0d;
                        }
                        else
                        {
                            input = pair.Input[i];
                        }

                        _network.AddWeight(0, i, currentAdaline,
                                          _learningRate*diff*input);
                    }
                }

                errorCalculation.UpdateError(output, pair.Ideal, pair.Significance);
            }

            // set the global error
            Error = errorCalculation.Calculate();
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
