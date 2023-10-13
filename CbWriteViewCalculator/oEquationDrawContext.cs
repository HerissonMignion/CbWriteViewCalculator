using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbWriteViewCalculator
{

	//WHU = width height upheight.
	public struct SizeWHU
	{
		public int Width;
		public int Height;
		public int UpHeight;

		public SizeWHU(int sWidth, int sHeight, int sUpHeight)
		{
			this.Width = sWidth;
			this.Height = sHeight;
			this.UpHeight = sUpHeight;
		}
	}

    public class oEquationDrawContext
    {


        public enum UiMultiplicationSymbole
        {
            symboleX,
            symboleDot
        }
        public UiMultiplicationSymbole UiMultiplicationSymboleToUse = UiMultiplicationSymbole.symboleDot;


        public bool DrawBorderOfAllObject = false; //specifie qu'il faut dessiner la bordure de tout les object et des equation
		
		[Obsolete("N'est plus utilisé")]
		public bool CenterObjectsAtVertical = true; //indique s'il faut (graphiquement) verticalement centrer les object sur la verticale





		/* 
		 * ici c'est la section du management des taille précalculé. pour la 2e méthode de génération de l'image d'une équation, il est d'abord précalculé la taille
		 * graphique de tout les composant de l'équation, et le draw context this est utilisé pour stocker les taille précalculé.
		 * 
		 */

		private Dictionary<oEquation, SizeWHU> dictEqu = new Dictionary<oEquation, SizeWHU>();
		private Dictionary<oEquationObject, SizeWHU> dictEo = new Dictionary<oEquationObject, SizeWHU>();

		//vide les dictionnaire des tailles précalculés
		public void ClearComputedSizes()
		{
			this.dictEqu.Clear();
			this.dictEo.Clear();
		}


		//ajoute un object et sa taille précalculé dans le dictionnaire des taille précalculé
		public SizeWHU AddComputedSize(oEquation equ, int Width, int Height, int UpHeight)
		{
			SizeWHU swhu = new SizeWHU(Width, Height, UpHeight);
			this.dictEqu.Add(equ, swhu);
			return swhu;
		}
		public SizeWHU AddComputedSize(oEquationObject eo, int Width, int Height, int UpHeight)
		{
			SizeWHU swhu = new SizeWHU(Width, Height, UpHeight);
			this.dictEo.Add(eo, swhu);
			return swhu;
		}


		//obtien la taille précalculé d'un objet
		public SizeWHU GetComputedSize(oEquation equ)
		{
			return this.dictEqu[equ];
		}
		public SizeWHU GetComputedSize(oEquationObject eo)
		{
			return this.dictEo[eo];
		}




    }
}
