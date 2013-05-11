using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Exceptions;

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    /// An EncogProgram expression value. These is how Encog stores variables and
    /// calculates values.
    /// </summary>
    [Serializable]
    public class ExpressionValue
    {
        /// <summary>
        /// If the value is a string, this contains the value.
        /// </summary>
        private readonly String stringValue;

        /// <summary>
        /// If the value is a float, this contains the value.
        /// </summary>
        private readonly double floatValue;

        /// <summary>
        /// If the value is a boolean, this contains the value.
        /// </summary>
        private readonly bool boolValue;

        /// <summary>
        /// The type of this expression.
        /// </summary>
        private readonly EPLValueType expressionType;

        /// <summary>
        /// If the value is an int, this contains the value.
        /// </summary>
        private readonly long intValue;

        /// <summary>
        /// If the value is an enum, this contains the value.
        /// </summary>
        private readonly int enumType;

        /// <summary>
        /// Construct a boolean expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(bool theValue)
        {
            this.boolValue = theValue;
            this.expressionType = EPLValueType.booleanType;
            this.floatValue = 0;
            this.stringValue = null;
            this.intValue = 0;
            this.enumType = -1;
        }

        /// <summary>
        /// Construct a boolean expression. 
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(double theValue)
        {
            this.floatValue = theValue;
            this.expressionType = EPLValueType.floatingType;
            this.boolValue = false;
            this.stringValue = null;
            this.intValue = 0;
            this.enumType = -1;
        }

        /// <summary>
        /// Construct a expression based on an expression.
        /// </summary>
        /// <param name="other">The value to construct.</param>
        public ExpressionValue(ExpressionValue other)
        {
            switch (this.expressionType = other.expressionType)
            {
                case EPLValueType.booleanType:
                    this.boolValue = other.boolValue;
                    this.floatValue = 0;
                    this.stringValue = null;
                    this.intValue = 0;
                    this.enumType = -1;
                    break;
                case EPLValueType.floatingType:
                    this.floatValue = other.floatValue;
                    this.boolValue = false;
                    this.stringValue = null;
                    this.intValue = 0;
                    this.enumType = -1;
                    break;
                case EPLValueType.intType:
                    this.intValue = other.intValue;
                    this.boolValue = false;
                    this.floatValue = 0;
                    this.stringValue = null;
                    this.enumType = -1;
                    break;
                case EPLValueType.stringType:
                    this.stringValue = other.stringValue;
                    this.boolValue = false;
                    this.floatValue = 0;
                    this.intValue = 0;
                    this.enumType = -1;
                    break;
                case EPLValueType.enumType:
                    this.intValue = other.intValue;
                    this.boolValue = false;
                    this.floatValue = 0;
                    this.stringValue = null;
                    this.enumType = other.enumType;
                    break;
                default:
                    throw new EARuntimeError("Unsupported type.");

            }
        }

        /// <summary>
        /// Construct an enum expression.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(int enumType, long theValue)
        {
            this.intValue = theValue;
            this.expressionType = EPLValueType.enumType;
            this.boolValue = false;
            this.floatValue = 0;
            this.stringValue = null;
            this.enumType = enumType;
        }

        /// <summary>
        /// Construct an integer expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(long theValue)
        {
            this.intValue = theValue;
            this.expressionType = EPLValueType.intType;
            this.boolValue = false;
            this.floatValue = 0;
            this.stringValue = null;
            this.enumType = -1;
        }

        /// <summary>
        /// Construct a string expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(String theValue)
        {
            this.stringValue = theValue;
            this.expressionType = EPLValueType.stringType;
            this.boolValue = false;
            this.floatValue = 0;
            this.intValue = 0;
            this.enumType = -1;
        }

        /// <summary>
        /// Construct a value of the specified type.
        /// </summary>
        /// <param name="theType">The value to construct.</param>
        public ExpressionValue(EPLValueType theType)
        {
            this.expressionType = theType;
            this.intValue = 0;
            this.boolValue = false;
            this.floatValue = 0;
            this.stringValue = null;
            this.enumType = -1;
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
        /// The expression type.
        /// </summary>
        public EPLValueType ExprType
        {
            get
            {
                return this.expressionType;
            }
        }

        /// <summary>
        /// True, if this is a boolean.
        /// </summary>
        public bool IsBoolean
        {
            get
            {
                return this.expressionType == EPLValueType.booleanType;
            }
        }

        /// <summary>
        /// True, if this is an enum.
        /// </summary>
        public bool IsEnum
        {
            get
            {
                return this.expressionType == EPLValueType.enumType;
            }
        }

        /// <summary>
        /// True, if this is a float.
        /// </summary>
        public bool IsFloat
        {
            get
            {
                return this.expressionType == EPLValueType.floatingType;
            }
        }

        /// <summary>
        /// True, if this is an int.
        /// </summary>
        public bool IsInt
        {
            get
            {
                return this.expressionType == EPLValueType.intType;
            }
        }

        /// <summary>
        /// True, if the value is either int or float.
        /// </summary>
        public bool IsNumeric
        {
            get
            {
                return IsFloat || IsInt;
            }
        }

        /// <summary>
        /// True, if this is a string.
        /// </summary>
        /// <returns>True, if this is a string.</returns>
        public bool IsString()
        {
            return this.expressionType == EPLValueType.stringType;
        }

        /// <summary>
        /// The value as a boolean, or type mismatch if conversion is not
        /// possible.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ToBooleanValue()
        {
            switch (this.expressionType)
            {
                case EPLValueType.intType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                            + this.intValue + " to boolean.");
                case EPLValueType.floatingType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                            + this.floatValue + " to boolean.");
                case EPLValueType.booleanType:
                    return this.boolValue;
                case EPLValueType.stringType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                            + this.stringValue + " to boolean.");
                case EPLValueType.enumType:
                    throw new EARuntimeError(
                            "Type Mismatch: can't convert enum to boolean.");
                default:
                    throw new EARuntimeError("Unknown type: " + this.expressionType);
            }
        }

        /// <summary>
        /// The value as a float, or type mismatch if conversion is not
        /// possible.
        /// </summary>
        /// <returns>The value.</returns>
        public double ToFloatValue()
        {
            switch (this.expressionType)
            {
                case EPLValueType.intType:
                    return this.intValue;
                case EPLValueType.floatingType:
                    return this.floatValue;
                case EPLValueType.booleanType:
                    throw new EARuntimeError(
                            "Type Mismatch: can't convert float to boolean.");
                case EPLValueType.stringType:
                    try
                    {
                        return Double.Parse(this.stringValue);
                    }
                    catch (FormatException ex)
                    {
                        throw new EARuntimeError("Type Mismatch: can't convert "
                                + this.stringValue + " to floating point.");
                    }
                case EPLValueType.enumType:
                    throw new EARuntimeError(
                            "Type Mismatch: can't convert enum to float.");
                default:
                    throw new EARuntimeError("Unknown type: " + this.expressionType);
            }
        }

        /// <summary>
        /// The value as a int, or type mismatch if conversion is not
        /// possible.
        /// </summary>
        /// <returns>The value.</returns>
        public long ToIntValue()
        {
            switch (this.expressionType)
            {
                case EPLValueType.intType:
                    return this.intValue;
                case EPLValueType.floatingType:
                    return (int)this.floatValue;
                case EPLValueType.booleanType:
                    throw new EARuntimeError(
                            "Type Mismatch: can't convert int to boolean.");
                case EPLValueType.stringType:
                    try
                    {
                        return long.Parse(this.stringValue);
                    }
                    catch (FormatException ex)
                    {
                        throw new EARuntimeError("Type Mismatch: can't convert "
                                + this.stringValue + " to int.");
                    }
                case EPLValueType.enumType:
                    return this.intValue;
                default:
                    throw new EARuntimeError("Unknown type: " + this.expressionType);
            }
        }

        /// <inheritdoc/>
        public string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ExpressionValue: ");
            result.Append("type: ");
            result.Append(ExprType.ToString());
            result.Append(", String Value: ");
            result.Append(ToStringValue());
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// The value as a string, or type mismatch if conversion is not
        /// possible.
        /// </summary>
        /// <returns>The value.</returns>
        public String ToStringValue()
        {
            switch (this.expressionType)
            {
                case EPLValueType.intType:
                    return "" + this.intValue;
                case EPLValueType.floatingType:
                    return "" + this.floatValue;
                case EPLValueType.booleanType:
                    return "" + this.boolValue;
                case EPLValueType.stringType:
                    return this.stringValue;
                case EPLValueType.enumType:
                    return "[" + this.enumType + ":" + this.intValue + "]";
                default:
                    throw new EARuntimeError("Unknown type: " + this.expressionType);
            }
        }
    }
}
