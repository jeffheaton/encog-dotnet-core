//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.Linq;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Encog.Util.Time;

namespace Encog.ML.Data.Temporal
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
    public class TemporalMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the temporal dataset are not supported.  "
                                                + "Add TemporalPoint objects and call generate.";

        /// <summary>
        /// Descriptions of the data needed.
        /// </summary>
        private readonly IList<TemporalDataDescription> _descriptions =
            new List<TemporalDataDescription>();

        /// <summary>
        /// The temporal points at which we have data.
        /// </summary>
        private readonly List<TemporalPoint> _points = new List<TemporalPoint>();

        /// <summary>
        /// How big would we like the input size to be.
        /// </summary>
        private int _desiredSetSize;

        /// <summary>
        /// The highest sequence.
        /// </summary>
        private int _highSequence;

        /// <summary>
        /// How many input neurons will be used.
        /// </summary>
        private int _inputNeuronCount;

        /// <summary>
        /// The size of the input window, this is the data being used to predict.
        /// </summary>
        private int _inputWindowSize;

        /// <summary>
        /// The lowest sequence.
        /// </summary>
        private int _lowSequence;

        /// <summary>
        /// How many output neurons will there be.
        /// </summary>
        private int _outputNeuronCount;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        private int _predictWindowSize;

        /// <summary>
        /// What is the granularity of the temporal points? Days, months, years,
        /// etc?
        /// </summary>
        private TimeUnit _sequenceGrandularity;

        /// <summary>
        /// What is the date for the first temporal point.
        /// </summary>
        private DateTime _startingPoint = DateTime.MinValue;

        /// <summary>
        /// Construct a dataset.
        /// </summary>
        /// <param name="inputWindowSize">What is the input window size.</param>
        /// <param name="predictWindowSize">What is the prediction window size.</param>
        public TemporalMLDataSet(int inputWindowSize,
                                     int predictWindowSize)
        {
            _inputWindowSize = inputWindowSize;
            _predictWindowSize = predictWindowSize;
            _lowSequence = int.MinValue;
            _highSequence = int.MaxValue;
            _desiredSetSize = int.MaxValue;
            _startingPoint = DateTime.MinValue;
            _sequenceGrandularity = TimeUnit.Days;
        }


        /// <summary>
        /// The data descriptions.
        /// </summary>
        public virtual IList<TemporalDataDescription> Descriptions
        {
            get { return _descriptions; }
        }

        /// <summary>
        /// The points, or time slices to take data from.
        /// </summary>
        public virtual IList<TemporalPoint> Points
        {
            get { return _points; }
        }

        /// <summary>
        /// Get the size of the input window.
        /// </summary>
        public virtual int InputWindowSize
        {
            get { return _inputWindowSize; }
            set { _inputWindowSize = value; }
        }

        /// <summary>
        /// The prediction window size.
        /// </summary>
        public virtual int PredictWindowSize
        {
            get { return _predictWindowSize; }
            set { _predictWindowSize = value; }
        }

        /// <summary>
        /// The low value for the sequence.
        /// </summary>
        public virtual int LowSequence
        {
            get { return _lowSequence; }
            set { _lowSequence = value; }
        }

        /// <summary>
        /// The high value for the sequence.
        /// </summary>
        public virtual int HighSequence
        {
            get { return _highSequence; }
            set { _highSequence = value; }
        }

        /// <summary>
        /// The desired dataset size.
        /// </summary>
        public virtual int DesiredSetSize
        {
            get { return _desiredSetSize; }
            set { _desiredSetSize = value; }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public virtual int InputNeuronCount
        {
            get { return _inputNeuronCount; }
            set { _inputNeuronCount = value; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public virtual int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
            set { _outputNeuronCount = value; }
        }

        /// <summary>
        /// The starting point.
        /// </summary>
        public virtual DateTime StartingPoint
        {
            get { return _startingPoint; }
            set { _startingPoint = value; }
        }

        /// <summary>
        /// The size of the timeslices.
        /// </summary>
        public virtual TimeUnit SequenceGrandularity
        {
            get { return _sequenceGrandularity; }
            set { _sequenceGrandularity = value; }
        }


        /// <summary>
        /// Add a data description.
        /// </summary>
        /// <param name="desc">The data description to add.</param>
        public virtual void AddDescription(TemporalDataDescription desc)
        {
            if (_points.Count > 0)
            {
                throw new TemporalError(
                    "Can't add anymore descriptions, there are "
                    + "already temporal points defined.");
            }

            int index = _descriptions.Count;
            desc.Index = index;

            _descriptions.Add(desc);
            CalculateNeuronCounts();
        }

        /// <summary>
        /// Clear the entire dataset.
        /// </summary>
        public virtual void Clear()
        {
            _descriptions.Clear();
            _points.Clear();
            Data.Clear();
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used</param>
        /// <param name="idealData">Not used</param>
        public sealed override void Add(IMLData inputData, IMLData idealData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public sealed override void Add(IMLDataPair inputData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="data">Not used.</param>
        public sealed override void Add(IMLData data)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Create a temporal data point using a sequence number. They can also be
        /// created using time. No two points should have the same sequence number.
        /// </summary>
        /// <param name="sequence">The sequence number.</param>
        /// <returns>A new TemporalPoint object.</returns>
        public virtual TemporalPoint CreatePoint(int sequence)
        {
            var point = new TemporalPoint(_descriptions.Count) {Sequence = sequence};
            _points.Add(point);
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

            if (_startingPoint != DateTime.MinValue)
            {
                var span = new TimeSpanUtil(_startingPoint, when);
                sequence = (int) span.GetSpan(_sequenceGrandularity);
            }
            else
            {
                _startingPoint = when;
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
            var point = new TemporalPoint(_descriptions.Count) {Sequence = sequence};
            _points.Add(point);
            return point;
        }

        /// <summary>
        /// Calculate how many points are in the high and low range. These are the
        /// points that the training set will be generated on.
        /// </summary>
        /// <returns>The number of points.</returns>
        public virtual int CalculatePointsInRange()
        {
            return _points.Count(IsPointInRange);
        }

        /// <summary>
        /// Calculate the actual set size, this is the number of training set entries
        /// that will be generated.
        /// </summary>
        /// <returns>The size of the training set.</returns>
        public virtual int CalculateActualSetSize()
        {
            int result = CalculatePointsInRange();
            result = Math.Min(_desiredSetSize, result);
            return result;
        }

        /// <summary>
        /// Calculate how many input and output neurons will be needed for the
        /// current data.
        /// </summary>
        public virtual void CalculateNeuronCounts()
        {
            _inputNeuronCount = 0;
            _outputNeuronCount = 0;

            foreach (TemporalDataDescription desc in _descriptions)
            {
                if (desc.IsInput)
                {
                    _inputNeuronCount += _inputWindowSize;
                }
                if (desc.IsPredict)
                {
                    _outputNeuronCount += _predictWindowSize;
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
            return ((point.Sequence >= LowSequence) && (point.Sequence <= HighSequence));
        }

        /// <summary>
        /// Generate input neural data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate neural data for.</param>
        /// <returns>The input neural data generated.</returns>
        public virtual BasicNeuralData GenerateInputNeuralData(int index)
        {
            if (index + _inputWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate input temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_inputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _inputWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsInput)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
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
        private double GetDataRaw(TemporalDataDescription desc,
                                  int index)
        {
            TemporalPoint point = _points[index - 1];
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
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
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
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
            double currentValue = point[desc.Index];
            double previousValue = previousPoint[desc.Index];
            return (currentValue - previousValue)/previousValue;
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
            var result = new double[1];

            switch (desc.DescriptionType)
            {
                case TemporalDataDescription.Type.DeltaChange:
                    result[0] = GetDataDeltaChange(desc, index);
                    break;
                case TemporalDataDescription.Type.PercentChange:
                    result[0] = GetDataPercentChange(desc, index);
                    break;
                case TemporalDataDescription.Type.Raw:
                    result[0] = GetDataRaw(desc, index);
                    break;
                default:
                    throw new TemporalError("Unsupported data type.");
            }

            if (desc.ActivationFunction != null)
            {
                desc.ActivationFunction.ActivationFunction(result, 0, 1);
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
            if (index + _predictWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate prediction temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_outputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _predictWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsPredict)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
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
            for (int i = 0; i < _points.Count; i++)
            {
                TemporalPoint point = _points[i];
                if (IsPointInRange(point))
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
            _points.Sort();
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
                        + (setSize - _predictWindowSize - _inputWindowSize);

            for (int i = start; i < range; i++)
            {
                BasicNeuralData input = GenerateInputNeuralData(i);
                BasicNeuralData ideal = GenerateOutputNeuralData(i
                                                                 + _inputWindowSize);
                var pair = new BasicNeuralDataPair(input, ideal);
                base.Add(pair);
            }
        }
    }
}
