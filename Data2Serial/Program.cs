using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Data2Serial
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool ok;
            System.Threading.Mutex m = new System.Threading.Mutex(true, "Data2Serial", out ok);

            if (!ok)
            {
                MessageBox.Show("Another instance is already running.","Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));

            GC.KeepAlive(m);   
        }
    }
}
