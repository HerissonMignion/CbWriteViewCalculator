using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CbWriteViewCalculator
{
    public class EQoOperator : oEquationObject
    {


        public enum OperatorType
        {
            none,
            Addition,
            Substraction,
            Multiplication,
            Division
        }
        private OperatorType zzzActualOperatorType = OperatorType.none;
        public OperatorType ActualOperatorType
        {
            get { return this.zzzActualOperatorType; }
            set { this.zzzActualOperatorType = value; }
        }
		public void SetOpperatorType(OperatorType NewType)
		{
			this.zzzActualOperatorType = NewType;
			switch (NewType)
			{
				case OperatorType.Addition:
					this.zzzActualSpecificObjectType = SpecificObjectType.mAddition;
					break;
				case OperatorType.Substraction:
					this.zzzActualSpecificObjectType = SpecificObjectType.mSubstraction;
					break;
				case OperatorType.Multiplication:
					this.zzzActualSpecificObjectType = SpecificObjectType.mMultiplication;
					break;
				case OperatorType.Division:
					this.zzzActualSpecificObjectType = SpecificObjectType.mDivision;
					break;
				default:
				case OperatorType.none:
					this.zzzActualSpecificObjectType = SpecificObjectType.none;
					break;
			}

			this.RefreshImage(true);
		}
		public bool UseMulDot = true;


		private int zzzImgNullSize = 15; //taille d'une image pour OperatorType.none

		public override int GetWidth()
        {
            int rep = this.zzzImgNullSize;
			if (this.ActualOperatorType != OperatorType.none && this.zzzActualImage != null)
			{
				rep = this.zzzActualImage.Width;
			}
            return rep;
        }
        public override int GetHeight()
		{
			int rep = this.zzzImgNullSize;
			if (this.ActualOperatorType != OperatorType.none && this.zzzActualImage != null)
			{
				rep = this.zzzActualImage.Height;
			}
			return rep;
		}

		private Bitmap zzzActualImage;
		private OperatorType uiLastOpperator = OperatorType.none;
		private bool uiLastUseMulDot = true; //false= ×      true= dot ·
		private Color uiLastBackColor = Color.Empty;
		private void RefreshImage(bool Force = false)
		{
			if (Force || (this.uiLastOpperator != this.ActualOperatorType || (this.ActualOperatorType == OperatorType.Multiplication && this.uiLastUseMulDot != this.UseMulDot) || this.uiLastBackColor != this.BackColor))
			{

				switch (this.ActualOperatorType)
				{
					default:
						this.zzzActualImage = new Bitmap(this.zzzImgNullSize, this.zzzImgNullSize);
						break;
					case OperatorType.Addition:
						this.zzzActualImage = cWriteViewAssets.GetImgOppAdd(this.BackColor);
						break;
					case OperatorType.Substraction:
						this.zzzActualImage = cWriteViewAssets.GetImgOppSub(this.BackColor);
						break;
					case OperatorType.Multiplication:
						if (this.UseMulDot)
						{
							this.zzzActualImage = cWriteViewAssets.GetImgOppMulDot(this.BackColor);
						}
						else
						{
							this.zzzActualImage = cWriteViewAssets.GetImgOppMulX(this.BackColor);
						}
						break;
					case OperatorType.Division:
						this.zzzActualImage = cWriteViewAssets.GetImgOppDiv(this.BackColor);
						break;
				}

				this.uiLastOpperator = this.ActualOperatorType;
				this.uiLastUseMulDot = this.UseMulDot;
				this.uiLastBackColor = this.BackColor;
			}
		}
        public override System.Drawing.Bitmap GetImage(oEquationDrawContext TheDrawContext)
        {
			this.UseMulDot = TheDrawContext.UiMultiplicationSymboleToUse == oEquationDrawContext.UiMultiplicationSymbole.symboleDot;

			this.RefreshImage();

            Bitmap img = new Bitmap(this.zzzActualImage);
            Graphics g = Graphics.FromImage(img);
			
            //s'il faut dessiner la bordure
            if (TheDrawContext.DrawBorderOfAllObject)
            {
                g.DrawRectangle(Pens.Silver, 0, 0, img.Width - 1, img.Height - 1);
            }


            g.Dispose();
            return img;
        }

        public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
        {
            sEquationResult rep = new sEquationResult(0d);
            rep.AnErrorOccurred = true;
            rep.ActualError = sEquationResult.ErrorType.NotAFunction;
            return rep;
        }


        //void new()
        public EQoOperator(OperatorType StartOperatorType = OperatorType.Addition)
        {
            this.zzzActualOperatorType = StartOperatorType;

            this.zzzActualEquationObjectType = EquationObjectType.Operator;
            switch (this.ActualOperatorType)
            {
                case OperatorType.Addition:
                    this.zzzActualSpecificObjectType = SpecificObjectType.mAddition;
                    break;
                case OperatorType.Substraction:
                    this.zzzActualSpecificObjectType = SpecificObjectType.mSubstraction;
                    break;
                case OperatorType.Multiplication:
                    this.zzzActualSpecificObjectType = SpecificObjectType.mMultiplication;
                    break;
                case OperatorType.Division:
                    this.zzzActualSpecificObjectType = SpecificObjectType.mDivision;
                    break;
                default:
                case OperatorType.none:
                    this.zzzActualSpecificObjectType = SpecificObjectType.none;
                    break;
            }

			this.RefreshImage(true);
        }





		public override string GetReadableName()
		{
			string rep = this.ToString();
			switch (this.ActualOperatorType)
			{
				case OperatorType.Addition:
					rep = "+";
					break;
				case OperatorType.Substraction:
					rep = "-";
					break;
				case OperatorType.Multiplication:
					rep = "×";
					break;
				case OperatorType.Division:
					rep = "÷";
					break;
			}

			return rep;
		}

		public override oEquationObject GetCopy()
		{
			EQoOperator copy = new EQoOperator(this.ActualOperatorType);
			return copy;
		}

		public override string GetSaveString()
		{
			//if (this.ActualOperatorType == OperatorType.Addition) { return "\\4madd"; }
			//if (this.ActualOperatorType == OperatorType.Substraction) { return "\\4msub"; }
			//if (this.ActualOperatorType == OperatorType.Multiplication) { return "\\4mmul"; }
			//if (this.ActualOperatorType == OperatorType.Division) { return "\\4mdiv"; }
			if (this.ActualOperatorType == OperatorType.Addition) { return "\\1+"; }
			if (this.ActualOperatorType == OperatorType.Substraction) { return "\\1-"; }
			if (this.ActualOperatorType == OperatorType.Multiplication) { return "\\4mmul"; }
			if (this.ActualOperatorType == OperatorType.Division) { return "\\4mdiv"; }
			return "\\4null";
		}
	}
}
