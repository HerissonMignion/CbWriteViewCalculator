using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CbWriteViewCalculator.GraphingCalculator
{
	//une fonction qu'il faut dessiner. contient de nombreuse information et gère plusieurs chose
	public class DrawFunction
	{
		private VirtualContext zzzVC = null;
		public VirtualContext VC
		{
			get { return this.zzzVC; }
		}


		public string Name = "new draw function";


		public oEquation Equ = null; //l'équation à calculer


		public bool Visible = true; // true indique si la draw function est actuellement visible dans le plan cartésien.
		
		private Color zzzColor = Color.Lime;
		private Pen zzzPen = Pens.Lime;
		private Brush zzzBrush = Brushes.Lime;
		public Color Color
		{
			get { return this.zzzColor; }
			set
			{
				this.zzzColor = value;
				this.zzzPen = new Pen(value);
				this.zzzBrush = new SolidBrush(value);
			}
		}
		public Pen Pen
		{
			get { return this.zzzPen; }
		}
		public Brush Brush
		{
			get { return this.zzzBrush; }
		}



		#region events
		public event EventHandler SomethingChanged; //raised lorsque qqc a été modifié
		private void Raise_SomethingChanged()
		{
			if (this.SomethingChanged != null)
			{
				this.SomethingChanged(this, new EventArgs());
			}
		}
		//à caller depuis l'extérieur. sert à dire à this que qqc a changé
		public void Inform_SomethingChanged()
		{
			//on raise l'event
			this.Raise_SomethingChanged();

			//on "informe" le Virtual Context que qqc a changé
			this.VC.Inform_DrawFunctionSomethingChanged(this);

		}




		#endregion
		public DrawFunction(VirtualContext sVC)
		{
			this.zzzVC = sVC;
			//sVC.listFunctions.Add(this);

			this.Equ = new oEquation();
			this.Equ.ListEquationObject.Add(new EQoVariable("x"));



			//on s'ajoute au Virtual Context
			sVC.AddDrawFunction(this);
		}


		#region compilation
		
		private CompileContext cc = null; //le compile context. c'est lui qui gère l'ÉC et qui l'a calcule.
		public bool NeedToRecompile = true; //indique si, en l'état actuel, il faut recompiler l'équation actuel avant de pouvoir s'en servir.

		/* dit à this qu'il devra recompiler l'équation.
		 * 
		 * l'équation est à recompiler, par exemple, dans les cas suivant:
		 * - l'équation a été modifié
		 * - une fonction global a été modifié ou supprimmé
		 * - une variable global a été supprimé ou renommé
		 * 
		 */
		public void RecompileNeeded()
		{
			this.NeedToRecompile = true;
		}

		//refait la compilation
		public void Recompile()
		{
			////crée la liste des variable global
			List<string> listVarG = this.VC.ECC.AllVariable.Select(v => v.Name).ToList();
			//make sure qu'il y a la variable x, c'est la variable de la coordonné horizontal donc elle doit absolument y être.
			if (!listVarG.Contains("x")) { listVarG.Add("x"); }

			//on crée le compile context
			CompileContext ccc = new CompileContext(listVarG, this.VC.ECC);
			this.cc = ccc;

			//on lui fait compiler l'équation
			ccc.CompileEquation(this.Equ);

			//on vient de la compiler
			this.NeedToRecompile = false;
		}






		//calcul la valeur de la fonction pour la coordonné graphique x spécifié.
		public sCompiledEquationResult ComputeAtX(double x)
		{
			//check s'il faut recompiler l'équation avant de s'en servir
			if (this.NeedToRecompile) { this.Recompile(); }


			//génère la liste qui indique à l'ÉC les valeur des variable
			List<KeyValuePair<string, double>> listVarNameValue = new List<KeyValuePair<string, double>>();
			//on ajoute la variable x
			listVarNameValue.Add(new KeyValuePair<string, double>("x", x));
			//maintenant on ajoute toute les variable globale, sauf celles qui s'appelle "x" (si y'en a une, elle est juste ignoré)
			foreach (Variable v in this.VC.ECC.AllVariable)
			{
				if (v.Name != "x")
				{
					listVarNameValue.Add(new KeyValuePair<string, double>(v.Name, v.Value));
				}
			}

			//maintenant on retourne la réponse de l'équation
			return this.cc.Compute(listVarNameValue);
		}




		#endregion
		#region computed point
		/* 
		 * cette section est géré depuis l'extérieur de this, en l'occurence, un uiVirtualContextViewer.
		 * 
		 */

		//une liste de coordonnés de la fonction qui ont été calculés. il est assumé que les coordonnés sont en ordre croissant, les x les plus petit sont vers le début, à l'index 0, et les x les plus grand sont vers la fin, vers les derniers index.
		public List<PointD> listComputedPoints = new List<PointD>();

		//vide la liste des computed points
		public void ClearComputedPoints()
		{
			if (this.listComputedPoints.Count > 0)
			{
				this.listComputedPoints.RemoveRange(0, this.listComputedPoints.Count);
			}
		}

		//ajoute une nouvelle coordonné, à la fin de la liste
		public void AddComputedPoint(double vx, double vy)
		{
			this.listComputedPoints.Add(new PointD(vx, vy));
		}
		public void AddComputedPoint(PointD p)
		{
			this.listComputedPoints.Add(p);
		}



		#endregion



		public void Dispose()
		{
			this.Equ = null;
			this.zzzVC = null;

		}

	}
}
