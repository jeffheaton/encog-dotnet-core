//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util.Arrayutil;

namespace Encog.Examples.Lunar
{
    public class NeuralPilot
    {
        private readonly NormalizedField _fuelStats;
        private readonly BasicNetwork _network;
        private readonly bool _track;
        private readonly NormalizedField _altitudeStats;
        private readonly NormalizedField _velocityStats;

        public NeuralPilot(BasicNetwork network, bool track)
        {
            _fuelStats = new NormalizedField(NormalizationAction.Normalize, "fuel", 200, 0, -0.9, 0.9);
            _altitudeStats = new NormalizedField(NormalizationAction.Normalize, "altitude", 10000, 0, -0.9, 0.9);
            _velocityStats = new NormalizedField(NormalizationAction.Normalize, "velocity",
                                                LanderSimulator.TerminalVelocity, -LanderSimulator.TerminalVelocity,
                                                -0.9, 0.9);

            _track = track;
            _network = network;
        }

        public int ScorePilot()
        {
            var sim = new LanderSimulator();
            while (sim.Flying)
            {
                var input = new BasicMLData(3);
                input[0] = _fuelStats.Normalize(sim.Fuel);
                input[1] = _altitudeStats.Normalize(sim.Altitude);
                input[2] = _velocityStats.Normalize(sim.Velocity);
                IMLData output = _network.Compute(input);
                double value = output[0];

                bool thrust;

                if (value > 0)
                {
                    thrust = true;
                    if (_track)
                        Console.WriteLine(@"THRUST");
                }
                else
                    thrust = false;

                sim.Turn(thrust);
                if (_track)
                    Console.WriteLine(sim.Telemetry());
            }
            return (sim.Score);
        }
    }
}
