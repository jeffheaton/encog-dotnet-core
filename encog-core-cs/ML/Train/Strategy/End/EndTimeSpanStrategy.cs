using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// This End time span strategy gives greater specificity than EndMinutesStrategy. 
    /// You can specify Days, Hours, Minutes, Seconds etc...
    /// </summary>
    public class EndTimeSpanStrategy : IEndTrainingStrategy
    {
        private TimeSpan _duration;
        private bool _started;
        private DateTime _startedTime;
       
        /// <inheritdoc/>
        public EndTimeSpanStrategy(TimeSpan duration)
        {
            _duration = duration;
        }
        public void Init(IMLTrain train)
        {
            _started = true;
            _startedTime = DateTime.Now;
        }
        
        /// <inheritdoc/>
        public void PostIteration()
        {

        }

        /// <inheritdoc/>
        public void PreIteration()
        {
        
        }

        /// <inheritdoc/>
        public virtual bool ShouldStop()
        {
            lock (this)
            {
                return (DateTime.Now.Subtract(_startedTime) > _duration);
            }
        }
    }
}
