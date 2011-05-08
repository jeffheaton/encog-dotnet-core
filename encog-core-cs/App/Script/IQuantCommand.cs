using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Script
{
    public interface IQuantCommand
    {
        String CommandName { get; }
        void Execute(EncogQuantScript script, ParseLine line);
    }
}
