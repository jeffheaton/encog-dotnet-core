using Encog.App.Analyst;
using Encog.App.Analyst.Analyze;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using System;
using System.Collections.Generic;
using System.Data;

namespace EncogExtensions.Normalization
{
    public class PerformAnalysis
    {
        private Encog.App.Analyst.Analyze.PerformAnalysis _performAnalysis;
        private AnalystScript _script;
        private AnalyzedField[] _fields;

        public PerformAnalysis(AnalystScript script)
        {
            _script = script;
        }

        public void Process(EncogAnalyst target, DataSet data)
        {
            var count = 0;

            // pass one, calculate the min/max
            count = PerformCalculateMinMax(data, count);

            // pass two, standard deviation
            count = PerformStandardDeviation(data, count);

            String str = _script.Properties.GetPropertyString(ScriptProperties.SetupConfigAllowedClasses) ?? "";

            bool allowInt = str.Contains("int");
            bool allowReal = str.Contains("real") || str.Contains("double");
            bool allowString = str.Contains("string");


            // remove any classes that did not qualify
            RemoveUnqualifiedClasses(allowInt, allowReal, allowString);

            // merge with existing
            MergeWithExisting(target);

            // now copy the fields
            CopyDataFields(target);
        }

        private void CopyDataFields(EncogAnalyst target)
        {
            var df = new DataField[_fields.Length];

            for (int i_4 = 0; i_4 < df.Length; i_4++)
            {
                df[i_4] = _fields[i_4].FinalizeField();
            }

            target.Script.Fields = df;
        }

        private void MergeWithExisting(EncogAnalyst target)
        {
            if ((target.Script.Fields != null)
                && (_fields.Length == target.Script.Fields.Length))
            {
                for (int i = 0; i < _fields.Length; i++)
                {
                    // copy the old field name
                    _fields[i].Name = target.Script.Fields[i].Name;

                    if (_fields[i].Class)
                    {
                        IList<AnalystClassItem> t = _fields[i].AnalyzedClassMembers;
                        IList<AnalystClassItem> s = target.Script.Fields[i].ClassMembers;

                        if (s.Count == t.Count)
                        {
                            for (int j = 0; j < s.Count; j++)
                            {
                                if (t[j].Code.Equals(s[j].Code))
                                {
                                    t[j].Name = s[j].Name;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveUnqualifiedClasses(bool allowInt, bool allowReal, bool allowString)
        {
            foreach (AnalyzedField field in _fields)
            {
                if (field.Class)
                {
                    if (!allowInt && field.Integer)
                    {
                        field.Class = false;
                    }

                    if (!allowString && (!field.Integer && !field.Real))
                    {
                        field.Class = false;
                    }

                    if (!allowReal && field.Real && !field.Integer)
                    {
                        field.Class = false;
                    }
                }
            }
        }

        private int PerformCalculateMinMax(DataSet data, int count)
        {
            for (int counter = 0; counter < data.Tables[0].Rows.Count; counter++)
            {
                if (_fields == null)
                {
                    GenerateFields(data.Tables[0].Columns);
                }

                var columnCount = data.Tables[0].Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    if (_fields != null)
                    {
                        var str = Convert.ToString(data.Tables[0].Rows[counter][i]);
                        _fields[i].Analyze1(str);
                    }
                }
                count++;
            }

            if (count == 0)
            {
                throw new AnalystError("Can't analyse data, it is empty.");
            }

            if (_fields != null)
            {
                foreach (AnalyzedField field in _fields)
                {
                    field.CompletePass1();
                }
            }

            return count;
        }

        private int PerformStandardDeviation(DataSet data, int count)
        {
            for (int counter = 0; counter < data.Tables[0].Rows.Count; counter++)
            {
                var columnCount = data.Tables[0].Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    if (_fields != null)
                    {
                        _fields[i].Analyze2(Convert.ToString(data.Tables[0].Rows[0][i]));
                    }
                }
                count++;
            }

            if (_fields != null)
            {
                foreach (AnalyzedField field in _fields)
                {
                    field.CompletePass2();
                }
            }

            return count;
        }

        private void GenerateFields(DataColumnCollection columns)
        {
            _fields = new AnalyzedField[columns.Count];
            for (int i = 0; i < _fields.Length; i++)
            {
                _fields[i] = new AnalyzedField(_script, "field:" + (i + 1));
            }
        }
    }
}