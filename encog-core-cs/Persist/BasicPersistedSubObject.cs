using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Persist
{
    public abstract class BasicPersistedSubObject : IEncogPersistedObject
    {
        public string Description
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        public string Name
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        public IEncogCollection Collection
        {
            get
            {
                return null;
            }
            set
            {
                // ignore
            }
        }

        /// <summary>
        /// Subclasses should override this and provide a persistor.
        /// </summary>
        /// <returns></returns>
        public abstract IPersistor CreatePersistor();

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
