using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Normalize;

namespace Encog.Persist.Persistors
{
    public class NormalizationStatsPersistor : IPersistor
    {
        public const String TAG_FIELDS = "fields";
        public const String TAG_STAT = "NormalizedFieldStats";
        public const String PROPERTY_AHIGH = "actualHigh";
        public const String PROPERTY_ALOW = "actualLow";
        public const String PROPERTY_HIGH = "high";
        public const String PROPERTY_LOW = "low";
        public const String PROPERTY_TYPE = "type";
        public const String TYPE_IGNORE = "IGNORE";
        public const String TYPE_PASS = "PASS";
        public const String TYPE_NORMALIZE = "NORMALIZE";
        public const String PROPERTY_NAME = "Name";

        private NormalizationStats norm;

        public IEncogPersistedObject Load(Parse.Tags.Read.ReadXML xmlIn)
        {
                        String name = xmlIn.LastTag.Attributes[EncogPersistedCollection.ATTRIBUTE_NAME];
            String description = xmlIn.LastTag.Attributes[
                   EncogPersistedCollection.ATTRIBUTE_DESCRIPTION];


            List<NormalizedFieldStats> list = new List<NormalizedFieldStats>();

            String end = xmlIn.LastTag.Name;
            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(NormalizationStatsPersistor.TAG_FIELDS, true))
                {
                    String end2 = xmlIn.LastTag.Name;
                    while (xmlIn.ReadToTag())
                    {
                        if (xmlIn.IsIt(NormalizationStatsPersistor.TAG_STAT, true))
                        {
                            IDictionary<string, string> properties = xmlIn.ReadPropertyBlock();
                            NormalizedFieldStats stat = new NormalizedFieldStats();
                            stat.ActualHigh = double.Parse(properties[NormalizationStatsPersistor.PROPERTY_AHIGH]);
                            stat.ActualLow = double.Parse(properties[NormalizationStatsPersistor.PROPERTY_ALOW]);
                            stat.NormalizedHigh = double.Parse(properties[NormalizationStatsPersistor.PROPERTY_HIGH]);
                            stat.NormalizedLow = double.Parse(properties[NormalizationStatsPersistor.PROPERTY_LOW]);
                            stat.Name = properties[NormalizationStatsPersistor.PROPERTY_NAME];
                            String type = properties[NormalizationStatsPersistor.PROPERTY_TYPE];

                            if (string.Compare(type, NormalizationStatsPersistor.TYPE_IGNORE, true) == 0)
                            {
                                stat.Action = NormalizationDesired.Ignore;
                            }
                            else if (string.Compare(type, NormalizationStatsPersistor.TYPE_NORMALIZE, true) == 0)
                            {
                                stat.Action = NormalizationDesired.Ignore;
                            }
                            else if (string.Compare(type, NormalizationStatsPersistor.TYPE_PASS, true) == 0)
                            {
                                stat.Action = NormalizationDesired.Ignore;
                            }
                            list.Add(stat);
                        }
                        else if (xmlIn.IsIt(end2, false))
                        {
                            break;
                        }
                    }
                }
                else if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            this.norm = new NormalizationStats(list.Count);
            this.norm.Data = list.ToArray();
            this.norm.Name = name;
            this.norm.Description = description;

            return this.norm;
        }

        public void Save(IEncogPersistedObject obj, Parse.Tags.Write.WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(EncogPersistedCollection.TYPE_NORM,
                xmlOut, obj, true);
            this.norm = (NormalizationStats)obj;

            xmlOut.BeginTag(NormalizationStatsPersistor.TAG_FIELDS);
            foreach (NormalizedFieldStats field in this.norm.Data)
            {
                xmlOut.BeginTag(NormalizationStatsPersistor.TAG_STAT);
                xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_AHIGH,field.ActualHigh);
                xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_ALOW, field.ActualLow);
                xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_HIGH, field.NormalizedHigh);
                xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_LOW, field.NormalizedLow);
                xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_NAME, field.Name);
                switch (field.Action)
                {
                    case NormalizationDesired.Ignore:
                        xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_TYPE, NormalizationStatsPersistor.TYPE_IGNORE);
                        break;
                    case NormalizationDesired.Normalize:
                        xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_TYPE, NormalizationStatsPersistor.TYPE_NORMALIZE);
                        break;
                    case NormalizationDesired.PassThrough:
                        xmlOut.AddProperty(NormalizationStatsPersistor.PROPERTY_TYPE, NormalizationStatsPersistor.TYPE_PASS);
                        break;
                }

                xmlOut.EndTag();
            }
            xmlOut.EndTag();
            xmlOut.EndTag();
        }
    }
}
