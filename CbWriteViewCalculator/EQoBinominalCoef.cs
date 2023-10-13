using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoBinominalCoef : oEquationObject
	{
		private int zzzParentheseWidth = 10; // 10
		private int zzzSpaceUpDown = 4; // 4
		private int zzzSpaceMiddle = 3;




		private oEquation equUp;
		private oEquation equDown;


		public override int GetWidth()
		{
			int TheLarger = this.equUp.GetWidth();

			int downequWidth = this.equDown.GetWidth();
			if (downequWidth > TheLarger) { TheLarger = downequWidth; }

			return this.zzzParentheseWidth + TheLarger + this.zzzParentheseWidth;
		}
		public override int GetHeight()
		{
			int equupHeight = this.equUp.GetHeight();
			int equdownHeight = this.equDown.GetHeight();
			return this.zzzSpaceUpDown + equupHeight + this.zzzSpaceMiddle + equdownHeight + this.zzzSpaceUpDown;
		}
		public override int GetUpHeight()
		{
			return base.GetUpHeight();
		}
		public override int GetUpHeight(int MyHeight)
		{
			return MyHeight / 2;
		}



		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);


			////dessine les parenthese
			Bitmap imgpLeft = cWriteViewAssets.GetImgParentheseLeft(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(imgpLeft, 0, 0);
			imgpLeft.Dispose();

			Bitmap imgpRight = cWriteViewAssets.GetImgParentheseRight(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(imgpRight, imgWidth - this.zzzParentheseWidth, 0);
			imgpRight.Dispose();


			////dessine les équation
			Bitmap imgUp = this.equUp.GetImage(TheDrawContext);
			int upLeft = (imgWidth / 2) - (imgUp.Width / 2);
			g.DrawImage(imgUp, upLeft, this.zzzSpaceUpDown);
			this.equUp.uiLastLeft = upLeft;
			this.equUp.uiLastTop = this.zzzSpaceUpDown;
			this.equUp.uiLastWidth = imgUp.Width;
			this.equUp.uiLastHeight = imgUp.Height;
			imgUp.Dispose();


			Bitmap imgDown = this.equDown.GetImage(TheDrawContext);
			int downTop = imgHeight - this.zzzSpaceUpDown - imgDown.Height;
			int downLeft = (imgWidth / 2) - (imgDown.Width / 2);
			g.DrawImage(imgDown, downLeft, downTop);
			this.equDown.uiLastTop = downTop;
			this.equDown.uiLastLeft = downLeft;
			this.equDown.uiLastWidth = imgDown.Width;
			this.equDown.uiLastHeight = imgDown.Height;
			imgDown.Dispose();
			

			g.Dispose();
			return img;
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult serUp = this.equUp.GetResult(TheCalculContext);
			if (serUp.AnErrorOccurred) { return serUp; }
			sEquationResult serDown = this.equDown.GetResult(TheCalculContext);
			if (serDown.AnErrorOccurred) { return serDown; }

			double dblUp = serUp.TheResult;
			double dblDown = serDown.TheResult;

			try
			{
				double factup = cWriteViewAssets.Factorial(dblUp);
				double factdownLeft = cWriteViewAssets.Factorial(dblDown);
				double factdownRight = cWriteViewAssets.Factorial(dblUp - dblDown);
				rep.AnErrorOccurred = false;
				rep.TheResult = factup / factdownLeft / factdownRight;
			}
			catch
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.Unknown;
				return rep;
			}

			return rep;
		}



		//void new()
		public EQoBinominalCoef()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fBinominalCoef;

			this.equUp = new oEquation();
			this.equDown = new oEquation();
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);

		}
		public EQoBinominalCoef(oEquation UpEq, oEquation DownEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fBinominalCoef;

			this.equUp = UpEq;
			this.equDown = DownEq;
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);
		}


		public override string GetReadableName()
		{
			return "Coefficient Binominaux";
		}

		public override oEquationObject GetCopy()
		{
			return new EQoBinominalCoef(this.equUp.GetCopy(), this.equDown.GetCopy());
		}

		public override string GetSaveString()
		{
			return "\\9binomcoef" + this.equUp.GetSaveString(true) + this.equDown.GetSaveString(true);
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			//met le code qui fait calculer l'équation d'en haut
			this.equUp.CompileToMSIL(cc);

			//on duplique la réponse de l'équation d'en haut
			cc.il.Emit(OpCodes.Dup);

			//met le code qui fait calculer l'équation d'en bas
			this.equDown.CompileToMSIL(cc);

			//on stock la valeur de l'équation d'en bas dans une variable local
			LocalBuilder lbDown = cc.il.DeclareLocal(typeof(double));
			cc.il.Emit(OpCodes.Dup);
			cc.il.Emit(OpCodes.Stloc, lbDown.LocalIndex);

			//calcul la différence entre up et down et le stock dans une variable local
			cc.il.Emit(OpCodes.Sub);
			LocalBuilder lbUpDown = cc.il.DeclareLocal(typeof(double));
			cc.il.Emit(OpCodes.Stloc, lbUpDown);

			//call la fonction factoriel sur up
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("Factorial"));
			//le divise par down!
			cc.il.Emit(OpCodes.Ldloc, lbDown.LocalIndex);
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("Factorial"));
			cc.il.Emit(OpCodes.Div);
			//le divise par (up-down)!
			cc.il.Emit(OpCodes.Ldloc, lbUpDown.LocalIndex);
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("Factorial"));
			cc.il.Emit(OpCodes.Div);





			//double factup = cWriteViewAssets.Factorial(dblUp);
			//double factdownLeft = cWriteViewAssets.Factorial(dblDown);
			//double factdownRight = cWriteViewAssets.Factorial(dblUp - dblDown);
			//rep.AnErrorOccurred = false;
			//rep.TheResult = factup / factdownLeft / factdownRight;

		}


	}
}
