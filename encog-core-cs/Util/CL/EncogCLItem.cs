using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CL
{
    /// <summary>
    /// Common data held by OpenCL devices and platforms.
    /// </summary>
    public class EncogCLItem
    {
        /// <summary>
        /// Is this device or platform enabled.  Disabling a platform 
        /// will cause its devices to not be used either, regardless of 
        /// their enabled/disabled status.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The name of this device or platform.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The vendor of this device or platform.
        /// </summary>
        public String Vender { get; set; }
    }
}
