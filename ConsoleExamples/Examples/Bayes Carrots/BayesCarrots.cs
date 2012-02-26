using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Examples.Bayes_Carrots
{
    using ConsoleExamples.Examples;
    using ML.Bayesian;
    using ML.Bayesian.Query.Enumeration;

    class BayesCarrots : IExample
    {
        #region Implementation of IExample

        public void Execute(IExampleInterface app)
        {
            DoBayesDemo();
        }


        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(BayesCarrots),
                    "BayesCarrot",
                    "Simple Bayes on carrots",
                    "Encog Bayes....");
                return info;
            }
        }

        #endregion




        private void DoBayesDemo()
        {
            // build the bayesian network structure
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent rained = network.CreateEvent("rained");
            BayesianEvent evenTemperatures = network.CreateEvent("temperature");
            BayesianEvent gardenGrew = network.CreateEvent("gardenGrew");
            BayesianEvent plentyOfCarrots = network.CreateEvent("carrots");
            BayesianEvent plentyOfTomatoes = network.CreateEvent("Tomatoes");
            network.CreateDependency(rained, gardenGrew);
            network.CreateDependency(evenTemperatures, gardenGrew);
            network.CreateDependency(gardenGrew, plentyOfCarrots);
            network.CreateDependency(gardenGrew, plentyOfTomatoes);
            network.FinalizeStructure();

            // build the truth tales
            rained.Table.AddLine(0.2, true);
            evenTemperatures.Table.AddLine(0.5, true);
            gardenGrew.Table.AddLine(0.9, true, true, true);
            gardenGrew.Table.AddLine(0.7, true, false, true);
            gardenGrew.Table.AddLine(0.5, true, true, false);
            gardenGrew.Table.AddLine(0.1, true, false, false);
            plentyOfCarrots.Table.AddLine(0.8, true, true);
            plentyOfCarrots.Table.AddLine(0.2, true, false);
            plentyOfTomatoes.Table.AddLine(0.6, true, true);
            plentyOfTomatoes.Table.AddLine(0.1, true, false);

            // validate the network
            network.Validate();

            // display basic stats
            Console.WriteLine(network.ToString());
            Console.WriteLine("Parameter count: " + network.CalculateParameterCount());

            EnumerationQuery query = new EnumerationQuery(network);
            //SamplingQuery query = new SamplingQuery(network);
            query.DefineEventType(rained, EventType.Evidence);
            query.DefineEventType(evenTemperatures, EventType.Evidence);
            query.DefineEventType(plentyOfCarrots, EventType.Outcome);
            query.SetEventValue(rained, true);
            query.SetEventValue(evenTemperatures, true);
            query.SetEventValue(plentyOfCarrots, true);
            query.Execute();

            Console.WriteLine(query.ToString());


        }

    }
}
