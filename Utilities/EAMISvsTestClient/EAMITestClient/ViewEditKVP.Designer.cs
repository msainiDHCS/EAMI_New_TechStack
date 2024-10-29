namespace EAMITestClient
{
    partial class ViewEditKVP
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
            this.txtR1Kvp1Value = new System.Windows.Forms.TextBox();
            this.label96 = new System.Windows.Forms.Label();
            this.txtR1Kvp1Name = new System.Windows.Forms.TextBox();
            this.label92 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtR1Kvp1Value
            // 
            this.txtR1Kvp1Value.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtR1Kvp1Value.Location = new System.Drawing.Point(64, 37);
            this.txtR1Kvp1Value.Name = "txtR1Kvp1Value";
            this.txtR1Kvp1Value.Size = new System.Drawing.Size(181, 13);
            this.txtR1Kvp1Value.TabIndex = 199;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label96.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label96.Location = new System.Drawing.Point(24, 37);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(37, 13);
            this.label96.TabIndex = 201;
            this.label96.Text = "Value:";
            // 
            // txtR1Kvp1Name
            // 
            this.txtR1Kvp1Name.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtR1Kvp1Name.Location = new System.Drawing.Point(64, 18);
            this.txtR1Kvp1Name.Name = "txtR1Kvp1Name";
            this.txtR1Kvp1Name.Size = new System.Drawing.Size(181, 13);
            this.txtR1Kvp1Name.TabIndex = 198;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label92.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label92.Location = new System.Drawing.Point(32, 18);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(28, 13);
            this.label92.TabIndex = 200;
            this.label92.Text = "Key:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(111, 68);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 23);
            this.btnSave.TabIndex = 202;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(173, 68);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 23);
            this.btnCancel.TabIndex = 203;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ViewEditKVP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 107);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtR1Kvp1Value);
            this.Controls.Add(this.label96);
            this.Controls.Add(this.txtR1Kvp1Name);
            this.Controls.Add(this.label92);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewEditKVP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View/Edit KVP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtR1Kvp1Value;
        private System.Windows.Forms.Label label96;
        private System.Windows.Forms.TextBox txtR1Kvp1Name;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}