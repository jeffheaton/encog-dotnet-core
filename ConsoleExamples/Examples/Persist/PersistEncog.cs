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
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Util.Simple;
using Encog.Neural.Networks;
using Encog.Persist;
using System.IO;
using Encog.Util;

namespace Encog.Examples.Persist
{
    public class PersistEncog : IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(PersistEncog),
                    "persist-encog",
                    "Persist using .Net Serialization",
                    "Create and persist a neural network using .Net serialization.");
                return info;
            }
        }

        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[2] { 0.0, 0.0 },
            new double[2] { 1.0, 0.0 },
			new double[2] { 0.0, 1.0 },
            new double[2] { 1.0, 1.0 } };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[1] { 0.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 0.0 } };

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT,XOR_IDEAL);
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 6, 0, 1, false);
            EncogUtility.TrainToError(network, trainingSet, 0.01);
            double error = network.CalculateError(trainingSet);
            EncogPersistedCollection encog = new EncogPersistedCollection("saved.eg",FileMode.Create);
            encog.Add("test", network);
            network = (BasicNetwork)encog.Find("test");
            double error2 = network.CalculateError(trainingSet);
            app.WriteLine("Error before save to EG: " + Format.FormatPercent(error));
            app.WriteLine("Error before after to EG: " + Format.FormatPercent(error2));
        }
    }
}
