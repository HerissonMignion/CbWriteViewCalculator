using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace CbWriteViewCalculator
{
	//c'est object est essentiel pour le processus de la compilation des équation parce qu'il contient des information importante et gère les variable.
	public class CompileContext
	{
		public oEquationCalculContext CalculContext = null;


		#region gestion des variable des équation
		//liste de toute les variable extérieur à l'équation compilé. souvant les variable globale.
		//l'ordre des variable dans cette list est très important. elles sont à fournir au même endroit dans le double[] à donner à this.dm. leurs index sont les même
		private List<string> listExternalVariables;
		



		//le fonctionnement des layer de variable ici c'est la même chose que pour equation calcul context

		public class VariableLayer
		{
			public Dictionary<string, LocalBuilder> dictPrivateVar = new Dictionary<string, LocalBuilder>();
		}
		public VariableLayer LayerExternal; //la couche qui contient toute les variable qui viennent de l'extérieur
		public List<VariableLayer> listLayer = new List<VariableLayer>(); //liste de toute les layer

		//crée, pour qui le veux, une nouvelle layer qui utilise les même variable et l'ajoute automatiquement à la liste des layer dans this.
		public VariableLayer CreateNewLayerFromLast(string ToNotCopyVarName = "")
		{
			VariableLayer vl = new VariableLayer();

			//copy toute les variable de la layer précédante
			if (this.listLayer.Count > 0)
			{
				//on copy les variable de la layer précédante
				foreach (KeyValuePair<string, LocalBuilder> pair in this.listLayer[this.listLayer.Count - 1].dictPrivateVar)
				{
					//on copy la variable si elle n'a pas le nom spécifié en paramètre
					if (pair.Key != ToNotCopyVarName)
					{
						vl.dictPrivateVar.Add(pair.Key, pair.Value);
					}
				}
			}

			this.listLayer.Add(vl);
			return vl;
		}

		//pop la dernière layer
		public void PopLastLayer()
		{
			if (this.listLayer.Count > 0)
			{
				this.listLayer.RemoveAt(this.listLayer.Count - 1);
			}
		}


		//retourne une variable d'équation, via son nom, selon le context actuel.
		public LocalBuilder GetVariableAccordingToContext(string VarName)
		{
			//on commence par vérifier sur l'étage contextuel actuel
			if (this.listLayer.Count > 0)
			{
				//parcourt toute les variable du dernier étage
				foreach (KeyValuePair<string, LocalBuilder> pair in this.listLayer[this.listLayer.Count - 1].dictPrivateVar)
				{
					//check si le nom de la variable correspond
					if (pair.Key == VarName)
					{
						//puisque c'est la bonne variable, on retourne son local builder
						return pair.Value;
					}
				}
			}

			//puisqu'il n'y a pas la variable recherché dans l'étage contextuel actuel, on cherche maintenant dans les variable d'équation extérieur
			foreach (KeyValuePair<string, LocalBuilder> pair in this.LayerExternal.dictPrivateVar)
			{
				//check si le nom de la variable correspond
				if (pair.Key == VarName)
				{
					//puisque c'est la bonne variable, on retourne son local builder
					return pair.Value;
				}
			}

			//si aucune variable n'a été trouvé, on retourne null
			return null;
		}
		
		#endregion



		public CompiledEquation DirectFunction; //la fonction ÉC terminé. on l'appelle avec un double[] en argument.

		//va appeller l'équation avec les valeurs indiqués des variable externe
		public sCompiledEquationResult Compute(List<KeyValuePair<string, double>> listVarNameValue)
		{
			CompileContext.ClearError();


			double[] param = new double[this.listExternalVariables.Count];
			//donne une valeur par défault à toute les variable
			for (int i = 0; i < this.listExternalVariables.Count; i++)
			{
				param[i] = 0d;
			}

			//maintenant on envoie dans le tableau des variable externe les valeur de toute les variable
			foreach (KeyValuePair<string, double> pair in listVarNameValue)
			{
				//on obtien l'index de la variable actuel et on y met sa valeur donné en paramètre
				int varindex = this.listExternalVariables.IndexOf(pair.Key);
				//check si l'index existe
				if (varindex >= 0 && varindex < this.listExternalVariables.Count)
				{
					param[varindex] = pair.Value;
				}
			}


			//fait calculer la réponse
			double rep = this.DirectFunction(param);


			//maintenant on prépare la structure à retourner
			sCompiledEquationResult toret = new sCompiledEquationResult(rep);
			//s'il y a des erreur managé par le code pendant l'execution du code, on ne le saura pas autrement que par ces variable parce que des valeur de secour (0d etc) sont inséré dans le stack pour ne pas "déstabiliser le stack".
			toret.AnErrorOccurred = CompileContext.AnErrorWasReported;
			toret.ErrorMessage = CompileContext.ErrorMessage;

			return toret;
		}

		#region erreur managé pendant l'execution du code compilé
		//les membre de cette région sont static pour pouvoir être callé depuis n'importe où par le code compilé

		public static bool AnErrorWasReported = false;
		public static string ErrorMessage = "";
		public static void ReportError(string msg)
		{
			//uniquement la première erreur est importante, car elle peut être la cause de d'autres erreurs qui n'auraient pas lieu autrement
			if (!CompileContext.AnErrorWasReported)
			{
				CompileContext.AnErrorWasReported = true;
				CompileContext.ErrorMessage = msg;
			}
		}
		public static void ReportErrorNum(double num)
		{
			//uniquement la première erreur est importante, car elle peut être la cause de d'autres erreurs qui n'auraient pas lieu autrement
			if (!CompileContext.AnErrorWasReported)
			{
				CompileContext.AnErrorWasReported = true;
				CompileContext.ErrorMessage = "A number has been reported : " + num.ToString();
			}
		}

		public static void ClearError()
		{
			CompileContext.AnErrorWasReported = false;
			CompileContext.ErrorMessage = "";
		}


		
		#endregion



		public oEquation TheEQ = null; //l'équation qui a été compilé
		public delegate double CompiledEquation(double[] param);
		public DynamicMethod dm = null;
		public ILGenerator il = null;
		
		public void CompileEquation(oEquation eq)
		{
			this.TheEQ = eq;
			
			//crée la méthode dynamique et le il generator
			this.dm = new DynamicMethod("CompiledEquation", typeof(double), new Type[] { typeof(double[]) });
			this.il = this.dm.GetILGenerator(256); // 256

			//on crée la layer qui contient toute les variable externe
			this.LayerExternal = new VariableLayer();
			//on y ajoute toute les variable, une par une.
			this.il.Emit(OpCodes.Ldarg_0); //on met le tableau des variable externe (paramètre de la fonction dynamique, équation compilé) sur le stack. le faire ici permet d'éviter de répéter cette opération plusieurs fois.
			int ActualIndex = 0;
			foreach (string ExternalVarName in this.listExternalVariables)
			{
				LocalBuilder lb = this.il.DeclareLocal(typeof(double)); //crée le local builder de la variable local assigné à la variable externe actuel

				//ajoute la variable externe et son local builder à la layer des variable externe
				this.LayerExternal.dictPrivateVar.Add(ExternalVarName, lb);

				//ajoute le code qui transfère la valeur donné en paramètre à sa variable local assigné
				this.il.Emit(OpCodes.Dup); //on duplique le tableau dans le stack parce que l'opération suivant le pop.
				this.il.Emit(OpCodes.Ldc_I4, ActualIndex); //on met l'index désiré dans le stack.
				this.il.Emit(OpCodes.Ldelem, typeof(double)); //le tableau est popé du stack et est remplacé par la valeur stocké à l'index du tableau.
				this.il.Emit(OpCodes.Stloc, lb.LocalIndex); //envoie la valeur de la variable externe dans sa variable local assigné.
				
				//next iteration
				ActualIndex++;
			}
			this.il.Emit(OpCodes.Pop); //on retire le tableau du stack. il y avait été ajouté avant le foreach juste dessus.

			////// à partir d'ici, les valeurs ds variable externe, fournit en paramètre de l'ÉC, ont étés assigné à leurs variable local respective.



			////// maintenant on ajoute le code compilé qui fait calculer l'équation. c'est l'équation qui se charge de cette parti, avec l'aide de this.
			eq.CompileToMSIL(this);

			//////maintenant que l'équation a ajouté son code, il faut ajouter l'instruction return.
			this.il.Emit(OpCodes.Ret);

			////// à partir d'ici, la génération du code MSIL est complètement terminé.


			//on crée le délégate qui sera utilisé pour caller la fonction
			this.DirectFunction = (CompiledEquation)(this.dm.CreateDelegate(typeof(CompiledEquation)));

			
		}



		//void new()
		public CompileContext(List<string> ExternalVariableNames, oEquationCalculContext TheCalculContext)
		{
			this.listExternalVariables = ExternalVariableNames;
			this.CalculContext = TheCalculContext;
			



		}
		

	}
}
