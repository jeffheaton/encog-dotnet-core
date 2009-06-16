using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using log4net;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Parse.Tags.Write;
using Encog.Util;
using Encog.Neural.Activation;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ContextLayer class.
    /// </summary>
    public class ContextLayerPersistor : IPersistor
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));


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
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_X, true))
                {
                    x = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_Y, true))
                {
                    y = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_THRESHOLD, true))
                {
                    threshold = xmlIn.ReadTextToTag();
                }
                else if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            if (neuronCount > 0)
            {
                ContextLayer layer;

                if (threshold == null)
                {
                    layer = new ContextLayer(activation, false, neuronCount);
                }
                else
                {
                    double[] t = ReadCSV.FromCommas(threshold);
                    layer = new ContextLayer(activation, true, neuronCount);
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
                    EncogPersistedCollection.TYPE_CONTEXT_LAYER, xmlOut, obj, false);
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
