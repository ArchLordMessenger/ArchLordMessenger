namespace MacroSwitch
{
    partial class ColorPicker
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
            this.prgColor = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.lblColor = new System.Windows.Forms.Label();
            this.lblDebug = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgColor
            // 
            this.prgColor.Location = new System.Drawing.Point(249, 141);
            this.prgColor.Name = "prgColor";
            this.prgColor.Size = new System.Drawing.Size(204, 201);
            this.prgColor.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y";
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(12, 157);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(33, 20);
            this.txtX.TabIndex = 3;
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(83, 157);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(33, 20);
            this.txtY.TabIndex = 4;
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(12, 183);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(104, 23);
            this.btnSet.TabIndex = 5;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(12, 59);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(0, 13);
            this.lblColor.TabIndex = 6;
            // 
            // lblDebug
            // 
            this.lblDebug.AutoSize = true;
            this.lblDebug.Location = new System.Drawing.Point(12, 263);
            this.lblDebug.Name = "lblDebug";
            this.lblDebug.Size = new System.Drawing.Size(0, 13);
            this.lblDebug.TabIndex = 7;
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 354);
            this.Controls.Add(this.lblDebug);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prgColor);
            this.Name = "ColorPicker";
            this.Text = "ColorPicker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Label lblDebug;
    }
}