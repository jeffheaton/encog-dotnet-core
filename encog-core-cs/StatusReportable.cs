using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog
{
    public interface StatusReportable
    {
        void Report(int total, int current, String message);
    }
}
