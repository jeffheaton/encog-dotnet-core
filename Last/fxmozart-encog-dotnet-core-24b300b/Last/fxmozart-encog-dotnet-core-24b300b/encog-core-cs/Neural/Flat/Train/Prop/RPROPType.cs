using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Allows the type of RPROP to be defined.  RPROPp is the classic RPROP.
    /// 
    /// For more information, visit:
    /// 
    /// http://www.heatonresearch.com/wiki/RPROP
    /// </summary>
    public enum RPROPType
    {
        /// <summary>
        /// RPROP+ : The classic RPROP algorithm.  Uses weight back tracking.
        /// </summary>
        RPROPp,

        /// <summary>
        /// RPROP- : No weight back tracking.
        /// </summary>
        RPROPm,

        /// <summary>
        /// iRPROP+ : New weight back tracking method, some consider this to be
        /// the most advanced RPROP.
        /// </summary>
        iRPROPp,

        /// <summary>
        /// iRPROP- : New RPROP without weight back tracking. 
        /// </summary>
        iRPROPm
    }
}
