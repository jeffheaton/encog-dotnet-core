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
using Encog.ML.Train.Strategy.End;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// Stop early when validation set no longer improves.
    ///
    /// Based on the following paper:
    /// 
    /// @techreport{Prechelt94c,
    /// author    = {Lutz Prechelt},
    /// title     = {{PROBEN1} --- {A} Set of Benchmarks and Benchmarking
    ///              Rules for Neural Network Training Algorithms},
    /// institution = {Fakult\"at f\"ur Informatik, Universit\"at Karlsruhe},
    /// year      = {1994},
    /// number    = {21/94},
    /// address   = {D-76128 Karlsruhe, Germany},
    /// month     = sep,
    /// note      = {Anonymous FTP: /pub/pa\-pers/tech\-reports/1994/1994-21.ps.Z
    ///              on ftp.ira.uka.de},
    /// }
    /// 
    /// </summary>
    public class EarlyStoppingStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// Alpha value, calculated for early stopping. Once "gl" is above alpha, training will stop.
        /// </summary>
        private readonly double _alpha;

        private readonly double _minEfficiency;

        /// <summary>
        /// Validation strip length.
        /// </summary>
        private readonly int _stripLength;

        /// <summary>
        /// The test set.
        /// </summary>
        private readonly IMLDataSet _testSet;

        /// <summary>
        /// The validation set.
        /// </summary>
        private readonly IMLDataSet _validationSet;

        /// <summary>
        /// The error calculation.
        /// </summary>
        private IMLError _calc;

        /// <summary>
        /// eOpt value, calculated for early stopping.  
        /// The lowest validation error so far.
        /// </summary>
        private double _eOpt;

        /// <summary>
        /// gl value, calculated for early stopping.
        /// </summary>
        private double _gl;

        /// <summary>
        /// The last time the test set was checked.
        /// </summary>
        private int _lastCheck;

        /// <summary>
        /// Has training stopped.
        /// </summary>
        private bool _stop;

        private double _stripEfficiency;

        private double _stripOpt;
        private double _stripTotal;

        /// <summary>
        /// Current test error.
        /// </summary>
        private double _testError;

        /// <summary>
        /// The trainer.
        /// </summary>
        private IMLTrain _train;

        /// <summary>
        /// Current training error.
        /// </summary>
        private double _trainingError;

        /// <summary>
        /// Current validation error.
        /// </summary>
        private double _validationError;

        /// <summary>
        /// Construct the early stopping strategy.
        /// Use default operating parameters. 
        /// </summary>
        /// <param name="theValidationSet">The validation set.</param>
        /// <param name="theTestSet">The test set.</param>
        public EarlyStoppingStrategy(IMLDataSet theValidationSet,
                                     IMLDataSet theTestSet)
            : this(theValidationSet, theTestSet, 5, 5, 0.1)
        {
        }

        /// <summary>
        /// Construct the early stopping strategy. 
        /// </summary>
        /// <param name="theValidationSet">The validation set.</param>
        /// <param name="theTestSet">The test set.</param>
        /// <param name="theStripLength">The number of training set elements to validate.</param>
        /// <param name="theAlpha">Stop once GL is below this value.</param>
        /// <param name="theMinEfficiency">The minimum training efficiency to stop.</param>
        public EarlyStoppingStrategy(IMLDataSet theValidationSet,
                                     IMLDataSet theTestSet, int theStripLength, double theAlpha,
                                     double theMinEfficiency)
        {
            _validationSet = theValidationSet;
            _testSet = theTestSet;
            _alpha = theAlpha;
            _stripLength = theStripLength;
            _minEfficiency = theMinEfficiency;
        }

        /// <summary>
        /// The training error.
        /// </summary>
        public double TrainingError
        {
            get { return _trainingError; }
        }

        /// <summary>
        /// The test error.
        /// </summary>
        public double TestError
        {
            get { return _testError; }
        }

        /// <summary>
        /// The validation error.
        /// </summary>
        public double ValidationError
        {
            get { return _validationError; }
        }

        /// <summary>
        /// The Opt.
        /// </summary>
        public double Opt
        {
            get { return _eOpt; }
        }

        /// <summary>
        /// The GL.
        /// </summary>
        public double Gl
        {
            get { return _gl; }
        }

        /// <summary>
        /// The strip length.
        /// </summary>
        public int StripLength
        {
            get { return _stripLength; }
        }

        /// <summary>
        /// StripOpt.
        /// </summary>
        public double StripOpt
        {
            get { return _stripOpt; }
        }

        /// <summary>
        /// The strip efficiency.
        /// </summary>
        public double StripEfficiency
        {
            get { return _stripEfficiency; }
        }

        /// <summary>
        /// The minimum efficicency to allow before stopping.
        /// </summary>
        public double MinEfficiency
        {
            get { return _minEfficiency; }
        }

        #region IEndTrainingStrategy Members

        /// <inheritdoc/>
        public void Init(IMLTrain theTrain)
        {
            _train = theTrain;
            _calc = (IMLError) _train.Method;
            _eOpt = Double.PositiveInfinity;
            _stripOpt = Double.PositiveInfinity;
            _stop = false;
            _lastCheck = 0;
        }

        /// <inheritdoc/>
        public void PreIteration()
        {
        }

        /// <inheritdoc/>
        public void PostIteration()
        {
            _lastCheck++;
            _trainingError = _train.Error;

            _stripOpt = Math.Min(_stripOpt, _trainingError);
            _stripTotal += _trainingError;

            if (_lastCheck > _stripLength)
            {
                _validationError = _calc.CalculateError(_validationSet);
                _testError = _calc.CalculateError(_testSet);
                _eOpt = Math.Min(_validationError, _eOpt);
                _gl = 100.0*((_validationError/_eOpt) - 1.0);

                _stripEfficiency = (_stripTotal)
                                  /(_stripLength*_stripOpt);

                //System.out.println("eff=" + this.stripEfficiency + ", gl=" + this.gl);

                // setup for next time
                _stripTotal = 0;
                _lastCheck = 0;

                // should we stop?
                _stop = (_gl > _alpha)
                       || (_stripEfficiency < _minEfficiency);
            }
        }

        /// <summary>
        /// Returns true if we should stop.
        /// </summary>
        /// <returns>Returns true if we should stop.</returns>
        public bool ShouldStop()
        {
            return _stop;
        }

        #endregion
    }
}
