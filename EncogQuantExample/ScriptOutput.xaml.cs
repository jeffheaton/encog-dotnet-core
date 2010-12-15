using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Encog.Engine;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for ScriptOutput.xaml
    /// </summary>
    public partial class ScriptOutput : UserControl, IStatusReportable
    {
        public ScriptOutput()
        {
            InitializeComponent();
        }

        public void Report(int total, int current, string message)
        {
            TextOutput.AppendText(message + "\n");
        }

        public void Clear()
        {
            this.TextOutput.Clear();
        }
    }
}
