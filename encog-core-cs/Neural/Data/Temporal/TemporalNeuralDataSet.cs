// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Neural.Data;
using Encog.Util.Time;

namespace Encog.Neural.NeuralData.Temporal
{
    /// <summary>
    /// This class implements a temporal neural data set. A temporal neural dataset
    /// is designed to use a neural network to predict.
    /// 
    /// A temporal dataset is a stream of data over a time range. This time range is
    /// broken up into "points". Each point can contain one or more values. These
    /// values are either the values that you would like to predict, or use to
    /// predict. It is possible for a value to be both predicted and used to predict.
    /// For example, if you were trying to predict a trend in a stock's price
    /// fluctuations you might very well use the security price for both.
    /// 
    /// Each point that we have data for is stored in the TemporalPoint class. Each
    /// TemporalPoint will contain one more data values. These data values are
    /// described by the TemporalDataDescription class. For example, if you had five
    /// TemporalDataDescription objects added to this class, each Temporal point
    /// object would contain five values.
    /// 
    /// Points are arranged by sequence number.  No two points can have the same 
    /// sequence numbers.  Methods are provided to allow you to add points using the
    /// Date class.  These dates are resolved to sequence number using the level
    /// of granularity specified for this class.  No two points can occupy the same
    /// granularity increment.
    /// </summary>
    public class TemporalNeuralDataSet : BasicNeuralDataSet
    {

        /// <summary>
        /// Descriptions of the data needed.
        /// </summary>
        private IList<TemporalDataDescription> descriptions =
            new List<TemporalDataDescription>();

        /// <summary>
        /// The temporal points at which we have data.
        /// </summary>
        private List<TemporalPoint> points = new List<TemporalPoint>();

        /// <summary>
        /// The size of the input window, this is the data being used to predict.
        /// </summary>
        private int inputWindowSize;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        private int predictWindowSize;

        /// <summary>
        /// The lowest sequence.
        /// </summary>
        private int lowSequence;

        /// <summary>
        /// The highest sequence.
        /// </summary>
        private int highSequence;

        /// <summary>
        /// How big would we like the input size to be.
        /// </summary>
        private int desiredSetSize;

        /// <summary>
        /// How many input neurons will be used.
        /// </summary>
        private int inputNeuronCount;

        /// <summary>
        /// How many output neurons will there be.
        /// </summary>
        private int outputNeuronCount;

        /// <summary>
        /// What is the date for the first temporal point.
        /// </summary>
        private DateTime startingPoint = DateTime.MinValue;

        /// <summary>
        /// What is the granularity of the temporal points? Days, months, years,
        /// etc?
        /// </summary>
        private TimeUnit sequenceGrandularity;


        /// <summary>
        /// The data descriptions.
        /// </summary>
        public virtual IList<TemporalDataDescription> Descriptions
        {
            get
            {
                return this.descriptions;
            }
        }

        /// <summary>
        /// The points, or time slices to take data from.
        /// </summary>
        public virtual IList<TemporalPoint> Points
        {
            get
            {
                return this.points;
            }
        }

        /// <summary>
        /// Get the size of the input window.
        /// </summary>
        public virtual int InputWindowSize
        {
            get
            {
                return this.inputWindowSize;
            }
            set
            {
                this.inputWindowSize = value;
            }
        }

        /// <summary>
        /// The prediction window size.
        /// </summary>
        public virtual int PredictWindowSize
        {
            get
            {
                return this.predictWindowSize;
            }
            set
            {
                this.predictWindowSize = value;
            }
        }

        /// <summary>
        /// The low value for the sequence.
        /// </summary>
        public virtual int LowSequence
        {
            get
            {
                return this.lowSequence;
            }
            set
            {
                this.lowSequence = value;
            }
        }

        /// <summary>
        /// The high value for the sequence.
        /// </summary>
        public virtual int HighSequence
        {
            get
            {
                return this.highSequence;
            }
            set
            {
                this.highSequence = value;
            }
        }

        /// <summary>
        /// The desired dataset size.
        /// </summary>
        public virtual int DesiredSetSize
        {
            get
            {
                return this.desiredSetSize;
            }
            set
            {
                this.desiredSetSize = value;
            }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public virtual int InputNeuronCount
        {
            get
            {
                return this.inputNeuronCount;
            }
            set
            {
                this.inputNeuronCount = value;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public virtual int OutputNeuronCount
        {
            get
            {
                return this.outputNeuronCount;
            }
            set
            {
                this.outputNeuronCount = value;
            }
        }

        /// <summary>
        /// The starting point.
        /// </summary>
        public virtual DateTime StartingPoint
        {
            get
            {
                return this.startingPoint;
            }
            set
            {
                this.startingPoint = value;
            }
        }

        /// <summary>
        /// The size of the timeslices.
        /// </summary>
        public virtual TimeUnit SequenceGrandularity
        {
            get
            {
                return this.sequenceGrandularity;
            }
            set
            {
                this.sequenceGrandularity = value;
            }
        }


        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String ADD_NOT_SUPPORTED = "Direct adds to the temporal dataset are not supported.  "
                + "Add TemporalPoint objects and call generate.";


        /// <summary>
        /// Construct a dataset.
        /// </summary>
        /// <param name="inputWindowSize">What is the input window size.</param>
        /// <param name="predictWindowSize">What is the prediction window size.</param>
        public TemporalNeuralDataSet(int inputWindowSize,
                 int predictWindowSize)
        {
            this.inputWindowSize = inputWindowSize;
            this.predictWindowSize = predictWindowSize;
            this.lowSequence = int.MinValue;
            this.highSequence = int.MaxValue;
            this.desiredSetSize = int.MaxValue;
            this.startingPoint = DateTime.MinValue;
            this.sequenceGrandularity = TimeUnit.DAYS;
        }

        /// <summary>
        /// Add a data description.
        /// </summary>
        /// <param name="desc">The data description to add.</param>
        public virtual void AddDescription(TemporalDataDescription desc)
        {
            if (this.points.Count > 0)
            {
                throw new TemporalError(
                        "Can't add anymore descriptions, there are "
                                + "already temporal points defined.");
            }

            int index = this.descriptions.Count;
            desc.Index = index;

            this.descriptions.Add(desc);
            CalculateNeuronCounts();
        }

        /// <summary>
        /// Clear the entire dataset.
        /// </summary>
        public virtual void Clear()
        {
            descriptions.Clear();
            points.Clear();
            this.Data.Clear();
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used</param>
        /// <param name="idealData">Not used</param>
        public override void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new TemporalError(TemporalNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public override void Add(INeuralDataPair inputData)
        {
            throw new TemporalError(TemporalNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="data">Not used.</param>
        public override void Add(INeuralData data)
        {
            throw new TemporalError(TemporalNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Create a temporal data point using a sequence number. They can also be
        /// created using time. No two points should have the same sequence number.
        /// </summary>
        /// <param name="sequence">The sequence number.</param>
        /// <returns>A new TemporalPoint object.</returns>
        public virtual TemporalPoint CreatePoint(int sequence)
        {
            TemporalPoint point = new TemporalPoint(this.descriptions.Count);
            point.Sequence = sequence;
            this.points.Add(point);
            return point;
        }

        /// <summary>
        /// Create a sequence number from a time. The first date will be zero, and
        /// subsequent dates will be increased according to the grandularity
        /// specified. 
        /// </summary>
        /// <param name="when">The date to generate the sequence number for.</param>
        /// <returns>A sequence number.</returns>
        public virtual int GetSequenceFromDate(DateTime when)
        {
            int sequence;

            if (startingPoint != DateTime.MinValue)
            {
                TimeSpanUtil span = new TimeSpanUtil(this.startingPoint, when);
                sequence = (int)span.GetSpan(this.sequenceGrandularity);
            }
            else
            {
                this.startingPoint = when;
                sequence = 0;
            }

            return sequence;
        }


        /// <summary>
        /// Create a temporal point from a time. Using the grandularity each date is
        /// given a unique sequence number. No two dates that fall in the same
        /// grandularity should be specified.
        /// </summary>
        /// <param name="when">The time that this point should be created at.</param>
        /// <returns>The point TemporalPoint created.</returns>
        public virtual TemporalPoint CreatePoint(DateTime when)
        {
            int sequence = GetSequenceFromDate(when);
            TemporalPoint point = new TemporalPoint(this.descriptions.Count);
            point.Sequence = sequence;
            this.points.Add(point);
            return point;
        }

        /// <summary>
        /// Calculate how many points are in the high and low range. These are the
        /// points that the training set will be generated on.
        /// </summary>
        /// <returns>The number of points.</returns>
        public virtual int CalculatePointsInRange()
        {
            int result = 0;

            foreach (TemporalPoint point in points)
            {
                if (IsPointInRange(point))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate the actual set size, this is the number of training set entries
        /// that will be generated.
        /// </summary>
        /// <returns>The size of the training set.</returns>
        public virtual int CalculateActualSetSize()
        {
            int result = CalculatePointsInRange();
            result = Math.Min(this.desiredSetSize, result);
            return result;
        }

        /// <summary>
        /// Calculate how many input and output neurons will be needed for the
        /// current data.
        /// </summary>
        public virtual void CalculateNeuronCounts()
        {
            this.inputNeuronCount = 0;
            this.outputNeuronCount = 0;

            foreach (TemporalDataDescription desc in this.descriptions)
            {
                if (desc.IsInput)
                {
                    this.inputNeuronCount += this.inputWindowSize;
                }
                if (desc.IsPredict)
                {
                    this.outputNeuronCount += this.predictWindowSize;
                }
            }
        }

        /// <summary>
        /// Is the specified point within the range. If a point is in the selection
        /// range, then the point will be used to generate the training sets.
        /// </summary>
        /// <param name="point">The point to consider.</param>
        /// <returns>True if the point is within the range.</returns>
        public virtual bool IsPointInRange(TemporalPoint point)
        {
            return ((point.Sequence >= this.LowSequence) && (point.Sequence <= this.HighSequence));

        }

        /// <summary>
        /// Generate input neural data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate neural data for.</param>
        /// <returns>The input neural data generated.</returns>
        public virtual BasicNeuralData GenerateInputNeuralData(int index)
        {
            if (index + this.inputWindowSize > this.points.Count)
            {
                throw new TemporalError("Can't generate input temporal data "
                        + "beyond the end of provided data.");
            }

            BasicNeuralData result = new BasicNeuralData(this.inputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < this.inputWindowSize; i++)
            {
                int descriptionIndex = 0;

                foreach (TemporalDataDescription desc in this.descriptions)
                {
                    if (desc.IsInput)
                    {
                        result[resultIndex++] = this.FormatData(desc, index
                                + i);
                    }
                    descriptionIndex++;
                }
            }
            return result;
        }

        /// <summary>
        /// Get data between two points in raw form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataRAW(TemporalDataDescription desc,
                 int index)
        {
            TemporalPoint point = this.points[index];
            return point[desc.Index];
        }

        /// <summary>
        /// Get data between two points in delta form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataDeltaChange(TemporalDataDescription desc,
                 int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = this.points[index];
            TemporalPoint previousPoint = this.points[index - 1];
            return point[desc.Index]
                    - previousPoint[desc.Index];
        }


        /// <summary>
        /// Get data between two points in percent form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataPercentChange(TemporalDataDescription desc,
                 int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = this.points[index];
            TemporalPoint previousPoint = this.points[index - 1];
            double currentValue = point[desc.Index];
            double previousValue = previousPoint[desc.Index];
            return (currentValue - previousValue) / previousValue;
        }

        /// <summary>
        /// Format data according to the type specified in the description.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to format the data at.</param>
        /// <returns>The formatted data.</returns>
        private double FormatData(TemporalDataDescription desc,
                int index)
        {
            double[] result = new double[1];

            switch (desc.DescriptionType)
            {
                case TemporalDataDescription.Type.DELTA_CHANGE:
                    result[0] = GetDataDeltaChange(desc, index);
                    break;
                case TemporalDataDescription.Type.PERCENT_CHANGE:
                    result[0] = GetDataPercentChange(desc, index);
                    break;
                case TemporalDataDescription.Type.RAW:
                    result[0] = GetDataRAW(desc, index);
                    break;
                default:
                    throw new TemporalError("Unsupported data type.");
            }

            if (desc.ActivationFunction != null)
            {
                desc.ActivationFunction.ActivationFunction(result,0,1);
            }

            return result[0];
        }

        /// <summary>
        /// Generate neural ideal data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate for.</param>
        /// <returns>The neural data generated.</returns>
        public virtual BasicNeuralData GenerateOutputNeuralData(int index)
        {
            if (index + this.predictWindowSize > this.points.Count)
            {
                throw new TemporalError("Can't generate prediction temporal data "
                        + "beyond the end of provided data.");
            }

            BasicNeuralData result = new BasicNeuralData(this.outputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < this.predictWindowSize; i++)
            {
                int descriptionIndex = 0;

                foreach (TemporalDataDescription desc in this.descriptions)
                {
                    if (desc.IsPredict)
                    {
                        result[resultIndex++] = this.FormatData(desc, index
                                + i);
                    }
                    descriptionIndex++;
                }

            }
            return result;
        }

        /// <summary>
        /// Calculate the index to start at.
        /// </summary>
        /// <returns>The starting point.</returns>
        public virtual int CalculateStartIndex()
        {
            for (int i = 0; i < this.points.Count; i++)
            {
                TemporalPoint point = this.points[i];
                if (this.IsPointInRange(point))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sort the points.
        /// </summary>
        public virtual void SortPoints()
        {
            this.points.Sort();
        }

        /// <summary>
        /// Generate the training sets.
        /// </summary>
        public virtual void Generate()
        {
            SortPoints();
            int start = CalculateStartIndex() + 1;
            int setSize = CalculateActualSetSize();
            int range = start
                    + (setSize - this.predictWindowSize - this.inputWindowSize);

            for (int i = start; i < range; i++)
            {
                BasicNeuralData input = GenerateInputNeuralData(i);
                BasicNeuralData ideal = GenerateOutputNeuralData(i
                        + this.inputWindowSize);
                BasicNeuralDataPair pair = new BasicNeuralDataPair(input, ideal);
                base.Add(pair);
            }
        }
    }
}
