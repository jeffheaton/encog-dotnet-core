using System;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Normalizers;
using Encog.ML.Data.Versatile.Normalizers.Strategy;
using Encog.ML.Factory;

namespace Encog.ML.Model.Config
{
    /// <summary>
    ///     Config class for EncogModel to use a NEAT neural network.
    /// </summary>
    public class NEATConfig : IMethodConfig
    {
        /// <inheritdoc />
        public String MethodName
        {
            get { return MLMethodFactory.TypeNEAT; }
        }

        /// <inheritdoc />
        public String SuggestModelArchitecture(VersatileMLDataSet dataset)
        {
            return ("cycles=4");
        }

        /// <inheritdoc />
        public INormalizationStrategy SuggestNormalizationStrategy(VersatileMLDataSet dataset, String architecture)
        {
            var result = new BasicNormalizationStrategy();
            result.AssignInputNormalizer(ColumnType.Continuous, new RangeNormalizer(0, 1));
            result.AssignInputNormalizer(ColumnType.Nominal, new OneOfNNormalizer(0, 1));
            result.AssignInputNormalizer(ColumnType.Ordinal, new OneOfNNormalizer(0, 1));

            result.AssignOutputNormalizer(ColumnType.Continuous, new RangeNormalizer(0, 1));
            result.AssignOutputNormalizer(ColumnType.Nominal, new OneOfNNormalizer(0, 1));
            result.AssignOutputNormalizer(ColumnType.Ordinal, new OneOfNNormalizer(0, 1));
            return result;
        }


        /// <inheritdoc />
        public String SuggestTrainingType()
        {
            return "neat-ga";
        }


        /// <inheritdoc />
        public String SuggestTrainingArgs(String trainingType)
        {
            return "";
        }

        /// <inheritdoc />
        public int DetermineOutputCount(VersatileMLDataSet dataset)
        {
            return dataset.NormHelper.CalculateNormalizedOutputCount();
        }
    }
}