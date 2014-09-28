using System;
using System.Text;
using Encog.ML.Data.Versatile;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Normalizers;
using Encog.ML.Data.Versatile.Normalizers.Strategy;
using Encog.ML.Factory;

namespace Encog.ML.Model.Config
{
    /// <summary>
    ///     Config class for EncogModel to use a RBF neural network.
    /// </summary>
    public class RBFNetworkConfig : IMethodConfig
    {
        /// <inheritdoc />
        public String MethodName
        {
            get { return MLMethodFactory.TypeRbfnetwork; }
        }

        /// <inheritdoc />
        public String SuggestModelArchitecture(VersatileMLDataSet dataset)
        {
            int inputColumns = dataset.NormHelper.InputColumns.Count;
            int outputColumns = dataset.NormHelper.OutputColumns.Count;
            var hiddenCount = (int) ((inputColumns + outputColumns)*1.5);
            var result = new StringBuilder();

            result.Append("?->gaussian(c=");
            result.Append(hiddenCount);
            result.Append(")->?");
            return result.ToString();
        }

        /// <inheritdoc />
        public INormalizationStrategy SuggestNormalizationStrategy(VersatileMLDataSet dataset, string architecture)
        {
            int outputColumns = dataset.NormHelper.OutputColumns.Count;

            ColumnType ct = dataset.NormHelper.OutputColumns[0].DataType;

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
            return "rprop";
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