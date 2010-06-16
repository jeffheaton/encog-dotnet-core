using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TuneEncogOpenCL
{
    static class EncogOpenCLTune
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool runningWin32NT = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? true : false;
            bool consoleAllocated = false;

            try
            {
                if (runningWin32NT) consoleAllocated = AllocConsole();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EncogTuneForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Clootils Error");
            }

            if (runningWin32NT && consoleAllocated) FreeConsole();
        }
    }
}
