namespace ComputerGraphicsImageEditor
{
    partial class ImageEditor
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.brightnessMinusButton = new System.Windows.Forms.Button();
            this.brightnessPlusButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.loadImageButton = new System.Windows.Forms.Button();
            this.contrastButton = new System.Windows.Forms.Button();
            this.negationButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.kernelWidthComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.editKernelButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.convolutionTemplateComboBox = new System.Windows.Forms.ComboBox();
            this.performConvolutionButton = new System.Windows.Forms.Button();
            this.gammaButton = new System.Windows.Forms.Button();
            this.originalImageBox = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.kernelHeightComboBox = new System.Windows.Forms.ComboBox();
            this.medianFilterButton = new System.Windows.Forms.Button();
            this.ditheringButton = new System.Windows.Forms.Button();
            this.kMeansButton = new System.Windows.Forms.Button();
            this.kMeansTxtBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.WriteImageButton = new System.Windows.Forms.Button();
            this.ReadImageButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(819, 653);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 690);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Brightness";
            // 
            // brightnessMinusButton
            // 
            this.brightnessMinusButton.Location = new System.Drawing.Point(167, 714);
            this.brightnessMinusButton.Name = "brightnessMinusButton";
            this.brightnessMinusButton.Size = new System.Drawing.Size(25, 25);
            this.brightnessMinusButton.TabIndex = 2;
            this.brightnessMinusButton.Text = "-";
            this.brightnessMinusButton.UseVisualStyleBackColor = true;
            this.brightnessMinusButton.Click += new System.EventHandler(this.brightnessMinusButton_Click);
            // 
            // brightnessPlusButton
            // 
            this.brightnessPlusButton.Location = new System.Drawing.Point(111, 714);
            this.brightnessPlusButton.Name = "brightnessPlusButton";
            this.brightnessPlusButton.Size = new System.Drawing.Size(25, 25);
            this.brightnessPlusButton.TabIndex = 3;
            this.brightnessPlusButton.Text = "+";
            this.brightnessPlusButton.UseVisualStyleBackColor = true;
            this.brightnessPlusButton.Click += new System.EventHandler(this.brightnessPlusButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(242, 656);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Functions";
            // 
            // loadImageButton
            // 
            this.loadImageButton.Location = new System.Drawing.Point(675, 712);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(100, 100);
            this.loadImageButton.TabIndex = 5;
            this.loadImageButton.Text = "Load Image";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // contrastButton
            // 
            this.contrastButton.Location = new System.Drawing.Point(256, 716);
            this.contrastButton.Name = "contrastButton";
            this.contrastButton.Size = new System.Drawing.Size(75, 23);
            this.contrastButton.TabIndex = 6;
            this.contrastButton.Text = "Contrast";
            this.contrastButton.UseVisualStyleBackColor = true;
            this.contrastButton.Click += new System.EventHandler(this.contrastButton_Click);
            // 
            // negationButton
            // 
            this.negationButton.Location = new System.Drawing.Point(256, 762);
            this.negationButton.Name = "negationButton";
            this.negationButton.Size = new System.Drawing.Size(75, 23);
            this.negationButton.TabIndex = 7;
            this.negationButton.Text = "Negation";
            this.negationButton.UseVisualStyleBackColor = true;
            this.negationButton.Click += new System.EventHandler(this.negationButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1083, 656);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Convolutions";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1648, 24);
            this.menuStrip1.TabIndex = 10;
            // 
            // kernelWidthComboBox
            // 
            this.kernelWidthComboBox.FormattingEnabled = true;
            this.kernelWidthComboBox.Location = new System.Drawing.Point(1088, 714);
            this.kernelWidthComboBox.Name = "kernelWidthComboBox";
            this.kernelWidthComboBox.Size = new System.Drawing.Size(121, 21);
            this.kernelWidthComboBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1088, 690);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Kernel Width";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // editKernelButton
            // 
            this.editKernelButton.Location = new System.Drawing.Point(1246, 756);
            this.editKernelButton.Name = "editKernelButton";
            this.editKernelButton.Size = new System.Drawing.Size(75, 23);
            this.editKernelButton.TabIndex = 13;
            this.editKernelButton.Text = "Edit Kernel";
            this.editKernelButton.UseVisualStyleBackColor = true;
            this.editKernelButton.Click += new System.EventHandler(this.editKernelButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1246, 690);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Convolution Template";
            // 
            // convolutionTemplateComboBox
            // 
            this.convolutionTemplateComboBox.FormattingEnabled = true;
            this.convolutionTemplateComboBox.Location = new System.Drawing.Point(1246, 714);
            this.convolutionTemplateComboBox.Name = "convolutionTemplateComboBox";
            this.convolutionTemplateComboBox.Size = new System.Drawing.Size(121, 21);
            this.convolutionTemplateComboBox.TabIndex = 15;
            this.convolutionTemplateComboBox.SelectedValueChanged += new System.EventHandler(this.convolutionTemplateComboBox_SelectedValueChanged);
            // 
            // performConvolutionButton
            // 
            this.performConvolutionButton.Location = new System.Drawing.Point(1088, 824);
            this.performConvolutionButton.Name = "performConvolutionButton";
            this.performConvolutionButton.Size = new System.Drawing.Size(150, 23);
            this.performConvolutionButton.TabIndex = 16;
            this.performConvolutionButton.Text = "Perform Convolution";
            this.performConvolutionButton.UseVisualStyleBackColor = true;
            this.performConvolutionButton.Click += new System.EventHandler(this.performConvolutionButton_Click);
            // 
            // gammaButton
            // 
            this.gammaButton.Location = new System.Drawing.Point(231, 805);
            this.gammaButton.Name = "gammaButton";
            this.gammaButton.Size = new System.Drawing.Size(129, 23);
            this.gammaButton.TabIndex = 17;
            this.gammaButton.Text = "Gamma Correction";
            this.gammaButton.UseVisualStyleBackColor = true;
            this.gammaButton.Click += new System.EventHandler(this.gammaButton_Click);
            // 
            // originalImageBox
            // 
            this.originalImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.originalImageBox.Location = new System.Drawing.Point(816, 0);
            this.originalImageBox.Name = "originalImageBox";
            this.originalImageBox.Size = new System.Drawing.Size(832, 653);
            this.originalImageBox.TabIndex = 18;
            this.originalImageBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1088, 756);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Kernel Height";
            // 
            // kernelHeightComboBox
            // 
            this.kernelHeightComboBox.FormattingEnabled = true;
            this.kernelHeightComboBox.Location = new System.Drawing.Point(1088, 791);
            this.kernelHeightComboBox.Name = "kernelHeightComboBox";
            this.kernelHeightComboBox.Size = new System.Drawing.Size(121, 21);
            this.kernelHeightComboBox.TabIndex = 20;
            // 
            // medianFilterButton
            // 
            this.medianFilterButton.Location = new System.Drawing.Point(256, 834);
            this.medianFilterButton.Name = "medianFilterButton";
            this.medianFilterButton.Size = new System.Drawing.Size(75, 23);
            this.medianFilterButton.TabIndex = 21;
            this.medianFilterButton.Text = "Median Filter";
            this.medianFilterButton.UseVisualStyleBackColor = true;
            this.medianFilterButton.Click += new System.EventHandler(this.medianFilterButton_Click);
            // 
            // ditheringButton
            // 
            this.ditheringButton.Location = new System.Drawing.Point(111, 762);
            this.ditheringButton.Name = "ditheringButton";
            this.ditheringButton.Size = new System.Drawing.Size(75, 23);
            this.ditheringButton.TabIndex = 22;
            this.ditheringButton.Text = "Dithering";
            this.ditheringButton.UseVisualStyleBackColor = true;
            this.ditheringButton.Click += new System.EventHandler(this.ditheringButton_Click);
            // 
            // kMeansButton
            // 
            this.kMeansButton.Location = new System.Drawing.Point(378, 714);
            this.kMeansButton.Name = "kMeansButton";
            this.kMeansButton.Size = new System.Drawing.Size(105, 23);
            this.kMeansButton.TabIndex = 23;
            this.kMeansButton.Text = "K-means Clustering";
            this.kMeansButton.UseVisualStyleBackColor = true;
            this.kMeansButton.Click += new System.EventHandler(this.kMeansButton_Click);
            // 
            // kMeansTxtBox
            // 
            this.kMeansTxtBox.Location = new System.Drawing.Point(378, 777);
            this.kMeansTxtBox.Name = "kMeansTxtBox";
            this.kMeansTxtBox.Size = new System.Drawing.Size(100, 20);
            this.kMeansTxtBox.TabIndex = 24;
            this.kMeansTxtBox.Text = "5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(375, 761);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Number of clusters:";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(378, 824);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(252, 23);
            this.progressBar1.TabIndex = 26;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // WriteImageButton
            // 
            this.WriteImageButton.Location = new System.Drawing.Point(816, 716);
            this.WriteImageButton.Name = "WriteImageButton";
            this.WriteImageButton.Size = new System.Drawing.Size(144, 23);
            this.WriteImageButton.TabIndex = 27;
            this.WriteImageButton.Text = "Write Image To Text File";
            this.WriteImageButton.UseVisualStyleBackColor = true;
            this.WriteImageButton.Click += new System.EventHandler(this.WriteImageButton_Click);
            // 
            // ReadImageButton
            // 
            this.ReadImageButton.Location = new System.Drawing.Point(816, 751);
            this.ReadImageButton.Name = "ReadImageButton";
            this.ReadImageButton.Size = new System.Drawing.Size(144, 23);
            this.ReadImageButton.TabIndex = 28;
            this.ReadImageButton.Text = "Read Image From Text File";
            this.ReadImageButton.UseVisualStyleBackColor = true;
            this.ReadImageButton.Click += new System.EventHandler(this.ReadImageButton_Click);
            // 
            // ImageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1648, 868);
            this.Controls.Add(this.ReadImageButton);
            this.Controls.Add(this.WriteImageButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.kMeansTxtBox);
            this.Controls.Add(this.kMeansButton);
            this.Controls.Add(this.ditheringButton);
            this.Controls.Add(this.medianFilterButton);
            this.Controls.Add(this.kernelHeightComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.originalImageBox);
            this.Controls.Add(this.gammaButton);
            this.Controls.Add(this.performConvolutionButton);
            this.Controls.Add(this.convolutionTemplateComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.editKernelButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.kernelWidthComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.negationButton);
            this.Controls.Add(this.contrastButton);
            this.Controls.Add(this.loadImageButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.brightnessPlusButton);
            this.Controls.Add(this.brightnessMinusButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ImageEditor";
            this.Text = "Image Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button brightnessMinusButton;
        private System.Windows.Forms.Button brightnessPlusButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadImageButton;
        private System.Windows.Forms.Button contrastButton;
        private System.Windows.Forms.Button negationButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ComboBox kernelWidthComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button editKernelButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox convolutionTemplateComboBox;
        private System.Windows.Forms.Button performConvolutionButton;
        private System.Windows.Forms.Button gammaButton;
        private System.Windows.Forms.PictureBox originalImageBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox kernelHeightComboBox;
        private System.Windows.Forms.Button medianFilterButton;
        private System.Windows.Forms.Button ditheringButton;
        private System.Windows.Forms.Button kMeansButton;
        private System.Windows.Forms.TextBox kMeansTxtBox;
        private System.Windows.Forms.Label label7;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button WriteImageButton;
        private System.Windows.Forms.Button ReadImageButton;
    }
}

