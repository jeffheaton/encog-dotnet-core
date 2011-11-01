using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSVanalyze.Properties;

namespace CSVanalyze
{
    public class SetSettings
    {



        #region change and view settings
        public static void ViewSettings()
        {

            Settings mysettings = new Settings();
            Console.WriteLine("Current Input Neurons :" + mysettings.InputSize);
            Console.WriteLine("Current Output Neurons :" + mysettings.OutPutSize);
            Console.WriteLine("Current Hidden Neuron 1 :" + mysettings.Hidden1);
            Console.WriteLine("Current Hidden Neuron 2 :" + mysettings.Hidden2);
            Console.WriteLine("Current Starting Date :" + mysettings.StartDate.ToShortDateString());
            Console.WriteLine("Current Ending Date :" + mysettings.EndDate.ToShortDateString());
            Console.WriteLine("Current Evaluation Date Start:" + mysettings.EvalStartDate.ToShortDateString());
            Console.WriteLine("Current Evaluation Date Start:" + mysettings.EvalEndDate.ToShortDateString());
            Console.WriteLine("Current Output all to :" + mysettings.OutputsAll);

        }


        public static void EditASetting(string aSettingName, string NewValue)
        {
            Settings mysettings = new Settings();



            try
            {
                Console.WriteLine("Trying to change " + aSettingName + " New value:" + NewValue);

                mysettings.SettingChanging += new System.Configuration.SettingChangingEventHandler(mysettings_SettingChanging);
                if (aSettingName == "From")
                {
                    mysettings.StartDate = DateTime.Parse(NewValue);
                }
                if (aSettingName == "Input")
                {
                    mysettings.InputSize = Int16.Parse(NewValue);
                }
                if (aSettingName == "Output")
                {
                    mysettings.OutPutSize = Int16.Parse(NewValue);
                }

                if (aSettingName == "Hidden1")
                {

                    mysettings.Hidden1 = Int16.Parse(NewValue);
                }
                if (aSettingName == "OutputsAll")
                {

                    mysettings.OutputsAll = bool.Parse(NewValue);
                }
                if (aSettingName == "Hidden2")
                {
                    mysettings.Hidden2 = Int16.Parse(NewValue);
                }
                if (aSettingName == "To")
                {
                    mysettings.EndDate = DateTime.Parse(NewValue);
                }
                if (aSettingName == "EvalStart")
                {
                    mysettings.EvalStartDate = DateTime.Parse(NewValue);
                }
                if (aSettingName == "EvalEnd")
                {
                    mysettings.EvalEndDate = DateTime.Parse(NewValue);
                }
                mysettings.Save();

            }


            catch (Exception ex)
            {
                Console.WriteLine("Error trying to edit settings:" + ex.Message);
            }

        }

        static void mysettings_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            Console.WriteLine("Saved new settings :" + e.SettingName + " New Value:" + e.NewValue);
        }



        #endregion



    }
}
