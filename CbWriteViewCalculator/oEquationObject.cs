using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{



    //c'est cet object ci qui est la classe de base des object dont le nom est de la forme "EQo***.cs"

    public abstract class oEquationObject
    {


        public List<oEquation> ListEquation = new List<oEquation>();


        
        public enum EquationObjectType
        {
            none,
            Function, // ce sont les nombre, les fraction, fonction sinus, racine carrer, parenthese, les variable et autre truc du genre
            Operator // les operation de base addition, soustraction, multiplication, division et RIEN D'AUTRE
        }
        protected EquationObjectType zzzActualEquationObjectType = EquationObjectType.none;
        public EquationObjectType ActualEquationObjectType { get { return this.zzzActualEquationObjectType; } }

        //permet de determiner avec plus de specification de quel type d'object il s'agit
        public enum SpecificObjectType
        {
            none,

            fNumber,
            fParenthese,
            fFraction,
			fSquareRoot,
			fRootN, //racine n-ième
            fExponent,

            mAddition,
            mSubstraction,
            mMultiplication,
            mDivision,

            fVariable,
			fUserFonction,  //fonction qui a été créer et défini par l'utilisateur
            fTrig,  //fonction trigonométrique
			fMiscFunction,
			fAbsolute, //fonction absolut  |x|
			fFactorial,
			fBinominalCoef,  //coefficient binominaux
			
			fSigmaSummation,  //sommation répété
			fPiProduct,  //produit répété
			fIntegral

        }
        protected SpecificObjectType zzzActualSpecificObjectType = SpecificObjectType.none;
        public SpecificObjectType ActualSpecificObjectType { get { return this.zzzActualSpecificObjectType; } }
		
		
		//position graphique de this lors du dernier refresh
		public int uiLastTop = 0;
		public int uiLastLeft = 0;
		public int uiLastWidth = 24;
		public int uiLastHeight = 32;

		public Color BackColor = Color.White;

        public virtual int GetWidth() { return 24; }
        public virtual int GetHeight() { return 32; }

		//la position verticale d'un object est défini celon l'altitude imaginaire, le EquationLevel. chaque object a une position verticale, défini par UpHeight, qui indique à quel point il est supérieure à cette ligne.
		//en toute logique, il n'est pas possible que UpHeight > Height
		public virtual int GetUpHeight() { return this.GetUpHeight(this.GetHeight()); }
		public virtual int GetUpHeight(int MyHeight) { return MyHeight / 2; }

        public virtual Point GetPosOfEquationByIndex(int TheEquationIndex) { return new Point(-1, -1); } //permet d'optenir la position du bord superieure gauche d'une equation en fonction de son index dans la list des equation. /!\ /!\ IMPORTANT : la position retourner est en coordoner relative de this. lorsque l'index n'existe pas, la reponse qui doit etre retourner est (-1, -1)
        public virtual Bitmap GetImage(oEquationDrawContext TheDrawContext) { return new Bitmap(this.GetWidth(), this.GetHeight()); }
        public virtual sEquationResult GetResult(oEquationCalculContext TheCalculContext) { return new sEquationResult(0d); }
        public virtual bool IsOk(bool CheckChildren = true)
		{
			foreach (oEquation eq in this.ListEquation)
			{
				bool subrep = eq.IsOk(CheckChildren);
				if (!subrep) { return false; }
			}
			return true;
		}



		//execute le précalcul récursif de la taille de l'object.
		//le précalcul n'a une utilité que si les equ object override cette fonction pour l'optimiser.
		public virtual SizeWHU RecursivePrecomputeWHU(oEquationDrawContext DrawC)
		{
			foreach (oEquation equ in this.ListEquation)
			{
				equ.RecursivePrecomputeWHU(DrawC);
			}
			int w = this.GetWidth();
			int h = this.GetHeight();
			int uh = this.GetUpHeight();
			return DrawC.AddComputedSize(this, w, h, uh);
		}






		//les coordonner donner en parametre sont des coordonner relative à this
		//retourne l'object qui est toucher (défini en tant que object "principale") et l'autre object est sont parent
		public virtual sEquationObjectAndEquation GetEquationObjectAtUiPos(int uiX, int uiY)
		{
			//check d'abord si le point est à l'interieur de this
			//puisque les coordonner uiX uiY sont en coordonner relative à this, le point doit etre dans le rectangle (0, 0), (thiswidth, thisheight)
			if (!(uiX > 0 && uiY > 0 && uiX <= this.uiLastWidth && uiY <= this.uiLastHeight))
			{
				return new sEquationObjectAndEquation(null, null, sEquationObjectAndEquation.enumMainObject.none); //l'autre constructeur met none par default mais juste pour etre sûr
			}


			sEquationObjectAndEquation Rep = new sEquationObjectAndEquation(this, null, sEquationObjectAndEquation.enumMainObject.EquationObject);

			Rectangle recPos = new Rectangle(uiX, uiY, 1, 1);

			bool IsAnyEquationTouched = false;
			oEquation TouchedEquation = null;
			foreach (oEquation eq in this.ListEquation)
			{
				//cree un rectangle qui represente graphiquement l'equation
				Rectangle eqRec = new Rectangle(eq.uiLastLeft, eq.uiLastTop, eq.uiLastWidth, eq.uiLastHeight);

				//check si le point est dans le rectangle
				if (eqRec.IntersectsWith(recPos))
				{
					IsAnyEquationTouched = true;
					TouchedEquation = eq;
					break;
				}
			}

			//le code qui suit pourait etre dans la if de la foreach
			if (IsAnyEquationTouched)
			{
				//cree les coordonner relative
				int LocalX = uiX - TouchedEquation.uiLastLeft;
				int LocalY = uiY - TouchedEquation.uiLastTop;

				sEquationObjectAndEquation SubRep = TouchedEquation.GetEquationObjectAtUiPos(LocalX, LocalY);
				Rep = SubRep;

				//si l'object principale est l'equation et que cette equation est celle dont il est question ici (TouchedEquation), il défini l'autre object sur this car this est sont parent
				if (Rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
				{
					if (Rep.TheEqu == TouchedEquation)
					{
						Rep.TheEquObj = this;
					}
				}


			}



			return Rep;
		}
		
		//retourne le coin superieure gauche
		//retourne (-1, -1) lorsque l'étément n'etait pas à l'interieure
		public Point GetUiPosOfEquationObject(oEquationObject TheEquObj)
		{
			if (TheEquObj == this) { return new Point(this.uiLastLeft, this.uiLastTop); }
			Point Rep = new Point(-1, -1);

			bool ObjectFound = false;
			foreach (oEquation eq in this.ListEquation)
			{
				Point SubRep = eq.GetUiPosOfEquationObject(TheEquObj);
				if (SubRep.X != -1 && SubRep.Y != -1)
				{
					Rep = SubRep;
					ObjectFound = true;
					break;
				}
			}

			if (ObjectFound)
			{
				Rep.X += this.uiLastLeft;
				Rep.Y += this.uiLastTop;
			}
			else
			{
				Rep = new Point(-1, -1);
			}

			return Rep;
		}
		
		//0 correspond à un object situé immédiatement à l'interieure
		//retourne -1 si l'object n'est pas trouvé à l'interieure
		public int GetDepthOfEquationObject(oEquationObject eo)
		{
			if (eo == this) { return 0; } //dans ce cas précis, il doit retourner 0 car this ne se situe pas à l'intérieure de this, mais à l'intérieure de l'équation qui a appelé la void

			foreach (oEquation eq in this.ListEquation)
			{
				int subrep = eq.GetDepthOfEquationObject(eo);
				if (subrep != -1) { return subrep + 1; }
			}
			
			return -1; //if (rep < 0) { return -1; } else { return rep + 1; }   // -1 +1 = 0 donc il faut faire une exeption
		}
		
		//retourne l'equation parente d'un oEquationObject
		public oEquation GetParentOfEquationObject(oEquationObject eo)
		{
			foreach (oEquation subeq in this.ListEquation)
			{
				oEquation subrep = subeq.GetParentOfEquationObject(eo);
				if (subrep != null) { return subrep; }
			}

			return null;
		}

		//retourne l'object parent d'une equation et le parent de cette object
		//retourne null si l'equation n'est pas trouvé
		public oEquationObject GetParentOfEquation(oEquation e)
		{
			//check si l'equation n'est pas à l'interieure de this
			foreach (oEquation sube in this.ListEquation)
			{
				if (sube == e) { return this; }
			}
			//check si l'equation n'est pas à l'interieure des equation contenue dans this
			foreach (oEquation sube in this.ListEquation)
			{
				oEquationObject subrep = sube.GetParentOfEquation(e);
				if (subrep != null) { return subrep; }
			}
			return null;
		}


		//retourne si l'objet est "compliqué" à calculer. retourne true seulement s'il y a au moins une intégrale, sommation ou produit en série à l'intérieur
		public virtual bool IsComplique(oEquationCalculContext ecc)
		{
			if (this.ActualSpecificObjectType == SpecificObjectType.fIntegral || this.ActualSpecificObjectType == SpecificObjectType.fSigmaSummation || this.ActualSpecificObjectType == SpecificObjectType.fPiProduct)
			{
				return true;
			}
			else
			{
				//il faut vérifier toute les équation enfant pour vérifier s'il y a l'un des objet "compliqué" à calculer
				foreach (oEquation eq in this.ListEquation)
				{
					if (eq.IsComplique(ecc)) { return true; }
				}
				return false;
			}
		}








		public virtual string GetReadableName() { return this.ToString(); }

		public virtual oEquationObject GetCopy() { return new EQoVariable("cannotcopy"); }

		#region SAVE
		public virtual string GetSaveString() { return "\\4null"; }

		#endregion





		public virtual void CompileToMSIL(CompileContext cc)
		{
			cc.il.Emit(OpCodes.Ldc_R8, 0d);
			//cc.il.Emit(OpCodes.Ldc_R4, 0d); //"le clr a détecté un programme non valide"
		}







		private int zzzHashInt1 = (int)(cWriteViewAssets.RND.NextDouble() * 10000000d);
		public int HashInt1 { get { return this.zzzHashInt1; } }
		//private int zzzHashInt2 = (int)(cWriteViewAssets.RND.NextDouble() * 10000000d);
		//public int HashInt2 { get { return this.zzzHashInt2; } }
		public override int GetHashCode()
		{
			return this.zzzHashInt1;
		}


	}
}
