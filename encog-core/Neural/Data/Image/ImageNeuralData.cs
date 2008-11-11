using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using System.Drawing;
using Encog.Util.DownSample;

namespace Encog.Neural.NeuralData.Image
{
    public class ImageNeuralData : BasicNeuralData
    {

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
        /// <param name="Image">The image to use.</param>
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
