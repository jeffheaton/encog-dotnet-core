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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Logic;
using System.Reflection;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the BasicNetwork class.
    /// </summary>
    public class BasicNetworkPersistor : IPersistor
    {

        /// <summary>
        /// The layers tag.
        /// </summary>
        public const String TAG_LAYERS = "layers";

        /// <summary>
        /// The synapses tag.
        /// </summary>
        public const String TAG_SYNAPSES = "synapses";

        /// <summary>
        /// The synapse tag.
        /// </summary>
        public const String TAG_SYNAPSE = "synapse";

        /// <summary>
        /// The properties tag.
        /// </summary>
        public const String TAG_PROPERTIES = "properties";

        /// <summary>
        /// Logic tag.
        /// </summary>
        public const String TAG_LOGIC = "logic";

        /// <summary>
        /// The layer synapse.
        /// </summary>
        public const String TAG_LAYER = "layer";

        /// <summary>
        /// Property tag.
        /// </summary>
        public const String TAG_PROPERTY = "Property";

        /// <summary>
        /// The id attribute.
        /// </summary>
        public const String ATTRIBUTE_ID = "id";

        /// <summary>
        /// Name attribute.
        /// </summary>
        public const String ATTRIBUTE_NAME = "name";

        /// <summary>
        /// Value tag.
        /// </summary>
        public const String ATTRIBUTE_VALUE = "value";

        /// <summary>
        /// The type attribute.
        /// </summary>
        public const String ATTRIBUTE_TYPE = "type";

        /// <summary>
        /// The input layer type.
        /// </summary>
        public const String ATTRIBUTE_TYPE_INPUT = "input";

        /// <summary>
        /// The output layer type.
        /// </summary>
        public const String ATTRIBUTE_TYPE_OUTPUT = "output";

        /// <summary>
        /// The hidden layer type.
        /// </summary>
        public const String ATTRIBUTE_TYPE_HIDDEN = "hidden";

        /// <summary>
        /// The both layer type.
        /// </summary>
        public const String ATTRIBUTE_TYPE_BOTH = "both";

        /// <summary>
        /// The unknown layer type.
        /// </summary>
        public const String ATTRIBUTE_TYPE_UNKNOWN = "unknown";

        /// <summary>
        /// The from attribute.
        /// </summary>
        public const String ATTRIBUTE_FROM = "from";

        /// <summary>
        /// The to attribute.
        /// </summary>
        public const String ATTRIBUTE_TO = "to";

        /// <summary>
        /// The network that is being loaded.
        /// </summary>
        private BasicNetwork currentNetwork;

        /// <summary>
        /// A mapping from layers to index numbers.
        /// </summary>
        private IDictionary<ILayer, int> layer2index = new Dictionary<ILayer, int>();

        /// <summary>
        /// A mapping from index numbers to layers.
        /// </summary>
        private IDictionary<int, ILayer> index2layer = new Dictionary<int, ILayer>();

        /// <summary>
        /// Handle any layers that should be loaded.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        private void HandleLayers(ReadXML xmlIn)
        {
            String end = xmlIn.LastTag.Name;
            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicNetworkPersistor.TAG_LAYER, true))
                {
                    int num = xmlIn.LastTag.GetAttributeInt(
                           BasicNetworkPersistor.ATTRIBUTE_ID);
                    String type = xmlIn.LastTag.GetAttributeValue(
                           BasicNetworkPersistor.ATTRIBUTE_TYPE);
                    xmlIn.ReadToTag();
                    IPersistor persistor = PersistorUtil.CreatePersistor(xmlIn
                           .LastTag.Name);
                    ILayer layer = (ILayer)persistor.Load(xmlIn);
                    this.index2layer[num] = layer;

                    // the type attribute is actually "legacy", but if its there
                    // then use it!
                    if (type != null)
                    {
                        if (type.Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_INPUT))
                        {
                            this.currentNetwork.TagLayer(BasicNetwork.TAG_INPUT,
                                    layer);
                        }
                        else if (type
                                .Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_OUTPUT))
                        {
                            this.currentNetwork.TagLayer(BasicNetwork.TAG_OUTPUT,
                                    layer);
                        }
                        else if (type
                              .Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_BOTH))
                        {
                            this.currentNetwork.TagLayer(BasicNetwork.TAG_INPUT,
                                    layer);
                            this.currentNetwork.TagLayer(BasicNetwork.TAG_OUTPUT,
                                    layer);
                        }
                    }
                    // end of legacy processing
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Process any synapses that should be loaded.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        private void HandleSynapses(ReadXML xmlIn)
        {
            String end = xmlIn.LastTag.Name;
            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicNetworkPersistor.TAG_SYNAPSE, true))
                {
                    int from = xmlIn.LastTag.GetAttributeInt(
                           BasicNetworkPersistor.ATTRIBUTE_FROM);
                    int to = xmlIn.LastTag.GetAttributeInt(
                           BasicNetworkPersistor.ATTRIBUTE_TO);
                    xmlIn.ReadToTag();
                    IPersistor persistor = PersistorUtil.CreatePersistor(xmlIn
                           .LastTag.Name);
                    ISynapse synapse = (ISynapse)persistor.Load(xmlIn);
                    synapse.FromLayer = this.index2layer[from];
                    synapse.ToLayer = this.index2layer[to];
                    synapse.FromLayer.AddSynapse(synapse);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {

            String name = xmlIn.LastTag.Attributes[EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlIn.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_DESCRIPTION];

            this.currentNetwork = new BasicNetwork();
            this.currentNetwork.Name = name;
            this.currentNetwork.Description = description;

            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicNetworkPersistor.TAG_LAYERS, true))
                {
                    HandleLayers(xmlIn);
                }
                else if (xmlIn.IsIt(BasicNetworkPersistor.TAG_SYNAPSES, true))
                {
                    HandleSynapses(xmlIn);
                }
                else if (xmlIn.IsIt(BasicNetworkPersistor.TAG_PROPERTIES, true))
                {
                    HandleProperties(xmlIn);
                }
                else if (xmlIn.IsIt(BasicNetworkPersistor.TAG_LOGIC, true))
                {
                    HandleLogic(xmlIn);
                }

            }
            this.currentNetwork.Structure.FinalizeStructure();
            return this.currentNetwork;
        }

        /// <summary>
        /// Load the neural logic object.
        /// </summary>
        /// <param name="xmlIn">The XML object.</param>
        private void HandleLogic(ReadXML xmlIn)
        {
            String value = xmlIn.ReadTextToTag();
            if (value.Equals("ART1Logic"))
            {
                this.currentNetwork.Logic = new ART1Logic();
            }
            else if (value.Equals("BAMLogic"))
            {
                this.currentNetwork.Logic = new BAMLogic();
            }
            else if (value.Equals("BoltzmannLogic"))
            {
                this.currentNetwork.Logic = new BoltzmannLogic();
            }
            else if (value.Equals("FeedforwardLogic"))
            {
                this.currentNetwork.Logic = new FeedforwardLogic();
            }
            else if (value.Equals("HopfieldLogic"))
            {
                this.currentNetwork.Logic = new HopfieldLogic();
            }
            else if (value.Equals("SimpleRecurrentLogic"))
            {
                this.currentNetwork.Logic = new SimpleRecurrentLogic();
            }
            else
            {
                INeuralLogic logic = (INeuralLogic)Assembly.GetExecutingAssembly().CreateInstance(value);
                this.currentNetwork.Logic = logic;
            }
        }

        /// <summary>
        /// Load the properties.
        /// </summary>
        /// <param name="xmlIn">The XML object.</param>
        private void HandleProperties(ReadXML xmlIn)
        {
            String end = xmlIn.LastTag.Name;
            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicNetworkPersistor.TAG_PROPERTY, true))
                {
                    String name = xmlIn.LastTag.GetAttributeValue(
                            BasicNetworkPersistor.ATTRIBUTE_NAME);

                    String value = xmlIn.ReadTextToTag();
                    this.currentNetwork.SetProperty(name, value);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_BASIC_NET,
                    xmlOut, obj, true);
            this.currentNetwork = (BasicNetwork)obj;

            this.currentNetwork.Structure.FinalizeStructure();

            // save the layers
            xmlOut.BeginTag(BasicNetworkPersistor.TAG_LAYERS);
            SaveLayers(xmlOut);
            xmlOut.EndTag();

            // save the structure of these layers
            xmlOut.BeginTag(BasicNetworkPersistor.TAG_SYNAPSES);
            SaveSynapses(xmlOut);
            xmlOut.EndTag();

            SaveProperties(xmlOut);
            SaveLogic(xmlOut);

            xmlOut.EndTag();
        }

        /// <summary>
        /// Save the neural logic object.
        /// </summary>
        /// <param name="xmlOut">The XML object.</param>
        private void SaveLogic(WriteXML xmlOut)
        {
            xmlOut.BeginTag(BasicNetworkPersistor.TAG_LOGIC);
            INeuralLogic logic = this.currentNetwork.Logic;
            if (logic is FeedforwardLogic
                    || logic is SimpleRecurrentLogic
                    || logic is BoltzmannLogic
                    || logic is ART1Logic || logic is BAMLogic
                    || logic is HopfieldLogic)
            {
                xmlOut.AddText(logic.GetType().Name);
            }
            else
                xmlOut.AddText(logic.GetType().Name);
            xmlOut.EndTag();
        }

        /// <summary>
        /// Save the neural properties.
        /// </summary>
        /// <param name="xmlOut">The xml object.</param>
        private void SaveProperties(WriteXML xmlOut)
        {
            // save any properties
            xmlOut.BeginTag(BasicNetworkPersistor.TAG_PROPERTIES);
            foreach (String key in this.currentNetwork.Properties.Keys)
            {
                String value = this.currentNetwork.Properties[key];
                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_NAME, key);
                xmlOut.BeginTag(BasicNetworkPersistor.TAG_PROPERTY);
                xmlOut.AddText(value.ToString());
                xmlOut.EndTag();
            }
            xmlOut.EndTag();
        }

        /// <summary>
        /// Save the layers to the specified XML writer.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        private void SaveLayers(WriteXML xmlOut)
        {
            int current = 1;
            foreach (ILayer layer in this.currentNetwork.Structure.Layers)
            {

                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_ID, "" + current);
                xmlOut.BeginTag(BasicNetworkPersistor.TAG_LAYER);
                IPersistor persistor = layer.CreatePersistor();
                persistor.Save(layer, xmlOut);
                xmlOut.EndTag();
                this.layer2index[layer] = current;
                current++;
            }
        }

        /// <summary>
        /// Save the synapses to the specified XML writer.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        private void SaveSynapses(WriteXML xmlOut)
        {
            foreach (ISynapse synapse in this.currentNetwork.Structure.Synapses)
            {
                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_FROM, ""
                        + this.layer2index[synapse.FromLayer]);
                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_TO, ""
                        + this.layer2index[synapse.ToLayer]);
                xmlOut.BeginTag(BasicNetworkPersistor.TAG_SYNAPSE);
                IPersistor persistor = synapse.CreatePersistor();
                persistor.Save(synapse, xmlOut);
                xmlOut.EndTag();
            }
        }
    }
}
