using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Thermometer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            using (Mutex mutex = new Mutex(false, "Thermometer"))
            {
                if (mutex.WaitOne(0, false))
                {
                    Application.Run(new MainForm());
                }
                else // App already launched.
                {
                    MessageBox.Show("This application is already launched!", "Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
    }
}
