using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Tree;

namespace Encog.ML
{
    public class MLDelegates
    {
        public delegate bool TreeTraversalTask(ITreeNode node);
    }
}
