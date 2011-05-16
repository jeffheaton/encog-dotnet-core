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
using System.Drawing;
using System.Text;
using Encog.ML.Data.Basic;
using Encog.Util.DownSample;

namespace Encog.ML.Data.Image
{
    /// <summary>
    /// An extension of the BasicMLData class that is designed to hold images for
    /// input into a neural network. This class should only be used with the
    /// ImageNeuralDataSet collection.
    /// 
    /// This class provides the ability to associate images with the elements of a
    /// dataset. These images will be downsampled to the resolution specified in the
    /// ImageNeuralData set class that they are added to.
    /// </summary>
    public class ImageMlData : BasicMLData
    {
        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageMlData(Bitmap image)
            : base(1)
        {
            Image = image;
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


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
            double[] sample = downsampler.DownSample(Image, height,
                                                     width);

            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = ((sample[i] - 0)
                             /(255 - 0))
                            *(hi - lo) + lo;
            }

            Data = sample;
        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}

#endif