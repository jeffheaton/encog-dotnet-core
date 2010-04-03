using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Util.Identity
{
    public class BasicGenerateID : IGenerateID
    {
        [@EGAttribute]
        private long currentID;

        public BasicGenerateID()
        {
            this.currentID = 1;
        }

        public long Generate()
        {
            lock (this)
            {
                return this.currentID++;
            }
        }
    }
}
