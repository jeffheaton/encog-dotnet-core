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
using System.IO;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for EditScript.xaml
    /// </summary>
    public partial class EditScript : UserControl
    {
        public EditScript()
        {
            InitializeComponent();
        }

        public void Save(String filename)
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter(filename);

            // write a line of text to the file
            tw.WriteLine(this.TextArea.Text);

            // close the stream
            tw.Close();
        }

        public void Load(String filename)
        {
            TextReader tr = new StreamReader(filename);
            String str = tr.ReadToEnd();
            this.TextArea.Text = str;
            tr.Close();
        }
    }
}
