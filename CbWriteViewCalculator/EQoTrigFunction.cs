using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoTrigFunction : oEquationObject
	{
		private int zzzParentheseWidth = 7;
		private int zzzSpaceUpDown = 2; //espacement en haut et en bas


		public Font NameFont = new Font("consolas", 15f); // new Font("consolas", 15f);

		private string zzzActualFuncStr = "";
		public string ActualFuncStr
		{
			get { return this.zzzActualFuncStr; }
		}
		private Size ActualNameSize = new Size(0, 0);

		public void SetActualFuncStr(string NewFunc)
		{
			this.zzzActualFuncStr = NewFunc;
			this.ActualNameSize = cWriteViewAssets.GetTextSize(NewFunc, this.NameFont);
		}


		private oEquation equ;



		public override int GetWidth()
		{
			int TotalWidth = 0;
			TotalWidth += this.ActualNameSize.Width;
			TotalWidth += this.zzzParentheseWidth;
			TotalWidth += this.equ.GetWidth();
			TotalWidth += this.zzzParentheseWidth;
			return TotalWidth;
		}
		public override int GetHeight()
		{
			int MaxHeight = this.ActualNameSize.Height;

			int RightHeight = this.zzzSpaceUpDown + this.equ.GetHeight() + this.zzzSpaceUpDown;
			if (RightHeight > MaxHeight) { MaxHeight = RightHeight; }

			return MaxHeight;
		}


		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			//ecrit le nom de la fonction
			g.DrawString(this.ActualFuncStr, this.NameFont, Brushes.Black, 0f, (float)((imgHeight / 2) - (this.ActualNameSize.Height / 2)));

			//dessine la parenthese gauche
			Bitmap imgpleft = cWriteViewAssets.GetImgParentheseLeft(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(imgpleft, this.ActualNameSize.Width, 0);
			imgpleft.Dispose();

			//dessine l'équation
			Bitmap imgequ = this.equ.GetImage(TheDrawContext);
			int equTop = (imgHeight / 2) - (imgequ.Height / 2);
			int equLeft = this.ActualNameSize.Width + this.zzzParentheseWidth;
			g.DrawImage(imgequ, equLeft, equTop);
			this.equ.uiLastTop = equTop;
			this.equ.uiLastLeft = equLeft;
			this.equ.uiLastWidth = imgequ.Width;
			this.equ.uiLastHeight = imgequ.Height;

			//desine la parenthese droite
			Bitmap imgpright = cWriteViewAssets.GetImgParentheseRight(imgHeight, this.zzzParentheseWidth, this.BackColor);
			g.DrawImage(imgpright, imgWidth - this.zzzParentheseWidth, 0);
			imgpright.Dispose();

			imgequ.Dispose();

			g.Dispose();
			return img;
		}




		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rEqu = this.equ.GetResult(TheCalculContext);
			if (rEqu.AnErrorOccurred) { return rEqu; }
			double x = rEqu.TheResult;

			try
			{

				if (this.ActualFuncStr == "sin")
				{
					rep.TheResult = Math.Sin(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "cos")
				{
					rep.TheResult = Math.Cos(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "tan")
				{
					rep.TheResult = Math.Tan(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "sec")
				{
					rep.TheResult = 1d / Math.Cos(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "csc")
				{
					rep.TheResult = 1d / Math.Sin(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "cot")
				{
					rep.TheResult = 1d / Math.Tan(TheCalculContext.angleConvertUsedToRadian(x));
				}
				else if (this.ActualFuncStr == "arcsin")
				{
					if (x < -1d || x > 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Asin(x));
				}
				else if (this.ActualFuncStr == "arccos")
				{
					if (x < -1d || x > 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Acos(x));
				}
				else if (this.ActualFuncStr == "arctan")
				{
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Atan(x));
				} 
				else if (this.ActualFuncStr == "arcsec")
				{
					if (x >= -1d && x <= 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Acos(1d / x));
				}
				else if (this.ActualFuncStr == "arccsc")
				{
					if (x >= -1d && x <= 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Asin(1d / x));
				}
				else if (this.ActualFuncStr == "arccot")
				{
					rep.TheResult = TheCalculContext.angleConvertRadianToUsed((Math.PI / 2d) - Math.Atan(x));
				}
				//hyperbolique function :
				else if (this.ActualFuncStr == "sinh")
				{
					rep.TheResult = Math.Sinh(x);
				}
				else if (this.ActualFuncStr == "cosh")
				{
					rep.TheResult = Math.Cosh(x);
				}
				else if (this.ActualFuncStr == "tanh")
				{
					rep.TheResult = Math.Tanh(x);
				}
				else if (this.ActualFuncStr == "sech")
				{
					rep.TheResult = 1d / Math.Cosh(x);
				}
				else if (this.ActualFuncStr == "csch")
				{
					rep.TheResult = 1d / Math.Sinh(x);
				}
				else if (this.ActualFuncStr == "coth")
				{
					double e2x = Math.Exp(x);
					e2x *= e2x;
					rep.TheResult = (e2x + 1d) / (e2x - 1d);
				}
				else if (this.ActualFuncStr == "arsinh")
				{
					rep.TheResult = Math.Log(x + Math.Sqrt((x * x) + 1d));
				}
				else if (this.ActualFuncStr == "arcosh")
				{
					if (x < 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = Math.Log(x + Math.Sqrt((x * x) - 1d));
				}
				else if (this.ActualFuncStr == "artanh")
				{
					if (x <= -1d || x >= 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = Math.Log((1d + x) / (1d - x)) / 2d;
				}
				else if (this.ActualFuncStr == "arsech")
				{
					if (x <= 0d || x > 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = Math.Log((1d + Math.Sqrt(1d - (x * x))) / x);
				}
				else if (this.ActualFuncStr == "arcsch")
				{
					rep.TheResult = Math.Log((1d / x) + Math.Sqrt((1d / (x * x)) + 1d));
				}
				else if (this.ActualFuncStr == "arcoth")
				{
					if (x >= -1d && x <= 1d)
					{
						rep.AnErrorOccurred = true;
						rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
						return rep;
					}
					rep.TheResult = Math.Log((x + 1d) / (x - 1d)) / 2d;
				}
				else
				{
					rep.AnErrorOccurred = true;
					rep.ActualError = sEquationResult.ErrorType.UndefinedTrigFunction;
					return rep;
				}



			}
			catch
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.Unknown;
			}

			return rep;
		}


		//void new()
		public EQoTrigFunction(string sFunc = "sin")
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fTrig;
			
			this.equ = new oEquation();
			this.ListEquation.Add(this.equ);

			this.SetActualFuncStr(sFunc);
		}
		public EQoTrigFunction(string sFunc, oEquation InEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fTrig;

			this.equ = InEq;
			this.ListEquation.Add(this.equ);

			this.SetActualFuncStr(sFunc);
		}


		public override string GetReadableName()
		{
			//return "Fonction trig : " + this.ActualFuncStr;
			return this.ActualFuncStr + "( )";
		}

		public override oEquationObject GetCopy()
		{
			EQoTrigFunction copy = new EQoTrigFunction(this.ActualFuncStr, this.equ.GetCopy());
			return copy;
		}
		public override string GetSaveString()
		{
			return "\\4trig" + this.ActualFuncStr + ";" + this.equ.GetSaveString(true);
		}




		public override void CompileToMSIL(CompileContext cc)
		{
			

			if (this.ActualFuncStr == "sin")
			{
				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le sin
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Sin"));
				

				//rep.TheResult = Math.Sin(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "cos")
			{
				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le cos
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Cos"));


				//rep.TheResult = Math.Cos(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "tan")
			{
				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le tan
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Tan"));


				//rep.TheResult = Math.Tan(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "sec")
			{
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //met 1d sur le stack

				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le cos
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Cos"));

				cc.il.Emit(OpCodes.Div);


				//rep.TheResult = 1d / Math.Cos(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "csc")
			{
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //met 1d dans le stack

				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le sin
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Sin"));

				cc.il.Emit(OpCodes.Div);


				//rep.TheResult = 1d / Math.Sin(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "cot")
			{
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //met 1d sur le stack

				//met sur le stack la réponse de l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//convertie les mesure d'angle s'il le faut
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Mul);
				}

				//maintenant on fait calculer le tan
				cc.il.Emit(OpCodes.Call, typeof(System.Math).GetMethod("Tan"));

				cc.il.Emit(OpCodes.Div);


				//rep.TheResult = 1d / Math.Tan(TheCalculContext.angleConvertUsedToRadian(x));
			}
			else if (this.ActualFuncStr == "arcsin")
			{
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();


				//on duplique la réponse de l'équation 2 fois pour effectuer 2 goto conditionnel
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Bgt, lblError); //goto err si x > 1
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, -1d);
				cc.il.Emit(OpCodes.Blt, lblError); //goto err si x < -1

				////calcul
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Asin"));

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}


				cc.il.Emit(OpCodes.Br, lblEnd);
				////erreur
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //il faut enlever du stack la réponse de l'équation
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack pour le garder stable
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arcsin");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

				//fin
				cc.il.MarkLabel(lblEnd);

				//if (x < -1d || x > 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Asin(x));
			}
			else if (this.ActualFuncStr == "arccos")
			{
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();


				//on duplique la réponse de l'équation 2 fois pour effectuer 2 goto conditionnel
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Bgt, lblError); //goto err si x > 1
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, -1d);
				cc.il.Emit(OpCodes.Blt, lblError); //goto err si x < -1

				////calcul
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Acos"));

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}


				cc.il.Emit(OpCodes.Br, lblEnd);
				////erreur
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //il faut enlever du stack la réponse de l'équation
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack pour le garder stable
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arccos");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

				//fin
				cc.il.MarkLabel(lblEnd);

				//if (x < -1d || x > 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Acos(x));
			}
			else if (this.ActualFuncStr == "arctan")
			{
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				//appelle la fonction arctangente
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Atan"));

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}


				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Atan(x));
			}
			else if (this.ActualFuncStr == "arcsec")
			{
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);
				cc.il.Emit(OpCodes.Div);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();


				//on duplique la réponse de l'équation 2 fois pour effectuer 2 goto conditionnel
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Bgt, lblError); //goto err si x > 1
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, -1d);
				cc.il.Emit(OpCodes.Blt, lblError); //goto err si x < -1

				////calcul
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Acos"));

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}


				cc.il.Emit(OpCodes.Br, lblEnd);
				////erreur
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //il faut enlever du stack la réponse de l'équation
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack pour le garder stable
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arcsec");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

				//fin
				cc.il.MarkLabel(lblEnd);

				//if (x >= -1d && x <= 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Acos(1d / x));
			}
			else if (this.ActualFuncStr == "arccsc")
			{
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);
				cc.il.Emit(OpCodes.Div);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();


				//on duplique la réponse de l'équation 2 fois pour effectuer 2 goto conditionnel
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Bgt, lblError); //goto err si x > 1
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, -1d);
				cc.il.Emit(OpCodes.Blt, lblError); //goto err si x < -1

				////calcul
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Asin"));

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}


				cc.il.Emit(OpCodes.Br, lblEnd);
				////erreur
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //il faut enlever du stack la réponse de l'équation
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0d sur le stack pour le garder stable
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arccsc");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

				//fin
				cc.il.MarkLabel(lblEnd);

				//if (x >= -1d && x <= 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed(Math.Asin(1d / x));
			}
			else if (this.ActualFuncStr == "arccot")
			{
				cc.il.Emit(OpCodes.Ldc_R8, Math.PI / 2d); //met pi/2 sur le stack
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//on call arctengente
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Atan"));

				cc.il.Emit(OpCodes.Sub); //pi/2 - atan(x)
				

				//on convertie vers l'unité d'angle utilisé
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 180d);
					cc.il.Emit(OpCodes.Mul);
				}
				if (cc.CalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian)
				{
					cc.il.Emit(OpCodes.Ldc_R8, Math.PI);
					cc.il.Emit(OpCodes.Div);
					cc.il.Emit(OpCodes.Ldc_R8, 200d);
					cc.il.Emit(OpCodes.Mul);
				}
				//rep.TheResult = TheCalculContext.angleConvertRadianToUsed((Math.PI / 2d) - Math.Atan(x));
			}
			//hyperbolic function :
			else if (this.ActualFuncStr == "sinh")
			{
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//call la fonction
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sinh"));
				
				//rep.TheResult = Math.Sinh(x);
			}
			else if (this.ActualFuncStr == "cosh")
			{
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//call la fonction
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Cosh"));

				//rep.TheResult = Math.Cosh(x);
			}
			else if (this.ActualFuncStr == "tanh")
			{
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//call la fonction
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Tanh"));

				//rep.TheResult = Math.Tanh(x);
			}
			else if (this.ActualFuncStr == "sech")
			{
				//met 1 sur le stack
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//call la fonction
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Cosh"));

				cc.il.Emit(OpCodes.Div);

				//rep.TheResult = 1d / Math.Cosh(x);
			}
			else if (this.ActualFuncStr == "csch")
			{
				//met 1 sur le stack
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//call la fonction
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sinh"));

				cc.il.Emit(OpCodes.Div);

				//rep.TheResult = 1d / Math.Sinh(x);
			}
			else if (this.ActualFuncStr == "coth")
			{
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);
				//call l'exponentiel dessus
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Exp"));
				//le met au carré
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Mul);

				//on calcule e2x + 1
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //on met 1 sur le stack
				cc.il.Emit(OpCodes.Add); //e2x + 1

				//on le duplique puis on lui soustrait 2 pour récupérer e2x - 1
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 2d);
				cc.il.Emit(OpCodes.Sub);

				//maintenant on les divise
				cc.il.Emit(OpCodes.Div);

				//double e2x = Math.Exp(x);
				//e2x *= e2x;
				//rep.TheResult = (e2x + 1d) / (e2x - 1d);
			}
			else if (this.ActualFuncStr == "arsinh")
			{
				//on met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//on le duplique pour faire l'intérieur de la racine carrée
				cc.il.Emit(OpCodes.Dup);
				//on le duplique pour le mettre au carrée (^2)
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Mul);
				//lui ajoute 1
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);
				//lui applique la racine carrée
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt"));
				//lui additionne x
				cc.il.Emit(OpCodes.Add);
				//lui applique le log
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));


				//rep.TheResult = Math.Log(x + Math.Sqrt((x * x) + 1d));
			}
			else if (this.ActualFuncStr == "arcosh")
			{
				//met le code qui le calcul
				this.equ.CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();

				//si x est plus petit que 1, il faut signaler une erreur
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Blt, lblError);

				////calcul
				//on le duplique pour faire l'intérieur de la racine carrée
				cc.il.Emit(OpCodes.Dup);
				//on le duplique pour le mettre au carrée (^2)
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Mul);
				//lui soustrait 1
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Sub);
				//lui applique la racine carrée
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt"));
				//lui additionne x
				cc.il.Emit(OpCodes.Add);
				//lui applique le log
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));



				cc.il.Emit(OpCodes.Br, lblEnd);
				////error
				cc.il.MarkLabel(lblError);
				//on pop la valeur de l'équation du stack
				cc.il.Emit(OpCodes.Pop);
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on injecte 0d dans le stack

				//on fait le message d'erreur
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arcosh");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));


				//fin
				cc.il.MarkLabel(lblEnd);

				//if (x < 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = Math.Log(x + Math.Sqrt((x * x) - 1d));
			}
			else if (this.ActualFuncStr == "artanh")
			{
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();

				////make sure que x > -1 && x < 1
				cc.il.Emit(OpCodes.Dup); //on duplique parce qu'il sera consommé
				cc.il.Emit(OpCodes.Ldc_R8, -1d); //on met -1 sur le stack
				cc.il.Emit(OpCodes.Ble, lblError); //goto err si x <= -1

				cc.il.Emit(OpCodes.Dup); //on duplique parce qu'il sera consommé
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //on met 1 sur le stack
				cc.il.Emit(OpCodes.Bge, lblError); //goto err si x >= 1

				////calcul
				//on lui ajoute 1 pour faire 1 + x
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);
				//on le duplique
				cc.il.Emit(OpCodes.Dup);
				//on lui soustrait 2 et on le negate pour faire 1 - x
				cc.il.Emit(OpCodes.Ldc_R8, 2d);
				cc.il.Emit(OpCodes.Sub);
				cc.il.Emit(OpCodes.Neg); //nous avon maintenant 1 - x

				//on fait la division (1 + x) / (1 - x)
				cc.il.Emit(OpCodes.Div);
				//on lui applique le logarithme
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));
				//on le divise par 2
				cc.il.Emit(OpCodes.Ldc_R8, 2d);
				cc.il.Emit(OpCodes.Div);


				cc.il.Emit(OpCodes.Br, lblEnd);
				////error
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //on pop la réponse de l'équation du stack
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0 sur le stack pour le garder stable

				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de artanh"); //on prépare le message d'erreur
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError")); //on call la fonction d'erreur
				

				cc.il.MarkLabel(lblEnd);


				//if (x <= -1d || x >= 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = Math.Log((1d + x) / (1d - x)) / 2d;
			}
			else if (this.ActualFuncStr == "arsech")
			{
				//met le code qui fait calculer l'équation à l'intérieur
				this.equ.CompileToMSIL(cc);

				Label lblError = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();

				////make sure que x > 0 && x <= 1
				cc.il.Emit(OpCodes.Dup); //on duplique parce qu'il sera consommé
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met -1 sur le stack
				cc.il.Emit(OpCodes.Ble, lblError); //goto err si x <= 0

				cc.il.Emit(OpCodes.Dup); //on duplique parce qu'il sera consommé
				cc.il.Emit(OpCodes.Ldc_R8, 1d); //on met 1 sur le stack
				cc.il.Emit(OpCodes.Bgt, lblError); //goto err si x > 1

				////calcul
				//on commence par dupliquer la valeur de l'équation pour la stocker dans une variable local
				cc.il.Emit(OpCodes.Dup);
				LocalBuilder lbTemp = cc.il.DeclareLocal(typeof(double));
				cc.il.Emit(OpCodes.Stloc, lbTemp.LocalIndex); //on stock la valeur de l'équation dans une variable local pour pouvoir y réacéder plus tard
				
				

				//on fait x^2
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Mul);
				//on le transforme en 1 - x^2
				cc.il.Emit(OpCodes.Neg);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);
				//on lui applique la racine carrée
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt"));
				//on lui additionne 1
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);

				//on le divise maintenant par x. on doit importer la sauvgarde de la valeur x (la réponse de l'équation à l'intérieur de this)
				cc.il.Emit(OpCodes.Ldloc, lbTemp.LocalIndex);
				//on le divise
				cc.il.Emit(OpCodes.Div);
				//on lui applique le logarithme
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));



				cc.il.Emit(OpCodes.Br, lblEnd);
				////error
				cc.il.MarkLabel(lblError);
				cc.il.Emit(OpCodes.Pop); //on pop la réponse de l'équation du stack
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0 sur le stack pour le garder stable

				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arsech"); //on prépare le message d'erreur
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError")); //on call la fonction d'erreur


				cc.il.MarkLabel(lblEnd);



				//if (x <= 0d || x > 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = Math.Log((1d + Math.Sqrt(1d - (x * x))) / x);
			}
			else if (this.ActualFuncStr == "arcsch")
			{
				//on met 1 sur le stack
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//on met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				//on fait calculer 1/x
				cc.il.Emit(OpCodes.Div);

				//on le duplique pour fait tout le block de la racine carrée
				cc.il.Emit(OpCodes.Dup);

				//on calcule 1/x^2
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Mul);
				//on additionne 1
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);
				//on applique la racine carrée
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt"));

				//on l'additionne avec 1/x
				cc.il.Emit(OpCodes.Add);
				
				//on lui applique le logarithme
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));

				
				//rep.TheResult = Math.Log((1d / x) + Math.Sqrt((1d / (x * x)) + 1d));
			}
			else if (this.ActualFuncStr == "arcoth")
			{
				//met le code qui fait calculer l'équation
				this.equ.CompileToMSIL(cc);

				Label lblCode = cc.il.DefineLabel();
				Label lblEnd = cc.il.DefineLabel();

				//make sure que la valeur de x est dans les bound. si la valeur de x est dans les bound, on goto au calcul de la fonction
				//check si x < -1. si true, on goto to code
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, -1d);
				cc.il.Emit(OpCodes.Blt, lblCode);
				//check si x > 1. si true, on goto to code
				cc.il.Emit(OpCodes.Dup);
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Bgt, lblCode);

				////error
				cc.il.Emit(OpCodes.Pop); //on pop la réponse de l'équation du stack
				cc.il.Emit(OpCodes.Ldc_R8, 0d); //on met 0 sur le stack

				//on envoit sur le stack le message d'erreur
				cc.il.Emit(OpCodes.Ldstr, "valeur à l'extérieur du domaine de arcoth");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError")); //on call la fonction d'erreur



				cc.il.Emit(OpCodes.Br, lblEnd);
				////calcul
				cc.il.MarkLabel(lblCode);

				//on additionne 1 à x
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				cc.il.Emit(OpCodes.Add);
				//on le duplique
				cc.il.Emit(OpCodes.Dup);
				//on lui soustrait 2 pour avoir x - 1
				cc.il.Emit(OpCodes.Ldc_R8, 2d);
				cc.il.Emit(OpCodes.Sub);

				//fait (x+1) / (x-1)
				cc.il.Emit(OpCodes.Div);

				//on lui applique le logarithme
				cc.il.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));

				//on le divise par 2
				cc.il.Emit(OpCodes.Ldc_R8, 2d);
				cc.il.Emit(OpCodes.Div);


				//fin
				cc.il.MarkLabel(lblEnd);


				//if (x >= -1d && x <= 1d)
				//{
				//	rep.AnErrorOccurred = true;
				//	rep.ActualError = sEquationResult.ErrorType.OutOfDomain;
				//	return rep;
				//}
				//rep.TheResult = Math.Log((x + 1d) / (x - 1d)) / 2d;
			}
			else
			{
				//on met qqc pour ne pas déstabiliser le stack
				cc.il.Emit(OpCodes.Ldc_R8, 0d);
			}




		}


	}
}
