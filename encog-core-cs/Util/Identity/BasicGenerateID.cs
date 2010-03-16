using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Identity
{
    public class BasicGenerateID : IGenerateID
    {
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
