using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Persist
{
    public interface IEncogPersistedObject
    {
        String Description
        {
            get;
            set;
        }

        String Name
        {
            get;
            set;
        }
    }
}
