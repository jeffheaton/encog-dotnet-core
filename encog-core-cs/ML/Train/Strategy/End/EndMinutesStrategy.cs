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

namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// End training when a specified number of minutes is up.
    /// </summary>
    public class EndMinutesStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The number of minutes to train for.
        /// </summary>
        private readonly int _minutes;

        /// <summary>
        /// The number of minutes that are left.
        /// </summary>
        private int _minutesLeft;

        /// <summary>
        /// True if training has started.
        /// </summary>
        private bool _started;

        /// <summary>
        /// The starting time for training.
        /// </summary>
        private long _startedTime;

        /// <summary>
        /// Construct the strategy object.
        /// </summary>
        /// <param name="minutes"></param>
        public EndMinutesStrategy(int minutes)
        {
            _minutes = minutes;
            _started = false;
            _minutesLeft = minutes;
        }

        /// <value>the minutesLeft</value>
        public int MinutesLeft
        {
            get
            {
                lock (this)
                {
                    return _minutesLeft;
                }
            }
        }


        /// <value>the minutes</value>
        public int Minutes
        {
            get { return _minutes; }
        }

        #region EndTrainingStrategy Members

        /// <inheritdoc/>
        public virtual bool ShouldStop()
        {
            lock (this)
            {
                return _started && _minutesLeft <= 0;
            }
        }

        /// <inheritdoc/>
        public virtual void Init(IMLTrain train)
        {
            _started = true;
            _startedTime = DateTime.Now.Ticks;
        }

        /// <inheritdoc/>
        public virtual void PostIteration()
        {
            lock (this)
            {
                long now = DateTime.Now.Ticks;
                long elapsedTicks = now - _startedTime;
                int elapsedMinutes = (int)(elapsedTicks / TimeSpan.TicksPerMinute);
                _minutesLeft = _minutes - elapsedMinutes;
            }
        }

        /// <inheritdoc/>
        public virtual void PreIteration()
        {
        }

        #endregion
    }
}
