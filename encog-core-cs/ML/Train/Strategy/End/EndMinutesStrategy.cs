using System;

namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// End training when a specified number of minutes is up.
    /// </summary>
    public class EndMinutesStrategy : EndTrainingStrategy
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
                return _started && _minutesLeft >= 0;
            }
        }

        /// <inheritdoc/>
        public virtual void Init(MLTrain train)
        {
            _started = true;
            _startedTime = DateTime.Now.Millisecond;
        }

        /// <inheritdoc/>
        public virtual void PostIteration()
        {
            lock (this)
            {
                long now = DateTime.Now.Millisecond;
                _minutesLeft = ((int) ((now - _startedTime)/60000));
            }
        }

        /// <inheritdoc/>
        public virtual void PreIteration()
        {
        }

        #endregion
    }
}