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
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.Neural.CPN.Training
{
    /// <summary>
    /// Used for Instar training of a CPN neural network. A CPN network is a hybrid
    /// supervised/unsupervised network. The Outstar training handles the supervised
    /// portion of the training.
    /// </summary>
    ///
    public class TrainOutstar : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network being trained.
        /// </summary>
        ///
        private readonly CPNNetwork _network;

        /// <summary>
        /// The training data. Supervised training, so both input and ideal must be
        /// provided.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// If the weights have not been initialized, then they must be initialized
        /// before training begins. This will be done on the first iteration.
        /// </summary>
        ///
        private bool _mustInit;

        /// <summary>
        /// Construct the outstar trainer.
        /// </summary>
        ///
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theTraining">The training data, must provide ideal outputs.</param>
        /// <param name="theLearningRate">The learning rate.</param>
        public TrainOutstar(CPNNetwork theNetwork, IMLDataSet theTraining,
                            double theLearningRate) : base(TrainingImplementationType.Iterative)
        {
            _mustInit = true;
            _network = theNetwork;
            _training = theTraining;
            _learningRate = theLearningRate;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <inheritdoc />
        public override IMLMethod Method
        {
            get { return _network; }
        }

        #region ILearningRate Members

        /// <inheritdoc />
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        /// <summary>
        /// Approximate the weights based on the input values.
        /// </summary>
        ///
        private void InitWeight()
        {
            for (int i = 0; i < _network.OutstarCount; i++)
            {
                int j = 0;

                foreach (IMLDataPair pair  in  _training)
                {
                    _network.WeightsInstarToOutstar[j++, i] =
                        pair.Ideal[i];
                }
            }
            _mustInit = false;
        }

        /// <inheritdoc />
        public override sealed void Iteration()
        {
            if (_mustInit)
            {
                InitWeight();
            }

            var error = new ErrorCalculation();


            foreach (IMLDataPair pair  in  _training)
            {
                IMLData xout = _network.ComputeInstar(pair.Input);

                int j = EngineArray.IndexOfLargest(xout);
                for (int i = 0; i < _network.OutstarCount; i++)
                {
                    double delta = _learningRate
                                   *(pair.Ideal[i] - _network.WeightsInstarToOutstar[j, i]);
                    _network.WeightsInstarToOutstar.Add(j, i, delta);
                }

                IMLData out2 = _network.ComputeOutstar(xout);
                error.UpdateError(out2, pair.Ideal, pair.Significance);
            }

            Error = error.Calculate();
        }

        /// <inheritdoc />
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc />
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }
}
