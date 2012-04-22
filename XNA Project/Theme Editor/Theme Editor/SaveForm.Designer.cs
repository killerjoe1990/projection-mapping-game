namespace Theme_Editor
{
    partial class SaveForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_SaveName = new System.Windows.Forms.TextBox();
            this.btn_SaveOK = new System.Windows.Forms.Button();
            this.btn_SaveCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Theme Name:";
            // 
            // txt_SaveName
            // 
            this.txt_SaveName.Location = new System.Drawing.Point(92, 19);
            this.txt_SaveName.Name = "txt_SaveName";
            this.txt_SaveName.Size = new System.Drawing.Size(287, 20);
            this.txt_SaveName.TabIndex = 1;
            // 
            // btn_SaveOK
            // 
            this.btn_SaveOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_SaveOK.Location = new System.Drawing.Point(57, 76);
            this.btn_SaveOK.Name = "btn_SaveOK";
            this.btn_SaveOK.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveOK.TabIndex = 2;
            this.btn_SaveOK.Text = "OK";
            this.btn_SaveOK.UseVisualStyleBackColor = true;
            this.btn_SaveOK.Click += new System.EventHandler(this.btn_SaveOK_Click);
            // 
            // btn_SaveCancel
            // 
            this.btn_SaveCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_SaveCancel.Location = new System.Drawing.Point(250, 76);
            this.btn_SaveCancel.Name = "btn_SaveCancel";
            this.btn_SaveCancel.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveCancel.TabIndex = 3;
            this.btn_SaveCancel.Text = "Cancel";
            this.btn_SaveCancel.UseVisualStyleBackColor = true;
            // 
            // SaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 122);
            this.Controls.Add(this.btn_SaveCancel);
            this.Controls.Add(this.btn_SaveOK);
            this.Controls.Add(this.txt_SaveName);
            this.Controls.Add(this.label1);
            this.Name = "SaveForm";
            this.Text = "Save Theme";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_SaveName;
        private System.Windows.Forms.Button btn_SaveOK;
        private System.Windows.Forms.Button btn_SaveCancel;
    }
}