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
using Encog.ML.Bayesian.Query;
using Encog.Util;
using Encog.ML.Bayesian.Query.Enumeration;
using Encog.ML.Bayesian.Query.Sample;
using Encog.ML.Bayesian.Table;

namespace Encog.ML.Bayesian
{
    /// <summary>
    /// Persist a bayes network.
    /// </summary>
    public class PersistBayes : IEncogPersistor
    {
        /// <summary>
        /// The file version.
        /// </summary>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            BayesianNetwork result = new BayesianNetwork();
            EncogReadHelper input = new EncogReadHelper(istream);
            EncogFileSection section;
            String queryType = "";
            String queryStr = "";
            String contentsStr = "";

            while ((section = input.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PARAM"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    queryType = p["queryType"];
                    queryStr = p["query"];
                    contentsStr = p["contents"];
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-TABLE"))
                {

                    result.Contents = contentsStr;

                    // first, define relationships (1st pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineRelationship(line);
                    }

                    result.FinalizeStructure();

                    // now define the probabilities (2nd pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineProbability(line);
                    }
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PROPERTIES"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
            }

            // define query, if it exists
            if (queryType.Length > 0)
            {
                IBayesianQuery query = null;
                if (queryType.Equals("EnumerationQuery"))
                {
                    query = new EnumerationQuery(result);
                }
                else
                {
                    query = new SamplingQuery(result);
                }

                if (query != null && queryStr.Length > 0)
                {
                    result.Query = query;
                    result.DefineClassificationStructure(queryStr);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            EncogWriteHelper o = new EncogWriteHelper(os);
            BayesianNetwork b = (BayesianNetwork)obj;
            o.AddSection("BAYES-NETWORK");
            o.AddSubSection("BAYES-PARAM");
            String queryType = "";
            String queryStr = b.ClassificationStructure;

            if (b.Query != null)
            {
                queryType = b.Query.GetType().Name;
            }

            o.WriteProperty("queryType", queryType);
            o.WriteProperty("query", queryStr);
            o.WriteProperty("contents", b.Contents);
            o.AddSubSection("BAYES-PROPERTIES");
            o.AddProperties(b.Properties);

            o.AddSubSection("BAYES-TABLE");
            foreach (BayesianEvent e in b.Events)
            {
                foreach (TableLine line in e.Table.Lines)
                {
                    if (line == null)
                        continue;
                    StringBuilder str = new StringBuilder();
                    str.Append("P(");

                    str.Append(BayesianEvent.FormatEventName(e, line.Result));

                    if (e.Parents.Count > 0)
                    {
                        str.Append("|");
                    }

                    int index = 0;
                    bool first = true;
                    foreach (BayesianEvent parentEvent in e.Parents)
                    {
                        if (!first)
                        {
                            str.Append(",");
                        }
                        first = false;
                        int arg = line.Arguments[index++];
                        if (parentEvent.IsBoolean)
                        {
                            if (arg == 0)
                            {
                                str.Append("+");
                            }
                            else
                            {
                                str.Append("-");
                            }
                        }
                        str.Append(parentEvent.Label);
                        if (!parentEvent.IsBoolean)
                        {
                            str.Append("=");
                            if (arg >= parentEvent.Choices.Count)
                            {
                                throw new BayesianError("Argument value " + arg + " is out of range for event " + parentEvent.ToString());
                            }
                            str.Append(parentEvent.GetChoice(arg));
                        }
                    }
                    str.Append(")=");
                    str.Append(line.Probability);
                    str.Append("\n");
                    o.Write(str.ToString());
                }
            }

            o.Flush();
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return "BayesianNetwork";
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BayesianNetwork); }
        }

    }
}
