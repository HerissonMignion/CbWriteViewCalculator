using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	public class EQoIntegral : oEquationObject
	{
		private int zzzSigmaWidth = 20; // 20
		private int zzzSigmaHeight = 60; // 60
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


		public string VarName = "u";
		public Font VarFont = new Font("consolas", 17f); // new Font("consolas", 10f);

		// paramètre d'intégration :
		public double MaxSubdivisionWidth = 0.005d; // 0.005d
		public int ThreadCount = 1;


		public override int GetWidth()
		{
			int equupWidth = this.equUp.GetWidth();
			int equdownWidth = this.equDown.GetWidth();
			int equmainWidth = this.equMain.GetWidth();

			int TotalWidth = 0;

			//left
			int leftWidth = this.zzzSigmaWidth;
			if (equupWidth > leftWidth) { leftWidth = equupWidth; }
			//string downString = this.VarName + "=";
			int downWidth = equdownWidth;
			if (downWidth > leftWidth) { leftWidth = downWidth; }
			TotalWidth += leftWidth;

			//middle
			if (this.DoesItNeedParenthese)
			{
				TotalWidth += this.zzzParentheseWidth * 2;
			}

			//right
			TotalWidth += equmainWidth;
			string rightString = "d" + this.VarName;
			TotalWidth += cWriteViewAssets.GetTextSize(rightString, this.VarFont).Width;
			

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
			string rightString = "d" + this.VarName;
			Size rightStringSize = cWriteViewAssets.GetTextSize(rightString, this.VarFont);
			int downWidth = equdownWidth;
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


			//calcule la position verticale du symbole sigma
			int sigmaTop = 0;

			if (rightUpHeight > leftUpHeight)
			{
				sigmaTop = rightUpHeight - leftUpHeight + equupHeight;
			}
			else
			{
				sigmaTop = equupHeight;
			}




			////dessine le symbole de l'intégrale
			int sigmaLeft = (leftWidth / 2) - (this.zzzSigmaWidth / 2);
			Pen sigmaPen = new Pen(Color.Black, 2f);
			//g.DrawRectangle(Pens.Black, sigmaLeft, sigmaTop, this.zzzSigmaWidth - 1, this.zzzSigmaHeight - 1);
			g.DrawLine(sigmaPen, sigmaLeft + (this.zzzSigmaWidth / 2), sigmaTop, sigmaLeft + (this.zzzSigmaWidth / 2), sigmaTop + this.zzzSigmaHeight);
			g.DrawLine(sigmaPen, sigmaLeft + (this.zzzSigmaWidth / 2), sigmaTop + 1, sigmaLeft + this.zzzSigmaWidth, sigmaTop + 1);
			g.DrawLine(sigmaPen, sigmaLeft, sigmaTop + this.zzzSigmaHeight - 1, sigmaLeft + (this.zzzSigmaWidth / 2), sigmaTop + this.zzzSigmaHeight - 1);
			int petitheight = 10;
			g.DrawLine(sigmaPen, sigmaLeft + this.zzzSigmaWidth - 1, sigmaTop, sigmaLeft + this.zzzSigmaWidth - 1, sigmaTop + petitheight);
			g.DrawLine(sigmaPen, sigmaLeft + 1, sigmaTop + this.zzzSigmaHeight, sigmaLeft + 1, sigmaTop + this.zzzSigmaHeight - petitheight);


			//// dessine l'équation du bas
			//g.DrawString(downString, this.VarFont, Brushes.Black, (float)(sigmaLeft + (this.zzzSigmaWidth / 2) - (downWidth / 2)), (float)(sigmaTop + this.zzzSigmaHeight + (imgDown.Height / 2) - (downStringSize.Height / 2)));
			int equdownLeft = sigmaLeft + (this.zzzSigmaWidth / 2) - (downWidth / 2);
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
			int equmainLeft = leftWidth + 1; // int equmainLeft = leftWidth;
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

			//dessine le nom de la variable d'intégration à droite
			int rightStringLeft = equmainLeft + equmainWidth;
			if (this.DoesItNeedParenthese) { rightStringLeft += this.zzzParentheseWidth; }
			g.DrawString(rightString, this.VarFont, Brushes.Black, (float)(rightStringLeft), (float)(equupTop + leftUpHeight - (rightStringSize.Height / 2)));



			imgUp.Dispose();
			imgDown.Dispose();
			imgMain.Dispose();
			g.Dispose();
			return img;
		}


		public override sEquationResult GetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rUp = this.equUp.GetResult(TheCalculContext);
			if (rUp.AnErrorOccurred) { return rUp; }
			sEquationResult rDown = this.equDown.GetResult(TheCalculContext);
			if (rDown.AnErrorOccurred) { return rDown; }



			////bounds
			bool RevertSignAtEnd = false;

			double UpBound = rUp.TheResult;
			double DownBound = rDown.TheResult;
			if (DownBound > UpBound)
			{
				RevertSignAtEnd = true;
				UpBound = rDown.TheResult;
				DownBound = rUp.TheResult;
			}

			//si les bound sont les même, il retourne 0, parce que c'est systématiquement la réponse
			if (UpBound == DownBound)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = 0d;
				return rep;
			}



			////crée le layer de variable
			oEquationCalculContext.FuncVariableLayer Layer = new oEquationCalculContext.FuncVariableLayer();
			Variable TheVar = new Variable(this.VarName, 0d);
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

			//////// CALCUL :
			
			////calcul le nombre de division à effectuer de sorte à respecter this.MaxSubdivisionWidth
			double BoundDiff = UpBound - DownBound;
			int TotalDivision = (int)(BoundDiff / this.MaxSubdivisionWidth) + 5;
			//calcule les delta
			double deltaVar = BoundDiff / (double)TotalDivision;
			double ddd2 = deltaVar / 2;


			bool AnErrorOccurred = false;

			////calcul la première valeur, la DownBound
			TheVar.Value = DownBound;
			sEquationResult asdfDownRep = this.equMain.GetResult(TheCalculContext);
			if (asdfDownRep.AnErrorOccurred)
			{
				AnErrorOccurred = true; //la boucle while ne va pas démarer. après la boucle while, layer est retiré et rep sera retourné.
				rep = asdfDownRep;
			}


			double ActualSum = 0d; //valeur actuel de la sommation
			double ActualValue = DownBound + deltaVar; //valeur actuel de la variable
			int ActualDiv = 1; //itération actuel
			double LastY = asdfDownRep.TheResult;
			while (ActualDiv <= TotalDivision && !AnErrorOccurred)
			{
				if (ActualDiv == TotalDivision) { ActualValue = UpBound; } //juste pour ne pas que l'utilisateur ait de surprise : la upbound est la derniere valeur réel calculable

				//défini la valeur actuel de la variable
				TheVar.Value = ActualValue;
				//calcul l'équation
				sEquationResult SubRep = this.equMain.GetResult(TheCalculContext);
				if (SubRep.AnErrorOccurred)
				{
					//Program.wdebug(deltaVar);
					//Program.wdebug(TotalDivision);
					//Program.wdebug(ActualDiv);
					//Program.wdebug(ActualValue);
					AnErrorOccurred = true;
					rep.AnErrorOccurred = true;
					rep.ActualError = SubRep.ActualError;
					break;
				}
				else
				{
					ActualSum += (SubRep.TheResult + LastY); // / 2d * deltaVar;
				}

				//next iteration
				ActualDiv++;
				ActualValue += deltaVar; //augmente la valeur de la variable
				LastY = SubRep.TheResult;
			}

			//après le calcul, il retire son layer
			TheCalculContext.AllFuncLayer.Remove(Layer);

			//si aucune erreur ne s'est produite, il défini la réponse. s'il y a eu une erreur, la variable rep contien l'erreur et il ne faut pas y toucher
			if (!AnErrorOccurred)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = ActualSum * (deltaVar / 2d);
				if (RevertSignAtEnd) { rep.TheResult *= -1d; }
			}

			return rep;
		}


		#region multi-thread get result

		private sEquationResult[] multitAllResult;

		private sEquationResult multitGetResult(oEquationCalculContext TheCalculContext)
		{
			sEquationResult rep = new sEquationResult(-1d);

			sEquationResult rUp = this.equUp.GetResult(TheCalculContext);
			if (rUp.AnErrorOccurred) { return rUp; }
			sEquationResult rDown = this.equDown.GetResult(TheCalculContext);
			if (rDown.AnErrorOccurred) { return rDown; }



			////bounds
			bool RevertSignAtEnd = false;

			double UpBound = rUp.TheResult;
			double DownBound = rDown.TheResult;
			if (DownBound > UpBound)
			{
				RevertSignAtEnd = true;
				UpBound = rDown.TheResult;
				DownBound = rUp.TheResult;
			}

			//si les bound sont les même, il retourne 0, parce que c'est systématiquement la réponse
			if (UpBound == DownBound)
			{
				rep.AnErrorOccurred = false;
				rep.TheResult = 0d;
				return rep;
			}


			////crée le layer de variable
			oEquationCalculContext.FuncVariableLayer Layer = new oEquationCalculContext.FuncVariableLayer();
			Variable TheVar = new Variable(this.VarName, 0d);
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


			//////// CALCUL :

			////calcul le nombre de division à effectuer de sorte à respecter this.MaxSubdivisionWidth
			double BoundDiff = UpBound - DownBound;
			int TotalDivision = (int)(BoundDiff / this.MaxSubdivisionWidth) + 5;
			//make sure que TotalDivision est un multiple de ThreadCount
			while (TotalDivision % this.ThreadCount != 0) { TotalDivision++; }

			////calcule les delta
			//double deltaVar = BoundDiff / (double)TotalDivision;
			//double ddd2 = deltaVar / 2;


			//////////   THREADS

			this.multitAllResult = new sEquationResult[this.ThreadCount];
			oEquationCalculContext[] eccs = new oEquationCalculContext[this.ThreadCount];
			oEquation[] equs = new oEquation[this.ThreadCount];
			ParameterizedThreadStart[] ptss = new ParameterizedThreadStart[this.ThreadCount];
			Thread[] ts = new Thread[this.ThreadCount];

			for (int i = 0; i < this.ThreadCount; i++)
			{
				//initialize la réponse
				this.multitAllResult[i] = new sEquationResult(0d);

				//initialize le calcul context
				eccs[i] = TheCalculContext.GetLittleCopy();

				//initialize l'équation
				equs[i] = this.equMain.GetCopy();

				//initialize le thread start
				ptss[i] = new ParameterizedThreadStart(this.multit_AnyThreadVoid);

				//initialize le thread
				ts[i] = new Thread(ptss[i]);
				
			}

			//démarre les thread
			for (int i = 0; i < this.ThreadCount; i++)
			{
				object[] paramss = new object[10];
				paramss[0] = i;
				paramss[1] = equs[i]; //équation
				paramss[2] = eccs[i]; //equ calcul context




				ts[i].Start(paramss);
			}





			////////// END THREADS



			//après le calcul, il retire son layer
			TheCalculContext.AllFuncLayer.Remove(Layer);


			return rep;
		}

		//0=thread index
		private void multit_AnyThreadVoid(object param)
		{




		}

		#endregion


		//void new()
		public EQoIntegral()
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fIntegral;
			
			this.equMain = new oEquation();
			this.equUp = new oEquation();
			this.equDown = new oEquation();
			this.ListEquation.Add(this.equMain);
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);
			
		}
		public EQoIntegral(oEquation MainEq, oEquation UpEq, oEquation DownEq)
		{
			this.zzzActualEquationObjectType = EquationObjectType.Function;
			this.zzzActualSpecificObjectType = SpecificObjectType.fIntegral;
			
			this.equMain = MainEq;
			this.equUp = UpEq;
			this.equDown = DownEq;
			this.ListEquation.Add(this.equMain);
			this.ListEquation.Add(this.equUp);
			this.ListEquation.Add(this.equDown);
		}


		public override string GetReadableName()
		{
			return "Intégrale";
		}

		public override oEquationObject GetCopy()
		{
			EQoIntegral copy = new EQoIntegral(this.equMain.GetCopy(), this.equUp.GetCopy(), this.equDown.GetCopy());
			copy.MaxSubdivisionWidth = this.MaxSubdivisionWidth;
			copy.VarName = this.VarName;
			return copy;
		}

		public override string GetSaveString()
		{
			string strSubDivWidth = cWriteViewAssets.TrimStrNumber(cWriteViewAssets.RemoveSpace(this.MaxSubdivisionWidth.ToString("N20")));
			string strqUp = this.equUp.GetSaveString(true);
			string strqDown = this.equDown.GetSaveString(true);
			string strqMain = this.equMain.GetSaveString(true);
			return "\\8integral" + this.VarName + ";" + strSubDivWidth + ";" + strqUp + strqDown + strqMain;
		}





		public override void CompileToMSIL(CompileContext cc)
		{
			//la méthode des rectangle de cette facon est meilleur qu'avec des trapèze car les 2 se calculent vraiment de la même facon mais celle des trapèze calcule directement les bound et les bound sont parfois des singularité donc la méthode des rectangle, telle que actuellement implémenté est meilleur.
			this.CompileToMSIL_v1(cc);

			//this.CompileToMSIL_v2(cc);

		}

		//procède par la méthode des rectangle. la facon la moins compliqué.
		private void CompileToMSIL_v1(CompileContext cc)
		{
			Label lblEnd = cc.il.DefineLabel();

			//// on calcul la borne supérieur
			LocalBuilder lbBoundUp = cc.il.DeclareLocal(typeof(double));
			this.equUp.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Stloc, lbBoundUp.LocalIndex);

			//// on calcul la borne inférieur
			LocalBuilder lbBoundDown = cc.il.DeclareLocal(typeof(double));
			this.equDown.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Stloc, lbBoundDown);


			//// si les deux bound sont identique, l'intégrale vaut 0.
			Label lblNullIntegral = cc.il.DefineLabel();
			//on met sur le stack les 2 bound. si les 2 bound sont égaux, ont goto directement à la fin, à un endroit spécial qui va laisser 0 tout seul sur le stack.
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp.LocalIndex);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown.LocalIndex);
			cc.il.Emit(OpCodes.Beq, lblNullIntegral);

			//// make sure que la bound up est plus grande que la bound down
			LocalBuilder lbBoundsOk = cc.il.DeclareLocal(typeof(int)); //si 0, l'intégrale n'est pas à inverser. si 1, le signe de la réponse est à inverser.
			cc.il.Emit(OpCodes.Ldc_I4, 0);
			cc.il.Emit(OpCodes.Stloc, lbBoundsOk); //on met 0 à l'intérieur pour indiquer qu'il ne faut pas changer le signe
			Label lblBoundsOk = cc.il.DefineLabel();
			//on met les 2 bound sur le stack et si elles sont correcte, on skip la partie suivante qui les échange
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Bgt, lblBoundsOk); //si la bound up est supérieur à la bound down, on skip ce qui suit

			//si on est ici, les bounds sont à inverser
			//on charge sur le stack les valeur des bound et on les replace dans l'ordre inverse
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Stloc, lbBoundUp);
			cc.il.Emit(OpCodes.Stloc, lbBoundDown);
			//maintenant qu'on a inversé les bound il faut mettre 1 dans la variable qui indique si les bounds sont inversé
			cc.il.Emit(OpCodes.Ldc_I4, 1);
			cc.il.Emit(OpCodes.Stloc, lbBoundsOk);

			//// fin du check des bound
			cc.il.MarkLabel(lblBoundsOk);

			////// à partir d'ici les bounds sont correcte //////


			//maintenant que les bornes de départ et de fin ont été calculé
			//crée la nouvelle layer, en se basant sur la layer actuel, et en y ajoutant sa variable
			CompileContext.VariableLayer layer = cc.CreateNewLayerFromLast(this.VarName);
			//crée le jeton msil pour la variable de l'intégration
			LocalBuilder varlb = cc.il.DeclareLocal(typeof(double));
			//ajoute la variable dans la layer
			layer.dictPrivateVar.Add(this.VarName, varlb);


			//// préparation du calcul
			//on calcul la quantité de division à faire. ca dépend de l'espace qu'il y a entre les bound.
			//on charge dans le stack les 2 bound et on les soustrait pour connaitre l'espace
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Sub);

			//on duplique l'espace entre les bounds pour effectuer un autre calcul après le calcul du nombre de division
			cc.il.Emit(OpCodes.Dup);

			//maintenant on le divise par MaxSubdivWidth pour connaitre le nombre de section aproximatif
			cc.il.Emit(OpCodes.Ldc_R8, this.MaxSubdivisionWidth);
			cc.il.Emit(OpCodes.Div);
			//on le converti en integer
			cc.il.Emit(OpCodes.Conv_I4);
			//on lui ajoute 5. c'est juste pour être sûr qu'on calcul un minimum de région
			cc.il.Emit(OpCodes.Ldc_I4, 5); // 5
			cc.il.Emit(OpCodes.Add);

			//maintenant on stocke le nombre de division dans une variable
			cc.il.Emit(OpCodes.Dup); //on duplique pour ne pas avoir à recharger cette valeur après
			LocalBuilder lbDivCount = cc.il.DeclareLocal(typeof(int));
			cc.il.Emit(OpCodes.Stloc, lbDivCount);

			//maintenant on calcul la largeur d'une division. il faut d'abord reconvertir en double puis effectuer la division
			cc.il.Emit(OpCodes.Conv_R8);
			cc.il.Emit(OpCodes.Div); //effectue la division de l'espace entre les borne par le nombre de section à faire

			//maintenant on stocke la largeur d'une région dans une variable
			LocalBuilder lbDivWidth = cc.il.DeclareLocal(typeof(double));
			cc.il.Emit(OpCodes.Stloc, lbDivWidth);


			//maintenant on donne sa valeur de début à la variable de l'intégration. valeur départ = bound début + delta/2
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			//on va diviser la largeur d'une division par 2 pour que la variable de l'intégration soit au milieu
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Ldc_R8, 2d);
			cc.il.Emit(OpCodes.Div);
			cc.il.Emit(OpCodes.Add);
			//maintenant on envoie cette valeur de départ dans la variable de l'intégration
			cc.il.Emit(OpCodes.Stloc, varlb);


			//maintenant on crée la variable qui va contenir la somme de toute les région. c'est cette variable la réponse de l'intégration
			LocalBuilder lbSum = cc.il.DeclareLocal(typeof(double));
			//on l'initialize avec 0
			cc.il.Emit(OpCodes.Ldc_R8, 0d);
			cc.il.Emit(OpCodes.Stloc, lbSum);




			//on met et on garde sur le stack le nombre de division à effectuer. cette position du stack est décrémenté et lorsqu'à 0, la boucle de calcul est quitté
			cc.il.Emit(OpCodes.Ldloc, lbDivCount);


			Label lblLoop = cc.il.DefineLabel();
			Label lblExitLoop = cc.il.DefineLabel();
			//////// début du calcul ////////
			cc.il.MarkLabel(lblLoop);
			//on vérifie si nous avons terminé de calculer tout les secteur.
			cc.il.Emit(OpCodes.Dup); //on le duplique car il sera consommé
			cc.il.Emit(OpCodes.Ldc_I4, 0); //on compare le nombre de secteur restant à 0
			cc.il.Emit(OpCodes.Ble, lblExitLoop); //si le nombre restant de secteur est inférieur ou égale à 0, on quitte la boucle


			//on met le code qui fait calculer la valeur actuel de l'équation de l'intégrale
			this.equMain.CompileToMSIL(cc);
			//on ajoute cela à l'air totale de l'intégrale
			cc.il.Emit(OpCodes.Ldloc, lbSum);
			cc.il.Emit(OpCodes.Add);
			//on renvoit ca dans l'air totale de l'intégrale.
			cc.il.Emit(OpCodes.Stloc, lbSum);
			
			////// next iteration
			//on décrémente de 1 la position du stack qui indique le nombre de section restante à faire
			cc.il.Emit(OpCodes.Ldc_I4, 1);
			cc.il.Emit(OpCodes.Sub);

			//on augmente la valeur de la variable de l'intégration. on doit l'incrémenter de DivWidth pour passer à la section suivante
			cc.il.Emit(OpCodes.Ldloc, varlb);
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Add);
			cc.il.Emit(OpCodes.Stloc, varlb);


			cc.il.Emit(OpCodes.Br, lblLoop);
			//////// fin de la boucle ////////
			cc.il.MarkLabel(lblExitLoop);
			cc.il.Emit(OpCodes.Pop); //on pop du stack le nombre de secteur qui restait
			cc.PopLastLayer();


			//on doit laisser sur le stack la valeur de l'intégrale. c'est ici qu'on multiplie la somme totale des secteur par leur largeur DivWidth
			cc.il.Emit(OpCodes.Ldloc, lbSum);
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Mul);



			//// fin du calcul
			//ici on check s'il faut inverser le signe de la réponse parce que les bound auraient été inversé au début.
			//la cette variable vaut 0, on quitte immédiatement
			cc.il.Emit(OpCodes.Ldloc, lbBoundsOk);
			cc.il.Emit(OpCodes.Ldc_I4, 0);
			cc.il.Emit(OpCodes.Beq, lblEnd);
			//puisque les bounds on été inversé, il faut changer le signe de la réponse
			cc.il.Emit(OpCodes.Neg);
			cc.il.Emit(OpCodes.Br, lblEnd);
			

			//// fin si l'intégrale est null
			cc.il.MarkLabel(lblNullIntegral);
			cc.il.Emit(OpCodes.Ldc_R8, 0d);

			cc.il.MarkLabel(lblEnd);
		}

		//procède par la méthode des trapèze.
		private void CompileToMSIL_v2(CompileContext cc)
		{
			Label lblEnd = cc.il.DefineLabel();

			//// on calcul la borne supérieur
			LocalBuilder lbBoundUp = cc.il.DeclareLocal(typeof(double));
			this.equUp.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Stloc, lbBoundUp.LocalIndex);

			//// on calcul la borne inférieur
			LocalBuilder lbBoundDown = cc.il.DeclareLocal(typeof(double));
			this.equDown.CompileToMSIL(cc);
			cc.il.Emit(OpCodes.Stloc, lbBoundDown);


			//// si les deux bound sont identique, l'intégrale vaut 0.
			Label lblNullIntegral = cc.il.DefineLabel();
			//on met sur le stack les 2 bound. si les 2 bound sont égaux, ont goto directement à la fin, à un endroit spécial qui va laisser 0 tout seul sur le stack.
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp.LocalIndex);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown.LocalIndex);
			cc.il.Emit(OpCodes.Beq, lblNullIntegral);

			//// make sure que la bound up est plus grande que la bound down
			LocalBuilder lbBoundsOk = cc.il.DeclareLocal(typeof(int)); //si 0, l'intégrale n'est pas à inverser. si 1, le signe de la réponse est à inverser.
			cc.il.Emit(OpCodes.Ldc_I4, 0);
			cc.il.Emit(OpCodes.Stloc, lbBoundsOk); //on met 0 à l'intérieur pour indiquer qu'il ne faut pas changer le signe
			Label lblBoundsOk = cc.il.DefineLabel();
			//on met les 2 bound sur le stack et si elles sont correcte, on skip la partie suivante qui les échange
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Bgt, lblBoundsOk); //si la bound up est supérieur à la bound down, on skip ce qui suit

			//si on est ici, les bounds sont à inverser
			//on charge sur le stack les valeur des bound et on les replace dans l'ordre inverse
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Stloc, lbBoundUp);
			cc.il.Emit(OpCodes.Stloc, lbBoundDown);
			//maintenant qu'on a inversé les bound il faut mettre 1 dans la variable qui indique si les bounds sont inversé
			cc.il.Emit(OpCodes.Ldc_I4, 1);
			cc.il.Emit(OpCodes.Stloc, lbBoundsOk);

			//// fin du check des bound
			cc.il.MarkLabel(lblBoundsOk);

			////// à partir d'ici les bounds sont correcte //////


			//maintenant que les bornes de départ et de fin ont été calculé
			//crée la nouvelle layer, en se basant sur la layer actuel, et en y ajoutant sa variable
			CompileContext.VariableLayer layer = cc.CreateNewLayerFromLast(this.VarName);
			//crée le jeton msil pour la variable de l'intégration
			LocalBuilder varlb = cc.il.DeclareLocal(typeof(double));
			//ajoute la variable dans la layer
			layer.dictPrivateVar.Add(this.VarName, varlb);


			//// préparation du calcul
			//on calcul la quantité de division à faire. ca dépend de l'espace qu'il y a entre les bound.
			//on charge dans le stack les 2 bound et on les soustrait pour connaitre l'espace
			cc.il.Emit(OpCodes.Ldloc, lbBoundUp);
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			cc.il.Emit(OpCodes.Sub);

			//on duplique l'espace entre les bounds pour effectuer un autre calcul après le calcul du nombre de division
			cc.il.Emit(OpCodes.Dup);

			//maintenant on le divise par MaxSubdivWidth pour connaitre le nombre de section aproximatif
			cc.il.Emit(OpCodes.Ldc_R8, this.MaxSubdivisionWidth);
			cc.il.Emit(OpCodes.Div);
			//on le converti en integer
			cc.il.Emit(OpCodes.Conv_I4);
			//on lui ajoute 5. c'est juste pour être sûr qu'on calcul un minimum de région
			cc.il.Emit(OpCodes.Ldc_I4, 5); // 5
			cc.il.Emit(OpCodes.Add);

			//maintenant on stocke le nombre de division dans une variable
			cc.il.Emit(OpCodes.Dup); //on duplique pour ne pas avoir à recharger cette valeur après
			LocalBuilder lbDivCount = cc.il.DeclareLocal(typeof(int));
			cc.il.Emit(OpCodes.Stloc, lbDivCount);

			//maintenant on calcul la largeur d'une division. il faut d'abord reconvertir en double puis effectuer la division
			cc.il.Emit(OpCodes.Conv_R8);
			cc.il.Emit(OpCodes.Div); //effectue la division de l'espace entre les borne par le nombre de section à faire

			//maintenant on stocke la largeur d'une région dans une variable
			LocalBuilder lbDivWidth = cc.il.DeclareLocal(typeof(double));
			cc.il.Emit(OpCodes.Stloc, lbDivWidth);


			//maintenant on donne sa valeur de début à la variable de l'intégration. valeur départ = bound début
			cc.il.Emit(OpCodes.Ldloc, lbBoundDown);
			//maintenant on envoie cette valeur de départ dans la variable de l'intégration
			cc.il.Emit(OpCodes.Stloc, varlb);


			//maintenant on crée la variable qui va contenir la somme de toute les région. c'est cette variable la réponse de l'intégration
			LocalBuilder lbSum = cc.il.DeclareLocal(typeof(double));
			//on l'initialize avec 0
			cc.il.Emit(OpCodes.Ldc_R8, 0d);
			cc.il.Emit(OpCodes.Stloc, lbSum);



			////les borne de début et de fin sont à ajouter à la somme totale mais seulement en x0.5. il faut donc calculer pour la borne de départ et de fin séparément.
			////ici on fait le calcul de la valeur de départ, on le divise par 2 puis ensuite on l'ajoute à la variable de la somme
			//on met le code qui fait calculer l'équation, alors que la variable de l'intégrale est sur la borne de départ
			this.equMain.CompileToMSIL(cc);
			//on doit diviser cette valeur par 2 avant de l'ajouter à la somme total
			cc.il.Emit(OpCodes.Ldc_R8, 2d);
			cc.il.Emit(OpCodes.Div);
			//on l'ajoute à la somme
			cc.il.Emit(OpCodes.Stloc, lbSum); //ici, la somme est actuellement égale à 0 donc on peut juste l'envoyer directement

			//maintenenat on doit augmenter la variable de l'intégration pour passer au secteur suivant
			cc.il.Emit(OpCodes.Ldloc, varlb);
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Add);
			cc.il.Emit(OpCodes.Stloc, varlb);




			//on met et on garde sur le stack le nombre de division à effectuer. cette position du stack est décrémenté et lorsqu'à 0, la boucle de calcul est quitté
			cc.il.Emit(OpCodes.Ldloc, lbDivCount);


			Label lblLoop = cc.il.DefineLabel();
			Label lblExitLoop = cc.il.DefineLabel();
			//////// début du calcul ////////
			cc.il.MarkLabel(lblLoop);
			//on vérifie si nous avons terminé de calculer tout les secteur, sauf le dernier.
			cc.il.Emit(OpCodes.Dup); //on le duplique car il sera consommé
			cc.il.Emit(OpCodes.Ldc_I4, 1); //on compare le nombre de secteur restant à 1
			cc.il.Emit(OpCodes.Ble, lblExitLoop); //si le nombre restant de secteur est inférieur ou égale à 1, on quitte la boucle


			//on met le code qui fait calculer la valeur actuel de l'équation de l'intégrale
			this.equMain.CompileToMSIL(cc);
			//on ajoute cela à l'air totale de l'intégrale
			cc.il.Emit(OpCodes.Ldloc, lbSum);
			cc.il.Emit(OpCodes.Add);
			//on renvoit ca dans l'air totale de l'intégrale.
			cc.il.Emit(OpCodes.Stloc, lbSum);

			////// next iteration
			//on décrémente de 1 la position du stack qui indique le nombre de section restante à faire
			cc.il.Emit(OpCodes.Ldc_I4, 1);
			cc.il.Emit(OpCodes.Sub);

			//on augmente la valeur de la variable de l'intégration. on doit l'incrémenter de DivWidth pour passer à la section suivante
			cc.il.Emit(OpCodes.Ldloc, varlb);
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Add);
			cc.il.Emit(OpCodes.Stloc, varlb);


			cc.il.Emit(OpCodes.Br, lblLoop);
			//////// fin de la boucle ////////
			cc.il.MarkLabel(lblExitLoop);
			cc.il.Emit(OpCodes.Pop); //on pop du stack le nombre de secteur qui restait

			////on doit calculer la borne de fin. la variable de l'intégration a déjà la bonne valeur
			//on met le code qui fait calculer l'équation.
			this.equMain.CompileToMSIL(cc);
			//on divise cette valeur par 2
			cc.il.Emit(OpCodes.Ldc_R8, 2d);
			cc.il.Emit(OpCodes.Div);
			//on l'additionne au reste de l'intégrale
			cc.il.Emit(OpCodes.Ldloc, lbSum);
			cc.il.Emit(OpCodes.Add);
			//on doit laisser sur le stack la valeur de l'intégrale. c'est ici qu'on multiplie la somme totale des secteur par leur largeur DivWidth
			cc.il.Emit(OpCodes.Ldloc, lbDivWidth);
			cc.il.Emit(OpCodes.Mul);


			cc.PopLastLayer();


			//// fin du calcul
			//ici on check s'il faut inverser le signe de la réponse parce que les bound auraient été inversé au début.
			//la cette variable vaut 0, on quitte immédiatement
			cc.il.Emit(OpCodes.Ldloc, lbBoundsOk);
			cc.il.Emit(OpCodes.Ldc_I4, 0);
			cc.il.Emit(OpCodes.Beq, lblEnd);
			//puisque les bounds on été inversé, il faut changer le signe de la réponse
			cc.il.Emit(OpCodes.Neg);
			cc.il.Emit(OpCodes.Br, lblEnd);


			//// fin si l'intégrale est null
			cc.il.MarkLabel(lblNullIntegral);
			cc.il.Emit(OpCodes.Ldc_R8, 0d);

			cc.il.MarkLabel(lblEnd);
		}



	}
}
