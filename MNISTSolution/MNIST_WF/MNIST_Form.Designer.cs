namespace MNIST_WF {
    partial class MNIST_Form {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.FreeDrawBox = new System.Windows.Forms.PictureBox();
            this.Clear = new System.Windows.Forms.Button();
            this.Evaluate = new System.Windows.Forms.Button();
            this.PredictionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FreeDrawBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FreeDrawBox
            // 
            this.FreeDrawBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FreeDrawBox.Location = new System.Drawing.Point(55, 55);
            this.FreeDrawBox.Name = "FreeDrawBox";
            this.FreeDrawBox.Size = new System.Drawing.Size(140, 140);
            this.FreeDrawBox.TabIndex = 0;
            this.FreeDrawBox.TabStop = false;
            this.FreeDrawBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FreeDrawBox_MouseDown);
            this.FreeDrawBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FreeDrawBox_MouseMove);
            this.FreeDrawBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FreeDrawBox_MouseUp);
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(201, 143);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(75, 23);
            this.Clear.TabIndex = 1;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // Evaluate
            // 
            this.Evaluate.Location = new System.Drawing.Point(201, 172);
            this.Evaluate.Name = "Evaluate";
            this.Evaluate.Size = new System.Drawing.Size(75, 23);
            this.Evaluate.TabIndex = 1;
            this.Evaluate.Text = "Evaluate";
            this.Evaluate.UseVisualStyleBackColor = true;
            this.Evaluate.Click += new System.EventHandler(this.Evaluate_Click);
            // 
            // PredictionLabel
            // 
            this.PredictionLabel.AutoSize = true;
            this.PredictionLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PredictionLabel.Location = new System.Drawing.Point(383, 110);
            this.PredictionLabel.Name = "PredictionLabel";
            this.PredictionLabel.Size = new System.Drawing.Size(44, 32);
            this.PredictionLabel.TabIndex = 2;
            this.PredictionLabel.Text = "---";
            // 
            // MNIST_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 275);
            this.Controls.Add(this.PredictionLabel);
            this.Controls.Add(this.Evaluate);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.FreeDrawBox);
            this.Name = "MNIST_Form";
            this.Text = "Machine Learned Digit Recognition (ml-mnist)";
            ((System.ComponentModel.ISupportInitialize)(this.FreeDrawBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox FreeDrawBox;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.Button Evaluate;
        private System.Windows.Forms.Label PredictionLabel;
    }
}

