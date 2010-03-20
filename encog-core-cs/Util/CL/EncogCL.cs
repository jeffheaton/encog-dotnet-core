using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    public class EncogCL
    {
        private ComputeContextPropertyList cp;
        private IList<EncogCLAdapter> adapters = new List<EncogCLAdapter>();

        public EncogCL()
        {
            for (int i = 0; i < ComputePlatform.Platforms.Count; i++)
            {
                ComputePlatform platform = ComputePlatform.Platforms[i];
                EncogCLAdapter adapter = new EncogCLAdapter(platform);
                this.adapters.Add(adapter);
            }
        }

        public void Init()
        {
            foreach( EncogCLAdapter adapter in this.adapters )
            {
                if( adapter.Enabled )
                    adapter.Init();
            }
        }

        public IList<EncogCLAdapter> Adapters
        {
            get
            {
                return this.adapters;
            }
        }

        public EncogCLAdapter ChooseAdapter()
        {
            foreach (EncogCLAdapter adapter in adapters)
            {
                if (adapter.Enabled)
                    return adapter;
            }

            return null;
        }
    }
}
