namespace MediaPlayerCtl
{
    partial class RegisnForm
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
            this.btnRegisn = new System.Windows.Forms.Button();
            this.textBoxRegisnCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRegisn
            // 
            this.btnRegisn.Location = new System.Drawing.Point(185, 181);
            this.btnRegisn.Name = "btnRegisn";
            this.btnRegisn.Size = new System.Drawing.Size(75, 23);
            this.btnRegisn.TabIndex = 0;
            this.btnRegisn.Text = "注册";
            this.btnRegisn.UseVisualStyleBackColor = true;
            this.btnRegisn.Click += new System.EventHandler(this.btnRegisn_Click);
            // 
            // textBoxRegisnCode
            // 
            this.textBoxRegisnCode.Location = new System.Drawing.Point(23, 183);
            this.textBoxRegisnCode.Name = "textBoxRegisnCode";
            this.textBoxRegisnCode.Size = new System.Drawing.Size(100, 21);
            this.textBoxRegisnCode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "注册失败，请重试";
            // 
            // RegisnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRegisnCode);
            this.Controls.Add(this.btnRegisn);
            this.Name = "RegisnForm";
            this.Text = "RegisnForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRegisn;
        private System.Windows.Forms.TextBox textBoxRegisnCode;
        private System.Windows.Forms.Label label1;
    }
}