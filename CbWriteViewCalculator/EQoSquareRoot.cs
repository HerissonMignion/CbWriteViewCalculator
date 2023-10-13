using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoSquareRoot : oEquationObject
	{

		private int zzzUpSpace = 5; //espace qui depasse au dessus
		private int zzzLeftWidth = 11; //espace à gauche de l'equation
		private int zzzRightWidth = 10; //espace à droite de l'equation



		private oEquation Equ;



		public override int GetWidth()
		{
			return this.zzzLeftWidth + this.Equ.GetWidth() + this.zzzRightWidth;
		}
		public override int GetHeight()
		{
			return this.zzzUpSpace + this.Equ.GetHeight();
		}

		public override int GetUpHeight()
		{
			return this.GetUpHeight(this.GetHeight());
		}
		public override int GetUpHeight(int MyHeight)
		{
			int uph = this.zzzUpSpace + this.Equ.GetHeightAndUpHeight().Y;
			return uph;
		}

		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			Bitmap img = new Bitmap(this.GetWidth(), this.GetHeight());
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			//dessine la racine carré
			int UpLineY = this.zzzUpSpace / 2;
			int VerBarLeft = this.zzzLeftWidth - 2; //position horizontale de la barre verticale
			g.DrawLine(Pens.Black, VerBarLeft, UpLineY, VerBarLeft, img.Height - 1);
			g.DrawLine(Pens.Black, VerBarLeft, UpLineY, img.Width - (this.zzzRightWidth / 2), UpLineY);
			int LeftBarTop = img.Height - 15; //barre en diago à gauche
			g.DrawLine(Pens.Black, 0, LeftBarTop, VerBarLeft, img.Height);




			//dessine l'equation
			Bitmap eqimg = this.Equ.GetImage(TheDrawContext);
			g.DrawImage(eqimg, this.zzzLeftWidth, this.zzzUpSpace);
			this.Equ.uiLastLeft = this.zzzLeftWidth;
			this.Equ.uiLastTop = this.zzzUpSpace;
			this.Equ.uiLastWidth = eqimg.Width;
			this.Equ.uiLastHeight = eqimg.Height;



			g.Dispose();
			return img;
		}




		//void new()
		public EQoSquareRoot()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fSquareRoot;

			this.Equ = new oEquation();
			this.ListEquation.Add(this.Equ);

		}
		public EQoSquareRoot(oEquation InEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fSquareRoot;

			this.Equ = InEq;
			this.ListEquation.Add(this.Equ);
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(0d);

			sEquationResult subrep = this.Equ.GetResult(TheCalculContext);
			if (subrep.AnErrorOccurred) { return subrep; }

			double dblrep = subrep.TheResult;
			if (dblrep < 0)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.RootOfNegativeNumber;
				return rep;
			}

			rep.TheResult = Math.Sqrt(dblrep);
			return rep;
		}



		public override string GetReadableName()
		{
			return "Racine Carrée";
		}

		public override oEquationObject GetCopy()
		{
			EQoSquareRoot copy = new EQoSquareRoot(this.Equ.GetCopy());
			return copy;
		}
		public override string GetSaveString()
		{
			return "\\4sqrt" + this.Equ.GetSaveString(true);
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			////on ajoute le code qui calcul l'équation à l'intérieur de la racine carré. après on appelle System.Math.Sqrt() sur le stack
			this.Equ.CompileToMSIL(cc); //met la réponse de l'équation sur le stack

			Label lblEnd = cc.il.DefineLabel(); //le label qui est à la fin de la racine carré
			Label lblError = cc.il.DefineLabel(); //le label qui mène à la génération de l'erreur

			//puisqu'on effectue une comparaison, il faut dupliquer le nombre auquel on applique la racine carré parce qu'il sera consommé
			cc.il.Emit(OpCodes.Dup);
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met zero sur le stack

			//vérifie si l'input de la racine carrée est inférieur à zero
			cc.il.Emit(OpCodes.Blt, lblError); // if < goto
			
			//on fait appeller la fonction racine carré
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Sqrt"));
			cc.il.Emit(OpCodes.Br, lblEnd); //on skip jusqu'à la fin du code de la racine carrée

			cc.il.MarkLabel(lblError);
			//on pop le contenue de la racine carrée du stack
			cc.il.Emit(OpCodes.Pop);
			//on met le message d'erreur sur le stack pour l'envoyer en paramètre
			cc.il.Emit(OpCodes.Ldstr, "square root of negative number");
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));
			//on retourne 0
			cc.il.Emit(OpCodes.Ldc_R8, 0d);

			//marque la fin
			cc.il.MarkLabel(lblEnd);

		}


	}
}
