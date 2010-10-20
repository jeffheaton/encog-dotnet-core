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
using System.Windows.Shapes;

namespace EncogOpenCLBenchmark
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public String UID { get; set; }
        public String PWD { get; set; }

        public Login()
        {
            InitializeComponent();
        }

        private void DialogShare_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            UID = TextUID.Text;
            PWD = TextPWD.Password;
        }
    }
}
