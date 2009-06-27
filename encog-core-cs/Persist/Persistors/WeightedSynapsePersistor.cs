// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Neural.Networks.Synapse;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ActivationBiPolar class.
    /// </summary>
    public class WeightedSynapsePersistor : IPersistor
    {

        /// <summary>
        /// The weights tag.
        /// </summary>
        public const String TAG_WEIGHTS = "weights";


        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            WeightedSynapse synapse = new WeightedSynapse();

            String end = xmlIn.LastTag.Name;

            while (xmlIn.ReadToTag())
            {

                if (xmlIn.IsIt(WeightedSynapsePersistor.TAG_WEIGHTS, true))
                {
                    xmlIn.ReadToTag();
                    synapse.WeightMatrix = PersistorUtil.loadMatrix(xmlIn);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            return synapse;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil
                    .BeginEncogObject(
                            EncogPersistedCollection.TYPE_WEIGHTED_SYNAPSE, xmlOut,
                            obj, false);
            WeightedSynapse synapse = (WeightedSynapse)obj;

            xmlOut.BeginTag(WeightedSynapsePersistor.TAG_WEIGHTS);
            PersistorUtil.saveMatrix(synapse.WeightMatrix, xmlOut);
            xmlOut.EndTag();

            xmlOut.EndTag();
        }

    }

}
