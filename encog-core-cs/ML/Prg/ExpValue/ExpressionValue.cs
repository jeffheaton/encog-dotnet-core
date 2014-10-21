//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.Text;
using Encog.ML.EA.Exceptions;

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    ///     An EncogProgram expression value. These is how Encog stores variables and
    ///     calculates values.
    /// </summary>
    [Serializable]
    public class ExpressionValue
    {
        /// <summary>
        ///     If the value is a boolean, this contains the value.
        /// </summary>
        private readonly bool _boolValue;

        /// <summary>
        ///     If the value is an enum, this contains the value.
        /// </summary>
        private readonly int _enumType;

        /// <summary>
        ///     The type of this expression.
        /// </summary>
        private readonly EPLValueType _expressionType;

        /// <summary>
        ///     If the value is a float, this contains the value.
        /// </summary>
        private readonly double _floatValue;

        /// <summary>
        ///     If the value is an int, this contains the value.
        /// </summary>
        private readonly long _intValue;

        /// <summary>
        ///     If the value is a string, this contains the value.
        /// </summary>
        private readonly String _stringValue;

        /// <summary>
        ///     Construct a boolean expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(bool theValue)
        {
            _boolValue = theValue;
            _expressionType = EPLValueType.BooleanType;
            _floatValue = 0;
            _stringValue = null;
            _intValue = 0;
            _enumType = -1;
        }

        /// <summary>
        ///     Construct a boolean expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(double theValue)
        {
            _floatValue = theValue;
            _expressionType = EPLValueType.FloatingType;
            _boolValue = false;
            _stringValue = null;
            _intValue = 0;
            _enumType = -1;
        }

        /// <summary>
        ///     Construct a expression based on an expression.
        /// </summary>
        /// <param name="other">The value to construct.</param>
        public ExpressionValue(ExpressionValue other)
        {
            switch (_expressionType = other._expressionType)
            {
                case EPLValueType.BooleanType:
                    _boolValue = other._boolValue;
                    _floatValue = 0;
                    _stringValue = null;
                    _intValue = 0;
                    _enumType = -1;
                    break;
                case EPLValueType.FloatingType:
                    _floatValue = other._floatValue;
                    _boolValue = false;
                    _stringValue = null;
                    _intValue = 0;
                    _enumType = -1;
                    break;
                case EPLValueType.IntType:
                    _intValue = other._intValue;
                    _boolValue = false;
                    _floatValue = 0;
                    _stringValue = null;
                    _enumType = -1;
                    break;
                case EPLValueType.StringType:
                    _stringValue = other._stringValue;
                    _boolValue = false;
                    _floatValue = 0;
                    _intValue = 0;
                    _enumType = -1;
                    break;
                case EPLValueType.EnumType:
                    _intValue = other._intValue;
                    _boolValue = false;
                    _floatValue = 0;
                    _stringValue = null;
                    _enumType = other._enumType;
                    break;
                default:
                    throw new EARuntimeError("Unsupported type.");
            }
        }

        /// <summary>
        ///     Construct an enum expression.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(int enumType, long theValue)
        {
            _intValue = theValue;
            _expressionType = EPLValueType.EnumType;
            _boolValue = false;
            _floatValue = 0;
            _stringValue = null;
            _enumType = enumType;
        }

        /// <summary>
        ///     Construct an integer expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(long theValue)
        {
            _intValue = theValue;
            _expressionType = EPLValueType.IntType;
            _boolValue = false;
            _floatValue = 0;
            _stringValue = null;
            _enumType = -1;
        }

        /// <summary>
        ///     Construct a string expression.
        /// </summary>
        /// <param name="theValue">The value to construct.</param>
        public ExpressionValue(String theValue)
        {
            _stringValue = theValue;
            _expressionType = EPLValueType.StringType;
            _boolValue = false;
            _floatValue = 0;
            _intValue = 0;
            _enumType = -1;
        }

        /// <summary>
        ///     Construct a value of the specified type.
        /// </summary>
        /// <param name="theType">The value to construct.</param>
        public ExpressionValue(EPLValueType theType)
        {
            _expressionType = theType;
            _intValue = 0;
            _boolValue = false;
            _floatValue = 0;
            _stringValue = null;
            _enumType = -1;
        }

        /// <summary>
        ///     The enum type.
        /// </summary>
        public int EnumType
        {
            get { return _enumType; }
        }

        /// <summary>
        ///     The expression type.
        /// </summary>
        public EPLValueType ExprType
        {
            get { return _expressionType; }
        }

        /// <summary>
        ///     True, if this is a boolean.
        /// </summary>
        public bool IsBoolean
        {
            get { return _expressionType == EPLValueType.BooleanType; }
        }

        /// <summary>
        ///     True, if this is an enum.
        /// </summary>
        public bool IsEnum
        {
            get { return _expressionType == EPLValueType.EnumType; }
        }

        /// <summary>
        ///     True, if this is a float.
        /// </summary>
        public bool IsFloat
        {
            get { return _expressionType == EPLValueType.FloatingType; }
        }

        /// <summary>
        ///     True, if this is an int.
        /// </summary>
        public bool IsInt
        {
            get { return _expressionType == EPLValueType.IntType; }
        }

        /// <summary>
        ///     True, if the value is either int or float.
        /// </summary>
        public bool IsNumeric
        {
            get { return IsFloat || IsInt; }
        }

        /// <summary>
        ///     True, if this is a string.
        /// </summary>
        public bool IsString
        {
            get { return _expressionType == EPLValueType.StringType; }
        }

        /// <summary>
        ///     The value as a boolean, or type mismatch if conversion is not
        ///     possible.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ToBooleanValue()
        {
            switch (_expressionType)
            {
                case EPLValueType.IntType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                                             + _intValue + " to boolean.");
                case EPLValueType.FloatingType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                                             + _floatValue + " to boolean.");
                case EPLValueType.BooleanType:
                    return _boolValue;
                case EPLValueType.StringType:
                    throw new EARuntimeError("Type Mismatch: can't convert "
                                             + _stringValue + " to boolean.");
                case EPLValueType.EnumType:
                    throw new EARuntimeError(
                        "Type Mismatch: can't convert enum to boolean.");
                default:
                    throw new EARuntimeError("Unknown type: " + _expressionType);
            }
        }

        /// <summary>
        ///     The value as a float, or type mismatch if conversion is not
        ///     possible.
        /// </summary>
        /// <returns>The value.</returns>
        public double ToFloatValue()
        {
            switch (_expressionType)
            {
                case EPLValueType.IntType:
                    return _intValue;
                case EPLValueType.FloatingType:
                    return _floatValue;
                case EPLValueType.BooleanType:
                    throw new EARuntimeError(
                        "Type Mismatch: can't convert float to boolean.");
                case EPLValueType.StringType:
                    try
                    {
                        return Double.Parse(_stringValue);
                    }
                    catch (FormatException)
                    {
                        throw new EARuntimeError("Type Mismatch: can't convert "
                                                 + _stringValue + " to floating point.");
                    }
                case EPLValueType.EnumType:
                    throw new EARuntimeError(
                        "Type Mismatch: can't convert enum to float.");
                default:
                    throw new EARuntimeError("Unknown type: " + _expressionType);
            }
        }

        /// <summary>
        ///     The value as a int, or type mismatch if conversion is not
        ///     possible.
        /// </summary>
        /// <returns>The value.</returns>
        public long ToIntValue()
        {
            switch (_expressionType)
            {
                case EPLValueType.IntType:
                    return _intValue;
                case EPLValueType.FloatingType:
                    return (int) _floatValue;
                case EPLValueType.BooleanType:
                    throw new EARuntimeError(
                        "Type Mismatch: can't convert int to boolean.");
                case EPLValueType.StringType:
                    try
                    {
                        return long.Parse(_stringValue);
                    }
                    catch (FormatException)
                    {
                        throw new EARuntimeError("Type Mismatch: can't convert "
                                                 + _stringValue + " to int.");
                    }
                case EPLValueType.EnumType:
                    return _intValue;
                default:
                    throw new EARuntimeError("Unknown type: " + _expressionType);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[ExpressionValue: ");
            result.Append("type: ");
            result.Append(ExprType.ToString());
            result.Append(", String Value: ");
            result.Append(ToStringValue());
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     The value as a string, or type mismatch if conversion is not
        ///     possible.
        /// </summary>
        /// <returns>The value.</returns>
        public String ToStringValue()
        {
            switch (_expressionType)
            {
                case EPLValueType.IntType:
                    return "" + _intValue;
                case EPLValueType.FloatingType:
                    return "" + _floatValue;
                case EPLValueType.BooleanType:
                    return "" + _boolValue;
                case EPLValueType.StringType:
                    return _stringValue;
                case EPLValueType.EnumType:
                    return "[" + _enumType + ":" + _intValue + "]";
                default:
                    throw new EARuntimeError("Unknown type: " + _expressionType);
            }
        }
    }
}
