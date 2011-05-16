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
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// A class that is able to downsample an image.
    /// </summary>
    public interface IDownSample
    {
        /// <summary>
        /// Get the bottom boundary of the image.
        /// </summary>
        int DownSampleBottom { get; }

        /// <summary>
        /// The left boundary of the image.
        /// </summary>
        int DownSampleLeft { get; }

        /// <summary>
        /// Get the right boundary of the image.
        /// </summary>
        int DownSampleRight { get; }

        /// <summary>
        /// Get the top boundary of the image.
        /// </summary>
        int DownSampleTop { get; }


        /// <summary>
        /// The height of the image.
        /// </summary>
        int ImageHeight { get; }

        /// <summary>
        /// The width of the image.
        /// </summary>
        int ImageWidth { get; }

        /// <summary>
        /// The image pixel map.
        /// </summary>
        int[] PixelMap { get; }

        /// <summary>
        /// The x-ratio of the downsample.
        /// </summary>
        double RatioX { get; }

        /// <summary>
        /// The y-ratio of the downsample.
        /// </summary>
        double RatioY { get; }

        /// <summary>
        /// Downsample the image to the specified height and width.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <returns>The downsampled image.</returns>
        double[] DownSample(Bitmap image, int height, int width);

        /// <summary>
        /// Find the bounds around the image to exclude whitespace.
        /// </summary>
        void FindBounds();

        /// <summary>
        /// Process the specified image.
        /// </summary>
        /// <param name="image">The image to process.</param>
        void ProcessImage(Bitmap image);
    }
}

#endif