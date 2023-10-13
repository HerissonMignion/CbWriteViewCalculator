using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public class panelEQoNumberEdit
	{
		private EQoNumber zzzTheNumber;
		public EQoNumber TheNumber { get { return this.zzzTheNumber; } }



		public int Width
		{
			get { return this.forme.Width; }
			set
			{
				Point last = this.UpCenter;
				this.forme.Width = value;
				this.UpCenter = last;
			}
		}


		public Point UpCenter
		{
			get
			{
				return new Point(this.forme.Left + (this.forme.Width / 2), this.forme.Top);
			}
			set
			{
				this.forme.Top = value.Y;
				this.forme.Left = value.X - (this.forme.Width / 2);
			}
		}


		private Form forme;
		private wvNumberBox numbox;
		private wvCbButton2 CancelButton;
		private wvCbButton2 OkButton;


		public void ShowDialog()
		{
			this.RefreshTextSize();
			this.numbox.Text = this.TheNumber.ActualStrValue;
			this.forme.ShowDialog();
		}



		private void RefreshTextSize()
		{
			this.CancelButton.Top = this.numbox.Top + this.numbox.Height + 1;
			this.CancelButton.Left = this.numbox.Left;
			this.CancelButton.Height = this.forme.Height - this.CancelButton.Top - 1;
			this.CancelButton.Width = (this.forme.Width - 3) / 2;

			this.OkButton.Top = this.CancelButton.Top;
			this.OkButton.Left = this.CancelButton.Left + this.CancelButton.Width + 1;
			this.OkButton.Height = this.CancelButton.Height;
			this.OkButton.Width = this.forme.Width - this.OkButton.Left - 1;

		}



		//void new()
		public panelEQoNumberEdit(EQoNumber StartNumber)
		{
			this.zzzTheNumber = StartNumber;

			this.forme = new Form();
			this.forme.StartPosition = FormStartPosition.Manual;
			this.forme.MinimumSize = new Size(1, 1);
			this.forme.FormBorderStyle = FormBorderStyle.None;
			this.forme.Height = 42;
			//this.forme.Width = 200;
			this.forme.ShowInTaskbar = false;

			this.numbox = new wvNumberBox();
			this.numbox.Parent = this.forme;
			this.numbox.Location = new Point(1, 1);
			this.numbox.Width = this.forme.Width - this.numbox.Left - 1;
			this.numbox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.numbox.KeyDown += new KeyEventHandler(this.numbox_KeyDown);
			this.numbox.BorderStyle = BorderStyle.FixedSingle;

			this.CancelButton = new wvCbButton2();
			this.CancelButton.Parent = this.forme;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);

			this.OkButton = new wvCbButton2();
			this.OkButton.Parent = this.forme;
			this.OkButton.Text = "Done";
			this.OkButton.Click += new EventHandler(this.OkButton_Click);



		}


		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.forme.Close();
		}
		private void OkButton_Click(object sender, EventArgs e)
		{
			this.TheNumber.ActualStrValue = this.numbox.Text;
			this.forme.Close();
		}

		private void numbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.TheNumber.ActualStrValue = this.numbox.Text;
				this.forme.Close();
			}
		}



	}
}
