using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks;

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
        /// The layer synapse.
        /// </summary>
        public const String TAG_LAYER = "layer";

        /// <summary>
        /// The id attribute.
        /// </summary>
        public const String ATTRIBUTE_ID = "id";

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
        private IDictionary<ILayer, int> layer2index
            = new Dictionary<ILayer, int>();

        /// <summary>
        /// A mapping from index numbers to layers.
        /// </summary>
        private IDictionary<int, ILayer> index2layer
            = new Dictionary<int, ILayer>();

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
                    if (type.Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_INPUT))
                    {
                        this.currentNetwork.InputLayer = layer;
                    }
                    else if (type
                            .Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_OUTPUT))
                    {
                        this.currentNetwork.OutputLayer = layer;
                    }
                    else if (type
                          .Equals(BasicNetworkPersistor.ATTRIBUTE_TYPE_BOTH))
                    {
                        this.currentNetwork.InputLayer = layer;
                        this.currentNetwork.OutputLayer = layer;
                    }
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

            String name = xmlIn.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlIn.LastTag.GetAttributeValue(
                   EncogPersistedCollection.ATTRIBUTE_DESCRIPTION);

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

            }
            this.currentNetwork.Structure.FinalizeStructure();
            return this.currentNetwork;
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
            xmlOut.EndTag();
        }

        /// <summary>
        /// Save the layers to the specified XML writer.
        /// </summary>
        /// <param name="xmlOut">The XML writer.</param>
        private void SaveLayers(WriteXML xmlOut)
        {
            int current = 1;
            foreach (ILayer layer
                    in this.currentNetwork.Structure.Layers)
            {
                String type;

                if (this.currentNetwork.IsInput(layer)
                        && this.currentNetwork.IsOutput(layer))
                {
                    type = BasicNetworkPersistor.ATTRIBUTE_TYPE_BOTH;
                }
                else if (this.currentNetwork.IsInput(layer))
                {
                    type = BasicNetworkPersistor.ATTRIBUTE_TYPE_INPUT;
                }
                else if (this.currentNetwork.IsOutput(layer))
                {
                    type = BasicNetworkPersistor.ATTRIBUTE_TYPE_OUTPUT;
                }
                else if (this.currentNetwork.IsHidden(layer))
                {
                    type = BasicNetworkPersistor.ATTRIBUTE_TYPE_HIDDEN;
                }
                else
                {
                    type = BasicNetworkPersistor.ATTRIBUTE_TYPE_UNKNOWN;
                }

                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_ID, "" + current);
                xmlOut.AddAttribute(BasicNetworkPersistor.ATTRIBUTE_TYPE, type);
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
            foreach (ISynapse synapse in this.currentNetwork.Structure
                    .Synapses)
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
