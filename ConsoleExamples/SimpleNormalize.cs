using System;
using ConsoleExamples.Examples;
using Encog.Util.Arrayutil;

namespace Encog.Examples.Normalize
{
    public class SimpleNormalize : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (SimpleNormalize),
                    "normalize-simple",
                    "Simple normalize example.",
                    "A very simple normalize/denormalize example.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            // Normalize values with an actual range of (0 to 100) to (-1 to 1)
            var norm = new NormalizedField(NormalizationAction.Normalize,
                                           null, 100, 0, 1, -1);

            double x = 5;
            double y = norm.Normalize(x);

            Console.WriteLine(x + @" normalized is " + y);

            double z = norm.DeNormalize(y);

            Console.WriteLine(y + @" denormalized is " + z);
        }

        #endregion
    }
}