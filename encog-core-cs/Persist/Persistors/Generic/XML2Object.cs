using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Encog.Parse.Tags.Read;
using Encog.Util;
using Encog.Parse.Tags;
using System.Collections;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// A generic class used to take an XML segment and produce an object for it.
    /// Some of the Encog persistors make use of this class. The Encog generic
    /// persistor makes use of this class.
    /// </summary>
    public class XML2Object
    {
        /// <summary>
        /// The object mapper to use to resolve references.
        /// </summary>
        private ObjectMapper mapper = new ObjectMapper();

        /// <summary>
        /// Used to read the XML.
        /// </summary>
        private ReadXML xmlIn;

        /// <summary>
        /// Load an object from XML.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        /// <param name="target">The object to load.</param>
        public void Load(ReadXML xmlIn, IEncogPersistedObject target)
        {
            this.xmlIn = xmlIn;
            this.mapper.Clear();
            target.Name = xmlIn.LastTag.GetAttributeValue("name");
            target.Description = xmlIn.LastTag.GetAttributeValue("description");
            LoadActualObject(null, target);
            this.mapper.Resolve();
        }

        /// <summary>
        /// Load an object.
        /// </summary>
        /// <param name="objectField">The object's field.</param>
        /// <param name="target">The object that will get the value.</param>
        private void LoadActualObject(FieldInfo objectField, Object target)
        {


            // handle attributes
            foreach (String key in this.xmlIn.LastTag.Attributes.Keys)
            {
                if (key.Equals("native"))
                {
                    continue;
                }

                // see if there is an id
                if (key.Equals("id"))
                {
                    int reff = int.Parse(this.xmlIn.LastTag
                            .GetAttributeValue("id"));
                    this.mapper.AddObjectMapping(reff, target);
                    continue;
                }

                FieldInfo field = ReflectionUtil.FindField(target.GetType(),
                       key);

                if (field == null)
                {
                    ReflectionUtil.FindField(target.GetType(),
                       key);
                }

                String value = this.xmlIn.LastTag.GetAttributeValue(key);
                SetFieldValue(field, target, value);
            }

            // handle properties
            while (this.xmlIn.ReadToTag())
            {
                if (this.xmlIn.LastTag.TagType == Tag.Type.BEGIN)
                {
                    String tagName = this.xmlIn.LastTag.Name;
                    FieldInfo field = ReflectionUtil.FindField(target
                           .GetType(), tagName);
                    if (field == null)
                    {
                        continue;
                    }

                    Object currentValue = field.GetValue(target);

                    if (ReflectionUtil.IsSimple(field.FieldType))
                    {
                        String value = this.xmlIn.ReadTextToTag();
                        SetFieldValue(field, target, value);
                    }
                    else if (currentValue is ICollection)
                    {
                        LoadCollection((IList)currentValue);
                    }
                    else
                    {
                        this.xmlIn.ReadToTag();
                        Object nextObject = LoadObject(field, target);
                        field.SetValue(target, nextObject);
                    }
                }
                else if (this.xmlIn.LastTag.TagType == Tag.Type.END)
                {
                    if (this.xmlIn.LastTag.Name.Equals(
                            target.GetType().Name))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Load a collection.
        /// </summary>
        /// <param name="collection">The collection to load.</param>
        private void LoadCollection(IList collection)
        {

            while (this.xmlIn.ReadToTag())
            {
                if (this.xmlIn.LastTag.TagType == Tag.Type.BEGIN)
                {
                    String tagName = this.xmlIn.LastTag.Name;
                    String c = ReflectionUtil
                           .ResolveEncogClass(tagName);
                    Object target = Assembly.GetExecutingAssembly().CreateInstance(c);
                    LoadActualObject(null, target);
                    collection.Add(target);
                }
                else if (this.xmlIn.LastTag.TagType == Tag.Type.END)
                {
                    return;
                }
            }

        }


        /// <summary>
        /// Load an object and handle reference if needed.
        /// </summary>
        /// <param name="objectField">The field.</param>
        /// <param name="parent">The object that holds the field.</param>
        /// <returns>The loaded object.</returns>
        private Object LoadObject(FieldInfo objectField, Object parent)
        {
            String reff = this.xmlIn.LastTag.GetAttributeValue("ref");

            // handle ref
            if (reff != null)
            {
                int reff2 = int.Parse(this.xmlIn.LastTag
                       .GetAttributeValue("ref"));
                this.mapper.AddFieldMapping(reff2, objectField, parent);
                this.xmlIn.ReadToTag();
                return null;
            }
            else
            {
                String c = ReflectionUtil.ResolveEncogClass(this.xmlIn
                        .LastTag.Name);
                Object obj = Assembly.GetExecutingAssembly().CreateInstance(c);
                LoadActualObject(objectField, obj);
                return obj;
            }
        }

        /// <summary>
        /// Set a field value.
        /// </summary>
        /// <param name="field">The field to set.</param>
        /// <param name="target">The object that contains the field.</param>
        /// <param name="value">The field value.</param>
        private void SetFieldValue(FieldInfo field, Object target,
                 String value)
        {
            Type type = field.GetType();
            if (type == typeof(long))
            {
                field.SetValue(target, long.Parse(value));
            }
            else if (type == typeof(int))
            {
                field.SetValue(target, int.Parse(value));
            }
            else if (type == typeof(short))
            {
                field.SetValue(target, short.Parse(value));
            }
            else if (type == typeof(double))
            {
                field.SetValue(target, double.Parse(value));
            }
            else if (type == typeof(float))
            {
                field.SetValue(target, float.Parse(value));
            }
            else if (type == typeof(String))
            {
                field.SetValue(target, value);
            }
            else if (type == typeof(bool))
            {
                field.SetValue(target,
                        value.ToLower().Equals("true"));
            }

        }
    }
}
