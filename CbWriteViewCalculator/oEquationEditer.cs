using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public sealed class oEquationEditer
	{
		private bool IsMouseLeftDown = false;
		private bool IsMouseLeftDownOnButton = false; //indique si le button gauche de la sourie a été enfoncé sur un button
		private Color TextCursorColor = Color.Red; //lorsque l'espèce de curseur textuel est affiché, cette variable indique sa couleur
		public Point MousePos { get { return this.ImageBox.PointToClient(Cursor.Position); } }
		public Rectangle MouseRec { get { return new Rectangle(this.MousePos, new Size(1, 1)); } }
		public Point MousePosBound = new Point(0, 0); //optien la position de la sourie dans les bound. elle ne peut pas etre sortie de this.ImageBox
		public Rectangle MouseRecBound { get { return new Rectangle(this.MousePosBound, new Size(1, 1)); } }


		//equation et etat de l'interface graphique
		private oEquation zzzTheEquation;
		public oEquation TheEquation { get { return this.zzzTheEquation; } }
		public void SetTheEquationToEdit(oEquation neweq)
		{
			this.zzzTheEquation = neweq;
			this.Refresh();
		}

		private Point EquationDrawPos = new Point(10, 40); // 10, 40
		private Point EquationMousePos
		{
			get
			{
				Point mpos = this.MousePos;
				return new Point(mpos.X - this.EquationDrawPos.X, mpos.Y - this.EquationDrawPos.Y);
			}
		}

		private bool zzzAnyDrawContextDefined = false;
		public bool AnyDrawContextDefined { get { return this.zzzAnyDrawContextDefined; } }
		private oEquationDrawContext zzzActualEquationDrawContext = null;
		public oEquationDrawContext ActualEquationDrawContext
		{
			get
			{
				//s'il n'y a pas de context de dessin defini, il retourne une erreur
				if (!this.AnyDrawContextDefined)
				{
					throw new Exception("no drawcontext defined");
				}
				else
				{
					return this.zzzActualEquationDrawContext;
				}
			}
			set
			{
				this.zzzActualEquationDrawContext = value;
				this.zzzAnyDrawContextDefined = true;
				this.Refresh();
			}
		}


		public enum UiStade
		{
			none,
			View, //l'equation peut etre afficher
			Adding, //l'utilisateur est en train d'ajouter un object a l'equation
			Moving, //tres similaire à Adding
			Selecting, // le button gauche de la sourie est enfoncé
		}
		private UiStade zzzActualUiStade = UiStade.none;
		public UiStade ActualUiStade { get { return this.zzzActualUiStade; } }

		//autre
		private PictureBox ImageBox = new PictureBox();


		public bool DoubleClickEnabled = true; //permet de désactiver la possibilité de modifier immédiatement le nom d'un élément en faisant un double click dessus. au cas où la fonctionalité n'est pas encore "stable", cette variable permet de la désactiver



		//property
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
		public AnchorStyles Anchor
		{
			get { return this.ImageBox.Anchor; }
			set { this.ImageBox.Anchor = value; }
		}







		//void & function





		public void RefreshImage()
		{
			int imgWidth = this.ImageBox.Width;
			int imgHeight = this.ImageBox.Height;
			if (imgWidth < 200) { imgWidth = 200; }
			if (imgHeight < 200) { imgHeight = 200; }
			Bitmap newimg = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(newimg);
			g.Clear(Color.White); // White

			Font TextFont = new Font("lucida console", 10);

			//dessine le statut actuel de l'interface
			string strUiStatut = "Statut : " + this.ActualUiStade.ToString();
			g.DrawString(strUiStatut, TextFont, Brushes.Black, 0f, 0f);
			int UiStatutDown = 0 + cWriteViewAssets.GetTextSize(strUiStatut, TextFont).Height; //optien la position du bas du text du statut



			//======== dessine l'equation
			oEquationDrawContext DrawContextToUse;
			if (this.AnyDrawContextDefined)
			{
				DrawContextToUse = this.ActualEquationDrawContext;
			}
			else
			{
				DrawContextToUse = new oEquationDrawContext();
			}



			//l'equation peut etre dessiner seulement si l'etat graphique actuel le permet
			if (this.ActualUiStade == UiStade.View || this.ActualUiStade == UiStade.Adding || this.ActualUiStade == UiStade.Moving || this.ActualUiStade == UiStade.Selecting)
			{
				this.TheEquation.uiLastTop = this.EquationDrawPos.Y; //UiStatutDown;
				this.TheEquation.uiLastLeft = this.EquationDrawPos.X; //0;
				this.TheEquation.uiLastWidth = this.TheEquation.GetWidth();
				this.TheEquation.uiLastHeight = this.TheEquation.GetHeight();
				Bitmap imgEquation = this.TheEquation.GetImage(DrawContextToUse);

				this.EquationDrawPos.X = this.TheEquation.uiLastLeft;
				this.EquationDrawPos.Y = this.TheEquation.uiLastTop;
				int uidecal = 0;
				if (this.TheEquation.ListEquationObject.Count == 0) { uidecal = -1; }
				g.DrawImage(imgEquation, this.EquationDrawPos.X + uidecal, this.EquationDrawPos.Y + uidecal);
				
			}


			/////////////////////////////////////////////////////////////// INTERFACE GRAPHIQUE ///////////////////////////////////////////////////////////////
			Point mpos = this.MousePos;
			Rectangle mrec = this.MouseRec;
			//button
			foreach (uiButton b in this.listUiButton)
			{
				if (b.Visible)
				{
					Rectangle uirec = new Rectangle(b.Left, b.Top, b.Width - 1, b.Height - 1);

					//determine le back color à utiliser
					Color BackColorToUse = b.BackColor;
					if (this.IsMouseLeftDown && uirec.IntersectsWith(mrec)) { BackColorToUse = b.DownColor; }

					g.FillRectangle(new SolidBrush(BackColorToUse), uirec);

					//dessine back image
					if (b.BackImage != null)
					{
						int imgx = b.Left + (b.Width / 2) - (b.BackImage.Width / 2);
						int imgy = b.Top + (b.Height / 2) - (b.BackImage.Height / 2);
						g.DrawImage(b.BackImage, imgx, imgy);
					}

					g.DrawRectangle(Pens.Black, uirec);


					//dessine le text
					Point TextPos = cWriteViewAssets.GetTextPosCenteredAt(b.Text, b.Font, uirec);
					g.DrawString(b.Text, b.Font, Brushes.Black, (PointF)TextPos);

				}
			}






			g.Dispose();
			if (this.ImageBox.Image != null) { this.ImageBox.Image.Dispose(); }
			this.ImageBox.Image = newimg;
			GC.Collect();
		}
		private void SelectEquationObject(oEquationObject TheObj, bool RefreshThis = true)
		{
			TheObj.BackColor = Color.LightBlue;
			if (TheObj.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber) { ((EQoNumber)TheObj).RefreshImage(); }
			this.listAllSelectedObject.Add(TheObj);
			if (RefreshThis) { this.Refresh(); }
		}
		private void UnselectEquationObject(oEquationObject TheObj, bool RefreshThis = true)
		{
			TheObj.BackColor = Color.White;
			if (TheObj.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber) { ((EQoNumber)TheObj).RefreshImage(); }
			this.listAllSelectedObject.Remove(TheObj);
			if (RefreshThis) { this.Refresh(); }
		}
		public void UnselectAllObject(bool RefreshThis = true)
		{
			while (this.listAllSelectedObject.Count > 0)
			{
				this.UnselectEquationObject(this.listAllSelectedObject[0]);
			}
			if (RefreshThis) { this.Refresh(); }
		}
		private List<oEquationObject> listAllSelectedObject = new List<oEquationObject>();
		//fait selectionner tout les element de A à B, pourvut qu'ils ont le meme parent. et il faut preciser le parent
		private void SelectAToB(oEquation Parent, oEquationObject eoA, oEquationObject eoB)
		{
			int indexA = Parent.ListEquationObject.IndexOf(eoA);
			int indexB = Parent.ListEquationObject.IndexOf(eoB);

			//defini les index de depart et de fin
			int StartIndex = indexA;
			int EndIndex = indexB;
			if (indexA > indexB)
			{
				StartIndex = indexB;
				EndIndex = indexA;
			}

			//fait selectionner tout les element concerné
			int ActualIndex = StartIndex;
			while (ActualIndex <= EndIndex)
			{
				this.SelectEquationObject(Parent.ListEquationObject[ActualIndex]);

				//next iteration
				ActualIndex++;
			}

		}
		private bool IsSelected(oEquationObject eo)
		{
			foreach (oEquationObject subeo in this.listAllSelectedObject)
			{
				if (subeo == eo) { return true; }
			}
			return false;
		}


		//cette void effectue tout les refresh, dans l'ordre aproprier s'il y en a un
		public void Refresh()
		{
			this.RefreshImage();


		}


		//void new()
		public oEquationEditer(oEquation StartEquation)
		{
			this.zzzTheEquation = StartEquation;
			this.zzzActualUiStade = UiStade.View; //maintenant qu'une equation est defini, on peut se mettre en mode view
			
			this.ImageBox.BorderStyle = BorderStyle.FixedSingle;
			this.ImageBox.SizeChanged += new EventHandler(this.ImageBox_SizeChanged);
			this.ImageBox.MouseMove += new MouseEventHandler(this.ImageBox_MouseMove);
			this.ImageBox.MouseDown += new MouseEventHandler(this.ImageBox_MouseDown);
			this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);
			this.ImageBox.MouseDoubleClick += new MouseEventHandler(this.ImageBox_MouseDoubleClick);

			this.CreateInterface();
			this.CreateNavpad();
			this.CreateSaveZone();
			this.Refresh();
		}

		//les private event
		private void ImageBox_SizeChanged(object sender, EventArgs e)
		{
			this.RefreshInterfaceTextSize();
			this.RefreshNavpadTextSize();
			this.RefreshSaveZoneTextSize();
			this.Refresh();
		}

		private int MouseMove_FrameSkip = 2; // 2
		private int MouseMove_FrameSkip_Pos = 0;
		private void ImageBox_MouseMove(object sender, MouseEventArgs e)
		{
			this.MousePosBound.X = e.X;
			this.MousePosBound.Y = e.Y;

			//un frameskip pour ne pas faire laguer tout le program à cause que la tache de dessin est trop exigante
			this.MouseMove_FrameSkip_Pos++;
			if (this.MouseMove_FrameSkip_Pos >= this.MouseMove_FrameSkip)
			{
				this.ImageBox.Refresh();
				Graphics g = this.ImageBox.CreateGraphics();


				if (this.ActualUiStade == UiStade.Adding || this.ActualUiStade == UiStade.Moving)
				{

					////affiche à l'utilisateur où sera inséré le nouvelle élément
					//obtien l'element dessous la sourie
					Point eqmpos = this.EquationMousePos;
					sEquationObjectAndEquation ObjectUnderMouse = this.TheEquation.GetEquationObjectAtUiPos(eqmpos.X, eqmpos.Y);
					if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
					{
						if (ObjectUnderMouse.TheEqu.ListEquationObject.Count == 0)
						{
							oEquation eq = ObjectUnderMouse.TheEqu;
							//obtien la position graphique du parent
							Point ParentUpLeftPos = this.TheEquation.GetUiPosOfEquationObject(ObjectUnderMouse.TheEquObj);
							Point EquUpLeftPos = new Point(ParentUpLeftPos.X + eq.uiLastLeft, ParentUpLeftPos.Y + eq.uiLastTop);

							g.FillRectangle(new SolidBrush(this.TextCursorColor), EquUpLeftPos.X + 5, EquUpLeftPos.Y + 5, 14, 22); // EquUpLeftPos.X, EquUpLeftPos.Y, eq.uiLastWidth, eq.uiLastHeight

						}
					}
					if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
					{
						oEquationObject eo = ObjectUnderMouse.TheEquObj;
						oEquation eq = ObjectUnderMouse.TheEqu; //equation parente
						Point UpLeftPos = this.TheEquation.GetUiPosOfEquationObject(eo);
						int xmiddle = UpLeftPos.X + (eo.uiLastWidth / 2); //position horizontale de la moitier de l'object

						//dessine la barre à gauche ou à droite celon la posiion de la sourie
						Pen ThePen = new Pen(this.TextCursorColor, 3);
						//int eqUiY = UpLeftPos.Y; // + (eo.uiLastHeight / 2);
						//int eoHeight = eo.GetHeight();
						if (e.X < xmiddle)
						{
							g.DrawLine(ThePen, UpLeftPos.X, UpLeftPos.Y, UpLeftPos.X, UpLeftPos.Y + eo.uiLastHeight);
						}
						else
						{
							g.DrawLine(ThePen, UpLeftPos.X + eo.uiLastWidth, UpLeftPos.Y, UpLeftPos.X + eo.uiLastWidth, UpLeftPos.Y + eo.uiLastHeight);
						}
					}




					if (this.ActualUiStade == UiStade.Adding)
					{
						//affiche à coté de la sourie c'est quoi le nouvel element
						//string MouseString = this.addmodeNewObject.GetReadableName();
						//g.DrawString(MouseString, new Font("consolas", 10f), Brushes.Black, (float)e.X + 13f, (float)e.Y); //c'est décalé horizontalement car sinon, les premier caractere sont dessous le curseur
						PointF ActualPos = new PointF((float)e.X + 13f, (float)e.Y);
						Font asdfFont = new Font("consolas", 10f);
						foreach (oEquationObject subeo in this.addmodeNewObjects)
						{
							string MouseString = subeo.GetReadableName();
							g.DrawString(MouseString, asdfFont, Brushes.Black, ActualPos);
							//next iteration
							ActualPos.Y += 10f;
						}
						asdfFont.Dispose();

					}
					else if (this.ActualUiStade == UiStade.Moving)
					{
						PointF ActualPos = new PointF((float)e.X + 13f, (float)e.Y);
						Font asdfFont = new Font("consolas", 10f);
						foreach (oEquationObject subeo in this.movemodeArrayObject)
						{
							string MouseString = subeo.GetReadableName();
							g.DrawString(MouseString, asdfFont, Brushes.Black, ActualPos);
							//next iteration
							ActualPos.Y += 10f;
						}
						asdfFont.Dispose();
					}

				}
				else if (this.ActualUiStade == UiStade.Selecting)
				{
					Point eqmpos = this.EquationMousePos;

					//check si l'utilisateur a la sourie sur un element qui est de la meme equation
					sEquationObjectAndEquation rep = this.TheEquation.GetEquationObjectAtUiPos(eqmpos.X, eqmpos.Y);
					//cette if make sure que c'est bien un element qu'il y a sous la sourie 
					if (rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
					{
						oEquationObject eo = rep.TheEquObj;
						oEquation eq = rep.TheEqu;
						int ActualDepth = this.TheEquation.GetDepthOfEquationObject(eo);

						while (ActualDepth > this.selectinfoStart_Depth)
						{
							eo = this.TheEquation.GetParentOfEquation(eq);
							eq = this.TheEquation.GetParentOfEquationObject(eo);
							ActualDepth--;
						}

						if (eq == this.selectinfoParent)
						{

							this.UnselectAllObject();
							this.SelectAToB(this.selectinfoParent, this.selectinfoStart, eo);


						}
					}




				}





				this.MouseMove_FrameSkip_Pos = 0;
			}
		}
		private void ImageBox_MouseDown(object sender, MouseEventArgs e)
		{
			Point mpos = this.MousePos;
			Point EquMPos = new Point(mpos.X - this.EquationDrawPos.X, mpos.Y - this.EquationDrawPos.Y); //position verticale relative à l'image
			if (e.Button == MouseButtons.Left)
			{
				this.IsMouseLeftDown = true;
				uiButton TheButton = this.IsPointOnAnyButton(mpos.X, mpos.Y);

				if (TheButton != null)
				{
					TheButton.Raise_MouseLeftDown();
					this.IsMouseLeftDownOnButton = true;


					this.Refresh(); //il y a des refresh pendant l'execution de mouseup, mais les button de l'interface graphique doit etre refresher quand meme dans toute les situation ou il pourait avoir changer d'aspect

				}
				else
				{

					if (this.ActualUiStade == UiStade.View)
					{
						this.StartSelection();

						//pas besoin de refresher ici parce que StartSelection() fait un refresh à la fin

					}






				}
			}


		}
		private bool MouseUp_UnselectAllObjectAfterMouseUp = false;
		private void ImageBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.IsMouseLeftDown = false;

				if (!this.IsMouseLeftDownOnButton)
				{
					this.TotalHideSaveZone();
					this.RefreshSaveZoneTextSize(); //sert à refresher les + - derrière le text des button

					Point mpos = this.MousePos;
					Point EquMPos = this.EquationMousePos; //new Point(mpos.X - this.EquationDrawPos.X, mpos.Y - this.EquationDrawPos.Y); //position verticale relative à l'image
														   ////// certaine opperation doivent etre executer AVANT de changer this.ActualUiStade
					if (this.ActualUiStade == UiStade.View)
					{
						//check si l'utilisateur a clicker sur un element de l'equation qui est modifiable, pour faire ouvrire une fenetre qui lui permetteras de le modifier
						sEquationObjectAndEquation rep = this.TheEquation.GetEquationObjectAtUiPos(EquMPos.X, EquMPos.Y);
						if (rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
						{
							oEquationObject eo = rep.TheEquObj;
							//change le BackColor de l'object sélectionné
							this.UnselectAllObject();
							this.SelectEquationObject(eo);


							int depth = this.TheEquation.GetDepthOfEquationObject(eo);
							//Program.wdebug(depth);

							//MessageBox.Show(rep.MainObject.ToString());


							//remet le BackColor de l'object sélectionné
							//this.UnselectEquationObject(eo);
						}
						else if (rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
						{

							this.UnselectAllObject();
						}
						else if (rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.none)
						{
							this.UnselectAllObject();
						}



					}
					else if (this.ActualUiStade == UiStade.Adding)
					{
						this.FinishAddMode();
					}
					else if (this.ActualUiStade == UiStade.Moving)
					{
						this.FinishMoveMode();
					}
					else if (this.ActualUiStade == UiStade.Selecting)
					{
						this.EndSelection();
					}


				}
				else
				{

				}

				//execute les mouseup
				uiButton TheButton = this.IsPointOnAnyButton(this.MousePos.X, this.MousePos.Y);
				if (TheButton != null)
				{
					TheButton.Raise_MouseLeftUp();
				}
				this.GeneralMouseUp();


				this.IsMouseLeftDownOnButton = false;
			}
			if (e.Button == MouseButtons.Right)
			{
				Point mpos = this.MousePos;
				Point EquMPos = this.EquationMousePos; //new Point(mpos.X - this.EquationDrawPos.X, mpos.Y - this.EquationDrawPos.Y); //position verticale relative à l'image
				if (this.ActualUiStade == UiStade.View)
				{
					sEquationObjectAndEquation SubObject = this.TheEquation.GetEquationObjectAtUiPos(EquMPos.X, EquMPos.Y);
					oEquation eq = SubObject.TheEqu;
					oEquationObject eo = SubObject.TheEquObj;

					if (SubObject.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
					{

						//si l'utilisateur a selectionner plusieur element et que la sourie est sur l'un d'entre eux, c'est un rightclick special qu'il faut afficher
						bool IsPlusieur = this.listAllSelectedObject.Count > 1;
						bool IsAlreadySelected = this.IsSelected(eo);

						if (IsPlusieur && IsAlreadySelected)
						{// sélection de plusieurs element :

							string optDelete = "Delete";
							string optMove = "Move";
							string optCopy = "Copier";

							wvRightClick2 rc = new wvRightClick2();
							rc.AddSeparator();
							rc.AddChoice(optDelete);
							rc.AddChoice(optMove);
							rc.AddChoice(optCopy);
							string rep = rc.GetChoice();

							if (rep == optDelete)
							{
								//pour chaque object selectioné, il obtien son parent et le delete de sa list d'enfant
								while (this.listAllSelectedObject.Count > 0)
								{
									oEquation eoParent = this.TheEquation.GetParentOfEquationObject(this.listAllSelectedObject[0]);
									eoParent.ListEquationObject.Remove(this.listAllSelectedObject[0]);
									this.listAllSelectedObject.RemoveAt(0);
								}
								
							}
							if (rep == optMove)
							{
								List<oEquationObject> allobjectcopy = new List<oEquationObject>();
								allobjectcopy.AddRange(this.listAllSelectedObject);
								
								//pour chaque object selectioné, il obtien son parent et le delete de sa list d'enfant
								foreach (oEquationObject subeo in this.listAllSelectedObject)
								{
									oEquation eoParent = this.TheEquation.GetParentOfEquationObject(subeo);
									eoParent.ListEquationObject.Remove(subeo);
								}

								this.SetToMoveMode(allobjectcopy.ToArray());
								

							}
							if (rep == optCopy)
							{
								List<oEquationObject> allobjectcopy = new List<oEquationObject>();
								//il fait une copy de tout les object
								foreach (oEquationObject subeo in this.listAllSelectedObject)
								{
									allobjectcopy.Add(subeo.GetCopy());
								}
								
								this.SetToAddMode(allobjectcopy.ToArray());
							}



						}
						else
						{// selection d'un seul element :


							this.UnselectAllObject();
							this.SelectEquationObject(eo);

							//right click adapté celon le type d'object
							if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Modifier la valeur";
								string[] optsNumbers1 = new string[] { "0", "1", "2", "3", "4" };
								string[] optsNumbers2 = new string[] { "5", "6", "7", "8", "9" };

								wvRightClick3 rc = new wvRightClick3();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddChoice(optEdit);
								rc.AddSeparator();
								rc.AddChoice(optsNumbers1);
								rc.AddChoice(optsNumbers2);
								//rc.Width = 200;
								string rep = rc.GetChoice();


								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);

								}
								else if (rep == optMove)
								{
									//eq.ListEquationObject.Remove(eo);
									//this.SetToMoveMode(new oEquationObject[] { eo });

									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								else if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								else if (rep == optEdit)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoNumberEdit pne = new panelEQoNumberEdit((EQoNumber)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();

									
								}
								else
								{
									//on vérifi si l'user a clické sur un nombre (0 à 9)
									for (int i = 0; i <= 9; i++)
									{
										if (i.ToString() == rep)
										{
											((EQoNumber)eo).ActualStrValue = i.ToString();
										}
									}



								}



							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fVariable)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Changer de variable";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddChoice(optEdit);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);

								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optEdit)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoVariableEdit pne = new panelEQoVariableEdit((EQoVariable)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();
								}


							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fUserFonction)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Changer de fonction";
								string optParam0 = "0";
								string optParam1 = "1";
								string optParam2 = "2";
								string optParam3 = "3";
								string optParam4 = "4";
								string optParam5 = "5";

								wvRightClick3 rc = new wvRightClick3();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddChoice(optEdit);
								rc.AddSeparator();
								//rc.AddChoice(optParam0);
								//rc.AddChoice(optParam1);
								//rc.AddChoice(optParam2);
								//rc.AddChoice(optParam3);
								//rc.AddChoice(optParam4);
								//rc.AddChoice(optParam5);
								rc.AddChoice(new string[] { optParam0, optParam1, optParam2, optParam3, optParam4, optParam5 });
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);

								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optEdit)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoUserFunctionEdit pne = new panelEQoUserFunctionEdit((EQoUserFunction)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();
								}
								if (rep == optParam0) { ((EQoUserFunction)eo).SetParamCount(0); }
								if (rep == optParam1) { ((EQoUserFunction)eo).SetParamCount(1); }
								if (rep == optParam2) { ((EQoUserFunction)eo).SetParamCount(2); }
								if (rep == optParam3) { ((EQoUserFunction)eo).SetParamCount(3); }
								if (rep == optParam4) { ((EQoUserFunction)eo).SetParamCount(4); }
								if (rep == optParam5) { ((EQoUserFunction)eo).SetParamCount(5); }


							}
							else if (eo.ActualEquationObjectType == oEquationObject.EquationObjectType.Operator)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optAdd = "+";
								string optSub = "-";
								string optMul = "×";
								string optDiv = "÷";

								//wvRightClick2 rc = new wvRightClick2();
								//rc.AddSeparator();
								//rc.AddChoice(optDelete);
								//rc.AddChoice(optMove);
								//rc.AddSeparator();
								//rc.AddChoice(optAdd);
								//rc.AddChoice(optSub);
								//rc.AddChoice(optMul);
								//rc.AddChoice(optDiv);
								wvRightClick3 rc = new wvRightClick3();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddSeparator();
								rc.AddChoice(new string[] { optAdd, optSub, optMul, optDiv });
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optAdd)
								{
									((EQoOperator)(eo)).SetOpperatorType(EQoOperator.OperatorType.Addition);
								}
								if (rep == optSub)
								{
									((EQoOperator)(eo)).SetOpperatorType(EQoOperator.OperatorType.Substraction);
								}
								if (rep == optMul)
								{
									((EQoOperator)(eo)).SetOpperatorType(EQoOperator.OperatorType.Multiplication);
								}
								if (rep == optDiv)
								{
									((EQoOperator)(eo)).SetOpperatorType(EQoOperator.OperatorType.Division);
								}


							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fParenthese)
							{
								string optDelete = "Delete";
								string optDeleteWithContent = "Delete with content";
								string optMove = "Move";
								string optCopy = "Copier";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optDeleteWithContent);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									//obtien l'index actuel de la parenthese pour savoir ou inserer tout les element qu'elle contien
									int index = eq.ListEquationObject.IndexOf(eo);
									//ajoute tout les element de la parenthese dans le parent
									foreach (oEquationObject subeo in eo.ListEquation[0].ListEquationObject)
									{
										eq.ListEquationObject.Insert(index, subeo);
										//next iteration
										index++;
									}

									//supprimme la parenthese
									eq.ListEquationObject.Remove(eo);


								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optDeleteWithContent)
								{
									eq.ListEquationObject.Remove(eo);

								}


							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fSigmaSummation)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Modifier la variable";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddChoice(optEdit);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optEdit)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoSigmaSummationEdit pns = new panelEQoSigmaSummationEdit((EQoSigmaSummation)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}
							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fPiProduct)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Modifier la variable";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddChoice(optEdit);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optEdit)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoPiProductEdit pns = new panelEQoPiProductEdit((EQoPiProduct)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}
							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fIntegral)
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";
								string optEdit = "Modifier la variable";
								string optParam = "Paramètres de l'intégrale ...";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								rc.AddSeparator();
								rc.AddChoice(optEdit);
								rc.AddChoice(optParam);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}
								if (rep == optEdit)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoIntegralEdit pns = new panelEQoIntegralEdit((EQoIntegral)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}
								if (rep == optParam)
								{
									FormIntegralParam fip = new FormIntegralParam((EQoIntegral)eo);
									fip.ShowDialog();
								}
							}
							else if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fTrig)
							{
								string optDelete = "Delete";
								string optMove = "Move";

								wvRightClick3 rc = new wvRightClick3();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddSeparator();
								rc.AddChoice(new string[] { "sin", "arcsin", "sinh", "arsinh" });
								rc.AddChoice(new string[] { "cos", "arccos", "cosh", "arcosh" });
								rc.AddChoice(new string[] { "tan", "arctan", "tanh", "artanh" });
								rc.AddSeparator();
								rc.AddChoice(new string[] { "sec", "arcsec", "sech", "arsech" });
								rc.AddChoice(new string[] { "csc", "arccsc", "csch", "arcsch" });
								rc.AddChoice(new string[] { "cot", "arccot", "coth", "arcoth" });
								rc.Width = 250;
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (cWriteViewAssets.IsATrigFunctionName(rep))
								{
									((EQoTrigFunction)eo).SetActualFuncStr(rep);
								}

							}
							else //RightClick par default
							{
								string optDelete = "Delete";
								string optMove = "Move";
								string optCopy = "Copier";

								wvRightClick2 rc = new wvRightClick2();
								rc.AddSeparator();
								rc.AddChoice(optDelete);
								rc.AddChoice(optMove);
								rc.AddChoice(optCopy);
								string rep = rc.GetChoice();

								if (rep == optDelete)
								{
									eq.ListEquationObject.Remove(eo);
								}
								if (rep == optMove)
								{
									this.ExecMoveOfSingleEquationObject(eo, eq);
								}
								if (rep == optCopy)
								{
									this.SetToAddMode(eo.GetCopy());
								}

							}





							this.UnselectAllObject();

						}
					}
					else if (SubObject.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
					{
						this.UnselectAllObject();
					}
					else if (SubObject.TheMainObject == sEquationObjectAndEquation.enumMainObject.none)
					{
						this.UnselectAllObject();
					}





				}
				else if (this.ActualUiStade == UiStade.Adding)
				{
					this.CancelAddMode();


				}

			}

			////// 

			if (this.MouseUp_UnselectAllObjectAfterMouseUp)
			{
				this.UnselectAllObject();
				this.MouseUp_UnselectAllObjectAfterMouseUp = false;
			}

			this.Refresh(); //il y a des refresh pendant l'execution de mouseup, mais les button de l'interface graphique doit etre refresher quand meme dans toute les situation ou il pourait avoir changer d'aspect
		}
		private void ImageBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this.DoubleClickEnabled)
			{
				Point mpos = this.MousePos;
				if (this.IsPointOnAnyButton(mpos.X, mpos.Y) == null)
				{
					if (e.Button == MouseButtons.Left)
					{
						Point EquMPos = this.EquationMousePos; //new Point(mpos.X - this.EquationDrawPos.X, mpos.Y - this.EquationDrawPos.Y); //position verticale relative à l'image

						sEquationObjectAndEquation SubObject = this.TheEquation.GetEquationObjectAtUiPos(EquMPos.X, EquMPos.Y);
						oEquation eq = SubObject.TheEqu;
						oEquationObject eo = SubObject.TheEquObj;

						if (!SubObject.IsTotalNull())
						{
							if (SubObject.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
							{
								//si l'utilisateur a double-clické sur un object, celon le type de l'object, il va lancer l'édition du nom de l'object

								this.UnselectAllObject();
								this.SelectEquationObject(eo);

								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fNumber)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoNumberEdit pne = new panelEQoNumberEdit((EQoNumber)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();

								}
								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fVariable)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoVariableEdit pne = new panelEQoVariableEdit((EQoVariable)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();
								}
								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fUserFonction)
								{
									//obtien la coordonner graphique de l'object
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoUserFunctionEdit pne = new panelEQoUserFunctionEdit((EQoUserFunction)eo);
									pne.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pne.Width = 150;
									pne.ShowDialog();
								}
								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fSigmaSummation)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoSigmaSummationEdit pns = new panelEQoSigmaSummationEdit((EQoSigmaSummation)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}
								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fPiProduct)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoPiProductEdit pns = new panelEQoPiProductEdit((EQoPiProduct)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}
								if (eo.ActualSpecificObjectType == oEquationObject.SpecificObjectType.fIntegral)
								{
									Point screenpos = this.ImageBox.PointToScreen(this.TheEquation.GetUiPosOfEquationObject(eo));

									//fait apparaitre le paneau pour modifier la valeur
									panelEQoIntegralEdit pns = new panelEQoIntegralEdit((EQoIntegral)eo);
									pns.UpCenter = new Point(screenpos.X + (eo.uiLastWidth / 2), screenpos.Y + eo.uiLastHeight);
									pns.Width = 150;
									pns.ShowDialog();
								}

								//l'event mouse up a lieu immédiatement après mouse double click
								this.MouseUp_UnselectAllObjectAfterMouseUp = true;
							}
						}


					}


				}
			}
		}



		#region AJOUT D'OBJECTS
		private oEquationObject[] addmodeNewObjects;

		//met l'éditeur (this) en mode d'ajout
		public void SetToAddMode(oEquationObject NewObj)
		{
			//this.UnselectAllObject();
			//this.addmodeNewObject = NewObj;
			//this.zzzActualUiStade = UiStade.Adding;
			//this.Refresh();
			this.SetToAddMode(new oEquationObject[] { NewObj });
		}
		public void SetToAddMode(oEquationObject[] NewObjs)
		{
			this.UnselectAllObject();
			this.addmodeNewObjects = NewObjs;
			this.zzzActualUiStade = UiStade.Adding;
			this.Refresh();
		}
		private void FinishAddMode()
		{
			if (this.ActualUiStade == UiStade.Adding)
			{
				Point mpos = this.MousePos;
				//obtien l'element dessous la sourie
				Point eqmpos = this.EquationMousePos;
				sEquationObjectAndEquation ObjectUnderMouse = this.TheEquation.GetEquationObjectAtUiPos(eqmpos.X, eqmpos.Y);
				oEquationObject eo = ObjectUnderMouse.TheEquObj;
				oEquation eq = ObjectUnderMouse.TheEqu;
				if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
				{
					//si l'equation est vide
					if (eq.ListEquationObject.Count == 0)
					{
						//eq.ListEquationObject.Add(this.addmodeNewObject);
						eq.ListEquationObject.AddRange(this.addmodeNewObjects);
					}
					else
					{

					}
				}
				else if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
				{
					////check s'il faut ajouter l'object à gauche ou à droite
					Point UpLeftPos = this.TheEquation.GetUiPosOfEquationObject(eo);
					int xmiddle = UpLeftPos.X + (eo.uiLastWidth / 2); //position horizontale de la moitier de l'object

					bool AddToRight = false;
					if (mpos.X > xmiddle) { AddToRight = true; }

					//obtien l'index auquel ajouter le nouvelle element
					int index = eq.ListEquationObject.IndexOf(eo);
					if (AddToRight) { index++; }

					//ajoute l'object
					//eq.ListEquationObject.Insert(index, this.addmodeNewObject);
					eq.ListEquationObject.InsertRange(index, this.addmodeNewObjects);

				}



				//end
				this.addmodeNewObjects = null;
				this.zzzActualUiStade = UiStade.View;
				this.Refresh();
			}
		}
		private void CancelAddMode()
		{
			if (this.ActualUiStade == UiStade.Adding)
			{
				this.addmodeNewObjects = null;
				this.zzzActualUiStade = UiStade.View;
				this.Refresh();
			}
		}
		

		private oEquationObject[] movemodeArrayObject;
		public void SetToMoveMode(oEquationObject[] TheObjs)
		{
			//this.UnselectAllObject();
			this.movemodeArrayObject = TheObjs;
			this.zzzActualUiStade = UiStade.Moving;
			this.Refresh();
		}
		private void FinishMoveMode()
		{
			if (this.ActualUiStade == UiStade.Moving)
			{
				Point mpos = this.MousePos;
				//obtien l'element dessous la sourie
				Point eqmpos = this.EquationMousePos;
				sEquationObjectAndEquation ObjectUnderMouse = this.TheEquation.GetEquationObjectAtUiPos(eqmpos.X, eqmpos.Y);
				oEquationObject eo = ObjectUnderMouse.TheEquObj;
				oEquation eq = ObjectUnderMouse.TheEqu;

				bool CanFinish = false;
				if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.Equation)
				{
					//si l'equation est vide
					if (eq.ListEquationObject.Count == 0)
					{
						eq.ListEquationObject.AddRange(this.movemodeArrayObject.ToList());
						CanFinish = true;
					}
					else
					{
						CanFinish = false;
					}
				}
				else if (ObjectUnderMouse.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
				{
					////check s'il faut ajouter l'object à gauche ou à droite
					Point UpLeftPos = this.TheEquation.GetUiPosOfEquationObject(eo);
					int xmiddle = UpLeftPos.X + (eo.uiLastWidth / 2); //position horizontale de la moitier de l'object

					bool AddToRight = false;
					if (mpos.X > xmiddle) { AddToRight = true; }

					//obtien l'index auquel ajouter le nouvelle element
					int index = eq.ListEquationObject.IndexOf(eo);
					if (AddToRight) { index++; }

					//ajoute l'object
					eq.ListEquationObject.InsertRange(index, this.movemodeArrayObject.ToList());
					CanFinish = true;
				}



				//end
				if (CanFinish)
				{
					//avant de perdre la liste de tout les object deplacé, make sure qu'il n'ont pas le back color d'object sélectionné
					//foreach (oEquationObject subeo in this.movemodeArrayObject)
					//{
					//	subeo.BackColor = Color.White;
					//}
					this.UnselectAllObject();


					this.movemodeArrayObject = null;
					this.zzzActualUiStade = UiStade.View;
					this.Refresh();
				}
			}
		}


		//cette void ne sert qu'à me faciliter la vie
		//il faut donner l'object et sont parent
		private void ExecMoveOfSingleEquationObject(oEquationObject TheObj, oEquation TheEqu)
		{
			TheEqu.ListEquationObject.Remove(TheObj);
			this.SetToMoveMode(new oEquationObject[] { TheObj });
		}

		#endregion
		#region SELECTION D'OBJECT
		private oEquation selectinfoParent; //l'equation parente des deux object selectionner
		private oEquationObject selectinfoStart;
		private int selectinfoStart_Depth = -1;
		
		//demare et termine la selection à la position graphique de la sourie
		private void StartSelection()
		{
			if (this.ActualUiStade == UiStade.View)
			{
				Point mpos = this.MousePos;
				Point eqmpos = this.EquationMousePos;
				sEquationObjectAndEquation rep = this.TheEquation.GetEquationObjectAtUiPos(eqmpos.X, eqmpos.Y);
				//on peut demarer une selection seulement si c'est une oEquationObject que l'utilisateur vise
				if (rep.TheMainObject == sEquationObjectAndEquation.enumMainObject.EquationObject)
				{
					this.zzzActualUiStade = UiStade.Selecting;
					this.UnselectAllObject();



					this.selectinfoParent = rep.TheEqu;
					this.selectinfoStart = rep.TheEquObj;
					this.selectinfoStart_Depth = this.TheEquation.GetDepthOfEquationObject(rep.TheEquObj);

					this.SelectEquationObject(rep.TheEquObj);


					this.Refresh();
				}
			}
		}
		private void EndSelection()
		{
			if (this.ActualUiStade == UiStade.Selecting)
			{
				this.zzzActualUiStade = UiStade.View; //dès le debut pour pas que l'event mousemove fasse chier la procedure (meme s'il n'est pas censer pourvoir s'executer en meme temps)

				Point mpos = this.MousePos;





				//set tout sur null
				this.selectinfoParent = null;
				this.selectinfoStart = null;
				//this.selectinfoEnd = null;
				
				//end
				this.Refresh();
			}
		}

		#endregion
		#region INTERFACE GRAPHIQUE
		private uiButton interNumberButton;
		private uiButton interOppAddButton;
		private uiButton interOppSubButton;
		private uiButton interOppMulButton;
		private uiButton interOppDivButton;
		private uiButton interParentheseButton;

		private uiButton interVariableButton;
		private uiButton interFunctionButton;

		//colonne 2
		private uiButton interFractionButton;
		private uiButton interSquareRootButton;
		private uiButton interRootNButton;
		private uiButton interExponentButton;

		//colonne 3
		private uiButton interTrigToggleButton;
		private uiButton interAbsButton; //fonction absolue
		private uiButton interLogButton;
		private uiButton interExpButton;
		private uiButton interAGMButton;
		private uiButton interFactButton;

		//colonne 4
		private uiButton interSommationButton;
		private uiButton interProductButton;
		private uiButton interIntegralButton;
		private uiButton interBinominalCoef;



		private void CreateInterface()
		{
			this.interNumberButton = new uiButton(this);
			this.interNumberButton.Text = "Nombre";
			this.interNumberButton.MouseLeftClick += new EventHandler(this.interNumberButton_MouseLeftClick);

			Font oppfont = new Font("consolas", 20);
			this.interOppAddButton = new uiButton(this);
			this.interOppAddButton.Text = "+";
			this.interOppAddButton.Font = oppfont;
			this.interOppAddButton.MouseLeftClick += this.interOppAddButton_MouseLeftClick;

			this.interOppSubButton = new uiButton(this);
			this.interOppSubButton.Text = "-";
			this.interOppSubButton.Font = oppfont;
			this.interOppSubButton.MouseLeftClick += this.interOppSubButton_MouseLeftClick;

			this.interOppMulButton = new uiButton(this);
			this.interOppMulButton.Text = "×";
			this.interOppMulButton.Font = oppfont;
			this.interOppMulButton.MouseLeftClick += this.interOppMulButton_MouseLeftClick;

			this.interOppDivButton = new uiButton(this);
			this.interOppDivButton.Text = "÷";
			this.interOppDivButton.Font = oppfont;
			this.interOppDivButton.MouseLeftClick += this.interOppDivButton_MouseLeftClick;

			this.interParentheseButton = new uiButton(this);
			this.interParentheseButton.Text = "Parenthèse";
			this.interParentheseButton.MouseLeftClick += new EventHandler(this.interParentheseButton_MouseLeftClick);


			this.interVariableButton = new uiButton(this);
			this.interVariableButton.Text = "Variable";
			this.interVariableButton.MouseLeftClick += new EventHandler(this.interVariableButton_MouseLeftClick);

			this.interFunctionButton = new uiButton(this);
			this.interFunctionButton.Text = "Fonction";
			this.interFunctionButton.MouseLeftClick += new EventHandler(this.interFunctionButton_MouseLeftClick);



			//colonne #2
			this.interFractionButton = new uiButton(this);
			this.interFractionButton.Text = "Fraction";
			this.interFractionButton.MouseLeftClick += new EventHandler(this.interFractionButton_MouseLeftClick);

			this.interSquareRootButton = new uiButton(this);
			this.interSquareRootButton.Text = "Racine Carrée";
			this.interSquareRootButton.MouseLeftClick += new EventHandler(this.interSquareRootButton_MouseLeftClick);

			this.interRootNButton = new uiButton(this);
			this.interRootNButton.Text = "Racine n-ième";
			this.interRootNButton.MouseLeftClick += new EventHandler(this.interRootNButton_MouseLeftClick);

			this.interExponentButton = new uiButton(this);
			this.interExponentButton.Text = "Exposant";
			this.interExponentButton.MouseLeftClick += new EventHandler(this.interExponentButton_MouseLeftClick);


			//colonne #3
			this.interTrigToggleButton = new uiButton(this);
			this.interTrigToggleButton.Text = "Trigo";
			this.interTrigToggleButton.MouseLeftClick += new EventHandler(this.interTrigToggleButton_MouseLeftClick);

			this.interAbsButton = new uiButton(this);
			this.interAbsButton.Text = "| x |";
			this.interAbsButton.MouseLeftClick += new EventHandler(this.interAbsButton_MouseLeftClick);

			this.interLogButton = new uiButton(this);
			this.interLogButton.Text = "ln(x)";
			this.interLogButton.MouseLeftClick += new EventHandler(this.interLogButton_MouseLeftClick);

			this.interExpButton = new uiButton(this);
			this.interExpButton.Text = "exp(x)";
			this.interExpButton.MouseLeftClick += new EventHandler(this.interExpButton_MouseLeftClick);

			this.interAGMButton = new uiButton(this);
			this.interAGMButton.Text = "AGM(a,b)";
			this.interAGMButton.MouseLeftClick += new EventHandler(this.interAGMButton_MouseLeftClick);

			this.interFactButton = new uiButton(this);
			this.interFactButton.Text = "n!";
			this.interFactButton.MouseLeftClick += new EventHandler(this.interFactButton_MouseLeftClick);

			//colonne 4
			this.interSommationButton = new uiButton(this);
			this.interSommationButton.Text = "Sommation";
			this.interSommationButton.MouseLeftClick += new EventHandler(this.interSommationButton_MouseLeftClick);

			this.interProductButton = new uiButton(this);
			this.interProductButton.Text = "Produit";
			this.interProductButton.MouseLeftClick += new EventHandler(this.interProductButton_MouseLeftClick);

			this.interIntegralButton = new uiButton(this);
			this.interIntegralButton.Text = "Intégrale";
			this.interIntegralButton.MouseLeftClick += new EventHandler(this.interIntegralButton_MouseLeftClick);

			this.interBinominalCoef = new uiButton(this);
			this.interBinominalCoef.Text = "Coefficient\nBinominaux";
			this.interBinominalCoef.MouseLeftClick += new EventHandler(this.interBinominalCoef_MouseLeftClick);



			this.RefreshInterfaceTextSize();
			this.CreateAllTrigButton();
		}
		private void RefreshInterfaceTextSize()
		{
			//cette variable est la position verticale maximale de tout les control (except peut etre certain button tres special)
			int MaxTop = this.Height - 110;

			this.interNumberButton.Left = 3; // 3
			this.interNumberButton.Top = MaxTop;
			this.interNumberButton.SetSize(75, 35);

			int oppwidth = 35;
			int oppheight = 35;
			this.interOppAddButton.Left = this.interNumberButton.Left;
			this.interOppAddButton.Top = this.interNumberButton.Top + this.interNumberButton.Height + 5;
			this.interOppAddButton.SetSize(oppwidth, oppheight);

			this.interOppSubButton.Left = this.interOppAddButton.Left + this.interOppAddButton.Width + 5;
			this.interOppSubButton.Top = this.interOppAddButton.Top;
			this.interOppSubButton.SetSize(oppwidth, oppheight);

			this.interOppMulButton.Left = this.interOppSubButton.Left + this.interOppSubButton.Width + 5;
			this.interOppMulButton.Top = this.interOppSubButton.Top;
			this.interOppMulButton.SetSize(oppwidth, oppheight);

			this.interOppDivButton.Left = this.interOppMulButton.Left + this.interOppMulButton.Width + 5;
			this.interOppDivButton.Top = this.interOppMulButton.Top;
			this.interOppDivButton.SetSize(oppwidth, oppheight);

			this.interParentheseButton.Left = this.interNumberButton.Left + this.interNumberButton.Width + 5;
			this.interParentheseButton.Top = this.interNumberButton.Top;
			this.interParentheseButton.SetSize(75, 35);



			//variable et fonction
			this.interVariableButton.Top = this.interOppAddButton.Top + this.interOppAddButton.Height + 5;
			this.interVariableButton.Left = this.interOppAddButton.Left;
			this.interVariableButton.SetSize(75, 25); // 75 , 20

			this.interFunctionButton.Top = this.interVariableButton.Top;
			this.interFunctionButton.Left = this.interVariableButton.Left + this.interVariableButton.Width + 5;
			this.interFunctionButton.SetSize(75, 25); // 75 , 20



			//colonne #2
			this.interFractionButton.Left = this.interParentheseButton.Left + this.interParentheseButton.Width + 5;
			this.interFractionButton.Top = this.interParentheseButton.Top;
			this.interFractionButton.SetSize(100, 22); // 21

			this.interSquareRootButton.Left = this.interFractionButton.Left;
			this.interSquareRootButton.Top = this.interFractionButton.Top + this.interFractionButton.Height + 5;
			this.interSquareRootButton.SetSize(100, 22); // 21

			this.interRootNButton.Left = this.interSquareRootButton.Left;
			this.interRootNButton.Top = this.interSquareRootButton.Top + this.interSquareRootButton.Height + 5;
			this.interRootNButton.SetSize(100, 22); // 21

			this.interExponentButton.Left = this.interRootNButton.Left;
			this.interExponentButton.Top = this.interRootNButton.Top + this.interRootNButton.Height + 5;
			this.interExponentButton.SetSize(100, 22); // 21


			//colonne #3
			this.interTrigToggleButton.SetSize(60, 21);
			this.interTrigToggleButton.Top = this.interFractionButton.Top;
			this.interTrigToggleButton.Left = this.interFractionButton.Left + this.interFractionButton.Width + 10;

			this.interAbsButton.SetSize(45, 30);
			this.interAbsButton.Left = this.interTrigToggleButton.Left;
			this.interAbsButton.Top = this.interTrigToggleButton.Top + this.interTrigToggleButton.Height + 5;

			this.interLogButton.SetSize(45, 30);
			this.interLogButton.Top = this.interAbsButton.Top;
			this.interLogButton.Left = this.interAbsButton.Left + this.interAbsButton.Width + 5;

			this.interExpButton.SetSize(45, 30);
			this.interExpButton.Top = this.interLogButton.Top;
			this.interExpButton.Left = this.interLogButton.Left + this.interLogButton.Width + 5;

			this.interAGMButton.SetSize(60, 30);
			this.interAGMButton.Left = this.interAbsButton.Left;
			this.interAGMButton.Top = this.interAbsButton.Top + this.interAbsButton.Height + 5;

			this.interFactButton.SetSize(30, 30);
			this.interFactButton.Left = this.interAGMButton.Left + this.interAGMButton.Width + 5;
			this.interFactButton.Top = this.interAGMButton.Top;


			//colonne 4
			int col4left = this.interTrigToggleButton.Left + this.interTrigToggleButton.Width + 90;
			int col4top = MaxTop;

			this.interSommationButton.SetSize(80, 25);
			this.interSommationButton.Left = col4left;
			this.interSommationButton.Top = col4top;

			this.interProductButton.SetSize(80, 25);
			this.interProductButton.Left = this.interSommationButton.Left;
			this.interProductButton.Top = this.interSommationButton.Top + this.interSommationButton.Height + 5;

			this.interIntegralButton.SetSize(80, 25);
			this.interIntegralButton.Left = this.interProductButton.Left;
			this.interIntegralButton.Top = this.interProductButton.Top + this.interProductButton.Height + 5;


			this.interBinominalCoef.SetSize(80, 33);
			this.interBinominalCoef.Top = this.interSommationButton.Top;
			this.interBinominalCoef.Left = this.interSommationButton.Left + this.interSommationButton.Width + 5;



			this.RefreshTrigTextSize();
		}

		//private events
		private void interNumberButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoNumber("1"));
		}
		private void interOppAddButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Addition));
		}
		private void interOppSubButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Substraction));
		}
		private void interOppMulButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Multiplication));
		}
		private void interOppDivButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Division));
		}
		private void interParentheseButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoParenthese());
		}
		
		private void interVariableButton_MouseLeftClick(object sender, EventArgs e)
		{
			oEquationObject newobj = new EQoVariable("x");
			this.SetToAddMode(newobj);
		}
		private void interFunctionButton_MouseLeftClick(object sender, EventArgs e)
		{
			oEquationObject newobj = new EQoUserFunction("f");
			this.SetToAddMode(newobj);
		}

		//colonne 2
		private void interFractionButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoFraction());
		}
		private void interSquareRootButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoSquareRoot());
		}
		private void interRootNButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoRootN());
		}
		private void interExponentButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoExponent());
		}

		//colonne 3
		private void interTrigToggleButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.ToggleTrigButtonVisibility();
		}
		private void interAbsButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoAbsolute());
		}
		private void interLogButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoMiscFunction("ln"));
		}
		private void interExpButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoMiscFunction("exp"));
		}
		private void interAGMButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoMiscFunction("AGM"));
		}
		private void interFactButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoFactorial());
		}

		//colonne 4
		private void interSommationButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoSigmaSummation());
		}
		private void interProductButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoPiProduct());
		}
		private void interIntegralButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoIntegral());
		}

		private void interBinominalCoef_MouseLeftClick(object sender, EventArgs e)
		{
			this.SetToAddMode(new EQoBinominalCoef());
		}




		#region trigonomic function

		private List<uiButton> interlistAllTrigButton = new List<uiButton>();


		private bool TrigButtonVisibility = false;
		private void ShowTrigButton()
		{
			this.TrigButtonVisibility = true;
			this.RefreshTrigTextSize();
		}
		private void HideTrigButton()
		{
			this.TrigButtonVisibility = false;
			this.RefreshTrigTextSize();
		}
		private void ToggleTrigButtonVisibility()
		{
			this.TrigButtonVisibility = !this.TrigButtonVisibility;
			this.RefreshTrigTextSize();
		}

		private void CreateAllTrigButton()
		{
			string[] AllFuncName = { "sin", "cos", "tan", "sec", "csc", "cot", "arcsin", "arccos", "arctan", "arcsec", "arccsc", "arccot", "sinh", "cosh", "tanh", "sech", "csch", "coth", "arsinh", "arcosh", "artanh", "arsech", "arcsch", "arcoth" };

			//crée un button pour chaque fonction trigo
			foreach (string funcname in AllFuncName)
			{
				uiButton newb = new uiButton(this);
				newb.Text = funcname;
				newb.Tag = funcname;

				newb.MouseLeftClick += new EventHandler(this.AnyTrigButton_MouseLeftClick);

				this.interlistAllTrigButton.Add(newb);
			}

		}
		private void RefreshTrigTextSize()
		{
			if (this.TrigButtonVisibility)
			{
				this.interTrigToggleButton.Text = "trigo -";
			}
			else
			{
				this.interTrigToggleButton.Text = "trigo +";
			}

			int buttonWidth = this.interTrigToggleButton.Width;
			int buttonHeight = 27; // 21

			int StartTop = this.interTrigToggleButton.Top - 5 - buttonHeight;
			int ActualTop = StartTop;
			int ActualLeft = this.interTrigToggleButton.Left - 100;
			int index = 0;
			foreach (uiButton b in this.interlistAllTrigButton)
			{
				
				b.SetSize(buttonWidth, buttonHeight);
				b.Left = ActualLeft;
				b.Top = ActualTop;
				b.Visible = this.TrigButtonVisibility;

				//next iteration
				index++;
				ActualTop -= 1 + buttonHeight;

				if (index % 6 == 0) // % 3
				{
					ActualTop = StartTop;
					ActualLeft += buttonWidth + 1;
				}
			}
			
		}

		private void AnyTrigButton_MouseLeftClick(object sender, EventArgs e)
		{
			uiButton b = (uiButton)sender;
			string funcname = (string)(b.Tag);
			this.SetToAddMode(new EQoTrigFunction(funcname));

			this.HideTrigButton();
		}


		#endregion

		#endregion
		#region NAVIGATION PAD

		private uiButton navpadCenterButton;

		private uiButton navpadUpButton;
		private uiButton navpadDownButton;
		private uiButton navpadRightButton;
		private uiButton navpadLeftButton;

		private void CreateNavpad()
		{
			this.navpadCenterButton = new uiButton(this);
			this.navpadCenterButton.Text = "O";
			this.navpadCenterButton.MouseLeftClick += new EventHandler(this.navpadCenterButton_MouseLeftClick);


			this.navpadUpButton = new uiButton(this);
			this.navpadUpButton.Text = "/\\";
			this.navpadUpButton.MouseLeftClick += new EventHandler(this.navpadUpButton_MouseLeftClick);

			this.navpadDownButton = new uiButton(this);
			this.navpadDownButton.Text = "\\/";
			this.navpadDownButton.MouseLeftClick += new EventHandler(this.navpadDownButton_MouseLeftClick);

			this.navpadRightButton = new uiButton(this);
			this.navpadRightButton.Text = ">"; // "\\\n/";
			this.navpadRightButton.MouseLeftClick += new EventHandler(this.navpadRightButton_MouseLeftClick);

			this.navpadLeftButton = new uiButton(this);
			this.navpadLeftButton.Text = "<";
			this.navpadLeftButton.MouseLeftClick += new EventHandler(this.navpadLeftButton_MouseLeftClick);

		}
		private void RefreshNavpadTextSize()
		{
			this.navpadCenterButton.SetSize(40, 30);
			this.navpadCenterButton.Top = 31;
			this.navpadCenterButton.Left = this.Width - 73;


			this.navpadUpButton.SetSize(40, 27);
			this.navpadUpButton.Left = this.navpadCenterButton.Left;
			this.navpadUpButton.Top = this.navpadCenterButton.Top - 2 - this.navpadUpButton.Height;

			this.navpadDownButton.SetSize(40, 27);
			this.navpadDownButton.Left = this.navpadCenterButton.Left;
			this.navpadDownButton.Top = this.navpadCenterButton.Top + this.navpadCenterButton.Height + 2;

			this.navpadRightButton.SetSize(27, 55);
			this.navpadRightButton.Left = this.navpadCenterButton.Left + this.navpadCenterButton.Width + 2;
			this.navpadRightButton.Top = this.navpadCenterButton.Top + (this.navpadCenterButton.Height / 2) - (this.navpadRightButton.Height / 2);

			this.navpadLeftButton.SetSize(27, 55);
			this.navpadLeftButton.Top = this.navpadRightButton.Top;
			this.navpadLeftButton.Left = this.navpadCenterButton.Left - 2 - this.navpadLeftButton.Width;


		}


		private void navpadCenterButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.EquationDrawPos.X = 10;
			this.EquationDrawPos.Y = 40;
		}

		private int navpadDecalValue = 30;
		private void navpadUpButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.EquationDrawPos.Y -= this.navpadDecalValue;
		}
		private void navpadDownButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.EquationDrawPos.Y += this.navpadDecalValue;
		}
		private void navpadRightButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.EquationDrawPos.X += this.navpadDecalValue;
		}
		private void navpadLeftButton_MouseLeftClick(object sender, EventArgs e)
		{
			this.EquationDrawPos.X -= this.navpadDecalValue;
		}

		#endregion
		#region SAVE ZONE

		private uiButton szMainSaveButton;
		private uiButton szSaveFileButton;
		private uiButton szSaveStringButton;

		private uiButton szMainLoadButton;
		private uiButton szLoadFileButton;
		private uiButton szLoadStringButton;

		private uiButton szAddEquTextButton;
		private uiButton szAddEquFileButton;


		private void TotalHideSaveZone()
		{
			this.szSetSaveVisibility(false);
			this.szSetLoadVisibility(false);
		}

		private bool szActualSaveVisibility = false;
		private void szSetSaveVisibility(bool newval)
		{
			this.szActualSaveVisibility = newval;
			this.szSaveFileButton.Visible = newval;
			this.szSaveStringButton.Visible = newval;
		}

		private bool szActualLoadVisibility = false;
		private void szSetLoadVisibility(bool newval)
		{
			this.szActualLoadVisibility = newval;
			this.szLoadFileButton.Visible = newval;
			this.szLoadStringButton.Visible = newval;
		}


		private void CreateSaveZone()
		{
			this.szMainSaveButton = new uiButton(this);
			this.szMainSaveButton.MouseLeftClick += new EventHandler(this.szMainSaveButton_MouseLeftClick);
			this.szSaveFileButton = new uiButton(this);
			this.szSaveFileButton.MouseLeftClick += new EventHandler(this.szSaveFileButton_MouseLeftClick);
			this.szSaveStringButton = new uiButton(this);
			this.szSaveStringButton.MouseLeftClick += new EventHandler(this.szSaveStringButton_MouseLeftClick);
			
			this.szMainLoadButton = new uiButton(this);
			this.szMainLoadButton.MouseLeftClick += new EventHandler(this.szMainLoadButton_MouseLeftClick);
			this.szLoadFileButton = new uiButton(this);
			this.szLoadFileButton.MouseLeftClick += new EventHandler(this.szLoadFileButton_MouseLeftClick);
			this.szLoadStringButton = new uiButton(this);
			this.szLoadStringButton.MouseLeftClick += new EventHandler(this.szLoadStringButton_MouseLeftClick);

			this.szAddEquTextButton = new uiButton(this);
			this.szAddEquTextButton.MouseLeftClick += new EventHandler(this.szAddEquTextButton_MouseLeftClick);
			this.szAddEquFileButton = new uiButton(this);
			this.szAddEquFileButton.MouseLeftClick += new EventHandler(this.szAddEquFileButton_MouseLeftClick);


			this.TotalHideSaveZone();
		}
		private void RefreshSaveZoneTextSize()
		{
			int MaxTop = this.navpadDownButton.Top + this.navpadDownButton.Height + 5;
			
			this.szMainSaveButton.Text = "Save";
			if (this.szActualSaveVisibility) { this.szMainSaveButton.Text += " -"; } else { this.szMainSaveButton.Text += " +"; }
			this.szMainSaveButton.SetSize(65, 23);
			this.szMainSaveButton.Top = MaxTop;
			this.szMainSaveButton.Left = this.Width - 5 - this.szMainLoadButton.Width;

			this.szSaveFileButton.Text = "Save as";
			this.szSaveFileButton.SetSize(65, 23);
			this.szSaveFileButton.Top = this.szMainSaveButton.Top;
			this.szSaveFileButton.Left = this.szMainSaveButton.Left - 5 - this.szSaveFileButton.Width;

			this.szSaveStringButton.Text = "Text";
			this.szSaveStringButton.SetSize(65, 23);
			this.szSaveStringButton.Top = this.szSaveFileButton.Top;
			this.szSaveStringButton.Left = this.szSaveFileButton.Left - 3 - this.szSaveStringButton.Width;
			


			this.szMainLoadButton.Text = "Load";
			if (this.szActualLoadVisibility) { this.szMainLoadButton.Text += " -"; } else { this.szMainLoadButton.Text += " +"; }
			this.szMainLoadButton.SetSize(65, 23);
			this.szMainLoadButton.Top = MaxTop + this.szMainSaveButton.Height + 3;
			this.szMainLoadButton.Left = this.szMainSaveButton.Left;

			this.szLoadFileButton.Text = "Load file";
			this.szLoadFileButton.SetSize(65, 23);
			this.szLoadFileButton.Top = this.szMainLoadButton.Top;
			this.szLoadFileButton.Left = this.szMainLoadButton.Left - 5 - this.szLoadFileButton.Width;

			this.szLoadStringButton.Text = "Text";
			this.szLoadStringButton.SetSize(65, 23);
			this.szLoadStringButton.Top = this.szLoadFileButton.Top;
			this.szLoadStringButton.Left = this.szLoadFileButton.Left - 3 - this.szLoadFileButton.Width;



			this.szAddEquTextButton.Text = "Add Equation\n    from text";
			this.szAddEquTextButton.SetSize(92, 35);
			this.szAddEquTextButton.Top = this.szMainLoadButton.Top + this.szMainLoadButton.Height + 5;
			this.szAddEquTextButton.Left = this.Width - 5 - this.szAddEquTextButton.Width;

			this.szAddEquFileButton.Text = "Add Equation\n    from file";
			this.szAddEquFileButton.SetSize(92, 35);
			this.szAddEquFileButton.Top = this.szAddEquTextButton.Top + this.szAddEquTextButton.Height + 3;
			this.szAddEquFileButton.Left = this.szAddEquTextButton.Left;



		}




		private void szMainSaveButton_MouseLeftClick(object sender, EventArgs e)
		{
			bool temp = this.szActualSaveVisibility;
			this.TotalHideSaveZone();
			this.szSetSaveVisibility(!temp);
			this.RefreshSaveZoneTextSize();
		}
		private void szSaveFileButton_MouseLeftClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			DialogResult rep = sfd.ShowDialog();
			if (rep == DialogResult.OK)
			{
				string filepath = sfd.FileName;
				if (filepath.Substring(filepath.Length - 4) != ".equ") { filepath += ".equ"; }
				this.TheEquation.SaveToFile(filepath);
			}
			this.TotalHideSaveZone();
			this.RefreshSaveZoneTextSize();
		}
		private void szSaveStringButton_MouseLeftClick(object sender, EventArgs e)
		{
			string thesave = this.TheEquation.GetSaveString();
			FormShowAskText fsat = new FormShowAskText();
			fsat.SetMode(FormShowAskText.saMode.Show);
			fsat.Text = thesave;
			fsat.ShowDialog();
			this.TotalHideSaveZone();
			this.RefreshSaveZoneTextSize();
		}


		private void szMainLoadButton_MouseLeftClick(object sender, EventArgs e)
		{
			bool temp = this.szActualLoadVisibility;
			this.TotalHideSaveZone();
			this.szSetLoadVisibility(!temp);
			this.RefreshSaveZoneTextSize();
		}
		private void szLoadFileButton_MouseLeftClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = false;
			DialogResult rep = ofd.ShowDialog();
			if (rep == DialogResult.OK)
			{
				string filepath = ofd.FileName;
				oEquation neweq = oEquation.LoadFromFile(filepath);
				//il ne faut pas créer un nouvel objet. il faut changer le contenu que this s'est fait assigné depuis l'extérieur
				//this.SetTheEquationToEdit(neweq);

				while (this.TheEquation.ListEquationObject.Count > 0) { this.TheEquation.ListEquationObject.RemoveAt(0); }
				this.TheEquation.ListEquationObject.AddRange(neweq.ListEquationObject);

			}
			this.TotalHideSaveZone();
			this.RefreshSaveZoneTextSize();
		}
		private void szLoadStringButton_MouseLeftClick(object sender, EventArgs e)
		{
			FormShowAskText fsaf = new FormShowAskText();
			fsaf.SetMode(FormShowAskText.saMode.Ask);
			fsaf.Message = "Entrez l'équation :";
			fsaf.ShowDialog();

			if (fsaf.TheExitMethod == FormShowAskText.saExitMethod.Ok)
			{
				oEquation neweq = oEquation.LoadFromSaveString(fsaf.Text);

				while (this.TheEquation.ListEquationObject.Count > 0) { this.TheEquation.ListEquationObject.RemoveAt(0); }
				this.TheEquation.ListEquationObject.AddRange(neweq.ListEquationObject);
			}

			this.TotalHideSaveZone();
			this.RefreshSaveZoneTextSize();
		}

		private void szAddEquTextButton_MouseLeftClick(object sender, EventArgs e)
		{
			FormShowAskText fsaf = new FormShowAskText();
			fsaf.SetMode(FormShowAskText.saMode.Ask);
			fsaf.Message = "Entrez l'équation :";
			fsaf.ShowDialog();

			if (fsaf.TheExitMethod == FormShowAskText.saExitMethod.Ok)
			{
				oEquation neweq = oEquation.LoadFromSaveString(fsaf.Text);

				this.SetToAddMode(neweq.ListEquationObject.ToArray());
			}
		}
		private void szAddEquFileButton_MouseLeftClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = false;
			DialogResult rep = ofd.ShowDialog();
			if (rep == DialogResult.OK)
			{
				string filepath = ofd.FileName;
				oEquation neweq = oEquation.LoadFromFile(filepath);
				//il ne faut pas créer un nouvel objet. il faut changer le contenu que this s'est fait assigné depuis l'extérieur
				//this.SetTheEquationToEdit(neweq);
				Application.DoEvents();
				this.SetToAddMode(neweq.ListEquationObject.ToArray());
			}
		}

		#endregion



		//ceci permet la creation de raccourci, puisque l'utilisateur peut alors de mettre en mode d'ajout juste en appuyant sur une touche.
		//on peut directement donner cette void au constructeur de KeyEventHandler
		public void SomeOne_KeyDown(object sender, KeyEventArgs e)
		{
			this.Exec_KeyDown(e.KeyCode);
		}
		public void Exec_KeyDown(Keys key)
		{
			switch (key)
			{
				//backspace pour deleter la selection
				case Keys.Back:
					if (this.listAllSelectedObject.Count > 0)
					{
						while (this.listAllSelectedObject.Count > 0)
						{
							oEquationObject eo = this.listAllSelectedObject[0];
							this.TheEquation.GetParentOfEquationObject(eo).ListEquationObject.Remove(eo);
							this.listAllSelectedObject.RemoveAt(0);
						}
					}
					break;
				case Keys.T:
					this.ToggleTrigButtonVisibility();
					break;


				case Keys.N:
					this.SetToAddMode(new EQoNumber("1"));
					break;
				case Keys.F:
					this.SetToAddMode(new EQoFraction());
					break;
				case Keys.R:
					this.SetToAddMode(new EQoSquareRoot());
					break;
				case Keys.P:
					this.SetToAddMode(new EQoParenthese());
					break;
				case Keys.V:
					this.SetToAddMode(new EQoVariable("x"));
					break;
				case Keys.E:
					this.SetToAddMode(new EQoExponent());
					break;



					
				case Keys.Add:
					this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Addition));
					break;
				case Keys.Subtract:
					this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Substraction));
					break;
				case Keys.Multiply:
					this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Multiplication));
					break;
				case Keys.Divide:
					this.SetToAddMode(new EQoOperator(EQoOperator.OperatorType.Division));
					break;

					

				default:
					//MessageBox.Show(key.ToString());
					break;
			}
			this.Refresh();
		}







		//========================================[ UI ELEMENTS ]========================================
		private List<uiButton> listUiButton = new List<uiButton>();
		private uiButton IsPointOnAnyButton(int x, int y)
		{
			uiButton rep = null;
			Rectangle prec = new Rectangle(x, y, 1, 1);
			foreach (uiButton b in this.listUiButton)
			{
				Rectangle brec = new Rectangle(b.Left, b.Top, b.Width, b.Height);
				if (brec.IntersectsWith(prec) && b.Visible)
				{
					rep = b;
					break;
				}
			}
			return rep;
		}
		private void GeneralMouseUp()
		{
			foreach (uiButton b in this.listUiButton)
			{
				b.ExecDownReset();
			}
		}


		private class uiButton
		{
			public oEquationEditer Parent;
			
			public Font Font = new Font("polices raster", 9f); // Font("consolas", 9f);
			public int Top = 0;
			public int Left = 0;
			public int Width = 70;
			public int Height = 20;
			public Color BackColor = Color.Silver;
			public Color DownColor = Color.White; //backcolor lorsque mouseleft is down
			public Bitmap BackImage = null;
			public bool Visible = true;
			public string Text = "notext";

			public void SetSize(int NewWidth, int NewHeight)
			{
				this.Width = NewWidth;
				this.Height = NewHeight;
			}

			private bool IsDownOnMe = false;
			public event EventHandler MouseLeftDown;
			public event EventHandler MouseLeftUp;
			public event EventHandler MouseLeftClick;
			public void Raise_MouseLeftDown()
			{
				if (this.Visible)
				{
					this.IsDownOnMe = true;
					if (this.MouseLeftDown != null) { this.MouseLeftDown(this, new EventArgs()); }
				}
			}
			public void Raise_MouseLeftUp()
			{
				if (this.Visible)
				{
					if (this.MouseLeftUp != null) { this.MouseLeftUp(this, new EventArgs()); }
					if (this.IsDownOnMe)
					{
						if (this.MouseLeftClick != null) { this.MouseLeftClick(this, new EventArgs()); }
					}
					this.IsDownOnMe = false;
				}
			}
			public void ExecDownReset()
			{
				this.IsDownOnMe = false;
			}

			//void new()
			public uiButton(oEquationEditer StartParent)
			{
				this.Parent = StartParent;
				this.Parent.listUiButton.Add(this);

			}

			public object Tag = null;
		}





	}
}
