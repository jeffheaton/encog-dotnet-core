using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg.ExpValue;
using Encog.ML.EA.Exceptions;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    /// Provides a template for parameters to the opcodes. This defines the accepted
    /// types and if type of a given parameter passes through to the return type.
    /// </summary>
    public class ParamTemplate
    {
        /// <summary>
        /// Possible types for this parameter.
        /// </summary>
        private HashSet<EPLValueType> possibleTypes = new HashSet<EPLValueType>();

        /// <summary>
        /// Is this a pass through argument. If so, then the return type of the
        /// parent opcode will depend on the actual type of this parameter.
        /// </summary>
        public bool PassThrough { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ParamTemplate()
        {
        }

        /// <summary>
        /// Add all known types.
        /// </summary>
        public void AddAllTypes()
        {
            foreach (EPLValueType t in Enum.GetValues(typeof(EPLValueType)))
            {
                AddType(t);
            }
        }

        /// <summary>
        /// Add the specified type.
        /// </summary>
        /// <param name="theType">The type to add.</param>
        public void AddType(String theType)
        {
            if (theType.Equals("b"))
            {
                AddType(EPLValueType.booleanType);
            }
            else if (theType.Equals("e"))
            {
                AddType(EPLValueType.enumType);
            }
            else if (theType.Equals("f"))
            {
                AddType(EPLValueType.floatingType);
            }
            else if (theType.Equals("i"))
            {
                AddType(EPLValueType.intType);
            }
            else if (theType.Equals("s"))
            {
                AddType(EPLValueType.stringType);
            }
            else if (theType.Equals("*"))
            {
                AddAllTypes();
            }
            else
            {
                throw new EACompileError("Unknown type: " + theType);
            }
        }

        /// <summary>
        /// Add a type using a type enum.
        /// </summary>
        /// <param name="theType">The type to add.</param>
        public void AddType(EPLValueType theType)
        {
            this.possibleTypes.Add(theType);
        }

        /// <summary>
        /// Determine the possable argument types, given the parent types.
        /// </summary>
        /// <param name="parentTypes">The parent types.</param>
        /// <returns>The possable types.</returns>
        public IList<EPLValueType> DetermineArgumentTypes(
                IList<EPLValueType> parentTypes)
        {
            if (PassThrough)
            {
                return parentTypes;
            }

            IList<EPLValueType> result = new List<EPLValueType>();
            result = result.Union(PossibleTypes).ToList();
            return result;
        }

        /// <summary>
        /// All possible types.
        /// </summary>
        public HashSet<EPLValueType> PossibleTypes
        {
            get
            {
                return this.possibleTypes;
            }
        }

    }
}
