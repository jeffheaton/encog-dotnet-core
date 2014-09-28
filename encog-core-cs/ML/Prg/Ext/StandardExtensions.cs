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
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    ///     Standard operators and functions.
    /// </summary>
    public class StandardExtensions
    {
        /// <summary>
        ///     Variable support.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_VAR_SUPPORT = new BasicTemplate(
            BasicTemplate.NoPrec, "#var():{*}", NodeType.Leaf,
            true, 1, (actual) =>
                {
                    var idx = (int) actual.Data[0].ToIntValue();
                    ExpressionValue result = actual.Owner.Variables.GetVariable(idx);
                    if (result == null)
                    {
                        throw new EARuntimeError("Variable has no value: "
                                                 + actual.Owner.Variables.GetVariableName(idx));
                    }
                    return result;
                }, (context, rtn) =>
                    {
                        foreach (VariableMapping mapping in context.DefinedVariables)
                        {
                            if (mapping.VariableType == rtn)
                            {
                                return true;
                            }
                        }
                        return false;
                    }, (rnd, desiredTypes, actual, minValue, maxValue) =>
                        {
                            int variableIndex = actual.Owner.SelectRandomVariable(rnd, desiredTypes);
                            if (variableIndex == -1)
                            {
                                throw new EncogError("Can't find any variables of type "
                                                     + desiredTypes.ToString() + " to generate.");
                            }
                            actual.Data[0] = new ExpressionValue(variableIndex);
                        });


        /// <summary>
        ///     Numeric const.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CONST_SUPPORT = new BasicTemplate(
            BasicTemplate.NoPrec, "#const():{*}", NodeType.Leaf,
            false, 1, (actual) => { return actual.Data[0]; }, null,
            (rnd, desiredTypes, actual, minValue, maxValue) =>
                {
                    EPLValueType pickedType = desiredTypes[rnd
                                                               .Next(desiredTypes.Count)];
                    EncogProgramContext context = actual.Owner.Context;
                    switch (pickedType)
                    {
                        case EPLValueType.FloatingType:
                            actual.Data[0] = new ExpressionValue(
                                RangeRandomizer.Randomize(rnd, minValue, maxValue));
                            break;
                        case EPLValueType.StringType:
                            // this will be added later
                            break;
                        case EPLValueType.BooleanType:
                            actual.Data[0] = new ExpressionValue(rnd.NextDouble() > 0.5);
                            break;
                        case EPLValueType.IntType:
                            actual.Data[0] = new ExpressionValue(
                                (int) RangeRandomizer
                                          .Randomize(rnd, minValue, maxValue));
                            break;
                        case EPLValueType.EnumType:
                            int enumType = rnd.Next(context.GetMaxEnumType() + 1);
                            int enumCount = context.GetEnumCount(enumType);
                            int enumIndex = rnd.Next(enumCount);
                            actual.Data[0] = new ExpressionValue(enumType, enumIndex);
                            break;
                    }
                });

        /// <summary>
        ///     Standard unary minus operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_NEG = new BasicTemplate(3,
                                                                                  "-({f,i}):{f,i}", NodeType.Unary,
                                                                                  false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              new ExpressionValue(
                                                                                                  -actual.GetChildNode(0)
                                                                                                         .Evaluate()
                                                                                                         .ToFloatValue());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard binary add operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ADD = new BasicTemplate(6,
                                                                                  "+({f,i,s}{f,i,s}):{f,i,s}",
                                                                                  NodeType.OperatorLeft, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              EvaluateExpr.Add(
                                                                                                  actual.GetChildNode(0)
                                                                                                        .Evaluate(),
                                                                                                  actual
                                                                                                      .GetChildNode(1)
                                                                                                      .Evaluate());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard binary sub operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_SUB = new BasicTemplate(6,
                                                                                  "-({f,i}{f,i}):{f,i}",
                                                                                  NodeType.OperatorLeft, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              EvaluateExpr.Sub(
                                                                                                  actual.GetChildNode(0)
                                                                                                        .Evaluate(),
                                                                                                  actual
                                                                                                      .GetChildNode(1)
                                                                                                      .Evaluate());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard binary multiply operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_MUL = new BasicTemplate(5,
                                                                                  "*({f,i}{f,i}):{f,i}",
                                                                                  NodeType.OperatorLeft, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              EvaluateExpr.Mul(
                                                                                                  actual.GetChildNode(0)
                                                                                                        .Evaluate(),
                                                                                                  actual
                                                                                                      .GetChildNode(1)
                                                                                                      .Evaluate());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard binary div operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_DIV = new BasicTemplate(5,
                                                                                  "/({f,i}{f,i}):{f,i}",
                                                                                  NodeType.OperatorLeft, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              EvaluateExpr.Div(
                                                                                                  actual.GetChildNode(0)
                                                                                                        .Evaluate(),
                                                                                                  actual
                                                                                                      .GetChildNode(1)
                                                                                                      .Evaluate());
                                                                                      }, null, null);


        /// <summary>
        ///     Standard binary protected div operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_PDIV = new BasicTemplate(
            5, "%({f,i}{f,i}):{f,i}", NodeType.OperatorLeft, false, 0,
            (actual) => EvaluateExpr.ProtectedDiv(actual.GetChildNode(0).Evaluate(),
                                                  actual.GetChildNode(1).Evaluate())
            , null, null);


        /// <summary>
        ///     Standard binary power operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_POWER = new BasicTemplate(
            1, "^({f,i}{f,i}):{f,i}", NodeType.OperatorRight, false, 0,
            (actual) => EvaluateExpr.Pow(actual.GetChildNode(0).Evaluate(), actual.GetChildNode(1).Evaluate()), null,
            null);


        /// <summary>
        ///     Standard boolean binary and operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_AND = new BasicTemplate(
            10, "&({b}{b}):{b}", NodeType.OperatorLeft, false, 0,
            (actual) =>
                {
                    return new ExpressionValue(actual.GetChildNode(0).Evaluate()
                                                     .ToBooleanValue()
                                               && actual.GetChildNode(1).Evaluate().ToBooleanValue());
                }, null, null);


        /// <summary>
        ///     Standard boolean binary and operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_NOT = new BasicTemplate(3,
                                                                                  "!({b}):{b}", NodeType.Unary, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              new ExpressionValue(
                                                                                                  !actual.GetChildNode(0)
                                                                                                         .Evaluate()
                                                                                                         .ToBooleanValue
                                                                                                       ());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard boolean binary or operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_OR = new BasicTemplate(12,
                                                                                 "|({b}{b}):{b}", NodeType.OperatorLeft,
                                                                                 false, 0,
                                                                                 (actual) =>
                                                                                     {
                                                                                         return
                                                                                             new ExpressionValue(actual
                                                                                                                     .GetChildNode
                                                                                                                     (0)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToBooleanValue
                                                                                                                     ()
                                                                                                                 ||
                                                                                                                 actual
                                                                                                                     .GetChildNode
                                                                                                                     (1)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToBooleanValue
                                                                                                                     ());
                                                                                     }, null, null);


        /// <summary>
        ///     Standard boolean binary equal operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_EQUAL = new BasicTemplate(
            9, "=({*}{*}):{b}", NodeType.OperatorRight, false, 0,
            (actual) =>
                {
                    return EvaluateExpr.Equ(actual.GetChildNode(0).Evaluate(), actual
                                                                                   .GetChildNode(1).Evaluate());
                }, null, null);


        /// <summary>
        ///     Standard boolean not equal operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_NOT_EQUAL = new BasicTemplate(
            9, "<>({*}{*}):{b}", NodeType.OperatorRight, false, 0,
            (actual) =>
                {
                    return EvaluateExpr.Notequ(actual.GetChildNode(0).Evaluate(),
                                               actual.GetChildNode(1).Evaluate());
                }, null, null);

        /// <summary>
        ///     Standard boolean binary greater than operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_GT = new BasicTemplate(8,
                                                                                 ">({i,f}{i,f}):{b}",
                                                                                 NodeType.OperatorRight, false, 0,
                                                                                 (actual) =>
                                                                                     {
                                                                                         return
                                                                                             new ExpressionValue(actual
                                                                                                                     .GetChildNode
                                                                                                                     (0)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToFloatValue
                                                                                                                     () >
                                                                                                                 actual
                                                                                                                     .GetChildNode
                                                                                                                     (1)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToFloatValue
                                                                                                                     ());
                                                                                     }, null, null);

        /// <summary>
        ///     Standard boolean binary less than operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LT = new BasicTemplate(8,
                                                                                 "<({i,f}{i,f}):{b}",
                                                                                 NodeType.OperatorRight, false, 0,
                                                                                 (actual) =>
                                                                                     {
                                                                                         return
                                                                                             new ExpressionValue(actual
                                                                                                                     .GetChildNode
                                                                                                                     (0)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToFloatValue
                                                                                                                     () <
                                                                                                                 actual
                                                                                                                     .GetChildNode
                                                                                                                     (1)
                                                                                                                     .Evaluate
                                                                                                                     ()
                                                                                                                     .ToFloatValue
                                                                                                                     ());
                                                                                     }, null, null);


        /// <summary>
        ///     Standard boolean binary greater than operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_GTE = new BasicTemplate(8,
                                                                                  ">=({i,f}{i,f}):{b}",
                                                                                  NodeType.OperatorRight, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              new ExpressionValue(actual
                                                                                                                      .GetChildNode
                                                                                                                      (0)
                                                                                                                      .Evaluate
                                                                                                                      ()
                                                                                                                      .ToFloatValue
                                                                                                                      () >=
                                                                                                                  actual
                                                                                                                      .GetChildNode
                                                                                                                      (1)
                                                                                                                      .Evaluate
                                                                                                                      ()
                                                                                                                      .ToFloatValue
                                                                                                                      ());
                                                                                      }, null, null);

        /// <summary>
        ///     Standard boolean binary less than operator.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LTE = new BasicTemplate(8,
                                                                                  "<=({i,f}{i,f}):{b}",
                                                                                  NodeType.OperatorRight, false, 0,
                                                                                  (actual) =>
                                                                                      {
                                                                                          return
                                                                                              new ExpressionValue(actual
                                                                                                                      .GetChildNode
                                                                                                                      (0)
                                                                                                                      .Evaluate
                                                                                                                      ()
                                                                                                                      .ToFloatValue
                                                                                                                      () <=
                                                                                                                  actual
                                                                                                                      .GetChildNode
                                                                                                                      (1)
                                                                                                                      .Evaluate
                                                                                                                      ()
                                                                                                                      .ToFloatValue
                                                                                                                      ());
                                                                                      }, null, null);


        /// <summary>
        ///     Standard numeric absolute value function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ABS = new BasicTemplate(
            "abs({f,i}):{f,i}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Abs(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric acos function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ACOS = new BasicTemplate(
            "acos({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Acos(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric asin function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ASIN = new BasicTemplate(
            "asin({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Asin(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric atan function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ATAN = new BasicTemplate(
            "atan({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Atan(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric atan2 function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ATAN2 = new BasicTemplate(
            "atan2({f}{f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Atan2(
                        actual.GetChildNode(0).Evaluate().ToFloatValue(),
                        actual.GetChildNode(1).Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric ceil function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CEIL = new BasicTemplate(
            "ceil({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Ceiling(actual.GetChildNode(0)
                                                                  .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric cos function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_COS = new BasicTemplate(
            "cos({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Cos(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric cosh function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_COSH = new BasicTemplate(
            "cosh({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Cosh(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric exp function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_EXP = new BasicTemplate(
            "exp({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Exp(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric floor function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_FLOOR = new BasicTemplate(
            "floor({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Floor(actual.GetChildNode(0)
                                                                .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric log function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LOG = new BasicTemplate(
            "log({f}):{f}", (actual) =>
                {
                    return new ExpressionValue(Math.Log(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric log10 function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LOG10 = new BasicTemplate(
            "log10({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Log10(actual.GetChildNode(0)
                                                                .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric max function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_MAX = new BasicTemplate(
            "max({f,s,i}({f,s,i}):{f,s,i}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Max(
                        actual.GetChildNode(0).Evaluate().ToFloatValue(),
                        actual.GetChildNode(1).Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric max function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_MIN = new BasicTemplate(
            "min({f,s,i}({f,s,i}):{f,s,i}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Min(
                        actual.GetChildNode(0).Evaluate().ToFloatValue(),
                        actual.GetChildNode(1).Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric pow function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_POWFN = new BasicTemplate(
            "pow({f}{f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Pow(
                        actual.GetChildNode(0).Evaluate().ToFloatValue(),
                        actual.GetChildNode(1).Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric random function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_RANDOM = new BasicTemplate(
            "rand():{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Log(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric log10 function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_ROUND = new BasicTemplate(
            "round({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Round(actual.GetChildNode(0)
                                                                .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric sin function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_SIN = new BasicTemplate(
            "sin({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Sin(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric sinh function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_SINH = new BasicTemplate(
            "sinh({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Sinh(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric sqrt function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_SQRT = new BasicTemplate(
            "sqrt({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Sqrt(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric tan function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_TAN = new BasicTemplate(
            "tan({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Tan(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric tanh function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_TANH = new BasicTemplate(
            "tanh({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Tanh(actual.GetChildNode(0)
                                                               .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard numeric toDegrees function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_TODEG = new BasicTemplate(
            "todeg({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Sin(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });


        /// <summary>
        ///     Standard numeric toRadians function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_TORAD = new BasicTemplate(
            "torad({f}):{f}",
            (actual) =>
                {
                    return new ExpressionValue(Math.Log(actual.GetChildNode(0)
                                                              .Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     Standard string length function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LENGTH = new BasicTemplate(
            "length({s}):{i}",
            (actual) => { return new ExpressionValue(actual.GetChildNode(0).Evaluate().ToStringValue().Length); });

        /// <summary>
        ///     Numeric formatting function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_FORMAT = new BasicTemplate(
            "format({f}{i}):{s}",
            (actual) =>
                {
                    return new ExpressionValue(actual.Owner.Context.Format
                                                     .Format(actual.GetChildNode(0).Evaluate().ToFloatValue(),
                                                             (int) actual.GetChildNode(1).Evaluate().ToFloatValue()));
                });

        /// <summary>
        ///     String left function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_LEFT = new BasicTemplate(
            "left({s}{i}):{s}",
            (actual) =>
                {
                    String str = actual.GetChildNode(0).Evaluate().ToStringValue();
                    var idx = (int) actual.GetChildNode(1).Evaluate().ToFloatValue();
                    String result = str.Substring(0, idx);
                    return new ExpressionValue(result);
                });


        /// <summary>
        ///     String right function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_RIGHT = new BasicTemplate(
            "right({s}{i}):{s}",
            (actual) =>
                {
                    String str = actual.GetChildNode(0).Evaluate().ToStringValue();
                    var idx = (int) actual.GetChildNode(1).Evaluate().ToFloatValue();
                    String result = str.Substring(idx);
                    return new ExpressionValue(result);
                });


        /// <summary>
        ///     Standard string cint function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CINT = new BasicTemplate(
            "cint({f}):{i}",
            (actual) => { return new ExpressionValue(actual.GetChildNode(0).Evaluate().ToIntValue()); });


        /// <summary>
        ///     Standard string cfloat function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CFLOAT = new BasicTemplate(
            "cfloat({i}):{f}",
            (actual) => { return new ExpressionValue(actual.GetChildNode(0).Evaluate().ToFloatValue()); });

        /// <summary>
        ///     Standard string cstr function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CSTR = new BasicTemplate(
            "cstr({*}):{s}",
            (actual) => { return new ExpressionValue(actual.GetChildNode(0).Evaluate().ToStringValue()); });

        /// <summary>
        ///     Standard string cbool function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CBOOL = new BasicTemplate(
            "cbool({i,f}):{b}",
            (actual) => { return new ExpressionValue(actual.GetChildNode(0).Evaluate().ToBooleanValue()); });


        /// <summary>
        ///     Standard string iff function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_IFF = new BasicTemplate(
            "iff({b}:{*}:{*}):{*}",
            (actual) =>
                {
                    bool a = actual.GetChildNode(0).Evaluate().ToBooleanValue();
                    if (a)
                    {
                        return actual.GetChildNode(1).Evaluate();
                    }
                    else
                    {
                        return actual.GetChildNode(2).Evaluate();
                    }
                });


        /// <summary>
        ///     Standard string clamp function.
        /// </summary>
        public static IProgramExtensionTemplate EXTENSION_CLAMP = new BasicTemplate(
            "clamp({f}{f}{f}):{f}",
            (actual) =>
                {
                    bool a = actual.GetChildNode(0).Evaluate().ToBooleanValue();
                    if (a)
                    {
                        return actual.GetChildNode(1).Evaluate();
                    }
                    else
                    {
                        return actual.GetChildNode(2).Evaluate();
                    }
                });

        /// <summary>
        ///     Add all known opcodes to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateAll(EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            foreach (IProgramExtensionTemplate temp in EncogOpcodeRegistry.Instance
                                                                          .FindAllOpcodes())
            {
                factory.AddExtension(temp);
            }
        }

        /// <summary>
        ///     Add the opcodes for basic operations to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateBasicFunctions(EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_ABS);
            factory.AddExtension(EXTENSION_CEIL);
            factory.AddExtension(EXTENSION_EXP);
            factory.AddExtension(EXTENSION_FLOOR);
            factory.AddExtension(EXTENSION_LOG);
            factory.AddExtension(EXTENSION_LOG10);
            factory.AddExtension(EXTENSION_MAX);
            factory.AddExtension(EXTENSION_MIN);
            factory.AddExtension(EXTENSION_POWFN);
            factory.AddExtension(EXTENSION_RANDOM);
            factory.AddExtension(EXTENSION_ROUND);
            factory.AddExtension(EXTENSION_SQRT);
            factory.AddExtension(EXTENSION_CLAMP);
        }

        /// <summary>
        ///     Add the opcodes for boolean operations to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateBooleanOperators(EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_AND);
            factory.AddExtension(EXTENSION_OR);
            factory.AddExtension(EXTENSION_EQUAL);
            factory.AddExtension(EXTENSION_LT);
            factory.AddExtension(EXTENSION_GT);
            factory.AddExtension(EXTENSION_LTE);
            factory.AddExtension(EXTENSION_GTE);
            factory.AddExtension(EXTENSION_IFF);
            factory.AddExtension(EXTENSION_NOT_EQUAL);
            factory.AddExtension(EXTENSION_NOT);
        }

        /// <summary>
        ///     Add the opcodes for type conversion operations to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateConversionFunctions(
            EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_CINT);
            factory.AddExtension(EXTENSION_CFLOAT);
            factory.AddExtension(EXTENSION_CSTR);
            factory.AddExtension(EXTENSION_CBOOL);
        }

        /// <summary>
        ///     Add the opcodes for numeric operations to a context, do not use protected
        ///     division.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateNumericOperators(EncogProgramContext context)
        {
            CreateNumericOperators(context, false);
        }

        /// <summary>
        ///     Add the opcodes for numeric operations to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        /// <param name="protectedDiv">Should protected division be used.</param>
        public static void CreateNumericOperators(
            EncogProgramContext context, bool protectedDiv)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_VAR_SUPPORT);
            factory.AddExtension(EXTENSION_CONST_SUPPORT);
            factory.AddExtension(EXTENSION_NEG);
            factory.AddExtension(EXTENSION_ADD);
            factory.AddExtension(EXTENSION_SUB);
            factory.AddExtension(EXTENSION_MUL);
            if (protectedDiv)
            {
                factory.AddExtension(EXTENSION_PDIV);
            }
            else
            {
                factory.AddExtension(EXTENSION_DIV);
            }
            factory.AddExtension(EXTENSION_POWER);
        }

        /// <summary>
        ///     Add the opcodes for string operations to a context.
        /// </summary>
        /// <param name="context">
        ///     <The context to add the opcodes to./ param>
        public static void CreateStringFunctions(EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_LENGTH);
            factory.AddExtension(EXTENSION_FORMAT);
            factory.AddExtension(EXTENSION_LEFT);
            factory.AddExtension(EXTENSION_RIGHT);
        }

        /// <summary>
        ///     Add the opcodes for trig functions operations to a context.
        /// </summary>
        /// <param name="context">The context to add the opcodes to.</param>
        public static void CreateTrigFunctions(EncogProgramContext context)
        {
            FunctionFactory factory = context.Functions;
            factory.AddExtension(EXTENSION_ACOS);
            factory.AddExtension(EXTENSION_ASIN);
            factory.AddExtension(EXTENSION_ATAN);
            factory.AddExtension(EXTENSION_ATAN2);
            factory.AddExtension(EXTENSION_COS);
            factory.AddExtension(EXTENSION_COSH);
            factory.AddExtension(EXTENSION_SIN);
            factory.AddExtension(EXTENSION_SINH);
            factory.AddExtension(EXTENSION_TAN);
            factory.AddExtension(EXTENSION_TANH);
        }
    }
}
