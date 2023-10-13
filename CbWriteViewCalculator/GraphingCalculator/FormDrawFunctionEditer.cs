using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator.GraphingCalculator
{
	//nouvelle fenêtre dans laquelle l'user peut modifier les propriété et l'équation d'un DrawFunction
	public class FormDrawFunctionEditer
	{
		private DrawFunction zzzDF = null;
		public DrawFunction DF
		{
			get { return this.zzzDF; }
		}



		private Form forme;
		private wvCbButton2 btnValidate;
		private Label lblName;
		private TextBox tbName;
		private oEquationEditer EquE;




		private void PrepareToShow()
		{
			//met le nom du df dans le textbox du nom
			this.EventActivated_tnName_TextChanged = false;
			this.tbName.Text = this.DF.Name;
			this.EventActivated_tnName_TextChanged = true;


		}
		public void Show()
		{
			this.PrepareToShow();
			this.forme.Show();
		}
		public void ShowDialog()
		{
			this.PrepareToShow();
			this.forme.ShowDialog();
		}





		private void voidnew()
		{
			this.forme = new Form();
			this.forme.Size = new Size(630, 500);
			this.forme.ShowIcon = false;
			this.forme.Text = "Modifier une équation";
			this.forme.MinimizeBox = false;
			this.forme.FormClosed += new FormClosedEventHandler(this.forme_FormClosed);
			

			this.btnValidate = new wvCbButton2();
			this.btnValidate.Parent = this.forme;
			this.btnValidate.Location = new Point(1, 1);
			this.btnValidate.Size = new Size(70, 30);
			this.btnValidate.Text = "Validate";
			this.btnValidate.MouseUp += new MouseEventHandler(this.btnValidate_MouseUp);
			this.btnValidate.KeyDown += new KeyEventHandler(this.btnValidate_KeyDown);

			this.lblName = new Label();
			this.lblName.Parent = this.forme;
			this.lblName.Top = 2;
			this.lblName.Left = this.btnValidate.Left + this.btnValidate.Width + 20;
			this.lblName.Text = "Nom :";
			this.lblName.AutoSize = true;
			this.lblName.Font = new Font("microsoft sans serif", 15f);

			this.tbName = new TextBox();
			this.tbName.Parent = this.forme;
			this.tbName.Top = 1;
			this.tbName.Left = this.lblName.Left + this.lblName.Width + 2;
			this.tbName.Font = new Font("microsoft sans serif", 15f);
			this.tbName.Width = this.forme.Width - 20 - 5 - this.tbName.Left;
			this.tbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.tbName.TextChanged += new EventHandler(this.tbName_TextChanged);
			this.tbName.KeyDown += new KeyEventHandler(this.tbName_KeyDown);


			this.EquE = new oEquationEditer(this.DF.Equ);
			this.EquE.Parent = this.forme;
			this.EquE.Top = this.tbName.Top + this.tbName.Height + 2;
			this.EquE.Left = 1;
			this.EquE.Width = this.forme.Width - 17 - this.EquE.Left;
			this.EquE.Height = this.forme.Height - 40 - this.EquE.Top;
			this.EquE.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;


		}
		public FormDrawFunctionEditer(DrawFunction sDF)
		{
			this.zzzDF = sDF;

			this.voidnew();
			
		}

		private void forme_FormClosed(object sender, FormClosedEventArgs e)
		{
			//on fait désélectionner tout les objet pour reset leur BackColor
			this.EquE.UnselectAllObject(false);

			//la règle de la fonction a sûrement été changé
			this.DF.RecompileNeeded();

			//signale que un ou des truc on été changé
			this.DF.Inform_SomethingChanged();
			
		}

		private void btnValidate_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.forme.Close();
			}
		}
		private void btnValidate_KeyDown(object sender, KeyEventArgs e)
		{
			this.EquE.Exec_KeyDown(e.KeyCode);
		}

		private bool EventActivated_tnName_TextChanged = true; //mettre sur false pour ne pas réagir à l'event text changed du textbox
		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (this.EventActivated_tnName_TextChanged)
			{

				//si l'user change le text du nom, on change le nom du df
				this.DF.Name = this.tbName.Text;


			}
		}
		private void tbName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.btnValidate.Focus();
			}
		}






	}
}
