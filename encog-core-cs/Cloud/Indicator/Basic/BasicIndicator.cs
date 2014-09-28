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
using System.Collections.Generic;
using Encog.Cloud.Indicator.Server;

namespace Encog.Cloud.Indicator.Basic
{
    /// <summary>
    /// This abstract class provides low-level functionality to all Encog based
    /// indicators.
    /// </summary>
    public abstract class BasicIndicator : IIndicatorListener
    {
        /// <summary>
        /// Is this indicator blocking, should it wait for a result after each bar.
        /// </summary>
        private readonly bool _blocking;

        /// <summary>
        /// The number of bars requested per data item.
        /// </summary>
        private readonly IList<int> _dataCount = new List<int>();

        /// <summary>
        /// The data that has been requested from the remote side.  This is
        /// typically HLOC(High, Low, Open, Close) data that is needed by the
        /// Encog indicator to compute.
        /// </summary>
        private readonly IList<String> _dataRequested = new List<string>();

        /// <summary>
        /// The communication link between the indicator and remote.
        /// </summary>
        private IndicatorLink _link;

        /// <summary>
        /// Construc the basic indicator.
        /// </summary>
        /// <param name="theBlocking">Are we blocking?</param>
        protected BasicIndicator(bool theBlocking)
        {
            _blocking = theBlocking;
        }

        /// <summary>
        /// The current error message;
        /// </summary>
        public String ErrorMessage { get; set; }

        /// <summary>
        /// The data that has been requested from the remote side.  This is
        /// typically HLOC(High, Low, Open, Close) data that is needed by the
        /// Encog indicator to compute.
        /// </summary>
        public IList<String> DataRequested
        {
            get { return _dataRequested; }
        }

        /// <summary>
        /// Are we blocking?
        /// </summary>
        public bool Blocking
        {
            get { return _blocking; }
        }

        /// <summary>
        /// The link.
        /// </summary>
        public IndicatorLink Link
        {
            get { return _link; }
        }

        /// <summary>
        /// The count.
        /// </summary>
        public IList<int> DataCount
        {
            get { return _dataCount; }
        }

        #region IIndicatorListener Members

        /// <summary>
        /// Notify that this indicator is now connected.  This is called
        /// once, at the beginning of a connection.  Indicators are not
        /// reused.  
        /// </summary>
        /// <param name="theLink">The link.</param>
        public void NotifyConnect(IndicatorLink theLink)
        {
            _link = theLink;
            if (ErrorMessage != null)
            {
                String[] args = {ErrorMessage};
                Link.WritePacket(IndicatorLink.PacketError, args);
            }
            else
            {
                _link.InitConnection(_dataRequested, _blocking);
            }
        }


        public abstract void NotifyPacket(IndicatorPacket packet);

        public abstract void NotifyTermination();

        #endregion

        /// <summary>
        /// Request the specified data. i.e. HIGH, LOW, etc. 
        /// </summary>
        /// <param name="str">The data being requested.</param>
        public void RequestData(String str)
        {
            _dataRequested.Add(str);

            int idx = str.IndexOf('[');

            if (idx == -1)
            {
                _dataCount.Add(1);
                return;
            }

            int idx2 = str.IndexOf(']', idx);

            if (idx2 == -1)
            {
                _dataCount.Add(1);
                return;
            }

            String s = str.Substring(idx + 1, idx2 - (idx + 1));
            _dataCount.Add(int.Parse(s));
        }
    }
}
