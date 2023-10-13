using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoFraction : oEquationObject
	{

		//////ces variable indique les distance graphique entre plusieurs element
		private int zzzFracHeight = 5; // épaisseur de la barre de la fraction au centre
		private int zzzPaddingHor = 5; // indique de quel longueur la fraction dépasse à gauche et dépasse
		private int zzzPaddingVer = 3; // indique de quel longueur la fraction dépasse en haut et en bas




		public oEquation equUp;
		public oEquation equDown;


		public override int GetWidth()
		{
			int rep = 10;

			//obtien la largeur de l'equation la plus large
			int bigwidth = this.equUp.GetWidth();
			int downwidth = this.equDown.GetWidth();
			if (downwidth > bigwidth) { bigwidth = downwidth; }

			//ajoute les longueur horizontale suplementaire
			rep = this.zzzPaddingHor + bigwidth + this.zzzPaddingHor;

			return rep;
		}
		public override int GetHeight()
		{
			int rep = 10;
			rep = this.zzzPaddingVer + this.equUp.GetHeight() + this.zzzFracHeight + this.equDown.GetHeight() + this.zzzPaddingVer;
			return rep;
		}

		public override int GetUpHeight(int MyHeight)
		{
			return this.zzzPaddingVer + this.equUp.GetHeight() + 1 + (this.zzzFracHeight / 2);
		}

		public override Point GetPosOfEquationByIndex(int TheEquationIndex)
		{
			Point rep = new Point(-1, -1);
			if (TheEquationIndex == 0)
			{
				int thiswidth = this.GetWidth();
				int upwidth = this.equUp.GetWidth();

				int upleft = (thiswidth / 2) - (upwidth / 2);
				rep.X = upleft;
				rep.Y = this.zzzPaddingVer;
			}
			else if (TheEquationIndex == 1)
			{
				int thiswidth = this.GetWidth();
				int downwidth = this.equDown.GetWidth();

				int downleft = (thiswidth / 2) - (downwidth / 2);

				rep.X = downleft;
				rep.Y = this.zzzPaddingVer + this.equUp.GetHeight() + this.zzzFracHeight;
			}
			else
			{
				throw new Exception("index don't exist");
			}
			return rep;
		}


		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgwidth = this.GetWidth();
			int imgheight = this.GetHeight();
			Bitmap img = new Bitmap(imgwidth, imgheight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			//up
			Point eqUpPos = this.GetPosOfEquationByIndex(0);
			Bitmap imgUp = this.equUp.GetImage(TheDrawContext);
			g.DrawImage(imgUp, eqUpPos);
			this.equUp.uiLastTop = eqUpPos.Y;
			this.equUp.uiLastLeft = eqUpPos.X;
			this.equUp.uiLastWidth = imgUp.Width;
			this.equUp.uiLastHeight = imgUp.Height;

			//down
			Point eqDownPos = this.GetPosOfEquationByIndex(1);
			Bitmap imgDown = this.equDown.GetImage(TheDrawContext);
			g.DrawImage(imgDown, eqDownPos);
			this.equDown.uiLastTop = eqDownPos.Y;
			this.equDown.uiLastLeft = eqDownPos.X;
			this.equDown.uiLastWidth = imgDown.Width;
			this.equDown.uiLastHeight = imgDown.Height;

			//dessine la barre de la fraction
			int bartop = this.zzzPaddingVer + imgUp.Height + (this.zzzFracHeight / 2);
			g.DrawLine(new Pen(Color.Black, 3f), 0, bartop, imgwidth, bartop);


			imgUp.Dispose();
			imgDown.Dispose();
			g.Dispose();
			return img;
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(0d);

			sEquationResult repUp = this.equUp.GetResult(TheCalculContext);
			sEquationResult repDown = this.equDown.GetResult(TheCalculContext);

			//si une erreur s'est produite
			if (repUp.AnErrorOccurred || repDown.AnErrorOccurred)
			{
				if (repUp.AnErrorOccurred)
				{
					return repUp;
				}
				else
				{
					if (repDown.AnErrorOccurred)
					{
						return repDown;
					}
				}
			}
			else // si aucune erreur ne s'est produite
			{
				double numup = repUp.TheResult;
				double numdown = repDown.TheResult;

				if (numdown != 0)
				{
					rep.TheResult = numup / numdown;
					rep.ActualError = sEquationResult.ErrorType.NoError;
					rep.AnErrorOccurred = false;

				}
				else
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.DivisionByZero;
					rep.TheResult = 0;
				}
			}
			return rep;
		}



		//void new()
		public EQoFraction()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fFraction;

			this.equUp = new oEquation();
			this.equDown = new oEquation();
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);



		}
		public EQoFraction(oEquation UpEq, oEquation DownEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fFraction;

			this.equUp = UpEq;
			this.equDown = DownEq;
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);
		}


		public override string GetReadableName()
		{
			return "Fraction";
		}

		public override oEquationObject GetCopy()
		{
			EQoFraction copy = new EQoFraction(this.equUp.GetCopy(), this.equDown.GetCopy());
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\4frac" + this.equUp.GetSaveString(true) + this.equDown.GetSaveString(true);
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			////on met le code qui va faire calculer dans le stack l'équation d'en haut, celle du bas puis ensuite on ajoute div et il restera dans le stack d'évaluation la réponse

			this.equUp.CompileToMSIL(cc);
			this.equDown.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Div);
			

		}



	}
}
