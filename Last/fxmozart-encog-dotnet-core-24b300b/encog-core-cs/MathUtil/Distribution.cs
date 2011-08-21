using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Encog.MathUtil
{
    /// <summary>
    /// Frequency distribution data from a distribution analysis of some data series.
    /// </summary>
    public class FrequencyDistributionData
    {
        double _min;
        double _max;
        double _incr;
        int[] _frequencyArr;

        #region Constructor

        /// <summary>
        /// Construct with the provided frequency distribution data.
        /// </summary>
        /// <param name="min">The minimum value in the data series the distribution represents.</param>
        /// <param name="max">The maximum value in the data series the distribution represents.</param>
        /// <param name="increment">The range of a single category bucket.</param>
        /// <param name="frequencyArr">The array of category frequency counts.</param>
        public FrequencyDistributionData(double min, double max, double increment, int[] frequencyArr)
        {
            _min = min;
            _max = max;
            _incr = increment;
            _frequencyArr = frequencyArr;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The minimum value in the data series the distribution represents.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The maximum value in the data series the distribution represents.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// The range of a single category bucket.
        /// </summary>
        public double Increment
        {
            get { return _incr; }
        }

        /// <summary>
        /// The array of category frequency counts.
        /// </summary>
        public int[] FrequencyArray
        {
            get { return _frequencyArr; }
        }

        #endregion
    }

    public static class Utilities
    {
        /// <summary>
        /// Randomly shuffles items within a list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        public static void Shuffle<T>(IList<T> list, FastRandom rng)
        {
            // This approach was suggested by Jon Skeet in a dotNet newsgroup post and
            // is also the technique used by the OpenJDK. The use of rnd.Next(i+1) introduces
            // the possibility of swapping an item with itself, I suspect the reasoning behind this
            // has to do with ensuring the probability of each possible permutation is approximately equal.
            for (int i = list.Count - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                T tmp = list[swapIndex];
                list[swapIndex] = list[i];
                list[i] = tmp;
            }
        }

        /// <summary>
        /// Rounds up or down to a whole number by using the fractional part of the input value
        /// as the probability that the value will be rounded up.
        /// 
        /// This is useful if we wish to round values and then sum them without generating a rounding bias.
        /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number which
        /// then causes a bias towards even numbers.
        /// 
        /// This solution is more appropriate for certain types of scientific values.
        /// </summary>
        public static double ProbabilisticRound(double val, FastRandom rng)
        {
            double integerPart = Math.Floor(val);
            double fractionalPart = val - integerPart;
            return rng.NextDouble() < fractionalPart ? integerPart + 1.0 : integerPart;
        }

        /// <summary>
        /// Calculates the median value in a list of sorted values.
        /// </summary>
        public static double CalculateMedian(IList<double> valueList)
        {
            Debug.Assert(valueList.Count != 0 && IsSorted(valueList), "CalculateMedian() requires non-zero length sorted list of values.");

            if (valueList.Count == 1)
            {
                return valueList[0];
            }

            if (valueList.Count % 2 == 0)
            {   // Even number of values. The values are already sorted so we simply take the
                // mean of the two central values.
                int idx = valueList.Count / 2;
                return (valueList[idx - 1] + valueList[idx]) / 2.0;
            }

            // Odd number of values. Return the middle value.
            // (integer division truncates fractional part of result).
            return valueList[valueList.Count / 2];
        }

        /// <summary>
        /// Indicates if a list of doubles is sorted into ascending order.
        /// </summary>
        public static bool IsSorted(IList<double> valueList)
        {
            if (0 == valueList.Count)
            {
                return true;
            }

            double prev = valueList[0];
            int count = valueList.Count;
            for (int i = 1; i < count; i++)
            {
                if (valueList[i] < prev)
                {
                    return false;
                }
                prev = valueList[i];
            }
            return true;
        }

        /// <summary>
        /// Calculate a frequency distribution for the provided array of values.
        /// 1) The minimum and maximum values are found.
        /// 2) The resulting value range is divided into equal sized sub-ranges (categoryCount).
        /// 3) The number of values that fall into each category is determined.
        /// </summary>
        public static FrequencyDistributionData CalculateDistribution(double[] valArr, int categoryCount)
        {
            // Determine min/max.
            double min = valArr[0];
            double max = min;

            for (int i = 1; i < valArr.Length; i++)
            {
                double val = valArr[i];
                if (val < min)
                {
                    min = val;
                }
                else if (val > max)
                {
                    max = val;
                }
            }

            double range = max - min;

            // Handle special case where the data series contains a single value.
            if (0.0 == range)
            {
                return new FrequencyDistributionData(min, max, 0.0, new int[] { valArr.Length });
            }

            // Loop values and for each one increment the relevant category's frequency count.
            double incr = range / (categoryCount - 1);
            int[] frequencyArr = new int[categoryCount];
            for (int i = 0; i < valArr.Length; i++)
            {
                frequencyArr[(int)((valArr[i] - min) / incr)]++;
            }
            return new FrequencyDistributionData(min, max, incr, frequencyArr);
        }



        /// <summary>
        /// Magnifies the fitness range.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="metricThreshold">The metric threshold.</param>
        /// <param name="metricMax">The metric max.</param>
        /// <param name="fitnessThreshold">The fitness threshold.</param>
        /// <param name="fitnessMax">The fitness max.</param>
        /// <returns></returns>
        public static double MagnifyFitnessRange(double x, double metricThreshold, double metricMax, double fitnessThreshold, double fitnessMax)
        {
            if (x < 0.0)
            {
                x = 0.0;
            }
            else if (x > metricMax)
            {
                x = metricMax;
            }

            if (x > metricThreshold)
            {
                return ((x - metricThreshold) / (metricMax - metricThreshold) * (fitnessMax - fitnessThreshold)) + fitnessThreshold;
            }
            // else
            return (x / metricThreshold) * fitnessThreshold;
        }



    }


    /// <summary>
    /// A fast random number generator for .NET
    /// Colin Green, January 2005
    /// 
    /// Key points:
    ///  1) Based on a simple and fast xor-shift pseudo random number generator (RNG) specified in: 
    ///  Marsaglia, George. (2003). Xorshift RNGs.
    ///  http://www.jstatsoft.org/v08/i14/xorshift.pdf
    ///  
    ///  This particular implementation of xorshift has a period of 2^128-1. See the above paper to see
    ///  how this can be easily extened if you need a longer period. At the time of writing I could find no 
    ///  information on the period of System.Random for comparison.
    /// 
    ///  2) Faster than System.Random. Up to 8x faster, depending on which methods are called.
    /// 
    ///  3) Direct replacement for System.Random. This class implements all of the methods that System.Random 
    ///  does plus some additional methods. The like named methods are functionally equivalent.
    ///  
    ///  4) Allows fast re-initialisation with a seed, unlike System.Random which accepts a seed at construction
    ///  time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
    ///  if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
    ///  sequence of random numbers many times. An alternative might be to cache random numbers in an array, but that 
    ///  approach is limited by memory capacity and the fact that you may also want a large number of different sequences 
    ///  cached. Each sequence can be represented by a single seed value (int) when using FastRandom.
    ///  
    ///  Notes.
    ///  A further performance improvement can be obtained by declaring local variables as static, thus avoiding 
    ///  re-allocation of variables on each call. However care should be taken if multiple instances of
    ///  FastRandom are in use or if being used in a multi-threaded environment.
    /// 
    /// 
    /// Colin Green, September 4th 2005
    ///     Added NextBytesUnsafe() - commented out by default.
    ///     Fixed bug in Reinitialise() - y,z and w variables were not being reset.
    /// 
    /// 
    /// Colin Green, December 2008. 
    ///     Fix to Next() - Was previously able to return int.MaxValue, contrary to the method's contract and comments.
    ///     Modified NextBool() to use _bitMask instead of a count of remaining bits. Also reset the bit buffer in Reinitialise().
    /// </summary>
    public class FastRandom
    {
        // The +1 ensures NextDouble doesn't generate 1.0
        const double REAL_UNIT_INT = 1.0 / ((double)int.MaxValue + 1.0);
        const double REAL_UNIT_UINT = 1.0 / ((double)uint.MaxValue + 1.0);
        const uint Y = 842502087, Z = 3579807591, W = 273326509;

        uint _x, _y, _z, _w;

        #region Constructors

        /// <summary>
        /// Initialises a new instance using time dependent seed.
        /// </summary>
        public FastRandom()
        {
            // Initialise using the system tick count.
            Reinitialise((int)Environment.TickCount);
        }

        /// <summary>
        /// Initialises a new instance using an int value as seed.
        /// This constructor signature is provided to maintain compatibility with
        /// System.Random
        /// </summary>
        public FastRandom(int seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Reinitialisation]

        /// <summary>
        /// Reinitialises using an int value as a seed.
        /// </summary>
        public void Reinitialise(int seed)
        {
            // The only stipulation stated for the xorshift RNG is that at least one of
            // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
            // resetting of the x seed
            _x = (uint)seed;
            _y = Y;
            _z = Z;
            _w = W;

            _bitBuffer = 0;
            _bitMask = 1;
        }

        #endregion

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1.
        /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
        /// This does slightly eat into some of the performance gain over System.Random, but not much.
        /// For better performance see:
        /// 
        /// Call NextInt() for an int over the range 0 to int.MaxValue.
        /// 
        /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
        /// including negative values. 
        /// </summary>
        public int Next()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _w & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
            {
                return Next();
            }
            return (int)rtn;
        }

        /// <summary>
        /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        public int Next(int upperBound)
        {
            if (upperBound < 0)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            }

            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            return (int)((REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * upperBound);
        }

        /// <summary>
        /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
        /// upperBound must be >= lowerBound. lowerBound may be negative.
        /// </summary>
        public int Next(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
            }

            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            int range = upperBound - lowerBound;
            if (range < 0)
            {   // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
                // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.  
                return lowerBound + (int)((REAL_UNIT_UINT * (double)(_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * (double)((long)upperBound - (long)lowerBound));
            }

            // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
            // a little more performance.
            return lowerBound + (int)((REAL_UNIT_INT * (double)(int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * (double)range);
        }

        /// <summary>
        /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        public double NextDouble()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            //
            // Also note that the loss of one bit of precision is equivalent to what occurs within 
            // System.Random.
            return REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))));
        }

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// This method is functionally equivalent to System.Random.NextBytes(). 
        /// </summary>
        public void NextBytes(byte[] buffer)
        {
            // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
            uint x = this._x, y = this._y, z = this._z, w = this._w;
            int i = 0;
            uint t;
            for (int bound = buffer.Length - 3; i < bound; )
            {
                // Generate 4 bytes. 
                // Increased performance is achieved by generating 4 random bytes per loop.
                // Also note that no mask needs to be applied to zero out the higher order bytes before
                // casting because the cast ignores thos bytes. Thanks to Stefan Troschütz for pointing this out.
                t = x ^ (x << 11);
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if (i < buffer.Length)
            {
                // Generate 4 bytes.
                t = x ^ (x << 11);
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                if (i < buffer.Length)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if (i < buffer.Length)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if (i < buffer.Length)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }
            this._x = x; this._y = y; this._z = z; this._w = w;
        }

        ///// <summary>
        ///// A version of NextBytes that uses a pointer to set 4 bytes of the byte buffer in one operation
        ///// thus providing a nice speedup. The loop is also partially unrolled to allow out-of-order-execution,
        ///// this results in about a x2 speedup on an AMD Athlon. Thus performance may vary wildly on different CPUs
        ///// depending on the number of execution units available.
        ///// 
        ///// Another significant speedup is obtained by setting the 4 bytes by indexing pDWord (e.g. pDWord[i++]=_w)
        ///// instead of dereferencing it (e.g. *pDWord++=_w).
        ///// 
        ///// Note that this routine requires the unsafe compilation flag to be specified and so is commented out by default.
        ///// </summary>
        ///// <param name="buffer"></param>
        //      public unsafe void NextBytesUnsafe(byte[] buffer)
        //      {
        //          if(buffer.Length % 8 != 0)
        //              throw new ArgumentException("Buffer length must be divisible by 8", "buffer");
        //
        //          uint _x=this._x, _y=this._y, _z=this._z, _w=this._w;
        //          
        //          fixed(byte* pByte0 = buffer)
        //          {
        //              uint* pDWord = (uint*)pByte0;
        //              for(int i=0, len=buffer.Length>>2; i < len; i+=2) 
        //              {
        //                  uint t=(_x^(_x<<11));
        //                  _x=_y; _y=_z; _z=_w;
        //                  pDWord[i] = _w = (_w^(_w>>19))^(t^(t>>8));
        //
        //                  t=(_x^(_x<<11));
        //                  _x=_y; _y=_z; _z=_w;
        //                  pDWord[i+1] = _w = (_w^(_w>>19))^(t^(t>>8));
        //              }
        //          }
        //
        //          this._x=_x; this._y=_y; this._z=_z; this._w=_w;
        //      }
        #endregion

        #region Public Methods [Methods not present on System.Random]

        /// <summary>
        /// Generates a uint. Values returned are over the full range of a uint, 
        /// uint.MinValue to uint.MaxValue, inclusive.
        /// 
        /// This is the fastest method for generating a single random number because the underlying
        /// random number generator algorithm generates 32 random bits that can be cast directly to 
        /// a uint.
        /// </summary>
        public uint NextUInt()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            return _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
        }

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue, inclusive. 
        /// This method differs from Next() only in that the range is 0 to int.MaxValue
        /// and not 0 to int.MaxValue-1.
        /// 
        /// The slight difference in range means this method is slightly faster than Next()
        /// but is not functionally equivalent to System.Random.Next().
        /// </summary>
        public int NextInt()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            return (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))));
        }

        // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
        // with bitMask.
        uint _bitBuffer;
        uint _bitMask;

        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public bool NextBool()
        {
            if (_bitMask == 0)
            {
                // Generate 32 more bits.
                uint t = _x ^ (_x << 11);
                _x = _y; _y = _z; _z = _w;
                _bitBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

                // Reset the bitMask that tells us which bit to read next.
                _bitMask = 0x80000000;
                return (_bitBuffer & _bitMask) == 0;
            }

            return (_bitBuffer & (_bitMask >>= 1)) == 0;
        }

        #endregion
    }
}
