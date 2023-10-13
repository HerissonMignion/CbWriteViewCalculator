using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CbWriteViewCalculator.GraphingCalculator
{

	//contient toute les fonction à afficher et le calcul context
	public class VirtualContext
	{
		//le calcul context. il contient les variable et les fonction globale.
		//il est bien important de se rappeler la différence entre les fonction globales et les fonction à dessiner.
		public oEquationCalculContext ECC;



		#region draw functions
		
		//la liste des fonction à dessiner
		public List<DrawFunction> listFunctions = new List<DrawFunction>();
		public void AddDrawFunction(DrawFunction newDF)
		{
			//on l'ajoute à la liste
			this.listFunctions.Add(newDF);

			//on raise l'event associé
			this.Raise_NewDrawFunction(newDF);

		}
		public void RemoveDrawFunction(DrawFunction DF)
		{
			//on raise l'event pour indiquer qu'on s'apprête à retirer une drawfunction
			this.Raise_RemovingDrawFunction(DF);

			//on l'enlève de la liste
			this.listFunctions.Remove(DF);

			//on raise l'event qui indique qu'on l'a retiré
			this.Raise_DrawFunctionRemoved(DF);

		}



		//indique à toute les draw function qu'elles doivent se recompiler
		public void AllDF_RecompileNeeded()
		{
			foreach (DrawFunction df in this.listFunctions)
			{
				df.RecompileNeeded();
				
			}
		}







		//indique qu'il y a une nouvelle draw function
		public event EventHandler<DrawFunctionEventArgs> NewDrawFunction;
		private void Raise_NewDrawFunction(DrawFunction DF)
		{
			if (this.NewDrawFunction != null)
			{
				this.NewDrawFunction(this, new DrawFunctionEventArgs(DF));
			}
		}

		public event EventHandler<DrawFunctionEventArgs> RemovingDrawFunction; //raisé avant de retirer une draw function
		public event EventHandler<DrawFunctionEventArgs> DrawFunctionRemoved; //raisé après avoir retiré la draw function
		private void Raise_RemovingDrawFunction(DrawFunction DF)
		{
			if (this.RemovingDrawFunction != null)
			{
				this.RemovingDrawFunction(this, new DrawFunctionEventArgs(DF));
			}
		}
		private void Raise_DrawFunctionRemoved(DrawFunction DF)
		{
			if (this.DrawFunctionRemoved != null)
			{
				this.DrawFunctionRemoved(this, new DrawFunctionEventArgs(DF));
			}
		}


		//indique que qqc a changé a l'intérieur d'une draw function
		public event EventHandler<DrawFunctionEventArgs> DrawFunctionSomethingChanged;
		private void Raise_DrawFunctionSomethingChanged(DrawFunction DF)
		{
			if (this.DrawFunctionSomethingChanged != null)
			{
				this.DrawFunctionSomethingChanged(this, new DrawFunctionEventArgs(DF));
			}
		}
		//sert à informer this qu'une draw function a qqc de changé
		public void Inform_DrawFunctionSomethingChanged(DrawFunction DF)
		{
			this.Raise_DrawFunctionSomethingChanged(DF);
		}



		#endregion






		public VirtualContext(oEquationCalculContext sECC)
		{
			this.ECC = sECC;

			this.ECC.FonctionRuleChanged += new EventHandler<FonctionEventArgs>(this.ECC_FonctionRuleChanged);


		}


		//si une nouvelle variable est ajouté, il faut recompiler les draw function. par example, si l'user défini une draw function en utilisant une variable global qui n'existe pas encore, et qu'après il crée la variable globale, il faut recompiler les draw function
		private void ECC_VariableAdded(object sender, VariableEventArgs e)
		{
			this.AllDF_RecompileNeeded();


		}

		//lorsque cet event est raisé, il faut refaire compiler toutes les draw function qui utilise la fonction globale qui a été modifié
		private void ECC_FonctionRuleChanged(object sender, FonctionEventArgs e)
		{
			this.AllDF_RecompileNeeded();


			//sCompiledEquationResult rep = this.listFunctions[0].ComputeAtX(2d);
			//Program.wdebug(rep.TheResult);

		}




	}
}
