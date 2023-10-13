using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CbWriteViewCalculator
{
	public class FormVariableEditer
	{

		private bool zzzCreatedFromVariable = false;
		public bool CreatedFromVariable { get { return this.zzzCreatedFromVariable; } }
		private Variable zzzTheVariable = null;
		public Variable TheVariable { get { return this.zzzTheVariable; } }

		private Form forme;
		private Label Text1;
		private TextBox tbName;
		private Label Text2;
		private TextBox tbValue;
		private wvCbButton2 OkButton;
		private wvCbButton2 CancelButton;
		private Label ErrorLabel;

		public enum ExitMethod
		{
			none,
			OkButton,
			CancelButton,
			WindowClosed
		}
		private ExitMethod zzzTheExitMethod = ExitMethod.none;
		public ExitMethod TheExitMethod { get { return this.zzzTheExitMethod; } }

		public string Title
		{
			get { return this.forme.Text; }
			set { this.forme.Text = value; }
		}
		public string VarName
		{
			get { return this.tbName.Text; }
			set { this.tbName.Text = value; }
		}
		public string strVarValue
		{
			get { return this.tbValue.Text; }
			set { this.tbValue.Text = value; }
		}



		public void ShowDialog()
		{

			if (this.CreatedFromVariable)
			{
				this.tbName.Text = this.TheVariable.Name;
				//this.tbValue.Text = this.TheVariable.Value.ToString();
				this.tbValue.Text = cWriteViewAssets.TrimStrNumber(cWriteViewAssets.RemoveSpace(this.TheVariable.Value.ToString("N20")));
			}

			this.forme.Top = Cursor.Position.Y - (this.forme.Height / 2);
			this.forme.Left = Cursor.Position.X - (this.forme.Width / 2);
			if (this.forme.Left < 0) { this.forme.Left = 0; }
			this.ErrorLabel.Visible = false;
			this.forme.ShowDialog();

		}

		//void new()
		private void voidnew()
		{
			this.forme = new Form();
			this.forme.MaximizeBox = false;
			this.forme.MinimizeBox = false;
			this.forme.StartPosition = FormStartPosition.Manual;
			this.forme.ShowInTaskbar = false;
			this.forme.Width = 400;
			this.forme.Height = 164; // 164
			this.forme.FormBorderStyle = FormBorderStyle.FixedDialog;

			this.Text1 = new Label();
			this.Text1.Parent = this.forme;
			this.Text1.Top = 1;
			this.Text1.Left = 1;
			this.Text1.AutoSize = true;
			this.Text1.Font = new Font("ms sans serif", 12f);
			this.Text1.Text = "Nom :";

			this.tbName = new TextBox();
			this.tbName.Parent = this.forme;
			this.tbName.Top = this.Text1.Top + this.Text1.Height + 1;
			this.tbName.Left = 1;
			this.tbName.BorderStyle = BorderStyle.FixedSingle;
			this.tbName.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.tbName.Width = this.forme.Width - 20 - this.tbName.Left;

			this.Text2 = new Label();
			this.Text2.Parent = this.forme;
			this.Text2.Top = this.tbName.Top + this.tbName.Height + 10;
			this.Text2.Left = 1;
			this.Text2.AutoSize = true;
			this.Text2.Font = this.Text1.Font;
			this.Text2.Text = "Valeur :";

			this.tbValue = new TextBox();
			this.tbValue.Parent = this.forme;
			this.tbValue.Top = this.Text2.Top + this.Text2.Height + 1;
			this.tbValue.Left = 1;
			this.tbValue.BorderStyle = BorderStyle.FixedSingle;
			this.tbValue.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.tbValue.Width = this.forme.Width - 20 - this.tbValue.Left;

			Size buttonsize = new Size(100, 28);
			this.OkButton = new wvCbButton2();
			this.OkButton.Parent = this.forme;
			this.OkButton.Size = buttonsize;
			this.OkButton.Top = this.tbValue.Top + this.tbValue.Height + 2;
			this.OkButton.Left = this.forme.Width - 18 - this.OkButton.Width;
			this.OkButton.Text = "Ok";
			this.OkButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			this.CancelButton = new wvCbButton2();
			this.CancelButton.Parent = this.forme;
			this.CancelButton.Size = buttonsize;
			this.CancelButton.Top = this.tbValue.Top + this.tbValue.Height + 2;
			this.CancelButton.Left = this.OkButton.Left - 2 - this.CancelButton.Width;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			this.ErrorLabel = new Label();
			this.ErrorLabel.Parent = this.forme;
			this.ErrorLabel.Text = "Error POIUYTREWQqtypdfghjklb";
			this.ErrorLabel.AutoSize = true;
			this.ErrorLabel.Left = 1;
			this.ErrorLabel.Top = this.forme.Height - 45 - this.ErrorLabel.Height;
			this.ErrorLabel.ForeColor = Color.Red;
			this.ErrorLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;


			this.forme.FormClosing += new FormClosingEventHandler(this.forme_FormClosing);
			this.OkButton.Click += new EventHandler(this.OkButton_Click);
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);
			this.tbName.TextChanged += new EventHandler(this.AnyTextBox_TextChanged);
			this.tbValue.TextChanged += new EventHandler(this.AnyTextBox_TextChanged);


		}
		public FormVariableEditer()
		{
			this.zzzCreatedFromVariable = false;
			this.zzzTheVariable = null;

			this.voidnew();
		}
		public FormVariableEditer(Variable StartVariable)
		{
			this.zzzCreatedFromVariable = true;
			this.zzzTheVariable = StartVariable;

			this.voidnew();
		}


		private void forme_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.zzzTheExitMethod == ExitMethod.none)
			{
				this.zzzTheExitMethod = ExitMethod.WindowClosed;
			}
		}
		private void OkButton_Click(object sender, EventArgs e)
		{
			bool isvalidname = cWriteViewAssets.IsAVariableName(this.tbName.Text);
			if (isvalidname)
			{
				bool isvalidvalue = cWriteViewAssets.IsANumber(this.tbValue.Text, true, true);
				if (isvalidvalue)
				{
					this.zzzTheExitMethod = ExitMethod.OkButton;
					this.forme.Close();
				}
				else
				{
					this.ShowError("Valeur de variable invalide.");
				}
			}
			else
			{
				this.ShowError("Nom de variable invalide.");
			}
		}
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.zzzTheExitMethod = ExitMethod.CancelButton;
			this.forme.Close();
		}
		private void AnyTextBox_TextChanged(object sender, EventArgs e)
		{
			this.ErrorLabel.Visible = false;
		}


		private void ShowError(string msg)
		{
			this.ErrorLabel.Text = msg;
			this.ErrorLabel.Visible = true;
		}


	}
}
