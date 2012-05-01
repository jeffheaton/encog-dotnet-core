using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly bool blocking;

        /// <summary>
        /// The data that has been requested from the remote side.  This is
        /// typically HLOC(High, Low, Open, Close) data that is needed by the
        /// Encog indicator to compute.
        /// </summary>
        private readonly IList<String> dataRequested = new List<string>();

        /// <summary>
        /// The number of bars requested per data item.
        /// </summary>
        private readonly IList<int> dataCount = new List<int>();

        /// <summary>
        /// The communication link between the indicator and remote.
        /// </summary>
        private IndicatorLink link;

        /// <summary>
        /// The current error message;
        /// </summary>
        public String ErrorMessage { get; set; }

        /// <summary>
        /// Construc the basic indicator.
        /// </summary>
        /// <param name="theBlocking">Are we blocking?</param>
        public BasicIndicator(bool theBlocking)
        {
            this.blocking = theBlocking;
        }

        /// <summary>
        /// The data that has been requested from the remote side.  This is
        /// typically HLOC(High, Low, Open, Close) data that is needed by the
        /// Encog indicator to compute.
        /// </summary>
        public IList<String> DataRequested
        {
            get
            {
                return dataRequested;
            }
        }

        /// <summary>
        /// Request the specified data. i.e. HIGH, LOW, etc. 
        /// </summary>
        /// <param name="str">The data being requested.</param>
        public void RequestData(String str)
        {
            dataRequested.Add(str);

            int idx = str.IndexOf('[');

            if (idx == -1)
            {
                this.dataCount.Add(1);
                return;
            }

            int idx2 = str.IndexOf(']', idx);

            if (idx2 == -1)
            {
                this.dataCount.Add(1);
                return;
            }

            String s = str.Substring(idx + 1, idx2 - (idx + 1));
            this.dataCount.Add(int.Parse(s));
        }

        /// <summary>
        /// Notify that this indicator is now connected.  This is called
        /// once, at the beginning of a connection.  Indicators are not
        /// reused.  
        /// </summary>
        /// <param name="theLink">The link.</param>
        public void NotifyConnect(IndicatorLink theLink)
        {
            this.link = theLink;
            if (this.ErrorMessage != null)
            {
                String[] args = { this.ErrorMessage };
                this.Link.WritePacket(IndicatorLink.PACKET_ERROR, args);
            }
            else
            {
                this.link.InitConnection(this.dataRequested, this.blocking);
            }
        }

        /// <summary>
        /// Are we blocking?
        /// </summary>
        public bool Blocking
        {
            get
            {
                return blocking;
            }
        }

        /// <summary>
        /// The link.
        /// </summary>
        public IndicatorLink Link
        {
            get
            {
                return link;
            }
        }

        /// <summary>
        /// The count.
        /// </summary>
        public IList<int> DataCount
        {
            get
            {
                return dataCount;
            }
        }


        public abstract void NotifyPacket(IndicatorPacket packet);

        public abstract void NotifyTermination();
        
    }
}
