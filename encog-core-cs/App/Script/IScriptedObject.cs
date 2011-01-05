using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;

namespace Encog.App.Script
{
    public interface IScriptedObject
    {
        void PerformFinalize(EncogQuantScript script);
        bool IsFinal();
        IEncogPersistedObject EncogObject { get; }
    }
}
