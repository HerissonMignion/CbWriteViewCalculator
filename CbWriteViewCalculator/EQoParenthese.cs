using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
    public class EQoParenthese : oEquationObject
    {


        
        private int zzzParentheseWidth = 9; // 7
        private int zzzTopSpace = 2; // 2 hauteur suplementaire des parenthese dessus l'equation
        private int zzzDownSpace = 2; // 1 hauteur suplementaire dessous l'equation




        public override int GetWidth()
        {
            return this.ListEquation[0].GetWidth() + (2 * this.zzzParentheseWidth);
        }
        public override int GetHeight()
        {
            return this.ListEquation[0].GetHeight() + this.zzzTopSpace + this.zzzDownSpace;
        }

        public override Point GetPosOfEquationByIndex(int TheEquationIndex)
        {
            Point rep = new Point(0, 0);

            //pour les parenthese, il n'y a que l'index 0 qui exist donc une else fera l'affaire
            if (TheEquationIndex == 0)
            {
                rep = new Point(this.zzzParentheseWidth, this.zzzTopSpace);


            }
            else
            {
                //valeur par default d'un index inexistant
                //rep = new Point(-1, -1);

                // /!\ /!\ /!\ specialement pour les test, il genere une erreur pour que je puisse s'avoire qu'il y a des demande d'index innexistant. lorsque sa ne se produira plus, je remeterais la valeur par default
                throw new Exception("index don't exist");
            }


            return rep;
        }


        public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
        {
            Bitmap img = new Bitmap(this.GetWidth(), this.GetHeight());
            Graphics g = Graphics.FromImage(img);
            g.Clear(this.BackColor);

            
            Bitmap leftp = cWriteViewAssets.GetImgParentheseLeft(img.Height, this.zzzParentheseWidth, this.BackColor);
            Bitmap rightp = cWriteViewAssets.GetImgParentheseRight(img.Height, this.zzzParentheseWidth, this.BackColor);


            g.DrawImage(leftp, 0, 0);
			Bitmap SubEquationImg = this.ListEquation[0].GetImage(TheDrawContext);
			this.ListEquation[0].uiLastTop = this.zzzTopSpace;
			this.ListEquation[0].uiLastLeft = leftp.Width;
			this.ListEquation[0].uiLastWidth = SubEquationImg.Width;
			this.ListEquation[0].uiLastHeight = SubEquationImg.Height;
			g.DrawImage(SubEquationImg, leftp.Width, this.zzzTopSpace);
            g.DrawImage(rightp, img.Width - rightp.Width, 0);


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
            return this.ListEquation[0].GetResult(TheCalculContext);
        }


        



        //void new()
        public EQoParenthese()
        {
            this.zzzActualEquationObjectType = EquationObjectType.Function;
            this.zzzActualSpecificObjectType = SpecificObjectType.fParenthese;

            oEquation MainEquation = new oEquation();
            this.ListEquation.Add(MainEquation);
			
        }
		public EQoParenthese(oEquation MainEqu)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fParenthese;

			oEquation MainEquation = MainEqu;
			this.ListEquation.Add(MainEquation);
		}



		public override string GetReadableName()
		{
			return "Parenthèse";
		}

		public override oEquationObject GetCopy()
		{
			EQoParenthese copy = new EQoParenthese(this.ListEquation[0].GetCopy());
			return copy;
		}

		public override string GetSaveString()
		{
			string rep = "\\6subequ";
			string strequ = this.ListEquation[0].GetSaveString(true);
			rep += strequ;
			return rep;
		}




		public override void CompileToMSIL(CompileContext cc)
		{
			//on insère juste le code qui fait calculer l'équation à l'intérieur
			this.ListEquation[0].CompileToMSIL(cc);

		}


	}
}
