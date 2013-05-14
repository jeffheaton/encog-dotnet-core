using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.App.Generate.Generators
{
    /// <summary>
    /// This interface defines a language specific generator. Implementors of this
    /// interface generate code for languages such as Java or C#.
    /// </summary>
    public interface
        ILanguageSpecificGenerator
    {
        /// <summary>
        /// The generated code.
        /// </summary>
        String Contents { get; }

        /// <summary>
        /// Write the generated code to a file. 
        /// </summary>
        /// <param name="targetFile">The target file.</param>
        void WriteContents(FileInfo targetFile);
    }
}
