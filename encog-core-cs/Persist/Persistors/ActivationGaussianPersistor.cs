using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Neural.Activation;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ActivationGaussian class.
    /// </summary>
    public class ActivationGaussianPersistor : IPersistor
    {
        /// <summary>
        /// The center attribute.
        /// </summary>
        public const String ATTRIBUTE_CENTER = "center";

        /// <summary>
        /// The peak attribute.
        /// </summary>
        public const String ATTRIBUTE_PEAK = "peak";

        /// <summary>
        /// The width attribute.
        /// </summary>
        public const String ATTRIBUTE_WIDTH = "width";

        /// <summary>
        /// Load the specified Encog object from an XML reader.
        /// </summary>
        /// <param name="xmlIn">The XML reader to use.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {
            IDictionary<String, String> map = xmlIn.readPropertyBlock();
            double center = double.Parse(map
                   [ActivationGaussianPersistor.ATTRIBUTE_CENTER]);
            double peak = Double.Parse(map
                   [ActivationGaussianPersistor.ATTRIBUTE_PEAK]);
            double width = Double.Parse(map
                   [ActivationGaussianPersistor.ATTRIBUTE_WIDTH]);
            return new ActivationGaussian(center, peak, width);
        }

        /// <summary>
        /// Save the specified Encog object to an XML writer.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlOut">The XML writer to save to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            ActivationGaussian g = (ActivationGaussian)obj;
            xmlOut.BeginTag(obj.GetType().Name);
            xmlOut.AddProperty(ActivationGaussianPersistor.ATTRIBUTE_CENTER, g
                    .Gausian.Center);
            xmlOut.AddProperty(ActivationGaussianPersistor.ATTRIBUTE_PEAK, g
                    .Gausian.Peak);
            xmlOut.AddProperty(ActivationGaussianPersistor.ATTRIBUTE_WIDTH, g
                    .Gausian.Width);
            xmlOut.EndTag();
        }

    }

}
