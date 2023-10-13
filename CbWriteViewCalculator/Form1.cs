using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
    public partial class Form1 : Form
    {

        oEquation ActualEquation;
        oEquationCalculContext ActualEquationCalculContext;
        oEquationDrawContext ActualEquationDrawContext;


        oEquationEditer TheEditer;
		oCalculContextEditer TheCalculContextEditer;





        //---------------------[ sub new() & private/public void ]------------------
        public Form1()
        {
            InitializeComponent();

            this.ActualEquation = new oEquation();


            this.ActualEquationDrawContext = new oEquationDrawContext(); //doit etre crée avant qu'il soit donné à this.TheEditer

            this.ActualEquationCalculContext = new oEquationCalculContext(); //doit etre crée avant qu'il soit donné à this.TheCalculContextEditer
			//this.ActualEquationCalculContext.AddVariableByParam("var1", 3.14d);
			//this.ActualEquationCalculContext.AddVariableByParam("var2", 2.71828d);
			//this.ActualEquationCalculContext.AddVariableByParam("var3", 3.14d);
			//this.ActualEquationCalculContext.AddVariableByParam("var4", 2.71828d);
			//this.ActualEquationCalculContext.AddVariableByParam("var5", 3.14d);
			//this.ActualEquationCalculContext.AddVariableByParam("var6", 2.71828d);
			//this.ActualEquationCalculContext.AddVariableByParam("var7", 3.14d);
			//this.ActualEquationCalculContext.AddVariableByParam("var8", 2.71828d);
			//this.ActualEquationCalculContext.AddVariableByParam("var9", 3.14d);
			this.ActualEquationCalculContext.AddVariableByParam("pi", Math.PI);
			this.ActualEquationCalculContext.AddVariableByParam("e", Math.E);


			this.TheCalculContextEditer = new oCalculContextEditer(this.ActualEquationCalculContext);
			this.TheCalculContextEditer.Parent = this;
			this.TheCalculContextEditer.Top = this.ComputeButton.Top + this.ComputeButton.Height + 3; //this.TestButton.Top + this.TestButton.Height + 10;
			this.TheCalculContextEditer.Left = 2;
			this.TheCalculContextEditer.Width = 300;
			this.TheCalculContextEditer.Height = this.Height - 41 - this.TheCalculContextEditer.Top;
			this.TheCalculContextEditer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;


            this.TheEditer = new oEquationEditer(this.ActualEquation);
            this.TheEditer.Parent = this;
            this.TheEditer.ActualEquationDrawContext = this.ActualEquationDrawContext;
			this.TheEditer.Top = this.TheCalculContextEditer.Top; //this.TestButton.Top + this.TestButton.Height + 10;
			this.TheEditer.Left = this.TheCalculContextEditer.Left + this.TheCalculContextEditer.Width + 2; //300; // 2
			this.TheEditer.Width = this.Width - this.TheEditer.Left - 18;
			this.TheEditer.Height = this.Height - 41 - this.TheEditer.Top;
			this.TheEditer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;

			

			this.ActualEquationDrawContext.DrawBorderOfAllObject = false;
			//this.ActualEquationDrawContext.CenterObjectsAtVertical = true; // true
			this.ActualEquationDrawContext.UiMultiplicationSymboleToUse = oEquationDrawContext.UiMultiplicationSymbole.symboleDot;

			


		}


        public void RefreshTextSize()
        {


            this.TheEditer.Refresh();
        }





        //--------------------[ private event ]------------------
        private void Form1_Load(object sender, EventArgs e)
        {
			this.KeyPreview = true;

			this.TestButton.Visible = false;
			this.TestButton2.Visible = false;


			this.tbAnswer.BackColor = this.BackColor;
			this.tbAnswer.ForeColor = Color.Black;
			this.tbAnswer.BorderStyle = BorderStyle.None;
			this.tbAnswer.ReadOnly = true;



            //events
            this.SizeChanged += new EventHandler(this.Form1_SizeChanged);
			this.KeyDown += new KeyEventHandler(this.TheEditer.SomeOne_KeyDown);

			this.tbAnswer.KeyDown += new KeyEventHandler(this.tbAnswer_KeyDown);


            //end
            this.RefreshTextSize();

        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.RefreshTextSize();
        }
		private void cbUseAnsVariable_CheckedChanged(object sender, EventArgs e)
		{
			this.ComputeButton.Focus();
		}
		private void cbCompileComplique_CheckedChanged(object sender, EventArgs e)
		{
			this.ComputeButton.Focus();
		}


        private void TestButton_Click(object sender, EventArgs e)
        {
			//string savestring = this.ActualEquation.GetSaveString(false);
			//////MessageBox.Show(savestring);
			////this.ShowAnswer(savestring);
			//FormShowAskText fsa = new FormShowAskText();
			//fsa.SetMode(FormShowAskText.saMode.Show);
			//fsa.Text = savestring;
			//fsa.ShowDialog();


			//crée la liste des variable externe, ainsi que celle qui sera donné pour faire calculer l'équation
			List<string> listEV = new List<string>();
			List<KeyValuePair<string, double>> listVarNameValue = new List<KeyValuePair<string, double>>();
			foreach (Variable var in this.ActualEquationCalculContext.AllVariable)
			{
				listEV.Add(var.Name);
				listVarNameValue.Add(new KeyValuePair<string, double>(var.Name, var.Value));
			}

			//crée le context
			CompileContext cc = new CompileContext(listEV, this.ActualEquationCalculContext);

			//lui demande de compiler l'équation
			cc.CompileEquation(this.ActualEquation);

			//teste l'équation
			sCompiledEquationResult rep = cc.Compute(listVarNameValue);
			if (!rep.AnErrorOccurred)
			{
				Program.wdebug(rep.TheResult);
			}
			else
			{
				MessageBox.Show("Une erreur s'est produite : " + rep.ErrorMessage);
			}


			


        }
        private void TestButton2_Click(object sender, EventArgs e)
        {
			oEquationObject newobj = new EQoBinominalCoef();
			this.TheEditer.SetToAddMode(newobj);
			this.RefreshTextSize();


			//FormShowAskText fsa = new FormShowAskText();
			//fsa.SetMode(FormShowAskText.saMode.Ask);
			//fsa.Message = "Entrez l'équation :";
			//fsa.ShowDialog();
			//if (fsa.TheExitMethod == FormShowAskText.saExitMethod.Ok)
			//{
			//	string save = fsa.Text;
			//	oEquation neweq = oEquation.LoadFromSaveString(save);
			//	this.ActualEquation = neweq;
			//	this.TheEditer.SetTheEquationToEdit(neweq);
			//}

		}

		private void tbAnswer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.ComputeButton.Focus();
			}
		}
		private void ComputeButton_Click(object sender, EventArgs e)
		{
			if (this.TheEditer.TheEquation.IsOk(true))
			{
				bool CompileIt = false; //devient true s'il faut plutôt compiler l'équation
				if (this.cbCompileComplique.Checked)
				{
					CompileIt = this.TheEditer.TheEquation.IsComplique(this.ActualEquationCalculContext);
				}

				if (!CompileIt)
				{

					this.ActualEquationCalculContext.RemoveAllLayer();
					sEquationResult rep = this.TheEditer.TheEquation.GetResult(this.ActualEquationCalculContext);
					if (rep.AnErrorOccurred == false)
					{
						double dblrep = rep.TheResult;

						this.ShowAnswer(dblrep.ToString());

						if (this.cbUseAnsVariable.Checked)
						{
							bool isexist = this.ActualEquationCalculContext.IsVariableExistByName("ans");
							if (isexist)
							{
								this.ActualEquationCalculContext.GetVariableByName("ans").Value = dblrep;
								this.ActualEquationCalculContext.Raise_SomethingChanged(this);
							}
							else
							{
								Variable newvar = new Variable("ans", dblrep);
								this.ActualEquationCalculContext.AllVariable.Insert(0, newvar);
								//this.ActualEquationCalculContext.Raise_VariableAdded(newvar);
								this.ActualEquationCalculContext.Raise_SomethingChanged(this);
							}
						}

					}
					else
					{
						MessageBox.Show("Une erreur s'est produite : " + rep.ActualError.ToString());
					}
				}
				else
				{


					//crée la liste des variable externe, ainsi que celle qui sera donné pour faire calculer l'équation
					List<string> listEV = new List<string>();
					List<KeyValuePair<string, double>> listVarNameValue = new List<KeyValuePair<string, double>>();
					foreach (Variable var in this.ActualEquationCalculContext.AllVariable)
					{
						listEV.Add(var.Name);
						listVarNameValue.Add(new KeyValuePair<string, double>(var.Name, var.Value));
					}

					//crée le context
					CompileContext cc = new CompileContext(listEV, this.ActualEquationCalculContext);

					//lui demande de compiler l'équation
					cc.CompileEquation(this.ActualEquation);

					//teste l'équation
					sCompiledEquationResult rep = cc.Compute(listVarNameValue);
					if (!rep.AnErrorOccurred)
					{
						
						this.ShowAnswer(rep.TheResult.ToString() + "   (compilé)");
						
						if (this.cbUseAnsVariable.Checked)
						{
							bool isexist = this.ActualEquationCalculContext.IsVariableExistByName("ans");
							if (isexist)
							{
								this.ActualEquationCalculContext.GetVariableByName("ans").Value = rep.TheResult;
								this.ActualEquationCalculContext.Raise_SomethingChanged(this);
							}
							else
							{
								Variable newvar = new Variable("ans", rep.TheResult);
								this.ActualEquationCalculContext.AllVariable.Insert(0, newvar);
								//this.ActualEquationCalculContext.Raise_VariableAdded(newvar);
								this.ActualEquationCalculContext.Raise_SomethingChanged(this);
							}
						}

					}
					else
					{
						MessageBox.Show("Une erreur s'est produite : " + rep.ErrorMessage);
					}
					

				}

			}
			else
			{
				MessageBox.Show("Une ou des équations sont incomplètes ou incorrectes.");
			}


			this.RefreshTextSize();
		}

		private void GraphCalcButton_Click(object sender, EventArgs e)
		{
			Program.NextForm = Program.NextFormToShow.GraphingCalculator;
			this.Close();
		}


		private void ShowAnswer(string Text)
		{
			this.tbAnswer.Text = Text;
		}

	}
}
