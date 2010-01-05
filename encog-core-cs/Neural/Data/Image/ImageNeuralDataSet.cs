// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Util.DownSample;
using System.Reflection;
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData.Image
{
    /// <summary>
    /// Store a collection of images for training with a neural network. This class
    /// collects and then downsamples images for use with a neural network. This is a
    /// memory based class, so large datasets can run out of memory.
    /// </summary>
    public class ImageNeuralDataSet : BasicNeuralDataSet
    {
   
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";

        /// <summary>
        /// The width of this image.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        /// The height of this image.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }



        /// <summary>
        /// The downsampler to use.
        /// </summary>
        private System.Type downsampler;

        /// <summary>
        /// The height to downsample to.
        /// </summary>
        private int height;

        /// <summary>
        /// The width to downsample to.
        /// </summary>
        private int width;

        /// <summary>
        /// Should the bounds be found and cropped.
        /// </summary>
        private bool findBounds;

        /// <summary>
        /// Construct this class with the specified downsampler.
        /// </summary>
        /// <param name="downsampler">The downsampler to use.</param>
        /// <param name="findBounds">Should the bounds be found and clipped?</param>
        public ImageNeuralDataSet(System.Type downsampler,
                bool findBounds)
        {
            this.downsampler = downsampler;
            this.findBounds = findBounds;
            this.height = -1;
            this.width = -1;
        }

        /// <summary>
        /// Add the specified data, must be an ImageNeuralData class.
        /// </summary>
        /// <param name="data">The data The object to add.</param>
        public override void Add(INeuralData data)
        {
            if (!(data is ImageNeuralData))
            {
                throw new NeuralNetworkError(ImageNeuralDataSet.MUST_USE_IMAGE);
            }

            base.Add(data);
        }

        /// <summary>
        /// Add the specified input and ideal object to the collection. 
        /// </summary>
        /// <param name="inputData">The image to train with.</param>
        /// <param name="idealData">The expected otuput form this image.</param>
        public override void Add(INeuralData inputData, INeuralData idealData)
        {
            if (!(inputData is ImageNeuralData))
            {
                throw new NeuralNetworkError(ImageNeuralDataSet.MUST_USE_IMAGE);
            }

            base.Add(inputData, idealData);
        }

        /// <summary>
        /// Add input and expected output.  This is used for supervised training.
        /// </summary>
        /// <param name="inputData">The input data to train on.</param>
        public override void Add(INeuralDataPair inputData)
        {
            if (!(inputData.Input is ImageNeuralData))
            {
                throw new NeuralNetworkError(ImageNeuralDataSet.MUST_USE_IMAGE);
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

            foreach (INeuralDataPair pair in this)
            {
                if (pair.Input is ImageNeuralData)
                {
                    throw new NeuralNetworkError(
                    "Invalid class type found in ImageNeuralDataSet, only "
                    + "ImageNeuralData items are allowed.");
                }

                IDownSample downsample;

                downsample = (IDownSample)Assembly.GetExecutingAssembly().CreateInstance(this.downsampler.Name);

                ImageNeuralData input = (ImageNeuralData)pair.Input;
                input.Downsample(downsample, this.findBounds, height, width);


            }
        }
    }
}
#endif