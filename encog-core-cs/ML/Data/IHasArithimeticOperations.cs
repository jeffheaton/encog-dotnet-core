using System;
namespace Encog.ML.Data
{
    public interface IHasArithimeticOperations
    {
        IMLData Minus(IMLData o);
        IMLData Plus(IMLData o);
        IMLData Times(double d);
    }
}
