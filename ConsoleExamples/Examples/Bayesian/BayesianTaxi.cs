using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Query.Enumeration;
using ConsoleExamples.Examples;
using Encog.ML.Bayesian.Query.Sample;

namespace Encog.Examples.Bayesian
{
    public class BayesianTaxi : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(BayesianTaxi),
                    "bayesian-taxi",
                    "The taxicab problem with Bayesian networks.",
                    "Perform a query of a bayesian network to answer the taxicab problem.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // build the bayesian network structure
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent BlueTaxi = network.CreateEvent("blue_taxi");
            BayesianEvent WitnessSawBlue = network.CreateEvent("saw_blue");
            network.CreateDependency(BlueTaxi, WitnessSawBlue);
            network.FinalizeStructure();
            // build the truth tales
            BlueTaxi.Table.AddLine(0.85, true);
            WitnessSawBlue.Table.AddLine(0.80, true, true);
            WitnessSawBlue.Table.AddLine(0.20, true, false);

            // validate the network
            network.Validate();
            // display basic stats
            Console.WriteLine(network.ToString());
            Console.WriteLine("Parameter count: " + network.CalculateParameterCount());
            EnumerationQuery query = new EnumerationQuery(network);
            //SamplingQuery query = new SamplingQuery(network);
            query.DefineEventType(WitnessSawBlue, EventType.Evidence);
            query.DefineEventType(BlueTaxi, EventType.Outcome);
            query.SetEventValue(WitnessSawBlue, false);
            query.SetEventValue(BlueTaxi, false);
            query.Execute();
            Console.WriteLine(query.ToString());
        }
    }
}
