using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Encog.Persist.Location;
using Encog.Persist.Attributes;

namespace Encog.Util
{
    /// <summary>
    /// A set of C# reflection utilities.
    /// </summary>
    public class ReflectionUtil
    {
        /// <summary>
        /// A map between short class names and the full path names.
        /// </summary>
        private static IDictionary<String, Type> classMap = new Dictionary<String, Type>();


        /// <summary>
        /// Find the specified field, look also in superclasses.
        /// </summary>
        /// <param name="c">The class to search.</param>
        /// <param name="name">The name of the field we are looking for.</param>
        /// <returns>The field.</returns>
        public static FieldInfo FindField(Type c, String name)
        {
            ICollection<FieldInfo> list = ReflectionUtil.GetAllFields(c);
            foreach (FieldInfo field in list)
            {
                if (field.Name.Equals(name))
                {
                    return field;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all of the fields from the specified class as a collection.
        /// </summary>
        /// <param name="c">The class to access.</param>
        /// <returns>All of the fields from this class and subclasses.</returns>
        public static ICollection<FieldInfo> GetAllFields(Type c)
        {
            IList<FieldInfo> result = new List<FieldInfo>();
            ReflectionUtil.GetAllFields(c, result);
            return result;
        }

        /// <summary>
        /// Get all of the fields in the specified class and super classes.
        /// </summary>
        /// <param name="c">The class to check.</param>
        /// <param name="fields">A collection to hold the classes.</param>
        public static void GetAllFields(Type c,
                 ICollection<FieldInfo> fields)
        {
            foreach (FieldInfo field in c.GetFields())
            {
                fields.Add(field);
            }

            Type s = c.BaseType;
            if (s != null)
            {
                ReflectionUtil.GetAllFields(s, fields);
            }
        }


        /// <summary>
        /// Determine if an object is "simple", that is it should be persisted just
        /// with a .ToString.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is simple.</returns>
        public static bool IsSimple(Object obj)
        {
            return (obj is File) || (obj is String) || (obj is int) || (obj is double) || (obj is float) || (obj is short);
        }

        /// <summary>
        /// Load the classmap file. This allows classes to be resolved using just the
        /// simple name.
        /// </summary>
        public static void LoadClassmap()
        {

            {
                ResourcePersistence resource = new ResourcePersistence(
                        "org/encog/data/classes.txt");
                Stream istream = resource.CreateStream(FileMode.Open);
                StreamReader sr = new StreamReader(istream);

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    Type c = Assembly.GetExecutingAssembly().GetType(line);
                    ReflectionUtil.classMap[c.Name] = c;
                }
                sr.Close();
                istream.Close();

            }
        }

        /// <summary>
        /// Resolve an encog class using its simple name.
        /// </summary>
        /// <param name="name">The simple name of the class.</param>
        /// <returns>The class requested.</returns>
        public static Type ResolveEncogClass(String name)
        {
            //Assembly.GetExecutingAssembly().GetType;

            if (ReflectionUtil.classMap.Count == 0)
            {
                ReflectionUtil.LoadClassmap();
            }
            return ReflectionUtil.classMap[name];
        }


        /// <summary>
        /// Determine if the specified field has the specified attribute.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <param name="t">See if the field has this attribute.</param>
        /// <returns>True if the field has the specified attribute.</returns>
        public static bool HasAttribute(FieldInfo field, Type t)
        {
            foreach (Object obj in field.GetCustomAttributes(true))
            {
                if (obj.GetType() == t)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if Encog persistence should access the specified field.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <param name="isBase">True if this is the actual Encog persisted class(top level)</param>
        /// <returns>True if the class should be accessed.</returns>
        public static bool ShouldAccessField(FieldInfo field,
                 bool isBase)
        {

            if (HasAttribute(field, typeof(EGIgnore)))
            {
                return false;
            }

            if (!field.IsStatic)
            {
                if (isBase)
                {
                    if (field.Name.Equals("name")
                            || field.Name.Equals("description"))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ReflectionUtil()
        {

        }

    }
}
