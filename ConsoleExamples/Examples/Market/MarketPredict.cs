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
using Encog.Util.Logging;
using System.IO;
using Encog.Persist;
using Encog.Neural.Networks;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.App.Quant.Loader.YahooFinance;
using Encog.Util.CSV;
using Encog.App.Quant.Sort;
using Encog.App.Quant.Indicators;

namespace Encog.Examples.Market
{
    public class MarketPredict
    {
        public enum Direction
        {
            up,
            down
        };

        public Direction determineDirection(double d)
        {
            if (d < 0)
                return Direction.down;
            else
                return Direction.up;
        }



        public void Run()
        {            
            DateTime end = DateTime.Now;
            DateTime begin = end.AddYears(-1);
            Console.WriteLine("Downloading data to predict from");
            YahooDownload loader = new YahooDownload();
            loader.LoadAllData(Config.TICKER, Config.FILENAME_PREDICT, CSVFormat.ENGLISH, begin, end);
            
            Console.WriteLine("Sorting downloaded data");
            SortCSV sort = new SortCSV();
            sort.SortOrder.Add(new SortedField(0, SortType.SortInteger, true));
            sort.Process(Config.FILENAME_PREDICT, Config.FILENAME_PREDICT, true, CSVFormat.ENGLISH);

            Console.WriteLine("Processing prediction data");
            ReadCSV csv = new ReadCSV(Config.FILENAME_PREDICT, true, CSVFormat.ENGLISH);
            while (csv.Next())
            {
                double priceClose = csv.GetDouble("adjusted price");
            }
        }
    }
}
