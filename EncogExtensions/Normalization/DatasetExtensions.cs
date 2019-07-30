using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EncogExtensions.Normalization
{
    public static class DatasetExtensions
    {
        public static DataSet Convert(this DataSet dataSet, List<LoadedMarketData> data, string dataSetName = "dataset")
        {
            if (data.Count < 1)
            {
                throw new ArgumentOutOfRangeException("Loaded Market Data passed to DataSet.Convert() method appears to be empty (Contains 0 Rows).");
            }

            var resultDataSet = new DataSet("dataset");
            DataTable table = new DataTable("Market Data Table");

            var dataRowCount = data.Count();
            var initialDataColumns = new List<DataColumn>();

            AddInitialColumn(initialDataColumns, "StockSymbol", typeof(String));
            AddInitialColumn(initialDataColumns, "Day", typeof(int));
            AddInitialColumn(initialDataColumns, "Month", typeof(int));
            AddInitialColumn(initialDataColumns, "Year", typeof(int));
            CopyInitialColumnsToTable(table, initialDataColumns);

            var dataColumnInitialIndex = initialDataColumns.Count;

            foreach (KeyValuePair<MarketDataType, double> column in data[0].Data)
            {
                DataColumn dataColumn = new DataColumn(column.Key.ToString());
                dataColumn.DataType = column.Value.GetType();
                table.Columns.Add(dataColumn);
                dataColumnInitialIndex++;
            }

            for (var dataRowIndex = 0; dataRowIndex < dataRowCount; dataRowIndex++)
            {
                var row = table.NewRow();
                row[0] = data[dataRowIndex].Ticker.Symbol; // stock symbol
                row[1] = data[dataRowIndex].When.Day;
                row[2] = data[dataRowIndex].When.Month;
                row[3] = data[dataRowIndex].When.Year;

                var dataColumnIndex = initialDataColumns.Count;

                foreach (KeyValuePair<MarketDataType, double> entry in data[dataRowIndex].Data)
                {
                    row[dataColumnIndex] = entry.Value;
                    dataColumnIndex++;
                }

                table.Rows.Add(row);
            }

            resultDataSet.Tables.Add(table);

            return resultDataSet;
        }

        private static void CopyInitialColumnsToTable(DataTable table, List<DataColumn> initialDataColumns)
        {
            table.Columns.AddRange(initialDataColumns.ToArray());
        }

        private static void AddInitialColumn(List<DataColumn> initialDataColumns, string name, Type dataType)
        {
            DataColumn InitialColumn = null;
            InitialColumn = new DataColumn(name);
            InitialColumn.DataType = dataType;
            initialDataColumns.Add(InitialColumn);
        }
    }
}