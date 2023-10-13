using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CbWriteViewCalculator
{
	public class FormFonctionEditer
	{
		private oFonction zzzTheFonction = null;
		public oFonction TheFonction
		{
			get { return this.zzzTheFonction; }
		}
		private oEquationCalculContext zzzTheCalculContext = null;
		public oEquationCalculContext TheCalculContext
		{
			get { return this.zzzTheCalculContext; }
		}


		private Form forme;
		private wvCbButton2 TagButton;
		private oEquationEditer TheEditer;

		private Label LabelFuncName;
		private TextBox TextBoxFunctionName;

		private Label LabelFuncParams;
		private TextBox TextBoxParam;


		public void Show()
		{
			this.RefreshTextSize();
			this.forme.Show();
		}

		public bool AllowParamEdition
		{
			get { return this.TextBoxParam.Enabled; }
			set { this.TextBoxParam.Enabled = value; }
		}


		

		//void new()
		public FormFonctionEditer(oFonction StartFonction, oEquationCalculContext StartCalculContext)
		{
			this.zzzTheFonction = StartFonction;
			this.zzzTheCalculContext = StartCalculContext;

			this.forme = new Form();
			this.forme.Size = new Size(620, 500); // 600, 600
			this.forme.MinimizeBox = false;
			this.forme.MaximizeBox = false;
			this.forme.ShowIcon = false;
			this.forme.FormClosed += new FormClosedEventHandler(this.forme_FormClosed);


			this.TagButton = new wvCbButton2();
			this.TagButton.Parent = this.forme;
			this.TagButton.Size = new Size(50, 25);
			this.TagButton.Text = "Tag";
			this.TagButton.Top = 1;
			this.TagButton.Left = this.forme.Width - 17 - this.TagButton.Width;
			this.TagButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.TagButton.MouseClick += new MouseEventHandler(this.TagButton_MouseClick);
			this.TagButton.KeyDown += new KeyEventHandler(this.TagButton_KeyDown);

			//nom
			this.LabelFuncName = new Label();
			this.LabelFuncName.Parent = this.forme;
			this.LabelFuncName.Font = new Font("ms sans serif", 9f);
			this.LabelFuncName.Text = "Nom de la fonction :";
			this.LabelFuncName.Location = new Point(1, 5);
			this.LabelFuncName.AutoSize = true;

			this.TextBoxFunctionName = new TextBox();
			this.TextBoxFunctionName.Parent = this.forme;
			this.TextBoxFunctionName.Top = 1;
			this.TextBoxFunctionName.Left = this.LabelFuncName.Left + this.LabelFuncName.Width + 5;
			this.TextBoxFunctionName.Width = 200;
			this.TextBoxFunctionName.Font = new Font("ms sans serif", 12f);
			this.TextBoxFunctionName.Text = this.TheFonction.Name;
			this.TextBoxFunctionName.TextChanged += new EventHandler(this.TextBoxFunctionName_TextChanged);
			this.TextBoxFunctionName.KeyDown += new KeyEventHandler(this.TextBoxFunctionName_KeyDown);
			this.TextBoxFunctionName.BorderStyle = BorderStyle.FixedSingle;

			//parametre
			this.LabelFuncParams = new Label();
			this.LabelFuncParams.Parent = this.forme;
			this.LabelFuncParams.Font = new Font("ms sans serif", 9f);
			this.LabelFuncParams.Text = "Paramètres de la fonction : (Utilisez une virgule pour séparer les paramètres)";
			this.LabelFuncParams.Left = 1;
			this.LabelFuncParams.Top = this.TextBoxFunctionName.Top + this.TextBoxFunctionName.Height;
			this.LabelFuncParams.AutoSize = true;

			this.TextBoxParam = new TextBox();
			this.TextBoxParam.Parent = this.forme;
			this.TextBoxParam.Font = new Font("ms sans serif", 12f);
			this.TextBoxParam.Left = 1;
			this.TextBoxParam.Top = this.LabelFuncParams.Top + this.LabelFuncParams.Height + 2;
			this.TextBoxParam.Width = 400;
			this.SetParamTextboxFromFonction();
			this.TextBoxParam.KeyDown += new KeyEventHandler(this.TextBoxParam_KeyDown);
			this.TextBoxParam.KeyPress += new KeyPressEventHandler(this.TextBoxParam_KeyPress);
			this.TextBoxParam.TextChanged += new EventHandler(this.TextBoxParam_TextChanged);
			this.TextBoxParam.BorderStyle = BorderStyle.FixedSingle;


			this.TheEditer = new oEquationEditer(this.TheFonction.TheEqu);
			this.TheEditer.Parent = this.forme;
			this.TheEditer.Top = 75;
			this.TheEditer.Left = 1;
			this.TheEditer.Width = this.forme.Width - 18;
			this.TheEditer.Height = this.forme.Height - 40 - this.TheEditer.Top;
			this.TheEditer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;



		}
		
		private void forme_FormClosed(object sender, FormClosedEventArgs e)
		{
			//fait désélectionner tout les élément
			this.TheEditer.UnselectAllObject();

			//raise l'event qui indique que la fonction a changé
			this.TheCalculContext.Raise_FonctionRuleChanged(this.TheFonction);



		}
		private void TagButton_MouseClick(object sender, MouseEventArgs e)
		{
			this.TheFonction.Raise_TagEditionWanted();
		}
		private void TagButton_KeyDown(object sender, KeyEventArgs e)
		{
			this.TheEditer.Exec_KeyDown(e.KeyCode);
		}
		private void TextBoxFunctionName_TextChanged(object sender, EventArgs e)
		{
			string newfname = this.TextBoxFunctionName.Text;
			bool IsValidName = cWriteViewAssets.IsAVariableName(newfname);
			if (IsValidName)
			{
				this.TextBoxFunctionName.BackColor = Color.White;
				this.TheFonction.Name = newfname;
			}
			else
			{
				this.TextBoxFunctionName.BackColor = Color.LightCoral;
			}
			this.RefreshTextSize();
		}
		private void TextBoxFunctionName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.TagButton.Focus();
			}
		}
		private void TextBoxParam_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.TagButton.Focus();
				e.Handled = false;
			}
		}
		private void TextBoxParam_KeyPress(object sender, KeyPressEventArgs e)
		{
			string stringkeychar = e.KeyChar.ToString();
			string BannedCaracter = "«»<>°`~ ^çà;è.é¨ÇÀ:ÈÉ\"'=+-_)(*&?%$#@!/\\|";
			int index = 0;
			while (index <= BannedCaracter.Length - 1)
			{
				string cs = BannedCaracter.Substring(index, 1);
				if (cs == stringkeychar)
				{
					e.Handled = true;
					break;
				}
				index++;
			}
		}
		private void TextBoxParam_TextChanged(object sender, EventArgs e)
		{
			string tbText = this.TextBoxParam.Text;

			//si le textbox est vide, il fait juste supprimmer tout les parametre
			if (tbText.Length <= 0)
			{
				while (this.TheFonction.listParam.Count > 0)
				{
					this.TheFonction.listParam.RemoveAt(0);
				}
			}
			else
			{
				List<string> newparams = new List<string>();

				////lit tout les paramètre entré dans le textbox
				string actualparam = "";

				foreach (char c in tbText)
				{
					string cs = c.ToString();
					if (cs == ",")
					{
						//ajoute le parametre et reset actualparam
						newparams.Add(actualparam);
						actualparam = "";
					}
					else //ajoute le caractere à actualparam
					{
						actualparam += cs;
					}
				}
				if (actualparam.Length > 0) { newparams.Add(actualparam); }


				//vide les parametre de TheFonction et ajoute les nouveau
				while (this.TheFonction.listParam.Count > 0) { this.TheFonction.listParam.RemoveAt(0); }
				this.TheFonction.listParam.AddRange(newparams);



			}




		}



		private void SetParamTextboxFromFonction()
		{
			string str = "";
			if (this.TheFonction.listParam.Count > 0)
			{
				str = this.TheFonction.listParam[0];

				int index = 1;
				while (index <= this.TheFonction.listParam.Count - 1)
				{
					str += ",";
					str += this.TheFonction.listParam[index];
					//next iteration
					index++;
				}
				
			}

			this.TextBoxParam.Text = str;
		}
		private void RefreshTextSize()
		{
			this.forme.Text = this.TheFonction.Name + " - Éditeur de fonction";


		}




	}
}
