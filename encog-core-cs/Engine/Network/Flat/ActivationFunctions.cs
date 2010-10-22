/*
 * Encog(tm) Core v2.5 
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 * 
 * Copyright 2008-2010 by Heaton Research Inc.
 * 
 * Released under the LGPL.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 * 
 * Encog and Heaton Research are Trademarks of Heaton Research, Inc.
 * For information on Heaton Research trademarks, visit:
 * 
 * http://www.heatonresearch.com/copyright.html
 */

namespace Encog.Engine.Network.Flat
{

    using Encog.Engine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Util;

    /// <summary>
    /// The activation functions used by the flat networks.
    /// </summary>
    ///
    public sealed class ActivationFunctions
    {

        /// <summary>
        /// The names of all of the params.
        /// </summary>
        ///
        public static readonly String[][] PARAM_NAMES = { new String[] { "slope" }, new String[] { "slope" },
				new String[] { "slope" }, new String[] {}, new String[] {},
				new String[] { "center", "low", "high" },
				new String[] { "thresholdHigh", "thresholdLow", "high", "low" },
				new String[] { "maxWinners" }, new String[] {}, new String[] {},
				new String[] { "center", "peak", "width" } };

        /// <summary>
        /// The offset to the parameter that holds the linear slope.
        /// </summary>
        ///
        public const int PARAM_LINEAR_SLOPE = 0;

        /// <summary>
        /// The offset to the parameter that holds the tanh slope.
        /// </summary>
        ///
        public const int PARAM_TANH_SLOPE = 0;

        /// <summary>
        /// The offset to the parameter that holds the sigmoid slope.
        /// </summary>
        ///
        public const int PARAM_SIGMOID_SLOPE = 0;

        /// <summary>
        /// The offset to the parameter that holds the max winners.
        /// </summary>
        ///
        public const int PARAM_COMPETITIVE_MAX_WINNERS = 0;

        /// <summary>
        /// The step center parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_CENTER = 0;

        /// <summary>
        /// The step low parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_LOW = 1;

        /// <summary>
        /// The step high parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_HIGH = 2;

        /// <summary>
        /// The ramp high threshold parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_HIGH_THRESHOLD = 0;

        /// <summary>
        /// The ramp low threshold parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_LOW_THRESHOLD = 1;

        /// <summary>
        /// The ramp high parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_HIGH = 2;

        /// <summary>
        /// The ramp low parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_LOW = 3;

        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_CENTER = 0;

        /// <summary>
        /// The offset to the parameter that holds the peak.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_PEAK = 1;

        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_WIDTH = 2;

        /// <summary>
        /// A linear activation function.
        /// </summary>
        ///
        public const int ACTIVATION_LINEAR = 0;

        /// <summary>
        /// A TANH activation function.
        /// </summary>
        ///
        public const int ACTIVATION_TANH = 1;

        /// <summary>
        /// A sigmoid activation function.
        /// </summary>
        ///
        public const int ACTIVATION_SIGMOID = 2;

        /// <summary>
        /// A Soft Max activation function.
        /// </summary>
        ///
        public const int ACTIVATION_SOFTMAX = 3;

        /// <summary>
        /// A Bipolar activation function.
        /// </summary>
        ///
        public const int ACTIVATION_BIPOLAR = 4;

        /// <summary>
        /// A Step activation function.
        /// </summary>
        ///
        public const int ACTIVATION_STEP = 5;

        /// <summary>
        /// A Ramp activation function.
        /// </summary>
        ///
        public const int ACTIVATION_RAMP = 6;

        /// <summary>
        /// A Competitive activation function.
        /// </summary>
        ///
        public const int ACTIVATION_COMPETITIVE = 7;

        /// <summary>
        /// A Bipolar activation function.
        /// </summary>
        ///
        public const int ACTIVATION_SIN = 8;

        /// <summary>
        /// A Log activation function.
        /// </summary>
        ///
        public const int ACTIVATION_LOG = 9;

        /// <summary>
        /// A Gaussian activation function.
        /// </summary>
        ///
        public const int ACTIVATION_GAUSSIAN = 10;

        /// <summary>
        /// Calculate an activation.
        /// </summary>
        ///
        /// <param name="type"/>The type of activation.</param>
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivation(int type, double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            switch (type)
            {
                case ActivationFunctions.ACTIVATION_LINEAR:
                    ActivationFunctions.CalculateActivationLINEAR(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ActivationFunctions.ACTIVATION_TANH:
                    ActivationFunctions.CalculateActivationTANH(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ActivationFunctions.ACTIVATION_SIGMOID:
                    ActivationFunctions.CalculateActivationSIGMOID(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_SOFTMAX:
                    ActivationFunctions.CalculateActivationSOFTMAX(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_BIPOLAR:
                    ActivationFunctions.CalculateActivationBIPOLAR(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_STEP:
                    ActivationFunctions.CalculateActivationSTEP(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_RAMP:
                    ActivationFunctions.CalculateActivationRAMP(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_COMPETITIVE:
                    ActivationFunctions.CalculateActivationCOMPETITIVE(x, paras,
                            xOffset, xLength, paramOffset);
                    break;
                case ACTIVATION_SIN:
                    ActivationFunctions.CalculateActivationSIN(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_LOG:
                    ActivationFunctions.CalculateActivationLOG(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                case ACTIVATION_GAUSSIAN:
                    ActivationFunctions.CalculateActivationGAUSSIAN(x, paras, xOffset,
                            xLength, paramOffset);
                    break;
                default:
                    throw new EncogEngineError("Unknown activation type: " + type);
            }
        }

        /// <summary>
        /// Calculate the bipolar activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationBIPOLAR(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                if (x[i] > 0)
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
            }
        }

        /// <summary>
        /// Calculate the competitive activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationCOMPETITIVE(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {

            bool[] winners = new bool[xLength];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < paras[paramOffset]; i++)
            {
                double maxFound = System.Double.NegativeInfinity;
                int winner = -1;

                // find one winner
                for (int j = 0; j < xLength; j++)
                {
                    if (!winners[j] && (x[xOffset + j] > maxFound))
                    {
                        winner = j;
                        maxFound = x[xOffset + j];
                    }
                }
                sumWinners += maxFound;
                winners[winner] = true;
            }

            // adjust weights for winners and non-winners
            for (int i_0 = 0; i_0 < xLength; i_0++)
            {
                if (winners[i_0])
                {
                    x[xOffset + i_0] = x[xOffset + i_0] / sumWinners;
                }
                else
                {
                    x[xOffset + i_0] = 0.0d;
                }
            }
        }

        /// <summary>
        /// Calculate the derivative of the activation. It is assumed that the value
        /// x, which is passed to this method, was the output from this activation.
        /// This prevents this method from having to recalculate the activation, just
        /// to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="type"/>The type of activation.</param>
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivative(int type,
                double x, double[] paras, int paramOffset)
        {
            switch (type)
            {
                case ActivationFunctions.ACTIVATION_LINEAR:
                    return ActivationFunctions.CalculateActivationDerivativeLINEAR(x,
                            paras, paramOffset);
                case ActivationFunctions.ACTIVATION_TANH:
                    return ActivationFunctions.CalculateActivationDerivativeTANH(x,
                            paras, paramOffset);
                case ActivationFunctions.ACTIVATION_SIGMOID:
                    return ActivationFunctions.CalculateActivationDerivativeSIGMOID(x,
                            paras, paramOffset);
                case ACTIVATION_SOFTMAX:
                    return ActivationFunctions.CalculateActivationDerivativeSOFTMAX(x,
                            paras, paramOffset);
                case ACTIVATION_BIPOLAR:
                    return ActivationFunctions.CalculateActivationDerivativeBIPOLAR(x,
                            paras, paramOffset);
                case ACTIVATION_STEP:
                    return ActivationFunctions.CalculateActivationDerivativeSTEP(x,
                            paras, paramOffset);
                case ACTIVATION_RAMP:
                    return ActivationFunctions.CalculateActivationDerivativeRAMP(x,
                            paras, paramOffset);
                case ACTIVATION_COMPETITIVE:
                    return ActivationFunctions
                            .CalculateActivationDerivativeCOMPETITIVE(x, paras,
                                    paramOffset);
                case ACTIVATION_SIN:
                    return ActivationFunctions.CalculateActivationDerivativeSIN(x,
                            paras, paramOffset);
                case ACTIVATION_LOG:
                    return ActivationFunctions.CalculateActivationDerivativeLOG(x,
                            paras, paramOffset);
                case ACTIVATION_GAUSSIAN:
                    return ActivationFunctions.CalculateActivationDerivativeGAUSSIAN(x,
                            paras, paramOffset);
                default:
                    throw new EncogEngineError("Unknown activation type: " + type);
            }
        }

        /// <summary>
        /// Calculate the derivative for bipolar activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeBIPOLAR(double x,
                double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for competitive activation. It is assumed that
        /// the value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeCOMPETITIVE(
                double x, double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for gaussian activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeGAUSSIAN(double x,
                double[] paras, int paramOffset)
        {
            double width = paras[ActivationFunctions.PARAM_GAUSSIAN_WIDTH];
            double peak = paras[ActivationFunctions.PARAM_GAUSSIAN_PEAK];
            return Math.Exp(-0.5d * width * width * x * x) * peak * width * width
                    * (width * width * x * x - 1);
        }

        /// <summary>
        /// Calculate the derivative for linear activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeLINEAR(double x,
                double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for log activation. It is assumed that the value
        /// x, which is passed to this method, was the output from this activation.
        /// This prevents this method from having to recalculate the activation, just
        /// to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeLOG(double x,
                double[] paras, int paramOffset)
        {
            if (x >= 0)
            {
                return 1 / (1 + x);
            }
            else
            {
                return 1 / (1 - x);
            }
        }

        /// <summary>
        /// Calculate the derivative for ramp activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeRAMP(double x,
                double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for sigmoid activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeSIGMOID(double x,
                double[] paras, int paramOffset)
        {
            return paras[paramOffset] * x * (1.0d - x);
        }

        /// <summary>
        /// Calculate the derivative for sin activation. It is assumed that the value
        /// x, which is passed to this method, was the output from this activation.
        /// This prevents this method from having to recalculate the activation, just
        /// to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeSIN(double x,
                double[] paras, int paramOffset)
        {
            return BoundMath.Cos(x);
        }

        /// <summary>
        /// Calculate the derivative for softmax activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeSOFTMAX(double x,
                double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for step activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeSTEP(double x,
                double[] paras, int paramOffset)
        {
            return 1;
        }

        /// <summary>
        /// Calculate the derivative for tanh activation. It is assumed that the
        /// value x, which is passed to this method, was the output from this
        /// activation. This prevents this method from having to recalculate the
        /// activation, just to recalculate the derivative.
        /// </summary>
        ///
        /// <param name="x"/>The activation to calculate for.</param>
        /// <param name="params"/>The parameters for this activation function.</param>
        /// <param name="paramOffset"/>The offset the parameters begin at.</param>
        /// <returns>The derivative.</returns>
        public static double CalculateActivationDerivativeTANH(double x,
                double[] paras, int paramOffset)
        {
            return (paras[paramOffset] * (1.0d - x * x));
        }

        /// <summary>
        /// Calculate the gaussian activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationGAUSSIAN(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                x[i] = paras[ActivationFunctions.PARAM_GAUSSIAN_PEAK]
                        * BoundMath
                                .Exp(-Math.Pow(x[i]
                                                                            - paras[ActivationFunctions.PARAM_GAUSSIAN_CENTER], 2)
                                        / (2.0d * paras[ActivationFunctions.PARAM_GAUSSIAN_WIDTH] * paras[ActivationFunctions.PARAM_GAUSSIAN_WIDTH]));
            }
        }

        /// <summary>
        /// Calculate the linear activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationLINEAR(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                x[i] = x[i] * paras[paramOffset];
            }
        }

        /// <summary>
        /// Calculate the log activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationLOG(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                if (x[i] >= 0)
                {
                    x[i] = BoundMath.Log(1 + x[i]);
                }
                else
                {
                    x[i] = -BoundMath.Log(1 - x[i]);
                }
            }

        }

        /// <summary>
        /// Calculate the ramp activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationRAMP(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            double slope = (paras[ActivationFunctions.PARAM_RAMP_HIGH_THRESHOLD] - paras[ActivationFunctions.PARAM_RAMP_LOW_THRESHOLD])
                    / (paras[ActivationFunctions.PARAM_RAMP_HIGH] - paras[ActivationFunctions.PARAM_RAMP_LOW]);

            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                if (x[i] < paras[ActivationFunctions.PARAM_RAMP_LOW_THRESHOLD])
                {
                    x[i] = paras[ActivationFunctions.PARAM_RAMP_LOW];
                }
                else if (x[i] > paras[ActivationFunctions.PARAM_RAMP_HIGH_THRESHOLD])
                {
                    x[i] = paras[ActivationFunctions.PARAM_RAMP_HIGH];
                }
                else
                {
                    x[i] = (slope * x[i]);
                }
            }
        }

        /// <summary>
        /// Calculate the sigmoid activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationSIGMOID(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                x[i] = 1.0d / (1.0d + BoundMath.Exp(-paras[paramOffset] * x[i]));
            }
        }

        /// <summary>
        /// Calculate the sin activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationSIN(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                x[i] = BoundMath.Sin(x[i]);
            }

        }

        /// <summary>
        /// Calculate the softmax activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationSOFTMAX(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {

            double sum = 0;
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                x[i] = BoundMath.Exp(x[i]);
                sum += x[i];
            }
            for (int i_0 = xOffset; i_0 < xOffset + xLength; i_0++)
            {
                x[i_0] = x[i_0] / sum;
            }
        }

        /// <summary>
        /// Calculate the step activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationSTEP(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {

            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                if (x[i] >= paras[ActivationFunctions.PARAM_STEP_CENTER])
                {
                    x[i] = paras[ActivationFunctions.PARAM_STEP_HIGH];
                }
                else
                {
                    x[i] = paras[ActivationFunctions.PARAM_STEP_LOW];
                }
            }
        }

        /// <summary>
        /// Calculate the tanh activation.
        /// </summary>
        ///
        /// <param name="x"/>The input and output array. Input values are provided and thearray is modified to reflect the output.</param>
        /// <param name="params"/>THe parameters needed for the calculation.</param>
        /// <param name="xOffset"/>The offset into X for where we are calculating.</param>
        /// <param name="xLength"/>The length of the array we are calculating.</param>
        /// <param name="paramOffset"/>The parameter offset.</param>
        public static void CalculateActivationTANH(double[] x,
                double[] paras, int xOffset, int xLength,
                int paramOffset)
        {
            for (int i = xOffset; i < xOffset + xLength; i++)
            {
                double z = BoundMath.Exp(-paras[paramOffset] * x[i]);
                x[i] = (1.0d - z) / (1.0d + z);
            }
        }

        /// <summary>
        /// Copy parameters. Accounts for variable sized parameters.
        /// </summary>
        ///
        /// <param name="source"/>The source.</param>
        /// <param name="target"/>The target.</param>
        /// <param name="index"/>The index.</param>
        /// <returns>The new index.</returns>
        public static int CopyParams(double[] source, double[] target,
                int index)
        {
            EngineArray.ArrayCopy(source, 0, target, index, source.Length);
            return index + source.Length;
        }

        /// <summary>
        /// Get the parameter names for an activation function.
        /// </summary>
        ///
        /// <param name="index"/>The activation function.</param>
        /// <returns>An array of names.</returns>
        public static String[] GetParams(int index)
        {
            return ActivationFunctions.PARAM_NAMES[index];
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private ActivationFunctions()
        {

        }
    }
}
