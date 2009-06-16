using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Write;
using Encog.Util;
using Encog.Parse.Tags.Read;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// Provides basic functions that many of the persistors will need.
    /// </summary>
    public class BasicLayerPersistor : IPersistor
    {
        /// <summary>
        /// The activation function tag.
        /// </summary>
        public const String TAG_ACTIVATION = "activation";

        /// <summary>
        /// The neurons property.
        /// </summary>
        public const String PROPERTY_NEURONS = "neurons";

        /// <summary>
        /// The threshold property.
        /// </summary>
        public const String PROPERTY_THRESHOLD = "threshold";

        /// <summary>
        /// The x-coordinate to place this object at.
        /// </summary>
        public const String PROPERTY_X = "x";

        /// <summary>
        /// The y-coordinate to place this object at.
        /// </summary>
        public const String PROPERTY_Y = "y";

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            int neuronCount = 0;
            int x = 0;
            int y = 0;
            String threshold = null;
            IActivationFunction activation = null;
            String end = xmlIn.LastTag.Name;

            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicLayerPersistor.TAG_ACTIVATION, true))
                {
                    xmlIn.ReadToTag();
                    String type = xmlIn.LastTag.Name;
                    IPersistor persistor = PersistorUtil.CreatePersistor(type);
                    activation = (IActivationFunction)persistor.Load(xmlIn);
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_NEURONS, true))
                {
                    neuronCount = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_THRESHOLD, true))
                {
                    threshold = xmlIn.ReadTextToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_X, true))
                {
                    x = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_Y, true))
                {
                    y = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            if (neuronCount > 0)
            {
                BasicLayer layer;

                if (threshold == null)
                {
                    layer = new BasicLayer(activation, false, neuronCount);
                }
                else
                {
                    double[] t = ReadCSV.FromCommas(threshold);
                    layer = new BasicLayer(activation, true, neuronCount);
                    for (int i = 0; i < t.Length; i++)
                    {
                        layer.Threshold[i] = t[i];
                    }
                }
                layer.X = x;
                layer.Y = y;
                return layer;
            }
            return null;
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(
                    EncogPersistedCollection.TYPE_BASIC_LAYER, xmlOut, obj, false);
            BasicLayer layer = (BasicLayer)obj;

            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_NEURONS, layer
                    .NeuronCount);
            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_X, layer.X);
            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_Y, layer.Y);

            if (layer.HasThreshold)
            {
                StringBuilder result = new StringBuilder();
                ReadCSV.ToCommas(result, layer.Threshold);
                xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_THRESHOLD, result
                        .ToString());
            }

            xmlOut.BeginTag(BasicLayerPersistor.TAG_ACTIVATION);
            IPersistor persistor = layer.ActivationFunction
                   .CreatePersistor();
            persistor.Save(layer.ActivationFunction, xmlOut);
            xmlOut.EndTag();

            xmlOut.EndTag();
        }
    }
}
