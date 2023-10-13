using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public class panelEQoUserFunctionEdit
	{
		private EQoUserFunction zzzTheFunction;
		public EQoUserFunction TheFunction { get { return this.zzzTheFunction; } }




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
		private TextBox namebox;
		private wvCbButton2 CancelButton;
		private wvCbButton2 OkButton;


		public void ShowDialog()
		{
			this.RefreshTextSize();
			this.namebox.Text = this.TheFunction.Name;
			this.forme.ShowDialog();
		}



		private void RefreshTextSize()
		{
			this.CancelButton.Top = this.namebox.Top + this.namebox.Height + 1;
			this.CancelButton.Left = this.namebox.Left;
			this.CancelButton.Height = this.forme.Height - this.CancelButton.Top - 1;
			this.CancelButton.Width = (this.forme.Width - 3) / 2;

			this.OkButton.Top = this.CancelButton.Top;
			this.OkButton.Left = this.CancelButton.Left + this.CancelButton.Width + 1;
			this.OkButton.Height = this.CancelButton.Height;
			this.OkButton.Width = this.forme.Width - this.OkButton.Left - 1;

		}


		
		//void new()
		public panelEQoUserFunctionEdit(EQoUserFunction StartVariable)
		{
			this.zzzTheFunction = StartVariable;

			this.forme = new Form();
			this.forme.StartPosition = FormStartPosition.Manual;
			this.forme.MinimumSize = new Size(1, 1);
			this.forme.FormBorderStyle = FormBorderStyle.None;
			this.forme.Height = 42;
			//this.forme.Width = 200;
			this.forme.ShowInTaskbar = false;

			this.namebox = new TextBox();
			this.namebox.Parent = this.forme;
			this.namebox.Location = new Point(1, 1);
			this.namebox.Width = this.forme.Width - this.namebox.Left - 1;
			this.namebox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.namebox.KeyDown += new KeyEventHandler(this.numbox_KeyDown);
			this.namebox.BorderStyle = BorderStyle.FixedSingle;

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
			this.voidOkButton();
		}
		private void numbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.voidOkButton();
			}

		}

		private void voidOkButton()
		{
			if (this.namebox.Text.Length <= 0)
			{
				this.namebox.Text = "noname";
			}
			else
			{
				if (cWriteViewAssets.IsAVariableName(this.namebox.Text))
				{
					this.TheFunction.Name = this.namebox.Text;
					this.forme.Close();
				}
				else
				{
					MessageBox.Show("Invalid variable name.");
				}
			}
		}


	}
}
