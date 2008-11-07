using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Time
{
    interface ITimeUnitNames
    {
        String Singular(TimeUnit unit);
        String Plural(TimeUnit unit);
        String Code(TimeUnit unit);
    }
}
