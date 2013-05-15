using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Generate.Program;

namespace Encog.App.Generate.Generators
{
    /// <summary>
    /// This interface defines a generator that works from program blocks, rather
    /// than a template. Encog uses this generator for Java, C# and Javascript.
    /// </summary>
    public interface IProgramGenerator: ILanguageSpecificGenerator
    {
        void Generate(EncogGenProgram program, bool embed);
    }
}
