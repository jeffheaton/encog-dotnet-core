using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace Encog.Util.CL
{
    public class EncogCL
    {
        private ComputeContextPropertyList cp;
        private IList<EncogCLAdapter> adapters = new List<EncogCLAdapter>();

        public void Init()
        {
            for(int i=0;i<ComputePlatform.Platforms.Count;i++)
            {
                ComputePlatform platform = ComputePlatform.Platforms[i];
                EncogCLAdapter adapter = new EncogCLAdapter(platform);
                this.adapters.Add(adapter);
            }
        }

        public IList<EncogCLAdapter> Adapters
        {
            get
            {
                return this.adapters;
            }
        }

        public void Compile(EncogKernel kernel)
        {
            foreach(EncogCLAdapter adapter in this.adapters)
            {
                adapter.Compile(kernel);
            }
        }
    }
}
