using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoFactorial : oEquationObject
	{
		private int zzzSpaceUpDown = 2; // 2 espacement en haut et en bas
		private int zzzParentheseWidth = 8; // 8 largeur des parenthese. c'est aussi l'espacement à gauche lorsqu'il n'y a pas de parenthese
		private int zzzFactWidth = 7; // 7 largeur du symbole factoriel
		private int zzzFactHeight = 22; //height du symbole factoriel

		private int zzzMinHeight = 20; //hauteur minimale de l'intérieur

		private oEquation equ;

		public override int GetWidth()
		{
			int TotalWidth = 0;

			//left
			TotalWidth += this.zzzParentheseWidth;

			//middle
			TotalWidth += this.equ.GetWidth();

			//right
			if (this.DoesItNeedParenthese) { TotalWidth += this.zzzParentheseWidth; }
			TotalWidth += this.zzzFactWidth;
			
			return TotalWidth;
		}
		public override int GetHeight()
		{
			int equHeight = this.equ.GetHeight();
			if (equHeight < this.zzzMinHeight) { equHeight = this.zzzMinHeight; }
			return this.zzzSpaceUpDown + equHeight + this.zzzSpaceUpDown;
		}


		public bool DoesItNeedParenthese
		{
			get
			{
				if (this.equ.ListEquationObject.Count > 1) { return true; }
				if (this.equ.ListEquationObject.Count == 1)
				{
					if (this.equ.ListEquationObject[0].ActualEquationObjectType == EquationObjectType.Operator) { return true; }

					SpecificObjectType FirstType = this.equ.ListEquationObject[0].ActualSpecificObjectType;
					if (FirstType == SpecificObjectType.fFraction)
					{
						return true;
					}
					if (FirstType == SpecificObjectType.fExponent)
					{
						return true;
					}
					if (FirstType == SpecificObjectType.fRootN)
					{
						return true;
					}
					if (FirstType == SpecificObjectType.fNumber)
					{
						if (((EQoNumber)(this.equ.ListEquationObject[0])).IsNegate)
						{
							return true;
						}
					}
					if (FirstType == SpecificObjectType.fIntegral)
					{
						return true;
					}
					if (FirstType == SpecificObjectType.fSigmaSummation)
					{
						return true;
					}
					if (FirstType == SpecificObjectType.fPiProduct)
					{
						return true;
					}
				}
				return false;
			}
		}



		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			Bitmap equImg = this.equ.GetImage(TheDrawContext);
			int equWidth = equImg.Width;
			int equHeight = equImg.Height;
			int equLeft = this.zzzParentheseWidth;
			int equTop = (imgHeight / 2) - (equHeight / 2);
			g.DrawImage(equImg, equLeft, equTop);
			this.equ.uiLastLeft = equLeft;
			this.equ.uiLastTop = equTop;
			this.equ.uiLastWidth = equWidth;
			this.equ.uiLastHeight = equHeight;
			equImg.Dispose();
			
			if (this.DoesItNeedParenthese)
			{
				int pheight = imgHeight - (2 * this.zzzSpaceUpDown);
				int ptop = this.zzzSpaceUpDown;

				//left
				Bitmap pimgLeft = cWriteViewAssets.GetImgParentheseLeft(pheight, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pimgLeft, 0, ptop);
				pimgLeft.Dispose();

				//right
				Bitmap pimgRight = cWriteViewAssets.GetImgParentheseRight(pheight, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pimgRight, equLeft + equWidth, ptop);
				pimgRight.Dispose();
			}

			//symbole
			int factLeft = equLeft + equWidth;
			if (this.DoesItNeedParenthese) { factLeft += this.zzzParentheseWidth; }
			factLeft += (this.zzzFactWidth / 2);
			int factTop = (imgHeight / 2) - (this.zzzFactHeight / 2);
			Pen factp = new Pen(Color.Black, 2f);
			int downheight = 5;
			g.DrawLine(factp, factLeft, factTop, factLeft, factTop + this.zzzFactHeight - downheight);
			g.DrawLine(factp, factLeft, factTop + this.zzzFactHeight - downheight + 2, factLeft, factTop + this.zzzFactHeight);

			g.Dispose();
			return img;
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);
			try
			{
				sEquationResult equrep = this.equ.GetResult(TheCalculContext);
				if (equrep.AnErrorOccurred) { return equrep; }

				double x = equrep.TheResult;

				rep.AnErrorOccurred = false;
				rep.TheResult = cWriteViewAssets.Factorial(x);
			}
			catch
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.Unknown;
			}
			return rep;
		}

		//void new()
		public EQoFactorial()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fFactorial;
			
			this.equ = new oEquation();
			this.ListEquation.Add(this.equ);
			
		}
		public EQoFactorial(oEquation InEqu)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fFactorial;

			this.equ = InEqu;
			this.ListEquation.Add(this.equ);
		}



		public override string GetReadableName()
		{
			return "Factoriel";
		}

		public override oEquationObject GetCopy()
		{
			EQoFactorial copy = new EQoFactorial(this.equ.GetCopy());
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\5fact!" + this.equ.GetSaveString(true);
		}




		public override void CompileToMSIL(CompileContext cc)
		{
			////ajoute sur le stack la réponse de l'équation puis appelle la fonction factoriel
			this.equ.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("Factorial"));

		}

	}
}
