using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Encog.Parse.Tags.Write;
using Encog.Util;
using System.Collections;
using Encog.Persist.Attributes;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// A generic class used to take an object and produce XML for it. Some of the
    /// Encog persistors make use of this class. The Encog generic persistor makes
    /// use of this class.
    /// </summary>
    public class Object2XML
    {
        /// <summary>
        /// The XML writer used.
        /// </summary>
        private WriteXML xmlOut;

        /// <summary>
        /// The object tagger, allows the objects to be tagged with references.
        /// </summary>
        private ObjectTagger tagger = new ObjectTagger();

        /// <summary>
        /// Save the object to XML.
        /// </summary>
        /// <param name="encogObject">The object to save.</param>
        /// <param name="xmlOut">The XML writer.</param>
        public void Save(IEncogPersistedObject encogObject, WriteXML xmlOut)
        {
            this.xmlOut = xmlOut;

            PersistorUtil.BeginEncogObject(encogObject.GetType().Name
                    , xmlOut, encogObject, true);

            this.tagger.Analyze(encogObject);

            foreach (FieldInfo childField in ReflectionUtil
                    .GetAllFields(encogObject.GetType()))
            {
                if (ReflectionUtil.ShouldAccessField(childField, true))
                {
                    Object childValue = childField.GetValue(encogObject);
                    xmlOut.BeginTag(childField.Name);
                    SaveField(childValue);
                    xmlOut.EndTag();
                }
            }

            xmlOut.EndTag();


        }

        /// <summary>
        /// Save a collection.
        /// </summary>
        /// <param name="value">The collection to save.</param>
        private void SaveCollection(ICollection value)
        {

            foreach (Object obj in value)
            {
                SaveObject(obj);
            }
        }

        /// <summary>
        /// Save a field.
        /// </summary>
        /// <param name="fieldObject">The field to save.</param>
        private void SaveField(Object fieldObject)
        {
            if (fieldObject != null)
            {
                if (fieldObject == null)
                {
                    // nothing to save
                }
                else if (fieldObject is ICollection)
                {
                    SaveCollection((ICollection)fieldObject);
                }
                else if (fieldObject is Boolean)
                {
                    this.xmlOut.AddText(fieldObject.ToString().ToLower());
                }
                else if (ReflectionUtil.IsSimple(fieldObject.GetType()))
                {
                    this.xmlOut.AddText(fieldObject.ToString());
                }
                else if (fieldObject is String)
                {
                    this.xmlOut.AddText(fieldObject.ToString());
                }
                else
                {
                    SaveObject(fieldObject);
                }
            }
        }

        /// <summary>
        /// Save a field by reference.
        /// </summary>
        /// <param name="fieldObject">The field to save.</param>
        private void SaveFieldReference(Object fieldObject)
        {
            if (this.tagger.HasReference(fieldObject))
            {
                this.xmlOut.AddAttribute("ref", ""
                        + this.tagger.GetReference(fieldObject));
            }
            else
            {
                this.xmlOut.AddAttribute("ref", "");
            }

            this.xmlOut.BeginTag(fieldObject.GetType().Name);
            this.xmlOut.EndTag();
        }

        /// <summary>
        /// Save an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveObject(Object obj)
        {
            // does this object have an ID?

            if (this.tagger.HasReference(obj))
            {
                int id = this.tagger.GetReference(obj);
                this.xmlOut.AddAttribute("id", "" + id);
            }

            // get all fields
            ICollection<FieldInfo> allFields = ReflectionUtil
                    .GetAllFields(obj.GetType());
            // handle attributes
            foreach (FieldInfo childField in allFields)
            {
                if (ReflectionUtil.ShouldAccessField(childField, false)
                        && (ReflectionUtil.HasAttribute(childField, typeof(EGAttribute))))
                {
                    Object childValue = childField.GetValue(obj);
                    if (childValue is Boolean)
                    {
                        this.xmlOut.AddAttribute(childField.Name, childValue
                            .ToString().ToLower());
                    }
                    else
                    {
                        this.xmlOut.AddAttribute(childField.Name, childValue
                            .ToString());
                    }
                }
            }
            // handle actual fields
            this.xmlOut.BeginTag(obj.GetType().Name);
            foreach (FieldInfo childField in allFields)
            {
                if (ReflectionUtil.ShouldAccessField(childField, false)
                        && !ReflectionUtil.HasAttribute(childField, typeof(EGAttribute)))
                {

                    Object childValue = childField.GetValue(obj);

                    this.xmlOut.BeginTag(childField.Name);
                    if (ReflectionUtil.HasAttribute(childField, typeof(EGReference)))
                    {
                        SaveFieldReference(childValue);
                    }
                    else
                    {
                        SaveField(childValue);

                    }
                    this.xmlOut.EndTag();
                }
            }
            this.xmlOut.EndTag();
        }
    }
}
