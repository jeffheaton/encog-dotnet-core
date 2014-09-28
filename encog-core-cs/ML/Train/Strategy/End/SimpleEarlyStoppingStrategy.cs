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