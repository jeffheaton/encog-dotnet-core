using Encog.App.Analyst;
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Script.Normalize;
using Encog.Util.Arrayutil;
using System;
using System.Data;

namespace EncogExtensions.Normalization
{
    public class AnalystNormalizeDataSet
    {
        private EncogAnalyst _analyst;

        public AnalystNormalizeDataSet(EncogAnalyst analyst)
        {
            this._analyst = analyst;
        }

        public double[,] Normalize(DataSet dataSet)
        {
            int outputLength = _analyst.DetermineTotalColumns();
            double[,] result = new double[dataSet.Tables[0].Rows.Count, outputLength];

            for (var irow = 0; irow < dataSet.Tables[0].Rows.Count; irow++)
            {
                var normalizedRowFields = ExtractFields(_analyst, dataSet.Tables[0].Rows[irow], outputLength, false);
                if (normalizedRowFields != null)
                {
                    for (var iNormColumn = 0; iNormColumn < outputLength; iNormColumn++)
                    {
                        result[irow, iNormColumn] = normalizedRowFields[iNormColumn];
                    }
                }
            }

            return result;
        }

        private double[] ExtractFields(EncogAnalyst analyst, DataRow rowData, int outputLength, bool v)
        {
            var output = new double[outputLength];
            int outputIndex = 0;

            foreach (AnalystField stat in analyst.Script.Normalize.NormalizedFields)
            {
                stat.Init();
                if (stat.Action == NormalizationAction.Ignore)
                {
                    continue;
                }

                var index = Array.FindIndex(analyst.Script.Fields, x => x.Name == stat.Name);
                var str = rowData[index].ToString();
                // is this an unknown value?
                if (str.Equals("?") || str.Length == 0)
                {
                    IHandleMissingValues handler = analyst.Script.Normalize.MissingValues;
                    double[] d = handler.HandleMissing(analyst, stat);

                    // should we skip the entire row
                    if (d == null)
                    {
                        return null;
                    }

                    // copy the returned values in place of the missing values
                    for (int i = 0; i < d.Length; i++)
                    {
                        output[outputIndex++] = d[i];
                    }
                }
                else
                {
                    // known value

                    if (stat.Action == NormalizationAction.Normalize)
                    {
                        double d = double.Parse(str.Trim());
                        d = stat.Normalize(d);
                        output[outputIndex++] = d;
                    }
                    else if (stat.Action == NormalizationAction.PassThrough)
                    {
                        double d = double.Parse(str);
                        output[outputIndex++] = d;
                    }
                    else
                    {
                        double[] d = stat.Encode(str.Trim());

                        foreach (double element in d)
                        {
                            output[outputIndex++] = element;
                        }
                    }
                }
            }

            return output;
        }

        /*
                private double[] ExtractFields(EncogAnalyst _analyst, DataSet dataSet, bool skipOutput)
                {
                    var output = new List<double>();
                    int outputIndex = 0;

                    foreach (AnalystField stat in _analyst.Script.Normalize.NormalizedFields)
                    {
                        stat.Init();
                        if (stat.Action == NormalizationAction.Ignore)
                        {
                            continue;
                        }

                        if (stat.Output && skipOutput)
                        {
                            continue;
                        }

                        if(stat.Action == NormalizationAction.Normalize)
                        {
                            output[outputIndex] =
                        }
                    }
                }
          */
    }
}