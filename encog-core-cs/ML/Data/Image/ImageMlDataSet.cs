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

#if !SILVERLIGHT

using System;
using Encog.ML.Data.Basic;
using Encog.Neural;
using Encog.Util.DownSample;

namespace Encog.ML.Data.Image
{
    /// <summary>
    /// Store a collection of images for training with a neural network. This class
    /// collects and then downsamples images for use with a neural network. This is a
    /// memory based class, so large datasets can run out of memory.
    /// </summary>
    public class ImageMlDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";

        /// <summary>
        /// The downsampler to use.
        /// </summary>
        private readonly IDownSample downsampler;

        /// <summary>
        /// Should the bounds be found and cropped.
        /// </summary>
        private readonly bool findBounds;

        /// <summary>
        /// The high value to normalize to.
        /// </summary>
        private readonly double hi;

        /// <summary>
        /// The low value to normalize to.
        /// </summary>
        private readonly double lo;

        /// <summary>
        /// The height to downsample to.
        /// </summary>
        private int height;

        /// <summary>
        /// The width to downsample to.
        /// </summary>
        private int width;


        /// <summary>
        /// Construct this class with the specified downsampler.
        /// </summary>
        /// <param name="downsampler">The downsampler to use.</param>
        /// <param name="findBounds">Should the bounds be found and clipped.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public ImageMlDataSet(IDownSample downsampler,
                              bool findBounds, double hi, double lo)
        {
            this.downsampler = downsampler;
            this.findBounds = findBounds;
            height = -1;
            width = -1;
            this.hi = hi;
            this.lo = lo;
        }

        /// <summary>
        /// The height.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public int Width
        {
            get { return width; }
        }


        /// <summary>
        /// Add the specified data, must be an ImageNeuralData class.
        /// </summary>
        /// <param name="data">The data The object to add.</param>
        public override void Add(MLData data)
        {
            if (!(data is ImageMlData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(data);
        }

        /// <summary>
        /// Add the specified input and ideal object to the collection.
        /// </summary>
        /// <param name="inputData">The image to train with.</param>
        /// <param name="idealData">The expected otuput form this image.</param>
        public override void Add(MLData inputData, MLData idealData)
        {
            if (!(inputData is ImageMlData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData, idealData);
        }

        /// <summary>
        /// Add input and expected output. This is used for supervised training.
        /// </summary>
        /// <param name="inputData">The input data to train on.</param>
        public override void Add(MLDataPair inputData)
        {
            if (!(inputData.Input is ImageMlData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData);
        }


        /// <summary>
        /// Downsample all images and generate training data.
        /// </summary>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        public void Downsample(int height, int width)
        {
            this.height = height;
            this.width = width;

            foreach (MLDataPair pair in this)
            {
                if (!(pair.Input is ImageMlData))
                {
                    throw new NeuralNetworkError(
                        "Invalid class type found in ImageNeuralDataSet, only "
                        + "ImageNeuralData items are allowed.");
                }

                var input = (ImageMlData) pair.Input;
                input.Downsample(downsampler, findBounds, height, width,
                                 hi, lo);
            }
        }
    }
}

#endif