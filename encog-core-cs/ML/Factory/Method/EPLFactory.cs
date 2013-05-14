using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Factory.Parse;
using Encog.Util;
using Encog.ML.Prg;
using Encog.ML.Prg.Train;
using Encog.ML.Prg.Generator;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.Ext;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// Create an EPL method.
    /// </summary>
    public class EPLFactory
    {
        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public IMLMethod Create(String architecture, int input,
                int output)
        {

            if (input <= 0)
            {
                throw new EncogError("Must have at least one input for EPL.");
            }

            if (output <= 0)
            {
                throw new EncogError("Must have at least one output for EPL.");
            }


            IDictionary<String, String> args = ArchitectureParse.ParseParams(architecture);
            ParamsHolder holder = new ParamsHolder(args);

            int populationSize = holder.GetInt(
                    MLMethodFactory.PropertyPopulationSize, false, 1000);
            String variables = holder.GetString("vars", false, "x");
            String funct = holder.GetString("funct", false, null);

            EncogProgramContext context = new EncogProgramContext();
            string[] tok = variables.Split(',');
            foreach (string v in tok)
            {
                context.DefineVariable(v);
            }

            if (string.Compare("numeric", funct, true) == 0)
            {
                StandardExtensions.CreateNumericOperators(context);
            }

            PrgPopulation pop = new PrgPopulation(context, populationSize);

            if (context.Functions.Count > 0)
            {
                (new RampedHalfAndHalf(context, 2, 6)).Generate(new EncogRandom(), pop);
            }
            return pop;
        }
    }
}
