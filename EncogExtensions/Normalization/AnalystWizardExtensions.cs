using Encog.App.Analyst;
using Encog.App.Analyst.Wizard;
using System;
using System.Collections.Generic;
using System.Data;

namespace EncogExtensions.Normalization
{
    public static class AnalystWizardExtensions
    {


        public static void Wizard(this AnalystWizard wizard, DataSet data)
        {
            EncogAnalyst analyst = wizard.GetPrivateField<EncogAnalyst>("_analyst");
            int lagWindowSize = wizard.GetPrivateField<int>("_lagWindowSize");
            int leadWindowSize = wizard.GetPrivateField<int>("_leadWindowSize");

            wizard.SetPrivateField("_timeSeries", (lagWindowSize > 0 || leadWindowSize > 0));
           
            wizard.CallPrivateMethod("DetermineClassification");
            wizard.CallPrivateMethod("GenerateSettings");
            
           
            analyst.Analyze(data);


            wizard.CallPrivateMethod("GenerateNormalizedFields");
            wizard.CallPrivateMethod("GenerateSegregate");
            wizard.CallPrivateMethod("GenerateGenerate");
            wizard.CallPrivateMethod("GenerateTasks");

            if (wizard.GetPrivateField<bool>("_timeSeries") 
                && (lagWindowSize > 0) 
                && (leadWindowSize > 0))
            {
                wizard.CallPrivateMethod("ExpandTimeSlices");
            }
        }
    }
}
