using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    public class EndMinutesStrategy : IEndTrainingStrategy
    {
        private int minutes;
        private bool started;
        private int minutesLeft;
        private Stopwatch watch;

        public EndMinutesStrategy(int minutes)
        {
            this.minutes = minutes;
            started = false;
            minutesLeft = minutes;
            this.watch = new Stopwatch();
        }

        /**
         * {@inheritDoc}
         */
        public bool ShouldStop()
        {
            lock (this)
            {
                return started && this.minutesLeft >= 0;
            }
        }

        /**
         * {@inheritDoc}
         */
        public void Init(ITrain train)
        {
            this.started = true;
            this.watch.Start();
        }

        /**
         * {@inheritDoc}
         */
        public void PostIteration()
        {
            long elapsed = watch.ElapsedMilliseconds / 60000;
            this.minutesLeft = (int)(elapsed - this.minutes);
        }

        /**
         * {@inheritDoc}
         */
        public void PreIteration()
        {
        }

        /**
         * @return the minutesLeft
         */
        public int MinutesLeft
        {
            get
            {
                lock (this)
                {
                    return minutesLeft;
                }
            }
        }

        /**
         * @return the minutes
         */
        public int Minutes
        {
            get
            {
                return minutes;
            }
        }
    }
}
