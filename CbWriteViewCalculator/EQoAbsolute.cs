using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoAbsolute : oEquationObject
	{
		private int zzzSpaceLeftRight = 10;
		private int zzzUpSpace = 2;
		private int zzzBarreDist = 2; //distance graphique qui sépare les deux barre verticale du bord de l'image

		private int MinHeight = 25;



		private oEquation Equ;


		public override int GetWidth()
		{
			int TotalWidth = 0;
			TotalWidth += this.zzzSpaceLeftRight;
			TotalWidth += this.Equ.GetWidth();
			TotalWidth += this.zzzSpaceLeftRight;
			return TotalWidth;
		}
		public override int GetHeight()
		{
			int MaxHeight = this.MinHeight;

			int CenterHeight = this.zzzUpSpace + this.Equ.GetHeight();
			if (CenterHeight > MaxHeight) { MaxHeight = CenterHeight; }

			return MaxHeight;
		}

		public override int GetUpHeight()
		{
			return this.GetUpHeight(this.GetHeight());
		}
		public override int GetUpHeight(int MyHeight)
		{
			int TotalUp = this.zzzUpSpace + this.Equ.GetHeightAndUpHeight().Y;
			return TotalUp;
		}

		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);


			g.DrawLine(Pens.Black, this.zzzBarreDist, 0, this.zzzBarreDist, imgHeight - 1);

			Bitmap imgEqu = this.Equ.GetImage(TheDrawContext);
			int equTop = imgHeight - imgEqu.Height;
			g.DrawImage(imgEqu, this.zzzSpaceLeftRight, equTop);
			this.Equ.uiLastLeft = this.zzzSpaceLeftRight;
			this.Equ.uiLastTop = equTop;
			this.Equ.uiLastWidth = imgEqu.Width;
			this.Equ.uiLastHeight = imgEqu.Height;
			imgEqu.Dispose();


			g.DrawLine(Pens.Black, imgWidth - this.zzzBarreDist - 1, 0, imgWidth - this.zzzBarreDist - 1, imgHeight - 1);


			g.Dispose();
			return img;
		}


		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rEqu = this.Equ.GetResult(TheCalculContext);
			if (rEqu.AnErrorOccurred) { return rEqu; }

			double x = rEqu.TheResult;
			if (x >= 0d)
			{
				rep.TheResult = x;
			}
			else
			{
				rep.TheResult = -x;
			}

			return rep;
		}


		//void new()
		public EQoAbsolute()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fAbsolute;
			
			this.Equ = new oEquation();
			this.ListEquation.Add(this.Equ);

		}
		public EQoAbsolute(oEquation InEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fAbsolute;

			this.Equ = InEq;
			this.ListEquation.Add(this.Equ);
		}




		public override string GetReadableName()
		{
			return "Valeur absolue";
		}

		public override oEquationObject GetCopy()
		{
			EQoAbsolute copy = new EQoAbsolute(this.Equ.GetCopy());
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\3abs" + this.Equ.GetSaveString(true);
		}




		public override void CompileToMSIL(CompileContext cc)
		{
			////on ajoute la réponse de l'équation intérieur au stack ensuite on call Math.Abs
			this.Equ.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Abs", new Type[] { typeof(double) }));

		}

	}
}
