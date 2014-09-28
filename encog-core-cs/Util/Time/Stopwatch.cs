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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Encog.Util.Time
{
    public class Stopwatch
    {
        private long _startTick;
        private long _elapsed;
        private bool _isRunning;

        /// <summary>
        /// Creates a new instance of the class and starts the watch immediately.
        /// </summary>
        /// <returns>An instance of Stopwatch, running.</returns>
        public static Stopwatch StartNew()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// Creates an instance of the Stopwatch class.
        /// </summary>
        public Stopwatch() { }

        /// <summary>
        /// Completely resets and deactivates the timer.
        /// </summary>
        public void Reset()
        {
            _elapsed = 0;
            _isRunning = false;
            _startTick = 0;
        }

        /// <summary>
        /// Begins the timer.
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _startTick = GetCurrentTicks();
                _isRunning = true;
            }
        }

        /// <summary>
        /// Stops the current timer.
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                _elapsed += GetCurrentTicks() - _startTick;
                _isRunning = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is currently recording.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        /// <summary>
        /// Gets the Elapsed time as a Timespan.
        /// </summary>
        public TimeSpan Elapsed
        {
            get { return TimeSpan.FromMilliseconds(ElapsedMilliseconds); }
        }

        /// <summary>
        /// Gets the Elapsed time as the total number of milliseconds.
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return GetCurrentElapsedTicks() / TimeSpan.TicksPerMillisecond; }
        }

        /// <summary>
        /// Gets the Elapsed time as the total number of ticks (which is faked
        /// as Silverlight doesn't have a way to get at the actual "Ticks")
        /// </summary>
        public long ElapsedTicks
        {
            get { return GetCurrentElapsedTicks(); }
        }

        private long GetCurrentElapsedTicks()
        {
            return (long)(this._elapsed + (IsRunning ? (GetCurrentTicks() - _startTick) : 0));
        }

        private long GetCurrentTicks()
        {
            // TickCount: Gets the number of milliseconds elapsed since the system started.
            return Environment.TickCount * TimeSpan.TicksPerMillisecond;
        }

    }
}
