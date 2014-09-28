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
using Encog.Neural.Networks.Training;
using Encog.Util.Logging;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// A simple greedy strategy. If the last iteration did not improve training,
    /// then discard it. Care must be taken with this strategy, as sometimes a
    /// training algorithm may need to temporarily decrease the error level before
    /// improving it.
    /// </summary>
    ///
    public class Greedy : IStrategy
    {
        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// The last state of the network, so that we can restore to this
        /// state if needed.
        /// </summary>
        ///
        private double[] _lastNetwork;

        /// <summary>
        /// The method training.
        /// </summary>
        private IMLEncodable _method;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start 
        /// evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public virtual void Init(IMLTrain train)
        {
            _train = train;
            _ready = false;

            if (!(train.Method is IMLEncodable))
            {
                throw new TrainingError(
                    "To make use of the Greedy strategy the machine learning method must support MLEncodable.");
            }

            _method = ((IMLEncodable) train.Method);
            _lastNetwork = new double[_method.EncodedArrayLength()];
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                if (_train.Error > _lastError)
                {
                    EncogLogging.Log(EncogLogging.LevelDebug,
                                     "Greedy strategy dropped last iteration.");
                    _train.Error = _lastError;
                    _method.DecodeFromArray(_lastNetwork);
                }
            }
            else
            {
                _ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public virtual void PreIteration()
        {
            if (_method != null)
            {
                _lastError = _train.Error;
                _method.EncodeToArray(_lastNetwork);
                _train.Error = _lastError;
            }
        }

        #endregion
    }
}
