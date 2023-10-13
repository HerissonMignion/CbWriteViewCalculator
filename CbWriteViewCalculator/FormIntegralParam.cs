using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public class FormIntegralParam
	{
		private EQoIntegral zzzTheInte;
		public EQoIntegral TheInte { get { return this.zzzTheInte; } }


		private Form forme;
		private Label Text1;
		private TextBox tbSubdivWidth;
		private wvCbButton2 OkButton;
		private wvCbButton2 CancelButton;



		public void ShowDialog()
		{
			//optien la position de la souris à l'écran
			Point mpos = Cursor.Position;
			//aligne la forme sur la souris
			this.forme.Left = mpos.X - this.forme.Width / 2;
			this.forme.Top = mpos.Y - this.forme.Height / 2;
			//showdialog
			this.forme.ShowDialog();
		}


		//void new()
		public FormIntegralParam(EQoIntegral StartInte)
		{
			this.zzzTheInte = StartInte;

			this.forme = new Form();
			this.forme.StartPosition = FormStartPosition.Manual;
			this.forme.ShowIcon = false;
			this.forme.ShowInTaskbar = false;
			this.forme.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.forme.Text = "Paramètres de l'intégrale";
			this.forme.MaximizeBox = false;
			this.forme.MinimizeBox = false;
			this.forme.Size = new Size(200, 99); // 200 99

			this.Text1 = new Label();
			this.Text1.Parent = this.forme;
			this.Text1.Text = "Max Subdivision Width =";
			this.Text1.Location = new Point(0, 0);
			this.Text1.AutoSize = true;

			this.tbSubdivWidth = new TextBox();
			this.tbSubdivWidth.Parent = this.forme;
			this.tbSubdivWidth.Left = 1;
			this.tbSubdivWidth.Top = this.Text1.Top + this.Text1.Height;
			this.tbSubdivWidth.Width = this.forme.Width - 30;
			this.tbSubdivWidth.BorderStyle = BorderStyle.FixedSingle;
			string num = cWriteViewAssets.RemoveSpace(this.TheInte.MaxSubdivisionWidth.ToString("N20"));
			num = cWriteViewAssets.TrimStrNumber(num);
			this.tbSubdivWidth.Text = num;
			this.tbSubdivWidth.BackColor = Color.White;
			this.tbSubdivWidth.ForeColor = Color.Black;
			this.tbSubdivWidth.TextChanged += new EventHandler(this.tbSubdivWidth_TextChanged);
			this.tbSubdivWidth.KeyDown += new KeyEventHandler(this.tbSubdivWidth_KeyDown);

			this.OkButton = new wvCbButton2();
			this.OkButton.Parent = this.forme;
			//this.OkButton.Size = new Size(70, 25);
			this.OkButton.Width = (this.forme.Width - 18) / 2;
			this.OkButton.Height = 25; // 25
			this.OkButton.Top = this.tbSubdivWidth.Top + this.tbSubdivWidth.Height + 1;
			this.OkButton.Left = this.forme.Width - 17 - this.OkButton.Width;
			this.OkButton.Text = "Valider";
			this.OkButton.Click += new EventHandler(this.OkButton_Click);

			this.CancelButton = new wvCbButton2();
			this.CancelButton.Parent = this.forme;
			this.CancelButton.Left = 1;
			this.CancelButton.Top = this.OkButton.Top;
			this.CancelButton.Height = this.OkButton.Height;
			this.CancelButton.Width = this.OkButton.Left - this.CancelButton.Left - 1;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);
			
		}

		private void tbSubdivWidth_TextChanged(object sender, EventArgs e)
		{
			this.tbSubdivWidth.BackColor = Color.White;
			this.tbSubdivWidth.ForeColor = Color.Black;
		}
		private void tbSubdivWidth_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) { this.VoidOk(); }
		}
		private void OkButton_Click(object sender, EventArgs e)
		{
			this.VoidOk();
			this.tbSubdivWidth.Focus();
		}
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.forme.Close();
		}




		

		private void VoidOk()
		{
			string strrep = this.tbSubdivWidth.Text;
			if (cWriteViewAssets.IsANumber(strrep, true, false))
			{
				string trimrep = cWriteViewAssets.TrimStrNumber(strrep);
				if (trimrep == "0") { MessageBox.Show("Max Subdivision Width ne peut pas être 0"); }
				else
				{
					double dblval = Convert.ToDouble(trimrep.Replace(".", ","));
					this.TheInte.MaxSubdivisionWidth = dblval;
					this.forme.Close();
				}
			}
			else
			{
				this.tbSubdivWidth.BackColor = Color.Crimson;
				this.tbSubdivWidth.ForeColor = Color.White;
			}
		}




	}
}
