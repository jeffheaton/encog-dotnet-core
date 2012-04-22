//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using System.Text;
using Encog.Util;

namespace Encog.Examples.Lunar
{
    public class LanderSimulator
    {
        public const double Gravity = 1.62;
        public const double Thrust = 10;
        public const double TerminalVelocity = 40;

        public LanderSimulator()
        {
            Fuel = 200;
            Seconds = 0;
            Altitude = 10000;
            Velocity = 0;
        }

        public int Fuel { get; set; }
        public int Seconds { get; set; }
        public double Altitude { get; set; }
        public double Velocity { get; set; }

        public int Score
        {
            get { return (int) ((Fuel*10) + Seconds + (Velocity*1000)); }
        }

        public void Turn(bool thrust)
        {
            Seconds++;
            Velocity -= Gravity;
            Altitude += Velocity;

            if (thrust && Fuel > 0)
            {
                Fuel--;
                Velocity += Thrust;
            }

            Velocity = Math.Max(-TerminalVelocity, Velocity);
            Velocity = Math.Min(TerminalVelocity, Velocity);

            if (Altitude < 0)
                Altitude = 0;
        }

        public String Telemetry()
        {
            var result = new StringBuilder();
            result.Append("Elapsed: ");
            result.Append(Seconds);
            result.Append(" s, Fuel: ");
            result.Append(Fuel);
            result.Append(" l, Velocity: ");
            result.Append(Format.FormatDouble(Velocity, 4));
            result.Append(" m/s, ");
            result.Append((int) Altitude);
            result.Append(" m");
            return result.ToString();
        }

        public bool Flying
        {
            get { return Altitude > 0; }
        }
    }
}
