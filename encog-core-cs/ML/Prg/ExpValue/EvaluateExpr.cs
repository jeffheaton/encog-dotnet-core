using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    /// Simple utility class that performs some basic operations on ExpressionValue
    /// objects.
    /// </summary>
    public sealed class EvaluateExpr
    {
        /// <summary>
        /// Perform an add on two expression values. a+b
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The result of adding two numbers. Concat for strings. If one is a
        /// string, the other is converted to string. If no string, then if
        /// one is float, both are converted to int.</returns>
        public static ExpressionValue Add(ExpressionValue a,
                ExpressionValue b)
        {
            if (a.IsString() || b.IsString())
            {
                return new ExpressionValue(a.ToStringValue() + b.ToStringValue());
            }
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(a.ToIntValue() + b.ToIntValue());
            }
            else
            {
                return new ExpressionValue(a.ToFloatValue() + b.ToFloatValue());
            }
        }

        /// <summary>
        /// Perform a division on two expression values. a/b An Encog division by
        /// zero exception can occur. If one param is a float, the other is converted
        /// to a float.
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
                return new ExpressionValue(a.ToIntValue() / i);
            }

            double denom = b.ToFloatValue();

            if (Math.Abs(denom) < EncogFramework.DefaultDoubleEqual)
            {
                throw new DivisionByZeroError();
            }

            return new ExpressionValue(a.ToFloatValue() / denom);
        }

        /// <summary>
        /// Perform an equal on two expressions. Booleans, ints and strings must
        /// exactly equal. Floating point must be equal within the default Encog
        /// tolerance.
        /// </summary>
        /// <param name="a">The first parameter to check.</param>
        /// <param name="b">The second parameter to check.</param>
        /// <returns>True/false.</returns>
        public static ExpressionValue Equ(ExpressionValue a,
            ExpressionValue b)
        {

            if (a.ExprType == EPLValueType.booleanType)
            {
                return new ExpressionValue(a.ToBooleanValue() == b.ToBooleanValue());
            }
            else if (a.ExprType == EPLValueType.enumType)
            {
                return new ExpressionValue(a.ToIntValue() == b.ToIntValue()
                        && a.EnumType == b.EnumType);
            }
            else if (a.ExprType == EPLValueType.stringType)
            {
                return new ExpressionValue(a.ToStringValue().Equals(
                        b.ToStringValue()));
            }
            else
            {
                double diff = Math.Abs(a.ToFloatValue() - b.ToFloatValue());
                return new ExpressionValue(diff < EncogFramework.DefaultDoubleEqual);
            }
        }

        /// <summary>
        /// Perform a multiply on two expression values. a*b If one param is a float,
        /// the other is converted to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue Mul(ExpressionValue a,
                ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(a.ToIntValue() * b.ToIntValue());
            }
            return new ExpressionValue(a.ToFloatValue() * b.ToFloatValue());
        }

        /**
         * 
         * 
         * @param a
         *            
         * @param b
         *            
         * @return 
         */

        /// <summary>
        /// Perform a non-equal on two expressions. Booleans, ints and strings must
        /// exactly non-equal. Floating point must be non-equal within the default
        /// Encog tolerance.
        /// </summary>
        /// <param name="a">The first parameter to check.</param>
        /// <param name="b">The second parameter to check.</param>
        /// <returns>True/false.</returns>
        public static ExpressionValue notequ(ExpressionValue a,
                ExpressionValue b)
        {
            if (a.ExprType == EPLValueType.booleanType)
            {
                return new ExpressionValue(a.ToBooleanValue() != b.ToBooleanValue());
            }
            else if (a.ExprType == EPLValueType.enumType)
            {
                return new ExpressionValue(a.ToIntValue() != b.ToIntValue()
                        && a.EnumType == b.EnumType);
            }
            else if (a.ExprType == EPLValueType.stringType)
            {
                return new ExpressionValue(!a.ToStringValue().Equals(
                        b.ToStringValue()));
            }
            else
            {
                double diff = Math.Abs(a.ToFloatValue() - b.ToFloatValue());
                return new ExpressionValue(diff > EncogFramework.DefaultDoubleEqual);
            }
        }

        /// <summary>
        /// Perform a protected div on two expression values. a/b If one param is a
        /// float, the other is converted to a float.
        /// </summary>
        /// <param name="a">The first argument, must be numeric.</param>
        /// <param name="b">The second argument, must be numeric.</param>
        /// <returns>The result of the operation.</returns>
        public static ExpressionValue pow(ExpressionValue a,
                ExpressionValue b)
        {
            if (a.IsInt && b.IsInt)
            {
                return new ExpressionValue(Math.Pow(a.ToIntValue(), b.ToIntValue()));
            }
            return new ExpressionValue(Math.Pow(a.ToFloatValue(), b.ToFloatValue()));
        }

        /// <summary>
        /// Perform a protected div on two expression values. a/b Division by zero
        /// results in 1.
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
                return new ExpressionValue(a.ToIntValue() / i);
            }

            double denom = b.ToFloatValue();

            if (Math.Abs(denom) < EncogFramework.DefaultDoubleEqual)
            {
                return new ExpressionValue(1);
            }

            return new ExpressionValue(a.ToFloatValue() / denom);
        }

        /// <summary>
        /// Perform a subtract on two expression values. a-b If one param is a float,
        /// the other is converted to a float.
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
            else
            {
                return new ExpressionValue(a.ToFloatValue() - b.ToFloatValue());
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private EvaluateExpr()
        {

        }
    }
}
