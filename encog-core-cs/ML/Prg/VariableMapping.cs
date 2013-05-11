using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg
{
    /// <summary>
    /// A variable mapping defines the type for each of the variables in an Encog
    /// program.
    /// </summary>
    [Serializable]
    public class VariableMapping
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        private readonly String name;

        /// <summary>
        /// The variable type.
        /// </summary>
        private readonly EPLValueType variableType;

        /// <summary>
        /// If this is an enum, what is the type.
        /// </summary>
        private readonly int enumType;

        /// <summary>
        /// The count for this given enum. If this is not an enum, then value is not
        /// used.
        /// </summary>
        private readonly int enumValueCount;

        /// <summary>
        /// Construct a variable mapping for a non-enum type.
        /// </summary>
        /// <param name="theName">The variable name.</param>
        /// <param name="theVariableType">The variable type.</param>
        public VariableMapping(String theName, EPLValueType theVariableType)
            : this(theName, theVariableType, 0, 0)
        {

        }

        /// <summary>
        /// Construct a variable mapping.
        /// </summary>
        /// <param name="theName">The name of the variable.</param>
        /// <param name="theVariableType">The type of the variable.</param>
        /// <param name="theEnumType">The enum type.</param>
        /// <param name="theEnumValueCount">The number of values for an enum.</param>
        public VariableMapping(String theName,
                EPLValueType theVariableType, int theEnumType,
                int theEnumValueCount)
        {
            this.name = theName;
            this.variableType = theVariableType;
            this.enumType = theEnumType;
            this.enumValueCount = theEnumValueCount;
        }

        /// <summary>
        /// The enum type.
        /// </summary>
        public int EnumType
        {
            get
            {
                return this.enumType;
            }
        }

        /// <summary>
        /// The enum value count.
        /// </summary>
        public int EnumValueCount
        {
            get
            {
                return this.enumValueCount;
            }
        }

        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// The type.
        /// </summary>
        public EPLValueType VariableType
        {
            get
            {
                return this.variableType;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[VariableMapping: name=");
            result.Append(this.name);
            result.Append(",type=");
            result.Append(this.variableType.ToString());
            result.Append(",enumType=");
            result.Append(this.enumType);
            result.Append(",enumCount=");
            result.Append(this.enumValueCount);
            result.Append("]");
            return result.ToString();
        }
    }
}
