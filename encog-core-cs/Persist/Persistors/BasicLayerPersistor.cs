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
using Encog.Parse.Tags.Write;
using Encog.MathUtil;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks.Layers;
using Encog.Util.CSV;
using Encog.Engine.Network.Activation;
using Encog.Util;
using System.Reflection;

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
        /// The bias activation.
        /// </summary>
	    public const String PROPERTY_BIAS_ACTIVATION = "biasActivation";

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
            double biasActivation = 1;

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
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_BIAS_ACTIVATION, true))
                {
                    biasActivation = double.Parse( xmlIn.ReadTextToTag() );
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
                    double[] t = NumberList.FromList(CSVFormat.EG_FORMAT, threshold);
                    layer = new BasicLayer(activation, true, neuronCount);
                    for (int i = 0; i < t.Length; i++)
                    {
                        layer.BiasWeights[i] = t[i];
                    }
                }
                layer.X = x;
                layer.Y = y;
                layer.BiasActivation = biasActivation;
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

            if (layer.HasBias)
            {
                StringBuilder result = new StringBuilder();
                NumberList.ToList(CSVFormat.EG_FORMAT, result, layer.BiasWeights);
                xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_THRESHOLD, result
                        .ToString());
            }

            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_BIAS_ACTIVATION, layer.BiasActivation);

            if (layer.ActivationFunction != null)
            {
                saveActivationFunction(layer.ActivationFunction, xmlOut);
            }

            xmlOut.EndTag();
        }

        
        public static void saveActivationFunction(
			IActivationFunction activationFunction, WriteXML xmlOut) {
		if (activationFunction != null) {
			xmlOut.BeginTag(BasicLayerPersistor.TAG_ACTIVATION);
			xmlOut.BeginTag(activationFunction.GetType().Name);
			String[] names = activationFunction.ParamNames;
			for (int i = 0; i < names.Length; i++) {
				String str = names[i];
				double d = activationFunction.Params[i];
                xmlOut.AddAttribute(str, "" + CSVFormat.EG_FORMAT.Format(d, 10));
			}
			xmlOut.EndTag();
			xmlOut.EndTag();
		}
	}

	public static IActivationFunction loadActivation(String type, ReadXML xmlIn) {

		try {
			String clazz = ReflectionUtil.ResolveEncogClass(type);

            IActivationFunction result = (IActivationFunction)Assembly.GetExecutingAssembly().CreateInstance(clazz);

			foreach (String key in xmlIn.LastTag.Attributes.Keys ) {
				int index = -1;

				for (int i = 0; i < result.ParamNames.Length; i++) {
					if ( String.Compare(key,result.ParamNames[i],true)==0) {
						index = i;
						break;
					}

					if (index != -1) {
						String str = xmlIn.LastTag.GetAttributeValue(key);
						double d = CSVFormat.EG_FORMAT.Parse(str);
						result.SetParam(index, d);
					}
				}
			}

			return result;
		} catch (Exception e) {
			throw new EncogError(e);
		} 
	}

    }
}
