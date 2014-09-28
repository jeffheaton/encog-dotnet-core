//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using Encog.Util.CSV;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    /// A class that reads csv columns.
    /// Useful if you need to read inputs from multiple CSV, or just need to read one input but not the rest..Less intensive on the memory usage.
    /// </summary>
    public class QuickCSVUtils
    {

        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="formatused">The formatused.</param>
        /// <param name="Name">The name of the column to parse..</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, CSVFormat formatused, string Name)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, formatused);
            while (csv.Next())
            {
                returnedArrays.Add(csv.GetDouble(Name));
            }
            return returnedArrays;
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name)
        {
            List<double> returnedArrays = new List<double>();

            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            while (csv.Next())
            {
                returnedArrays.Add(csv.GetDouble(Name));
            }
            return returnedArrays;
        }


        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name, int size)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            int currentRead = 0;
            while (csv.Next() && currentRead < size)
            {
                returnedArrays.Add(csv.GetDouble(Name));
                currentRead++;
            }
            return returnedArrays;
        }

        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read and the number of lines you want to skip from the start line
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="Name">The name of the column to parse.</param>
        /// <param name="size">The size.</param>
        /// <param name="StartLine">The start line (how many lines you want to skip from the start of the file.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, string Name, int size, int StartLine)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
          
            int currentRead = 0;
            int currentLine = 0;
            while (csv.Next())
            {
                if (currentRead < size && currentLine > StartLine)
                {
                    returnedArrays.Add(csv.GetDouble(Name));
                    currentRead++;
                }
                currentLine++;
            }
            return returnedArrays;
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="columnNumber">The column number to get.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, int columnNumber, int size)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            int currentRead = 0;
            while (csv.Next() && currentRead < size)
            {
                returnedArrays.Add(csv.GetDouble(columnNumber));
                currentRead++;
            }
            return returnedArrays;
        }

        /// <summary>
        /// use this method to find a date in your csv , and it will return the line number..
        /// This is useful for evaluation purpose when you need to find the line number so you can check against the real price and the network output prices.
        ///You must specify the DateFormat ("yyyy-MM-dd HH:mm:ss") for example.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="datetoFind">The date to find.</param>
        /// <param name="DateFormat">The date format.</param>
        /// <returns></returns>
        public static int QuickParseCSVForDate(string file, DateTime datetoFind, String DateFormat)
        {
           
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
           int currentLine = 0;
            csv.DateFormat = DateFormat;
            while (csv.Next())
            {
                    csv.GetDate(0);
                if (csv.GetDate(0) == datetoFind)
                {
                    return currentLine;
                }
                else
                    currentLine++;
            }
            return currentLine;
        }
        /// <summary>
        /// use this method to find a date in your csv , and it will return the line number..
        /// This is useful for evaluation purpose when you need to find the line number so you can check against the real price and the network output prices.
        /// You can specifiy which column number you date are with this method.
        /// You must specify the DateFormat ("yyyy-MM-dd HH:mm:ss") for example.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="datetoFind">The date to find.</param>
        /// <param name="DateFormat">The date format.</param>
        /// <param name="Columnnumber">The columnnumber.</param>
        /// <returns></returns>
        public static int QuickParseCSVForDate(string file, DateTime datetoFind, String DateFormat,int Columnnumber)
        {

            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            int currentLine = 0;
            csv.DateFormat = DateFormat;
            while (csv.Next())
            {
                DateTime x = csv.GetDate(Columnnumber);
                if (x == datetoFind)
                {
                    return currentLine;
                }
                else
                    currentLine++;
            }
            return currentLine;
        }
        /// <summary>
        /// parses one column of a csv and returns an array of doubles.
        /// you can only return one double array with this method.
        /// We are assuming CSVFormat english in this quick parse csv method.
        /// You can input the size (number of lines) to read , and the number of lines you want to skip start from the first line.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="columnNumber">The column number to get.</param>
        /// <param name="size">The size.</param>
        /// <param name="startLine">The start line (how many lines you want to skip from the first line.</param>
        /// <returns></returns>
        public static List<double> QuickParseCSV(string file, int columnNumber, int size, int startLine)
        {
            List<double> returnedArrays = new List<double>();
            ReadCSV csv = new ReadCSV(file, true, CSVFormat.English);
            int currentRead = 0;
            int currentLine = 0;
            while (csv.Next())
            {
                if (currentRead < size && currentLine > startLine)
                {
                    returnedArrays.Add(csv.GetDouble(columnNumber));
                    currentRead++;
                }
                currentLine++;
            }
            return returnedArrays;
        }
    }
}
