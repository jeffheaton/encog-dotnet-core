using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.App.Analyst;
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Wizard;
using Encog.Util.File;

namespace EncogCmd
{
    public class EncogCmd
    {
        private ParseCommand cmd;

        private void WizardCommand()
        {
            String targetCSVFile = cmd.Args[0];

            String egaFile = FileUtil.ForceExtension(targetCSVFile, "ega");
	
				String egFile = FileUtil.ForceExtension(targetCSVFile, "eg");

				EncogAnalyst analyst = new EncogAnalyst();
				AnalystWizard wizard = new AnalystWizard(analyst);
                bool headers = cmd.PromptBoolean("headers"); 
				AnalystFileFormat format = dialog.getFormat();

				wizard.MethodType = dialog.getMethodType();
            wizard.TargetField = cmd.PromptString("targetField");
				
				String m = (String)dialog.getMissing().getSelectedValue(); 
				if( m.Equals("DiscardMissing") ) {
					wizard.Missing = new DiscardMissing();	
				} else if( m.Equals("MeanAndModeMissing") ) {
					wizard.Missing = new MeanAndModeMissing();	
				} else if( m.Equals("NegateMissing") ) {
					wizard.Missing = new NegateMissing();	
				} else {
					wizard.Missing = new DiscardMissing();
				}
				
				wizard.Goal = dialog.getGoal();
            wizard.LagWindowSize = cmd.PromptInteger("lagWindow");
            wizard.LeadWindowSize = cmd.PromptInteger("leadWindow");
            wizard.IncludeTargetField = cmd.PromptBoolean("includeTarget");
				wizard.Range = dialog.getRange();
                wizard.TaskNormalize = cmd.PromptBoolean("normalize");
				wizard.TaskRandomize = cmd.PromptBoolean("randomize");
				wizard.TaskSegregate = cmd.PromptBoolean("segregate");
				wizard.TaskBalance = cmd.PromptBoolean("balance");
                wizard.TaskCluster = cmd.PromptBoolean("cluster");

            wizard.Wizard();
        }
        static void Main(string[] args)
        {
            ParseCommand cmd = new ParseCommand(args);

            if( cmd.Command == null )
            {
                Console.WriteLine("Usage:");
            }
            else if( cmd.Command.Equals("wizard") )
            {
                
            }
            else if (cmd.Command.Equals("analyst"))
            {

            }
        }
    }
}
