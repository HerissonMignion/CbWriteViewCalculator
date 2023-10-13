using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoPiProduct : oEquationObject
	{
		private int zzzSigmaWidth = 50; // 40
		private int zzzSigmaHeight = 50; // 60
		private int zzzParentheseWidth = 10; //largeur des parenthèse. aussi l'espacement à droite





		private oEquation equMain;
		private oEquation equUp;
		private oEquation equDown;


		public bool DoesItNeedParenthese
		{
			get
			{
				bool rep = false;

				if (this.equMain.ListEquationObject.Count > 1) { return true; }
				if (this.equMain.ListEquationObject.Count == 1)
				{
					oEquationObject.SpecificObjectType thetype = this.equMain.ListEquationObject[0].ActualSpecificObjectType;
					if (thetype == SpecificObjectType.fSigmaSummation) { return true; }
					if (thetype == SpecificObjectType.fPiProduct) { return true; }
				}

				return rep;
			}
		}


		public string VarName = "k";
		public Font VarFont = new Font("consolas", 17f); // new Font("consolas", 10f);



		public override int GetWidth()
		{
			int equupWidth = this.equUp.GetWidth();
			int equdownWidth = this.equDown.GetWidth();
			int equmainWidth = this.equMain.GetWidth();

			int TotalWidth = 0;

			//left
			int leftWidth = this.zzzSigmaWidth;
			if (equupWidth > leftWidth) { leftWidth = equupWidth; }
			string downString = this.VarName + "=";
			int downWidth = cWriteViewAssets.GetTextSize(downString, this.VarFont).Width + equdownWidth;
			if (downWidth > leftWidth) { leftWidth = downWidth; }
			TotalWidth += leftWidth;

			//middle
			if (this.DoesItNeedParenthese)
			{
				TotalWidth += this.zzzParentheseWidth;
			}

			//right
			TotalWidth += equmainWidth;

			TotalWidth += this.zzzParentheseWidth;

			return TotalWidth;
		}
		public override int GetHeight()
		{
			int equupHeight = this.equUp.GetHeight();
			int equdownHeight = this.equDown.GetHeight();
			Point equmainHUpH = this.equMain.GetHeightAndUpHeight();
			int equmainHeight = equmainHUpH.X;
			int equmainUpHeight = equmainHUpH.Y;

			int leftHeight = equupHeight + this.zzzSigmaHeight + equdownHeight;
			int leftUpHeight = equupHeight + (this.zzzSigmaHeight / 2); //leftHeight / 2;

			int rightHeight = equmainHeight;
			int rightUpHeight = equmainUpHeight;


			////calcule la réponse
			int rep = 0;
			//up
			if (rightUpHeight > leftUpHeight)
			{
				rep += rightUpHeight;
			}
			else { rep += leftUpHeight; }

			//down
			int leftDownHeight = leftHeight - leftUpHeight;
			int rightDownHeight = rightHeight - rightUpHeight;
			if (rightDownHeight > leftDownHeight)
			{
				rep += rightDownHeight;
			}
			else { rep += leftDownHeight; }


			return rep;
		}

		public override int GetUpHeight()
		{
			return this.GetUpHeight(100); //un nombre quelconque
		}
		public override int GetUpHeight(int MyHeight)
		{
			int equupHeight = this.equUp.GetHeight();
			//int equdownHeight = this.equDown.GetHeight();
			Point equmainHUpH = this.equMain.GetHeightAndUpHeight();
			int equmainHeight = equmainHUpH.X;
			int equmainUpHeight = equmainHUpH.Y;

			//int leftHeight = equupHeight + this.zzzSigmaHeight + equdownHeight;
			int leftUpHeight = equupHeight + (this.zzzSigmaHeight / 2); //leftHeight / 2;

			int rightHeight = equmainHeight;
			int rightUpHeight = equmainUpHeight;


			////calcule la réponse
			int rep = 0;
			if (rightUpHeight > leftUpHeight)
			{
				rep = rightUpHeight;
			}
			else { rep = leftUpHeight; }

			return rep;
		}

		public override Bitmap GetImage(oEquationDrawContext TheDrawContext)
		{
			int imgWidth = this.GetWidth();
			int imgHeight = this.GetHeight();
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.BackColor);

			Bitmap imgUp = this.equUp.GetImage(TheDrawContext);
			Bitmap imgDown = this.equDown.GetImage(TheDrawContext);
			Bitmap imgMain = this.equMain.GetImage(TheDrawContext);

			//// width
			int equupWidth = imgUp.Width;
			int equdownWidth = imgDown.Width;
			int equmainWidth = imgMain.Width;
			int leftWidth = this.zzzSigmaWidth;
			if (equupWidth > leftWidth) { leftWidth = equupWidth; }
			string downString = this.VarName + "=";
			Size downStringSize = cWriteViewAssets.GetTextSize(downString, this.VarFont);
			int downWidth = downStringSize.Width + equdownWidth;
			if (downWidth > leftWidth) { leftWidth = downWidth; }


			//// height
			int equupHeight = imgUp.Height;
			int equdownHeight = imgDown.Height;
			int equmainHeight = imgMain.Height;
			Point equmainHUpH = this.equMain.GetHeightAndUpHeight();
			int equmainUpHeight = equmainHUpH.Y;

			int leftHeight = equupHeight + this.zzzSigmaHeight + equdownHeight;
			int leftUpHeight = equupHeight + (this.zzzSigmaHeight / 2); //leftHeight / 2;

			int rightHeight = equmainHeight;
			int rightUpHeight = equmainUpHeight;


			//calcule la position verticale du symbole pi
			int sigmaTop = 0;

			if (rightUpHeight > leftUpHeight)
			{
				sigmaTop = rightUpHeight - leftUpHeight + equupHeight;
			}
			else
			{
				sigmaTop = equupHeight;
			}




			////dessine le symbole pi
			int sigmaLeft = (leftWidth / 2) - (this.zzzSigmaWidth / 2);
			Pen sigmaPen = new Pen(Color.Black, 2f);
			//g.DrawRectangle(Pens.Black, sigmaLeft, sigmaTop, this.zzzSigmaWidth - 1, this.zzzSigmaHeight - 1);
			int bardist = 12;
			g.DrawLine(sigmaPen, sigmaLeft + 2, sigmaTop + 4, sigmaLeft + this.zzzSigmaWidth - 2, sigmaTop + 4); //dessus
			g.DrawLine(sigmaPen, sigmaLeft + bardist, sigmaTop + 4, sigmaLeft + bardist, sigmaTop + this.zzzSigmaHeight - 2); //gauche
			g.DrawLine(sigmaPen, sigmaLeft + this.zzzSigmaWidth - bardist, sigmaTop + 4, sigmaLeft + this.zzzSigmaWidth - bardist, sigmaTop + this.zzzSigmaHeight - 2); //droite



			//// dessine le nom de la variable et l'équation du bas
			g.DrawString(downString, this.VarFont, Brushes.Black, (float)(sigmaLeft + (this.zzzSigmaWidth / 2) - (downWidth / 2)), (float)(sigmaTop + this.zzzSigmaHeight + (imgDown.Height / 2) - (downStringSize.Height / 2)));
			int equdownLeft = sigmaLeft + (this.zzzSigmaWidth / 2) - (downWidth / 2) + downStringSize.Width;
			int equdownTop = sigmaTop + this.zzzSigmaHeight;
			g.DrawImage(imgDown, equdownLeft, equdownTop);
			this.equDown.uiLastTop = equdownTop;
			this.equDown.uiLastLeft = equdownLeft;
			this.equDown.uiLastWidth = imgDown.Width;
			this.equDown.uiLastHeight = imgDown.Height;


			//// dessine l'équation du dessus
			int equupTop = sigmaTop - equupHeight;
			int equupLeft = sigmaLeft + (this.zzzSigmaWidth / 2) - (equupWidth / 2);
			g.DrawImage(imgUp, equupLeft, equupTop);
			this.equUp.uiLastTop = equupTop;
			this.equUp.uiLastLeft = equupLeft;
			this.equUp.uiLastWidth = equupWidth;
			this.equUp.uiLastHeight = equupHeight;





			//// dessine l'équation de droite
			int equmainLeft = leftWidth;
			if (this.DoesItNeedParenthese) { equmainLeft += this.zzzParentheseWidth; }
			int equmainTop = equupTop + leftUpHeight - rightUpHeight;
			// dessine les parenthese
			if (this.DoesItNeedParenthese)
			{
				Bitmap pleft = cWriteViewAssets.GetImgParentheseLeft(equmainHeight, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pleft, equmainLeft - this.zzzParentheseWidth, equmainTop);
				pleft.Dispose();
				Bitmap pright = cWriteViewAssets.GetImgParentheseRight(equmainHeight, this.zzzParentheseWidth, this.BackColor);
				g.DrawImage(pright, equmainLeft + equmainWidth, equmainTop);
				pright.Dispose();
			}
			g.DrawImage(imgMain, equmainLeft, equmainTop);
			this.equMain.uiLastTop = equmainTop;
			this.equMain.uiLastLeft = equmainLeft;
			this.equMain.uiLastWidth = imgMain.Width;
			this.equMain.uiLastHeight = imgMain.Height;




			imgUp.Dispose();
			imgDown.Dispose();
			imgMain.Dispose();
			g.Dispose();
			return img;
		}


		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rEndVal = this.equUp.GetResult(TheCalculContext);
			if (rEndVal.AnErrorOccurred) { return rEndVal; }
			sEquationResult rStartVar = this.equDown.GetResult(TheCalculContext);
			if (rStartVar.AnErrorOccurred) { return rStartVar; }

			//crée le layer de variable
			oEquationCalculContext.FuncVariableLayer Layer = new oEquationCalculContext.FuncVariableLayer();
			Variable TheVar = new Variable(this.VarName, 0d); // 0d  cette valeur de départ n'est pas censé être importante
			Layer.listAllPrivateVar.Add(TheVar);
			//copie toute les variable de la layer précédante
			if (TheCalculContext.AllFuncLayer.Count > 0)
			{
				oEquationCalculContext.FuncVariableLayer LastLayer = TheCalculContext.AllFuncLayer[TheCalculContext.AllFuncLayer.Count - 1];
				foreach (Variable v in LastLayer.listAllPrivateVar)
				{
					//copie la variable seulement si elle n'as pas le meme nom que la variable de this
					if (v.Name != this.VarName)
					{
						Layer.listAllPrivateVar.Add(v.GetCopy());
					}
				}
			}

			TheCalculContext.AllFuncLayer.Add(Layer); //ajoute sa couche de variable

			////// CALCUL :


			double StartValue = cWriteViewAssets.RoundDown(rStartVar.TheResult + 0.5d); //arondi la réponse à l'unité près
			int TotalLoopCount = (int)cWriteViewAssets.RoundDown(rEndVal.TheResult + 0.5d) - (int)(StartValue) + 1;
			if (TotalLoopCount <= 0)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = 1d;

				//retire son layer de variable
				TheCalculContext.AllFuncLayer.Remove(Layer);
				return rep;
			}


			double ActualProduct = 1d; //valeur actuel de la sommation
			double ActualValue = StartValue; //valeur actuel de la variable
			int ActualLoop = 1; //itération actuel
			bool AnErrorOccurred = false;
			while (ActualLoop <= TotalLoopCount)
			{
				//défini la valeur actuel de la variable
				TheVar.Value = ActualValue;

				//calcul l'équation
				sEquationResult SubRep = this.equMain.GetResult(TheCalculContext);
				if (SubRep.AnErrorOccurred)
				{
					AnErrorOccurred = true;
					rep.AnErrorOccurred = true;
					rep.ActualError = SubRep.ActualError;
					break;
				}
				else
				{
					ActualProduct *= SubRep.TheResult;
				}

				//next iteration
				ActualLoop++;
				ActualValue += 1d; //augmente de 1 la valeur de la variable
			}

			//après le calcul, il retire son layer
			TheCalculContext.AllFuncLayer.Remove(Layer);

			//si aucune erreur ne s'est produite, il défini la réponse. s'il y a eu une erreur, la variable rep contien l'erreur et il ne faut pas y toucher
			if (!AnErrorOccurred)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = ActualProduct;
			}

			return rep;
		}

		
		//void new()
		public EQoPiProduct()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fPiProduct;


			this.equMain = new oEquation();
			this.equUp = new oEquation();
			this.equDown = new oEquation();
			this.ListEquation.Add(this.equMain);
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);



		}
		public EQoPiProduct(oEquation MainEq, oEquation UpEq, oEquation DownEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fPiProduct;

			this.equMain = MainEq;
			this.equUp = UpEq;
			this.equDown = DownEq;
			this.ListEquation.Add(this.equMain);
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);
		}


		public override string GetReadableName()
		{
			return "Produit";
		}


		public override oEquationObject GetCopy()
		{
			EQoPiProduct copy = new EQoPiProduct(this.equMain.GetCopy(), this.equUp.GetCopy(), this.equDown.GetCopy());
			copy.VarName = this.VarName;
			return copy;
		}

		public override string GetSaveString()
		{
			string strqUp = this.equUp.GetSaveString(true);
			string strqDown = this.equDown.GetSaveString(true);
			string strqMain = this.equMain.GetSaveString(true);
			return "\\9piproduct" + this.VarName + ";" + strqUp + strqDown + strqMain;
		}




		public override void CompileToMSIL(CompileContext cc)
		{

			// /!\ /!\ /!\ copié collé de SigmaSummation. modifié pour qu'il effectue des multiplication


			//la création de la layer doit être effectué après le calcul de la valeur de départ et de fin

			//double StartValue = cWriteViewAssets.RoundDown(rStartVar.TheResult + 0.5d); //arondi la réponse à l'unité près
			//int TotalLoopCount = (int)cWriteViewAssets.RoundDown(rEndVal.TheResult + 0.5d) - (int)(StartValue) + 1;


			//////on calcul la borne de fin
			//on fait calculer la valeur de fin
			this.equUp.CompileToMSIL(cc);
			//on met o.5d sur le stack
			cc.il.Emit(OpCodes.Ldc_R8, 0.5d);
			//on les additionne
			cc.il.Emit(OpCodes.Add);
			//on call round down. c'est un truc spécial qui immite réelement la fonction escalier
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("RoundDown"));
			//on converti en int
			cc.il.Emit(OpCodes.Conv_I4);



			//////on calcul la borne de départ
			//on crée le jeton pour la variable contenant la borne de départ
			LocalBuilder lbStartVal = cc.il.DeclareLocal(typeof(double));
			//on fait calculer la valeur de départ
			this.equDown.CompileToMSIL(cc);
			//on met 0.5d sur le stack
			cc.il.Emit(OpCodes.Ldc_R8, 0.5d);
			//on les additionne
			cc.il.Emit(OpCodes.Add);
			//on call round down. c'est un truc spécial qui immite réelement la fonction escalier
			cc.il.Emit(OpCodes.Call, typeof(cWriteViewAssets).GetMethod("RoundDown"));
			cc.il.Emit(OpCodes.Dup);
			//stock la start value dans sa variable
			cc.il.Emit(OpCodes.Stloc, lbStartVal);

			////maintenant on calcul totalloopcount, qui est upbound - downbound + 1
			//on converti la start value en int
			cc.il.Emit(OpCodes.Conv_I4);
			cc.il.Emit(OpCodes.Sub);
			cc.il.Emit(OpCodes.Ldc_I4, 1);
			cc.il.Emit(OpCodes.Add);

			////désormais, totalloopcount est sur le stack et il est stocké sur le stack. on a terminé la sommation lorsque cette valeur est arrivé à 0



			//maintenant que les bornes de départ et de fin ont été calculé
			//crée la nouvelle layer, en se basant sur la layer actuel, et en y ajoutant sa variable
			CompileContext.VariableLayer layer = cc.CreateNewLayerFromLast(this.VarName);
			//crée le jeton msil pour la variable de la sommation
			LocalBuilder varlb = cc.il.DeclareLocal(typeof(double));
			//ajoute la variable dans la layer
			layer.dictPrivateVar.Add(this.VarName, varlb);




			//oncrée la variable qui contient la somme de tout les terme
			LocalBuilder lbSum = cc.il.DeclareLocal(typeof(double));
			//on met la variable de la série à 1
			cc.il.Emit(OpCodes.Ldc_R8, 1d);
			cc.il.Emit(OpCodes.Stloc, lbSum);


			//on crée 2 label : un sert à revenir au début de la boucle, l'autre sert a quitter la boucle
			Label lblBoucle = cc.il.DefineLabel();
			Label lblEnd = cc.il.DefineLabel();



			//on donne à la variable de la sommation sa valeur de départ
			cc.il.Emit(OpCodes.Ldloc, lbStartVal); //load la valeur de départ
			cc.il.Emit(OpCodes.Stloc, varlb); //donne la valeur à la variable de la sommation




			//////// DÉBUT DE LA BOUCLE ////////
			cc.il.MarkLabel(lblBoucle);

			////on vérifie si on a terminé toute les itération qui étaient à faire
			//on duplique la valeur du nombre d'itération restante parce qu'elle sera popé du stack par le branchement conditionnel
			cc.il.Emit(OpCodes.Dup);
			cc.il.Emit(OpCodes.Ldc_I4, 0); //on met 0 dans le stack
			cc.il.Emit(OpCodes.Ble, lblEnd); //goto à end si totalloopcount <= 0


			////calcul du terme
			//on insert le code qui fait calculer l'équation de la sommation
			this.equMain.CompileToMSIL(cc);
			//on importe au stack la somme actuel, on la multiplie avec le terme actuel puis on renvoit ca dans la variable de la somme
			cc.il.Emit(OpCodes.Ldloc, lbSum);
			cc.il.Emit(OpCodes.Mul);
			cc.il.Emit(OpCodes.Stloc, lbSum);


			////next iteration
			//décrémente le nombre restant d'itération
			cc.il.Emit(OpCodes.Ldc_I4, 1); //met 1 dans le stack
			cc.il.Emit(OpCodes.Sub); //soustrait 1 à la position du stack indiquant le nombre restant d'itération

			//augmente de 1d la valeur de la variable de la sommation
			cc.il.Emit(OpCodes.Ldloc, varlb); //load la valeur de la variable
			cc.il.Emit(OpCodes.Ldc_R8, 1d); //met 1d sur le stack
			cc.il.Emit(OpCodes.Add); //les additionne
			cc.il.Emit(OpCodes.Stloc, varlb); //renvoit varlb + 1 dans varlb


			cc.il.Emit(OpCodes.Br, lblBoucle);
			//////// FIN DE LA BOUCLE ////////
			cc.il.MarkLabel(lblEnd);

			//on pop totalloopcount du stack
			cc.il.Emit(OpCodes.Pop);

			//on met la somme complète sur le stack
			cc.il.Emit(OpCodes.Ldloc, lbSum);




			//pop notre layer de variable, qui est une layer de jeton local buider. c'est trop facile à oublier comme différence.
			cc.PopLastLayer();



		}


	}
}
