using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
	public sealed class oCalculContextEditer
	{
		public Point MousePos { get { return this.ImageBox.PointToClient(Cursor.Position); } }
		public Rectangle MouseRec { get { return new Rectangle(this.MousePos, new Size(1, 1)); } }
		private bool IsMouseLeftDown = false;
		private bool IsMouseRightDown = false;
		private bool IsMouseAnyDown { get { return this.IsMouseLeftDown | this.IsMouseRightDown; } }


		private oEquationCalculContext zzzTheCalculContext;
		public oEquationCalculContext TheCalculContext { get { return this.zzzTheCalculContext; } }

		
		#region permission de l'utilisateur
		public bool AllowUserTo_AddVariable
		{
			get { return this.interAddVariableButton.Visible; }
			set
			{
				this.interAddVariableButton.Visible = value;
				this.Refresh();
			}
		}
		public bool AllowUserTo_AddFonction
		{
			get { return this.interAddFunctionButton.Visible; }
			set
			{
				this.interAddFunctionButton.Visible = value;
				this.Refresh();
			}
		}

		private bool zzzAUT_EditVariable = true;
		public bool AllowUserTo_EditVariable
		{
			get { return this.zzzAUT_EditVariable; }
			set
			{
				this.zzzAUT_EditVariable = value;
			}
		}
		private bool zzzAUT_RemoveVariable = true;
		public bool AllowUserTo_RemoveVariable
		{
			get { return this.zzzAUT_RemoveVariable; }
			set
			{
				this.zzzAUT_RemoveVariable = value;
			}
		}

		private bool zzzAUT_EditFonction = true;
		public bool AllowUserTo_EditFonction
		{
			get { return this.zzzAUT_EditFonction; }
			set
			{
				this.zzzAUT_EditFonction = value;
			}
		}
		private bool zzzAUT_RemoveFonction = true;
		public bool AllowUserTo_RemoveFonction
		{
			get { return this.zzzAUT_RemoveFonction; }
			set
			{
				this.zzzAUT_RemoveFonction = value;
			}
		}
		private bool zzzAUT_EditParamFonction = true;
		public bool AllowUserTo_EditParamFonction
		{
			get { return this.zzzAUT_EditParamFonction; }
			set
			{
				this.zzzAUT_EditParamFonction = value;
			}
		}

		public bool AllowUserTo_EditAngleUnit
		{
			get { return this.interAngleType.Visible; }
			set { this.interAngleType.Visible = value; this.Refresh(); }
		}
		
		#endregion


		private PictureBox ImageBox = new PictureBox();

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






		public void RefreshImage()
		{
			int imgWidth = this.Width;
			int imgHeight = this.Height;
			if (imgWidth < 100) { imgWidth = 100; }
			if (imgHeight < 100) { imgHeight = 100; }
			Bitmap img = new Bitmap(imgWidth, imgHeight);
			Graphics g = Graphics.FromImage(img);
			g.Clear(Color.White);

			Region DefaultClip = g.Clip;


			//----------[ variable ]----------
			g.Clip = new Region(this.GetVarZoneRectangle());
			//dessine graphiquement les variable
			if (this.TheCalculContext.AllVariable.Count <= 0)
			{
				g.DrawString("Aucune variable", new Font("ms sans serif", 9f), Brushes.Black, 5f, (float)(this.uiVarStart));
				
			}
			else
			{

				int ActualIndex = this.IndexOfTopItem;
				int ActualUiY = this.uiVarStart;


				while (ActualUiY <= this.uiFuncStart && ActualIndex <= this.TheCalculContext.AllVariable.Count - 1)
				{
					Variable ActualVar = this.TheCalculContext.AllVariable[ActualIndex];
					Rectangle itemrec = new Rectangle(2, ActualUiY, this.Width - this.uiScrollWidth - 5, this.uiItemHeight);

					if (ActualIndex == this.SelectedIndex) { g.FillRectangle(Brushes.LightBlue, itemrec); }

					g.DrawRectangle(Pens.DimGray, itemrec);

					Size NameSize = cWriteViewAssets.GetTextSize(ActualVar.Name, this.uiItemNameFont);
					int NameTop = ActualUiY + (this.uiItemHeight / 2) - (NameSize.Height / 2);
					g.DrawString(ActualVar.Name, this.uiItemNameFont, Brushes.Black, 5f, (float)NameTop);

					Size ValueSize = cWriteViewAssets.GetTextSize(ActualVar.Value.ToString(), this.uiItemValueFont);
					int ValueTop = ActualUiY + (this.uiItemHeight / 2) - (ValueSize.Height / 2);
					g.DrawString(" = " + ActualVar.Value.ToString(), this.uiItemValueFont, Brushes.Black, (float)(5 + NameSize.Width), (float)(ValueTop));


					//next iteration
					ActualIndex++;
					ActualUiY += this.uiItemHeight;
				}



			}
			
			//----------[ fonction ]----------
			g.Clip = new Region(this.GetFuncZoneRectangle());

			if (this.TheCalculContext.AllFonction.Count <= 0)
			{
				g.DrawString("Aucune fonction", new Font("ms sans serif", 9f), Brushes.Black, 5f, (float)(this.uiFuncStart));

			}
			else
			{

				int ActualIndex = this.funcIndexOfTopItem;
				int ActualUiY = this.uiFuncStart;
				while (ActualUiY <= imgHeight && ActualIndex <= this.TheCalculContext.AllFonction.Count - 1)
				{
					oFonction ActualFonc = this.TheCalculContext.AllFonction[ActualIndex];
					Rectangle itemrec = new Rectangle(2, ActualUiY, this.Width - this.uiScrollWidth - 5, this.uiItemHeight);

					if (ActualIndex == this.funcSelectedIndex) { g.FillRectangle(Brushes.LightBlue, itemrec); }

					g.DrawRectangle(Pens.DimGray, itemrec);

					string TextToWrite = ActualFonc.ToString();
					Size NameSize = cWriteViewAssets.GetTextSize(TextToWrite, this.uiItemNameFont);
					int NameTop = ActualUiY + (this.uiItemHeight / 2) - (NameSize.Height / 2);
					g.DrawString(TextToWrite, this.uiItemNameFont, Brushes.Black, 5f, (float)NameTop);



					//next iteration
					ActualIndex++;
					ActualUiY += this.uiItemHeight;
				}



			}

			

			g.Clip = DefaultClip;
			//====================================================================
			//uiButton
			foreach (uiButton b in this.listButton)
			{
				if (b.Visible)
				{
					Brush backbrush = Brushes.White;
					if (!b.IsMouseLeftDownOnYou) { backbrush = new SolidBrush(b.BackColor); }

					g.FillRectangle(backbrush, b.Rec);
					g.DrawRectangle(Pens.Black, b.Rec);

					Point TextPos = cWriteViewAssets.GetTextPosCenteredAt(b.Text, b.Font, b.Rec);
					g.DrawString(b.Text, b.Font, Brushes.Black, (PointF)TextPos);
				}
			}

			//uiLine
			foreach (uiLine l in this.listLine)
			{
				g.DrawLine(l.ThePen, l.p1, l.p2);
			}



			////écrit le titre des zone au dessus des séparateur
			//variable
			g.DrawString("Variables", new Font("ms sans serif", 9f), Brushes.Black, 3f, (float)(this.uiVarStart - 18));
			//fonction
			g.FillRectangle(Brushes.White, 2, this.uiFuncStart - 15, 60, 11);
			g.DrawString("Fonctions", new Font("ms sans serif", 9f), Brushes.Black, 3f, (float)(this.uiFuncStart - 18));



			g.Dispose();
			if (this.ImageBox.Image != null) { this.ImageBox.Image.Dispose(); }
			this.ImageBox.Image = img;
			this.ImageBox.Refresh();
			GC.Collect();
		}





		public void Refresh()
		{
			this.RefreshImage();


		}


		//void new()
		public oCalculContextEditer(oEquationCalculContext StartCalculContext)
		{
			this.zzzTheCalculContext = StartCalculContext;

			this.ImageBox = new PictureBox();
			this.ImageBox.BorderStyle = BorderStyle.FixedSingle;
			this.ImageBox.BackColor = Color.White;
			this.ImageBox.SizeChanged += new EventHandler(this.ImageBox_SizeChanged);
			this.ImageBox.MouseWheel += new MouseEventHandler(this.ImageBox_MouseWheel);
			this.ImageBox.MouseDown += new MouseEventHandler(this.ImageBox_MouseDown);
			this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);
			this.ImageBox.MouseMove += new MouseEventHandler(this.ImageBox_MouseMove);

			this.TheCalculContext.SomethingChanged += new EventHandler(this.TheCalculContext_SomethingChanged);
			

			//end
			this.CreateInterface();
			this.CreateScroll();
			this.RefreshTextSize();

		}
		private void ImageBox_SizeChanged(object sender, EventArgs e)
		{
			this.RefreshTextSize();
			this.Refresh();
		}
		private void ImageBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (this.IsMouseInsideVarZone())
			{
				this.ScrollUp(e.Delta / 119, true);
			}
			if (this.IsMouseInsideFuncZone())
			{
				this.funcScrollUp(e.Delta / 119, true);
			}
		}
		private void ImageBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.IsMouseLeftDown = true;

				if (this.IsMouseOnAnyControl())
				{
					this.Exec_MouseLeftDownOnEachControl();
				}
				else
				{

					//fait selectionner l'item sous la sourie
					if (this.IsMouseInsideVarZone())
					{
						int mouseindex = this.GetIndexOfMouse();
						if (this.IsIndexExist(mouseindex))
						{
							this.SelectIndex(mouseindex);

						}
						else
						{
							this.UnselectIndex();
						}
					}
					if (this.IsMouseInsideFuncZone())
					{
						int mouseindex = this.funcGetIndexOfMouse();
						if (this.funcIsIndexExist(mouseindex))
						{
							this.funcSelectIndex(mouseindex);

						}
						else
						{
							this.funcUnselectIndex();
						}
					}




				}
			}
			if (e.Button == MouseButtons.Right)
			{
				this.IsMouseRightDown = true;
				if (this.IsMouseOnAnyControl())
				{


				}
				else
				{

					//fait un rightclick
					if (!this.IsMouseLeftDown)
					{

						if (this.IsMouseInsideVarZone())
						{
							int mouseindex = this.GetIndexOfMouse();
							if (this.IsIndexExist(mouseindex))
							{
								this.SelectIndex(mouseindex);
								this.RefreshImage();

								string optEdit = "Edit...";
								string optRemove = "Remove";
								string optMoveUp = "Move up";
								string optMoveDown = "Move down";
								wvRightClick2 rc = new wvRightClick2();

								rc.AddSeparator();
								if (this.zzzAUT_EditVariable) { rc.AddChoice(optEdit); }
								if (this.zzzAUT_RemoveVariable) { rc.AddChoice(optRemove); }
								if (mouseindex > 0) { rc.AddChoice(optMoveUp); }
								if (mouseindex < this.TheCalculContext.AllVariable.Count - 1) { rc.AddChoice(optMoveDown); }
								string rep = rc.GetChoice();

								if (rep == optEdit)
								{
									Variable thevar = this.TheCalculContext.AllVariable[mouseindex];
									FormVariableEditer fve = new FormVariableEditer(thevar);

									fve.Title = "Édition de \"" + thevar.Name + "\"";
									fve.ShowDialog();

									if (fve.TheExitMethod == FormVariableEditer.ExitMethod.OkButton)
									{
										thevar.Name = fve.VarName;
										thevar.Value = Convert.ToDouble(fve.strVarValue.Replace(".", ","));
										//on signale qu'une variable a changé
										this.TheCalculContext.Raise_VariableValueChanged(thevar);
									}

								}
								if (rep == optRemove)
								{
									//this.TheCalculContext.AllVariable.RemoveAt(mouseindex);
									Variable thef = this.TheCalculContext.AllVariable[mouseindex];
									this.TheCalculContext.RemoveVariable(thef);
									this.UnselectIndex();
								}
								if (rep == optMoveUp)
								{
									Variable thevar = this.TheCalculContext.AllVariable[mouseindex];
									this.TheCalculContext.AllVariable.RemoveAt(mouseindex);
									this.TheCalculContext.AllVariable.Insert(mouseindex - 1, thevar);
									this.SelectedIndex--;
								}
								if (rep == optMoveDown)
								{
									Variable thevar = this.TheCalculContext.AllVariable[mouseindex];
									this.TheCalculContext.AllVariable.RemoveAt(mouseindex);
									this.TheCalculContext.AllVariable.Insert(mouseindex + 1, thevar);
									this.SelectedIndex++;
								}

								this.IsMouseRightDown = false;
							}
						}
						if (this.IsMouseInsideFuncZone())
						{
							int mouseindex = this.funcGetIndexOfMouse();
							if (this.funcIsIndexExist(mouseindex))
							{
								this.funcSelectIndex(mouseindex);
								this.RefreshImage();

								string optEdit = "Edit...";
								string optRemove = "Remove";
								string optMoveUp = "Move up";
								string optMoveDown = "Move down";
								wvRightClick2 rc = new wvRightClick2();

								rc.AddSeparator();
								if (this.zzzAUT_EditFonction) { rc.AddChoice(optEdit); }
								if (this.zzzAUT_RemoveFonction) { rc.AddChoice(optRemove); }
								if (mouseindex > 0) { rc.AddChoice(optMoveUp); }
								if (mouseindex < this.TheCalculContext.AllFonction.Count - 1) { rc.AddChoice(optMoveDown); }
								string rep = rc.GetChoice();

								if (rep == optEdit)
								{
									oFonction thef = this.TheCalculContext.AllFonction[mouseindex];
									FormFonctionEditer ffe = new FormFonctionEditer(thef, this.TheCalculContext);
									ffe.AllowParamEdition = this.zzzAUT_EditParamFonction;
									ffe.Show();
								}
								if (rep == optRemove)
								{
									//this.TheCalculContext.AllFonction.RemoveAt(mouseindex);
									oFonction thef = this.TheCalculContext.AllFonction[mouseindex];
									this.TheCalculContext.RemoveFonction(thef);
									this.funcUnselectIndex();
								}
								if (rep == optMoveUp)
								{
									oFonction thef = this.TheCalculContext.AllFonction[mouseindex];
									this.TheCalculContext.AllFonction.RemoveAt(mouseindex);
									this.TheCalculContext.AllFonction.Insert(mouseindex - 1, thef);
									this.funcSelectedIndex--;
								}
								if (rep == optMoveDown)
								{
									oFonction thef = this.TheCalculContext.AllFonction[mouseindex];
									this.TheCalculContext.AllFonction.RemoveAt(mouseindex);
									this.TheCalculContext.AllFonction.Insert(mouseindex + 1, thef);
									this.funcSelectedIndex++;
								}

								this.IsMouseRightDown = false;
							}
						}

					}


				}
			}
			this.Refresh();
		}
		private void ImageBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.IsMouseLeftDown = false;

				this.Exec_MouseLeftUpOnEachControl();

			}
			if (e.Button == MouseButtons.Right)
			{
				this.IsMouseRightDown = false;


			}
			this.Refresh();
		}

		private int MouseMove_FrameSkip = 3;
		private int MouseMove_frameSkip_State = 0;
		private void ImageBox_MouseMove(object sender, MouseEventArgs e)
		{
			//Graphics g = this.ImageBox.CreateGraphics();
			//this.ImageBox.Refresh();
			//g.DrawString(this.IsMouseAnyDown.ToString(), new Font("consolas", 10f), Brushes.Red, 0f, 0f);

			this.MouseMove_frameSkip_State++;
			if (this.MouseMove_frameSkip_State >= this.MouseMove_FrameSkip)
			{

				if (this.IsMouseAnyDown)
				{
					if (this.IsMouseInsideVarZone())
					{
						int mouseindex = this.GetIndexOfMouse();
						if (this.IsIndexExist(mouseindex))
						{
							this.SelectIndex(mouseindex);
						}
					}
					if (this.IsMouseInsideFuncZone())
					{
						int mouseindex = this.funcGetIndexOfMouse();
						if (this.funcIsIndexExist(mouseindex))
						{
							this.funcSelectIndex(mouseindex);
						}
					}
				}




				this.MouseMove_frameSkip_State = 0;
				this.Refresh();
			}
		}

		
		private void TheCalculContext_SomethingChanged(object sender, EventArgs e)
		{
			this.Refresh();
		}



		//interface graphique des zone de scroll
		private uiButton scrollScrollUpButton;
		private uiButton scrollScrollDownButton;
		private uiButton scrollFuncScrollUpButton;
		private uiButton scrollFuncScrollDownButton;
		private void CreateScroll()
		{
			this.scrollScrollUpButton = new uiButton(this);
			this.scrollScrollUpButton.MouseLeftDown += new EventHandler(this.ScrollUpButton_MouseLeftDown);

			this.scrollScrollDownButton = new uiButton(this);
			this.scrollScrollDownButton.MouseLeftDown += new EventHandler(this.ScrollDownButton_MouseLeftDown);

			this.scrollFuncScrollUpButton = new uiButton(this);
			this.scrollFuncScrollUpButton.MouseLeftDown += new EventHandler(this.FuncScrollUpButton_MouseLeftDown);

			this.scrollFuncScrollDownButton = new uiButton(this);
			this.scrollFuncScrollDownButton.MouseLeftDown += new EventHandler(this.FuncScrollDownButton_MouseLeftDown);

		}
		private void scrollRefreshTextSize()
		{
			this.scrollScrollUpButton.Left = this.Width - this.uiScrollWidth;
			this.scrollScrollUpButton.Width = this.uiScrollWidth - 5;
			this.scrollScrollUpButton.Height = 30;
			this.scrollScrollUpButton.Top = this.uiVarStart;
			this.scrollScrollUpButton.Text = "/\\";
			
			this.scrollScrollDownButton.Left = this.Width - this.uiScrollWidth;
			this.scrollScrollDownButton.Width = this.uiScrollWidth - 5;
			this.scrollScrollDownButton.Height = 30;
			this.scrollScrollDownButton.Top = this.scrollScrollUpButton.Top + this.scrollScrollUpButton.Height + 2;
			this.scrollScrollDownButton.Text = "\\/";


			this.scrollFuncScrollUpButton.Left = this.Width - this.uiScrollWidth;
			this.scrollFuncScrollUpButton.Width = this.uiScrollWidth - 5;
			this.scrollFuncScrollUpButton.Height = 30;
			this.scrollFuncScrollUpButton.Top = this.uiFuncStart;
			this.scrollFuncScrollUpButton.Text = "/\\";

			this.scrollFuncScrollDownButton.Left = this.Width - this.uiScrollWidth;
			this.scrollFuncScrollDownButton.Width = this.uiScrollWidth - 5;
			this.scrollFuncScrollDownButton.Height = 30;
			this.scrollFuncScrollDownButton.Top = this.scrollFuncScrollUpButton.Top + this.scrollFuncScrollUpButton.Height + 2;
			this.scrollFuncScrollDownButton.Text = "\\/";

		}

		private int ScrollButtonLength = 3;
		private void ScrollUpButton_MouseLeftDown(object sender, EventArgs e)
		{
			this.ScrollUp(this.ScrollButtonLength, true);
		}
		private void ScrollDownButton_MouseLeftDown(object sender, EventArgs e)
		{
			this.ScrollDown(this.ScrollButtonLength, true);
		}
		private void FuncScrollUpButton_MouseLeftDown(object sender, EventArgs e)
		{
			this.funcScrollUp(this.ScrollButtonLength, true);
		}
		private void FuncScrollDownButton_MouseLeftDown(object sender, EventArgs e)
		{
			this.funcScrollDown(this.ScrollButtonLength, true);
		}

		#region SCROLL VARIABLE
		private int IndexOfTopItem = 0;
		private int GetIndexOfMouse()
		{
			int rep = this.IndexOfTopItem;
			int my = this.MousePos.Y;
			rep += (my - this.uiVarStart) / this.uiItemHeight;
			return rep;
		}

		//make sure que le scrolling est dans les bound
		private void CheckScroll()
		{
			if (this.IndexOfTopItem > this.TheCalculContext.AllVariable.Count - 1) { this.IndexOfTopItem = this.TheCalculContext.AllVariable.Count - 1; }
			if (this.IndexOfTopItem < 0) { this.IndexOfTopItem = 0; }
		}
		private void ScrollUp(int length, bool CallRefresh = true)
		{
			this.IndexOfTopItem -= length;
			this.CheckScroll();
			if (CallRefresh) { this.Refresh(); }
		}
		private void ScrollDown(int length, bool CallRefresh = true)
		{
			this.IndexOfTopItem += length;
			this.CheckScroll();
			if (CallRefresh) { this.Refresh(); }
		}

		private int SelectedIndex = -1;
		private bool IsAnyIndexSelected { get { return this.IsIndexExist(this.SelectedIndex); } }
		private void SelectIndex(int index)
		{
			this.SelectedIndex = index;
		}
		private void UnselectIndex() { this.SelectedIndex = -1; }
		

		private bool IsIndexExist(int index)
		{
			if (index < 0) { return false; }
			if (index >= this.TheCalculContext.AllVariable.Count) { return false; }
			return true;
		}


		#endregion
		#region SCROLL FONCTION
		private int funcIndexOfTopItem = 0;
		private int funcGetIndexOfMouse()
		{
			int rep = this.funcIndexOfTopItem;
			int my = this.MousePos.Y;
			rep += (my - this.uiFuncStart) / this.uiItemHeight;
			return rep;
		}

		//make sure que le scrolling est dans les bound
		private void funcCheckScroll()
		{
			if (this.funcIndexOfTopItem > this.TheCalculContext.AllFonction.Count - 1) { this.funcIndexOfTopItem = this.TheCalculContext.AllFonction.Count - 1; }
			if (this.funcIndexOfTopItem < 0) { this.funcIndexOfTopItem = 0; }
		}
		private void funcScrollUp(int length, bool CallRefresh = true)
		{
			this.funcIndexOfTopItem -= length;
			this.funcCheckScroll();
			if (CallRefresh) { this.Refresh(); }
		}
		private void funcScrollDown(int length, bool CallRefresh = true)
		{
			this.funcIndexOfTopItem += length;
			this.funcCheckScroll();
			if (CallRefresh) { this.Refresh(); }
		}

		private int funcSelectedIndex = -1;
		private bool funcIsAnyIndexSelected { get { return this.funcIsIndexExist(this.funcSelectedIndex); } }
		private void funcSelectIndex(int index)
		{
			this.funcSelectedIndex = index;
		}
		private void funcUnselectIndex() { this.funcSelectedIndex = -1; }


		private bool funcIsIndexExist(int index)
		{
			if (index < 0) { return false; }
			if (index >= this.TheCalculContext.AllFonction.Count) { return false; }
			return true;
		}

		#endregion
		#region INTERFACE GRAPHIQUE

		private int uiVarStart = 70; // 70 position verticale de depart de l'affichage des variable
		private int uiFuncStart = 200; //position verticale de depart de l'affichage des fonction
		private Rectangle GetVarZoneRectangle()
		{
			Rectangle vr = new Rectangle(0, 0, 10, 10);
			vr.Y = this.uiVarStart;
			vr.X = 2;
			vr.Width = this.Width - vr.X - 3 - this.uiScrollWidth + 1;
			vr.Height = this.uiFuncStart - this.uiVarStart - 3;
			return vr;
		}
		private Rectangle GetFuncZoneRectangle()
		{
			Rectangle fr = new Rectangle(0, 0, 10, 10);
			fr.Y = this.uiFuncStart;
			fr.X = 2;
			fr.Width = this.Width - fr.X - 3 - this.uiScrollWidth + 1;
			fr.Height = this.Height - this.uiFuncStart - 3;
			return fr;
		}

		private int uiItemHeight = 30;
		private Font uiItemNameFont = new Font("ms sans serif", 9f); // "ms sans serif"
		private Font uiItemValueFont = new Font("consolas", 9f);

		private int uiScrollWidth = 30;

		//determine si la souris est à l'interieure de la zone dans laquel se situent les variable (ceci EXCLUT la scroll bar)
		private bool IsMouseInsideVarZone()
		{
			Rectangle mrec = this.MouseRec;
			Rectangle vr = this.GetVarZoneRectangle();
			return vr.IntersectsWith(mrec);
		}
		private bool IsMouseInsideFuncZone()
		{
			Rectangle mrec = this.MouseRec;
			Rectangle fr = this.GetFuncZoneRectangle();
			return fr.IntersectsWith(mrec);
		}




		private uiButton interAngleType;
		private uiLine interVarSepLine;
		private uiLine interFuncSepLine;
		
		private uiButton interAddVariableButton;
		private uiButton interAddFunctionButton;


		private void CreateInterface()
		{
			this.interAngleType = new uiButton(this);
			this.interAngleType.MouseLeftUp += new EventHandler(this.interAngleType_MouseLeftUp);

			this.interVarSepLine = new uiLine(this);
			this.interFuncSepLine = new uiLine(this);

			this.interAddVariableButton = new uiButton(this);
			this.interAddVariableButton.MouseLeftUp += new EventHandler(this.interAddVariableButton_MouseLeftUp);

			this.interAddFunctionButton = new uiButton(this);
			this.interAddFunctionButton.MouseLeftUp += new EventHandler(this.interAddFunctionButton_MouseLeftUp);
			
		}
		public void RefreshTextSize()
		{
			this.interAngleType.SetSize(100, 25);
			this.interAngleType.SetLocation(this.Width - 7 - this.interAngleType.Width, 5);
			if (this.TheCalculContext.ActualAngleType == oEquationCalculContext.AngleType.Degree) { this.interAngleType.Text = "Degré"; }
			if (this.TheCalculContext.ActualAngleType == oEquationCalculContext.AngleType.Radian) { this.interAngleType.Text = "Radian"; }
			if (this.TheCalculContext.ActualAngleType == oEquationCalculContext.AngleType.Gradian) { this.interAngleType.Text = "Grade"; }

			this.uiFuncStart = (this.Height + this.uiVarStart) / 2;

			this.interVarSepLine.ThePen = new Pen(Color.Silver, 3f);
			this.interVarSepLine.p1 = new Point(5, this.uiVarStart - 3);
			this.interVarSepLine.p2 = new Point(this.Width - 6, this.uiVarStart - 3);

			this.interFuncSepLine.ThePen = new Pen(Color.Silver, 3f);
			this.interFuncSepLine.p1 = new Point(5, this.uiFuncStart - 3);
			this.interFuncSepLine.p2 = new Point(this.Width - 6, this.uiFuncStart - 3);


			this.interAddVariableButton.SetLocation(5, 5);
			this.interAddVariableButton.SetSize(150, 20);
			this.interAddVariableButton.Text = "Nouvelle variable";

			this.interAddFunctionButton.SetLocation(this.interAddVariableButton.Left, this.interAddVariableButton.Top + this.interAddVariableButton.Height + 3);
			this.interAddFunctionButton.SetSize(150, 20);
			this.interAddFunctionButton.Text = "Nouvelle fonction";


			this.scrollRefreshTextSize();
		}


		//events
		private void interAngleType_MouseLeftUp(object sender, EventArgs e)
		{
			switch (this.TheCalculContext.ActualAngleType)
			{
				case oEquationCalculContext.AngleType.Degree:
					this.TheCalculContext.ActualAngleType = oEquationCalculContext.AngleType.Radian;
					break;
				case oEquationCalculContext.AngleType.Radian:
					this.TheCalculContext.ActualAngleType = oEquationCalculContext.AngleType.Gradian;
					break;
				case oEquationCalculContext.AngleType.Gradian:
					this.TheCalculContext.ActualAngleType = oEquationCalculContext.AngleType.Degree;
					break;
			}
			this.RefreshTextSize();
		}
		private void interAddVariableButton_MouseLeftUp(object sender, EventArgs e)
		{
			FormVariableEditer fve = new FormVariableEditer();
			fve.Title = "Créer une variable";
			fve.ShowDialog();
			if (fve.TheExitMethod == FormVariableEditer.ExitMethod.OkButton)
			{
				Variable newvar = new Variable(fve.VarName, Convert.ToDouble(fve.strVarValue.Replace(".", ",")));
				this.TheCalculContext.AddVariable(newvar);
			}
		}
		private void interAddFunctionButton_MouseLeftUp(object sender, EventArgs e)
		{
			//oFonction newf = new oFonction("f" + this.TheCalculContext.AllFonction.Count.ToString(), false);
			//this.TheCalculContext.AllFonction.Add(newf);

			FormGetFonctionName fgfn = new FormGetFonctionName();
			fgfn.Title = "Créer une fonction";
			fgfn.Message = "Entrez le nom de la fonction à créer :";
			fgfn.Answer = "f";
			fgfn.ShowDialog();
			if (fgfn.TheExitMethod == FormGetFonctionName.ExitMethod.OkButton)
			{
				string newfname = fgfn.Answer;
				oFonction newf = new oFonction(newfname, false);
				//this.TheCalculContext.AllFonction.Add(newf);
				this.TheCalculContext.AddFonction(newf);
			}

		}

		#endregion






		#region TESTEST

		//private uiButton testB1;
		//private uiButton testB2;

		//private void testCreate()
		//{
		//	this.testB1 = new uiButton(this);
		//	this.testB1.Text = "button 1";
		//	this.testB1.Top = 50;
		//	this.testB1.Left = 100;
		//	this.testB1.MouseLeftDown += new EventHandler(this.b1down);
		//	this.testB1.MouseLeftUp += new EventHandler(this.b1up);

		//	this.testB2 = new uiButton(this);
		//	this.testB2.Text = "button 2";
		//	this.testB2.Top = 10;
		//	this.testB2.Left = 10;
		//	this.testB2.MouseLeftDown += new EventHandler(this.b2down);
		//	this.testB2.MouseLeftUp += new EventHandler(this.b2up);

		//}

		//private void b1down(object sender, EventArgs e)
		//{
		//	Program.wdebug("1 down");
		//}
		//private void b1up(object sender, EventArgs e)
		//{
		//	Program.wdebug("1 up");
		//}
		//private void b2down(object sender, EventArgs e)
		//{
		//	Program.wdebug("2 down");
		//}
		//private void b2up(object sender, EventArgs e)
		//{
		//	Program.wdebug("2 up");
		//}





		#endregion





		//================================== UI CONTROLS =====================================
		private List<uiButton> listButton = new List<uiButton>();
		private List<uiLine> listLine = new List<uiLine>();


		private bool IsMouseOnAnyControl()
		{
			bool rep = false;
			Rectangle mrec = this.MouseRec;
			foreach (uiButton b in this.listButton)
			{
				if (b.Visible)
				{
					if (b.Rec.IntersectsWith(mrec))
					{
						rep = true;
						break;
					}
				}
			}
			return rep;
		}
		private void Exec_MouseLeftDownOnEachControl()
		{
			Rectangle mrec = this.MouseRec;
			foreach (uiButton b in this.listButton)
			{
				if (b.Visible && b.Rec.IntersectsWith(mrec))
				{
					b.MouseLeftDownOnYou();
				}
			}
		}
		private void Exec_MouseLeftUpOnEachControl()
		{
			foreach (uiButton b in this.listButton)
			{
				b.GeneralMouseLeftUp();
			}
		}




		private class uiButton
		{
			private oCalculContextEditer zzzParent;
			public oCalculContextEditer Parent { get { return this.zzzParent; } }

			public Rectangle Rec = new Rectangle(0, 0, 100, 30);
			public int Top
			{
				get { return this.Rec.Y; }
				set { this.Rec.Y = value; }
			}
			public int Left
			{
				get { return this.Rec.X; }
				set { this.Rec.X = value; }
			}
			public int Width
			{
				get { return this.Rec.Width; }
				set { this.Rec.Width = value; }
			}
			public int Height
			{
				get { return this.Rec.Height; }
				set { this.Rec.Height = value; }
			}
			public bool Visible = true;
			public string Text = "notext";
			public Font Font = new Font("consolas", 10f);
			public Color BackColor = Color.Silver;

			public void SetLocation(int newLeft, int newTop)
			{
				this.Rec.X = newLeft;
				this.Rec.Y = newTop;
			}
			public void SetSize(int newWidth, int newHeight)
			{
				this.Rec.Width = newWidth;
				this.Rec.Height = newHeight;
			}


			//void new()
			public uiButton(oCalculContextEditer StartParent)
			{
				this.zzzParent = StartParent;
				StartParent.listButton.Add(this);
				
			}
			
			private bool IsMouseLeftDownOnThis = false;
			public bool IsMouseLeftDownOnYou { get { return this.IsMouseLeftDownOnThis; } }

			public event EventHandler MouseLeftDown;
			public event EventHandler MouseLeftUp;

			private void Raise_MouseLeftDown(bool Force = false)
			{
				if (this.Visible || Force)
				{
					if (this.MouseLeftDown != null)
					{
						this.MouseLeftDown(this, new EventArgs());
					}
				}
			}
			private void Raise_MouseLeftUp(bool Force = false)
			{
				if (this.Visible || Force)
				{
					if (this.MouseLeftUp != null)
					{
						this.MouseLeftUp(this, new EventArgs());
					}
				}
			}
			
			public void MouseLeftDownOnYou()
			{
				if (this.Visible)
				{
					this.IsMouseLeftDownOnThis = true;
					this.Raise_MouseLeftDown(false);
				}
			}
			public void GeneralMouseLeftUp()
			{
				if (this.IsMouseLeftDownOnThis)
				{
					this.Raise_MouseLeftUp(true);
				}
				this.IsMouseLeftDownOnThis = false;
			}
			
		}
		private class uiLine
		{
			private oCalculContextEditer zzzParent;
			public oCalculContextEditer Parent { get { return this.zzzParent; } }

			public Pen ThePen = Pens.Black;

			public Point p1 = new Point(0, 0);
			public Point p2 = new Point(10, 10);


			//void new()
			public uiLine(oCalculContextEditer StartParent)
			{
				this.zzzParent = StartParent;
				StartParent.listLine.Add(this);

			}

		}



	}
}
