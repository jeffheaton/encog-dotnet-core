// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using System.Drawing;
using Encog.Util.DownSample;

namespace Encog.Neural.NeuralData.Image
{
    /// <summary>
    /// An extension of the BasicNeuralData class that is designed to hold images for
    /// input into a neural network. This class should only be used with the
    /// ImageNeuralDataSet collection.
    /// 
    /// This class provides the ability to associate images with the elements of a
    /// dataset. These images will be downsampled to the resolution specified in the
    /// ImageNeuralData set class that they are added to.
    /// </summary>
    public class ImageNeuralData : BasicNeuralData
    {

        /// <summary>
        /// The image that will be downsampled.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        private Bitmap image;


        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageNeuralData(Bitmap image)
            : base(1)
        {
            this.image = image;
        }

        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to</param>
        public void Downsample(IDownSample downsampler,
                 bool findBounds, int height, int width)
        {
            if (findBounds)
            {
                downsampler.FindBounds();
            }
            double[] sample = downsampler.DownSample(height, width);
            this.Data = sample;
        }
    }
}
