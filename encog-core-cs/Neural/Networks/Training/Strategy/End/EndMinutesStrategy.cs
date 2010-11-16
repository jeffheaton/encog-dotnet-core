using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Encog.Util.Time;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    /// <summary>
    /// Strategy that ends after a number of minutes.
    /// </summary>
    public class EndMinutesStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The number of minutes to run for.
        /// </summary>
        private int minutes;

        /// <summary>
        /// Has the countdown started?
        /// </summary>
        private bool started;

        /// <summary>
        /// How many minutes are left.
        /// </summary>
        private int minutesLeft;

        /// <summary>
        /// The watch used for the countdown.
        /// </summary>
        private Stopwatch watch;

        /// <summary>
        /// Construct a countdown strategy.
        /// </summary>
        /// <param name="minutes">The number of minutes to run for.</param>
        public EndMinutesStrategy(int minutes)
        {
            this.minutes = minutes;
            started = false;
            minutesLeft = minutes;
            this.watch = new Stopwatch();
        }

        /// <inheritdoc/>
        public bool ShouldStop()
        {
            lock (this)
            {
                return started && this.minutesLeft >= 0;
            }
        }

        /// <inheritdoc/>
        public void Init(ITrain train)
        {
            this.started = true;
            this.watch.Start();
        }

        /// <inheritdoc/>
        public void PostIteration()
        {
            long elapsed = watch.ElapsedMilliseconds / 60000;
            this.minutesLeft = (int)(elapsed - this.minutes);
        }

        /// <inheritdoc/>
        public void PreIteration()
        {
        }

        /// <summary>
        /// The number of minutes left.
        /// </summary>
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

        /// <summary>
        /// The number of mintues to run for.
        /// </summary>
        public int Minutes
        {
            get
            {
                return minutes;
            }
        }
    }
}
