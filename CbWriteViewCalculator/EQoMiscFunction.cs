using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoMiscFunction : oEquationObject
	{
		private int zzzParentheseWidth = 7;
		private int zzzSpaceUpDown = 2; //espacement en haut et en bas
		private int zzzParamSpace = 6; //espace en pixel entre les paramètre


		public Font NameFont = new Font("consolas", 15f);
		private string zzzActualNameStr = "";
		public string NameStr
		{
			get { return this.zzzActualNameStr; }
		}
		private Size ActualNameSize = new Size(0, 0);

		public void SetActualNameStr(string NewFunc, bool DefineParamCount = true)
		{
			this.zzzActualNameStr = NewFunc;
			this.ActualNameSize = cWriteViewAssets.GetTextSize(NewFunc, this.NameFont);

			//défini le nombre de paramètre que la fonction possède
			if (DefineParamCount)
			{
				if (NewFunc == "ln") { this.SetParamCount(1); }
				if (NewFunc == "exp") { this.SetParamCount(1); }
				if (NewFunc == "AGM") { this.SetParamCount(2); }
			}
		}

		
		public int ParamCount
		{
			get
			{
				return this.ListEquation.Count;
			}
		}
		private void SetParamCount(int TargetParamCount)
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

			Size NameSize = ActualNameSize;
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
			Size NameSize = this.ActualNameSize;
			g.DrawString(this.NameStr, this.NameFont, Brushes.Black, 1f, (float)(imgHeight / 2) - (float)(NameSize.Height / 2));
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

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rp1;
			sEquationResult rp2;
			sEquationResult rp3;
			double p1 = 0d;
			double p2 = 0d;
			double p3 = 0d;
			//lit les paramètre
			if (this.ParamCount >= 1) { rp1 = this.ListEquation[0].GetResult(TheCalculContext); if (rp1.AnErrorOccurred) { return rp1; } else { p1 = rp1.TheResult; } }
			if (this.ParamCount >= 2) { rp2 = this.ListEquation[1].GetResult(TheCalculContext); if (rp2.AnErrorOccurred) { return rp2; } else { p2 = rp2.TheResult; } }
			if (this.ParamCount >= 3) { rp3 = this.ListEquation[2].GetResult(TheCalculContext); if (rp3.AnErrorOccurred) { return rp3; } else { p3 = rp3.TheResult; } }
			
			if (this.NameStr == "ln")
			{
				if (p1 <= 0d)
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
					return rep;
				}
				rep.TheResult = Math.Log(p1);
				return rep;
			}
			else if (this.NameStr == "exp")
			{
				rep.TheResult = Math.Exp(p1);
				return rep;
			}
			else if (this.NameStr == "AGM")
			{
				rep.TheResult = cWriteViewAssets.AGM(p1, p2);
				return rep;
			}

			return rep;
		}


		//void new
		public EQoMiscFunction(string sFuncName = "ln")
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fMiscFunction;
			
			this.SetActualNameStr(sFuncName);
		}
		public EQoMiscFunction(string sFuncName, List<oEquation> AllEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fMiscFunction;

			this.SetActualNameStr(sFuncName, false);
			this.ListEquation.AddRange(AllEq);
		}


		public override string GetReadableName()
		{
			if (this.NameStr == "ln") { return "ln(x)"; }
			if (this.NameStr == "exp") { return "exp(x)"; }
			if (this.NameStr == "AGM") { return "AGM(a,b)"; }

			return "EQoMiscFunction";
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
			EQoMiscFunction copy = new EQoMiscFunction(this.zzzActualNameStr, allequcopy);
			return copy;
		}

		public override string GetSaveString()
		{
			string rep = "\\4misc" + this.NameStr + ";" + this.ParamCount + ";";
			foreach (oEquation eq in this.ListEquation)
			{
				rep += eq.GetSaveString(true);
			}
			return rep;
		}




		public override void CompileToMSIL(CompileContext cc)
		{
			if (this.NameStr == "ln")
			{
				//on met le code qui fait calculer l'équation à l'intérieur de this
				this.ListEquation[0].CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel(); //à faire si le paramètre est inférieur ou égale à 0
				Label lblEnd = cc.il.DefineLabel(); //la fin du calcul de la fonction logarithme

				//on doit vérifier si le paramètre est dans les bound.
				//si le paramètre est inférieur ou égale à 0, on goto lblError.
				cc.il.Emit(OpCodes.Dup); //il faut dupliquer le paramètre parce qu'il sera consommé
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack
				cc.il.Emit(OpCodes.Ble, lblError); //si le paramètre est <= à 0, on goto à lblError

				////calcul du logarithme naturel
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Log", new Type[] { typeof(double) }));
				cc.il.Emit(OpCodes.Br, lblEnd); //on va à la fin

				////si le paramètre n'est pas dans le domaine de calcul
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //il faut retirer du stack le paramètre qui y est actuellement
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack pour ne pas le déstabiliser

				//on met le message d'erreur sur le stack pour l'envoyer en paramètre
				cc.il.Emit(OpCodes.Ldstr, "logarithm of a number lower or equal to 0");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

				cc.il.MarkLabel(lblEnd);
			}
			else if (this.NameStr == "exp")
			{
				//on met le code qui fait calculer l'équation qu'il y a à l'intérieur
				this.ListEquation[0].CompileToMSIL(cc);

				//on lui fait appeller l'exponentiel sur la réponse de l'équation
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Exp"));
				

			}
			else if (this.NameStr == "AGM")
			{
				//on met le code qui fait calculer les deux équation qu'il y a à l'intérieur
				this.ListEquation[0].CompileToMSIL(cc);
				this.ListEquation[1].CompileToMSIL(cc);

				//on appelle l'AGM
				cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("AGM"));


			}
			else
			{
				base.CompileToMSIL(cc);
			}
		}

	}
}
