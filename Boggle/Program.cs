using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Boggle
{
    static class Program
    {
        /**
        * Main entry point. Creates a BoggleForm and runs it.
        * 
        * @author Jason Allen
        */
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BoggleForm());
        }
    }
}
