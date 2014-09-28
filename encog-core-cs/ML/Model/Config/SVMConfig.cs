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
    ///     Config class for EncogModel to use an SVM.
    /// </summary>
    public class SVMConfig: IMethodConfig
    {
        /// <inheritdoc />
        public String MethodName
        {
            get { return MLMethodFactory.TypeSVM; }
        }


        /// <inheritdoc />
        public String SuggestModelArchitecture(VersatileMLDataSet dataset)
        {
            int outputColumns = dataset.NormHelper.OutputColumns.Count;

            if (outputColumns > 1)
            {
                throw new EncogError("SVM does not support multiple output columns.");
            }

            ColumnType ct = dataset.NormHelper.OutputColumns[0].DataType;
            var result = new StringBuilder();
            result.Append("?->");
            result.Append(ct == ColumnType.Nominal ? "C" : "R");
            result.Append("->?");
            return result.ToString();
        }

        /// <inheritdoc />
        public INormalizationStrategy SuggestNormalizationStrategy(VersatileMLDataSet dataset, string architecture)
        {
            int outputColumns = dataset.NormHelper.OutputColumns.Count;

            if (outputColumns > 1)
            {
                throw new EncogError("SVM does not support multiple output columns.");
            }

            ColumnType ct = dataset.NormHelper.OutputColumns[0].DataType;

            var result = new BasicNormalizationStrategy();
            result.AssignInputNormalizer(ColumnType.Continuous, new RangeNormalizer(0, 1));
            result.AssignInputNormalizer(ColumnType.Nominal, new OneOfNNormalizer(0, 1));
            result.AssignInputNormalizer(ColumnType.Ordinal, new OneOfNNormalizer(0, 1));

            result.AssignOutputNormalizer(ColumnType.Continuous, new RangeNormalizer(0, 1));
            result.AssignOutputNormalizer(ColumnType.Nominal, new IndexedNormalizer());
            result.AssignOutputNormalizer(ColumnType.Ordinal, new OneOfNNormalizer(0, 1));
            return result;
        }


        /// <inheritdoc />
        public string SuggestTrainingType()
        {
            return MLTrainFactory.TypeSVM;
        }


        /// <inheritdoc />
        public String SuggestTrainingArgs(string trainingType)
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