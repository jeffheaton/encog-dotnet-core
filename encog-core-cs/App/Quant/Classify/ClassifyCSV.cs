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

        public bool KeyIsNumeric
        {
            get { return this.KeyIsNumeric; }            
        }

        public IList<ClassItem> Classes { get { return this.Classes; } }
        public int ClassField { get; set; }
        public double High { get; set; }
        public double Low { get; set; }

        private bool keyIsNumeric;
        private IList<ClassItem> classes = new List<ClassItem>();
        private Equilateral eq;
        private IDictionary<String, int> lookup = new Dictionary<String, int>();

        public ClassifyCSV()
        {
            High = 1;
            Low = -1;
        }

        public void Analyze(String inputFile, bool headers, CSVFormat format,int classField)
        {
            IList<String> classesFound = new List<String>();
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;
            this.ClassField = classField;

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
            this.keyIsNumeric = true;
            foreach (String key in classesFound)
            {
                int d;
                if (!int.TryParse(key, out d))
                {
                    this.keyIsNumeric = false;
                    break;
                }
            }

            // sort either by string or numeric
            if (keyIsNumeric)
            {
                // sort numeric
                int[] temp = new int[classesFound.Count];
                for (int i = 0; i < classesFound.Count; i++)
                    temp[i] = int.Parse(classesFound[i]);
                Array.Sort(temp);

                // create classes
                for (int i = 0; i < temp.Length; i++)
                {
                    this.classes.Add(new ClassItem(""+temp[i],i));
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
                    this.classes.Add(new ClassItem(temp[i], i));
                }
            }

            // build lookup map
            for (int i = 0; i < classes.Count; i++)
            {
                this.lookup[classes[i].Name] = this.lookup.Count;
            }
        }

        private String EncodeOneOf(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < this.classes.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(this.InputFormat.Separator);
                }

                if (i == classNumber)
                {
                    result.Append(High);
                }
                else
                {
                    result.Append(Low);
                }
            }
            return result.ToString();
        }

        private String EncodeEquilateral(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            double[] d = this.eq.Encode(classNumber);
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

            TextWriter tw = this.PrepareOutputFile(outputFile);
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            this.eq = new Equilateral(this.classes.Count, High, Low);

            while (csv.Next())
            {
                StringBuilder line = new StringBuilder();
                int classNumber = this.lookup[csv.Get(this.ClassField)];
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

                    if (!includeOrigional && i == this.ClassField)
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
        }
    }
}
