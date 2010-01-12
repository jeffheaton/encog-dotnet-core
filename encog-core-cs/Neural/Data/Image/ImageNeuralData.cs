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
using System.Drawing;
using Encog.Util.DownSample;
using Encog.Normalize.Output;

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
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageNeuralData(Bitmap image)
            : base(1)
        {
            this.Image = image;
        }


        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public void Downsample(IDownSample downsampler,
                 bool findBounds, int height, int width,
                 double hi, double lo)
        {
            if (findBounds)
            {
                downsampler.FindBounds();
            }
            double[] sample = downsampler.DownSample(this.Image, height,
                   width);

            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = OutputFieldRangeMapped.Calculate(sample[i], 0,
                        255, hi, lo);
            }

            this.Data = sample;
        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < this.Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(this.Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }

    }

}

#endif