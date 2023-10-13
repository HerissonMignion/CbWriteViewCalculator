using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CbWriteViewCalculator
{
    static class Program
    {
		public static void wdebug(object text) { System.Diagnostics.Debug.WriteLine(text); }


		public enum NextFormToShow
		{
			none,
			Calculator,
			GraphingCalculator
		}
		public static NextFormToShow NextForm = NextFormToShow.GraphingCalculator; // Calculator
		public static Form MainForm;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



			while (Program.NextForm != NextFormToShow.none)
			{
				NextFormToShow nf = Program.NextForm;
				Program.NextForm = NextFormToShow.none;

				if (nf == NextFormToShow.Calculator)
				{
					Program.MainForm = new Form1();
					Application.Run(Program.MainForm);
				}
				else if (nf == NextFormToShow.GraphingCalculator)
				{
					Program.MainForm = new GraphingCalculator.FormGraphingCalculator();
					Application.Run(Program.MainForm);

				}
			}



        }
    }
	

}
