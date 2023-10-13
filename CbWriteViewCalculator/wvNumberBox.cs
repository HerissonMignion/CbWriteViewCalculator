using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public class wvNumberBox : TextBox
	{


		public double Value
		{
			get
			{
				double rep = 0;
				string srep = this.Text;
				srep = srep.Replace(".", ",");
				if (srep.Length < 1 || srep == "-") { srep = "0"; }
				try
				{
					rep = Convert.ToDouble(srep);
				}
				catch { rep = 0; }
				return rep;
			}
			set
			{
				this.Text = value.ToString().Replace(",", ".");
			}
		}

		private string LastText = "0";


		

		private bool OnTextChangedEnabled = true;
		protected override void OnTextChanged(EventArgs e)
		{
			if (this.OnTextChangedEnabled)
			{
				this.OnTextChangedEnabled = false;


				string NewText = this.Text.Replace(",", ".");
				bool IsNewTextANumber = cWriteViewAssets.IsANumber(NewText, true, true);

				if (IsNewTextANumber)
				{
					this.LastText = NewText;
					base.OnTextChanged(e);
				}
				else
				{
					this.Text = this.LastText;
				}
				
				this.OnTextChangedEnabled = true;
			}
		}





		//void new()
		public wvNumberBox()
		{
			this.Text = "0";
		}

	}
}
