using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst;

namespace Encog.App.Generate.Generators
{
    /// <summary>
    /// This interface defines a generator that works by template. A template is a
    /// file that allows parts if it to be replaced by tokens. Encog uses this sort
    /// of generator for NinjaScript and MQL4.
    /// </summary>
    public interface ITemplateGenerator: ILanguageSpecificGenerator
    {
        /// <summary>
        /// Generate the template based on Encog Analyst script.
        /// </summary>
        /// <param name="analyst">The encog analyst.</param>
        void Generate(EncogAnalyst analyst);
    }
}
