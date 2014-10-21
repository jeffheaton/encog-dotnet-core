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
using Encog.ML.Data;

namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    ///     A simple early stopping strategy that halts training when the validation set no longer improves.
    /// </summary>
    public class SimpleEarlyStoppingStrategy : IEndTrainingStrategy
    {
        private readonly int _checkFrequency;

        /// <summary>
        ///     The validation set.
        /// </summary>
        private readonly IMLDataSet _validationSet;

        /// <summary>
        ///     Current validation error.
        /// </summary>
        private IMLError _calc;

        private int _lastCheck;

        private double _lastError;

        /// <summary>
        ///     Current validation error.
        /// </summary>
        private double _lastValidationError;

        /// <summary>
        ///     Has training stopped.
        /// </summary>
        private bool _stop;

        /// <summary>
        ///     The trainer.
        /// </summary>
        private IMLTrain _train;

        /// <summary>
        ///     Current training error.
        /// </summary>
        private double _trainingError;


        public SimpleEarlyStoppingStrategy(IMLDataSet theValidationSet) :
            this(theValidationSet, 5)
        {
        }


        public SimpleEarlyStoppingStrategy(IMLDataSet theValidationSet,
            int theCheckFrequency)
        {
            _validationSet = theValidationSet;
            _checkFrequency = theCheckFrequency;
        }

        /// <summary>
        ///     Thetraining error.
        /// </summary>
        public double TrainingError
        {
            get { return _trainingError; }
        }


        /// <summary>
        ///     The validation error.
        /// </summary>
        public double ValidationError
        {
            get { return _lastValidationError; }
        }

        /// <inheritdoc />
        public void Init(IMLTrain theTrain)
        {
            _train = theTrain;
            _calc = (IMLError) _train.Method;
            _stop = false;
            _lastCheck = 0;
            _lastValidationError = _calc.CalculateError(_validationSet);
        }

        /// <inheritdoc />
        public void PreIteration()
        {
        }

        /// <inheritdoc />
        public void PostIteration()
        {
            _lastCheck++;
            _trainingError = _train.Error;

            if (_lastCheck > _checkFrequency || Double.IsInfinity(_lastValidationError))
            {
                _lastCheck = 0;

                double currentValidationError = _calc.CalculateError(_validationSet);

                if (currentValidationError >= _lastValidationError)
                {
                    _stop = true;
                }

                _lastValidationError = currentValidationError;
            }
        }

        /// <inheritdoc />
        public bool ShouldStop()
        {
            return _stop;
        }
    }
}
