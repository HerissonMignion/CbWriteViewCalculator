using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoRootN : oEquationObject
	{
		private int zzzLeftSpace = 7; // 7 espacement à gauche
		private int zzzRightSpace = 7; // 7 espacement à droite
		private int zzzBarreWidth = 10; // 10 espacement horizontale entre equRoot et equBase
		private int zzzUpBarreHeight = 4; // 4 height de la barre en haut à gauche
		private int zzzRootAltitude = 15; // 10 espacement en bas entre equRoot et le bas

		


		private oEquation equBase; //intérieur de la racine
		private oEquation equRoot; //équivalent de la puissance chez les exposant

		public override int GetWidth()
		{
			int TotalWidth = 0;

			TotalWidth += this.zzzLeftSpace;
			TotalWidth += this.equRoot.GetWidth();
			TotalWidth += this.zzzBarreWidth;
			TotalWidth += this.equBase.GetWidth();
			TotalWidth += this.zzzRightSpace;
			
			return TotalWidth;
		}
		public override int GetHeight()
		{
			int TotalHeight = 0;

			//partie gauche
			int leftTotalHeight = this.equRoot.GetHeight() + this.zzzRootAltitude;


			//partie droite
			int rightTotalHeight = this.zzzUpBarreHeight + this.equBase.GetHeight();


			//compare lequel des deux côté nécéssite le plus de height
			if (leftTotalHeight > rightTotalHeight)
			{
				TotalHeight = leftTotalHeight;
			}
			else
			{
				TotalHeight = rightTotalHeight;
			}

			return TotalHeight;
		}

		public override int GetUpHeight()
		{
			return this.GetUpHeight(this.GetHeight());
		}
		public override int GetUpHeight(int MyHeight)
		{
			Point baseHUpH = this.equBase.GetHeightAndUpHeight();
			int baseHeight = baseHUpH.X;
			int baseUpHeight = baseHUpH.Y;
			int rep = MyHeight - baseHeight + baseUpHeight;
			return rep;
		}


		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			////root
			Bitmap imgRoot = this.equRoot.GetImage(TheDrawContext);
			int rootTop = imgHeight - this.zzzRootAltitude - imgRoot.Height;
			int rootLeft = this.zzzLeftSpace;
			g.DrawImage(imgRoot, rootLeft, rootTop);
			this.equRoot.uiLastTop = rootTop;
			this.equRoot.uiLastLeft = rootLeft;
			this.equRoot.uiLastWidth = imgRoot.Width;
			this.equRoot.uiLastHeight = imgRoot.Height;
			imgRoot.Dispose();

			////base
			Bitmap imgBase = this.equBase.GetImage(TheDrawContext);
			int baseLeft = imgWidth - this.zzzRightSpace - imgBase.Width;
			int baseTop = imgHeight - imgBase.Height;
			g.DrawImage(imgBase, baseLeft, baseTop);
			this.equBase.uiLastTop = baseTop;
			this.equBase.uiLastLeft = baseLeft;
			this.equBase.uiLastWidth = imgBase.Width;
			this.equBase.uiLastHeight = imgBase.Height;

			//dessine les barre de la racine
			g.DrawLine(Pens.Black, baseLeft - 2, baseTop - 2, imgWidth, baseTop - 2); //en haut à droite
			g.DrawLine(Pens.Black, baseLeft - 2, baseTop - 2, baseLeft - 2, imgHeight - 1); //partie verticale
			g.DrawLine(Pens.Black, baseLeft - 2, imgHeight - 1, baseLeft - this.zzzBarreWidth, imgHeight - this.zzzRootAltitude); //partie en diagonalde
			g.DrawLine(Pens.Black, baseLeft - this.zzzBarreWidth, imgHeight - this.zzzRootAltitude, this.zzzLeftSpace, imgHeight - this.zzzRootAltitude); //barre horizontale en dessous de equRoot


			imgBase.Dispose();



			g.Dispose();
			return img;
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rBase = this.equBase.GetResult(TheCalculContext);
			if (rBase.AnErrorOccurred) { return rBase; }
			sEquationResult rRoot = this.equRoot.GetResult(TheCalculContext);
			if (rRoot.AnErrorOccurred) { return rRoot; }
			double dBase = rBase.TheResult;
			double dRoot = rRoot.TheResult;


			if (dRoot == 0d)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.NaN;
			}

			if (dBase == 0d)
			{
				if (dRoot > 0d)
				{
					rep.AnErrorOccurred = false;
					rep.TheResult = 0d;
					return rep;
				}
				else
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.NaN;
					return rep;
				}
			}
			if (dBase > 0d)
			{
				if (dRoot == 0d)
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.NaN;
					return rep;
				}
				else
				{
					rep.AnErrorOccurred = false;
					rep.TheResult = Math.Pow(dBase, 1d / dRoot);
					return rep;
				}
			}
			if (dBase < 0d)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.RootOfNegativeNumber;
				return rep;
			}


			return rep;
		}




		//void new()
		public EQoRootN()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fRootN;

			this.equBase = new oEquation();
			this.equRoot = new oEquation();
			this.ListEquation.Add(this.equBase);
			this.ListEquation.Add(this.equRoot);
			
		}
		public EQoRootN(oEquation BaseEq, oEquation RootEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fRootN;

			this.equBase = BaseEq;
			this.equRoot = RootEq;
			this.ListEquation.Add(this.equBase);
			this.ListEquation.Add(this.equRoot);
		}




		public override string GetReadableName()
		{
			return "Racine n-ième";
		}

		public override oEquationObject GetCopy()
		{
			EQoRootN copy = new EQoRootN(this.equBase.GetCopy(), this.equRoot.GetCopy());
			return copy;
		}
		public override string GetSaveString()
		{
			string strqBase = this.equBase.GetSaveString(true);
			string strqRoot = this.equRoot.GetSaveString(true);
			return "\\5rootn" + strqBase + strqRoot;
		}


		public override void CompileToMSIL(CompileContext cc)
		{
			////pour la racine n-ième, on utilise la fonction pow, mais on fait 1/n pour la puissance
			Label lblEnd = cc.il.DefineLabel();
			Label lblError = cc.il.DefineLabel();

			Label lblPart1 = cc.il.DefineLabel(); // x == 0
			Label lblPart2 = cc.il.DefineLabel(); // x < 0
			Label lblPart3 = cc.il.DefineLabel(); // x > 0
			

			//on met l'intérieur de la racine sur le stack
			this.equBase.CompileToMSIL(cc); // = x

			//////part 1
			////si x == 0, on goto part1, sinon c'est un goto automatique à part2.
			cc.il.Emit(OpCodes.Dup); //on duplique x parce qu'il sera consomé
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack
			cc.il.Emit(OpCodes.Beq, lblPart1); //si x == 0, on goto part1
			cc.il.Emit(OpCodes.Br, lblPart2); //goto part2
			cc.il.MarkLabel(lblPart1); // x == 0

			////rendu ici, x == 0
			Label lblPart1NoError = cc.il.DefineLabel();
			//on ne génère pas d'erreur si n > 0
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack
			cc.il.Emit(OpCodes.Bgt, lblPart1NoError); //on skip la génération de l'erreur si x > 0

			cc.il.Emit(OpCodes.Ldstr, "impossible root of 0");
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

			cc.il.MarkLabel(lblPart1NoError);
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met la réponse. c'est aussi la valeur par défaut qu'on laisse sur le stack en cas d'erreur
			
			cc.il.Emit(OpCodes.Br, lblEnd);
			//////part 2
			cc.il.MarkLabel(lblPart2);
			////on check si x > 0.
			cc.il.Emit(OpCodes.Dup); //on duplique x parce qu'il sera consommé
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack
			cc.il.Emit(OpCodes.Bgt, lblPart3); //si x > 0d, on goto part3
			
			////rendu ici, x < 0
			cc.il.Emit(OpCodes.Neg); //il faut inverser le signe de x.

			////on fait calculer n
			cc.il.Emit(OpCodes.Ldc_R8, 1d); //plus tard il faudra faire 1d / n
			this.equRoot.CompileToMSIL(cc); // = n
			Label lblPart2Integer = cc.il.DefineLabel(); //si n est un nombre entier
			Label lblPart2OddInteger = cc.il.DefineLabel(); //si n est un nombre entier impaire

			////on call la fonction "IsDoubleAnInteger" qui nous indique si ce double est un nombre entier
			cc.il.Emit(OpCodes.Dup); //il faut dupliquer n car il sera consommé
			cc.il.Emit(OpCodes.Call, typeof(CbWriteViewCalculator.cWriteViewAssets).GetMethod("IsDoubleAnInteger"));
			cc.il.Emit(OpCodes.Brtrue, lblPart2Integer); //si n est un nombre entier, on continue

			////si n n'est pas un nombre entier, il faut vider le stack, raiser l'erreur et mettre une réponse par défaut
			//on vide le stack
			cc.il.Emit(OpCodes.Pop); //pop n
			cc.il.Emit(OpCodes.Pop); //pop 1d
			cc.il.Emit(OpCodes.Pop); //pop x

			//on raise l'erreur
			cc.il.Emit(OpCodes.Ldstr, "impossible root of a negative number");
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));
			//on met une valeur par défaut sur le stack
			cc.il.Emit(OpCodes.Ldc_R8, 1d);

			cc.il.Emit(OpCodes.Br, lblEnd);
			cc.il.MarkLabel(lblPart2Integer);
			////ici, n est un nombre entier relatif. il faut maintenant vérifier qu'il est impair, peut importe son signe.
			////pour cette vérification, il faut d'abord convertir n en int32
			cc.il.Emit(OpCodes.Dup); //il faut dupliquer n car il sera consommé

			//cc.il.Emit(OpCodes.Ldc_R8, 0.49d);
			//cc.il.Emit(OpCodes.Add);
			//cc.il.Emit(OpCodes.Conv_I4);
			cc.il.Emit(OpCodes.Call, typeof(System.Convert).GetMethod("ToInt32", new Type[] { typeof(double) }));
			
			cc.il.Emit(OpCodes.Call, typeof(CbWriteViewCalculator.cWriteViewAssets).GetMethod("IsImpair2"));
			cc.il.Emit(OpCodes.Brtrue, lblPart2OddInteger); //on goto là bas si n est un nombre impair, peut importe son signe

			////rendu ici, il faut vider le stack, raiser l'erreur puis mettre une valeur par défaut sur le stack
			//on vide le stack
			cc.il.Emit(OpCodes.Pop); //pop n
			cc.il.Emit(OpCodes.Pop); //pop 1d
			cc.il.Emit(OpCodes.Pop); //pop x

			//on raise l'erreur
			cc.il.Emit(OpCodes.Ldstr, "impossible root of a negative number");
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));
			//on met une valeur par défaut sur le stack
			cc.il.Emit(OpCodes.Ldc_R8, 1d);
			
			cc.il.Emit(OpCodes.Br, lblEnd);
			cc.il.MarkLabel(lblPart2OddInteger);
			////rendu ici, n est un nombre entier impair, peut importe son signe

			//on fait calculer 1d / n
			cc.il.Emit(OpCodes.Div);
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Pow")); //on calcul x ^ (1d / n)

			//il faut inverser le signe de la réponse puisque nous sommes dans les x négatif
			cc.il.Emit(OpCodes.Neg);
			
			cc.il.Emit(OpCodes.Br, lblEnd);
			//////part 3
			cc.il.MarkLabel(lblPart3); // x > 0
			Label lblPart3Error = cc.il.DefineLabel();
			////rendu ici, x > 0
			//il faut calculer la valeur de 1 / n
			cc.il.Emit(OpCodes.Ldc_R8, 1d); //on load 1d sur le stack, plus tard il faudra calculer 1d / n.
			this.equRoot.CompileToMSIL(cc); // = n. on fait calculer la root

			//si la root == 0d, il faut raiser une erreur
			cc.il.Emit(OpCodes.Dup); //il faut dupliquer la valeur de n
			cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack
			cc.il.Emit(OpCodes.Beq, lblPart3Error); //si n == 0, on goto part3 error

			////rendu ici, n != 0. il faut juste calculer la réponse
			cc.il.Emit(OpCodes.Div); //on calcul 1d / n
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Pow")); //on calcul x ^ (1d / n). la réponse restera sur le stack pour qui en a besoin à l'extérieur de this.
			cc.il.Emit(OpCodes.Br, lblEnd); //on a terminé le calcul
			cc.il.MarkLabel(lblPart3Error);
			////si n == 0, il faut vider le stack, raiser l'erreur puis mettre une valeur défault sur le stack pour ne pas déstabiliser le stack.
			//on vide le stack
			cc.il.Emit(OpCodes.Pop); //pop n
			cc.il.Emit(OpCodes.Pop); //pop 1d
			cc.il.Emit(OpCodes.Pop); //pop x

			//on raise l'erreur
			cc.il.Emit(OpCodes.Ldstr, "root of 0 is impossible");
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));
			//on met 1d sur le stack. this aura indiqué qu'il vaut 1d. on load cette valeur pour ne pas déstabiliser le stack
			cc.il.Emit(OpCodes.Ldc_R8, 1d);
			cc.il.Emit(OpCodes.Br, lblEnd);
			//////// fin
			//erreur
			cc.il.MarkLabel(lblError);
			cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));
			//la fin
			cc.il.MarkLabel(lblEnd);

			////appelle la fonction pow
			//cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Pow"));
		}

		

		public void Old_CompileToMSIL(CompileContext cc)
		{
			////pour la racine n-ième, on utilise la fonction pow, mais on fait 1/x pour la puissance

			//on met l'intérieur de la racine sur le stack
			this.equBase.CompileToMSIL(cc);

			//on met 1d, on met la réponse de la root, ensuite on fait div pour avoir en puissance 1/root
			cc.il.Emit(OpCodes.Ldc_R8, 1d);
			this.equRoot.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Div);

			//appelle la fonction pow
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Pow"));

		}






	}
}
