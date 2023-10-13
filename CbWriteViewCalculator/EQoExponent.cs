using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoExponent : oEquationObject
	{
		private int zzzParentheseWidth = 7; //défini également l'espacement à gauche et à droite




		private oEquation equBase;
		private oEquation equPow;



		public override int GetWidth()
		{
			int TotalWidth = 0;
			TotalWidth += this.zzzParentheseWidth;
			TotalWidth += this.equBase.GetWidth();
			if (this.DoesItNeedParenthese) { TotalWidth += this.zzzParentheseWidth; }
			TotalWidth += this.equPow.GetWidth();
			TotalWidth += this.zzzParentheseWidth;
			return TotalWidth;
		}
		public override int GetHeight()
		{
			int TotalHeight = 0;
			TotalHeight += this.equBase.GetHeight();
			TotalHeight += this.equPow.GetHeight();
			return TotalHeight;
		}
		public override int GetUpHeight()
		{
			return this.GetUpHeight(this.GetHeight());
		}
		public override int GetUpHeight(int MyHeight)
		{
			int baseHeight = this.equBase.GetHeight();
			//int powHeight = this.equPow.GetHeight();
			int rep = MyHeight - (baseHeight / 2);
			return rep;
		}

		public bool DoesItNeedParenthese
		{
			get
			{
				if (this.equBase.ListEquationObject.Count > 1) { return true; }
				if (this.equBase.ListEquationObject.Count == 1)
				{
					if (this.equBase.ListEquationObject[0].ActualEquationObjectType == EquationObjectType.Operator) { return true; }

					SpecificObjectType FirstType = this.equBase.ListEquationObject[0].ActualSpecificObjectType;
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
						if (((EQoNumber)(this.equBase.ListEquationObject[0])).IsNegate)
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

			////dessine la base
			Bitmap imgBase = this.equBase.GetImage(TheDrawContext);
			int baseTop = imgHeight - imgBase.Height;
			g.DrawImage(imgBase, this.zzzParentheseWidth, baseTop);
			this.equBase.uiLastTop = baseTop;
			this.equBase.uiLastLeft = this.zzzParentheseWidth;
			this.equBase.uiLastWidth = imgBase.Width;
			this.equBase.uiLastHeight = imgBase.Height;

			//dessine les parenthèse
			if (this.DoesItNeedParenthese)
			{
				//left
				Bitmap pLeft = cWriteViewAssets.GetImgParentheseLeft(imgBase.Height, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pLeft, 0, baseTop);
				pLeft.Dispose();

				//right
				Bitmap pRight = cWriteViewAssets.GetImgParentheseRight(imgBase.Height, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pRight, this.zzzParentheseWidth + imgBase.Width + 1, baseTop);
				pRight.Dispose();

			}

			imgBase.Dispose();

			////dessine la puissance
			Bitmap imgPow = this.equPow.GetImage(TheDrawContext);
			int powLeft = imgWidth - this.zzzParentheseWidth - imgPow.Width;
			g.DrawImage(imgPow, powLeft, 0);
			this.equPow.uiLastLeft = powLeft;
			this.equPow.uiLastTop = 0;
			this.equPow.uiLastWidth = imgPow.Width;
			this.equPow.uiLastHeight = imgPow.Height;
			imgPow.Dispose();




			g.Dispose();
			return img;
		}


		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1);

			sEquationResult rBase = this.equBase.GetResult(TheCalculContext);
			if (rBase.AnErrorOccurred) { return rBase; }
			sEquationResult rPow = this.equPow.GetResult(TheCalculContext);
			if (rPow.AnErrorOccurred) { return rPow; }

			double dBase = rBase.TheResult;
			double dPow = rPow.TheResult;

			if (dPow == 0d)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = 1d;
				return rep;
			}

			if (dBase == 0d)
			{
				if (dPow > 0d)
				{
					rep.AnErrorOccurred = false;
					rep.TheResult = 0d;
					return rep;
				}
				if (dPow == 0d)
				{
					rep.AnErrorOccurred = false;
					rep.TheResult = 1d;
					return rep;
				}
				if (dPow < 0d)
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.NaN;
					return rep;
				}
			}
			if (dBase > 0d)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = Math.Pow(dBase, dPow);
				return rep;
			}
			if (dBase < 0d)
			{
				if (Math.Round(dPow) == dPow)
				{
					rep.TheResult = Math.Pow(dBase, Math.Round(dPow));
					return rep;
				}
				else
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.NaN;
					return rep;
				}
			}
			

			return rep;
		}



		//void new()
		public EQoExponent()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fExponent;

			this.equBase = new oEquation();
			this.equPow = new oEquation();
			this.ListEquation.Add(this.equBase);
			this.ListEquation.Add(this.equPow);
			
		}
		public EQoExponent(oEquation BaseEq, oEquation PowEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fExponent;

			this.equBase = BaseEq;
			this.equPow = PowEq;
			this.ListEquation.Add(this.equBase);
			this.ListEquation.Add(this.equPow);
		}




		public override string GetReadableName()
		{
			return "Exposant";
		}

		public override oEquationObject GetCopy()
		{
			EQoExponent copy = new EQoExponent(this.equBase.GetCopy(), this.equPow.GetCopy());
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\3pow" + this.equBase.GetSaveString(true) + this.equPow.GetSaveString(true);
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			////met sur le stack la réponse de l'équation d'en bas, puis celle d'en haut, puis call Math.Pow(x, y)
			this.equBase.CompileToMSIL(cc);
			this.equPow.CompileToMSIL(cc);
			//maintenant call Math.Pow
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Pow"));

		}


	}
}
