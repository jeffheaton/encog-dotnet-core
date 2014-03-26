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
using Encog.Cloud.Indicator.Basic;
using Encog.Cloud.Indicator.Server;
using Encog.Util.CSV;

namespace Encog.Examples.Indicator.CustomInd
{
    public class EMA : BasicIndicator
    {
        public EMA()
            : base(true)
        {
            RequestData("CLOSE");
            RequestData("THIS[2]");
        }

        public override void NotifyPacket(IndicatorPacket packet)
        {
            if (string.Compare(packet.Command, IndicatorLink.PacketBar, true) == 0)
            {                
                double dataClose = CSVFormat.EgFormat.Parse(packet.Args[2]);
                double lastValue = CSVFormat.EgFormat.Parse(packet.Args[4]);
                const double period = 14;

                double result;

                if (double.IsNaN(lastValue))
                    result = dataClose;
                else
                    result = dataClose*(2.0/(1 + period)) + (1 - (2.0/(1 + period)))*lastValue;

                String[] args = {
                                    CSVFormat.EgFormat.Format(result, EncogFramework.DefaultPrecision),
                                    "?",
                                    "?",
                                    "?",
                                    "?",
                                    "?",
                                    "?",
                                    "?"
                                };

                Link.WritePacket(IndicatorLink.PacketInd, args);
            }
        }

        public override void NotifyTermination()
        {
            // don't really care		
        }
    }
}
