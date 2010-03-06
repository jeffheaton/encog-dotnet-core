using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace Encog.Util.CL
{
    public class EncogCL
    {
        private ComputeContextPropertyList cp;

        public void Init()
        {
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);
        }
    }
}
