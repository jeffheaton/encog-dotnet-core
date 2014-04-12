using ConsoleExamples.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Examples.Playground
{
    /// <summary>
    /// Not a real example.  Just used for experimentation.
    /// </summary>
    public class PlaygroundExample: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(PlaygroundExample),
                    "playground",
                    "Not an actual example.  Do not run.",
                    "Just a playground to use when you want to create code to run in the same workspace as Encog core. (mostly used by Jeff Heaton)");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            Console.WriteLine("Hello world");
        }

        #endregion
    }
}
