//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// Downsample an image keeping the RGB colors.
    /// </summary>
    public class RGBDownsample : IDownSample
    {
        /// <summary>
        /// The bottom boundary of the downsample.
        /// </summary>
        private int _downSampleBottom;

        /// <summary>
        /// The left boundary of the downsample.
        /// </summary>
        private int _downSampleLeft;

        /// <summary>
        /// The right boundary of the downsample.
        /// </summary>
        private int _downSampleRight;

        /// <summary>
        /// The top boundary of the downsample.
        /// </summary>
        private int _downSampleTop;

        /// <summary>
        /// The image height.
        /// </summary>
        private int _imageHeight;

        /// <summary>
        /// The image width.
        /// </summary>
        private int _imageWidth;

        /// <summary>
        /// The downsample x-ratio.
        /// </summary>
        private double _ratioX;

        /// <summary>
        /// The downsample y-ratio.
        /// </summary>
        private double _ratioY;

        /// <summary>
        /// The current red average.
        /// </summary>
        public int CurrentRed { get; set; }

        /// <summary>
        /// The current blue average.
        /// </summary>
        public int CurrentBlue { get; set; }

        /// <summary>
        /// The current green average.
        /// </summary>
        public int CurrentGreen { get; set; }

        /// <summary>
        /// The current image being processed.
        /// </summary>
        public Bitmap Image { get; set; }

        #region IDownSample Members

        /// <summary>
        /// The pixel map from the image.
        /// </summary>
        public int[] PixelMap { get; set; }

        /// <summary>
        /// Called to downsample the image and store it in the down sample component.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">THe width to downsample to.</param>
        /// <returns>The downsampled image.</returns>
        public virtual double[] DownSample(Bitmap image, int height,
                                           int width)
        {
            Image = image;
            ProcessImage(image);

            var result = new double[height*width*3];

            // now downsample

            _ratioX = (_downSampleRight - _downSampleLeft)
                     /(double) width;
            _ratioY = (_downSampleBottom - _downSampleTop)
                     /(double) height;

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    DownSampleRegion(x, y);
                    result[index++] = CurrentRed;
                    result[index++] = CurrentGreen;
                    result[index++] = CurrentBlue;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is called to automatically crop the image so that whitespace
        /// is removed.
        /// </summary>
        public void FindBounds()
        {
            // top line
            for (int y = 0; y < _imageHeight; y++)
            {
                if (!HLineClear(y))
                {
                    _downSampleTop = y;
                    break;
                }
            }
            // bottom line
            for (int y = _imageHeight - 1; y >= 0; y--)
            {
                if (!HLineClear(y))
                {
                    _downSampleBottom = y;
                    break;
                }
            }
            // left line
            for (int x = 0; x < _imageWidth; x++)
            {
                if (!VLineClear(x))
                {
                    _downSampleLeft = x;
                    break;
                }
            }

            // right line
            for (int x = _imageWidth - 1; x >= 0; x--)
            {
                if (!VLineClear(x))
                {
                    _downSampleRight = x;
                    break;
                }
            }
        }

        /// <summary>
        /// The bottom of the downsample.
        /// </summary>
        public int DownSampleBottom
        {
            get { return _downSampleBottom; }
        }

        /// <summary>
        /// The left of the downsample.
        /// </summary>
        public int DownSampleLeft
        {
            get { return _downSampleLeft; }
        }

        /// <summary>
        /// The right of the downsample.
        /// </summary>
        public int DownSampleRight
        {
            get { return _downSampleRight; }
        }

        /// <summary>
        /// The top of the downsample.
        /// </summary>
        public int DownSampleTop
        {
            get { return _downSampleTop; }
        }

        /// <summary>
        /// The image height.
        /// </summary>
        public int ImageHeight
        {
            get { return _imageHeight; }
        }

        /// <summary>
        /// The image width.
        /// </summary>
        public int ImageWidth
        {
            get { return _imageWidth; }
        }


        /// <summary>
        /// The x-ratio.
        /// </summary>
        public double RatioX
        {
            get { return _ratioX; }
        }

        /// <summary>
        /// The y-ratio.
        /// </summary>
        public double RatioY
        {
            get { return _ratioY; }
        }

        /// <summary>
        /// Process the image and prepare it to be downsampled.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        public void ProcessImage(Bitmap image)
        {
            Image = image;
            _imageHeight = Image.Height;
            _imageWidth = Image.Width;
            _downSampleLeft = 0;
            _downSampleTop = 0;
            _downSampleRight = _imageWidth;
            _downSampleBottom = _imageHeight;

            _ratioX = (_downSampleRight - _downSampleLeft)
                     /(double) ImageWidth;
            _ratioY = (_downSampleBottom - _downSampleTop)
                     /(double) ImageHeight;
        }

        #endregion

        /// <summary>
        /// Called to downsample a region of the image.
        /// </summary>
        /// <param name="x">The x coordinate of the resulting downsample.</param>
        /// <param name="y">The y coordinate of the resulting downsample.</param>
        public void DownSampleRegion(int x, int y)
        {
            var startX = (int) (_downSampleLeft + x*_ratioX);
            var startY = (int) (_downSampleTop + y*_ratioY);
            var endX = (int) (startX + _ratioX);
            var endY = (int) (startY + _ratioY);

            endX = Math.Min(_imageWidth, endX);
            endY = Math.Min(_imageHeight, endY);

            int redTotal = 0;
            int greenTotal = 0;
            int blueTotal = 0;

            int total = 0;

            for (int yy = startY; yy < endY; yy++)
            {
                for (int xx = startX; xx < endX; xx++)
                {
                    Color pixel = Image.GetPixel(xx, yy);
                    redTotal += pixel.R;
                    greenTotal += pixel.G;
                    blueTotal += pixel.B;
                    total++;
                }
            }

            CurrentRed = redTotal/total;
            CurrentGreen = greenTotal/total;
            CurrentBlue = blueTotal/total;
        }

        /// <summary>
        /// This method is called internally to see if there are any pixels in the
        /// given scan line. This method is used to perform autocropping.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool HLineClear(int y)
        {
            for (int i = 0; i < _imageWidth; i++)
            {
                Color pixel = Image.GetPixel(i, y);
                if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method is called internally to see if there are any pixels in the
        /// given scan line. This method is used to perform autocropping.
        /// </summary>
        /// <param name="x">The vertical line to scan.</param>
        /// <returns>True if there are any pixels in the specified vertical line.</returns>
        private bool VLineClear(int x)
        {
            for (int i = 0; i < _imageHeight; i++)
            {
                Color pixel = Image.GetPixel(x, i);
                if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
