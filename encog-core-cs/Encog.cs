// Encog(tm) Artificial Intelligence Framework v2.4
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if logging
using log4net;
using log4net.Config;
using Encog.Util.CL;
#endif

namespace Encog
{
    /// <summary>
    /// Main Encog class, does little more than provide version information.
    /// Also used to hold the ORM session that Encog uses to work with
    /// Hibernate.
    /// </summary>
    public class Encog
    {
		/// <summary>
        /// The current engog version, this should be read from the properties.
		/// </summary>
	    public static String VERSION = "2.4.0";

        /// <summary>
        /// The platform.
        /// </summary>
        public static String PLATFORM = "DotNet";
	
	    /// <summary>
        /// The current engog file version, this should be read from the properties.
	    /// </summary>
	    private static String FILE_VERSION = "1";

        /// <summary>
        /// If Encog is not using GPU/CL processing this attribute will be null.  
        /// Otherwise it holds the Encog CL object.
        /// </summary>
        public EncogCL CL { get; set; }
		
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(Encog));
#endif

        /// <summary>
        /// The default precision to use for compares.
        /// </summary>
        public const int DEFAULT_PRECISION = 10;

        /// <summary>
        /// Default point at which two doubles are equal.
        /// </summary>
        public const double DEFAULT_DOUBLE_EQUAL = 0.0000001;

        /// <summary>
        /// The version of the Encog JAR we are working with. Given in the form
        /// x.x.x.
        /// </summary>
        public const String ENCOG_VERSION = "encog.version";

        /// <summary>
        /// The encog file version. This determines of an encog file can be read.
        /// This is simply an integer, that started with zero and is incramented each
        /// time the format of the encog data file changes.
        /// </summary>
        public static String ENCOG_FILE_VERSION = "encog.file.version";

        /// <summary>
        /// The instance.
        /// </summary>
        private static Encog instance;

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        public static Encog Instance
        {
            get
            {
                if (Encog.instance == null)
                {
                    Encog.instance = new Encog();
                }
                return Encog.instance;
            }
        }

        /// <summary>
        /// Get the properties as a Map.
        /// </summary>
        private IDictionary<String, String> properties =
            new Dictionary<String, String>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Encog()
        {
            this.properties[Encog.ENCOG_VERSION] = VERSION;
            this.properties[Encog.ENCOG_FILE_VERSION] = FILE_VERSION;
        }

        /// <summary>
        /// The Encog properties.  Contains version information.
        /// </summary>
        public IDictionary<String, String> Properties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>
        /// Enable OpenCL processing.  OpenCL processing allows Encog to 
        /// use GPU devices to speed calculations.  Not all areas of Encog 
        /// can use this, however, GPU's can currently accelerate the 
        /// training of Feedforward neural networks.
        /// 
        /// To make use of the GPU you must have OpenCL drivers installed.
        /// For more information on getting OpenCL drivers, visit the following
        /// URL.
        /// 
        /// http://www.heatonresearch.com/encog/opencl
        /// </summary>
        public void InitCL()
        {
            try
            {
                EncogCL cl = new EncogCL();
                this.CL = cl;
            }
            catch (Exception e)
            {
                throw new EncogCLError(e);
            }
        }

    }
}
