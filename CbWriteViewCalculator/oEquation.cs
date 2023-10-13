using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public sealed class oEquation
    {

        //une equation correctement definit contiendra toujour un nombre impair d'element et devrons toujour commencer par une fonction puis ensuite alterner operateur puis fonction puis operateur puis ...
        //si f=function et o=operator alors une equation correctement definit contiendra un nombre impair d'element dispose comme ceci : f, fof, fofof, fofofof, et ainsi de suite.
        //cependant, l'utilisateur n'est pas forcer de suivre cette condition durant la formation de l'equation. 
        public List<oEquationObject> ListEquationObject = new List<oEquationObject>();
		



        private int zzzObjectSpace = 1; // 1 espacement entre 2 object


		// /!\ /!\ /!\ il est plutot important que toute les classe substituant oEquationObject N'OUBLIE PAS de definir les 4 variable ci-dessous pour leur sous-equations + ...
		//       ... + car elle sont plutot importante pour les fonction : .GetEquationObjectAtUiPos(,) qui sont definit dans oEquation et oEquationObject
		//position graphique de this lors du dernier refresh
		public int uiLastTop = 0;
		public int uiLastLeft = 0;
		public int uiLastWidth = 24; //valeur par default de merde
		public int uiLastHeight = 32;

		public int GetWidth()
        {
            int TotalWidth = 0;

            if (this.ListEquationObject.Count == 0)
            {
				TotalWidth = 24; //CbWriteViewCalculator.Properties.Resources.imgEmptyEquation.Width;
            }
            else
            {
                foreach (oEquationObject eo in this.ListEquationObject)
                {
                    TotalWidth += eo.GetWidth(); //largeur de l'object
                    TotalWidth += this.zzzObjectSpace; //espace entre 2 object
                }
            }

            return TotalWidth;
        }
        public int GetHeight()
        {
			//int MaxHeight = 0;
			//if (this.ListEquationObject.Count == 0)
			//{
			//	MaxHeight = 32; //CbWriteViewCalculator.Properties.Resources.imgEmptyEquation.Height;
			//}
			//else
			//{
			//	foreach (oEquationObject eo in this.ListEquationObject)
			//	{
			//		int eoHeight = eo.GetHeight();
			//		if (eoHeight > MaxHeight)
			//		{
			//			MaxHeight = eoHeight;
			//		}
			//	}
			//}
			//return MaxHeight;
			return this.GetHeightAndUpHeight().X;
		}

		//x: Height
		//y: UpHeight
		public Point GetHeightAndUpHeight()
		{
			Point rep = new Point(32, 16);

			if (this.ListEquationObject.Count > 0)
			{
				//commence par calculer le EquationLevel/UpHeight
				int MaxUpHeight = 0;
				foreach (oEquationObject eo in this.ListEquationObject)
				{
					int eoUpH = eo.GetUpHeight();
					if (eoUpH > MaxUpHeight)
					{
						MaxUpHeight = eoUpH;
					}
				}

				int MaxHeight = 0;
				foreach (oEquationObject eo in this.ListEquationObject)
				{
					int eoH = eo.GetHeight();
					int eoUpH = eo.GetUpHeight(eoH);
					int TotalHeight = MaxUpHeight + (eoH - eoUpH);
					if (TotalHeight > MaxHeight) { MaxHeight = TotalHeight; }
				}

				rep.X = MaxHeight;
				rep.Y = MaxUpHeight;
			}

			return rep;
		}



		public Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			Point pHUpH = this.GetHeightAndUpHeight();
			int MaxHeight = pHUpH.X;
			int EquLevel = pHUpH.Y;

			Bitmap img = new Bitmap(this.GetWidth(), MaxHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(Color.White);

			if (this.ListEquationObject.Count > 0)
			{

				//int HalfMaxHeight = MaxHeight / 2;
				int ActualLeft = 0;

				foreach (oEquationObject eo in this.ListEquationObject)
				{
					int eoWidth = eo.GetWidth();
					int eoHeight = eo.GetHeight();
					int eoUpHeight = eo.GetUpHeight(eoHeight);

					int eoTop = 0;
					//if (TheDrawContext.CenterObjectsAtVertical)
					//{
					//	eoTop = HalfMaxHeight - (eoHeight / 2); //pour que les object soit centré verticalement
					//}
					//else
					//{
					//	eoTop = MaxHeight - eoHeight; //pour que les object soient dessiner le plus bas possible, sur le "plancher" de l'equation
					//}
					eoTop = EquLevel - eoUpHeight;


					eo.uiLastTop = eoTop;
					eo.uiLastLeft = ActualLeft;
					eo.uiLastWidth = eoWidth;
					eo.uiLastHeight = eoHeight;
					g.DrawImage(eo.GetImage(TheDrawContext), ActualLeft, eoTop);

					ActualLeft += eoWidth; //largeur de l'object
					ActualLeft += this.zzzObjectSpace; //espacement entre les object
				}


			}
			else //s'il n'y a rien, c'est que l'equation est vide donc il faut renvoyer l'equation vide
			{
				//pas besoin d'avoir peur pour la taille de l'image car this.GetWidth() et this.GetHeight(), lorsqu'il n'y a rien dans l'equation, vont retourner les taille de imgEmptyEquation
				g.DrawImage(cWriteViewAssets.GetImgEmptyEquation(), 0, 0);
			}

			g.Dispose();
			return img;
		}


		public Bitmap GetImage2(oEquationDrawContext DrawC)
		{
			DrawC.ClearComputedSizes();
			////la première étape est de lancer le précalcul récursif de la taille de tout les composant de l'équation. les résultat sont sauvgardé dans le draw context


			Bitmap img = new Bitmap(10, 10);
			Graphics g = Graphics.FromImage(img);



			g.Dispose();
			return img;
		}

		//WHU = width height upheight.
		//execute le précalcul récursif de la taille de tout les composant de l'équation
		public SizeWHU RecursivePrecomputeWHU(oEquationDrawContext DrawC)
		{
			//on fait calculer et garde de côté la taille de tout les equ object à l'intérieur de this
			List<SizeWHU> listSize = new List<SizeWHU>();
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				listSize.Add(eo.RecursivePrecomputeWHU(DrawC));
			}





			return new SizeWHU(10, 10, 5);
		}





		//les coordonner donner en parametre sont des coordonner relative à this
		//retourne l'object qui est toucher (défini en tant que object "principale") et l'autre object est son parent
		public sEquationObjectAndEquation GetEquationObjectAtUiPos(int uiX, int uiY)
		{
			//check d'abord si le point est à l'interieur de this
			//puisque les coordonner uiX uiY sont en coordonner relative à this, le point doit etre dans le rectangle (0, 0), (thiswidth, thisheight)
			if (!(uiX > 0 && uiY > 0 && uiX <= this.uiLastWidth && uiY <= this.uiLastHeight))
			{
				return new sEquationObjectAndEquation(null, null, sEquationObjectAndEquation.enumMainObject.none); //l'autre constructeur met none par default mais juste pour etre sûr
			}


			sEquationObjectAndEquation Rep = new sEquationObjectAndEquation(null, this, sEquationObjectAndEquation.enumMainObject.Equation);

			Rectangle recPos = new Rectangle(uiX, uiY, 1, 1);

			bool IsAnyEquationObjectTouched = false;
			oEquationObject TouchedObject = null;
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				//cree un rectangle qui represente graphiquement l'object
				Rectangle eoRec = new Rectangle(eo.uiLastLeft, eo.uiLastTop, eo.uiLastWidth, eo.uiLastHeight);

				//check si le point donner est dans la hitbox de l'object
				if (eoRec.IntersectsWith(recPos))
				{
					IsAnyEquationObjectTouched = true;
					TouchedObject = eo;
					break;
				}
				
			}

			//le code qui suit pourait etre dans la if de la foreach
			if (IsAnyEquationObjectTouched)
			{
				//cree les coordonner relative
				int LocalX = uiX - TouchedObject.uiLastLeft;
				int LocalY = uiY - TouchedObject.uiLastTop;

				//obtien la reponse de l'object
				sEquationObjectAndEquation SubRep = TouchedObject.GetEquationObjectAtUiPos(LocalX, LocalY);
				Rep = SubRep;
				//si l'object pincipale est l'oEquationObject qui a été touché (TouchedObject), alors il défini l'equation sur this
				if (Rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
				{
					if (Rep.TheEquObj == TouchedObject)
					{
						Rep.TheEqu = this; //defini le parent de l'object
					}
				}

				
			}



			return Rep;
		}
		
		//retourne le coin superieure gauche
		//retourne (-1, -1) lorsque l'élément n'etait pas à l'interieure
		public Point GetUiPosOfEquationObject(oEquationObject TheEquObj)
		{
			Point Rep = new Point(-1, -1);

			bool ObjectFounded = false;
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				if (eo == TheEquObj)
				{
					Rep.X = eo.uiLastLeft;
					Rep.Y = eo.uiLastTop;
					ObjectFounded = true;
					break;
				}
				else
				{
					Point SubRep = eo.GetUiPosOfEquationObject(TheEquObj);
					if (SubRep.X != -1 && SubRep.Y != -1)
					{
						Rep = SubRep;
						ObjectFounded = true;
						break;
					}
				}
			}

			if (ObjectFounded)
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
			//check rapidement si l'element n'est pas situé immédiatement à l'interieure de lui
			foreach (oEquationObject subeo in this.ListEquationObject)
			{
				if (subeo == eo) { return 0; }
			}

			//check récursivement
			foreach (oEquationObject subeo in this.ListEquationObject)
			{
				int subrep = subeo.GetDepthOfEquationObject(eo);
				if (subrep != -1) { return subrep; }
			}
			
			return -1;
		}

		//retourne l'equation parente d'un oEquationObject
		//retourne null si l'object n'est pas a l'interieure
		public oEquation GetParentOfEquationObject(oEquationObject eo)
		{
			//check rapidement si l'oEquationObject n'est pas dans this
			foreach (oEquationObject subeo in this.ListEquationObject)
			{
				if (subeo == eo) { return this; }
			}

			//check dans les enfant
			foreach (oEquationObject subeo in this.ListEquationObject)
			{
				oEquation subrep = subeo.GetParentOfEquationObject(eo);
				if (subrep != null) { return subrep; }
			}

			return null;
		}

		//retourne l'object parent d'une equation et le parent de cette object
		//retourne null si l'equation n'est pas trouvé
		public oEquationObject GetParentOfEquation(oEquation e)
		{
			foreach (oEquationObject subeo in this.ListEquationObject)
			{
				oEquationObject subrep = subeo.GetParentOfEquation(e);
				if (subrep != null) { return subrep; }
			}
			return null;
		}
		


		//retourne si l'objet est "compliqué" à calculer. retourne true seulement s'il y a au moins une intégrale, sommation ou produit en série à l'intérieur
		public bool IsComplique(oEquationCalculContext ecc)
		{
			//on vérifie toute les enfant pour voir s'il en a au moins un qui est "compliqué" à calculer
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				if (eo.IsComplique(ecc)) { return true; }
			}
			return false;
		}



		//calcule l'equation. il faut specifier sont contexte de calcul

		public sEquationResult GetResult(oEquationCalculContext TheCalculContext)
        {
            sEquationResult rep = new sEquationResult(0d);

			if (this.ListEquationObject.Count > 0)
			{

				List<oEquationObject> manip = new List<oEquationObject>();
				//en meme temps de tout copier, il fait calculer les valeur des fonction
				foreach (oEquationObject eo in this.ListEquationObject)
				{
					if (eo.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
					{
						EQoOperator newopp = new EQoOperator(((EQoOperator)(eo)).ActualOperatorType);
						manip.Add(newopp);
					}
					if (eo.ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
					{
						sEquationResult subrep = eo.GetResult(TheCalculContext);
						if (!subrep.AnErrorOccurred)
						{
							double doublerep = subrep.TheResult;
							EQoNumber newnum = new EQoNumber(doublerep);
							manip.Add(newnum);
						}
						else //en cas d'erreur
						{
							return subrep;
						}
					}
				}

				if (manip.Count == 1)
				{
					rep.TheResult = ((EQoNumber)(manip[0])).ActualValue;
					return rep;
				}
				if (manip.Count > 1)
				{

					//remplace tout les block de plusieurs function (NON-SEPARÉ par des operateur), par le produit de ces function
					int ActualIndex = 0;
					while (ActualIndex <= (manip.Count - 1) - 1)
					{
						oEquationObject ActualItem = manip[ActualIndex];
						oEquationObject NextItem = manip[ActualIndex + 1];

						if (ActualItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
						{
							if (NextItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
							{
								EQoNumber ActualNum = (EQoNumber)ActualItem;
								EQoNumber NextNum = (EQoNumber)NextItem;
								double newval = ActualNum.ActualValue * NextNum.ActualValue;
								//met le resulta de la multiplication
								ActualNum.ActualValue = newval;
								manip.RemoveAt(ActualIndex + 1);
							}
							else
							{
								ActualIndex++;
							}
						}
						else
						{
							ActualIndex++;
						}
					}

					//effectue les opperation de multiplication et de division
					ActualIndex = 1;
					while (ActualIndex <= (manip.Count - 1) - 1)
					{
						oEquationObject LastItem = manip[ActualIndex - 1];
						oEquationObject ActualItem = manip[ActualIndex];
						oEquationObject NextItem = manip[ActualIndex + 1];

						if (ActualItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
						{

							if (LastItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function && NextItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
							{

								EQoNumber lastnum = (EQoNumber)LastItem;
								EQoNumber nextnum = (EQoNumber)NextItem;
								double newrep = 0;
								bool cancompute = false;
								if (ActualItem.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mMultiplication)
								{
									newrep = lastnum.ActualValue * nextnum.ActualValue;
									cancompute = true;
								}
								if (ActualItem.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mDivision)
								{
									newrep = lastnum.ActualValue / nextnum.ActualValue;
									cancompute = true;
								}

								//fin du calcul de l'opperation
								if (cancompute)
								{
									lastnum.ActualValue = newrep;
									//supprimme l'operateur et l'autre nombre
									manip.RemoveAt(ActualIndex);
									manip.RemoveAt(ActualIndex);
								}
								else
								{
									ActualIndex++;
								}
							}
							else
							{
								ActualIndex++;
							}

						}
						else
						{
							ActualIndex++;
						}
					}



					//effectue les opperation d'addition et soustraction
					ActualIndex = 1;
					while (ActualIndex <= (manip.Count - 1) - 1)
					{
						oEquationObject LastItem = manip[ActualIndex - 1];
						oEquationObject ActualItem = manip[ActualIndex];
						oEquationObject NextItem = manip[ActualIndex + 1];

						if (ActualItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
						{
							////normalement, c'est toujours vrai
							//if (LastItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function && NextItem.ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
							//{
							//}

							EQoNumber lastnum = (EQoNumber)LastItem;
							EQoNumber nextnum = (EQoNumber)NextItem;
							double newrep = 0;
							if (ActualItem.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mAddition)
							{
								newrep = lastnum.ActualValue + nextnum.ActualValue;
							}
							if (ActualItem.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mSubstraction)
							{
								newrep = lastnum.ActualValue - nextnum.ActualValue;
							}
							lastnum.ActualValue = newrep;
							//supprimme l'operateur et l'autre nombre
							manip.RemoveAt(ActualIndex);
							manip.RemoveAt(ActualIndex);

						}
						else
						{
							ActualIndex++;
						}
					}






					//retourne la reponse
					double doublerep = ((EQoNumber)(manip[0])).ActualValue;
					rep.AnErrorOccurred = false;
					rep.TheResult = doublerep;


				}


			}
			else //si l'equation est vide, il retourne 0
			{
				rep.AnErrorOccurred = true;
				rep.ActualError = sEquationResult.ErrorType.EmptyEquation;
			}

            return rep;
        }




        //determine si l'equation est valide pour effectuer un calcul
        public bool IsOk(bool CheckChildren = true)
        {
			if (this.ListEquationObject.Count >= 2)
			{
				bool rep = true;
				// /!\ /!\ /!\ il est faux de dire que tout les cas d'equation valide ont un nombre impaire d'element. y = 1000x en est un exemple

				////// A) il faut s'assurer que l'equation ne commence et ne termine pas par un opperateur
				////// B) il faut s'assurer qu'il n'y a jamais 2 EQoNumber consécutif
				////// C) il faut s'assurer qu'il n'y a jamais 2 EQoOperator consécutif


				int TotalCount = this.ListEquationObject.Count;

				// A)
				bool IsStartOk = this.ListEquationObject[0].ActualEquationObjectType == oEquationObject.EquationObjectType.Function;
				bool IsEndOk = this.ListEquationObject[TotalCount - 1].ActualEquationObjectType == oEquationObject.EquationObjectType.Function;

				if (IsStartOk && IsEndOk)
				{
					// B)  et  C)
					
					oEquationObject.EquationObjectType LastEquationObjectType = oEquationObject.EquationObjectType.none;
					oEquationObject.SpecificObjectType LastSpecificObjectType = oEquationObject.SpecificObjectType.none;
					
					int ActualIndex = 0;
					while (ActualIndex <= TotalCount - 1)
					{
						oEquationObject ActualItem = this.ListEquationObject[ActualIndex];
						oEquationObject.EquationObjectType ActualEquationObjectType = ActualItem.ActualEquationObjectType;
						oEquationObject.SpecificObjectType ActualSpecificObjectType = ActualItem.ActualSpecificObjectType;
						
						//ceci est éxécuté à la fin de la void
						//if (CheckChildren)
						//{
						//	bool subrep = ActualItem.IsOk(true);
						//	if (!subrep) { return false; }
						//}

						if (ActualIndex >= 1)
						{
							// B)
							if (LastSpecificObjectType == oEquationObject.SpecificObjectType.fNumber && ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber)
							{
								rep = false;
								return false;
							}

							// C)
							if (LastEquationObjectType == oEquationObject.EquationObjectType.Operator && ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
							{
								rep = false;
								return false;
							}


						}

						//next iteration
						LastEquationObjectType = ActualItem.ActualEquationObjectType;
						LastSpecificObjectType = ActualItem.ActualSpecificObjectType;
						ActualIndex++;
					}
					

				}
				else
				{
					rep = false; //just to be sure
					return false;
				}

				//si rep égale encore true
				if (rep == true && CheckChildren)
				{
					foreach (oEquationObject subeo in this.ListEquationObject)
					{
						bool subrep = subeo.IsOk(true);
						if (!subrep) { return false; }
					}
				}
				
				return rep;
			}
			else if (this.ListEquationObject.Count == 1)
			{
				if (this.ListEquationObject[0].ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
				{
					bool subrep = this.ListEquationObject[0].IsOk(CheckChildren);
					if (!subrep) { return false; }
					return true;
				}
				else { return false; }
			}
			//else if (this.ListEquationObject.Count == 2)
			//{
			//	if (this.ListEquationObject[0].ActualEquationObjectType == oEquationObject.EquationObjectType.Function && this.ListEquationObject[1].ActualEquationObjectType == oEquationObject.EquationObjectType.Function)
			//	{
			//		if (!(this.ListEquationObject[0].ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber && this.ListEquationObject[1].ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber))
			//		{
			//			return true;
			//		}
			//		else { return false; }
			//	}
			//	else { return false; }
			//}
			else if (this.ListEquationObject.Count == 0)
			{
				return false;
			}
			else
			{
				return false;
			}
        }


		

        //void new()
        public oEquation()
        {


        }


		public oEquation GetCopy()
		{
			oEquation copy = new oEquation();
			//copy et ajoute les élément
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				copy.ListEquationObject.Add(eo.GetCopy());
			}
			return copy;
		}

		#region SAVE
		public string GetSaveString()
		{
			return this.GetSaveString(false);
		}
		public string GetSaveString(bool AddBrackets = false)
		{
			string rep = "";

			//ajoute tout les enfant
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				string eosave = eo.GetSaveString();
				rep += eosave;
			}
			//rep += "\\1#";

			//////end
			if (AddBrackets) { rep = "{" + rep + "}"; }
			return rep;
		}
		public void SaveToFile(string FilePath)
		{
			System.IO.File.WriteAllLines(FilePath, new string[] { this.GetSaveString() });
		}


		//extrait une équation qui est entre crochet { }.   le caractère { doit être le premier à être lu
		public static string ExtractEquation(wvStringReader sr)
		{
			string first = sr.Read();
			if (first == "{")
			{
				string rep = "";
				int depth = 1;
				while (depth > 0)
				{
					string c = sr.Read();
					if (c == "{")
					{
						depth++;
						rep += "{";
					}
					else if (c == "}")
					{
						depth--;
						if (depth <= 0) { break; }
						rep += "}";
					}
					else
					{
						rep += c;
					}
				}

				return rep;
			}
			else
			{
				return "";
			}
		}

		public static oEquation LoadFromSaveString(string savestring)
		{
			oEquation rep = new oEquation();
			wvStringReader sr = new wvStringReader(savestring);

			while (!sr.EndOfString)
			{
				string bibi = sr.Read();
				if (bibi == "\\")
				{
					////obtien le nom
					int namelength = Convert.ToInt32(sr.Read(1));
					string name = sr.Read(namelength);

					////lit l'object

					if (name == "+") { rep.ListEquationObject.Add(new EQoOperator(EQoOperator.OperatorType.Addition)); }
					if (name == "-") { rep.ListEquationObject.Add(new EQoOperator(EQoOperator.OperatorType.Substraction)); }
					if (name == "mmul") { rep.ListEquationObject.Add(new EQoOperator(EQoOperator.OperatorType.Multiplication)); }
					if (name == "mdiv") { rep.ListEquationObject.Add(new EQoOperator(EQoOperator.OperatorType.Division)); }

					if (name == "n")
					{
						string strval = sr.ReadUntil(";");
						rep.ListEquationObject.Add(new EQoNumber(strval));
					}
					if (name == "var")
					{
						string varname = sr.ReadUntil(";");
						rep.ListEquationObject.Add(new EQoVariable(varname));
					}

					//parenthèse
					if (name == "subequ")
					{
						string strequ = oEquation.ExtractEquation(sr);
						oEquation equin = oEquation.LoadFromSaveString(strequ);
						rep.ListEquationObject.Add(new EQoParenthese(equin));
					}
					//fonction absolu
					if (name == "abs")
					{
						string strequ = oEquation.ExtractEquation(sr);
						oEquation equin = oEquation.LoadFromSaveString(strequ);
						rep.ListEquationObject.Add(new EQoAbsolute(equin));
					}
					//factoriel
					if (name == "fact!")
					{
						string strequ = oEquation.ExtractEquation(sr);
						oEquation equin = oEquation.LoadFromSaveString(strequ);
						rep.ListEquationObject.Add(new EQoFactorial(equin));
					}

					//pow
					if (name == "pow")
					{
						oEquation equBase = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equPow = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						rep.ListEquationObject.Add(new EQoExponent(equBase, equPow));
					}
					//racine carré
					if (name == "sqrt")
					{
						EQoSquareRoot news = new EQoSquareRoot(oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr)));
						rep.ListEquationObject.Add(news);
					}
					//root n
					if (name == "rootn")
					{
						oEquation equBase = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equRoot = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoRootN newroot = new EQoRootN(equBase, equRoot);
						rep.ListEquationObject.Add(newroot);
					}
					//fraction
					if (name == "frac")
					{
						oEquation equUp = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equDown = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						rep.ListEquationObject.Add(new EQoFraction(equUp, equDown));
					}



					if (name == "integral")
					{
						string varname = sr.ReadUntil(";");
						string strSubDivWidth = sr.ReadUntil(";");
						oEquation equUp = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equDown = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equMain = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoIntegral newi = new EQoIntegral(equMain, equUp, equDown);
						newi.VarName = varname;
						newi.MaxSubdivisionWidth = Convert.ToDouble(strSubDivWidth);
						rep.ListEquationObject.Add(newi);
					}
					if (name == "piproduct")
					{
						string varname = sr.ReadUntil(";");
						oEquation equUp = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equDown = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equMain = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoPiProduct newpp = new EQoPiProduct(equMain, equUp, equDown);
						newpp.VarName = varname;
						rep.ListEquationObject.Add(newpp);
					}
					if (name == "sigmasum")
					{
						string varname = sr.ReadUntil(";");
						oEquation equUp = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equDown = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equMain = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoSigmaSummation newpp = new EQoSigmaSummation(equMain, equUp, equDown);
						newpp.VarName = varname;
						rep.ListEquationObject.Add(newpp);
					}
					if (name == "binomcoef")
					{
						oEquation equUp = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						oEquation equDown = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoBinominalCoef newbc = new EQoBinominalCoef(equUp, equDown);
						rep.ListEquationObject.Add(newbc);
					}


					if (name == "misc")
					{
						string funcname = sr.ReadUntil(";");
						int paramcount = Convert.ToInt32(sr.ReadUntil(";"));
						List<oEquation> allequ = new List<oEquation>();
						for (int i = 1; i <= paramcount; i++)
						{
							allequ.Add(oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr)));
						}
						EQoMiscFunction newms = new EQoMiscFunction(funcname, allequ);
						rep.ListEquationObject.Add(newms);
					}
					if (name == "trig")
					{
						string funcname = sr.ReadUntil(";");
						oEquation equMain = oEquation.LoadFromSaveString(oEquation.ExtractEquation(sr));
						EQoTrigFunction newt = new EQoTrigFunction(funcname, equMain);
						rep.ListEquationObject.Add(newt);
					}
					if (name == "userfunc")
					{
						string funcname = sr.ReadUntil(";");
						int paramcount = Convert.ToInt32(sr.ReadUntil(";"));
						List<oEquation> allequ = new List<oEquation>();
						for (int i = 1; i <= paramcount; i++)
						{
							string strequ = oEquation.ExtractEquation(sr);
							oEquation eq = oEquation.LoadFromSaveString(strequ);
							allequ.Add(eq);
						}
						EQoUserFunction newf = new EQoUserFunction(funcname, allequ);
						rep.ListEquationObject.Add(newf);
					}






				}
			}


			return rep;
		}
		public static oEquation LoadFromFile(string FilePath)
		{
			string[] allline = System.IO.File.ReadAllLines(FilePath);
			return oEquation.LoadFromSaveString(allline[0]);
		}
		#endregion






		//il assume que le contenu de l'équation this a une syntaxe correcte. il faut vérifier this.IsOk avant de caller cette void.
		//il compile automatiquement l'équaiton this dans du code MSIL.
		public void CompileToMSIL(CompileContext cc)
		{
			//this.CompileToMSIL_v1(cc);
			this.CompileToMSIL_v2(cc);
		}
		
		private void CompileToMSIL_v1(CompileContext cc)
		{

			//liste de toute les EquationObject formant le terme actuel.
			//cette list contient, pour chaque fonction présente dans le terme, si la fonction est à multiplier au terme ou à diviser au terme. true=multiplier. false=diviser.
			List<KeyValuePair<oEquationObject, bool>> listMembers = new List<KeyValuePair<oEquationObject, bool>>();
			bool IsPositive = true; //indique si le terme actuel est positif. c'est indiqué au début du terme par l'opération + ou - et c'est utilisé à la fin seulement parce que msil est le language basé sur un stack d'évaluation.
			bool IsInverse = false; //pendant la lecture du terme actuel, indique, entre × ÷, si le dernier oppérateur était une division. true=division


			//on injecte 0d dans le stack parce que c'est le protocole que j'ai créée. cette position dans le stack est la valeur actuel de l'équation this. tout les terme vont y additionner leur valeur et cette position dans le stack sera laissé à usage quelconque à l'objet qui contient l'équation this.
			cc.il.Emit(OpCodes.Ldc_R8, 0d);

			//on commence à parcourir toute les élément de l'équation et on lit tout les terme.
			foreach (oEquationObject eo in this.ListEquationObject)
			{
				//check si nous somme tombé sur un oppérateur
				if (eo.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
				{
					//check si c'est un changement de terme. c'est le cas si on a + -
					if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mAddition || eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mSubstraction)
					{
						////le terme actuel est terminé et on va passer au suivant. il faut inscrire le code permettant de calculer le terme qu'on vient de finir.
						//juste pour être sûr, on check qu'il y a des terme
						if (listMembers.Count > 0)
						{
							//on ajoute 1d sur le stack. on y multiple et divise tout les membre du terme et à la fin c'est la valeur du terme
							cc.il.Emit(OpCodes.Ldc_R8, 1d);

							//on parcour tout les membre du terme pour ajouter leur code un par un.
							foreach (KeyValuePair<oEquationObject, bool> Pair in listMembers)
							{
								//on lui fait ajouter son code pour le calculer
								Pair.Key.CompileToMSIL(cc);

								//celon qu'il est à multipliquer ou à diviser au terme, il va le multiplier ou le diviser à
								if (!Pair.Value)
								{
									cc.il.Emit(OpCodes.Mul);
								}
								else
								{
									cc.il.Emit(OpCodes.Div);
								}

							}

							//on additionne le terme à la somme totale de l'équation
							if (IsPositive)
							{
								cc.il.Emit(OpCodes.Add);
							}
							else
							{
								cc.il.Emit(OpCodes.Sub);
							}

						}
						
						//maintenant on vide la liste des membre du terme
						while (listMembers.Count > 0)
						{
							listMembers.RemoveAt(0);
						}
						
						////nous avons terminé d'ajouter le code de calcul du terme.

						//maintenant on indique si le terme suivant est à additionner ou à soustraire
						IsPositive = eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mAddition;

						//on réinitialise puisqu'on change de terme
						IsInverse = false;

						
					}
					else
					{
						////nous sommes sur un × ÷

						//on prend en note si le/les prochain membre du terme sont à multiplier ou à diviser à
						IsInverse = eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mDivision;
						
					}
				}
				else //nous avons un membre du terme
				{
					//on ajoute le terme au dictionnaire des terme
					listMembers.Add(new KeyValuePair<oEquationObject, bool>(eo, IsInverse));
					
				}
			}

			////il n'y a pas d'oppérateur +- à la fin de l'équation qui provoquerait l'execution du code qui fait emit les ligne de code du terme donc on répète tout ici, une dernière fois.
			//on ajoute 1d sur le stack. on y multiple et divise tout les membre du terme et à la fin c'est la valeur du terme
			cc.il.Emit(OpCodes.Ldc_R8, 1d);
			//on parcour tout les membre du terme pour ajouter leur code un par un.
			foreach (KeyValuePair<oEquationObject, bool> Pair in listMembers)
			{
				//on lui fait ajouter son code pour le calculer
				Pair.Key.CompileToMSIL(cc);
				//celon qu'il est à multipliquer ou à diviser au terme, il va le multiplier au le diviser à
				if (!Pair.Value)
				{
					cc.il.Emit(OpCodes.Mul);
				}
				else
				{
					cc.il.Emit(OpCodes.Div);
				}
			}
			//on additionne le terme à la somme totale de l'équation
			if (IsPositive)
			{
				cc.il.Emit(OpCodes.Add);
			}
			else
			{
				cc.il.Emit(OpCodes.Sub);
			}
			

		}

		private void CompileToMSIL_v2(CompileContext cc)
		{
			bool Optimized = false; //devient true s'il ne faut pas utiliser la méthode général à la fin
			if (this.ListEquationObject.Count == 1)
			{
				Optimized = true;
				this.ListEquationObject[0].CompileToMSIL(cc);
			}

			if (!Optimized)
			{
				//liste de toute les EquationObject formant le terme actuel.
				//cette list contient, pour chaque fonction présente dans le terme, si la fonction est à multiplier au terme ou à diviser au terme. true=multiplier. false=diviser.
				List<KeyValuePair<oEquationObject, bool>> listMembers = new List<KeyValuePair<oEquationObject, bool>>();
				bool IsPositive = true; //indique si le terme actuel est positif. c'est indiqué au début du terme par l'opération + ou - et c'est utilisé à la fin seulement parce que msil est le language basé sur un stack d'évaluation.
				bool IsInverse = false; //pendant la lecture du terme actuel, indique, entre × ÷, si le dernier oppérateur était une division. true=division


				//on injecte 0d dans le stack parce que c'est le protocole que j'ai créée. cette position dans le stack est la valeur actuel de l'équation this. tout les terme vont y additionner leur valeur et cette position dans le stack sera laissé à usage quelconque à l'objet qui contient l'équation this.
				cc.il.Emit(OpCodes.Ldc_R8, 0d);

				//on commence à parcourir toute les élément de l'équation et on lit tout les terme.
				foreach (oEquationObject eo in this.ListEquationObject)
				{
					//check si nous somme tombé sur un oppérateur
					if (eo.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
					{
						//check si c'est un changement de terme. c'est le cas si on a + -
						if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mAddition || eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mSubstraction)
						{
							////le terme actuel est terminé et on va passer au suivant. il faut inscrire le code permettant de calculer le terme qu'on vient de finir.
							//juste pour être sûr, on check qu'il y a des terme
							if (listMembers.Count > 0)
							{
								//on ajoute 1d sur le stack. on y multiple et divise tout les membre du terme et à la fin c'est la valeur du terme
								cc.il.Emit(OpCodes.Ldc_R8, 1d);

								//on parcour tout les membre du terme pour ajouter leur code un par un.
								foreach (KeyValuePair<oEquationObject, bool> Pair in listMembers)
								{
									//on lui fait ajouter son code pour le calculer
									Pair.Key.CompileToMSIL(cc);

									//celon qu'il est à multipliquer ou à diviser au terme, il va le multiplier ou le diviser à
									if (!Pair.Value)
									{
										cc.il.Emit(OpCodes.Mul);
									}
									else
									{
										cc.il.Emit(OpCodes.Div);
									}

								}

								//on additionne le terme à la somme totale de l'équation
								if (IsPositive)
								{
									cc.il.Emit(OpCodes.Add);
								}
								else
								{
									cc.il.Emit(OpCodes.Sub);
								}

							}

							//maintenant on vide la liste des membre du terme
							while (listMembers.Count > 0)
							{
								listMembers.RemoveAt(0);
							}

							////nous avons terminé d'ajouter le code de calcul du terme.

							//maintenant on indique si le terme suivant est à additionner ou à soustraire
							IsPositive = eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mAddition;

							//on réinitialise puisqu'on change de terme
							IsInverse = false;


						}
						else
						{
							////nous sommes sur un × ÷

							//on prend en note si le/les prochain membre du terme sont à multiplier ou à diviser à
							IsInverse = eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.mDivision;

						}
					}
					else //nous avons un membre du terme
					{
						//on ajoute le terme au dictionnaire des terme
						listMembers.Add(new KeyValuePair<oEquationObject, bool>(eo, IsInverse));

					}
				}

				////il n'y a pas d'oppérateur +- à la fin de l'équation qui provoquerait l'execution du code qui fait emit les ligne de code du terme donc on répète tout ici, une dernière fois.
				//on ajoute 1d sur le stack. on y multiple et divise tout les membre du terme et à la fin c'est la valeur du terme
				cc.il.Emit(OpCodes.Ldc_R8, 1d);
				//on parcour tout les membre du terme pour ajouter leur code un par un.
				foreach (KeyValuePair<oEquationObject, bool> Pair in listMembers)
				{
					//on lui fait ajouter son code pour le calculer
					Pair.Key.CompileToMSIL(cc);
					//celon qu'il est à multipliquer ou à diviser au terme, il va le multiplier au le diviser à
					if (!Pair.Value)
					{
						cc.il.Emit(OpCodes.Mul);
					}
					else
					{
						cc.il.Emit(OpCodes.Div);
					}
				}
				//on additionne le terme à la somme totale de l'équation
				if (IsPositive)
				{
					cc.il.Emit(OpCodes.Add);
				}
				else
				{
					cc.il.Emit(OpCodes.Sub);
				}
			}

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
