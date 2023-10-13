using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public class FormShowAskText
	{

		private Form forme;
		private TextBox tbText;
		private wvCbButton2 SelectAllButton;
		private wvCbButton2 OkButton;
		private wvCbButton2 CancelButton;
		private Label MessageLabel;


		public string Title
		{
			get { return this.forme.Text; }
			set { this.forme.Text = value; }
		}
		public string Message
		{
			get { return this.MessageLabel.Text; }
			set { this.MessageLabel.Text = value; }
		}
		public string Text
		{
			get { return this.tbText.Text; }
			set { this.tbText.Text = value; }
		}
		public int Width
		{
			get { return this.forme.Width; }
			set { this.forme.Width = value; }
		}

		public enum saMode
		{
			Show,
			Ask
		}
		private saMode zzzActualMode;
		public saMode ActualMode { get { return this.zzzActualMode; } }
		public void SetMode(saMode NewMode)
		{
			this.zzzActualMode = NewMode;
			if (NewMode == saMode.Show)
			{
				this.SelectAllButton.Visible = true;
				this.CancelButton.Visible = false;
				this.OkButton.Visible = false;
				this.tbText.ReadOnly = true;
				this.MessageLabel.Visible = false;
			}
			if (NewMode == saMode.Ask)
			{
				this.SelectAllButton.Visible = false;
				this.CancelButton.Visible = true;
				this.OkButton.Visible = true;
				this.tbText.ReadOnly = false;
				this.MessageLabel.Visible = true;
			}
		}

		public enum saExitMethod
		{
			none,
			Ok,
			Cancel
		}
		private saExitMethod zzzTheExitMethod = saExitMethod.none;
		public saExitMethod TheExitMethod { get { return this.zzzTheExitMethod; } }






		public void ShowDialog()
		{
			Point mpos = Cursor.Position;
			this.forme.Top = mpos.Y - (this.forme.Height / 2);
			if (this.forme.Top < 0) { this.forme.Top = 0; }
			this.forme.Left = mpos.X - (this.forme.Width / 2);
			this.forme.ShowDialog();
		}



		//void new()
		public FormShowAskText()
		{
			this.forme = new Form();
			this.forme.StartPosition = FormStartPosition.Manual;
			this.forme.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.forme.MinimizeBox = false;
			this.forme.MaximizeBox = false;
			this.forme.ShowInTaskbar = false;
			this.forme.Size = new Size(400, 95); // 500 , 95

			this.tbText = new TextBox();
			this.tbText.Parent = this.forme;
			this.tbText.Font = new Font("consolas", 12f);
			this.tbText.Left = 1;
			this.tbText.Top = this.forme.Height - 40 - this.tbText.Height;
			this.tbText.Width = this.forme.Width - 17 - this.tbText.Left;
			this.tbText.BorderStyle = BorderStyle.FixedSingle;
			this.tbText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.tbText.KeyDown += new KeyEventHandler(this.tbText_KeyDown);

			int buttonheight = this.tbText.Top - 2;

			this.SelectAllButton = new wvCbButton2();
			this.SelectAllButton.Parent = this.forme;
			this.SelectAllButton.Text = "Select All";
			this.SelectAllButton.Location = new Point(1, 1);
			this.SelectAllButton.Height = buttonheight;
			this.SelectAllButton.Width = 90;
			this.SelectAllButton.Click += new EventHandler(this.SelectAllButton_Click);

			this.OkButton = new wvCbButton2();
			this.OkButton.Parent = this.forme;
			this.OkButton.Width = 50;
			this.OkButton.Height = buttonheight;
			this.OkButton.Top = 1;
			this.OkButton.Left = this.forme.Width - 17 - this.OkButton.Width;
			this.OkButton.Text = "Ok";
			this.OkButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.OkButton.Click += new EventHandler(this.OkButton_Click);

			this.CancelButton = new wvCbButton2();
			this.CancelButton.Parent = this.forme;
			this.CancelButton.Width = 50;
			this.CancelButton.Height = buttonheight;
			this.CancelButton.Top = 1;
			this.CancelButton.Left = this.OkButton.Left - 1 - this.CancelButton.Width;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);

			this.MessageLabel = new Label();
			this.MessageLabel.Parent = this.forme;
			this.MessageLabel.Location = new Point(1, 1);
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.Text = "notext";
			this.MessageLabel.Font = new Font("ms sans serif", 10f);


		}


		private void SelectAllButton_Click(object sender, EventArgs e)
		{
			this.tbText.SelectAll();
			this.tbText.Focus();
		}
		private void OkButton_Click(object sender, EventArgs e)
		{
			this.voidOk();
		}
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.zzzTheExitMethod = saExitMethod.Cancel;
			this.forme.Close();
		}
		private void tbText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.voidOk();
			}
		}




		private void voidOk()
		{
			if (this.ActualMode == saMode.Ask)
			{
				this.zzzTheExitMethod = saExitMethod.Ok;
				this.forme.Close();
			}
		}



	}
}
