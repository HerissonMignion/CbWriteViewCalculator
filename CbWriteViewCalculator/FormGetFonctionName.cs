using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CbWriteViewCalculator
{
	public class FormGetFonctionName
	{
		
		private Form forme;
		private Label LabelText;
		private TextBox InputTextBox;
		private wvCbButton2 OkButton;
		private wvCbButton2 CancelButton;
		private Label ErrorLabel;


		public string Title
		{
			get { return this.forme.Text; }
			set { this.forme.Text = value; }
		}
		public string Message
		{
			get { return this.LabelText.Text; }
			set { this.LabelText.Text = value; }
		}
		public string Answer
		{
			get { return this.InputTextBox.Text; }
			set { this.InputTextBox.Text = value; }
		}
		public string InvalidNameMsg = "Nom de fonction invalide";

		public enum ExitMethod
		{
			none,
			CancelButton,
			OkButton
		}
		private ExitMethod zzzTheExitMethod = ExitMethod.none;
		public ExitMethod TheExitMethod
		{
			get { return this.zzzTheExitMethod; }
		}


		public void ShowDialog()
		{
			Point mpos = Cursor.Position;
			//position de la form
			this.forme.Location = new Point(mpos.X - (this.forme.Width / 2), mpos.Y - (this.forme.Height / 2));
			if (this.forme.Left < 0) { this.forme.Left = 0; }
			if (this.forme.Top < 0) { this.forme.Top = 0; }
			//affiche la form
			this.HideError();
			this.forme.ShowDialog();
		}
		public void Close()
		{
			this.forme.Close();
		}



		//void new()
		public FormGetFonctionName()
		{
			this.forme = new Form();
			this.forme.Size = new Size(400, 110);
			this.forme.MinimizeBox = false;
			this.forme.MaximizeBox = false;
			this.forme.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.forme.ShowInTaskbar = false;
			this.forme.ShowIcon = false;
			this.forme.Text = "no title";
			this.forme.StartPosition = FormStartPosition.Manual;

			this.LabelText = new Label();
			this.LabelText.Parent = this.forme;
			this.LabelText.Location = new Point(0, 0);
			this.LabelText.Text = "no message";
			this.LabelText.Font = new Font("ms sans serif", 10f);
			this.LabelText.AutoSize = true;

			this.InputTextBox = new TextBox();
			this.InputTextBox.Parent = this.forme;
			this.InputTextBox.Left = 1;
			this.InputTextBox.Top = this.LabelText.Top + this.LabelText.Height + 3;
			this.InputTextBox.Font = new Font("ms sans serif", 10f);
			this.InputTextBox.Width = this.forme.Width - 17 - this.InputTextBox.Left;
			this.InputTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.InputTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.InputTextBox.TextChanged += new EventHandler(this.InputTextBox_TextChanged);
			this.InputTextBox.KeyDown += new KeyEventHandler(this.InputTextBox_KeyDown);

			this.OkButton = new wvCbButton2();
			this.OkButton.Parent = this.forme;
			this.OkButton.Width = 80;
			this.OkButton.Top = this.InputTextBox.Top + this.InputTextBox.Height + 1;
			this.OkButton.Height = this.forme.Height - 40 - this.OkButton.Top;
			this.OkButton.Left = this.forme.Width - 17 - this.OkButton.Width;
			this.OkButton.Text = "Ok";
			this.OkButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.OkButton.Click += new EventHandler(this.OkButton_Click);

			this.CancelButton = new wvCbButton2();
			this.CancelButton.Parent = this.forme;
			this.CancelButton.Width = 80;
			this.CancelButton.Top = this.OkButton.Top;
			this.CancelButton.Height = this.OkButton.Height;
			this.CancelButton.Left = this.OkButton.Left - 1 - this.CancelButton.Width;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);

			this.ErrorLabel = new Label();
			this.ErrorLabel.Parent = this.forme;
			this.ErrorLabel.Text = "no message";
			this.ErrorLabel.ForeColor = Color.Red;
			this.ErrorLabel.Left = 0;
			this.ErrorLabel.Top = this.InputTextBox.Top + this.InputTextBox.Height + 3;
			this.ErrorLabel.AutoSize = true;






		}
		private void InputTextBox_TextChanged(object sender, EventArgs e)
		{
			this.HideError();
		}
		private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.OkButtonVoid();
			}
		}
		private void OkButton_Click(object sender, EventArgs e)
		{
			this.OkButtonVoid();
		}
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.zzzTheExitMethod = ExitMethod.CancelButton;
			this.Close();
		}

		private void OkButtonVoid()
		{
			//check si le nom entré est valide. s'il ne l'est pas, il affiche une erreur à l'utilisateur
			if (cWriteViewAssets.IsAVariableName(this.Answer))
			{
				this.zzzTheExitMethod = ExitMethod.OkButton;
				this.Close();
			}
			else
			{
				this.ShowError(this.InvalidNameMsg);
			}
		}





		public void ShowError(string msg)
		{
			this.ErrorLabel.Text = msg;
			this.ErrorLabel.Visible = true;
		}
		public void HideError()
		{
			this.ErrorLabel.Visible = false;
		}


	}
}
