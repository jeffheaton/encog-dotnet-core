using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Persist.Location;

namespace Encog.Util.CL
{
    public class EncogKernel
    {
        public String Source { get; set; }

        public IDictionary<EncogCLAdapter, ComputeProgram> Programs
        {
            get
            {
                return this.programs;
            }
        }

        private Dictionary<EncogCLAdapter, ComputeProgram> programs = new Dictionary<EncogCLAdapter, ComputeProgram>();
        private EncogCL cl;

        public EncogKernel(EncogCL cl, String sourceFile)
        {
            ResourcePersistence resource = new ResourcePersistence(sourceFile);
            this.Source = resource.LoadString();
            this.cl = cl;
        }

        public void Execute(EncogCLAdapter adapter, CLPayload payload)
        {
            ComputeKernel kernel = this.Programs[adapter].CreateKernel("VectorAdd");

            int index = 0;

            foreach( EncogCLVector vec in payload.InputParams )
            {
                ComputeBuffer<float> p = new ComputeBuffer<float>(adapter.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, vec.Array);
                kernel.SetMemoryArgument(index++, p);
                vec.Buffer = p;
            }

            foreach ( EncogCLVector vec in payload.OutputParams)
            {
                ComputeBuffer<float> p = new ComputeBuffer<float>(adapter.Context, ComputeMemoryFlags.WriteOnly, vec.Array);
                kernel.SetMemoryArgument(index++, p);
                vec.Buffer = p;
            }

            ComputeEventList events = new ComputeEventList();

            adapter.Queue.Execute(kernel, null, new long[] { index }, null, events);

            foreach (EncogCLVector vec in payload.OutputParams)
            {
                vec.Array = adapter.Queue.Read(vec.Buffer, true, 0, vec.Array.Length, events);
            }
        }
    }
}
