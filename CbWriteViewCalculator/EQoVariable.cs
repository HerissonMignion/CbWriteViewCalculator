using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoVariable : oEquationObject
	{

		private string zzzName = "";
		public string Name
		{
			get { return this.zzzName; }
			set
			{
				this.zzzName = value;
				this.RefreshImage();
			}
		}
		public string UiName
		{
			get
			{
				string rep = this.Name;
				if (this.Name.Length > 1 || this.Name.Length == 0) { rep = "«" + this.Name + "»"; }
				return rep;
			}
		}

		public bool IsNameOk()
		{
			return cWriteViewAssets.IsAVariableName(this.Name);
		}



		public override int GetWidth()
		{
			return this.zzzActualImage.Width;
		}
		public override int GetHeight()
		{
			return this.zzzActualImage.Height;
		}

		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			if (this.ActualImageBackColor != this.BackColor) { this.RefreshImage(); }
			return new Bitmap(this.zzzActualImage);
		}


		private Bitmap zzzActualImage = null;
		private Color ActualImageBackColor = Color.Empty;
		private void RefreshImage()
		{
			if (this.zzzActualImage != null) { this.zzzActualImage.Dispose(); }
			string text = this.UiName;
			Font TextFont = new Font("consolas", 20f); // consolas 15f
			Size TextSize = cWriteViewAssets.GetTextSize(text, TextFont);
			int RemovedWidth = 3; // 3 nombre de pixel retiré à gauche et à droite
			int RemovedHeight = 4; // 2
			this.zzzActualImage = new Bitmap(TextSize.Width - RemovedWidth - RemovedWidth, TextSize.Height - RemovedHeight - RemovedHeight);
			Graphics g = Graphics.FromImage(this.zzzActualImage);
			g.Clear(this.BackColor);
			this.ActualImageBackColor = this.BackColor;

			g.DrawString(text, TextFont, Brushes.Black, (float)-RemovedWidth, (float)-RemovedHeight);


			g.Dispose();
		}


		//void new()
		public EQoVariable(string StartName = "noname")
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fVariable;

			this.zzzName = StartName;


			this.RefreshImage();
		}






		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(0d);

			bool nameok = this.IsNameOk();
			if (!nameok)
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.InvalidVariableName;
				return rep;
			}



			bool IsExist = TheCalculContext.IsVariableExistByName(this.Name);
			if (IsExist)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = TheCalculContext.GetVariableValueByName(this.Name);
			}
			else
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.UndefinedVariable;
			}

			return rep;
		}



		public override string GetReadableName()
		{
			return "Variable";
		}

		public override oEquationObject GetCopy()
		{
			EQoVariable copy = new EQoVariable(this.Name);
			return copy;
		}

		public override string GetSaveString()
		{
			return "\\3var" + this.Name + ";";
		}






		public override void CompileToMSIL(CompileContext cc)
		{
			//récupère le local builder de la variable que this représente
			LocalBuilder lb = cc.GetVariableAccordingToContext(this.Name);

			//si la variable n'a pas été trouvé, il aura retourné null
			if (lb != null)
			{
				//fait loader la valeur de la variable sur le stack
				cc.il.Emit(OpCodes.Ldloc, lb.LocalIndex);
			}
			else
			{
				//ceci est une erreur qui se produit plutot pendant le processus de compilation plutot que pendant le calcul lui-même.
				//ca pourait être géré d'une facon différente.


				//on met 0d sur le stack pour ne pas le déstabiliser
				cc.il.Emit(OpCodes.Ldc_R8, 0d);
				//on rapporte l'erreur
				cc.il.Emit(OpCodes.Ldstr, "variable not found");
				cc.il.Emit(OpCodes.Call, typeof(CompileContext).GetMethod("ReportError"));

			}
			
		}

	}
}
