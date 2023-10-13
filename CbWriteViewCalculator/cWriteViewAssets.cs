using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CbWriteViewCalculator
{
	public static class cWriteViewAssets
	{

		public static Random RND = new Random();


		//retourne true si le nombre est impair sinon retourne false. pour tout les nombre <= 0 il va retourner false
		public static bool IsImpair(int TheNumber)
		{
			bool rep = false;
			//si ca vaut la peine de faire une analyse
			if (TheNumber > 0)
			{
				int result = TheNumber % 2;
				if (result == 1)
				{
					rep = true;
				}
				else
				{
					rep = false;
				}
			}
			return rep;
		}
		//retourne true si le nombre est impair, peut importe qu'il soit positif ou négatif
		public static bool IsImpair2(int TheNumber)
		{
			int temp = TheNumber;
			if (temp < 0) { temp = -temp; }
			return temp % 2 == 1;
		}


		public static bool IsANumber(string TheStr, bool AllowDot = false, bool AcceptNegate = false)
		{
			if (TheStr.Length <= 0) { return false; }

			string TheStr2 = TheStr.Trim();
			if (AcceptNegate)
			{
				if (TheStr2.Length > 0)
				{
					if (TheStr2.Substring(0, 1) == "-")
					{
						TheStr2 = TheStr2.Substring(1);
					}
				}
			}

			string TheStr3 = TheStr2;
			bool rep = true;
			foreach (char c in TheStr3)
			{
				string sc = c.ToString();
				if (!(sc == "0" || sc == "1" || sc == "2" || sc == "3" || sc == "4" || sc == "5" || sc == "6" || sc == "7" || sc == "8" || sc == "9" || (AllowDot && (sc == "." || sc == ","))))
				{
					rep = false;
					break;
				}
			}
			//verifi la quantiter de point
			if (rep && AllowDot)
			{
				bool FirstDotEncountered = false;
				foreach (char c in TheStr3)
				{
					string sc = c.ToString();
					if (sc == "." || sc == ",")
					{
						if (FirstDotEncountered)
						{
							rep = false;
							break;
						}
						else
						{
							FirstDotEncountered = true;
						}
					}
				}
			}
			return rep;
		}
		public static bool IsAVariableName(string TheStr)
		{
			if (TheStr.Length <= 0) { return false; }

			bool isnumber = cWriteViewAssets.IsANumber(TheStr, true, true);
			if (isnumber) { return false; }

			if (TheStr.Length <= 0 || TheStr.Trim().Length != TheStr.Length) { return false; }

			string first = TheStr.Substring(0, 1);
			string BannedFirst = "0123456789,.";
			for (int i = 0; i <= BannedFirst.Length - 1; i++)
			{
				string ban = BannedFirst.Substring(i, 1);
				if (first == ban) { return false; }
			}

			return true;
		}
		public static bool IsATrigFunctionName(string name)
		{
			if (name == "sin" || name == "arcsin" || name == "sinh" || name == "arsinh") { return true; }
			if (name == "cos" || name == "arccos" || name == "cosh" || name == "arcosh") { return true; }
			if (name == "tan" || name == "arctan" || name == "tanh" || name == "artanh") { return true; }
			if (name == "sec" || name == "arcsec" || name == "sech" || name == "arsech") { return true; }
			if (name == "csc" || name == "arccsc" || name == "csch" || name == "arcsch") { return true; }
			if (name == "cot" || name == "arccot" || name == "coth" || name == "arcoth") { return true; }
			return false;
		}

		//retourne si un double représente une valeur qui est un entier positif ou négatif et avec 0 inclut, comme 4.000000000000001
		public static bool IsDoubleAnInteger(double x)
		{
			//check pour 0
			if (x > -1E-13 && x < 1E-13) { return true; }
			//tout sauf 0
			return (int)(x + 1E-13) == (int)(x - 1E-13) + 1;
		}


		//retourne une image de parenthese ( ou )
		public static Bitmap GetImgParentheseLeft(int Height, int Width)
		{
			return cWriteViewAssets.GetImgParentheseLeft(Height, Width, Color.White);
		}
		public static Bitmap GetImgParentheseLeft(int Height, int Width, Color BackColor)
		{
			Bitmap img = new Bitmap(Width, Height);
			Graphics g = Graphics.FromImage(img);
			g.Clear(BackColor);

			if (Height >= 15)
			{

				g.DrawLine(Pens.Black, Width - 2, 1, 1, 6);
				g.DrawLine(Pens.Black, 1, 6, 1, Height - 7);
				g.DrawLine(Pens.Black, 1, Height - 7, Width - 2, Height - 2);


			}
			else
			{
				int HalfHeight = Height / 2;

				g.DrawLine(Pens.Black, Width - 2, 1, 1, HalfHeight);
				g.DrawLine(Pens.Black, 1, HalfHeight, Width - 2, Height - 2);


			}

			g.Dispose();
			return img;
		}
		public static Bitmap GetImgParentheseRight(int Height, int Width)
		{
			return cWriteViewAssets.GetImgParentheseRight(Height, Width, Color.White);
		}
		public static Bitmap GetImgParentheseRight(int Height, int Width, Color BackColor)
		{
			Bitmap img = new Bitmap(Width, Height);
			Graphics g = Graphics.FromImage(img);
			g.Clear(BackColor);

			if (Height >= 15)
			{

				g.DrawLine(Pens.Black, 1, 1, Width - 2, 6);
				g.DrawLine(Pens.Black, Width - 2, 6, Width - 2, Height - 7);
				g.DrawLine(Pens.Black, Width - 2, Height - 7, 1, Height - 2);


			}
			else
			{
				int HalfHeight = Height / 2;

				g.DrawLine(Pens.Black, Width - 2, 1, 1, HalfHeight);
				g.DrawLine(Pens.Black, 1, HalfHeight, Width - 2, Height - 2);


			}

			g.Dispose();
			return img;
		}
		
		public static Bitmap GetImgOppAdd()
		{
			return cWriteViewAssets.GetImgOppAdd(Color.White);
		}
		public static Bitmap GetImgOppAdd(Color BackColor)
		{
			//variable d'ajustement
			int imgWidth = 19; //à l'"époque" où les image étaient précrée dans le fichier de resource, elle avait la taille 15x15
			int spacement = 1;
			float PenWidth = 3f;

			//debut
			int demiWidth = imgWidth / 2;
			Pen thepen = new Pen(Color.Black, PenWidth);
			Bitmap img = new Bitmap(imgWidth, imgWidth);
			using (Graphics g = Graphics.FromImage(img))
			{
				g.Clear(BackColor);
				g.DrawLine(thepen, spacement, demiWidth, imgWidth - spacement, demiWidth);
				g.DrawLine(thepen, demiWidth, spacement, demiWidth, imgWidth - spacement);
			}
			return img;
		}
		public static Bitmap GetImgOppSub()
		{
			return cWriteViewAssets.GetImgOppSub(Color.White);
		}
		public static Bitmap GetImgOppSub(Color BackColor)
		{
			//variable d'ajustement
			int imgWidth = 20; //à l'"époque" où les image étaient précrée dans le fichier de resource, elle avait la taille 15x15
			int spacement = 1;
			float PenWidth = 3f;

			//debut
			int demiWidth = imgWidth / 2;
			Pen thepen = new Pen(Color.Black, PenWidth);
			Bitmap img = new Bitmap(imgWidth, imgWidth);
			using (Graphics g = Graphics.FromImage(img))
			{
				g.Clear(BackColor);
				g.DrawLine(thepen, spacement, demiWidth, imgWidth - spacement, demiWidth);
				//g.DrawLine(thepen, demiWidth, spacement, demiWidth, imgWidth - spacement);
			}
			return img;
		}
		public static Bitmap GetImgOppMulX()
		{
			return cWriteViewAssets.GetImgOppMulX(Color.White);
		}
		public static Bitmap GetImgOppMulX(Color BackColor)
		{
			//variable d'ajustement
			int imgWidth = 20; //à l'"époque" où les image étaient précrée dans le fichier de resource, elle avait la taille 15x15
			int spacement = 1;
			float PenWidth = 3f;

			//debut
			int demiWidth = imgWidth / 2;
			Pen thepen = new Pen(Color.Black, PenWidth);
			Bitmap img = new Bitmap(imgWidth, imgWidth);
			using (Graphics g = Graphics.FromImage(img))
			{
				g.Clear(BackColor);
				g.DrawLine(thepen, spacement, spacement, imgWidth - spacement, imgWidth - spacement);
				g.DrawLine(thepen, spacement, imgWidth - spacement, imgWidth - spacement, spacement);
			}
			return img;
		}
		public static Bitmap GetImgOppMulDot()
		{
			return cWriteViewAssets.GetImgOppMulDot(Color.White);
		}
		public static Bitmap GetImgOppMulDot(Color BackColor)
		{
			//variable d'ajustement
			int imgWidth = 10; //à l'"époque" où les image étaient précrée dans le fichier de resource, elle avait la taille 15x15
			//int spacement = 1;
			//float PenWidth = 3f;

			//debut
			float demiWidth = (float)imgWidth / 2f;
			Bitmap img = new Bitmap(imgWidth, imgWidth);
			using (Graphics g = Graphics.FromImage(img))
			{
				g.Clear(BackColor);

				float radius = (float)imgWidth / 4f;
				g.FillEllipse(Brushes.Black, demiWidth - radius, demiWidth - radius, 2f * radius, 2f * radius);
				

			}
			return img;
		}
		public static Bitmap GetImgOppDiv()
		{
			return cWriteViewAssets.GetImgOppDiv(Color.White);
		}
		public static Bitmap GetImgOppDiv(Color BackColor)
		{
			//variable d'ajustement
			int imgWidth = 20; //à l'"époque" où les image étaient précrée dans le fichier de resource, elle avait la taille 15x15
			int spacement = 1;
			float PenWidth = 3f;

			//debut
			int demiWidth = imgWidth / 2;
			Pen thepen = new Pen(Color.Black, PenWidth);
			Bitmap img = new Bitmap(imgWidth, imgWidth);
			using (Graphics g = Graphics.FromImage(img))
			{
				g.Clear(BackColor);
				g.DrawLine(thepen, spacement, demiWidth, imgWidth - spacement, demiWidth);


				int diam = demiWidth * 3 / 4;
				g.FillEllipse(Brushes.Black, demiWidth - (diam / 2) - 1, 0, diam, diam);
				g.FillEllipse(Brushes.Black, demiWidth - (diam / 2) - 1, imgWidth - diam, diam, diam);


			}
			return img;
		}

		//taille : 24x32
		public static Bitmap GetImgEmptyEquation()
		{
			int Width = 24;
			int Height = 32;
			Bitmap img = new Bitmap(Width, Height);
			Graphics g = Graphics.FromImage(img);
			g.Clear(Color.White);
			g.DrawRectangle(Pens.DimGray, 3, 3, Width - 3 - 4, Height - 3 - 4); // Black
			g.Dispose();
			return img;
		}
		
		public static string TrimStrNumber(string TheNum)
		{
			string rep = "";
			bool IsNegate = false;

			string num = TheNum.Replace("-", "");
			if (num.Length != TheNum.Length) { IsNegate = true; }

			if (num.Replace(".", "").Replace(",", "").Length == num.Length)
			{ // NOMBRE ENTIER
				rep = num.TrimStart("0".ToCharArray());
				if (rep.Length == 0) { return "0"; }
			}
			else // NOMBRE A VIRGULE
			{
				rep = num.Trim("0".ToCharArray());
				if (rep == "." || rep == ",") { return "0"; }
				else
				{
					string cstart = rep.Substring(0, 1);
					if (cstart == "." || cstart == ",")
					{
						rep = "0" + rep;
					}
					else
					{
						string cend = rep.Substring(rep.Length - 1);
						if (cend == "." || cend == ",")
						{
							rep = rep.Substring(0, rep.Length - 1);
						}
					}
				}

			}

			if (IsNegate) { rep = "-" + rep; }
			return rep;
		}
		public static string RemoveSpace(string str)
		{
			string rep = "";
			foreach (char c in str)
			{
				string cs = c.ToString();
				if (!string.IsNullOrWhiteSpace(cs))
				{
					rep += cs;
				}
			}
			return rep;
		}


		//optien la taille d'une chaine de text
		public static Size GetTextSize(string TheText, Font TheFont)
		{
			Bitmap img = new Bitmap(10, 10); //cette image sert juste a la creation d'un object graphics
			Graphics g = Graphics.FromImage(img);

			float TextHeight = g.MeasureString("NEBGREFCDpytqlkjhgfdb", TheFont).Height;
			float TextWidth = g.MeasureString(TheText, TheFont).Width;
			Size TextSize = new Size((int)TextWidth, (int)TextHeight);

			g.Dispose();
			return TextSize;
		}

		public static Point GetTextPosCenteredAt(string TheText, Font TextFont, Point CenterPos, Graphics g)
		{
			SizeF TextSizeF = g.MeasureString(TheText, TextFont);
			int SizeX = (int)(TextSizeF.Width);
			int SizeY = (int)(TextSizeF.Height);
			int repx = CenterPos.X - (SizeX / 2);
			int repy = CenterPos.Y - (SizeY / 2);
			return new Point(repx, repy);
		}
		public static Point GetTextPosCenteredAt(string TheText, Font TextFont, Point CenterPos)
		{
			Bitmap img = new Bitmap(10, 10);
			Graphics g = Graphics.FromImage(img);
			Point rep = cWriteViewAssets.GetTextPosCenteredAt(TheText, TextFont, CenterPos, g);
			g.Dispose();
			img.Dispose();
			return rep;
		}
		public static Point GetTextPosCenteredAt(string TheText, Font TextFont, Rectangle RectToCenter, Graphics g)
		{
			Point CenterPos = new Point(RectToCenter.X + (RectToCenter.Width / 2), RectToCenter.Y + (RectToCenter.Height / 2));
			Point rep = cWriteViewAssets.GetTextPosCenteredAt(TheText, TextFont, CenterPos, g);
			return rep;
		}
		public static Point GetTextPosCenteredAt(string TheText, Font TextFont, Rectangle RectToCenter)
		{
			Point CenterPos = new Point(RectToCenter.X + (RectToCenter.Width / 2), RectToCenter.Y + (RectToCenter.Height / 2));
			Bitmap img = new Bitmap(10, 10);
			Graphics g = Graphics.FromImage(img);
			Point rep = cWriteViewAssets.GetTextPosCenteredAt(TheText, TextFont, CenterPos, g);
			g.Dispose();
			img.Dispose();
			return rep;
		}

		public static Color MultiplyLightLevel(Color TheColor, float MulValue)
		{
			Color rep = Color.Black;

			float oRed = (float)TheColor.R;
			float oGreen = (float)TheColor.G;
			float oBlue = (float)TheColor.B;

			int rRed = (int)(oRed * MulValue);
			int rGreen = (int)(oGreen * MulValue);
			int rBlue = (int)(oBlue * MulValue);

			rep = Color.FromArgb(rRed, rGreen, rBlue);
			return rep;
		}
		public static Color AdjustBrightness(Color TheColor, float MulValue)
		{
			Color rep = Color.Black;

			float dRed = 255f - (float)TheColor.R;
			float dGreen = 255f - (float)TheColor.G;
			float dBlue = 255f - (float)TheColor.B;
			if (dRed < 0f) { dRed = 0f; }
			if (dGreen < 0f) { dGreen = 0f; }
			if (dBlue < 0f) { dBlue = 0f; }

			float ndRed = dRed * MulValue;
			float ndGreen = dGreen * MulValue;
			float ndBlue = dBlue * MulValue;

			int rRed = (int)(255f - ndRed);
			int rGreen = (int)(255f - ndGreen);
			int rBlue = (int)(255f - ndBlue);

			rep = Color.FromArgb(rRed, rGreen, rBlue);
			return rep;
		}




		public static double AGM(double a, double b)
		{
			if (a == 0d || b == 0d) { return 0d; }
			if ((a > 0d && b < 0d) || (a < 0d && b > 0d)) { return -1d; }
			double aa = a;
			double bb = b;

			while (true)
			{
				double newa = (aa + bb) / 2d;
				double newb = Math.Sqrt(aa * bb);

				double diff = newa - newb;
				if (diff < 0d) { diff = -diff; }
				//check s'il a fini de converger
				//si on calcul l'agm de nombre très grand et que le résulta est très grand, par exemple ~1000, il n'est pas toujours possible d'avoir une différence aussi basse que E-15
				if (diff < 5E-15)
				{
					break;
				}
				//si les valeur ne changent pas, c'est une autre raison d'arreter
				if (newa == aa || newa == bb)
				{
					break;
				}

				//next iteration
				aa = newa;
				bb = newb;
			}

			return aa;
		}

		public static double RoundDown(double x)
		{
			double rep = (double)(int)x;
			if (rep > x) { rep -= 1d; }
			return rep;
		}



		

		static double[] table_Factorial = new double[] { 0d, 0d, 0.8224670334241132d, -0.400685634386531d, 0.270580808427785d, -0.207385551028674d, 0.169557176997408d, -0.144049896768846d, 0.125509669524743d, -0.111334265869565d, 0.100099457512782d, -0.0909540171458291d, 0.083353840546109d, -0.0769325164113522d, 0.0714329462953613d, -0.0666687058824205d, 0.062500955141213d, -0.0588239786586846d, 0.0555557676274036d, -0.0526316793796167d, 0.0500000476981017d, -0.0476190703301422d, 0.0454545562932047d, -0.0434782660530403d, 0.0416666691503412d, -0.0400000011921401d, 0.0384615390346752d, -0.0370370373129893d, 0.0357142858473334d, -0.0344827586849193d, 0.0333333333643776d, -0.0322580645311504d, 0.031250000007276d, -0.030303030306558d, 0.0294117647075944d, -0.0285714285722601d, 0.027777777778182d, -0.0270270270272237d, 0.0263157894737799d, -0.0256410256410723d, 0.0250000000000227d, -0.0243902439024501d, 0.0238095238095292d, -0.023255813953491d, 0.022727272727274d, -0.0222222222222229d, 0.0217391304347829d, -0.021276595744681d, 0.0208333333333334d, -0.0204081632653062d, 0.02d };
		public static double Factorial(double x)
		{
			if (x == 0d || x == 1d) { return 1d; }
			if (x == 2d) { return 2d; }
			if (x == 3d) { return 6d; }

			//check si x peut être considéré comme un nombre entier tellement qu'il serait proche de l'un d'entre eux
			if ((int)(x + 1E-13) == (int)(x - 1E-13) + 1)
			{
				//on arrondi le nombre
				int ix = (int)(x + 0.49d);
				if (x < 0d) { ix = (int)(x - 0.49d); }

				////rendu ici, c'est que l'input de la fonction est un nombre entier. si le nombre est positif, on calcul son factoriel. si négatif, on retourne NaN
				if (ix > 0)
				{
					double rep = 1d;
					for (int i = ix; i > 1; i--)
					{
						rep *= (double)i;
					}

					return rep;
				}
				else
				{
					//si le nombre est un entier négatif, il faut retourner NaN
					return double.NaN;
				}
			}

			if (x >= -0.5d && x <= 0.5d)
			{
				//double eg = 0.5772156649015328606d;

				double sum = 0d;
				double ActualXPow = x * x;
				for (int k = 2; k <= 50; k++)
				{
					//double left = table_Factorial[k]; //FastZeta((double)k, 10000) / (double)k;
					//double right = Math.Pow(x, (double)k);
					sum += table_Factorial[k] * ActualXPow;
					//next iteration
					ActualXPow *= x;
				}

				sum -= 0.5772156649015328606d * x;

				return Math.Exp(sum);
			}

			//si x est un nombre non entier supérieur à 0.5
			if (x > 0.5d)
			{
				//on rapporte le calcule de x! à une valeur de x comprise entre -0.5 et 0.5
				double rep = 1d;
				double actual = x;
				while (actual > 0.5d)
				{
					rep *= actual;
					actual -= 1d;
				}
				rep *= Factorial(actual);
				return rep;
			}

			if (x < -0.5d)
			{
				double mul = 1d;
				double actual = x;
				while (actual < -0.5d)
				{
					actual += 1d;
					mul *= actual;
				}

				return Factorial(actual) / mul;
			}


			return 0d;
		}
		
		public static double Gamma(double x)
		{
			return Factorial(x - 1d);
		}


	}
}
