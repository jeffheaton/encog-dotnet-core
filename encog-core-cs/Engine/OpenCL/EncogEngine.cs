/*
 * Encog(tm) Core v2.5 
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 * 
 * Copyright 2008-2010 by Heaton Research Inc.
 * 
 * Released under the LGPL.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 * 
 * Encog and Heaton Research are Trademarks of Heaton Research, Inc.
 * For information on Heaton Research trademarks, visit:
 * 
 * http://www.heatonresearch.com/copyright.html
 */
namespace Encog.Engine
{

    //using Encog.Engine.Opencl;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Opencl;

    /// <summary>
    /// The Encog Engine.
    /// The Engine forms the core of Encog's neural network calculation and training.
    /// </summary>
    ///
    public class EncogEngine
    {

        /// <summary>
        /// The default zero tolerance.
        /// </summary>
        ///
        public const double DEFAULT_ZERO_TOLERANCE = 0.000001d;

        /// <summary>
        /// The instance.
        /// </summary>
        ///
        private static EncogEngine instance;

#if !SILVERLIGHT
        /// <summary>
        /// If Encog is not using GPU/CL processing this attribute will be null.
        /// Otherwise it holds the Encog CL object.
        /// </summary>
        ///
        private EncogCL cl;
#endif

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        ///
        /// <returns>The instance.</returns>
        public static EncogEngine Instance
        {
            get
            {
                if (EncogEngine.instance == null)
                {
                    EncogEngine.instance = new EncogEngine();
                }
                return EncogEngine.instance;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Enable OpenCL processing. OpenCL processing allows Encog to use GPU
        /// devices to speed calculations. Not all areas of Encog can use this,
        /// however, GPU's can currently accelerate the training of Feedforward
        /// neural networks.
        /// To make use of the GPU you must have OpenCL drivers installed. For more
        /// information on getting OpenCL drivers, visit the following URL.
        /// http://www.heatonresearch.com/encog/opencl
        /// </summary>
        ///
        public void InitCL()
        {
            try
            {
                this.cl = new EncogCL();
            }
            catch (Exception e)
            {
                throw new EncogEngineError(e);
            }
        }
#endif

        /// <summary>
        /// Provides any shutdown that Encog may need. Currently this shuts down the
        /// thread pool.
        /// </summary>
        ///
        public void Shutdown()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// If Encog is not using GPU/CL processing this attribute will benull. Otherwise it holds the Encog CL object.
        /// </summary>
        public EncogCL CL
        {
            get
            {
                return this.cl;
            }
        }
#endif

    }
}
