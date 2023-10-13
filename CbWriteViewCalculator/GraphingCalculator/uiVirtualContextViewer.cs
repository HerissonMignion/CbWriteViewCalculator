using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator.GraphingCalculator
{
	public class uiVirtualContextViewer
	{
		private Point MousePos { get { return this.ImageBox.PointToClient(Cursor.Position); } }
		private Rectangle MouseRec { get { return new Rectangle(this.MousePos, new Size(1, 1)); } }


		private VirtualContext zzzVC;
		public VirtualContext VC { get { return this.zzzVC; } }
		public oEquationCalculContext ECC { get { return this.VC.ECC; } }


		private PictureBox ImageBox;

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
			set { this.ImageBox.Width = value; }
		}
		public int Height
		{
			get { return this.ImageBox.Height; }
			set { this.ImageBox.Height = value; }
		}
		public Size Size
		{
			get { return this.ImageBox.Size; }
			set { this.ImageBox.Size = value; }
		}
		public Point Location
		{
			get { return this.ImageBox.Location; }
			set { this.ImageBox.Location = value; }
		}
		public DockStyle Dock
		{
			get { return this.ImageBox.Dock; }
			set { this.ImageBox.Dock = value; }
		}
		public AnchorStyles Anchor
		{
			get { return this.ImageBox.Anchor; }
			set { this.ImageBox.Anchor = value; }
		}




		//décrit les état possible de l'interaction actuel avec l'user
		private enum Stade
		{
			none, //l'user n'interagit actuellement pas avec this.
			DragAndDrop, //l'user est en train de faire un drag and drop pour déplacer l'affichage


			MovingForm //l'user est en train de déplacer une uiForm

		}
		private Stade ActualStade = Stade.none;






		//void new()
		public uiVirtualContextViewer(VirtualContext StartVC)
		{
			this.zzzVC = StartVC;
			this.VC.NewDrawFunction += new EventHandler<DrawFunctionEventArgs>(this.VC_NewDrawFunction);
			this.VC.DrawFunctionRemoved += new EventHandler<DrawFunctionEventArgs>(this.VC_DrawFunctionRemoved);
			this.VC.DrawFunctionSomethingChanged += new EventHandler<DrawFunctionEventArgs>(this.VC_DrawFunctionSomethingChanged);


			this.ImageBox = new PictureBox();
			this.ImageBox.BorderStyle = BorderStyle.FixedSingle;
			this.ImageBox.SizeChanged += new EventHandler(this.ImageBox_SizeChanged);
			this.ImageBox.MouseMove += new MouseEventHandler(this.ImageBox_MouseMove);
			this.ImageBox.MouseDown += new MouseEventHandler(this.ImageBox_MouseDown);
			this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);
			this.ImageBox.MouseWheel += new MouseEventHandler(this.ImageBox_MouseWheel);


			this.ECC.VariableValueChanged += new EventHandler<VariableEventArgs>(this.ECC_VariableValueChanged);





			this.CreateDragAndDrop();
			this.CreateInterface();

			//this.interTESTEST();

			this.FullRefresh();
		}
		#region listened events
		
		private void ImageBox_SizeChanged(object sender, EventArgs e)
		{
			if (this.Width > 100 && this.Height > 100)
			{
				//make sure que toute les form sont accessible
				foreach (uiForm f in this.listForm)
				{
					f.MakeSureTitleIsInBound();
				}
			}

			this.ResizeInterface();

			this.FullRefresh();
		}

		private Font uiMousePosFont = new Font("consolas", 10f); //font utilisé pour afficher la position de la souris en haut à gauche
		private void ImageBox_MouseMove(object sender, MouseEventArgs e)
		{
			Point mpos = this.MousePos;
			PointD mvpos = this.Convert_UIToVirtual(mpos.X, mpos.Y);


			//si l'état graphique est sur none, on affiche la position de la souris en haut à gauche de l'écran
			if (this.ActualStade == Stade.none)
			{
				this.ImageBox.Refresh();
				//Graphics g = this.ImageBox.CreateGraphics();

				////on affiche la position de la souris
				//g.DrawString("x = " + mvpos.x.ToString(), this.uiMousePosFont, Brushes.White, 0f, 0f);
				//g.DrawString("y = " + mvpos.y.ToString(), this.uiMousePosFont, Brushes.White, 0f, 15f);


				//g.Dispose();
			}
			else if (this.ActualStade == Stade.MovingForm)
			{
				this.ImageBox.Refresh();
				Graphics g = this.ImageBox.CreateGraphics();

				//coordonné relative du coin supérieur gauche de la zone cliente
				Point ul = new Point(mpos.X - this.statemfDelta.X, mpos.Y - this.statemfDelta.Y);

				//on dessine le title
				g.FillRectangle(Brushes.Black, ul.X, ul.Y - this.uiFTitleHeight, this.statemfForm.TitleWidth, this.uiFTitleHeight);
				g.DrawRectangle(Pens.White, ul.X, ul.Y - this.uiFTitleHeight, this.statemfForm.TitleWidth, this.uiFTitleHeight);

				//si la zone cliente est visible, dessine un apercu d'elle
				if (this.statemfForm.Opened)
				{
					g.FillRectangle(Brushes.Black, ul.X, ul.Y, this.statemfForm.Width, this.statemfForm.Height);
					g.DrawRectangle(Pens.White, ul.X, ul.Y, this.statemfForm.Width, this.statemfForm.Height);

				}


				g.Dispose();
			}


			

		}
		private void ImageBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//check si l'état actuel est sur none
				if (this.ActualStade == Stade.none)
				{
					//check si la souris est sur un control graphique
					if (this.IsMouseOnAnyForm())
					{
						//on fait exécuter les truc en lien avec mouse left down
						this.Check_MouseLeftDown();

					}
					else
					{
						//si l'user left click dans le vide, c'est qu'il faut faire un déplacement/drag and drop
						this.StartDragAndDrop();


					}
				}
				else
				{



				}
			}
			if (e.Button == MouseButtons.Right)
			{
				if (this.ActualStade == Stade.none)
				{
					//check si la souris est actuellement dessus une form
					if (!this.IsMouseOnAnyForm())
					{
						string optSearchLocalMin = "Rechercher minimum local";
						string optSearchLocalMax = "Rechercher maximum local";
						string optComputeAtPos = "Calculer la fonction à un endroit précis";
						string optAproxTaylorSerie = "Aproximer la série de Taylor ici";

						wvRightClick3 rc3 = new wvRightClick3();
						rc3.ActualTheme = wvRightClick3.ColorTheme.Dark;
						rc3.AddSeparator();

						rc3.AddChoice(optSearchLocalMin);
						rc3.AddChoice(optSearchLocalMax);
						rc3.AddChoice(optComputeAtPos);
						rc3.AddChoice(optAproxTaylorSerie);

						string rep = rc3.GetChoice();
					}
					else
					{


					}



				}

				

			}
		}
		private void ImageBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//check si la souris est en train d'intéragir avec une form
				if (this.IsMouseLeftDownOnForm)
				{
					this.Check_MouseLeftUp();
					this.srefreshInterface(); //on refresh l'image
				}


				if (this.ActualStade == Stade.DragAndDrop)
				{
					//si l'user était en train de déplacer l'affichage, on arrête
					this.StopDragAndDrop();

				}
				else if (this.ActualStade == Stade.MovingForm)
				{
					//si l'user déplacait une fenêtre, alors on arrête le déplacement
					this.StopUserMovingForm();
				}

			}
			if (e.Button == MouseButtons.Right)
			{

			}
		}
		private void ImageBox_MouseWheel(object sender, MouseEventArgs e)
		{
			int d = e.Delta;

			int dist = 30;
			double mulfact = 1.05d;

			while (d > 10)
			{
				this.VirtualHeight /= mulfact;
				d -= dist;
			}
			while (d < -10)
			{
				this.VirtualHeight *= mulfact;
				d += dist;
			}

			this.srefreshFunctions();

		}


		//lorsque l'user appuis sur une touche, la form peut nous donner la touche avec cette void
		public void KeyDown(Keys k)
		{
			//ouvrir le menu
			if (k == Keys.E)
			{
				this.interFormMenu.Opened = !this.interFormMenu.Opened;
				this.interFormMenu.SendToTop();
				this.srefreshInterface();
			}
			//l'user veut faire recalculer les computed point des fonction. c'est seulement si le «recalcul automatique des fonction après déplacement» est désactivé.
			else if (k == Keys.C)
			{
				//puisque la touche c est un raccourci clavier pour un button qui existe dans l'interface graphique, on fait juste caller la void qui répond à l'event du button
				this.interBtnRefresh_MouseLeftUp(this.interBtnRefresh, new EventArgs());
				
			}



		}




		private void VC_NewDrawFunction(object sender, DrawFunctionEventArgs e)
		{
			//puisqu'un nouvelle fonction a été ajouté, il faut rafraichir l'image, mais pas besoin de recalculer toute les fonction existante parce qu'elle n'ont pas changé
			this.srefreshFunctions_SpecificRecompute(new DrawFunction[] { e.DF });
		}
		private void VC_DrawFunctionRemoved(object sender, DrawFunctionEventArgs e)
		{
			//on fait redessiner toute les draw function existante, mais sais recalculer leurs valeurs parce qu'elles n'ont pas changées
			this.srefreshFunctions_SpecificRecompute(new DrawFunction[] { });
		}
		private void VC_DrawFunctionSomethingChanged(object sender, DrawFunctionEventArgs e)
		{
			//uniquement la draw function qui vient d'être modifié a besoin d'être recalculé
			this.srefreshFunctions_SpecificRecompute(new DrawFunction[] { e.DF });
		}

		private void ECC_VariableValueChanged(object sender, VariableEventArgs e)
		{

			this.srefreshFunctions();

		}



		#endregion


		#region drag and drop (déplacement)
		private Timer dadTimer;

		private Point dad_UiStart = new Point(-1, -1); //position graphique de la souris au début du dad

		private void dadTimer_Tick(object sender, EventArgs e)
		{
			Point smpos = this.dad_UiStart;
			Point mpos = this.MousePos;

			//on refresh le picture box pour effacer les truc précédant
			this.ImageBox.Refresh();

			//on dessine l'image
			Graphics g = this.ImageBox.CreateGraphics();
			g.Clear(Color.Black);
			g.DrawImage(this.screenFunctions, mpos.X - smpos.X, mpos.Y - smpos.Y);
			g.FillRectangle(Brushes.Red, this.ImageBox.Width / 2 - 2, this.ImageBox.Height / 2 - 2, 4, 4);
			g.Dispose();

		}
		private void StartDragAndDrop()
		{
			this.ActualStade = Stade.DragAndDrop; //on set l'interface pour le dad
			this.dad_UiStart = this.MousePos; //on save la position de départ de la souris
			this.dadTimer.Start(); //on démare le timer qui gère graphiquement le dad

		}
		private void StopDragAndDrop()
		{
			Point smpos = this.dad_UiStart;
			Point mpos = this.MousePos;

			this.ActualStade = Stade.none; //on remet le stade sur none parce que c'est fini
			this.dadTimer.Stop();

			double dx = (double)(mpos.X - smpos.X);
			double dy = (double)(smpos.Y - mpos.Y);
			this.vposx -= dx / (double)(this.Height) * this.VirtualHeight;
			this.vposy -= dy / (double)(this.Height) * this.VirtualHeight;

			this.srefreshFunctions();
		}


		private void CreateDragAndDrop()
		{
			this.dadTimer = new Timer();
			this.dadTimer.Interval = 150;
			this.dadTimer.Tick += new EventHandler(this.dadTimer_Tick);


		}
		#endregion



		#region cossin virtuel
		
		private PointD vpos = new PointD(0d, 0d); //position virtuel du milieu de l'écran
		private double vposx
		{
			get { return this.vpos.x; }
			set { this.vpos.x = value; }
		}
		private double vposy
		{
			get { return this.vpos.y; }
			set { this.vpos.y = value; }
		}

		private double VirtualHeight = 11d; //height virtuel de l'écran

		
		//permet de passer des coordonné virtual à graphique et vise versa
		private Point Convert_VirtualToUI(PointD vp)
		{
			return this.Convert_VirtualToUI(vp.x, vp.y);
		}
		private Point Convert_VirtualToUI(double vx, double vy)
		{
			Point rep = new Point(0, 0);
			//rep.X = (int)(((double)(this.Width) / 2d) + ((vx - this.vposx) / this.VirtualHeight * (double)(this.Height)) + 0.5d);
			//rep.Y = (int)(((double)(this.Height) / 2d) - ((vy - this.vposy) / this.VirtualHeight * (double)(this.Height)) + 0.5d);
			rep.X = (int)(((double)(this.propImgWidth) / 2d) + ((vx - this.vposx) / this.VirtualHeight * (double)(this.propImgHeight)) + 0.5d);
			rep.Y = (int)(((double)(this.propImgHeight) / 2d) - ((vy - this.vposy) / this.VirtualHeight * (double)(this.propImgHeight)) + 0.5d);
			return rep;
		}
		private PointD Convert_UIToVirtual(Point p)
		{
			return this.Convert_UIToVirtual(p.X, p.Y);
		}
		private PointD Convert_UIToVirtual(int uix, int uiy)
		{
			PointD rep = new PointD(0d, 0d);
			//rep.x = this.vposx + (((double)uix - ((double)(this.Width) / 2d)) / (double)(this.Height) * this.VirtualHeight);
			//rep.y = this.vposy + ((((double)(this.Height) / 2d) - (double)uiy) / (double)(this.Height) * this.VirtualHeight);
			rep.x = this.vposx + (((double)uix - ((double)(this.propImgWidth) / 2d)) / (double)(this.propImgHeight) * this.VirtualHeight);
			rep.y = this.vposy + ((((double)(this.propImgHeight) / 2d) - (double)uiy) / (double)(this.propImgHeight) * this.VirtualHeight);
			return rep;
		}

		

		#endregion





		#region gestion complète des graphique
		//retourne la taille d'une nouvelle image (représentant la totalité du control this) à créer, mais avec une taille minimale.
		//leur nom est composé de prop au début parce que c'est une valeur raccourcit qui est défini comme une propriété private
		private int propImgWidth
		{
			get
			{
				int rep = this.Width;
				if (rep < 50) { rep = 50; }
				return rep;
			}
		}
		private int propImgHeight
		{
			get
			{
				int rep = this.Height;
				if (rep < 50) { rep = 50; }
				return rep;
			}
		}



		private Color zzzBackColor = Color.FromArgb(32, 32, 32);

		private Pen uiAxePen = Pens.Silver;
		private Font uiAxeFont = new Font("consolas", 10f); //la font à utiliser pour dessiner les numéro des graduation





		/* la création de l'image à afficher à l'écran est divisé en plusieur étage de dessin.
		 * 
		 */



		
		private Bitmap screenFunctions = null; //l'étage des fonction. contient les axe et leur graduation + le dessin des fonctions
		private Bitmap screenInterface = null; //l'étage de l'interface. contient screen functions mais a l'interface de dessiné par dessus

		//rafraichis l'étage des functions + les étage suppérieur
		private void srefreshFunctions()
		{
			int imgWidth = this.propImgWidth;
			int imgHeight = this.propImgHeight;
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.zzzBackColor);


			//on dessine les axe
			this.srf_DrawAxes(img, g);
			


			//////dessine les fonction
			foreach (DrawFunction DF in this.VC.listFunctions)
			{
				//on dessine la fonction seulement si elle est visible
				if (DF.Visible)
				{
					this.srf_DrawThisFunction(DF, img, g, this.propRecomputeAllAfterEveryMove, this.propLinkComputedP);
				}
			}
			


			g.Dispose();
			if (this.screenFunctions != null) { this.screenFunctions.Dispose(); }
			this.screenFunctions = img;

			//end
			this.srefreshInterface();
		}
		//va rafraichir l'étage des functions + les étage suppérieur, mais la plupart des fonction seront dessiné à partir de leur points déjà calculé. seulement les draw function spécifiés dans arToRecompute seront calculé/recalculé
		private void srefreshFunctions_SpecificRecompute(DrawFunction[] arToRecompute)
		{
			int imgWidth = this.propImgWidth;
			int imgHeight = this.propImgHeight;
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(this.zzzBackColor);


			//on dessine les axe
			this.srf_DrawAxes(img, g);

			

			//////dessine les fonction
			foreach (DrawFunction DF in this.VC.listFunctions)
			{
				//on dessine la fonction seulement si elle est visible
				if (DF.Visible)
				{
					this.srf_DrawThisFunction(DF, img, g, arToRecompute.Contains(DF), this.propLinkComputedP);
				}
			}



			g.Dispose();
			if (this.screenFunctions != null) { this.screenFunctions.Dispose(); }
			this.screenFunctions = img;

			//end
			this.srefreshInterface();
		}



		//rafraichis l'étage de l'interface + les étage suppérieur
		private void srefreshInterface()
		{
			int imgWidth = this.propImgWidth;
			int imgHeight = this.propImgHeight;
			Bitmap img = new Bitmap(this.screenFunctions);
			Graphics g = Graphics.FromImage(img);


			//dessine les uiForm
			foreach (uiForm f in this.listForm)
			{
				//on commence par checker si la uiForm est actuellement visible
				if (f.Visible)
				{
					////title
					//on commence par remplir l'arrière plan en noir, pour make sure que le title est correctement lisible peu importe ce qu'il y a derrière
					g.FillRectangle(Brushes.Black, f.Left, f.Top - this.uiFTitleHeight, f.TitleWidth, this.uiFTitleHeight);
					g.DrawRectangle(Pens.Gray, f.Left, f.Top - this.uiFTitleHeight, f.TitleWidth, this.uiFTitleHeight);

					////dessine le text du title
					int TitleTextLeft = this.uiFTitleHeight; //position horizontale du text dans le title
					if (f.UserCanClose) { TitleTextLeft += this.uiFTitleHeight; }
					g.DrawString(f.Title, this.uiFTitleFont, Brushes.White, (float)(f.Left + TitleTextLeft + 2), (float)(f.Top - (this.uiFTitleHeight / 2)) - (g.MeasureString(f.Title, this.uiFTitleFont).Height / 2f));
					
					////dessine le button qui permet d'ouvrir ou fermer la zone cliente
					g.FillRectangle(Brushes.DimGray, f.Left + 1, f.Top - this.uiFTitleHeight + 1, this.uiFTitleHeight - 2, this.uiFTitleHeight - 1);
					g.DrawLine(this.uiFTitleBtnPen, f.Left + 3, f.Top - (this.uiFTitleHeight / 2), f.Left + this.uiFTitleHeight - 3, f.Top - (this.uiFTitleHeight / 2));
					//on dessine la ligne verticale seulement si la zone cliente est actuellement caché
					if (!f.Opened) { g.DrawLine(this.uiFTitleBtnPen, f.Left + (this.uiFTitleHeight / 2), f.Top - this.uiFTitleHeight + 3, f.Left + (this.uiFTitleHeight / 2), f.Top - 3); }

					////on dessine le button qui permet de fermer la fenêtre pour de bon
					if (f.UserCanClose)
					{
						//on dessine l'arrière du button
						g.FillRectangle(Brushes.DimGray, f.Left + this.uiFTitleHeight, f.Top - this.uiFTitleHeight + 1, this.uiFTitleHeight, this.uiFTitleHeight - 1);

						//on dessine le x
						int space = 6;
						g.DrawLine(this.uiFTitleBtnPen, f.Left + this.uiFTitleHeight + space, f.Top - this.uiFTitleHeight + space, f.Left + (2 * this.uiFTitleHeight) - space, f.Top - space);
						g.DrawLine(this.uiFTitleBtnPen, f.Left + this.uiFTitleHeight + space, f.Top - space, f.Left + (2 * this.uiFTitleHeight) - space, f.Top - this.uiFTitleHeight + space);
						
					}



					////client area
					if (f.Opened)
					{
						//on set la clip pour ne pas que les controles à l'intérieur puissent dépasser la zone cliente
						g.SetClip(f.rec);
						g.Clear(f.BackColor); //on clear avec le backcolor

						//on dessine les label
						foreach (uiLabel lbl in f.listLabel)
						{
							if (lbl.Visible)
							{
								g.DrawString(lbl.Text, lbl.font, lbl.ForeBrush, (float)(f.Left + lbl.Left), (float)(f.Top + lbl.Top));
							}
						}

						//on dessine les button
						foreach (uiButton b in f.listButton)
						{
							if (b.Visible)
							{
								Rectangle uirec = new Rectangle(f.Left + b.Left, f.Top + b.Top, b.Width, b.Height);

								//on commence par l'arrière plan
								Brush backbrush = this.uiBtnBackBrush;
								Pen borderpen = Pens.Silver;
								if (b.isMouseLeftDown || !b.Enabled) { backbrush = this.uiBtnDownBackBrush; }
								if (!b.Enabled) { borderpen = Pens.DimGray; }
								g.FillRectangle(backbrush, uirec);
								g.DrawRectangle(borderpen, uirec.X, uirec.Y, uirec.Width - 1, uirec.Height - 1);

								//on dessine le text, centré horizontalement et verticalement
								SizeF TextSizeF = g.MeasureString(b.Text, b.font);
								g.DrawString(b.Text, b.font, Brushes.White, (float)(f.Left + b.Left + (b.Width / 2)) - (TextSizeF.Width / 2), (float)(f.Top + b.Top + (b.Height / 2)) - (TextSizeF.Height / 2f));


							}
						}

						//on dessine les textbox
						foreach (uiTextBox tb in f.listTextBox)
						{
							if (tb.Visible)
							{
								Rectangle uirec = new Rectangle(f.Left + tb.Left, f.Top + tb.Top, tb.Width, tb.Height);

								g.SetClip(uirec);
								g.Clear(Color.FromArgb(16, 16, 16)); //on met le back color

								//on dessine le text qui est à l'intérieur
								if (tb.Text.Length > 0)
								{
									float TextHeight = g.MeasureString(tb.Text, tb.font).Height;
									g.DrawString(tb.Text, tb.font, Brushes.White, (float)(uirec.X), (float)(uirec.Y + (tb.Height / 2)) - (TextHeight / 2f));

								}
								else
								{
									string txt = "(empty)";
									float TextHeight = g.MeasureString(txt, tb.font).Height;
									g.DrawString(txt, tb.font, Brushes.DimGray, (float)(uirec.X), (float)(uirec.Y + (tb.Height / 2)) - (TextHeight / 2f));

								}



								g.ResetClip();
								//on dessine la bordure du textbox
								g.DrawRectangle(Pens.DimGray, uirec.X, uirec.Y, uirec.Width - 1, uirec.Height - 1);


							}
						}

						foreach (uiCheckBox cb in f.listCheckBox)
						{
							if (cb.Visible)
							{
								////on commence par dessiner le carré de la checkbox
								Rectangle boxrec = new Rectangle(0, 0, cb.uiBoxWidth, cb.uiBoxWidth);
								boxrec.X = f.Left + cb.Left;
								boxrec.Y = f.Top + cb.Top + (cb.Height / 2) - (cb.uiBoxWidth / 2);
								g.FillRectangle(Brushes.Black, boxrec);
								g.DrawRectangle(Pens.DimGray, boxrec.X, boxrec.Y, boxrec.Width - 1, boxrec.Height - 1);
								//si la checkbox est checked, on remplit la box
								if (cb.Checked)
								{
									g.FillRectangle(Brushes.DimGray, boxrec.X + 2, boxrec.Y + 2, boxrec.Width - 4, boxrec.Height - 4);
								}
								
								////on dessine le text
								SizeF TextSizeF = g.MeasureString(cb.Text, cb.font);
								g.DrawString(cb.Text, cb.font, cb.ForeBrush, (float)(boxrec.X + boxrec.Width), (float)(boxrec.Y + (boxrec.Height / 2)) - (TextSizeF.Height / 2f));
								

							}
						}


						g.ResetClip();
						g.DrawRectangle(Pens.Gray, f.rec);
					}


				}
			}





			g.Dispose();
			if (this.screenInterface != null) { this.screenInterface.Dispose(); }
			this.screenInterface = img;

			//end
			this.ImageBox.Image = this.screenInterface;
			this.ImageBox.Refresh();
			GC.Collect();
		}
		

		public void FullRefresh()
		{
			this.srefreshFunctions();
		}



		//autre:

		//va dessiner les axe et les graduation dur l'image
		private void srf_DrawAxes(Bitmap img, Graphics g)
		{
			int imgWidth = img.Width;
			int imgHeight = img.Height;

			Point uiOrigin = this.Convert_VirtualToUI(0d, 0d);
			//////dessine les graduation
			////selon le niveau de zoom actuel, on détermine quel pas est le plus approprié pour les graduation
			long GradDelta = 1;
			//les graduation deviendront plus grande que 1 seulement si le niveau de zoom est suffisament élevé
			if (VirtualHeight >= 2d)
			{
				//largeur virtuel d'une pixel à l'écran. diviser une longueur virtuel par cette variable permet d'obtenir sa longueur graphique
				double PxWidth = this.VirtualHeight / (double)imgHeight;

				double MinPxWidth = 40d; // 75d distance graphique minimal en pixel entre 2 graduation

				//on va augmenter le pas des graduation jusqu'à temps que l'espace graphique entre 2 graduation soit au minimum MinPxWidth.
				//ne pas oublier que techniquement, GradDelta est une longueur virtuel.
				//les multiplication qui suivent ont pour but que les graduation répètent les nombre : 1 2 5. exemple :
				// 1 2 5   10 20 50   100 200 500   1000 2000 5000   10000 20000 50000 ...
				while ((double)GradDelta / PxWidth < MinPxWidth)
				{
					//on multiplie par 2
					GradDelta *= 2L;
					//on test si c'est suffisament grand désormais
					if ((double)GradDelta / PxWidth > MinPxWidth) { break; }

					//on multiplie par 5/2
					GradDelta = GradDelta * 5L / 2L;
					//on test si c'est suffisament grand désormais
					if ((double)GradDelta / PxWidth > MinPxWidth) { break; }

					//on multiplie par 2
					GradDelta *= 2L;
					//on test si c'est suffisament grand désormais
				}

			}
			double dblGradDelta = (double)GradDelta; //même chose mais convertie en double

			//vérifie si les axe x et y sont individuellement visible à l'écran.
			//cette facon de vérifier si les axe sont actuellement visible à l'écran ne nécessite pas uiOrigin.
			//j'ai voulu éviter uiOrigin parce qu'il se pourait que le zoom soit tellement élevé qu'il y ait un integer overflow sur les coordonné de uiOrigin
			bool IsAxeXVisible = Math.Abs(this.vposy) < this.VirtualHeight / 2d;
			bool IsAxeYVisible = Math.Abs(this.vposx) < this.VirtualHeight / (double)imgHeight * (double)imgWidth / 2d;


			PointD vUpLeft = this.Convert_UIToVirtual(0, 0); //on obtien la coordonné virtuel du coin supérieur gauche de l'écran, qui est le point de départ pour dessiner successivement toute les graduation

			//maintenant on entre dans le block où on dessine les graduation
			try
			{
				//il y a des "coche" qui acompagnent le numéro de la graduation. cette variable est la longueur graphique d'une "coche"
				int CocheLength = 4;


				////axe horizontale
				//si l'axe est visible à l'écran, cette variable indique s'il faut dessiner les nombre dessus ou en dessous de l'axe.
				//si l'axe est à l'extérieur de l'écran, true signifie qu'il faut dessiner les nombre en haut, false qu'il faut les dessiner en bas
				bool xWriteDown = true;
				if (IsAxeXVisible)
				{
					//on affiche les nombre en dessous si l'axe est dans les 2/3 supérieur de l'image
					xWriteDown = uiOrigin.Y < imgHeight * 2 / 3;
				}
				else
				{
					xWriteDown = this.vposy < 0d;
				}


				//on va faire l'horizontale X
				//on doit déterminer la graduation visible qui est le plus à gauche de l'écran.
				//pour cela on prend la position virtuel du côté gauche de l'écran, on la divise par le pas de graduation actuel, et on convertie en integer. cet integer est
				//par quoi multiplier GradDelta pour être à gauche de l'écran. puisque l'arrondissement des double -> integer est différente selon que les nombre soient
				//positif ou négatif, on va ensuite aller une couples de graduation supplémentaire à gauche pour commencer depuis l'extérieur de l'écran.

				long vxStart = ((long)(vUpLeft.x / dblGradDelta) * GradDelta) - (2L * GradDelta);
				//on parcourt toute les graduation horizontale jusqu'à arriver à droite de l'écran
				long vxActual = vxStart;
				while (true)
				{
					//on obtien le position graphique horizontale de la graduation actuel
					int uix = this.Convert_VirtualToUI((double)vxActual, vUpLeft.y).X;
					//si ca dépasse la droite de l'écran, on a terminé
					if (uix >= imgWidth) { break; }

					//on procède au dessin de la graduation si la ligne est dans l'image
					if (uix >= 0 && vxActual != 0L)
					{
						//on dessine la graduation horizontale, qui est une ligne vertical
						g.DrawLine(Pens.Black, uix, 0, uix, imgHeight - 1);

						////maintenant on dessine le numéro de la graduation. il faut procéder différement selon que l'axe soit dans l'écran ou non
						if (IsAxeXVisible)
						{
							//on prépare le text et on prépare aussi la position du text
							string Text = vxActual.ToString();
							SizeF TextSizeF = g.MeasureString(Text, this.uiAxeFont);
							int TextLeft = uix - (int)(TextSizeF.Width / 2f);
							//maintenant on prépare la position vertical du text, qui dépend de s'il faut le dessiner dessus ou en dessous de la ligne de l'axe
							int TextTop = uiOrigin.Y + CocheLength;
							if (xWriteDown)
							{
								g.DrawLine(this.uiAxePen, uix, uiOrigin.Y, uix, uiOrigin.Y + CocheLength);
							}
							else
							{
								TextTop = uiOrigin.Y - CocheLength - (int)(TextSizeF.Height);
								g.DrawLine(this.uiAxePen, uix, uiOrigin.Y, uix, uiOrigin.Y - CocheLength);
							}
							//maintenant on peut dessiner le text
							g.DrawString(Text, this.uiAxeFont, Brushes.White, (float)TextLeft, (float)TextTop);

						}
						else //l'axe horizontale est à l'extérieur de l'écran
						{
							//on prépare le text et on prépare aussi la position du text
							string Text = vxActual.ToString();
							SizeF TextSizeF = g.MeasureString(Text, this.uiAxeFont);
							int TextLeft = uix - (int)(TextSizeF.Width / 2f);
							//maintenant on prépare la position vertical du text
							int TextTop = CocheLength;
							if (xWriteDown)
							{
								g.DrawLine(this.uiAxePen, uix, 0, uix, CocheLength);
							}
							else
							{
								TextTop = imgHeight - CocheLength - (int)(TextSizeF.Height);
								g.DrawLine(this.uiAxePen, uix, imgHeight, uix, imgHeight - CocheLength);
							}
							//maintenant on peut dessiner le text
							g.DrawString(Text, this.uiAxeFont, Brushes.White, (float)TextLeft, (float)TextTop);

						}

					}

					//next iteration
					vxActual += GradDelta;
				}



				////axe vertical
				////pour dessiner les graduation vertical, on fait le même procédé que pour les graduation horizontale, mais on commence d'en haut de l'image.
				//si l'axe est visible à l'écran, cette variable indique s'il faut dessiner les graduation à gauche ou à droite de l'axe.
				//s'il n'est pas visible à l'écran, true c'est qu'il faut les afficher du côté droit, false qu'il faut les afficher du côté gauche.
				bool yWriteLeft = true;
				if (IsAxeYVisible)
				{
					yWriteLeft = uiOrigin.X > imgWidth / 4;
				}
				else
				{
					yWriteLeft = this.vposx < 0d;
				}


				long vyStart = ((long)(vUpLeft.y / dblGradDelta) * GradDelta) + (2L * GradDelta);
				//on parcourt toute les graduation vertical jusqu'au bas de l'écran
				long vyActual = vyStart;
				while (true)
				{
					//on obtien la position graphique vertical de la graduation
					int uiy = this.Convert_VirtualToUI(vUpLeft.x, (double)vyActual).Y;
					//si ca dépasse le bas de l'image, on a terminé
					if (uiy >= imgHeight) { break; }

					//on procède au dessin de la graduation si la ligne est dans l'image
					if (uiy >= 0)
					{
						//on dessine la graduation verticale, qui est une ligne horizontale
						g.DrawLine(Pens.Black, 0, uiy, imgWidth - 1, uiy);

						////maintenant on dessine le numéro de la graduation, ce qui dépend de si l'axe est dans l'écran ou non
						if (IsAxeYVisible)
						{
							//on prépare le text et on prépare aussi sa position
							string Text = vyActual.ToString();
							SizeF TextSizeF = g.MeasureString(Text, this.uiAxeFont);
							int TextTop = uiy - (int)(TextSizeF.Height / 2f);
							//maintenant on prépare la position horizontale du text, qui dépend de quel côté de l'axe il faut le dessiner
							int TextLeft = uiOrigin.X + CocheLength;
							if (yWriteLeft)
							{
								//il faut dessiner le text à gauche
								TextLeft = uiOrigin.X - CocheLength - (int)(TextSizeF.Width);
								g.DrawLine(this.uiAxePen, uiOrigin.X, uiy, uiOrigin.X - CocheLength, uiy);
							}
							else //il faut dessiner le text à droite
							{
								g.DrawLine(this.uiAxePen, uiOrigin.X, uiy, uiOrigin.X + CocheLength, uiy);
							}
							//maintenant on peut dessiner la graduation
							g.DrawString(Text, this.uiAxeFont, Brushes.White, (float)TextLeft, (float)TextTop);

						}
						else //l'axe n'est pas dans l'image, il est à l'extérieur
						{
							//on prépare le text et on prépare aussi sa position
							string Text = vyActual.ToString();
							SizeF TextSizeF = g.MeasureString(Text, this.uiAxeFont);
							int TextTop = uiy - (int)(TextSizeF.Height / 2f);
							//maintenant on prépare la position horizontale du text
							int TextLeft = CocheLength;
							if (yWriteLeft)
							{
								//il faut dessiner le text du côté droit
								TextLeft = imgWidth - CocheLength - (int)(TextSizeF.Width);
								g.DrawLine(this.uiAxePen, imgWidth, uiy, imgWidth - CocheLength, uiy);
							}
							else //il faut dessiner le text du côté gauche
							{
								g.DrawLine(this.uiAxePen, 0, uiy, CocheLength, uiy);
							}
							//maintenant on peut dessiner le text
							g.DrawString(Text, this.uiAxeFont, Brushes.White, (float)TextLeft, (float)TextTop);

						}


					}

					//next iteration
					vyActual -= GradDelta;
				}

			}
			catch
			{

			}



			//////dessine les axe
			try
			{
				if (IsAxeXVisible)
				{
					int oy = uiOrigin.Y;
					if (oy >= 0 && oy < imgHeight)
					{
						g.DrawLine(this.uiAxePen, 0, uiOrigin.Y, imgWidth - 1, uiOrigin.Y);
					}
				}
				if (IsAxeYVisible)
				{
					int ox = uiOrigin.X;
					if (ox >= 0 && ox < imgWidth)
					{
						g.DrawLine(this.uiAxePen, uiOrigin.X, 0, uiOrigin.X, imgHeight - 1);
					}
				}
			}
			catch { }




		}


		//va dessiner cette draw function sur l'image en faisant exécuter son calcul pour chaque coordonné x de l'image
		//RecomputeFunction indique s'il faut recalculer les valeur de la fonction.
		//si les valeur de la fonction ne sont pas à recalculer, alors les point déjà calculé seront utilisé.
		//dans ce cas, LinkComputedPoint indique s'il faut lier les point déjà calculé par une ligne, ou s'il faut les laisser déttaché et juste dessiner un point.
		private void srf_DrawThisFunction(DrawFunction DF, Bitmap img, Graphics g, bool RecomputeFunction, bool LinkComputedPoint)
		{
			int imgWidth = img.Width;
			int imgHeight = img.Height;

			//la façon qu'on va procéder pour dessiner les fonction dépend de s'il faut la recalculer pour chaque position horizontale, ou s'il faut utiliser les computed point
			if (RecomputeFunction)
			{
				//on retire tout les computed points précédant
				DF.ClearComputedPoints();

				int lastuiy = -1; //coordonné graphique y dans la colonne x précédante
				bool IsLastOk = false; //indique si le précédant point était valide

				//on calcul puis dessine tout les point pour toute les coordonné x
				for (int uix = 0; uix < imgWidth; uix++)
				{
					//on obtien la coordonné x virtuel actuel
					double vx = this.Convert_UIToVirtual(uix, 0).x;

					//on fait calculer la fonction
					sCompiledEquationResult rep = DF.ComputeAtX(vx);

					//s'il n'y a pas eu d'erreur, on dessine le point s'il est dans l'écran
					if (!rep.AnErrorOccurred)
					{
						//on ajoute la coordonné à la list des computed point
						DF.AddComputedPoint(vx, rep.TheResult);

						//convertie la coordonné y virtuel en coordonné graphique
						int uiy = this.Convert_VirtualToUI(0d, rep.TheResult).Y;
						//make sure qu'il n'est pas trop loin. ca sert à rien de dessier une ligne qui part du milieu jusqu'à y=-10000 si on peut juste la faire jusqu'à y=-5
						if (uiy < -5) { uiy = -5; }
						if (uiy > imgHeight + 5) { uiy = imgHeight + 5; }
						
						//if (uiy >= 0 && uiy < imgHeight)
						//{
						//	img.SetPixel(uix, uiy, DF.Color);
						//}
						if (IsLastOk)
						{
							try
							{
								g.DrawLine(DF.Pen, uix - 1, lastuiy, uix, uiy);
							}
							catch { }
						}
						
						lastuiy = uiy;
					}

					IsLastOk = !rep.AnErrorOccurred;
				}
			}
			else //il ne faut pas recalculer la fonction
			{
				//pour afficher les fonction à partir de leur computed point, selon le choix de l'user, soit on trace des ligne de point en point, soit on affiche les point
				if (!LinkComputedPoint)
				{
					//on dessine chaque point calculé
					foreach (PointD p in DF.listComputedPoints)
					{
						Point uipos = this.Convert_VirtualToUI(p);

						//on dessine le point seulement s'il est dans l'image
						if (uipos.X >= 0 && uipos.X < imgWidth)
						{
							if (uipos.Y >= 0 && uipos.Y < imgHeight)
							{
								try
								{
									img.SetPixel(uipos.X, uipos.Y, DF.Color);
								}
								catch { }
							}
						}

					}
				}
				else //on doit tracer des ligne de point en point
				{

					//avant de procéder au dessin, il faut trouver la plus petite distance horizontale qui sépare deux computed point
					double delta = 100d; if (DF.listComputedPoints.Count >= 2) { delta = DF.listComputedPoints[1].x - DF.listComputedPoints[0].x; }
					int index = 1;
					while (index < DF.listComputedPoints.Count)
					{
						double distx = DF.listComputedPoints[index].x - DF.listComputedPoints[index - 1].x;
						if (distx < delta) { delta = distx; }
						//next iteration
						index++;
					}
					
					bool IsLastExist = false; //indique s'il existe un point précédant
					int lastuiy = -1; //coordonné graphique y du point précédant
					int lastuix = -1; //coordonné graphique x du point précédant
					PointD lastvpos = new PointD(double.NaN, double.NaN); //le computed point précédant

					//à l'intérieur de la list, les point sont déjà censé être dans le bon ordre
					foreach (PointD vp in DF.listComputedPoints)
					{
						//on vérifie que la distance entre notre point actuel et le point précédant n'est pas trop grande, ce qui signifirait qu'il ne faut pas les lier entre eux
						bool IsLastOk = false;
						if (IsLastExist)
						{
							//on considère que deux computed point sont suffisament proche si la distance horizontale entre eux est inférieur à 1.5*distance minimal entre deux computed point
							IsLastOk = vp.x - lastvpos.x < 1.5d * delta;
						}

						//on obtien la position graphique du point actuel
						Point uipos = this.Convert_VirtualToUI(vp);
						//make sure qu'il n'est pas trop loin. ca sert à rien de dessier une ligne qui part du milieu jusqu'à y=-10000 si on peut juste la faire jusqu'à y=-5
						if (uipos.Y < -5) { uipos.Y = -5; }
						if (uipos.Y > imgHeight + 5) { uipos.Y = imgHeight + 5; }


						//on dessine la ligne si la distance n'est pas trop grande
						if (IsLastOk)
						{
							//on dessine la ligne seulement si la coordonné x est dans l'écran
							if (uipos.X >= 0 || uipos.X < imgWidth)
							{
								g.DrawLine(DF.Pen, lastuix, lastuiy, uipos.X, uipos.Y);
							}
						}

						//next iteration
						lastvpos = vp;
						lastuix = uipos.X;
						lastuiy = uipos.Y;
						IsLastExist = true;
					}



				}

			}
			

		}

		//lorsque le «recalcul automatique des fonction après déplacement» est désactivé, cette void recalcul des computed point pour la DF donné, pour chaque coordonné horizontale de l'écran
		private void RecomputePointsDF(DrawFunction DF)
		{
			int imgWidth = this.propImgWidth;

			//on efface tout les computed point précédant
			DF.ClearComputedPoints();

			int uix = 0; //coordonné horizontale graphique actuel
			while (uix < imgWidth)
			{
				//on obtien la coordonné virtuel x actuel
				double vx = this.Convert_UIToVirtual(uix, 0).x;

				//on fait calculer la fonction pour notre position vx
				sCompiledEquationResult rep = DF.ComputeAtX(vx);

				//check s'il y a eu une erreur dans le calcul
				if (!rep.AnErrorOccurred)
				{
					//s'il n'y a pas d'erreur, on ajoute cette coordonné à la liste des computed point
					DF.AddComputedPoint(vx, rep.TheResult);

				}
				
				//next iteration
				uix++;
			}
		}





		#endregion
		#region gestion de l'interface, et des paramètres décidés par l'utilisateur

		//indique s'il faut recalculer les fonctions après chaques déplacements
		private bool propRecomputeAllAfterEveryMove
		{
			get { return this.interCbAutoRefresh.Checked; }
		}

		//indique s'il faut, à l'affichage, lier les computed point entre eux.
		private bool propLinkComputedP
		{
			get { return this.interCbLinkComputedP.Checked; }
		}


		private uiForm interFormMenu;
		private uiCheckBox interCbAutoRefresh; //le checkbox pour décider s'il faut recalculer automatiquement les fonction après chaques déplacements
		private uiButton interBtnRefresh; //si le «refresh automatique après déplacement» est désactiver, ce button fait recalculer les fonction pour la position actuel de l'écran
		private uiCheckBox interCbLinkComputedP; //lorsqu'on dessine les computed point d'une fonction, indique s'il faut dessiner les lien entre les point, ou s'il faut juste afficher les point là où ils sont


		//une fenêtre qui offre à l'user des option pour le déplacement
		private uiForm interFormControls;
		private uiLabel interLblZoom;
		private uiButton interBtnZoomIn;
		private uiButton interBtnZoomOut;



		private void CreateInterface()
		{
			////on commence par créer le menu
			this.interFormMenu = new uiForm(this);
			this.interFormMenu.Top = 1 + this.uiFTitleHeight;
			this.interFormMenu.Left = 1;
			this.interFormMenu.Title = "Menu (E)";
			this.interFormMenu.Width = 300;
			this.interFormMenu.Height = 300;
			this.interFormMenu.UserCanClose = false;
			this.interFormMenu.UserCanMoveForm = false;
			this.interFormMenu.Opened = false;
			this.interFormMenu.Show();

			//le checkbox pour décider s'il faut recalculer automatiquement les fonction après chaque déplacement
			this.interCbAutoRefresh = new uiCheckBox(this.interFormMenu);
			this.interCbAutoRefresh.Top = 2;
			this.interCbAutoRefresh.Left = 3;
			this.interCbAutoRefresh.Text = "Recalculer les fonctions après\nchaques déplacements";
			this.interCbAutoRefresh.Checked = true;

			//le button qui fait recalculer les fonction à la demande
			this.interBtnRefresh = new uiButton(this.interFormMenu);
			this.interBtnRefresh.Left = this.interCbAutoRefresh.Left;
			this.interBtnRefresh.Top = this.interCbAutoRefresh.Top + this.interCbAutoRefresh.Height + 1;
			this.interBtnRefresh.SetSize(120, 40);
			this.interBtnRefresh.Text = "Recalculer les\nfonctions (C)";
			this.interBtnRefresh.MouseLeftUp += new EventHandler(this.interBtnRefresh_MouseLeftUp);

			this.interCbLinkComputedP = new uiCheckBox(this.interFormMenu);
			this.interCbLinkComputedP.Left = this.interBtnRefresh.Left + this.interBtnRefresh.Width + 2;
			this.interCbLinkComputedP.Top = this.interBtnRefresh.Top;
			this.interCbLinkComputedP.Text = "Lier les points\ncalculés";
			this.interCbLinkComputedP.Checked = true;
			this.interCbLinkComputedP.StateChanged += new EventHandler((o, e) =>
			{
				if (!this.propRecomputeAllAfterEveryMove)
				{
					this.srefreshFunctions();
				}
			});



			////on crée la form qui offre des option de déplacement
			this.interFormControls = new uiForm(this);
			this.interFormControls.Title = "Déplacement";
			this.interFormControls.Width = 200;
			this.interFormControls.Height = 100;
			this.interFormControls.UserCanClose = false;
			this.interFormControls.UserCanMoveForm = false;
			this.interFormControls.Opened = false;
			this.interFormControls.Show();

			//on crée les control du zoom. le mouse wheel ne fonctionne peut être pas sur windows 7 ou 8 donc sur ces OS c'est la seul facon de contrôler le zoom
			this.interLblZoom = new uiLabel(this.interFormControls);
			this.interLblZoom.Text = "Zoom :";
			this.interLblZoom.Top = 15;
			this.interLblZoom.Left = 10;

			this.interBtnZoomIn = new uiButton(this.interFormControls);
			this.interBtnZoomIn.Text = "+";
			this.interBtnZoomIn.SetSize(50, 30);
			this.interBtnZoomIn.Left = this.interLblZoom.Left + this.interLblZoom.Width + 5;
			this.interBtnZoomIn.Top = this.interLblZoom.Top + (this.interLblZoom.Height / 2) - (this.interBtnZoomIn.Height / 2);
			this.interBtnZoomIn.MouseLeftDown += new EventHandler((o, e) =>
			{
				this.VirtualHeight /= 1.3d;
				this.srefreshFunctions();
			});

			this.interBtnZoomOut = new uiButton(this.interFormControls);
			this.interBtnZoomOut.Text = "-";
			this.interBtnZoomOut.SetSize(50, 30);
			this.interBtnZoomOut.Left = this.interBtnZoomIn.Left + this.interBtnZoomIn.Width + 2;
			this.interBtnZoomOut.Top = this.interBtnZoomIn.Top;
			this.interBtnZoomOut.MouseLeftDown += new EventHandler((o, e) =>
			{
				this.VirtualHeight *= 1.3d;
				this.srefreshFunctions();
			});

			uiButton btnMoveToOrigin = new uiButton(this.interFormControls);
			btnMoveToOrigin.SetSize(180, 40);
			btnMoveToOrigin.Top = this.interBtnZoomIn.Top + this.interBtnZoomIn.Height + 5;
			btnMoveToOrigin.Left = 10;
			btnMoveToOrigin.Text = "Retourner à l'origine\ndu plan";
			btnMoveToOrigin.MouseLeftDown += new EventHandler((o, e) =>
			{
				this.vposx = 0d;
				this.vposy = 0d;
				this.VirtualHeight = 11d;
				this.srefreshFunctions();
			});




		}
		private void ResizeInterface()
		{
			//on repositionne la form des controle
			this.interFormControls.Top = 1 + this.uiFTitleHeight;
			this.interFormControls.Left = this.Width - 4 - this.interFormControls.Width;






		}

		private void interBtnRefresh_MouseLeftUp(object sender, EventArgs e)
		{
			//make sure que le «recalcul automatique des fonctions après déplacement» est désactivé
			if (!this.propRecomputeAllAfterEveryMove)
			{
				//on fait recalculer les point pour chaque drawfunction
				foreach (DrawFunction DF in this.VC.listFunctions)
				{
					this.RecomputePointsDF(DF);
				}
				//on fait refresher l'écran
				this.srefreshFunctions();
			}
		}



		#endregion
		#region interface (form, button, label, ...)

		//la liste de toute les uiForm présentent dans this
		private List<uiForm> listForm = new List<uiForm>();



		//retourne si la souris est sur n'importe quel control graphique
		private bool IsMouseOnAnyForm()
		{
			Rectangle mrec = this.MouseRec;
			foreach (uiForm f in this.listForm)
			{
				//on commence par checker si la uiForm est visible
				if (f.Visible)
				{
					//on check le title de la fenêtre
					if (this.IsPointOnFormTitle(f, mrec.X, mrec.Y)) { return true; }
					//on check si la souris est dans la zone cliente, si la zone cliente n'est pas caché
					if (f.Opened)
					{
						if (f.rec.IntersectsWith(mrec)) { return true; }
					}
				}
			}

			return false;
		}



		private bool IsMouseLeftDownOnForm = false; //devient true si la souris est en train d'intéragir avec une form.
		
		//pendant mouse left down, s'il s'est avéré que la souris était sur une form, alors cette void prend en charge l'exécution des event ou le lancement du déplacement des form
		private void Check_MouseLeftDown()
		{
			this.IsMouseLeftDownOnForm = true;

			Rectangle mrec = this.MouseRec;
			//il faut parcourir la liste par la fin puisque les forme en avant plan sont à la fin
			int index = this.listForm.Count - 1;
			while (index >= 0)
			{
				uiForm f = this.listForm[index];
				//on check si la form est actuellement visible
				if (f.Visible)
				{
					//on check si la souris est sur la title bar
					if (this.IsPointOnFormTitle(f, mrec.X, mrec.Y))
					{
						//check si l'user a "clické" sur le button pour cacher ou afficher la zone cliente
						if (mrec.X < f.Left + this.uiFTitleHeight)
						{
							//on toggle l'état visible de la form
							if (f.Opened) { f.Close(); }
							else { f.Open(); }
							//this.srefreshInterface();
						}//maintenant on check si l'user a clické sur le button close
						else if (f.UserCanClose && mrec.X < f.Left + (2 * this.uiFTitleHeight))
						{
							f.UserClickedClose();
						}
						else //si la souris n'était pas sur le button de la title bar, alors l'user veut déplacer la form
						{
							//check si l'user a le droit de déplacer la form
							if (f.UserCanMoveForm)
							{
								this.StartUserMovingForm(f, mrec.X, mrec.Y);
							}
							else
							{
								//si l'user ne peut pas déplacer la form, alors on toggle l'état affiché de la zone cliente de la form.
								//ca augmente légèrement l'ergonomie de l'ouverture des form n'ayant pas de shortcut pour les ouvrir. ca augmentre la zone de l'écran pouvant faire afficher la zone cliente avec la souris.
								//on toggle l'état visible de la form
								if (f.Opened) { f.Close(); }
								else { f.Open(); }
							}

						}
						f.SendToTop();
						this.srefreshInterface();
						break;
					}
					//si la zone cliente n'est pas caché, on check si la souris est dans la zone cliente
					if (f.Opened)
					{
						if (f.rec.IntersectsWith(mrec))
						{

							//on envoie à la form les coordonné de la souris et elle va vérifier s'il y a des contrôles dessous la souris
							f.Check_MouseLeftDown(mrec.X - f.Left, mrec.Y - f.Top);
							
							f.SendToTop();
							this.srefreshInterface();
							break;
						}
					}
				}

				//next iteration
				index--;
			}

		}
		private void Check_MouseLeftUp()
		{
			foreach (uiForm f in this.listForm)
			{
				f.Check_MouseLeftUp();
			}

			this.IsMouseLeftDownOnForm = false;
		}





		#region private measure string
		//un raccourci pour obtenir la taille d'une chaine de text
		private Graphics zzzg = Graphics.FromImage(new Bitmap(10, 10));
		private SizeF privateMeasureString(string text, Font font)
		{
			return this.zzzg.MeasureString(text, font);
		}
		#endregion
		#region state moving uiForm

		private uiForm statemfForm = null; //la form actuellement en déplacement
		private Point statemfDelta = new Point(0, 0); //la différence de position entre la souris et le coin supérieur gauche de la zone cliente de la form

		private void StartUserMovingForm(uiForm f, int mouseX, int mouseY)
		{
			this.statemfForm = f;
			this.statemfDelta = new Point(mouseX - f.Left, mouseY - f.Top);
			this.ActualStade = Stade.MovingForm;
		}

		private void StopUserMovingForm()
		{
			Point mpos = this.MousePos;
			this.statemfForm.Left = mpos.X - this.statemfDelta.X;
			this.statemfForm.Top = mpos.Y - this.statemfDelta.Y;
			this.statemfForm.MakeSureTitleIsInBound();
			this.statemfForm = null;
			this.ActualStade = Stade.none;
			this.srefreshInterface();
		}

		#endregion



		private int uiFTitleHeight = 30; //height de la "barre de titre" dessus la zone cliente d'une uiForm
		private Font uiFTitleFont = new Font("consolas", 10f); //font utilisé pour dessiner le title des uiForm
		private Pen uiFTitleBtnPen = new Pen(Color.White, 3f); //pen utilisé pour dessiner le - ou le + sur le button qui permet d'ouvrir ou fermer la zone cliente d'une uiForm


		private Brush uiBtnBackBrush = Brushes.DimGray; //back color d'un uiButton lorsque la souris est ailleur
		private Brush uiBtnDownBackBrush = new SolidBrush(Color.FromArgb(64, 64, 64)); //back color d'un uiButton lorsque mouse left est down




		//fonction raccourci pour savoir si un point est graphiquement situé dessus le titre d'une uiForm
		private bool IsPointOnFormTitle(uiForm f, int px, int py)
		{
			if (py <= f.Top && py >= f.Top - this.uiFTitleHeight)
			{
				if (px >= f.Left && px <= f.Left + f.TitleWidth)
				{
					return true;
				}
			}
			return false;
		}


		//s'ajout automatiquement à son parent uiVirtualContextViewer
		private class uiForm
		{
			private uiVirtualContextViewer zzzParent = null;
			public uiVirtualContextViewer Parent
			{
				get
				{
					return this.zzzParent;
				}
				set
				{
					//si nous avons déjà un parent, il faut se retirer de lui
					if (this.HasParent) { this.RemoveFromParent(); }

					//on défini le nouveau parent
					this.zzzParent = value;
					//maintenant on s'ajoute à l'intérieur du parent
					if (value != null)
					{
						value.listForm.Add(this);
					}
				}
			}

			public bool HasParent { get { return this.zzzParent != null; } }
			//fait se retirer de son parent, la form ne sera plus là.
			public void RemoveFromParent()
			{
				if (this.HasParent)
				{
					this.Parent.listForm.Remove(this);
					this.zzzParent = null;
				}
			}



			//les liste de tout les controles présent dans la form
			public List<uiLabel> listLabel = new List<uiLabel>();
			public List<uiButton> listButton = new List<uiButton>();
			public List<uiTextBox> listTextBox = new List<uiTextBox>();
			public List<uiCheckBox> listCheckBox = new List<uiCheckBox>();


			//envoie this à la fin de la liste des form pour être en premier plan
			public void SendToTop()
			{
				if (this.HasParent)
				{
					this.Parent.listForm.Remove(this);
					this.Parent.listForm.Add(this);
				}
			}

			public bool Visible = false; //indique si la form est actuellement visible à l'image, peu importe qu'elle soit ouverte ou non.
			public bool Opened = true; //indique si la form est ouverte ou réduite.

			//visible
			public void Show()
			{
				this.Visible = true;
			}
			public void Hide()
			{
				this.Visible = false;
			}
			//opened
			public void Open()
			{
				this.Opened = true;
			}
			public void Close()
			{
				this.Opened = false;
			}



			private string zzzTitle = "";
			public string Title
			{
				get { return this.zzzTitle; }
				set
				{
					this.zzzTitle = value;
					//maintenant on recalcul la largeur du title de la fenêtre
					this.zzzTitleTextWidth = (int)(this.Parent.privateMeasureString(value, this.Parent.uiFTitleFont).Width);
				}
			}

			/* le rectangle "rec" est le rectangle qui représente la zone cliente. la zone cliente est la zone dans laquel il est possible de voir les
			 * controles qui composent la uiForm. le "title" de la forme est dessiné au dessus de la zone cliente. le height d'un title de uiForm est
			 * défini dans la variable "uiFTitleHeight" qui se trouve dans le parent de this. il revien cependant à this de calculer la largeur de son
			 * title.
			 * 
			 * le title est toujours le plus à gauche possible au dessus de la zone cliente, de sorte que le côté gauche du title soit horizontalement
			 * aligné avec le côté gauche de la zone cliente.
			 * 
			 * le title est composé de 2 choses. de gauche à droite :
			 * - un bouton carré qui sert à ouvrir ou fermer la uiForm. (ne pas oublier que dans uiVirtualContextViewer, ouvrire/fermer une uiForm signifie la réduire (cacher la zone cliente) ou rendre la zone cliente visible).
			 * - le texte qui compose le title de la uiForm.
			 * 
			 * les propriétés top left width height concernent la zone cliente.
			 * 
			 */
			private int zzzTitleTextWidth = 0;
			public int TitleWidth
			{
				get
				{
					int rep = 0;
					rep += this.Parent.uiFTitleHeight; //on ajoute la largeur du button qui sert à ouvrire ou fermer la fenetre
					if (this.UserCanClose) { rep += this.Parent.uiFTitleHeight; } //on ajoute la largeur du button close s'il est là
					rep += 2;
					rep += this.zzzTitleTextWidth; //on ajoute la largeur du text du title
					rep += 2;
					if (rep < 150) { rep = 150; }
					return rep;
				}
			}
			public Rectangle rec = new Rectangle(10, 50, 150, 150); //rectangle de la zone cliente
			public int Top
			{
				get { return this.rec.Y; }
				set { this.rec.Y = value; }
			}
			public int Left
			{
				get { return this.rec.X; }
				set { this.rec.X = value; }
			}
			public int Width
			{
				get { return this.rec.Width; }
				set { this.rec.Width = value; }
			}
			public int Height
			{
				get { return this.rec.Height; }
				set { this.rec.Height = value; }
			}

			public bool UserCanMoveForm = true; //si false, l'user ne peut pas déplacer la form
			public bool UserCanClose = true; //si true, il y a dans la title bar un button qui sert à fermer la fenêtre pour de bon

			//si nécéssaire, déplace la form pour que la title bar soit dans les borne de l'image
			public void MakeSureTitleIsInBound()
			{
				if (this.Left < 2) { this.Left = 2; }
				if (this.Top < this.Parent.uiFTitleHeight + 2) { this.Top = this.Parent.uiFTitleHeight + 2; }
				if (this.Left + this.TitleWidth + 2 > this.Parent.propImgWidth) { this.Left = this.Parent.propImgWidth - 2 - this.TitleWidth; }
				if (this.Top + 2 > this.Parent.propImgHeight) { this.Top = this.Parent.propImgHeight - 2; }

			}


			public Color BackColor = Color.Black;



			public uiForm(uiVirtualContextViewer sParent)
			{
				this.zzzParent = sParent;
				this.Parent.listForm.Add(this); //s'ajout automatiquement à son parent uiVirtualContextViewer

				this.Title = "notitle";

			}


			//il faut donner les coordonné local
			public void Check_MouseLeftDown(int mx, int my)
			{
				Rectangle mrec = new Rectangle(mx, my, 1, 1);


				//l'event ne peut être recu que par un seul control. si true, il faut arrêter de scanner les controle potentiellement dessous la souris
				bool EventRaised = false;

				//check si y'a un button dessous la souris
				foreach (uiButton b in this.listButton)
				{
					//check si le button est visible
					if (b.Visible)
					{
						//check si la souris est dessus le button
						if (b.rec.IntersectsWith(mrec))
						{
							//on lui dit que l'user fait mouse left down dessus lui, puis après on quitte
							b.Check_MouseLeftDown();
							EventRaised = true; //on indique que l'event a été donné à un controle
							break;
						}
					}
				}

				//check si y'a un textbox sous la souris
				if (!EventRaised)
				{
					foreach (uiTextBox tb in this.listTextBox)
					{
						if (tb.Visible)
						{
							//check si la souris est dessus
							if (tb.rec.IntersectsWith(mrec))
							{
								//on lui dit que mouse left est down sur lui
								tb.Check_MouseLeftDown();
								EventRaised = true;
								break;
							}
						}
					}

				}

				//check si y'a un checkbox sous la souris
				if (!EventRaised)
				{
					foreach (uiCheckBox cb in this.listCheckBox)
					{
						if (cb.Visible)
						{
							//check si la souris est dessus
							if (cb.Left < mrec.X && cb.Top < mrec.Y && mrec.X < cb.Left + cb.Width && mrec.Y < cb.Top + cb.Height)
							{
								//on lui dit que l'user vient de clicker sur lui
								cb.Check_MouseLeftDown();
								EventRaised = true;
								break;
							}
						}
					}
				}


			}

			public void Check_MouseLeftUp()
			{
				foreach (uiButton b in this.listButton)
				{
					b.Check_MouseLeftUp();
				}

			}


			//est raisé lorsque l'user kill la form
			public event EventHandler FormClosed;
			private void Raise_FormClosed()
			{
				if (this.FormClosed != null)
				{
					this.FormClosed(this, new EventArgs());
				}
			}

			//est callé lorsque l'user a clické sur le button close de la title bar
			public void UserClickedClose()
			{
				//on se retire du parent
				this.RemoveFromParent();
				//on raise l'event
				this.Raise_FormClosed();

			}


		}
		private class uiLabel
		{
			private uiForm zzzParent = null;
			public uiForm Parent { get { return this.zzzParent; } }

			public bool Visible = true;

			public int Top = 0;
			public int Left = 0;

			private int zzzWidth = -1;
			private int zzzHeight = -1;
			public int Width { get { return this.zzzWidth; } }
			public int Height { get { return this.zzzHeight; } }

			private string zzzText = "";
			public string Text
			{
				get { return this.zzzText; }
				set
				{
					this.zzzText = value;
					SizeF tsf = this.Parent.Parent.privateMeasureString(value, this.font);
					this.zzzWidth = (int)(tsf.Width);
					this.zzzHeight = (int)(tsf.Height);
				}
			}

			private Font zzzfont = new Font("consolas", 10f);
			public Font font
			{
				get { return this.zzzfont; }
				set
				{
					this.zzzfont = value;
					SizeF tsf = this.Parent.Parent.privateMeasureString(this.Text, value);
					this.zzzWidth = (int)(tsf.Width);
					this.zzzHeight = (int)(tsf.Height);
				}
			}

			public Brush ForeBrush = Brushes.Silver;
			
			public uiLabel(uiForm sParent)
			{
				this.zzzParent = sParent;
				this.Parent.listLabel.Add(this);

				this.Text = "notext";

			}

		}
		private class uiButton
		{
			private uiForm zzzParent = null;
			public uiForm Parent { get { return this.zzzParent; } }

			public bool Visible = true;
			public bool Enabled = true;

			public Rectangle rec = new Rectangle(10, 50, 150, 150); //rectangle de la zone cliente
			public int Top
			{
				get { return this.rec.Y; }
				set { this.rec.Y = value; }
			}
			public int Left
			{
				get { return this.rec.X; }
				set { this.rec.X = value; }
			}
			public int Width
			{
				get { return this.rec.Width; }
				set { this.rec.Width = value; }
			}
			public int Height
			{
				get { return this.rec.Height; }
				set { this.rec.Height = value; }
			}
			public void SetSize(int newWidth, int newHeight)
			{
				this.Width = newWidth;
				this.Height = newHeight;
			}

			public string Text = "notext";
			public Font font = new Font("consolas", 10f);


			public uiButton(uiForm sParent)
			{
				this.zzzParent = sParent;
				this.Parent.listButton.Add(this);


			}

			public event EventHandler MouseLeftDown;
			public event EventHandler MouseLeftUp;
			private void Raise_MouseLeftDown()
			{
				if (this.MouseLeftDown != null)
				{
					this.MouseLeftDown(this, new EventArgs());
				}
			}
			private void Raise_MouseLeftUp()
			{
				if (this.MouseLeftUp != null)
				{
					this.MouseLeftUp(this, new EventArgs());
				}
			}



			public bool isMouseLeftDown = false; //indique si mouse left est down sur this. n'a qu'une utilité graphique
			

			//est callé seulement au contrôle qui se situe dessous la souris
			public void Check_MouseLeftDown()
			{
				this.isMouseLeftDown = true;
				if (this.Enabled)
				{
					this.Raise_MouseLeftDown();
				}
			}

			//est envoyé à tout les contrôles qui ont des events, peu importe que la souris avait clické dessus eux ou non
			public void Check_MouseLeftUp()
			{
				if (this.isMouseLeftDown)
				{
					if (this.Enabled)
					{
						this.Raise_MouseLeftUp();
					}
				}
				this.isMouseLeftDown = false;
			}


		}
		private class uiTextBox
		{
			private uiForm zzzParent = null;
			public uiForm Parent { get { return this.zzzParent; } }

			public bool Visible = true;

			public Rectangle rec = new Rectangle(10, 50, 150, 30); //rectangle de la zone cliente
			public int Top
			{
				get { return this.rec.Y; }
				set { this.rec.Y = value; }
			}
			public int Left
			{
				get { return this.rec.X; }
				set { this.rec.X = value; }
			}
			public int Width
			{
				get { return this.rec.Width; }
				set { this.rec.Width = value; }
			}
			public int Height
			{
				get { return this.rec.Height; }
				set { this.rec.Height = value; }
			}
			public void SetSize(int newWidth, int newHeight)
			{
				this.Width = newWidth;
				this.Height = newHeight;
			}

			public string Text = "";
			public Font font = new Font("consolas", 10f);

			public uiTextBox(uiForm sParent)
			{
				this.zzzParent = sParent;
				this.Parent.listTextBox.Add(this);



			}


			public event EventHandler TextChanged;
			private void Raise_TextChanged()
			{
				if (this.TextChanged != null)
				{
					this.TextChanged(this, new EventArgs());
				}
			}


			//si mouse left est down, on fait juste permettre à l'user de modifier le contenu du textbox
			public void Check_MouseLeftDown()
			{
				////on crée une form qui permet à l'user de modifier le contenu du textbox
				Form forme = new Form();
				forme.StartPosition = FormStartPosition.Manual;
				forme.MinimumSize = new Size(1, 1);
				forme.MinimizeBox = false;
				forme.MaximizeBox = false;
				forme.FormBorderStyle = FormBorderStyle.FixedDialog;
				forme.ShowIcon = false;
				forme.ShowInTaskbar = false;
				forme.Size = new Size(250, 110);

				//crée le textbox qui contiendra le text
				TextBox tb = new TextBox();
				tb.Parent = forme;
				tb.Top = 1;
				tb.Left = 1;
				tb.Width = forme.Width - 17 - tb.Left;
				tb.Font = new Font("consolas", 15f);
				tb.Text = this.Text;
				try { tb.Select(tb.Text.Length, 0); } catch { } //un try catch juste au cas où
				tb.KeyDown += new KeyEventHandler((o, e) =>
				{
					if (e.KeyCode == Keys.Enter)
					{
						this.Text = tb.Text; //on défini le nouveau text
						forme.Close();
						this.Raise_TextChanged();
					}
				});
				
				//on crée le button "ok"
				Button bOk = new Button();
				bOk.Parent = forme;
				bOk.Height = forme.Height - 40 - tb.Top - tb.Height;
				bOk.Width = 70;
				bOk.Top = tb.Top + tb.Height + 1;
				bOk.Left = forme.Width - 16 - bOk.Width;
				bOk.Text = "Ok";
				bOk.Click += new EventHandler((o, e) =>
				{
					this.Text = tb.Text; //on défini le nouveau text
					forme.Close();
					this.Raise_TextChanged();
				});

				//crée le button cancel
				Button bCancel = new Button();
				bCancel.Parent = forme;
				bCancel.Height = bOk.Height;
				bCancel.Width = bOk.Width;
				bCancel.Top = bOk.Top;
				bCancel.Left = bOk.Left - 1 - bCancel.Width;
				bCancel.Text = "Cancel";
				bCancel.Click += new EventHandler((o, e) =>
				{
					forme.Close();
				});

				
				//on centre la form autour de la souris
				Point mpos = Cursor.Position;
				forme.Top = mpos.Y - (forme.Height / 2);
				forme.Left = mpos.X - (forme.Width / 2);

				//on fait afficher la form
				forme.ShowDialog();
				
			}


		}
		private class uiCheckBox
		{
			private uiForm zzzParent = null;
			public uiForm Parent { get { return this.zzzParent; } }

			public bool Visible = true;

			public bool Checked = false;


			public int Top = 0;
			public int Left = 0;

			private int zzzWidth = -1;
			private int zzzHeight = -1;
			public int Width { get { return this.zzzWidth; } }
			public int Height { get { return this.zzzHeight; } }

			
			//cett variable est publique, mais n'est pas destiné à être modifié à aucun moment pendant l'execution
			public int uiBoxWidth = 20; //largeur d'un des côté du carré de la check box


			private string zzzText = "";
			public string Text
			{
				get { return this.zzzText; }
				set
				{
					this.zzzText = value;
					SizeF tsf = this.Parent.Parent.privateMeasureString(value, this.font);

					this.zzzWidth = (int)(this.uiBoxWidth + tsf.Width);

					int newheight = this.uiBoxWidth;
					if ((int)(tsf.Height) > newheight) { newheight = (int)(tsf.Height); }
					this.zzzHeight = newheight;
				}
			}

			private Font zzzfont = new Font("consolas", 10f);
			public Font font
			{
				get { return this.zzzfont; }
				set
				{
					this.zzzfont = value;
					SizeF tsf = this.Parent.Parent.privateMeasureString(this.Text, value);

					this.zzzWidth = (int)(this.uiBoxWidth + tsf.Width);

					int newheight = this.uiBoxWidth;
					if ((int)(tsf.Height) > newheight) { newheight = (int)(tsf.Height); }
					this.zzzHeight = newheight;
				}
			}

			public Brush ForeBrush = Brushes.Silver;

			public uiCheckBox(uiForm sParent)
			{
				this.zzzParent = sParent;
				this.Parent.listCheckBox.Add(this);

				this.Text = "notext";

			}

			
			public event EventHandler StateChanged;
			private void Raise_StateChanged()
			{
				if (this.StateChanged != null)
				{
					this.StateChanged(this, new EventArgs());
				}
			}

			public void Check_MouseLeftDown()
			{
				//on toggle l'état checked
				this.Checked = !this.Checked;
				//on raise l'event
				this.Raise_StateChanged();

			}
			
		}



		private void interTESTEST()
		{
			uiForm f = new uiForm(this);
			f.Show();
			f.Top = 100;
			f.Width = 200;
			f.Height = 200;
			f.UserCanMoveForm = true;
			
			uiCheckBox cb = new uiCheckBox(f);
			cb.Left = 5;
			cb.Top = 5;
			cb.Text = "premier check box";
			cb.Checked = f.UserCanClose;
			cb.StateChanged += new EventHandler((o, e) =>
			{
				f.UserCanClose = cb.Checked;
			});




			uiForm f2 = new uiForm(this);
			f2.Show();
			f2.Top = 200;
			f2.Left = 200;
			f2.Width = 200;
			f2.Height = 200;
			f2.UserCanMoveForm = true;
			f2.Title = "la deuxième form";
			f2.UserCanClose = true;

			uiLabel l1 = new uiLabel(f2);
			l1.Top = 30;
			l1.Text = "le tout premier label";
			l1.Visible = true;


			uiButton b = new uiButton(f2);
			b.SetSize(100, 30);
			b.Left = 5;
			b.Top = l1.Top + l1.Height;
			b.Text = "set title";
			//b.MouseLeftDown += new EventHandler((o, e) => { Program.wdebug("btn 1 DOWN"); });
			//b.MouseLeftUp += new EventHandler((o, e) => { Program.wdebug("btn 1 UP"); });

			uiButton b2 = new uiButton(f2);
			b2.SetSize(70, 30);
			b2.Top = b.Top;
			b2.Left = b.Left + b.Width + 2;
			b2.MouseLeftDown += new EventHandler((o, e) =>
			{
				f.UserCanClose = !f.UserCanClose;
			});
			//b2.MouseLeftDown += new EventHandler((o, e) => { Program.wdebug("btn 2 DOWN"); });
			//b2.MouseLeftUp += new EventHandler((o, e) => { Program.wdebug("btn 2 UP"); });
			//b2.Enabled = false;
			
			uiTextBox t = new uiTextBox(f2);
			t.Left = 5;
			t.Top = b.Top + b.Height + 10;
			t.TextChanged += new EventHandler((o, e) =>
			{
				f2.Title = t.Text;
			});


			b.MouseLeftDown += new EventHandler((o, e) =>
			{
				f.Title = t.Text;
			});






		}

		#endregion




	}
}
