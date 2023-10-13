using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{

	//fonction qui a été créer et défini par l'utilisateur.
	public class EQoUserFunction : oEquationObject
	{
		private int zzzParentheseWidth = 10;
		private int zzzSpaceUpDown = 2; //nombre de pixel qui dépasse en haut et en bas
		private int zzzParamSpace = 6; //espace en pixel entre les paramètre



		private string zzzName = "f";
		public string Name
		{
			get { return this.zzzName; }
			set
			{
				if (cWriteViewAssets.IsAVariableName(value))
				{
					this.zzzName = value;
				}
				else
				{
					this.zzzName = "f";
				}
			}
		}
		public Font NameFont = new Font("consolas", 14f); // new Font("ms sans serif", 10f);


		public int ParamCount
		{
			get
			{
				return this.ListEquation.Count;
			}
		}
		public void SetParamCount(int TargetParamCount)
		{
			//ajoute des équation tant qu'il en manque
			while (this.ParamCount < TargetParamCount)
			{
				oEquation neweq = new oEquation();
				this.ListEquation.Add(neweq);
			}

			//retire des équation tant qu'il y en a trop
			if (TargetParamCount >= 0)
			{
				while (this.ParamCount > TargetParamCount)
				{
					//retire une équation à la fin
					this.ListEquation.RemoveAt(this.ListEquation.Count - 1);
				}
			}
		}



		public override int GetWidth()
		{
			int TotalWidth = 0;

			Size NameSize = cWriteViewAssets.GetTextSize(this.Name, this.NameFont);
			TotalWidth += NameSize.Width;
			TotalWidth += this.zzzParentheseWidth;

			if (this.ListEquation.Count > 0)
			{
				bool IsFirst = true;
				foreach (oEquation eq in this.ListEquation)
				{
					//ajoute la virgule
					if (!IsFirst) { TotalWidth += this.zzzParamSpace; }
					//ajoute la largeur de l'équation
					TotalWidth += eq.GetWidth();

					//next iteration
					IsFirst = false;
				}
			}

			TotalWidth += this.zzzParentheseWidth;

			return TotalWidth;
		}
		public override int GetHeight()
		{
			int MaxHeight = 20; //implique que c'est la hauteur minimale d'une UserFunction, sans les bordure up et down

			//parcourt toute les equation 
			foreach (oEquation eq in this.ListEquation)
			{
				int ActualHeight = eq.GetHeight();
				if (ActualHeight > MaxHeight)
				{
					MaxHeight = ActualHeight;
				}
			}


			return this.zzzSpaceUpDown + MaxHeight + this.zzzSpaceUpDown;
		}



		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			int ActualLeft = 0;
			Size NameSize = cWriteViewAssets.GetTextSize(this.Name, this.NameFont);
			g.DrawString(this.Name, this.NameFont, Brushes.Black, 1f, (float)(imgHeight / 2) - (float)(NameSize.Height / 2));
			ActualLeft += NameSize.Width;

			//dessine premiere parenthese
			Bitmap p1 = cWriteViewAssets.GetImgParentheseLeft(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(p1, ActualLeft, 0);
			p1.Dispose();
			ActualLeft += this.zzzParentheseWidth;


			int demiHeight = imgHeight / 2;

			////dessine les équation
			if (this.ListEquation.Count > 0)
			{
				bool IsFirst = true;
				foreach (oEquation eq in this.ListEquation)
				{
					//virgule
					if (!IsFirst)
					{
						g.DrawLine(Pens.Black, ActualLeft + 2, demiHeight, ActualLeft + 2, demiHeight + 2);
						g.DrawLine(Pens.Black, ActualLeft + 3, demiHeight, ActualLeft + 3, demiHeight + 4);
						g.DrawLine(Pens.Black, ActualLeft + 2, demiHeight + 4, ActualLeft + 2, demiHeight + 5);

						ActualLeft += this.zzzParamSpace;
					}

					//equation
					Bitmap eqimg = eq.GetImage(TheDrawContext);
					int eqTop = demiHeight - (eqimg.Height / 2);
					g.DrawImage(eqimg, ActualLeft, eqTop);
					eq.uiLastLeft = ActualLeft;
					eq.uiLastTop = eqTop;
					eq.uiLastWidth = eqimg.Width;
					eq.uiLastHeight = eqimg.Height;

					ActualLeft += eqimg.Width;

					//next iteration
					IsFirst = false;
				}
			}

			//dessine parenthese de droite
			Bitmap p2 = cWriteViewAssets.GetImgParentheseRight(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(p2, imgWidth - this.zzzParentheseWidth, 0);
			p2.Dispose();
			

			g.Dispose();
			return img;
		}





		//void new()
		public EQoUserFunction(string StartName = "f")
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fUserFonction;
			this.Name = StartName;

			this.SetParamCount(1);
		}
		public EQoUserFunction(string StartName, List<oEquation> AllEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fUserFonction;
			this.Name = StartName;

			this.ListEquation.AddRange(AllEq);
		}


		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			oFonction thef = TheCalculContext.GetFonctionByName(this.Name);
			if (thef == null)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.UndefinedFunction;
				return rep;
			}
			if (thef.ParamCount != this.ParamCount)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.InvalidFunctionParamCount;
				return rep;
			}

			//calcul les paramètre de la fonction
			List<double> listAllParamValue = new List<double>();
			foreach (oEquation eq in this.ListEquation)
			{
				sEquationResult subrep = eq.GetResult(TheCalculContext);
				if (subrep.AnErrorOccurred) { return subrep; }
				listAllParamValue.Add(subrep.TheResult);
			}

			//calcul la valeur de la fonction
			rep = thef.GetResult(listAllParamValue.ToArray(), TheCalculContext);
			return rep;
		}




		public override string GetReadableName()
		{
			string rep = this.Name + "(";
			if (this.ListEquation.Count > 0)
			{
				for (int i = 1; i <= this.ListEquation.Count; i++)
				{
					if (i > 1) { rep += ","; }
					rep += i.ToString();
				}
			}
			rep += ")";
			return rep;
		}

		public override oEquationObject GetCopy()
		{
			//copy les équation
			List<oEquation> allequcopy = new List<oEquation>();
			foreach (oEquation eq in this.ListEquation)
			{
				allequcopy.Add(eq.GetCopy());
			}
			//fait une copy de this
			EQoUserFunction copy = new EQoUserFunction(this.Name, allequcopy);
			return copy;
		}
		public override string GetSaveString()
		{
			string equstr = "";
			foreach (oEquation eq in this.ListEquation)
			{
				equstr += eq.GetSaveString(true);
			}
			return "\\8userfunc" + this.Name + ";" + this.ParamCount.ToString() + ";" + equstr;
		}





		public override bool IsComplique(oEquationCalculContext ecc)
		{
			//on check si at least un des paramètre est "compliqué"
			if (base.IsComplique(ecc)) { return true; }

			//maintenant on vérifie l'équation la fonction correspondante à this et on check si elle est "compliqué"
			oFonction thef = ecc.GetFonctionByName(this.Name);
			if (thef != null)
			{
				if (thef.TheEqu.IsComplique(ecc)) { return true; }
			}
			
			return false;
		}


		public override void CompileToMSIL(CompileContext cc)
		{
			//on obtien la fonction
			oFonction thef = cc.CalculContext.GetFonctionByName(this.Name);
			if (thef != null)
			{
				////on commence par créer la layer des variable donné en paramètre
				CompileContext.VariableLayer vl = new CompileContext.VariableLayer();
				int index = 0;
				foreach (string paramname in thef.listParam)
				{
					//ajoute le code qui fait calculer le paramètre actuel
					this.ListEquation[index].CompileToMSIL(cc);

					//déclare la variable qui va recevoir cette valeur
					LocalBuilder lbparam = cc.il.DeclareLocal(typeof(double));
					vl.dictPrivateVar.Add(paramname, lbparam); //on ajoute la variable à l'étage du calcul de la fonction

					//on envoie la réponse de l'équation dans cette variable
					cc.il.Emit(OpCodes.Stloc, lbparam.LocalIndex);

					//next iteration
					index++;
				}

				//maintenant qu'on a finit de calculer tout les paramètre de la fonction, on ajoute l'étage de variable
				cc.listLayer.Add(vl);

				//maintenant on ajoute le code qui fait calculer la fonction
				thef.TheEqu.CompileToMSIL(cc);

				//fin
				cc.PopLastLayer();
			}
			else //si la fonction n'a pas été trouvé, on met qqc pour ne pas déstabiliser le stack et on signale l'erreur
			{
				cc.il.Emit(OpCodes.Ldc_R8, 0d);

				cc.il.Emit(OpCodes.Ldstr, "La fonction \"" + this.Name + "\" n'existe pas.");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

			}

		}


	}
}
