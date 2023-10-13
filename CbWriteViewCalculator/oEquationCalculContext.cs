using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbWriteViewCalculator
{







    public sealed class Variable
    {
        public string Name = "noname";
		public string UiName
		{
			get
			{
				string rep = this.Name;
				if (this.Name.Length > 1)
				{
					rep = "«" + this.Name + "»";
				}
				return rep;
			}
		}
        public double Value = 0;
        public Variable(string StartName = "noname", double StartValue = 0)
        {
            this.Name = StartName;
            this.Value = StartValue;
        }
		public override string ToString()
		{
			return "\"" + this.Name + "\" = " + this.Value.ToString();
		}

		public Variable GetCopy()
		{
			Variable copy = new Variable(this.Name, this.Value);
			return copy;
		}
	}

	public sealed class VariableEventArgs : EventArgs
	{
		public Variable TheVariable;
		public VariableEventArgs(Variable StartVariable)
		{
			this.TheVariable = StartVariable;
		}
	}




	public sealed class oFonction
	{
		public string Name = "";

		//cette liste contien tout les paramètre de la fonction, en ordre croissant.
		public List<string> listParam = new List<string>();
		public int ParamCount { get { return this.listParam.Count; } }


		//la règle de la fonction, son équation
		public oEquation TheEqu;



		//void new()
		public oFonction(string StartName = "f", bool CreateAsEmpty = true)
		{
			this.Name = StartName;

			this.TheEqu = new oEquation();

			if (!CreateAsEmpty)
			{
				this.listParam.Add("x");
				this.TheEqu.ListEquationObject.Add(new EQoVariable("x"));
			}

		}


		public sEquationResult GetResult(double[] ParamArray, oEquationCalculContext TheCalculContext)
		{
			//crée toute les variable de paramètre
			oEquationCalculContext.FuncVariableLayer layer = new oEquationCalculContext.FuncVariableLayer();
			int index = 0;
			while (index <= this.listParam.Count - 1)
			{
				//crée la variable pour le paramètre
				string varname = this.listParam[index];
				double varvalue = ParamArray[index];
				Variable newv = new Variable(varname, varvalue);
				layer.listAllPrivateVar.Add(newv);
				//next iteration
				index++;
			}

			//ajoute le layer
			TheCalculContext.AllFuncLayer.Add(layer);


			//sEquationResult rep = this.TheEqu.GetResult(TheCalculContext);
			sEquationResult rep = new sEquationResult(-1d);
			try
			{
				rep = this.TheEqu.GetResult(TheCalculContext);
			}
			catch
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.Unknown;
			}


			//retire son layer
			TheCalculContext.RemoveLastLayer();

			return rep;
		}


		public object Tag = null;
		public bool TagDefined = false;
		public event EventHandler TagEditionWanted;
		public void Raise_TagEditionWanted()
		{
			if (this.TagEditionWanted != null)
			{
				this.TagEditionWanted(this, new EventArgs());
			}
		}


		public override string ToString()
		{
			string rep = this.Name + "(";

			if (this.listParam.Count > 0)
			{
				rep += this.listParam[0];

				int index = 1;
				while (index <= this.listParam.Count - 1)
				{
					rep += "," + this.listParam[index];
					index++;
				}
			}

			rep += ")";
			return rep;
		}

	}

	public sealed class FonctionEventArgs : EventArgs
	{
		public oFonction TheFonction;
		public FonctionEventArgs(oFonction StartFonction)
		{
			this.TheFonction = StartFonction;
		}
	}


	//============================================================================================================================================================
	//============================================================================================================================================================
	//============================================================================================================================================================

	public sealed class oEquationCalculContext
    {
		//la copie est très loin d'être parfaite. c'est une copie utilisé pour pouvoir faire du calcul en multithreading.
		//cette function crée une copie contenant le minimum stricte pour pouvoir s'en servire pour calculer et avoir les même réponse à ses calcul. il manque tout les layer précédant.
		public oEquationCalculContext GetLittleCopy()
		{
			oEquationCalculContext copy = new oEquationCalculContext();

			////copie les variable globale
			foreach (Variable v in this.AllVariable)
			{
				copy.AddVariable(v.GetCopy());
			}

			////copie les variable privé
			//il n'y a pas nécéssairement des layer, mais s'il y en a, les variable local sont celle de la dernière layer
			if (this.AllFuncLayer.Count > 0)
			{
				//crée la layer qui sera dans la copie
				FuncVariableLayer newlayer = new FuncVariableLayer();
				//copie les variable privé
				foreach (Variable v in this.AllFuncLayer[this.AllFuncLayer.Count - 1].listAllPrivateVar)
				{
					newlayer.listAllPrivateVar.Add(v.GetCopy());
				}
				//ajoute la layer à la copie
				copy.AllFuncLayer.Add(newlayer);
			}


			////copie les autre paramètre
			copy.ActualAngleType = this.ActualAngleType;

			return copy;
		}





		//--------------------------------------------------------------[ fonction ]------------------------------------------------------
		//les variable à l'intérieur d'une layer sont appellé/considéré des "variable privé/locale"
		public List<oFonction> AllFonction = new List<oFonction>();
		public List<FuncVariableLayer> AllFuncLayer = new List<FuncVariableLayer>();
		public void RemoveLastLayer()
		{
			if (this.AllFuncLayer.Count > 0)
			{
				this.AllFuncLayer.RemoveAt(this.AllFuncLayer.Count - 1);
			}
		}
		public void RemoveAllLayer()
		{
			while (this.AllFuncLayer.Count > 0d) { this.AllFuncLayer.RemoveAt(0); }
		}
		public class FuncVariableLayer
		{
			public List<Variable> listAllPrivateVar = new List<Variable>();
		}

		public bool IsFonctionExistByName(string NameToSearch)
		{
			foreach (oFonction f in this.AllFonction)
			{
				if (f.Name == NameToSearch)
				{
					return true;
				}
			}
			return false;
		}
		public oFonction GetFonctionByName(string NameToSearch)
		{
			foreach (oFonction f in this.AllFonction)
			{
				if (f.Name == NameToSearch)
				{
					return f;
				}
			}
			return null;
		}


		public void AddFonction(oFonction newfunc)
		{
			this.AllFonction.Add(newfunc);
			this.Raise_FonctionAdded(newfunc);
		}
		//retourne si la fonction a été deleté
		public bool RemoveFonction(oFonction thefunc)
		{
			int lastcount = this.AllFonction.Count;
			try
			{
				this.AllFonction.Remove(thefunc);
			}
			catch { }
			int actualcount = this.AllFonction.Count;
			if (lastcount != actualcount)
			{
				this.Raise_FonctionRemoved(thefunc);
			}
			return lastcount != actualcount;
		}


		public event EventHandler<FonctionEventArgs> FonctionAdded;
		public event EventHandler<FonctionEventArgs> FonctionRemoved;
		public event EventHandler<FonctionEventArgs> FonctionRuleChanged; // "rule" changed signifie que la règle a changé, mais peut être aussi le nom ou juste les paramètre. ca signifie en gros que qqc d'important a changé et qu'il faut revoir tout ce qui concerne ou utilise cette fonction
		private void Raise_FonctionAdded(oFonction thefunc)
		{
			if (this.FonctionAdded != null)
			{
				FonctionEventArgs e = new FonctionEventArgs(thefunc);
				this.FonctionAdded(this, e);
			}
		}
		private void Raise_FonctionRemoved(oFonction thefunc)
		{
			if (this.FonctionRemoved != null)
			{
				FonctionEventArgs e = new FonctionEventArgs(thefunc);
				this.FonctionRemoved(this, e);
			}
		}
		public void Raise_FonctionRuleChanged(oFonction thefunc)
		{
			if (this.FonctionRuleChanged != null)
			{
				this.FonctionRuleChanged(this, new FonctionEventArgs(thefunc));
			}
		}



		//--------------------------------------------------------------[ variable ]------------------------------------------------------
		public List<Variable> AllVariable = new List<Variable>(); //les variables de cette liste sont appellé/considéré les "variable globale"
		
        //indique si une variable de ce nom existe deja dans la liste de toute les variable
        public bool IsVariableExistByName(string NameToSearch)
        {
			//verifie les variable privé
			if (this.AllFuncLayer.Count > 0)
			{
				foreach (Variable v in this.AllFuncLayer[this.AllFuncLayer.Count - 1].listAllPrivateVar)
				{
					if (v.Name == NameToSearch) { return true; }
				}
			}
			
			//verifie les variable globale
			foreach (Variable v in this.AllVariable)
			{
				if (v.Name == NameToSearch) { return true; }
			}

			//s'il n'a rien trouvé :
			return false;
        }
        
        //cette structure explique comment l'operation s'est derouler
        public struct structAddVariable
        {
            public enum VariableStade
            {
                none, //rien de definit
                VariableAdded, //la variable a ete ajouter
                VariableNotAddedBecauseAlreadyExist, //la variable n'a pas ete ajouter car une autre variable de meme nom existe deja
                AnErrorOccurred //une erreur quelconque s'est produite
            }
            public VariableStade ActualVariableStade;
            
            //void new()
            public structAddVariable(VariableStade StartActualVariableStade = VariableStade.none)
            {
                this.ActualVariableStade = StartActualVariableStade;
            }
        }
        public structAddVariable AddVariable(Variable TheVariable)
        {
            structAddVariable rep = new structAddVariable();
            //commence par checker qu'une variable de meme nom n'existe pas deja
            if (!this.IsVariableExistByName(TheVariable.Name))
            {
                try
                {
                    //ajoute la variable
                    this.AllVariable.Add(TheVariable);
                    //definit que la variable a ete ajouter
                    rep.ActualVariableStade = structAddVariable.VariableStade.VariableAdded;
					this.Raise_VariableAdded(TheVariable);
                }
                catch
                {
                    rep.ActualVariableStade = structAddVariable.VariableStade.AnErrorOccurred;
                }
            }
            else
            {
                rep.ActualVariableStade = structAddVariable.VariableStade.VariableNotAddedBecauseAlreadyExist;
            }
            return rep;
        }
        public structAddVariable AddVariableByParam(string TheVariableName, double TheVariableValue)
        {
            structAddVariable rep = new structAddVariable();
            //commence par checker qu'une variable de meme nom n'existe pas deja
            if (!this.IsVariableExistByName(TheVariableName))
            {
                try
                {
                    //cree et ajoute la variable
                    Variable NewVariable = new Variable(TheVariableName, TheVariableValue);
                    this.AllVariable.Add(NewVariable);
                    //definit que la variable a ete ajouter
                    rep.ActualVariableStade = structAddVariable.VariableStade.VariableAdded;
					this.Raise_VariableAdded(NewVariable);
                }
                catch
                {
                    rep.ActualVariableStade = structAddVariable.VariableStade.AnErrorOccurred;
                }
            }
            else
            {
                rep.ActualVariableStade = structAddVariable.VariableStade.VariableNotAddedBecauseAlreadyExist;
            }
            return rep;
        }

		//retourne si la variable a ete deleter
		public bool RemoveVariable(Variable TheVariable)
		{
			bool rep = false;
			bool isexist = this.AllVariable.Contains(TheVariable);
			if (isexist)
			{
				try
				{
					this.AllVariable.Remove(TheVariable);
					rep = true;
					this.Raise_VariableRemoved(TheVariable);
				}
				catch
				{
					rep = false;
				}
			}
			else
			{
				rep = false;
			}
			return rep;
		}
		public bool RemoveVariable(string VariableName)
		{
			bool rep = false;
			int index = 0;
			while (index <= this.AllVariable.Count - 1)
			{
				Variable var = this.AllVariable[index];
				if (var.Name == VariableName)
				{
					this.AllVariable.Remove(var);
					rep = true;
					this.Raise_VariableRemoved(var);
					break;
				}
				
				//next iteration
				index++;
			}

			return rep;
		}


        //permet d'optenir un variable par son nom
        public Variable GetVariableByName(string TheVariableName)
        {
			//verifie les variable privé
			if (this.AllFuncLayer.Count > 0)
			{
				foreach (Variable v in this.AllFuncLayer[this.AllFuncLayer.Count - 1].listAllPrivateVar)
				{
					if (v.Name == TheVariableName) { return v; }
				}
			}

			//verifie les variable globale
			foreach (Variable v in this.AllVariable)
			{
				if (v.Name == TheVariableName) { return v; }
			}
			
            //if (!isVariableFinded) { throw new Exception("variable \"" + TheVariableName + "\" not found"); }
			return null;
        }
        public double GetVariableValueByName(string TheVariableName)
		{
			//verifie les variable privé
			if (this.AllFuncLayer.Count > 0)
			{
				foreach (Variable v in this.AllFuncLayer[this.AllFuncLayer.Count - 1].listAllPrivateVar)
				{
					if (v.Name == TheVariableName) { return v.Value; }
				}
			}

			//verifie les variable globale
			foreach (Variable v in this.AllVariable)
			{
				if (v.Name == TheVariableName) { return v.Value; }
			}

			return -1d;
        }




		public event EventHandler<VariableEventArgs> VariableAdded;
		public event EventHandler<VariableEventArgs> VariableRemoved;
		public event EventHandler<VariableEventArgs> VariableValueChanged; //il se pourrait que cet event ne soit pas toujours callé
		private void Raise_VariableAdded(Variable thevar)
		{
			if (this.VariableAdded != null)
			{
				VariableEventArgs e = new VariableEventArgs(thevar);
				this.VariableAdded(this, e);
			}
		}
		private void Raise_VariableRemoved(Variable thevar)
		{
			if (this.VariableRemoved != null)
			{
				VariableEventArgs e = new VariableEventArgs(thevar);
				this.VariableRemoved(this, e);
			}
		}
		public void Raise_VariableValueChanged(Variable thevar)
		{
			if (this.VariableValueChanged != null)
			{
				this.VariableValueChanged(this, new VariableEventArgs(thevar));
			}
		}

		public event EventHandler SomethingChanged;
		public void Raise_SomethingChanged(object sender)
		{
			if (this.SomethingChanged != null)
			{
				this.SomethingChanged(sender, new EventArgs());
			}
		}




        //--------------------------------------------------------------[ les type de calcul qui sont a effectuer et afficher. c'est une sorte de "personalisation" du comportement de l'ensemble ]--------------------------------------------------------------
        public enum AngleType
        {
            Degree, //  ex:32 "degrée"    tout les angle qui sont rentrer ou retourner par les fonction trigonometrique sont transformer en degree
            Radian, //  ex:32 "radian"    tout les angle qui sont rentrer ou retourner par les fonction trigonometrique sont conserver en randian
			Gradian //  ex:32 "gon"
        }
        public AngleType ActualAngleType = AngleType.Radian;

		public double angleConvertUsedToRadian(double UsedAngle)
		{
			if (this.ActualAngleType == AngleType.Radian) { return UsedAngle; }
			if (this.ActualAngleType == AngleType.Degree) { return UsedAngle / 180d * Math.PI; }
			if (this.ActualAngleType == AngleType.Gradian) { return UsedAngle / 200d * Math.PI; }
			return UsedAngle;
		}
		public double angleConvertRadianToUsed(double RandianAngle)
		{
			if (this.ActualAngleType == AngleType.Radian) { return RandianAngle; }
			if (this.ActualAngleType == AngleType.Degree) { return RandianAngle / Math.PI * 180d; }
			if (this.ActualAngleType == AngleType.Gradian) { return RandianAngle / Math.PI * 200d; }
			return RandianAngle;
		}






		

        //-----------------[ void new() & private void ]---------
        public oEquationCalculContext()
        {



        }



		


		//public void SaveToFile(string FilePath)
		//{
		//	List<string> allline = new List<string>();
		//	foreach (Variable v in this.AllVariable)
		//	{
		//		allline.Add("var");
		//		allline.Add(v.Name);
		//		allline.Add(cWriteViewAssets.TrimStrNumber(cWriteViewAssets.RemoveSpace(v.Value.ToString("N20"))));
		//	}
		//}



    }
}
