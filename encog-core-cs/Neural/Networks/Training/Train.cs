using Encog.ML.Train;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// This is an alias class for Encog 2.5 compatibility.  This class aliases 
    /// MLTrain.  Newer code should use MLTrain in place of this class.
    /// </summary>
    ///
    public interface Train : MLTrain
    {
    }
}