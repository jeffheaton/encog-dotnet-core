/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Opencl
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Cloo;
    using Encog.Engine.Opencl.Exceptions;

    /// <summary>
    /// An OpenCL platform. A platform is a collection of OpenCL devices from the
    /// same vendor. Often, you will have only a single platform.
    /// </summary>
    ///
    public class EncogCL
    {
        /// <summary>
        /// The platforms detected.
        /// </summary>
        ///
        private readonly IList<EncogCLPlatform> platforms;

        /// <summary>
        /// All devices, from all platforms.
        /// </summary>
        ///
        private readonly IList<EncogCLDevice> devices;

        /// <summary>
        /// Construct an Encog OpenCL object.
        /// </summary>
        ///
        public EncogCL()
        {
            this.platforms = new List<EncogCLPlatform>();
            this.devices = new List<EncogCLDevice>();
            try
            {
                if (ComputePlatform.Platforms.Count == 0)
                    throw new EncogEngineError("Can't find any OpenCL platforms");

                foreach (ComputePlatform platform in ComputePlatform.Platforms)
                {
                    EncogCLPlatform encogPlatform = new EncogCLPlatform(platform);
                    platforms.Add(encogPlatform);
                    foreach (EncogCLDevice device in encogPlatform.Devices)
                    {
                        devices.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MissingOpenCLAdapterError(ex);
            }
        }


        /// <returns>True if CPUs are present.</returns>
        public bool AreCPUsPresent()
        {
            /* foreach */
            foreach (EncogCLDevice device in this.devices)
            {
                if (device.CPU)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Choose a device. If a GPU is found, return that. Otherwise try to find a
        /// CPU.
        /// </summary>
        ///
        /// <returns>The first device detected.</returns>
        public EncogCLDevice ChooseDevice()
        {
            EncogCLDevice result = ChooseDevice(true);
            if (result == null)
            {
                result = ChooseDevice(false);
            }
            return result;
        }

        /// <summary>
        /// Choose a device. Simply returns the first device detected.
        /// </summary>
        ///
        /// <param name="useGPU">Do we prefer to use the GPU?</param>
        /// <returns>The first device detected.</returns>
        public EncogCLDevice ChooseDevice(bool useGPU)
        {

            /* foreach */
            foreach (EncogCLDevice device in this.devices)
            {
                if (useGPU && !device.CPU)
                {
                    return device;
                }
                else if (!useGPU && device.CPU)
                {
                    return device;
                }
            }
            return null;
        }

        /// <summary>
        /// Disable all devices that are CPU's. This is a good idea to do if you are
        /// going to use regular CPU processing in conjunction with OpenCL processing
        /// where the CPU is made to look like a OpenCL device. Otherwise, you end up
        /// with the CPU serving as both a "regular CPU training task" and as an
        /// "OpenCL training task".
        /// </summary>
        ///
        public void DisableAllCPUs()
        {
            /* foreach */
            foreach (EncogCLDevice device in this.devices)
            {
                if (device.CPU)
                {
                    device.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Enable all devices that are CPU's.
        /// </summary>
        ///
        public void EnableAllCPUs()
        {
            /* foreach */
            foreach (EncogCLDevice device in this.devices)
            {
                if (device.CPU)
                {
                    device.Enabled = true;
                }
            }
        }


        /// <summary>
        /// The devices used by EncogCL, this spans over platform boundaries.
        /// </summary>
        public IList<EncogCLDevice> Devices
        {
           get
            {
                return this.devices;
            }
        }



        /// <summary>
        /// The devices used by EncogCL, this spans over platform boundaries.Does not include disabled devices or devices from disabled platforms.
        /// </summary>
        public IList<EncogCLDevice> EnabledDevices
        {
            get
            {
                IList<EncogCLDevice> result = new List<EncogCLDevice>();

                foreach (EncogCLDevice device in this.devices)
                {
                    if (device.Enabled && device.Platform.Enabled)
                    {
                        result.Add(device);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// All platforms detected.
        /// </summary>
        public IList<EncogCLPlatform> Platforms
        {
            get
            {
                return this.platforms;
            }
        }


        /// <summary>
        /// All platforms detected.
        /// </summary>
        /// <returns>A list of all platforms.</returns>
        public override System.String ToString()
        {
            StringBuilder result = new StringBuilder();
            
            foreach (EncogCLDevice device in this.devices)
            {
                result.Append(((String)device.ToString()));
                result.Append("\n");
            }
            return result.ToString();
        }

    }
}
