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
using ConsoleExamples.Examples;

namespace Encog.Examples.Market
{
    public class Market:IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(Market),
                    "market",
                    "Stock Market Prediction Attempt",
                    "Simple termporal neural network that attempts to predict the direction of a security.");
                return info;
            }
        }

        public static IExampleInterface app;

        public void Execute(IExampleInterface app)
        {
            Market.app = app;
            if (app.Args.Length == 0)
            {
                Market.app.WriteLine("Please call with: build, train or predict.");
            }
            else if (String.Compare(app.Args[0], "build", true)==0)
            {
                MarketBuildTraining m = new MarketBuildTraining();
                m.Run();
            }
            else if (String.Compare(app.Args[0], "train", true) == 0)
            {
                MarketTrain m = new MarketTrain();
                m.Run();
            }
            else if (String.Compare(app.Args[0], "predict", true) == 0)
            {
                MarketPredict m = new MarketPredict();
                m.Run();
            }
            else if (String.Compare(app.Args[0], "prune", true) == 0)
            {
                MarketPrune m = new MarketPrune();
                m.Run();
            }
            else
            {
                Console.WriteLine("Must be generate, train, predict, or prune.");
            }
        }
    }
}
