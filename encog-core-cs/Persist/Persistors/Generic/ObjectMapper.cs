using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Encog.Persist.Persistors.Generic
{
    /// <summary>
    /// Used to map objects to reference numbers. This is where reference numbers are
    /// resolved. This class is used by Encog generic persistence.
    /// </summary>
    public class ObjectMapper
    {
        /// <summary>
        /// A map from reference numbers to objects.
        /// </summary>
        private IDictionary<int, Object> objectMap = new Dictionary<int, Object>();

        /// <summary>
        /// A list of all of the field mappings.
        /// </summary>
        private IList<FieldMapping> list = new List<FieldMapping>();

        /// <summary>
        /// Add a field mapping to be resolved later. This builds a list of
        /// references to be resolved later when the resolve method is called.
        /// </summary>
        /// <param name="reff">The reference number.</param>
        /// <param name="field">The field to map.</param>
        /// <param name="target">The target object that holds the field.</param>
        public void AddFieldMapping(int reff, FieldInfo field,
                Object target)
        {
            this.list.Add(new FieldMapping(reff, field, target));
        }

        /// <summary>
        /// Add an object mapping to be resolved later.
        /// </summary>
        /// <param name="reff">The object reference.</param>
        /// <param name="obj">The object.</param>
        public void AddObjectMapping(int reff, Object obj)
        {
            this.objectMap[reff] = obj;
        }

        /// <summary>
        /// Clear the map and reference list.
        /// </summary>
        public void Clear()
        {
            this.objectMap.Clear();
            this.list.Clear();
        }

        /// <summary>
        /// Resolve all references and place the correct objects.
        /// </summary>
        public void Resolve()
        {

            foreach (FieldMapping field in this.list)
            {
                Object obj = this.objectMap[field.Reff];
                field.Field.SetValue(field.Target, obj);
            }
        }
    }
}
