using System;

namespace Encog.ML.Train.Strategy.End
{
    public class EndMinutesStrategy : EndTrainingStrategy
    {
        private readonly int minutes;
        private int minutesLeft;
        private bool started;
        private long startedTime;

        public EndMinutesStrategy(int minutes_0)
        {
            minutes = minutes_0;
            started = false;
            minutesLeft = minutes_0;
        }

        /// <value>the minutesLeft</value>
        public int MinutesLeft
        {
            /// <returns>the minutesLeft</returns>
            get
            {
                lock (this)
                {
                    return minutesLeft;
                }
            }
        }


        /// <value>the minutes</value>
        public int Minutes
        {
            /// <returns>the minutes</returns>
            get { return minutes; }
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            lock (this)
            {
                return started && minutesLeft >= 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(MLTrain train)
        {
            started = true;
            startedTime = DateTime.Now.Millisecond;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            lock (this)
            {
                long now = DateTime.Now.Millisecond;
                minutesLeft = ((int) ((now - startedTime)/60000));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        #endregion
    }
}