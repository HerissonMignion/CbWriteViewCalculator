using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator.GraphingCalculator
{
	//petit objet graphique qui donne à l'user la possibilité de modifier le DrawFunction associé
	public class uiDrawFunction
	{
		private DrawFunction zzzDF = null;
		public DrawFunction DF
		{
			get { return this.zzzDF; }
		}



		private Point MousePos { get { return this.ImageBox.PointToClient(Cursor.Position); } }
		public Rectangle MouseRec { get { return new Rectangle(this.MousePos, new Size(1, 1)); } }




		private PictureBox ImageBox = null;
		public Control Parent
		{
			get { return this.ImageBox.Parent; }
			set { this.ImageBox.Parent = value; }
		}
		public int Top
		{
			get { return this.ImageBox.Top; }
			set { this.ImageBox.Top = value; }
		}
		public int Left
		{
			get { return this.ImageBox.Left; }
			set { this.ImageBox.Left = value; }
		}
		public int Width
		{
			get { return this.ImageBox.Width; }
		}
		public int Height
		{
			get { return this.ImageBox.Height; }
		}
		private void SetSize(int newWidth, int newHeight)
		{
			this.ImageBox.Size = new Size(newWidth, newHeight);
		}







		public uiDrawFunction(DrawFunction sDF)
		{
			this.zzzDF = sDF;
			this.DF.SomethingChanged += new EventHandler(this.DF_SomethingChanged);
			
			this.ImageBox = new PictureBox();
			this.ImageBox.BorderStyle = BorderStyle.FixedSingle;
			this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);
			this.ImageBox.MouseDown += new MouseEventHandler(this.ImageBox_MouseDown);



			this.SetSize(150, 45); // 100, 45
			this.Refresh();
		}

		private void DF_SomethingChanged(object sender, EventArgs e)
		{
			this.ReSync();
			this.Refresh();
		}
		private void ImageBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Point mpos = this.MousePos;
				//check si la souris est dans la section verticale de gauche
				if (mpos.X <= this.uiDrawSpace)
				{
					int hd2 = this.Height / 2;
					//check si la souris est dessus ou dessous du milieu
					if (mpos.Y < hd2)
					{
						//la souris est en haut


					}
					else //la souris est en bas
					{

					}

				}
				else //la souris n'est pas dans la section verticale de gauche
				{
					int menuy = this.Height + 100;
					if (this.IsMenuOpen) { menuy = this.Height - this.uiMenuSpace; }

					//maintenant on check si la souris est dans l'espace du texte
					if (mpos.Y < menuy)
					{
						//la souris est dans l'espace réservé au texte

						//si l'user fait un left click, on pop directement la fenetre pour modifier le df
						FormDrawFunctionEditer fdfe = new FormDrawFunctionEditer(this.DF);
						fdfe.Show();
					}
					else //la souris est dans le menu
					{
						//on check si la souris est sur le button delete, qui est à droite complètement
						if (mpos.X >= this.Width - this.uiMenuSpace)
						{
							//puisque l'user a clické sur le button delete, on va aller chercher le virtual context, et on va lui dire de retirer la draw function associé à this
							this.DF.VC.RemoveDrawFunction(this.DF);

							//le control this sera effacé par la form qui a créée et qui contient this


						}
						else //la souris n'est pas sur le button delete
						{




						}

					}
				}


			}
			if (e.Button == MouseButtons.Right)
			{

			}
		}
		private void ImageBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Point mpos = this.MousePos;
				//check si la souris est dans la section verticale de gauche
				if (mpos.X <= this.uiDrawSpace)
				{
					int hd2 = this.Height / 2;
					//check si la souris est dessus ou dessous du milieu
					if (mpos.Y < hd2)
					{
						//la souris est en haut
						this.DF.Visible = !this.DF.Visible;
						this.Refresh();
						this.DF.Inform_SomethingChanged();
					}
					else //la souris est en bas
					{
						//on toggle l'état ouvert du menu
						this.IsMenuOpen = !this.IsMenuOpen;
						this.Refresh();
					}

				}
				else //la souris n'est pas dans la section verticale de gauche
				{
					int menuy = this.Height + 100;
					if (this.IsMenuOpen) { menuy = this.Height - this.uiMenuSpace; }

					//maintenant on check si la souris est dans l'espace du texte
					if (mpos.Y < menuy)
					{
						//la souris est dans l'espace réservé au texte
						
					}
					else //la souris est dans le menu
					{
						//on check si la souris est sur le button delete, qui est à droite complètement
						if (mpos.X >= this.Width - this.uiMenuSpace)
						{

						}
						else //la souris n'est pas sur le button delete
						{
							//on check si la souris est sur le button color
							if (this.uimenuColorStartX < mpos.X && mpos.X < this.uimenuColorEndX)
							{
								//on ouvre un dialog qui va permettre à l'user de changer la couleur
								ColorDialog cd = new ColorDialog();
								cd.Color = this.DF.Color;
								DialogResult rep = cd.ShowDialog();
								if (rep == DialogResult.OK)
								{
									this.DF.Color = cd.Color;
									this.DF.Inform_SomethingChanged();
								}

								//on ferme le menu
								this.IsMenuOpen = false;
								this.Refresh();

							}
							else //la souris n'est pas sur le button color
							{

							}
						}

					}
				}


			}
			if (e.Button == MouseButtons.Right)
			{

			}
		}


		
		//lorsque qqc a été modifié dans le DrawFunction, cette void resynchronise this avec toute les propriété du df dont this est en lien.
		private void ReSync()
		{

			this.Refresh();
		}







		//ces valeur sont défini par Refresh().
		//ces valeur indiquent où graphiquement, dans le menu, commencent et se terminent les différentes options donné à l'user
		private int uimenuColorStartX = 0;
		private int uimenuColorEndX = 10;






		/* 
		 * +--------------------------------------------------+
		 * | +----+ |                                         |
		 * | |####| | function name here                      |
		 * | +----+ |                                         |
		 * | +----+ +-----------------------------------------+
		 * | |  M | |        menu                             |
		 * | +----+ |                                         |
		 * +--------------------------------------------------+
		 * 
		 * en haut à gauche il y a une checkbox qui sert à controler si la fonction est actuellement visible ou non.
		 * en dessous de ce checkbox y'a un button qui sert à ouvrir le menu.
		 * en haut à droite il y a l'espace où est écrit le nom de la fonction.
		 * 
		 * 
		 */


		private int uiDrawSpace = 23; // 23 espace réservé à gauche pour le button de visibilité et celui pour ouvrir le menu
		private int uiMenuSpace = 20; //espace verticalement réservé en bas à droite pour le menu, lorsqu'il est ouvert


		private bool IsMenuOpen = false; //indique si le menu est ouvert



		private Font uiNameFont1 = new Font("times new roman", 12f); // 11f
		private Font uiNameFont2 = new Font("times new roman", 9f); // 9f


		public void Refresh()
		{
			int imgWidth = this.Width;
			int imgHeight = this.Height;
			if (imgWidth < 10) { imgWidth = 10; }
			if (imgHeight < 10) { imgHeight = 10; }
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(Color.Gainsboro);


			//séparation verticale à gauche pour les button qui controlent la visibilité
			g.DrawLine(Pens.DimGray, this.uiDrawSpace, 0, this.uiDrawSpace, imgHeight);
			g.FillRectangle(Brushes.Silver, 0, 0, this.uiDrawSpace, imgHeight);

			////maintenant on va dessiner le "rectangle de visibilité"
			int rdvWidth = this.uiDrawSpace - 5;
			if (this.DF.Visible)
			{
				//si le df est visible, on va remplir l'intérieur par la couleur utilisé dans le plan virtuel pour dessiner la courbe de la df
				g.FillRectangle(this.DF.Brush, 4, 4, rdvWidth - 3, rdvWidth - 3);
			}
			else
			{
				Pen penBlack = new Pen(Color.Black, 2f);
				//si le df n'est actuellent pas visible, on dessine un x à l'intérieur
				g.DrawLine(penBlack, 4, 4, 4 + rdvWidth - 4, 4 + rdvWidth - 4);
				g.DrawLine(penBlack, 4, 4 + rdvWidth - 4, 4 + rdvWidth - 4, 4);
				penBlack.Dispose();
			}
			//on dessiner la bordure du "ractangle de visibilité
			g.DrawRectangle(Pens.Black, 2, 2, rdvWidth, rdvWidth);


			////maintenant on va dessiner les 3 point du menu
			int dot3y = imgHeight * 3 / 4 - 2;
			g.FillRectangle(Brushes.Black, (this.uiDrawSpace / 2), dot3y, 2, 2);
			g.FillRectangle(Brushes.Black, (this.uiDrawSpace / 2) + 4, dot3y, 2, 2);
			g.FillRectangle(Brushes.Black, (this.uiDrawSpace / 2) - 4, dot3y, 2, 2);





			////dessin du nom de la df.
			//cette fonction sépare une chaine de texte en une liste de tout les mots, sans les espaces.
			List<string> DivideIntoWords(string text)
			{
				List<string> rep = new List<string>();
				if (text.Length < 1) { return rep; }

				string Current = ""; //mot actuellement en train d'être lu

				foreach (char c in text)
				{
					//on check si on est tombé sur un espace
					if (c == ' ')
					{
						//on check d'abord que notre mot n'est pas vide
						if (Current.Length > 0)
						{
							//puisque le mot n'est pas vide, on l'ajoute à la liste des mots
							rep.Add(Current);
						}

						//on doit reseter le mot
						Current = "";
					}
					else //ce n'est pas un espace
					{
						//puisque ce n'est pas un espace, on l'ajoute au mot actuel
						Current += c;
					}
				}
				//il se peut que le text ne se termine pas par un espace. donc il faut gérer le dernier mot ici. donc maintenant on check si current n'est pas vide
				if  (Current.Length > 0)
				{
					//puisque current n'est pas vide, on doit ajouter le dernier mot à la liste des mot
					rep.Add(Current);
					Current = "";
				}
				
				return rep;
			}

			//ceci est la liste des ligne à dessiner dans la zone de texte. le but de cette section ci est de faire des ligne avec le plus de mots possible pour pas que ca dépasse à droite.
			List<string> listLines = new List<string>();
			//on obtient tout les mots qui composent le nom de la df.
			List<string> listMots = DivideIntoWords(this.DF.Name);

			//on détermine la font qu'on va utiliser. la différence entre les font c'est la taille de la police
			Font NameFontToUse = this.uiNameFont1;
			if (this.DF.Name.Length >= 25) { NameFontToUse = this.uiNameFont2; }

			//s'il y a 1 mot ou plus dans le nom, on va envoyer les mots dans les ligne.
			if (listMots.Count > 0)
			{
				//on calcul la largeur maximal de chaque ligne
				int AvailableWidth = imgWidth - this.uiDrawSpace;

				//on va maintenant générer la liste des lignes, ligne par ligne
				while (listMots.Count > 0)
				{
					//on commence une nouvelle ligne. il y a au moins 1 mot par ligne, donc on commence immédiatement en prenant le premier mots disponible.
					string CurrentLine = listMots[0];
					listMots.RemoveAt(0); //on retire le premier mot de la liste parce qu'on vient de le consommer

					//on va ajouter autant de mots que possible jusqu'à ce qu'on tombe sur un mot qui fera dépasser la largeur maximale de la ligne
					while (listMots.Count > 0)
					{
						//on fait la ligne actuelle, mais avec le nouveau mots supplémentaire
						string temp = CurrentLine + " " + listMots[0];

						//on analyse maintenant la largeur de la ligne
						SizeF tempSizeF = g.MeasureString(temp, NameFontToUse);
						//on check si l'ajout du nouveau mot fait dépasser la largeur maximale d'une ligne
						if ((int)(tempSizeF.Width) > AvailableWidth)
						{
							//puisque l'ajout du mot fait dépasser la largeur maximale d'une ligne, on arrête d'ajouter des mots à la ligne actuel.
							break;
						}
						else //l'ajout du nouveau mot ne fait pas dépasser la largeur maximale d'une ligne
						{
							//la ligne actuel est désormait doté du nouveau mot qu'on lui ajoute et on retire le nouveau mot de la liste des mots disponible
							CurrentLine = temp;
							listMots.RemoveAt(0);
						}

						//maintenant, s'il reste des mot, on va continuer de checker si plus de mots peuvent rentrer sur la ligne actuel
					}
					
					//maintenant qu'on a fini de générer la ligne actuel, on l'ajoute à la liste des ligne
					listLines.Add(CurrentLine);
				}

				//maintenant on doit make sure qu'il n'y a pas plus de ligne qu'un certain nombre maximal


			}
			else //listMots.Count == 0
			{
				//il n'y a pas de mot, donc on va faire une seule ligne avec un contenu par défaut
				listLines.Add("(Aucun nom)");
			}


			//maintenant on dessine chaque ligne
			int CurrentLineY = 0;
			foreach (string line in listLines)
			{
				g.DrawString(line, NameFontToUse, Brushes.Black, (float)(this.uiDrawSpace), (float)CurrentLineY);

				SizeF lineSizeF = g.MeasureString(line, NameFontToUse);
				//next iteration
				CurrentLineY += (int)(lineSizeF.Height) + 1;
			}




			////menu

			////séparation horizontale au milieu qui sépare l'espace du haut où est écrit le nom, de l'espace d'en bas où il y a des menu
			if (this.IsMenuOpen)
			{
				g.FillRectangle(Brushes.Silver, this.uiDrawSpace, imgHeight - this.uiMenuSpace, imgWidth - this.uiDrawSpace, this.uiMenuSpace);
				g.DrawLine(Pens.DimGray, this.uiDrawSpace, imgHeight - this.uiMenuSpace, imgWidth, imgHeight - this.uiMenuSpace);

				//on dessine l'option "Color"
				string strText = "Color";
				SizeF TextSizeF = g.MeasureString(strText, this.uiNameFont2);
				g.DrawString(strText, this.uiNameFont2, Brushes.Black, this.uiDrawSpace, imgHeight - (this.uiMenuSpace / 2) - (int)(TextSizeF.Height / 2f));
				//on ajuste ces valeur
				this.uimenuColorStartX = this.uiDrawSpace;
				this.uimenuColorEndX = this.uiDrawSpace + (int)(TextSizeF.Width);







				//on dessine le button x à droite. il a la même largeur que sa hauteur
				g.FillRectangle(Brushes.Crimson, imgWidth - this.uiMenuSpace, imgHeight - this.uiMenuSpace, this.uiMenuSpace, this.uiMenuSpace);
				Pen penWhite = new Pen(Color.White, 2f);
				g.DrawLine(penWhite, imgWidth - this.uiMenuSpace + 2, imgHeight - this.uiMenuSpace + 2, imgWidth - 5, imgHeight - 5);
				g.DrawLine(penWhite, imgWidth - this.uiMenuSpace + 2, imgHeight - 5, imgWidth - 5, imgHeight - this.uiMenuSpace + 2);
				penWhite.Dispose();


			}



			g.Dispose();
			if (this.ImageBox.Image != null) { this.ImageBox.Image.Dispose(); }
			this.ImageBox.Image = img;
			this.ImageBox.Refresh();
		}





		//dispose les objet qu'il peut et détruit les lien avec l'extérieur
		public void Dispose()
		{
			this.ImageBox.Parent = null;
			this.ImageBox.Dispose();



		}
	}
}
