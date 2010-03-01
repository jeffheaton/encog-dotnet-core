// Encog(tm) Artificial Intelligence Framework v2.3
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
using System.Reflection;
using System.IO;
using Encog.Persist.Location;
using Encog.Persist.Attributes;

namespace Encog.MathUtil
{
    /// <summary>
    /// A set of C# reflection utilities.
    /// </summary>
    public class ReflectionUtil
    {
        /// <summary>
        /// A map between short class names and the full path names.
        /// </summary>
        private static IDictionary<String, String> classMap = new Dictionary<String, String>();


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
        public static IList<FieldInfo> GetAllFields(Type c)
        {
            IList<FieldInfo> result = new List<FieldInfo>();
            GetAllFields(c, result);
            return result;
        }

        /// <summary>
        /// Get all of the fields for the specified class and recurse to check the base class.
        /// </summary>
        /// <param name="c">The class to scan.</param>
        /// <param name="result">A list of fields.</param>
        public static void GetAllFields(Type c, IList<FieldInfo> result)
        {
            foreach (FieldInfo field in c.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                result.Add(field);
            }

            if( c.BaseType!=null )
                GetAllFields(c.BaseType, result);
        }

        /// <summary>
        /// Determine if an object is "simple", that is it should be persisted just
        /// with a .ToString.
        /// </summary>
        /// <param name="t">The type object to check.</param>
        /// <returns>True if the object is simple.</returns>
        public static bool IsSimple(Type t)
        {
            return (t == typeof(File)) || (t == typeof(String) ) || (t == typeof(int)) || (t == typeof(double) ) || (t == typeof(float) ) || (t==typeof(short)) || (t==typeof(char) || (t==typeof(bool)));
        }

        /// <summary>
        /// Load the classmap file. This allows classes to be resolved using just the
        /// simple name.
        /// </summary>
        public static void LoadClassmap()
        {
            {
                ResourcePersistence resource = new ResourcePersistence(
                        "Encog.Resources.classes.txt");
                Stream istream = resource.CreateStream(FileMode.Open);
                StreamReader sr = new StreamReader(istream);

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    int idx = line.LastIndexOf('.');
                    if (idx != -1)
                    {
                        String simpleName = line.Substring(idx+1);
                        ReflectionUtil.classMap[simpleName] = line;
                    }
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
        public static String ResolveEncogClass(String name)
        {
            if (ReflectionUtil.classMap.Count == 0)
            {
                ReflectionUtil.LoadClassmap();
            }

            if (!ReflectionUtil.classMap.ContainsKey(name))
                return null;
            else
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
        /// Determine if the specified type contains the specified attribute.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>True if the type contains the attribute.</returns>
        public static bool HasAttribute(Type t, Type attribute)
        {
            foreach (Object obj in t.GetCustomAttributes(true))
            {
                if (obj.GetType() == attribute)
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
