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

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    ///     Simple utility class that performs some basic operations on ExpressionValue
    ///     objects.
    /// </summary>
    public static class EvaluateExpr
    {
        /// <summary>
        ///     Perform an add on two expression values. a+b
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>
        ///     The result of adding two numbers. Concat for strings. If one is a
        ///     string, the other is converted to string. If no string, then if
        ///     one is float, both are converted to int.
        /// </returns>
        public static ExpressionValue Add(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.IsString || b.IsString)
            {
                return new ExpressionValue(a.ToStringValue() + b.ToStringValue());
            }
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(a.ToIntValue() + b.ToIntValue());
            }
            return new ExpressionValue(a.ToFloatValue() + b.ToFloatValue());
        }

        /// <summary>
        ///     Perform a division on two expression values. a/b An Encog division by
        ///     zero exception can occur. If one param is a float, the other is converted
        ///     to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns> The result of the operation.</returns>
        public static ExpressionValue Div(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                long i = b.ToIntValue();
                if (i == 0)
                {
                    throw new DivisionByZeroError();
                }
                return new ExpressionValue(a.ToIntValue()/i);
            }

            double denom = b.ToFloatValue();

            if (Math.Abs(denom) < EncogFramework.DefaultDoubleEqual)
            {
                throw new DivisionByZeroError();
            }

            return new ExpressionValue(a.ToFloatValue()/denom);
        }

        /// <summary>
        ///     Perform an equal on two expressions. Booleans, ints and strings must
        ///     exactly equal. Floating point must be equal within the default Encog
        ///     tolerance.
        /// </summary>
        /// <param name="a">The first parameter to check.</param>
        /// <param name="b">The second parameter to check.</param>
        /// <returns>True/false.</returns>
        public static ExpressionValue Equ(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.ExprType == EPLValueType.BooleanType)
            {
                return new ExpressionValue(a.ToBooleanValue() == b.ToBooleanValue());
            }
            if (a.ExprType == EPLValueType.EnumType)
            {
                return new ExpressionValue(a.ToIntValue() == b.ToIntValue()
                                           && a.EnumType == b.EnumType);
            }
            if (a.ExprType == EPLValueType.StringType)
            {
                return new ExpressionValue(a.ToStringValue().Equals(
                    b.ToStringValue()));
            }
            var diff = Math.Abs(a.ToFloatValue() - b.ToFloatValue());
            return new ExpressionValue(diff < EncogFramework.DefaultDoubleEqual);
        }

        /// <summary>
        ///     Perform a multiply on two expression values. a*b If one param is a float,
        ///     the other is converted to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue Mul(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(a.ToIntValue()*b.ToIntValue());
            }
            return new ExpressionValue(a.ToFloatValue()*b.ToFloatValue());
        }


        /// <summary>
        ///     Perform a non-equal on two expressions. Booleans, ints and strings must
        ///     exactly non-equal. Floating point must be non-equal within the default
        ///     Encog tolerance.
        /// </summary>
        /// <param name="a">The first parameter to check.</param>
        /// <param name="b">The second parameter to check.</param>
        /// <returns>True/false.</returns>
        public static ExpressionValue Notequ(ExpressionValue a,
                                             ExpressionValue b)
        {
            if (a.ExprType == EPLValueType.BooleanType)
            {
                return new ExpressionValue(a.ToBooleanValue() != b.ToBooleanValue());
            }
            if (a.ExprType == EPLValueType.EnumType)
            {
                return new ExpressionValue(a.ToIntValue() != b.ToIntValue()
                                           && a.EnumType == b.EnumType);
            }
            if (a.ExprType == EPLValueType.StringType)
            {
                return new ExpressionValue(!a.ToStringValue().Equals(
                    b.ToStringValue()));
            }
            double diff = Math.Abs(a.ToFloatValue() - b.ToFloatValue());
            return new ExpressionValue(diff > EncogFramework.DefaultDoubleEqual);
        }

        /// <summary>
        ///     Perform a protected div on two expression values. a/b If one param is a
        ///     float, the other is converted to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue Pow(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(Math.Pow(a.ToIntValue(), b.ToIntValue()));
            }
            return new ExpressionValue(Math.Pow(a.ToFloatValue(), b.ToFloatValue()));
        }

        /// <summary>
        ///     Perform a protected div on two expression values. a/b Division by zero
        ///     results in 1.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue ProtectedDiv(ExpressionValue a,
                                                   ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                long i = b.ToIntValue();
                if (i == 0)
                {
                    return new ExpressionValue(1);
                }
                return new ExpressionValue(a.ToIntValue()/i);
            }

            double denom = b.ToFloatValue();

            if (Math.Abs(denom) < EncogFramework.DefaultDoubleEqual)
            {
                return new ExpressionValue(1);
            }

            return new ExpressionValue(a.ToFloatValue()/denom);
        }

        /// <summary>
        ///     Perform a subtract on two expression values. a-b If one param is a float,
        ///     the other is converted to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue Sub(ExpressionValue a,
                                          ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(a.ToIntValue() - b.ToIntValue());
            }
            return new ExpressionValue(a.ToFloatValue() - b.ToFloatValue());
        }
    }
}
