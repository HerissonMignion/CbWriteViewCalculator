namespace CbWriteViewCalculator.GraphingCalculator
{
	partial class FormGraphingCalculator
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gbEquation = new System.Windows.Forms.GroupBox();
			this.flpEquations = new System.Windows.Forms.FlowLayoutPanel();
			this.btnGotoCalculator = new CbWriteViewCalculator.wvCbButton2();
			this.btnNewInequation = new CbWriteViewCalculator.wvCbButton2();
			this.btnNewFonction = new CbWriteViewCalculator.wvCbButton2();
			this.gbEquation.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbEquation
			// 
			this.gbEquation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbEquation.Controls.Add(this.btnNewInequation);
			this.gbEquation.Controls.Add(this.btnNewFonction);
			this.gbEquation.Controls.Add(this.flpEquations);
			this.gbEquation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.gbEquation.Location = new System.Drawing.Point(149, 1);
			this.gbEquation.Name = "gbEquation";
			this.gbEquation.Size = new System.Drawing.Size(814, 110);
			this.gbEquation.TabIndex = 0;
			this.gbEquation.TabStop = false;
			this.gbEquation.Text = "Équations";
			// 
			// flpEquations
			// 
			this.flpEquations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flpEquations.AutoScroll = true;
			this.flpEquations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flpEquations.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flpEquations.Location = new System.Drawing.Point(81, 11);
			this.flpEquations.Name = "flpEquations";
			this.flpEquations.Size = new System.Drawing.Size(727, 93);
			this.flpEquations.TabIndex = 0;
			// 
			// btnGotoCalculator
			// 
			this.btnGotoCalculator.BorderColor = System.Drawing.Color.DimGray;
			this.btnGotoCalculator.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnGotoCalculator.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.btnGotoCalculator.FlatAppearance.BorderSize = 0;
			this.btnGotoCalculator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnGotoCalculator.IsMouseHandWhenOnButton = false;
			this.btnGotoCalculator.Location = new System.Drawing.Point(1, 1);
			this.btnGotoCalculator.MouseDownBorderColor = System.Drawing.Color.DimGray;
			this.btnGotoCalculator.MouseOnBorderColor = System.Drawing.Color.Black;
			this.btnGotoCalculator.Name = "btnGotoCalculator";
			this.btnGotoCalculator.Size = new System.Drawing.Size(142, 71);
			this.btnGotoCalculator.TabIndex = 1;
			this.btnGotoCalculator.Text = "Calculatrice";
			this.btnGotoCalculator.UseVisualStyleBackColor = true;
			this.btnGotoCalculator.Click += new System.EventHandler(this.btnGotoCalculator_Click);
			// 
			// btnNewInequation
			// 
			this.btnNewInequation.BorderColor = System.Drawing.Color.DimGray;
			this.btnNewInequation.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnNewInequation.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.btnNewInequation.FlatAppearance.BorderSize = 0;
			this.btnNewInequation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNewInequation.IsMouseHandWhenOnButton = false;
			this.btnNewInequation.Location = new System.Drawing.Point(6, 62);
			this.btnNewInequation.MouseDownBorderColor = System.Drawing.Color.DimGray;
			this.btnNewInequation.MouseOnBorderColor = System.Drawing.Color.Black;
			this.btnNewInequation.Name = "btnNewInequation";
			this.btnNewInequation.Size = new System.Drawing.Size(69, 37);
			this.btnNewInequation.TabIndex = 2;
			this.btnNewInequation.Text = "Nouvelle Inéquation";
			this.btnNewInequation.UseVisualStyleBackColor = true;
			this.btnNewInequation.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnNewInequation_MouseClick);
			// 
			// btnNewFonction
			// 
			this.btnNewFonction.BorderColor = System.Drawing.Color.DimGray;
			this.btnNewFonction.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnNewFonction.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.btnNewFonction.FlatAppearance.BorderSize = 0;
			this.btnNewFonction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNewFonction.IsMouseHandWhenOnButton = false;
			this.btnNewFonction.Location = new System.Drawing.Point(6, 19);
			this.btnNewFonction.MouseDownBorderColor = System.Drawing.Color.DimGray;
			this.btnNewFonction.MouseOnBorderColor = System.Drawing.Color.Black;
			this.btnNewFonction.Name = "btnNewFonction";
			this.btnNewFonction.Size = new System.Drawing.Size(69, 37);
			this.btnNewFonction.TabIndex = 1;
			this.btnNewFonction.Text = "Nouvelle Fonction";
			this.btnNewFonction.UseVisualStyleBackColor = true;
			this.btnNewFonction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnNewFonction_MouseClick);
			// 
			// FormGraphingCalculator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(965, 662);
			this.Controls.Add(this.btnGotoCalculator);
			this.Controls.Add(this.gbEquation);
			this.Name = "FormGraphingCalculator";
			this.Text = "CB Graphing Calculator";
			this.Load += new System.EventHandler(this.FormGraphingCalculator_Load);
			this.gbEquation.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbEquation;
		private wvCbButton2 btnGotoCalculator;
		private System.Windows.Forms.FlowLayoutPanel flpEquations;
		private wvCbButton2 btnNewFonction;
		private wvCbButton2 btnNewInequation;
	}
}