using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.MathUtil;

namespace Encog.App.Quant.Classify
{
    public class ClassifyCSV: BasicFile
    {
        private ClassifyStats classify;

        public ClassifyStats Stats
        {
            get
            {
                return this.classify;
            }
        }

        public bool KeyIsNumeric
        {
            get { return this.KeyIsNumeric; }            
        }

        public ClassifyCSV()
        {
            this.classify = new ClassifyStats();
            this.classify.High = 1;
            this.classify.Low = -1;
        }

        public void Analyze(String inputFile, bool headers, CSVFormat format,int classField)
        {
            IList<String> classesFound = new List<String>();
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;
            this.classify.ClassField = classField;

            this.Analyzed = true;

            int recordCount = 0;
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            while (csv.Next())
            {
                String key = csv.Get(classField);
                if (!classesFound.Contains(key))
                    classesFound.Add(key);
                recordCount++;
            }
            this.RecordCount = recordCount;
            this.ColumnCount = csv.GetColumnCount();

            ReadHeaders(csv);
            csv.Close();

            // determine if class is numeric
            this.classify.IsNumeric = true;
            foreach (String key in classesFound)
            {
                int d;
                if (!int.TryParse(key, out d))
                {
                    this.classify.IsNumeric = false;
                    break;
                }
            }

            // sort either by string or numeric
            this.classify.Classes.Clear();
            if (this.classify.IsNumeric)
            {
                // sort numeric
                int[] temp = new int[classesFound.Count];
                for (int i = 0; i < classesFound.Count; i++)
                    temp[i] = int.Parse(classesFound[i]);
                Array.Sort(temp);
                
                // create classes
                for (int i = 0; i < temp.Length; i++)
                {
                    this.classify.Classes.Add(new ClassItem(""+temp[i],i));
                }
            }
            else
            {
                // sort string
                String[] temp = new String[classesFound.Count];
                for (int i = 0; i < classesFound.Count; i++)
                    temp[i] = classesFound[i];
                Array.Sort(temp);

                // create classes
                for (int i = 0; i < temp.Length; i++)
                {
                    this.classify.Classes.Add(new ClassItem(temp[i], i));
                }
            }

            this.classify.Init();
        }

        private String EncodeOneOf(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < this.classify.Classes.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(this.InputFormat.Separator);
                }

                if (i == classNumber)
                {
                    result.Append(this.classify.High);
                }
                else
                {
                    result.Append(this.classify.Low);
                }
            }
            return result.ToString();
        }

        private String EncodeEquilateral(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            double[] d = this.classify.EquilateralEncode.Encode(classNumber);
            NumberList.ToList(this.InputFormat, result, d);
            return result.ToString();
        }

        private String EncodeSingleField(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            result.Append(classNumber);
            return result.ToString();
        }

        private String Encode(ClassifyMethod method, int classNumber)
        {
            switch (method)
            {
                case ClassifyMethod.OneOf:
                    return EncodeOneOf(classNumber);
                case ClassifyMethod.Equilateral:
                    return EncodeEquilateral(classNumber);
                case ClassifyMethod.SingleField:
                    return EncodeSingleField(classNumber);
                default:
                    return null;
            }
        }

        public void Process(String outputFile, ClassifyMethod method, int insertAt, bool includeOrigional)
        {
            ValidateAnalyzed();

            this.Stats.Method = method;
            TextWriter tw = this.PrepareOutputFile(outputFile);
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            this.classify.Init();

            while (csv.Next())
            {
                StringBuilder line = new StringBuilder();
                int classNumber = this.classify.Lookup( csv.Get(this.classify.ClassField) );
                bool inserted = false;

                for (int i = 0; i < this.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        line.Append(this.InputFormat.Separator);
                    }

                    if (insertAt == i)
                    {
                        line.Append(Encode(method,classNumber));                        
                        line.Append(this.InputFormat.Separator);
                        inserted = true;
                    }

                    if (!includeOrigional && i == this.classify.ClassField)
                    {
                        continue;
                    }

                    line.Append(csv.Get(i));
                }

                // if we failed to insert the class field anywhere, then insert it at the end
                if (!inserted)
                {
                    if( line[line.Length-1]!=this.InputFormat.Separator )
                        line.Append(this.InputFormat.Separator);
                    line.Append(Encode(method, classNumber));
                }

                tw.WriteLine(line.ToString());
            }

            csv.Close();
            tw.Close();

            this.classify.Init();
        }
    }
}
