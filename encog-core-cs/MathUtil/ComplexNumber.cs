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
using Encog.Util;

namespace Encog.MathUtil
{
    /// <summary>
    /// A complex number class.  This class is based on source code by
    /// 
    /// Andrew G. Bennett, Department of Mathematics
    /// Kansas State University
    /// 
    /// The original version can be found here:
    /// 
    /// http://www.math.ksu.edu/~bennett/jomacg/c.html
    ///
    /// </summary>
    public class ComplexNumber
    {
        /// <summary>
        /// The real part.
        /// </summary>
        private readonly double _x;

        /// <summary>
        /// The imaginary part.
        /// </summary>
        private readonly double _y;

        /// <summary>
        /// Constructs the complex number z = u + i*v
        /// </summary>
        /// <param name="u">Real part</param>
        /// <param name="v">Imaginary part</param>
        public ComplexNumber(double u, double v)
        {
            _x = u;
            _y = v;
        }

        /// <summary>
        /// Create a complex number from another complex number. 
        /// </summary>
        /// <param name="other">The other complex number. </param>
        public ComplexNumber(ComplexNumber other)
        {
            _x = other.Real;
            _y = other.Imaginary;
        }

        /**
            
        */

        /// <summary>
        /// Real part of this Complex number 
        /// (the x-coordinate in rectangular coordinates).
        /// </summary>
        public double Real
        {
            get { return _x; }
        }

        /// <summary>
        /// Imaginary part of this Complex number         
        /// </summary>
        public double Imaginary
        {
            get { return _y; }
        }

        /**
            @return 
        */

        /// <summary>
        /// Modulus of this Complex number
        /// (the distance from the origin in polar coordinates).
        /// </summary>
        /// <returns>|z| where z is this Complex number.</returns>
        public double Mod()
        {
            if (_x != 0 || _y != 0)
            {
                return Math.Sqrt(_x*_x + _y*_y);
            }

            return 0d;
        }

        /// <summary>
        /// Argument of this Complex number 
        /// (the angle in radians with the x-axis in polar coordinates).
        /// </summary>
        /// <returns>arg(z) where z is this Complex number.</returns>
        public double Arg()
        {
            return Math.Atan2(_y, _x);
        }

        /// <summary>
        /// Complex conjugate of this Complex number
        /// (the conjugate of x+i*y is x-i*y).
        /// </summary>
        /// <returns>z-bar where z is this Complex number.</returns>
        public ComplexNumber Conj()
        {
            return new ComplexNumber(_x, -_y);
        }

        /// <summary>
        /// Addition of Complex numbers (doesn't change this Complex number).
        /// (x+i*y) + (s+i*t) = (x+s)+i*(y+t)
        /// </summary>
        /// <param name="c1">The first argument.</param>
        /// <param name="c2">The second argument.</param>
        /// <returns>The result of the addition.</returns>
        public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }


        /// <summary>
        /// Subtraction of Complex numbers.
        /// (x-i*y) + (s-i*t) = (x-s)+i*(y-t)
        /// </summary>
        /// <param name="c1">The first argument.</param>
        /// <param name="c2">The second argument.</param>
        /// <returns>The result of the addition.</returns>
        public static ComplexNumber operator -(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }

        /// <summary>
        /// Multiplication of Complex numbers.
        /// </summary>
        /// <param name="c1">The first argument.</param>
        /// <param name="c2">The second argument.</param>
        /// <returns>The result of the addition.</returns>
        public static ComplexNumber operator *(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real*c2.Real - c1.Imaginary*c2.Imaginary, c1.Real*c2.Imaginary + c1.Imaginary
                                                                                  *c2.Real);
        }

        /// <summary>
        /// Division of Complex numbers.
        /// </summary>
        /// <param name="c1">The first argument.</param>
        /// <param name="c2">The second argument.</param>
        /// <returns>The result of the addition.</returns>
        public static ComplexNumber operator /(ComplexNumber c1, ComplexNumber c2)
        {
			var mod = c2.Mod();
            double den = mod * mod;
            return new ComplexNumber((c1.Real*c2.Real + c1.Imaginary
                                      *c2.Imaginary)/den, (c1.Imaginary
                                                           *c2.Real - c1.Real*c2.Imaginary)/den);
        }
        

        /// <summary>
        /// Complex exponential (doesn't change this Complex number).
        /// </summary>
        /// <returns>exp(z) where z is this Complex number.</returns>
        public ComplexNumber Exp()
        {
            return new ComplexNumber(Math.Exp(_x)*Math.Cos(_y), Math.Exp(_x)
                                                              *Math.Sin(_y));
        }

        /// <summary>
        /// Principal branch of the Complex logarithm of this Complex number.
        /// (doesn't change this Complex number).
        /// The principal branch is the branch with -pi less arg les-equals pi.
        /// </summary>
        /// <returns>log(z) where z is this Complex number.</returns>
        public ComplexNumber Log()
        {
            return new ComplexNumber(Math.Log(Mod()), Arg());
        }

        
        /// <summary>
        /// Complex square root (doesn't change this complex number).
        /// Computes the principal branch of the square root, which 
        /// is the value with 0 less equals arg less pi.
        /// </summary>
        /// <returns>sqrt(z) where z is this Complex number.</returns>
        public ComplexNumber Sqrt()
        {
            double r = Math.Sqrt(Mod());
            double theta = Arg()/2;
            return new ComplexNumber(r*Math.Cos(theta), r*Math.Sin(theta));
        }

        
        /// <summary>
        /// Real cosh function (used to compute complex trig functions).
        /// </summary>
        /// <param name="theta">The argument.</param>
        /// <returns>The result.</returns>
        private static double Cosh(double theta)
        {
            return (Math.Exp(theta) + Math.Exp(-theta))/2;
        }

        /// <summary>
        /// Real sinh function (used to compute complex trig functions).
        /// </summary>
        /// <param name="theta">The argument.</param>
        /// <returns>The result.</returns>
        private static double Sinh(double theta)
        {
            return (Math.Exp(theta) - Math.Exp(-theta))/2;
        }

       
        /// <summary>
        /// Sine of this Complex number (doesn't change this Complex number).
        /// sin(z) = (exp(i*z)-exp(-i*z))/(2*i).
        /// </summary>
        /// <returns>sin(z) where z is this Complex number.</returns>
        public ComplexNumber Sin()
        {
            return new ComplexNumber(Cosh(_y)*Math.Sin(_x), Sinh(_y)*Math.Cos(_x));
        }

        /// <summary>
        /// Cosine of this Complex number (doesn't change this Complex number).
        /// cos(z) = (exp(i*z)+exp(-i*z))/ 2.
        /// </summary>
        /// <returns>cos(z) where z is this Complex number.</returns>
        public ComplexNumber Cos()
        {
            return new ComplexNumber(Cosh(_y)*Math.Cos(_x), -Sinh(_y)*Math.Sin(_x));
        }

        
        /// <summary>
        /// Hyperbolic sine of this Complex number 
        /// (doesn't change this Complex number).
        /// sinh(z) = (exp(z)-exp(-z))/2.
        /// </summary>
        /// <returns>sinh(z) where z is this Complex number.</returns>
        public ComplexNumber Sinh()
        {
            return new ComplexNumber(Sinh(_x)*Math.Cos(_y), Cosh(_x)*Math.Sin(_y));
        }
        
        /// <summary>
        ///  Hyperbolic cosine of this Complex number 
        ///  (doesn't change this Complex number).
        /// cosh(z) = (exp(z) + exp(-z)) / 2.
        /// </summary>
        /// <returns>cosh(z) where z is this Complex number.</returns>
        public ComplexNumber Cosh()
        {
            return new ComplexNumber(Cosh(_x)*Math.Cos(_y), Sinh(_x)*Math.Sin(_y));
        }

        /// <summary>
        /// Tangent of this Complex number (doesn't change this Complex number).
        /// </summary>
        /// <returns>tan(z) = sin(z)/cos(z).</returns>
        public ComplexNumber Tan()
        {
            return (Sin())/(Cos());
        }

        /// <summary>
        /// Negative of this complex number (chs stands for change sign). 
        /// This produces a new Complex number and doesn't change 
        /// this Complex number.
        /// -(x+i*y) = -x-i*y.
        /// </summary>
        /// <param name="op"></param>
        /// <returns>-op where op is this Complex number.</returns>
        public static ComplexNumber operator -(ComplexNumber op)
        {
            return new ComplexNumber(-op.Real, -op.Imaginary);
        } 
        
        /// <inheritdoc/>
        public new String ToString()
        {
            if (_x != 0 && _y > 0)
            {
                return _x + " + " + _y + "i";
            }
            if (_x != 0 && _y < 0)
            {
                return _x + " - " + (-_y) + "i";
            }
            if (_y == 0)
            {
                return Format.FormatDouble(_x, 4);
            }
            if (_x == 0)
            {
                return _y + "i";
            }
            // shouldn't get here (unless Inf or NaN)
            return _x + " + i*" + _y;
        }
    }
}
