//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Encog.Util.File;

namespace Encog.Util
{
    /// <summary>
    /// A set of C# reflection utilities.
    /// </summary>
    public class ReflectionUtil
    {
        /// <summary>
        /// Path to the activation functions.
        /// </summary>
        public const String AF_PATH = "Encog.Engine.Network.Activation.";

        /// <summary>
        /// Path to RBF's.
        /// </summary>
        public const String RBF_PATH = "Encog.MathUtil.RBF.";

        /// <summary>
        /// A map between short class names and the full path names.
        /// </summary>
        private static readonly IDictionary<String, String> classMap = new Dictionary<String, String>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ReflectionUtil()
        {
        }


        /// <summary>
        /// Find the specified field, look also in superclasses.
        /// </summary>
        /// <param name="c">The class to search.</param>
        /// <param name="name">The name of the field we are looking for.</param>
        /// <returns>The field.</returns>
        public static FieldInfo FindField(Type c, String name)
        {
            ICollection<FieldInfo> list = GetAllFields(c);
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
            foreach (
                FieldInfo field in c.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                result.Add(field);
            }

            if (c.BaseType != null)
                GetAllFields(c.BaseType, result);
        }

        /// <summary>
        /// Load the classmap file. This allows classes to be resolved using just the
        /// simple name.
        /// </summary>
        public static void LoadClassmap()
        {
            {
                Stream istream = ResourceLoader.CreateStream("Encog.Resources.classes.txt");
                var sr = new StreamReader(istream);

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    int idx = line.LastIndexOf('.');
                    if (idx != -1)
                    {
                        String simpleName = line.Substring(idx + 1);
                        classMap[simpleName] = line;
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
            if (classMap.Count == 0)
            {
                LoadClassmap();
            }

            if (!classMap.ContainsKey(name))
                return null;
            else
                return classMap[name];
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
        /// Resolve an enumeration.
        /// </summary>
        /// <param name="field">The field to resolve.</param>
        /// <param name="value_ren">The value to get the enum for.</param>
        /// <returns>The enum that was resolved.</returns>
        public static Object ResolveEnum(FieldInfo field, FieldInfo value_ren)
        {
            Type type = field.GetType();
            Object[] objs = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (MemberInfo obj in objs)
            {
                if (obj.Name.Equals(value_ren))
                    return obj;
            }
            return null;
        }

        /// <summary>
        /// Loop over all loaded assembles and try to create the class.
        /// </summary>
        /// <param name="name">The class to create.</param>
        /// <returns>The created class.</returns>
        public static Object LoadObject(String name)
        {
#if !SILVERLIGHT
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Object result = null;

            foreach (Assembly a in assemblies)
            {
                result = a.CreateInstance(name);
                if (result != null)
                    break;
            }

            return result;
#else
            Assembly a = Assembly.GetExecutingAssembly();
            return a.CreateInstance(name);
#endif
        }
    }
}
