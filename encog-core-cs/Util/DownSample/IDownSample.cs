// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// A class that is able to downsample an image.
    /// </summary>
    public interface IDownSample
    {
        /// <summary>
        /// The bitmap to downsample.
        /// </summary>
        Bitmap DownsampleImage
        {
            get;
        }

        /// <summary>
        /// The x-ratio for the downsample.
        /// </summary>
        double RatioX
        {
            get;
        }

        /// <summary>
        /// The y-ratio for the downsample.
        /// </summary>
        double RatioY
        {
            get;
        }
        
        /// <summary>
        /// The height of the image.
        /// </summary>
        int ImageHeight
        {
            get;
        }

        /// <summary>
        /// The width of the image.
        /// </summary>
        int ImageWidth
        {
            get;
        }

        /// <summary>
        /// The left boundary of the downsample.
        /// </summary>
        int DownSampleLeft
        {
            get;
        }

        /// <summary>
        /// The right boundary of the downsample.
        /// </summary>
        int DownSampleRight
        {
            get;
        }

        /// <summary>
        /// The top boundary of the downsample.
        /// </summary>
        int DownSampleTop
        {
            get;
        }

        /// <summary>
        /// The bottom boundary of the downsample.
        /// </summary>
        int DownSampleBottom
        {
            get;
        }

        /// <summary>
        /// Downsample the image.  This can be called multiple times.
        /// </summary>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <returns>The downsampled array from the image.</returns>
        double[] DownSample(int height, int width);

        /// <summary>
        /// Process the specified image.  It will not be downsampled until the
        /// DownSample method is called.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        void ProcessImage(Bitmap image);

        /// <summary>
        /// If you would like to trim off whitespace, this method will find the
        /// boundaries.
        /// </summary>
        void FindBounds();
    }
}
