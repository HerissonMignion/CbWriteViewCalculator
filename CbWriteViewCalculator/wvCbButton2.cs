using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CbWriteViewCalculator
{
    public class wvCbButton2 : Button
    {
		

        private bool zzzIsMouseOnButton = false; //indique si la sourie est a l'interieur du button
        private bool zzzIsMouseDown = false; //indique si l'un des button de la sourie est presser sur le button
		
        private bool zzzIsMouseHandWhenOnButton = false; //indique si la sourie est en mode hand lorsqu'elle est sur le button;
        public bool IsMouseHandWhenOnButton
        {
            get { return this.zzzIsMouseHandWhenOnButton; }
            set
            {
                //this.zzzIsMouseHandWhenOnButton = value;
                //if (this.zzzIsMouseHandWhenOnButton == true)
                //{
                //    //check si la sourie est sur le button
                //    if (this.zzzIsMouseOnButton == true)
                //    {
                //        this.Cursor = Cursors.Hand;
                //    }
                //}
                //if (this.zzzIsMouseHandWhenOnButton == false)
                //{
                //    //check si la sourie est sur le button
                //    if (this.zzzIsMouseOnButton == true)
                //    {
                //        this.Cursor = Cursors.Default;
                //    }
                //}

                this.zzzIsMouseHandWhenOnButton = value;
                if (this.zzzIsMouseHandWhenOnButton == true) { this.Cursor = Cursors.Hand; } else { this.Cursor = Cursors.Default; }
            }
        }


        //sa valeur par default est dans la void new
        public Color BorderColor
        {
            get { return this.FlatAppearance.BorderColor; }
            set
            {
                this.FlatAppearance.BorderColor = value;
                this.Invalidate();
            }
        }

        //bordercolor lorsque la sourie est sur le button
        private Color zzzMouseOnBorderColor = Color.Black;
        public Color MouseOnBorderColor
        {
            get { return this.zzzMouseOnBorderColor; }
            set
            {
                this.zzzMouseOnBorderColor = value;
                this.Invalidate();
            }
        }

        //bordercolor lorsque le button de la sourie est baisser
        private Color zzzMouseDownBorderColor = Color.DimGray;
        public Color MouseDownBorderColor
        {
            get { return this.zzzMouseDownBorderColor; }
            set
            {
                this.zzzMouseDownBorderColor = value;
                this.Invalidate();
            }
        }










        public wvCbButton2()
        {
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.BorderColor = Color.DimGray;

            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;



        }



        protected override void OnMouseEnter(EventArgs e)
        {
            this.zzzIsMouseOnButton = true;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this.zzzIsMouseOnButton = false;
            base.OnMouseLeave(e);
        }


        //indique les button de la sourie qui sont presser. ces variable permette de gerer correctement la variable this.zzzIsMouseDown
        private bool zzzIsButtonDown_Left = false;
        private bool zzzIsButtonDown_Right = false;
        private bool zzzIsButtonDown_Middle = false;
        private bool zzzIsButtonDown_XButton1 = false;
        private bool zzzIsButtonDown_XButton2 = false;
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            switch (mevent.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    this.zzzIsButtonDown_Left = true;
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    this.zzzIsButtonDown_Right = true;
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    this.zzzIsButtonDown_Middle = true;
                    break;
                case System.Windows.Forms.MouseButtons.XButton1:
                    this.zzzIsButtonDown_XButton1 = true;
                    break;
                case System.Windows.Forms.MouseButtons.XButton2:
                    this.zzzIsButtonDown_XButton2 = true;
                    break;
            }

            if (this.zzzIsButtonDown_Left == true || this.zzzIsButtonDown_Right == true || this.zzzIsButtonDown_Middle == true || this.zzzIsButtonDown_XButton1 == true || this.zzzIsButtonDown_XButton2 == true)
            {
                this.zzzIsMouseDown = true;
            }
            else
            {
                this.zzzIsMouseDown = false;
            }


            base.OnMouseDown(mevent);
            this.Invalidate(); //make sure qu'il y aura un refresh
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            switch (mevent.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    this.zzzIsButtonDown_Left = false;
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    this.zzzIsButtonDown_Right = false;
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    this.zzzIsButtonDown_Middle = false;
                    break;
                case System.Windows.Forms.MouseButtons.XButton1:
                    this.zzzIsButtonDown_XButton1 = false;
                    break;
                case System.Windows.Forms.MouseButtons.XButton2:
                    this.zzzIsButtonDown_XButton2 = false;
                    break;
            }

            if (this.zzzIsButtonDown_Left == true || this.zzzIsButtonDown_Right == true || this.zzzIsButtonDown_Middle == true || this.zzzIsButtonDown_XButton1 == true || this.zzzIsButtonDown_XButton2 == true)
            {
                this.zzzIsMouseDown = true;
            }
            else
            {
                this.zzzIsMouseDown = false;
            }


            base.OnMouseUp(mevent);
            this.Invalidate();//make sure qu'il y aura un refresh
        }





        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            

            //dessine la bordure
            Pen PenBorder = new Pen(this.BorderColor);
            if (this.zzzIsMouseOnButton == true) { PenBorder = new Pen(this.MouseOnBorderColor); }
            if (this.zzzIsMouseDown == true) { PenBorder = new Pen(this.MouseDownBorderColor); }
            Rectangle BorderDrawRectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            pevent.Graphics.DrawRectangle(PenBorder, BorderDrawRectangle);

            


        }



        //empeche le dessin du rectangle de pointillier qui apparait lorsque l'utilisateur se sert du clavier
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }





    }
}
