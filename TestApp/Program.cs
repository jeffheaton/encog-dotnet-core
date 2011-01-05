using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;

namespace TestApp
{
    /// <summary>
    /// This app is really just a "sandbox" to test various things as Encog is developed.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            NinjaStreamWriter nsw = new NinjaStreamWriter();
            nsw.Open("c:\\temp\\output.txt",true,CSVFormat.ENGLISH);
            for (int i = 0; i < 10; i++)
            {
                nsw.BeginBar(DateTime.Now.AddDays(i));
                nsw.StoreColumn("open", i);
                nsw.StoreColumn("close", i);
                nsw.EndBar();
            }
            nsw.Close();
        }
    }
}
