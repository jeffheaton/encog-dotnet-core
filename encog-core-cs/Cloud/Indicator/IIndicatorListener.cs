using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud.Indicator.Server;

namespace Encog.Cloud.Indicator
{
    /// <summary>
    /// This interface defines the actual indicator.  This allows the indicator
    /// to be notified on initial connection, when a packet is received,
    /// and also connection termination.
    /// </summary>
    public interface IIndicatorListener
    {        
        /// <summary>
        /// Notify that a link has been established.  This is called once. 
        /// </summary>
        /// <param name="theLink">The link used.</param>
        void NotifyConnect(IndicatorLink theLink);
        
        /// <summary>
        /// Notify that a packet has been received. 
        /// </summary>
        /// <param name="packet">The packet.</param>
        void NotifyPacket(IndicatorPacket packet);

        /// <summary>
        /// The connection has been terminated.
        /// </summary>
        void NotifyTermination();
    }
}
