using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util;
using Encog.Util.Logging;

namespace Encog.App.Finance.Indicators
{
   /// <summary>
   /// A set of financial / technical indicators.
   /// </summary>
   public class TechnicalIndicators
   {
        /// <summary>
        /// Gets or sets the fitness score.
        /// </summary>
        /// <value>
        /// The fitness score (netprofit / drawdown) * winning percentage..
        /// </value>
        private internal double FitNess
	    {
		    get { return _fitNess; }
		    set { _fitNess = value; }
	    }


        private internal double _forwardOscillator;

        public double NetProfit { get; set; }
        private double Drawdown { get; set;}
        private double WinningPercentage { get; set; }
        private internal double _fitNess = 0;

       private IList<double> _fitnessSerie = new List<double>();
	
       
       /// <summary>
       /// Gets or sets the fitness serie.
       /// </summary>
       /// <value>
       /// The fitness serie.
       /// </value>

       public IList<double> FitnessSerie
	    {
		    get { return _fitnessSerie; }
		    set { _fitnessSerie = value; }
	    }
        /// <summary>
        /// returns the current fitness score.
        /// </summary>
        /// <returns></returns>
        public double GetFitNess()
        {
            FitnessSerie.Add((NetProfit/Drawdown)*WinningPercentage);
            return (NetProfit/Drawdown)*WinningPercentage;
        }


        /// <summary>
        /// Our predicted closing values which will be used by the network to predict the forward indicator.
        /// </summary>
        private internal double[] _closes;
        /// <summary>
        /// Gets or sets the moving average length
        /// </summary>
        /// <value>
        /// The moving average length.
        /// </value>
        private internal int MovingAverageLenght { get; set; }
        /// <summary>
        /// Gets or sets the forward lookup value.
        /// This is the number of bars (or closes) we are looking into the future.
        /// </summary>
        /// <value>
        /// The forward lookup value.
        /// </value>
        private internal int ForwardLookupValue { get; set; }

        /// <summary>
        /// Sets the moving average lenght.
        /// </summary>
        /// <param name="Lenght">The lenght.</param>
        public void SetMovingAverageLenght(int Lenght)
        {
            MovingAverageLenght = Lenght;
        }

        /// <summary>
        /// Sets the closing values based on whatever closing values we have.
        /// </summary>
        /// <param name="myclosingValues">The myclosing values.</param>
        public void SetClosingValues(double [] myclosingValues)
        {
            if (myclosingValues != null)
                _closes = EngineArray.ArrayCopy(myclosingValues);
        }

    
        /// Sets the number of look forward values.
        /// </summary>
        /// <param name="valuesToLookForwardInto">The values to look forward into.</param>
        public void SetNumberOfLookForwardValues (int valuesToLookForwardInto)
        {
            ForwardLookupValue = valuesToLookForwardInto;
        }


       

   

      /// <summary>
        /// Calculates the forward oscillator.
        /// Which is :Close N - Average(Close , X);
        /// Where N is number of bars into the future and X is the lenght of our moving average.
        /// if this indicator is positive , then we are in bullish mode else we are in bearish mode.
        /// See Neural networks in the capital markets by John paul.
        /// </summary>
        /// <returns>double</returns>
       public double SimpleForwardOscillator(double predictedClose, int length , double currentClose)
      {

          double result = predictedClose - Average(currentClose, length);
          return result;

      }
       /// <summary>
       /// Averages two doubles.
       /// </summary>
       /// <param name="a">A.</param>
       /// <param name="b">The b.</param>
       /// <returns></returns>
       static double Average(double a, double b)
       {
          if(a == b) return a;

          if(a > b) return Average(b, a);

          while(true)
          {
            a++;
            if (a == b)
            {
              double mod = (b < 0) ? -b : b;
              return (mod % 2 == 0) ? b : a-1;
            }

            b--;

            if (a == b) 
              return a;
          }
       }

       private internal List<double> _movingAverageSeries;
       /// <summary>
       /// Gets the moving average serie.
       /// </summary>
       /// <returns></returns>
       public double [] GetMovingAverageSerie()
       {
           if (_movingAverageSeries != null)
           {
               return _movingAverageSeries.ToArray();
           }
           return null;
       }

       /// <summary>
        /// Calculate this indicator.
        /// </summary>
        ///
        /// <param name="data">The data to use.</param>
        /// <param name="length">The length to calculate over.</param>
        public void CalculateMovingAverageOfDoubleSerie(double [] data,
                                              int length)
       {

           if (data != null)
           {

               SetMovingAverageLenght(length);

               double[] close = data;
            
               if (_movingAverageSeries == null) 
                   _movingAverageSeries = new List<double>();
               int lookbackTotal = (MovingAverageLenght - 1);

               int start = lookbackTotal;
               if (start > (MovingAverageLenght - 1))
               {
                   return;
               }

               double periodTotal = 0;
               int trailingIdx = start - lookbackTotal;
               int i = trailingIdx;
               if (MovingAverageLenght > 1)
               {
                   while (i < start)
                   {
                       periodTotal += close[i++];
                   }
               }

               int outIdx = MovingAverageLenght - 1;
               do
               {
                   periodTotal += close[i++];
                   double t = periodTotal;
                   periodTotal -= close[trailingIdx++];
                   _movingAverageSeries[outIdx++] = t/MovingAverageLenght;
               } while (i < close.Length);

               BeginningIndex = MovingAverageLenght - 1;
               EndingIndex = _movingAverageSeries.Count - 1;

               for (i = 0; i < MovingAverageLenght - 1; i++)
               {
                   _movingAverageSeries[i] = 0;
               }

               return;
           }
           
           return;
       }
       
        /// <value>the beginningIndex to set</value>
        public int BeginningIndex { 
            get; 
            set; }


        /// <value>the endingIndex to set.</value>
        public int EndingIndex { 
            get; 
            set; }


   }
}