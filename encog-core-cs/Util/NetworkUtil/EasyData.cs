//
// Encog(tm) Core v3.3 - .Net Version
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
namespace Encog.Util.NetworkUtil
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Encog.ML.Data;
    using Encog.ML.Data.Basic;
    using Encog.Util.Arrayutil;

    /// <summary>
    /// Loads data into an imldata set
    /// Basically makes it easy to load live data in an imldataset.
    /// </summary>
    public class EasyData
    {
        /// <summary>
        /// Send all the (unNormalized)inputs in the as the network was trained and this outputs a list of double ready for a network.compute(imldata result)).
        /// </summary>
        /// <param name="WindoSize"> Size of the windo. </param>
        /// <param name="pparamInputs"> A variable-length parameters list containing pparam inputs. </param>
        /// <returns>
        ///  The compute pair ready for network computes)
        /// </returns>
        public static Tuple<List<double>,NormalizeArray> GetReadiedComputePair(int WindoSize, params double[][] pparamInputs)
        {
            try
            {
                //We make a dic with the count of inputs being the number of double series we are sending in.
                Dictionary<int, double[]> inputsDics = new Dictionary<int, double[]>(pparamInputs.Length);

                int indexS = 0;
                NormalizeArray Normee = new NormalizeArray(-1, 1);
                // PredictionStats.NormalizationClass NormingClass = new PredictionStats.NormalizationClass();
                foreach (double[] doubleSeries in pparamInputs)
                {
                    inputsDics.Add(indexS++, Normee.Process(doubleSeries));
                }
                List<double> dtda = new List<double>();
                int listindex = 0;
                int currentindex = 0;
                //count the fields -1 ,as it starts from zero.
                int dicinputsCount = inputsDics.Keys.Count - 1;
                foreach (double d in inputsDics[0])
                {
                    if (currentindex++ < WindoSize)
                    {
                        dtda.Add(d);
                        //we put all the fields which are in the dic.
                        while (dicinputsCount > 0)
                        {
                            dtda.Add(inputsDics[dicinputsCount--][listindex]);
                        }
                        //We reset the field count for a later pass.
                        dicinputsCount = inputsDics.Keys.Count - 1;
                    }
                    if (currentindex == WindoSize)
                    {
                        return new Tuple<List<double>, NormalizeArray>(dtda, Normee);
                    }
                    //Lets increment the indexes..
                    listindex++;
                }
                return new Tuple<List<double>, NormalizeArray>(dtda, Normee);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// Loads variables inputs and one ideal double series into an imldataset.
        /// </summary>
        /// <param name="idealsinputs"></param>
        /// <param name="WindoSize"></param>
        /// <param name="pparamInputs"></param>
        /// <returns></returns>
        public static Tuple<IMLDataSet,NormalizeArray> Load(double[] idealsinputs, int WindoSize, params double[][] pparamInputs)
        {
            try
            {
                var finalSet = new BasicMLDataSet();
                //We make a dic with the count of inputs being the number of double series we are sending in.
                Dictionary<int, double[]> inputsDics = new Dictionary<int, double[]>(pparamInputs.Length);
                int indexS = 0;
                //We make a normalizeArray which we will return as a tuple ready for denormalization.
                NormalizeArray Normer = new NormalizeArray(-1, 1);
                //Process each inputs.
                foreach (double[] doubleSeries in pparamInputs)
                {
                   inputsDics.Add(indexS++, Normer.Process(doubleSeries));
                }
             //Process the ideals.
                var idealNormed = Normer.Process(idealsinputs);
              
                //Make a list which will hold the inputs one after the others
                List<double> dtda = new List<double>();
                int listindex = 0;
                int currentindex = 0;
                //starts from zero so count -1..
                int dicinputsCount = inputsDics.Keys.Count - 1;
                //Process the input normed.
                foreach (double d in inputsDics[0])
                {
                    if (currentindex++ < WindoSize)
                    {
                        dtda.Add(d);
                        //we put all the fields which are in the dic.
                        while (dicinputsCount > 0)
                        {
                            dtda.Add(inputsDics[dicinputsCount--][listindex]);
                        }
                        //We reset the field count for a later pass.
                        dicinputsCount = inputsDics.Keys.Count - 1;
                    }

                    if (currentindex == WindoSize)
                    {
                        //Make an imldata pair, and add it to the imldataset...reset the temp list of inputs...
                        var pair = new BasicMLDataPair(
                                    new BasicMLData(dtda.ToArray()),
                                    new BasicMLData(new double[] { idealNormed[listindex] }));
                        currentindex = 0;
                        dtda.Clear();
                        finalSet.Add(pair);
                    }
                    //Lets increment the indexes..
                    listindex++;
                }
                //Return the dataset and the normalization array..
                return new Tuple<IMLDataSet, NormalizeArray>(finalSet,Normer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got an error : ", ex);
                throw new Exception("Error parsing points....");
            }
        }
    
    
    
    
    }
}
