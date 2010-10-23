using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Opencl;
using Encog.Util.Logging;

namespace Encog.Examples.CL
{
    public class CLInfo : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(CLInfo),
                    "opencl-info",
                    "OpenCL Information.",
                    "Dump basic OpenCL info to console.");
                return info;
            }
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            Logging.StopConsoleLogging();
            try
            {
                EncogFramework.Instance.InitCL();

                EncogCL cl = EncogFramework.Instance.CL;

                foreach (EncogCLPlatform platform in cl.Platforms)
                {
                    app.WriteLine("Found platform: " + platform.Name);
                    foreach (EncogCLDevice device in platform.Devices)
                    {
                        app.WriteLine("Found device: " + device.ToString());
                    }
                }


            }
            catch (EncogCLError e)
            {
                app.WriteLine("OpenCL Error: " + e.Message);
            }
        }
    }
}
