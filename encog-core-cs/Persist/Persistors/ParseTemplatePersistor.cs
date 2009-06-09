using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Recognize;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags.Write;
using Encog.Parse;
using System.Reflection;
using Encog.Parse.Signal;

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the ParseTemplate class.
    /// </summary>
    class ParseTemplatePersistor : IPersistor
    {


        /// <summary>
        /// The template currently being processed.
        /// </summary>
        private ParseTemplate template;

        /// <summary>
        /// Load ParseTemplate object.
        /// </summary>
        /// <param name="xmlin">The XML to read it from.</param>
        /// <returns>The loaded object.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            this.template = new ParseTemplate();

            String name = xmlin.LastTag.GetAttributeValue("name");
            String description = xmlin.LastTag.GetAttributeValue("description");

            this.template.Name = name;
            this.template.Description = description;

            String end = xmlin.LastTag.Name;
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt("recognize", true))
                {
                    LoadRecognize(xmlin);
                }
                else if (xmlin.IsIt(end, false))
                {
                    break;
                }

            }

            return this.template;
        }

        /// <summary>
        /// Load the specified char processor.
        /// </summary>
        /// <param name="element">The XML recognize element.</param>
        /// <param name="xmlin">The XML reader.</param>
        private void LoadChar(RecognizeElement element, ReadXML xmlin)
        {
            String value = xmlin.LastTag.GetAttributeValue("value");
            String from = xmlin.LastTag.GetAttributeValue("from");
            String to = xmlin.LastTag.GetAttributeValue("to");
            if (value != null)
            {
                element.Add(value[0]);
            }
            else
            {
                element.AddRange(from[0], to[0]);
            }
        }

        /// <summary>
        /// Load the specified recognize element.
        /// </summary>
        /// <param name="recognize">The element to load.</param>
        /// <param name="xmlin">The XML reader.</param>
        private void LoadElement(Recognize recognize, ReadXML xmlin)
        {
            String type = xmlin.LastTag.GetAttributeValue("type");
            RecognizeElement recognizeElement;

            if (type.Equals("ALLOW_ONE"))
            {
                recognizeElement = recognize
                        .CreateElement(RecognizeElement.ALLOW_ONE);
            }
            else if (type.Equals("ALLOW_MULTIPLE"))
            {
                recognizeElement = recognize
                        .CreateElement(RecognizeElement.ALLOW_MULTIPLE);
            }
            else
            {
                recognizeElement = null; // ERROR
            }

            String end = xmlin.LastTag.Name;
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt("char", true))
                {
                    LoadChar(recognizeElement, xmlin);
                }
                else if (xmlin.IsIt("unit", true))
                {
                    LoadUnit(recognizeElement, xmlin);
                }
                else if (xmlin.IsIt(end, false))
                {
                    break;
                }

            }

        }


        /// <summary>
        /// Load the current recognize object.
        /// </summary>
        /// <param name="xmlin">The XML reader.</param>
        private void LoadRecognize(ReadXML xmlin)
        {
            String id = xmlin.LastTag.GetAttributeValue("id");
            String ignore = xmlin.LastTag.GetAttributeValue("ignore");
            String recognizeClass = xmlin.LastTag.GetAttributeValue("class");

            Recognize recognize = new Recognize(id);
            try
            {
                if (recognizeClass != null)
                {
                    recognize.SignalClass = Assembly.GetExecutingAssembly().GetType(recognizeClass);
                }
            }
            catch (Exception e)
            {
                throw new ParseError(e);
            }

            if (string.Compare("true", ignore, true) == 0)
            {
                recognize.Ignore = true;
            }
            else
            {
                recognize.Ignore = false;
            }

            String end = xmlin.LastTag.Name;
            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt("element", true))
                {
                    LoadElement(recognize, xmlin);
                }
                else if (xmlin.IsIt(end, false))
                {
                    break;
                }

            }

            this.template.AddRecognizer(recognize);
        }

        /// <summary>
        /// Load the specified unit.
        /// </summary>
        /// <param name="element">The recognize element.</param>
        /// <param name="xmlin">The specified XML reader.</param>
        private void LoadUnit(RecognizeElement element, ReadXML xmlin)
        {
            String type = xmlin.LastTag.GetAttributeValue("type");
            String value = xmlin.LastTag.GetAttributeValue("value");
            element.Type = type;
            if ((value != null) && !value.Equals(""))
            {
                element.AddAcceptedSignal(type, value);
            }
            else
            {
                element.AddAcceptedSignal(type, null);
            }
        }

        /// <summary>
        /// Save the parse template.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">The XML writer.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {
            // TODO Auto-generated method stub

        }
    }
}
