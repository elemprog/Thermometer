namespace Thermometer
{
    partial class ColorForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxBack = new System.Windows.Forms.PictureBox();
            this.pictureBoxLines = new System.Windows.Forms.PictureBox();
            this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShutOff = new System.Windows.Forms.CheckBox();
            this.checkBoxLines = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownWidthShutOff = new System.Windows.Forms.NumericUpDown();
            this.pictureBoxShutOff = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownWidthGraph = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidthShutOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShutOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidthGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(139, 214);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(96, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(247, 214);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Background";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Scale Lines";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Graph Line";
            // 
            // pictureBoxBack
            // 
            this.pictureBoxBack.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxBack.Location = new System.Drawing.Point(128, 17);
            this.pictureBoxBack.Name = "pictureBoxBack";
            this.pictureBoxBack.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxBack.TabIndex = 6;
            this.pictureBoxBack.TabStop = false;
            this.pictureBoxBack.Click += new System.EventHandler(this.pictureBoxBack_Click);
            // 
            // pictureBoxLines
            // 
            this.pictureBoxLines.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxLines.Location = new System.Drawing.Point(128, 49);
            this.pictureBoxLines.Name = "pictureBoxLines";
            this.pictureBoxLines.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxLines.TabIndex = 7;
            this.pictureBoxLines.TabStop = false;
            this.pictureBoxLines.Click += new System.EventHandler(this.pictureBoxLines_Click);
            // 
            // pictureBoxGraph
            // 
            this.pictureBoxGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxGraph.Location = new System.Drawing.Point(128, 81);
            this.pictureBoxGraph.Name = "pictureBoxGraph";
            this.pictureBoxGraph.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxGraph.TabIndex = 8;
            this.pictureBoxGraph.TabStop = false;
            this.pictureBoxGraph.Click += new System.EventHandler(this.pictureBoxGraph_Click);
            // 
            // buttonDefault
            // 
            this.buttonDefault.Location = new System.Drawing.Point(15, 147);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(70, 30);
            this.buttonDefault.TabIndex = 3;
            this.buttonDefault.Text = "Set Default";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxShutOff);
            this.groupBox1.Controls.Add(this.checkBoxLines);
            this.groupBox1.Controls.Add(this.buttonDefault);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericUpDownWidthShutOff);
            this.groupBox1.Controls.Add(this.pictureBoxShutOff);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownWidthGraph);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pictureBoxGraph);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.pictureBoxLines);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.pictureBoxBack);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 192);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // checkBoxShutOff
            // 
            this.checkBoxShutOff.AutoSize = true;
            this.checkBoxShutOff.Location = new System.Drawing.Point(268, 116);
            this.checkBoxShutOff.Name = "checkBoxShutOff";
            this.checkBoxShutOff.Size = new System.Drawing.Size(51, 17);
            this.checkBoxShutOff.TabIndex = 8;
            this.checkBoxShutOff.Text = "show";
            // 
            // checkBoxLines
            // 
            this.checkBoxLines.AutoSize = true;
            this.checkBoxLines.Location = new System.Drawing.Point(268, 51);
            this.checkBoxLines.Name = "checkBoxLines";
            this.checkBoxLines.Size = new System.Drawing.Size(51, 17);
            this.checkBoxLines.TabIndex = 7;
            this.checkBoxLines.Text = "show";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(227, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "px";
            // 
            // numericUpDownWidthShutOff
            // 
            this.numericUpDownWidthShutOff.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.numericUpDownWidthShutOff.Location = new System.Drawing.Point(175, 114);
            this.numericUpDownWidthShutOff.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownWidthShutOff.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownWidthShutOff.Name = "numericUpDownWidthShutOff";
            this.numericUpDownWidthShutOff.ReadOnly = true;
            this.numericUpDownWidthShutOff.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownWidthShutOff.TabIndex = 6;
            this.numericUpDownWidthShutOff.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // pictureBoxShutOff
            // 
            this.pictureBoxShutOff.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxShutOff.Location = new System.Drawing.Point(128, 114);
            this.pictureBoxShutOff.Name = "pictureBoxShutOff";
            this.pictureBoxShutOff.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxShutOff.TabIndex = 13;
            this.pictureBoxShutOff.TabStop = false;
            this.pictureBoxShutOff.Click += new System.EventHandler(this.pictureBoxShutOff_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(227, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "px";
            // 
            // numericUpDownWidthGraph
            // 
            this.numericUpDownWidthGraph.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.numericUpDownWidthGraph.Location = new System.Drawing.Point(175, 81);
            this.numericUpDownWidthGraph.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numericUpDownWidthGraph.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownWidthGraph.Name = "numericUpDownWidthGraph";
            this.numericUpDownWidthGraph.ReadOnly = true;
            this.numericUpDownWidthGraph.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownWidthGraph.TabIndex = 5;
            this.numericUpDownWidthGraph.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Shut-Off Line";
            // 
            // ColorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Dialog ";
            this.Load += new System.EventHandler(this.ColorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidthShutOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShutOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidthGraph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxBack;
        private System.Windows.Forms.PictureBox pictureBoxLines;
        private System.Windows.Forms.PictureBox pictureBoxGraph;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownWidthGraph;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownWidthShutOff;
        private System.Windows.Forms.PictureBox pictureBoxShutOff;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxShutOff;
        private System.Windows.Forms.CheckBox checkBoxLines;
    }
}