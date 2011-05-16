namespace Encog.ML
{
    /// <summary>
    /// Defines a MLMethod that can handle autoassocation.  Autoassociation is a 
    /// simple form of pattern recognition where the MLMethod echos back the 
    /// exact pattern that the input most closely matches.  For example, if the 
    /// autoassociative MLMethod were trained to recognize an 8x8 grid of 
    /// characters, the return value would be the entire 8x8 grid of the 
    /// character recognized.
    /// This is the type of recognition performed by Hopfield Networks.  It is
    /// also an optional recognition form used by GR/PNN's.  This is a form of
    /// unsupervised training.
    /// </summary>
    ///
    public interface MLAutoAssocation : MLRegression
    {
    }
}