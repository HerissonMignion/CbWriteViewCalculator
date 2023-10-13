using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
    public class EQoNumber : oEquationObject
    {

        private double zzzActualValue = 12345d;
        private string zzzActualStrValue = "12345"; //valeur par default pour que l'utilisateur reconnaissee immediatement qu'il a bien placer un nombre. à cause du systeme de placement d'object, il pourait ne pas etre évident que l'opperation d'ajout a fonctionné
		// /!\ /!\ /!\ la valeur 12345 défini si-dessus est changé dans la void new()



        //specifie de quelle facon la valeur a ete defini : via un double ou un string. c'est important pour l'affichage du nombre
        public enum SettedObjectType
        {
            oDouble,
            oString
        }
        private SettedObjectType zzzActualSettedObjectType = SettedObjectType.oDouble;
        public SettedObjectType ActualSettedObjectType { get { return this.zzzActualSettedObjectType; } }


        public double ActualValue
        {
            get { return this.zzzActualValue; }
            set
            {
                this.zzzActualValue = value;
                this.zzzActualStrValue = value.ToString();
                this.zzzActualSettedObjectType = SettedObjectType.oDouble;
                this.RefreshImage();
            }
        }
        public string ActualStrValue
        {
            get { return this.zzzActualStrValue; }
            set
            {
                this.zzzActualStrValue = value;
				if (this.zzzActualStrValue == "") { this.zzzActualStrValue = "0"; }
                this.zzzActualValue = Convert.ToDouble(this.zzzActualStrValue.Replace(".", ","));
                this.zzzActualSettedObjectType = SettedObjectType.oString;
                this.RefreshImage();
            }
        }

		public bool IsNegate
		{
			get
			{
				return this.ActualValue < 0d;
			}
		}

        private Font zzzTextFont = new Font("lucida console", 20); // new Font("lucida console", 20);
		public Font TextFont
        {
            get { return this.zzzTextFont; }
            set
            {
                this.zzzTextFont = value;
                this.RefreshImage();
            }
        }

		
        private Bitmap zzzActualImg = new Bitmap(10, 10);
        public void RefreshImage()
        {
            Size TextSize = cWriteViewAssets.GetTextSize(this.ActualStrValue, this.TextFont);
			TextSize.Height = 22; // 22 pour Font("lucida console", 20)
			if (this.zzzActualImg != null) { this.zzzActualImg.Dispose(); }

			int RemovedWidth = 5;
            this.zzzActualImg = new Bitmap(TextSize.Width - RemovedWidth - RemovedWidth, TextSize.Height);
            Graphics g = Graphics.FromImage(this.zzzActualImg);
            g.Clear(this.BackColor);

            g.DrawString(this.ActualStrValue, this.TextFont, Brushes.Black, (float)-RemovedWidth, 0f);

			


        }

		public override int GetWidth()
		{
			return this.zzzActualImg.Width;
		}
		public override int GetHeight()
		{
			return this.zzzActualImg.Height;
		}
		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			Bitmap img = new Bitmap(this.zzzActualImg);
			using (Graphics g = Graphics.FromImage(img))
			{

				//s'il faut dessiner la bordure
				if (TheDrawContext.DrawBorderOfAllObject)
				{
					g.DrawRectangle(Pens.Silver, 0, 0, img.Width - 1, img.Height - 1);
				}
			}
			return img;
		}

		//void new()
		public EQoNumber(double StartValue = 12345d)
        {
            this.zzzActualEquationObjectType = EquationObjectType.Function;
            this.zzzActualSpecificObjectType = SpecificObjectType.fNumber;


            this.ActualValue = StartValue;
            this.RefreshImage(); //meme si actualvalue le fait, c'est juste pour etre sur
        }
		public EQoNumber(string StartValue = "12345")
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fNumber;


			this.ActualStrValue = StartValue;
			this.RefreshImage(); //meme si actualvalue le fait, c'est juste pour etre sur
		}

		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(this.ActualValue);
			return rep;
		}




		public override string GetReadableName()
		{
			return "Nombre";
		}

		public override oEquationObject GetCopy()
		{
			EQoNumber copy = new EQoNumber(this.ActualStrValue);
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\1n" + this.ActualStrValue + ";";
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			cc.il.Emit(OpCodes.Ldc_R8, this.ActualValue);
		}

	}
}
