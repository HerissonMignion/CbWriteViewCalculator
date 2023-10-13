using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CbWriteViewCalculator.GraphingCalculator
{
	public partial class FormGraphingCalculator : Form
	{
		private oEquationCalculContext ECC;
		private VirtualContext VC;
		private uiVirtualContextViewer BigView;

		private oCalculContextEditer CalculContextEditer;




		public FormGraphingCalculator()
		{
			InitializeComponent();
			this.KeyDown += new KeyEventHandler(this.FormGraphingCalculator_KeyDown);
			this.KeyPreview = true;


			//on crée les "context" que "tout le monde" aura de besoin puisque la calculatrice graphique se base sur ces features
			this.ECC = new oEquationCalculContext();
			this.ECC.ActualAngleType = oEquationCalculContext.AngleType.Radian;
			this.ECC.AddVariableByParam("pi", Math.PI);
			this.ECC.AddVariableByParam("e", Math.E);

			this.VC = new VirtualContext(this.ECC);
			this.VC.RemovingDrawFunction += new EventHandler<DrawFunctionEventArgs>(this.VC_RemovingDrawFunction);
			this.VC.DrawFunctionRemoved += new EventHandler<DrawFunctionEventArgs>(this.VC_DrawFunctionRemoved);


			this.CalculContextEditer = new oCalculContextEditer(this.ECC);
			this.CalculContextEditer.Parent = this;
			this.CalculContextEditer.AllowUserTo_EditAngleUnit = false;


			this.BigView = new uiVirtualContextViewer(this.VC);
			this.BigView.Parent = this;


		}
		private void FormGraphingCalculator_Load(object sender, EventArgs e)
		{
			this.RefreshSize();


		}
		private void FormGraphingCalculator_KeyDown(object sender, KeyEventArgs e)
		{
			this.BigView.KeyDown(e.KeyCode);
		}
		private void btnGotoCalculator_Click(object sender, EventArgs e)
		{
			Program.NextForm = Program.NextFormToShow.Calculator;
			this.Close();
		}


		private void VC_RemovingDrawFunction(object sender, DrawFunctionEventArgs e)
		{
			//à l'intérieur de la liste des uiDrawFunction que nous possédons, on cherche celui qui a la drawfunction qui est en train d'être retiré, et on fait disposer son uiDrawFunction
			//puisque la liste sera modifié, il faut éviter d'utiliser un foreach
			int index = 0;
			while (index < this.listUiDrawFunction.Count)
			{
				//on récupère l'object
				uiDrawFunction uidf = this.listUiDrawFunction[index];

				//on check si c'est ce uiDrawFunction et est associé à la draw function en train d'être retiré
				if (uidf.DF == e.DF)
				{
					//si on n'a trouvé celui qui est associé, on le supprimme
					uidf.Dispose();
					
					//on le retire de la liste qu'on possède
					this.listUiDrawFunction.Remove(uidf);

					//on quitte la boucle parce qu'on l'as trouvé
					break;
				}

				//next iteration
				index++;
			}


		}
		private void VC_DrawFunctionRemoved(object sender, DrawFunctionEventArgs e)
		{
			//on fait disposer la draw function
			e.DF.Dispose();
		}




		#region gestion des équation à dessiner
		//la liste de tout les objet graphique qui permettent à l'user d'intéragir avec les fonction à dessiner
		private List<uiDrawFunction> listUiDrawFunction = new List<uiDrawFunction>();


		int NewDrawFunctionCount = 0;
		private void btnNewFonction_MouseClick(object sender, MouseEventArgs e)
		{
			//on crée la nouvelle DrawFunction. les DrawFunction s'ajoutent eux mêmes à leur virtual context.
			DrawFunction newdf = new DrawFunction(this.VC);
			this.NewDrawFunctionCount++;
			if (this.NewDrawFunctionCount > 1)
			{
				newdf.Name = this.NewDrawFunctionCount.ToString() + " new draw function";
			}

			
			//maintenant on crée l'objet graphique qui permet à l'user d'intéragir avec le drawfunction pour modifier l'équation notament
			uiDrawFunction newuidf = new uiDrawFunction(newdf);
			this.listUiDrawFunction.Add(newuidf);
			newuidf.Parent = this.flpEquations;




		}


		private void btnNewInequation_MouseClick(object sender, MouseEventArgs e)
		{
			Program.wdebug("new inequation");


		}


		#endregion







		private void RefreshSize()
		{
			this.CalculContextEditer.Top = this.gbEquation.Top + this.gbEquation.Height + 3;
			this.CalculContextEditer.Left = 1;
			this.CalculContextEditer.Width = 215; // 200
			this.CalculContextEditer.Height = this.Height - 40 - this.CalculContextEditer.Top;
			this.CalculContextEditer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom; // | AnchorStyles.Right;


			this.BigView.Location = new Point(this.CalculContextEditer.Left + this.CalculContextEditer.Width + 3, this.gbEquation.Top + this.gbEquation.Height + 3);
			this.BigView.Width = this.Width - 17 - this.BigView.Left;
			this.BigView.Height = this.Height - 40 - this.BigView.Top;
			this.BigView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

		}

	}
}
