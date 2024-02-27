using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.valloon.ValloonShot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    var process = Process.GetProcessById(int.Parse(args[0]));
                    var LastHandle = process.MainWindowHandle;
                    Form1.ShowWindow(LastHandle, Form1.SW_RESTORE);
                    Form1.SetForegroundWindow(LastHandle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.ToString());
                }
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
