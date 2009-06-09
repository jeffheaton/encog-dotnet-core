using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Persist.Persistors
{
    public class BasicNeuralDataSetPersistor : IPersistor
    {
        #region IPersistor Members

        public IEncogPersistedObject Load(global::Encog.Parse.Tags.Read.ReadXML xmlin)
        {
            throw new NotImplementedException();
        }

        public void Save(IEncogPersistedObject obj, global::Encog.Parse.Tags.Write.WriteXML xmlout)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
