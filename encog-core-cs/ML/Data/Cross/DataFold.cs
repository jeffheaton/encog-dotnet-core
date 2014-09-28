using Encog.ML.Data.Versatile;

namespace Encog.ML.Data.Cross
{
    /// <summary>
    /// </summary>
    public class DataFold
    {
        private readonly MatrixMLDataSet _training;
        private readonly MatrixMLDataSet _validation;

        public DataFold(MatrixMLDataSet theTraining, MatrixMLDataSet theValidation)
        {
            _training = theTraining;
            _validation = theValidation;
        }

        public double Score { get; set; }
        public IMLMethod Method { get; set; }

        public MatrixMLDataSet Validation
        {
            get { return _validation; }
        }

        public MatrixMLDataSet Training
        {
            get { return _training; }
        }
    }
}