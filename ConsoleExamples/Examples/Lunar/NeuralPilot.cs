// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Data;
using Encog.App.Quant.Normalize;
using Encog.Neural.Data.Basic;

namespace Encog.Examples.Lunar
{
    public class NeuralPilot
    {
        private BasicNetwork network;       
        private bool track;
        private IExampleInterface app;
        private NormalizedFieldStats fuelStats;
        private NormalizedFieldStats altitudeStats;
        private NormalizedFieldStats velocityStats;

        public NeuralPilot(IExampleInterface app, BasicNetwork network, bool track)
        {
            fuelStats = new NormalizedFieldStats(NormalizationDesired.Normalize, "fuel", 200, 0, -0.9, 0.9);
            altitudeStats = new NormalizedFieldStats(NormalizationDesired.Normalize, "altitude", 10000, 0, -0.9, 0.9);
            velocityStats = new NormalizedFieldStats(NormalizationDesired.Normalize, "velocity", LanderSimulator.TERMINAL_VELOCITY, -LanderSimulator.TERMINAL_VELOCITY, -0.9, 0.9);

            this.track = track;
            this.network = network;
            this.app = app;
        }

        public int ScorePilot()
        {
            LanderSimulator sim = new LanderSimulator();
            while (sim.Flying)
            {                
                INeuralData input = new BasicNeuralData(3);
                input[0] = this.fuelStats.Normalize(sim.Fuel);
                input[1] = this.fuelStats.Normalize(sim.Altitude);
                input[2] = this.fuelStats.Normalize(sim.Velocity);
                INeuralData output = this.network.Compute(input);
                double value = output.Data[0];

                bool thrust;

                if (value > 0)
                {
                    thrust = true;
                    if (track)
                        app.WriteLine("THRUST");
                }
                else
                    thrust = false;

                sim.Turn(thrust);
                if (track)
                    app.WriteLine(sim.Telemetry());
            }
            return (sim.Score());
        }
    }
}
