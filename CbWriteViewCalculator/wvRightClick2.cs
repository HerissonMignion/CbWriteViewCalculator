using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
    public class wvRightClick2
    {
        //coordonner relative au rectangle graphique du rightclick
        public Point MousePos
        {
            get
            {
                Point rep = this.ImageBox.PointToClient(Cursor.Position);
                if (rep.X > this.ImageBox.Width - 1) { rep = new Point(-1, -1); }
                if (rep.Y > this.ImageBox.Height - 1) { rep = new Point(-1, -1); }
                return rep;
            }
        }
        public Rectangle MouseRec { get { return new Rectangle(this.MousePos, new Size(1, 1)); } }



        public struct sChoice
        {
            public bool IsSeparator;
            public string ChoiceName;
            public Font ChoiceFont;
            public sChoice(bool IsSep, string Name = "notext")
            {
                this.IsSeparator = IsSep;
                this.ChoiceName = Name;
                this.ChoiceFont = new Font("lucida console", 10); // consolas, 15
            }
            public Size GetChoiceSize()
            {
                Bitmap img = new Bitmap(10, 10);
                Graphics g = Graphics.FromImage(img);
                int Width = (int)(g.MeasureString(this.ChoiceName, this.ChoiceFont).Width);
                int Height = (int)(g.MeasureString("QWERTYqtyplkjhgfdb", this.ChoiceFont).Height);
                g.Dispose();
                img.Dispose();
                return new Size(Width, Height);
            }
            public override string ToString()
            {
                return this.ChoiceName;
            }
        }
        //liste et void de creation de la liste
        public List<sChoice> ListChoice = new List<sChoice>();
        public void AddChoice(string ChoiceName) { this.ListChoice.Add(new sChoice(false, ChoiceName)); }
        public void AddSeparator() { this.ListChoice.Add(new sChoice(true)); }
        public int GetMaxChoiceWidth()
        {
            int MaxWidth = 0;
            foreach (sChoice c in this.ListChoice)
            {
                int CWidth = c.GetChoiceSize().Width;
                if (CWidth > MaxWidth) { MaxWidth = CWidth; }
            }
            return MaxWidth;
        }



        public int ItemHeight = 22; // 20

        private Color zzzBackColor = Color.Gainsboro;
        private Brush zzzBackBrush = Brushes.Gainsboro;
        public Color BackColor
        {
            get { return this.zzzBackColor; }
            set
            {
                this.zzzBackColor = value;
                this.zzzBackBrush = new SolidBrush(value);
            }
        }
        public Brush BackBrush { get { return this.zzzBackBrush; } }

        private Color zzzForeColor = Color.FromArgb(32, 32, 32);
        private Brush zzzForeBrush = new SolidBrush(Color.FromArgb(32, 32, 32));
        public Color ForeColor
        {
            get { return this.zzzForeColor; }
            set
            {
                this.zzzForeColor = value;
                this.zzzForeBrush = new SolidBrush(value);
            }
        }
        public Brush ForeBrush { get { return this.zzzForeBrush; } }

        private Color zzzSelectedItemBackColor = Color.Silver;
        private Brush zzzSelectedItemBackBrush = Brushes.Silver;
        public Color SelectedItemBackColor
        {
            get { return this.zzzSelectedItemBackColor; }
            set
            {
                this.zzzSelectedItemBackColor = value;
                this.zzzSelectedItemBackBrush = new SolidBrush(value);
            }
        }
        public Brush SelectedItemBackBrush { get { return this.zzzSelectedItemBackBrush; } }

        private Color zzzBorderColor = Color.Gray;
        private Pen zzzBorderPen = Pens.Gray;
        public Color BorderColor
        {
            get { return this.zzzBorderColor; }
            set
            {
                this.zzzBorderColor = value;
                this.zzzBorderPen = new Pen(value);
            }
        }
        public Pen BorderPen { get { return this.zzzBorderPen; } }




        //ouvre le rightclick pour demander a l'utilisateur de choisir
        private string zzzTheChoice = "Annuler";
        public string GetChoice()
        {
            this.zzzTheChoice = "annuler";
            //quelque "constante"



            //determination de la position de la form
            this.forme.Location = Cursor.Position;

            //determination de la taille de la form
            int ActualHeight = 3;
            foreach (sChoice c in this.ListChoice)
            {
                if (c.IsSeparator)
                {
                    ActualHeight += 3;
                }
                else
                {
                    ActualHeight += this.ItemHeight;
                }
            }
            ActualHeight += 3;
            this.forme.Height = ActualHeight;

            //int FormWidth = 250; //250 represente la taille minimal de la form
            //int MaxChoiceWidth = this.GetMaxChoiceWidth() + 10;
            //if (MaxChoiceWidth > FormWidth) { FormWidth = MaxChoiceWidth; }
            this.forme.Width = this.FormWidth;


            //autre truc a faire avant l'affichage de la form



            //affichage de la form
            this.RefreshImage(); //devrait empecher que, parfois, lorsque le rightclick apparait, il n'y a aucun refreshimage qui a ete declancher et le rectangle est completement vide
            this.forme.ShowDialog();


            return this.zzzTheChoice;
        }





        private Form forme;
        private PictureBox ImageBox;

        public int FormWidth
        {
            get
            {
                int rep = this.GetMaxChoiceWidth() + 10;
                if (rep < 250) { rep = 250; }
                return rep;
            }
        }





        public void RefreshImage()
        {
            Bitmap img = new Bitmap(this.ImageBox.Width, this.ImageBox.Height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(this.BackColor);
            //g.DrawString("asdf", new Font("lucida console", 20), Brushes.White, 0f, 0f);

            g.DrawRectangle(this.BorderPen, 0, 0, img.Width - 1, img.Height - 1);
            


            //variable importante au dessin
            int ActualY = 3;
            int mousey = this.MousePos.Y; //le +2 est pour la hauteur du premier item
            int mousex = this.MousePos.X;
            Rectangle ActualBackColorRec = new Rectangle(3, ActualY, this.ImageBox.Width - 6, this.ItemHeight);
            
            
            //debut du dessin
            foreach (sChoice c in this.ListChoice)
            {

                //back color de l'item
                if (c.IsSeparator == false) //les separateur ne doivent pas avoir d'interaction graphique avec la sourie
                {
                    if (mousey >= ActualY && mousey < ActualY + this.ItemHeight && mousex > 1)
                    {
                        g.FillRectangle(this.SelectedItemBackBrush, ActualBackColorRec);
                    }
                }


                //text de l'item (ou la barre si c'est un separateur)
                if (c.IsSeparator == false)
                {
                    Size cSize = c.GetChoiceSize();
                    int cTextY = ActualY + (this.ItemHeight / 2) - (cSize.Height / 2);
                    g.DrawString(c.ChoiceName, c.ChoiceFont, this.ForeBrush, 3f, (float)cTextY);


                }
                else
                {
                    g.DrawLine(this.BorderPen, 4, ActualY + 1, this.ImageBox.Width - 5, ActualY + 1);
                }





                //prochaine iteration
                if (c.IsSeparator == false)
                {
                    ActualY += this.ItemHeight;
                    ActualBackColorRec.Y += this.ItemHeight;


                }
                else
                {
                    ActualY += 3;
                    ActualBackColorRec.Y += 3;


                }
            }





            g.Dispose();
            if (this.ImageBox.Image != null) { this.ImageBox.Image.Dispose(); }
            this.ImageBox.Image = img;
            this.ImageBox.Refresh();
        }

        //void new()
        public wvRightClick2()
        {
            this.forme = new Form();
            this.forme.StartPosition = FormStartPosition.Manual;
            this.forme.FormBorderStyle = FormBorderStyle.None;
            this.forme.Width = 300;
            this.forme.Height = 300;
            this.forme.MinimumSize = new Size(1, 1);
            this.forme.ShowInTaskbar = false;

            this.ImageBox = new PictureBox();
            this.ImageBox.Parent = this.forme;
            this.ImageBox.Dock = DockStyle.Fill;
            this.ImageBox.MouseMove += new MouseEventHandler(this.ImageBox_MouseMove);
            this.ImageBox.MouseLeave += new EventHandler(this.ImageBox_MouseLeave);
            this.ImageBox.MouseUp += new MouseEventHandler(this.ImageBox_MouseUp);


            //autre truc par default
            this.AddChoice("Annuler");

            


        }
        private void ImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.RefreshImage();
            GC.Collect();
        }
        private void ImageBox_MouseLeave(object sender, EventArgs e)
        {
            this.RefreshImage();
            GC.Collect();
        }

        private void ImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //definition des variable importante
                int ActualY = 3;
                int mousey = this.MousePos.Y; //le +2 est pour la hauteur du premier item
                int mousex = this.MousePos.X;


                //verificaiton pour tout les item
                foreach (sChoice c in this.ListChoice)
                {
                    if (c.IsSeparator == false)
                    {
                        if (mousey >= ActualY && mousey < ActualY + this.ItemHeight && mousex > 1)
                        {
                            this.zzzTheChoice = c.ChoiceName;
                            this.forme.Close();
                            break;
                        }
                    }

                    //prochaine iteration
                    if (c.IsSeparator == false)
                    {
                        ActualY += this.ItemHeight;

                    }
                    else
                    {
                        ActualY += 3;

                    }
                }



            }
        }



    }
}
