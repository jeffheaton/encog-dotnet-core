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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using System.IO;
using Encog.MathUtil.Matrices;
using Encog.ML.HMM.Distributions;
using Encog.ML.HMM;
using Encog.Util;

namespace Encog.ML.HMM
{
    /// <summary>
    /// Persist a HMM.
    /// </summary>
    public class PersistHMM : IEncogPersistor
    {
        /// <inheritdoc/>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return "HiddenMarkovModel";
            }
        }

        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            int states = 0;
            int[] items;
            double[] pi = null;
            Matrix transitionProbability = null;
            IDictionary<String, String> properties = null;
            IList<IStateDistribution> distributions = new List<IStateDistribution>();

            EncogReadHelper reader = new EncogReadHelper(istream);
            EncogFileSection section;

            while ((section = reader.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("HMM")
                        && section.SubSectionName.Equals("PARAMS"))
                {
                    properties = section.ParseParams();

                }
                if (section.SectionName.Equals("HMM")
                        && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    states = EncogFileSection.ParseInt(p, HiddenMarkovModel.TAG_STATES);

                    if (p.ContainsKey(HiddenMarkovModel.TAG_ITEMS))
                    {
                        items = EncogFileSection.ParseIntArray(p, HiddenMarkovModel.TAG_ITEMS);
                    }
                    pi = section.ParseDoubleArray(p, HiddenMarkovModel.TAG_PI);
                    transitionProbability = EncogFileSection.ParseMatrix(p, HiddenMarkovModel.TAG_TRANSITION);
                }
                else if (section.SectionName.Equals("HMM")
                      && section.SubSectionName.StartsWith("DISTRIBUTION-"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    String t = p[HiddenMarkovModel.TAG_DIST_TYPE];
                    if ("ContinousDistribution".Equals(t))
                    {
                        double[] mean = section.ParseDoubleArray(p, HiddenMarkovModel.TAG_MEAN);
                        Matrix cova = EncogFileSection.ParseMatrix(p, HiddenMarkovModel.TAG_COVARIANCE);
                        ContinousDistribution dist = new ContinousDistribution(mean, cova.Data);
                        distributions.Add(dist);
                    }
                    else if ("DiscreteDistribution".Equals(t))
                    {
                        Matrix prob = EncogFileSection.ParseMatrix(p, HiddenMarkovModel.TAG_PROBABILITIES);
                        DiscreteDistribution dist = new DiscreteDistribution(prob.Data);
                        distributions.Add(dist);
                    }
                }
            }

            HiddenMarkovModel result = new HiddenMarkovModel(states);
            EngineArray.PutAll(properties, result.Properties);
            result.TransitionProbability = transitionProbability.Data;
            result.Pi = pi;
            int index = 0;
            foreach (IStateDistribution dist in distributions)
            {
                result.StateDistributions[index++] = dist;
            }


            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            EncogWriteHelper writer = new EncogWriteHelper(os);
            HiddenMarkovModel net = (HiddenMarkovModel)obj;

            writer.AddSection("HMM");
            writer.AddSubSection("PARAMS");
            writer.AddProperties(net.Properties);
            writer.AddSubSection("CONFIG");

            writer.WriteProperty(HiddenMarkovModel.TAG_STATES, net.StateCount);
            if (net.Items != null)
            {
                writer.WriteProperty(HiddenMarkovModel.TAG_ITEMS, net.Items);
            }
            writer.WriteProperty(HiddenMarkovModel.TAG_PI, net.Pi);
            writer.WriteProperty(HiddenMarkovModel.TAG_TRANSITION, new Matrix(net.TransitionProbability));

            for (int i = 0; i < net.StateCount; i++)
            {
                writer.AddSubSection("DISTRIBUTION-" + i);
                IStateDistribution sd = net.StateDistributions[i];
                writer.WriteProperty(HiddenMarkovModel.TAG_DIST_TYPE, sd.GetType().Name);

                if (sd is ContinousDistribution)
                {
                    ContinousDistribution cDist = (ContinousDistribution)sd;
                    writer.WriteProperty(HiddenMarkovModel.TAG_MEAN, cDist.Mean);
                    writer.WriteProperty(HiddenMarkovModel.TAG_COVARIANCE, cDist.Covariance);

                }
                else if (sd is DiscreteDistribution)
                {
                    DiscreteDistribution dDist = (DiscreteDistribution)sd;
                    writer.WriteProperty(HiddenMarkovModel.TAG_PROBABILITIES, new Matrix(dDist.Probabilities));
                }
            }

            writer.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(HiddenMarkovModel); }
        }
    }
}
