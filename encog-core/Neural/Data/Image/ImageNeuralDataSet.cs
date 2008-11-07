using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using Encog.Util.DownSample;
using System.Reflection;

namespace Encog.Neural.Data.Image
{
    public class ImageNeuralDataSet : BasicNeuralDataSet
    {
   
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";


        public int Width
        {
            get
            {
                return this.width;
            }
        }

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
        public new void Add(INeuralData data)
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
        public new void Add(INeuralData inputData, INeuralData idealData)
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
        public new void Add(INeuralDataPair inputData)
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
