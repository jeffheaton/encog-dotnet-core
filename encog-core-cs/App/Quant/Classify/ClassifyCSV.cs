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
    /// <summary>
    /// Used to classify a CSV file.  Often a CSV file will contain a field that 
    /// specifies a class that a row belongs two.  This "class code"/"class number" must 
    /// be formatted to be processed by a neural network or other machine 
    /// learning construct.  
    /// </summary>
    public class ClassifyCSV: BasicFile
    {
        /// <summary>
        /// Holds stats on the field that is to be classified.
        /// </summary>
        private ClassifyStats classify;

        /// <summary>
        /// The stats on a field that is to be classified.
        /// </summary>
        public ClassifyStats Stats
        {
            get
            {
                return this.classify;
            }
        }

        /// <summary>
        /// True, if this field is numeric.
        /// </summary>
        public bool KeyIsNumeric
        {
            get { return this.KeyIsNumeric; }            
        }

        /// <summary>
        /// Construct the object and set the defaults.
        /// </summary>
        public ClassifyCSV()
        {
            this.classify = new ClassifyStats();
            this.classify.High = 1;
            this.classify.Low = -1;
        }

        /// <summary>
        /// Analyze the file.
        /// </summary>
        /// <param name="inputFile">The input file to analyze.</param>
        /// <param name="headers">True, if the input file has headers.</param>
        /// <param name="format">The format of the input file.</param>
        /// <param name="classField">The field to be classified.</param>
        public void Analyze(String inputFile, bool headers, CSVFormat format,int classField)
        {
            IList<String> classesFound = new List<String>();
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;
            this.classify.ClassField = classField;

            this.Analyzed = true;

            ResetStatus();
            int recordCount = 0;
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            while (csv.Next())
            {
                UpdateStatus(true);
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
            ReportDone(true);
        }

        /// <summary>
        /// Perform the encoding for "one of".
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded columns.</returns>
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

        /// <summary>
        /// Perform an equilateral encode.
        /// </summary>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The class to encode.</returns>
        private String EncodeEquilateral(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            double[] d = this.classify.EquilateralEncode.Encode(classNumber);
            NumberList.ToList(this.InputFormat, this.Precision, result, d);
            return result.ToString();
        }

        /// <summary>
        /// Encode a single field.
        /// </summary>
        /// <param name="classNumber">The class number to encode.</param>
        /// <returns>The encoded columns.</returns>
        private String EncodeSingleField(int classNumber)
        {
            StringBuilder result = new StringBuilder();
            result.Append(classNumber);
            return result.ToString();
        }

        /// <summary>
        /// Encode the class.
        /// </summary>
        /// <param name="method">The encoding method.</param>
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded class.</returns>
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

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The name of the output file.</param>
        /// <param name="orig">The name of original field.</param>
        /// <returns>The output stream for the text file.</returns>
        public TextWriter PrepareOutputFile(String outputFile, String originalName, int idx)
        {
            TextWriter tw = new StreamWriter(outputFile);

            // write headers, if needed
            if (ExpectInputHeaders)
            {
                int index = 0;
                StringBuilder line = new StringBuilder();
                foreach (String str in this.InputHeadings)
                {
                    if (index == idx)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(",");
                        }

                        line.Append("\"");
                        line.Append(originalName);
                        line.Append("\"");
                    }

                    if (line.Length > 0)
                    {
                        line.Append(",");
                    }
                    line.Append("\"");
                    line.Append(this.InputHeadings[index++]);
                    line.Append("\"");
                }
                tw.WriteLine(line.ToString());
            }

            return tw;
        }

        /// <summary>
        /// Process the file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <param name="method">The classification method.</param>
        /// <param name="insertAt">The column to insert the classified columns at, 
        /// or -1 for the end.</param>
        /// <param name="origionalName">If not null, include original column and name it this.  Usually null.</param>
        public void Process(String outputFile, ClassifyMethod method, int insertAt, String originalName)
        {
            TextWriter tw;

            ValidateAnalyzed();

            this.Stats.Method = method;
            
            if( originalName==null )
                tw = this.PrepareOutputFile(outputFile);
            else
                tw = this.PrepareOutputFile(outputFile, originalName, this.classify.ClassField);

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            this.classify.Init();

            ResetStatus();
            while (csv.Next())
            {
                UpdateStatus(false);
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

                    if (originalName==null && i == this.classify.ClassField)
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
            ReportDone(false);
            this.classify.Init();
        }
    }
}
