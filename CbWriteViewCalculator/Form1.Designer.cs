namespace CbWriteViewCalculator
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
			this.TestButton = new System.Windows.Forms.Button();
			this.TestButton2 = new System.Windows.Forms.Button();
			this.tbAnswer = new System.Windows.Forms.TextBox();
			this.cbUseAnsVariable = new System.Windows.Forms.CheckBox();
			this.cbCompileComplique = new System.Windows.Forms.CheckBox();
			this.ComputeButton = new CbWriteViewCalculator.wvCbButton2();
			this.GraphCalcButton = new CbWriteViewCalculator.wvCbButton2();
			this.SuspendLayout();
			// 
			// TestButton
			// 
			this.TestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TestButton.Location = new System.Drawing.Point(715, 12);
			this.TestButton.Name = "TestButton";
			this.TestButton.Size = new System.Drawing.Size(116, 25);
			this.TestButton.TabIndex = 1;
			this.TestButton.Text = "TestButton";
			this.TestButton.UseVisualStyleBackColor = true;
			this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
			// 
			// TestButton2
			// 
			this.TestButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TestButton2.Location = new System.Drawing.Point(715, 40);
			this.TestButton2.Name = "TestButton2";
			this.TestButton2.Size = new System.Drawing.Size(116, 25);
			this.TestButton2.TabIndex = 2;
			this.TestButton2.Text = "TestButton2";
			this.TestButton2.UseVisualStyleBackColor = true;
			this.TestButton2.Click += new System.EventHandler(this.TestButton2_Click);
			// 
			// tbAnswer
			// 
			this.tbAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAnswer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbAnswer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbAnswer.Location = new System.Drawing.Point(123, 12);
			this.tbAnswer.Name = "tbAnswer";
			this.tbAnswer.Size = new System.Drawing.Size(586, 26);
			this.tbAnswer.TabIndex = 4;
			// 
			// cbUseAnsVariable
			// 
			this.cbUseAnsVariable.AutoSize = true;
			this.cbUseAnsVariable.Checked = true;
			this.cbUseAnsVariable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbUseAnsVariable.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbUseAnsVariable.Location = new System.Drawing.Point(123, 45);
			this.cbUseAnsVariable.Name = "cbUseAnsVariable";
			this.cbUseAnsVariable.Size = new System.Drawing.Size(179, 22);
			this.cbUseAnsVariable.TabIndex = 5;
			this.cbUseAnsVariable.Text = "Utiliser la variable \"ans\"";
			this.cbUseAnsVariable.UseVisualStyleBackColor = true;
			this.cbUseAnsVariable.CheckedChanged += new System.EventHandler(this.cbUseAnsVariable_CheckedChanged);
			// 
			// cbCompileComplique
			// 
			this.cbCompileComplique.AutoSize = true;
			this.cbCompileComplique.Checked = true;
			this.cbCompileComplique.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbCompileComplique.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbCompileComplique.Location = new System.Drawing.Point(308, 45);
			this.cbCompileComplique.Name = "cbCompileComplique";
			this.cbCompileComplique.Size = new System.Drawing.Size(266, 22);
			this.cbCompileComplique.TabIndex = 6;
			this.cbCompileComplique.Text = "Compiler les équations compliquées";
			this.cbCompileComplique.UseVisualStyleBackColor = true;
			this.cbCompileComplique.CheckedChanged += new System.EventHandler(this.cbCompileComplique_CheckedChanged);
			// 
			// ComputeButton
			// 
			this.ComputeButton.BorderColor = System.Drawing.Color.DimGray;
			this.ComputeButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.ComputeButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.ComputeButton.FlatAppearance.BorderSize = 0;
			this.ComputeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ComputeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ComputeButton.IsMouseHandWhenOnButton = false;
			this.ComputeButton.Location = new System.Drawing.Point(12, 12);
			this.ComputeButton.MouseDownBorderColor = System.Drawing.Color.DimGray;
			this.ComputeButton.MouseOnBorderColor = System.Drawing.Color.Black;
			this.ComputeButton.Name = "ComputeButton";
			this.ComputeButton.Size = new System.Drawing.Size(105, 56);
			this.ComputeButton.TabIndex = 3;
			this.ComputeButton.Text = "=";
			this.ComputeButton.UseVisualStyleBackColor = true;
			this.ComputeButton.Click += new System.EventHandler(this.ComputeButton_Click);
			// 
			// GraphCalcButton
			// 
			this.GraphCalcButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.GraphCalcButton.BorderColor = System.Drawing.Color.DimGray;
			this.GraphCalcButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.GraphCalcButton.FlatAppearance.BorderSize = 0;
			this.GraphCalcButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GraphCalcButton.IsMouseHandWhenOnButton = false;
			this.GraphCalcButton.Location = new System.Drawing.Point(848, 12);
			this.GraphCalcButton.MouseDownBorderColor = System.Drawing.Color.DimGray;
			this.GraphCalcButton.MouseOnBorderColor = System.Drawing.Color.Black;
			this.GraphCalcButton.Name = "GraphCalcButton";
			this.GraphCalcButton.Size = new System.Drawing.Size(105, 56);
			this.GraphCalcButton.TabIndex = 7;
			this.GraphCalcButton.Text = "Calculatrice Graphique";
			this.GraphCalcButton.UseVisualStyleBackColor = true;
			this.GraphCalcButton.Click += new System.EventHandler(this.GraphCalcButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(965, 605);
			this.Controls.Add(this.GraphCalcButton);
			this.Controls.Add(this.cbCompileComplique);
			this.Controls.Add(this.cbUseAnsVariable);
			this.Controls.Add(this.tbAnswer);
			this.Controls.Add(this.ComputeButton);
			this.Controls.Add(this.TestButton2);
			this.Controls.Add(this.TestButton);
			this.Name = "Form1";
			this.Text = "CB Write View Calculator";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.Button TestButton2;
		private wvCbButton2 ComputeButton;
		private System.Windows.Forms.TextBox tbAnswer;
		private System.Windows.Forms.CheckBox cbUseAnsVariable;
		private System.Windows.Forms.CheckBox cbCompileComplique;
		private wvCbButton2 GraphCalcButton;
	}
}

