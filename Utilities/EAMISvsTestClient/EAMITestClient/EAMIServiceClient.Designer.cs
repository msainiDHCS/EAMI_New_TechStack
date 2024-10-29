namespace EAMITestClient
{
    partial class EAMIServiceClient
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
            this.btnPing = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserIdentity = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBasicAuthIdentity = new System.Windows.Forms.TextBox();
            this.rbtnDevDntl = new System.Windows.Forms.RadioButton();
            this.rbtnStagRX = new System.Windows.Forms.RadioButton();
            this.rbtnDevRX = new System.Windows.Forms.RadioButton();
            this.rbtnQADntl = new System.Windows.Forms.RadioButton();
            this.rbtnStagDntl = new System.Windows.Forms.RadioButton();
            this.rbtnQARX = new System.Windows.Forms.RadioButton();
            this.lnkServiceURI = new System.Windows.Forms.LinkLabel();
            this.btnPaymentSubmissionNew = new System.Windows.Forms.Button();
            this.btnNewStatusInquiry = new System.Windows.Forms.Button();
            this.btnNewRejectedPaymentInquiry = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPing
            // 
            this.btnPing.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPing.Location = new System.Drawing.Point(83, 279);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(345, 34);
            this.btnPing.TabIndex = 400;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(354, 337);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(15, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "User:";
            // 
            // txtUserIdentity
            // 
            this.txtUserIdentity.BackColor = System.Drawing.SystemColors.Control;
            this.txtUserIdentity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserIdentity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtUserIdentity.Location = new System.Drawing.Point(48, 15);
            this.txtUserIdentity.Name = "txtUserIdentity";
            this.txtUserIdentity.ReadOnly = true;
            this.txtUserIdentity.Size = new System.Drawing.Size(107, 13);
            this.txtUserIdentity.TabIndex = 8;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label63.Location = new System.Drawing.Point(7, 35);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(42, 13);
            this.label63.TabIndex = 191;
            this.label63.Text = "SvcUri:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBasicAuthIdentity);
            this.groupBox1.Controls.Add(this.rbtnDevDntl);
            this.groupBox1.Controls.Add(this.rbtnStagRX);
            this.groupBox1.Controls.Add(this.rbtnDevRX);
            this.groupBox1.Controls.Add(this.rbtnQADntl);
            this.groupBox1.Controls.Add(this.rbtnStagDntl);
            this.groupBox1.Controls.Add(this.rbtnQARX);
            this.groupBox1.Controls.Add(this.lnkServiceURI);
            this.groupBox1.Controls.Add(this.txtUserIdentity);
            this.groupBox1.Controls.Add(this.label63);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(-4, -4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(515, 110);
            this.groupBox1.TabIndex = 192;
            this.groupBox1.TabStop = false;
            // 
            // txtBasicAuthIdentity
            // 
            this.txtBasicAuthIdentity.BackColor = System.Drawing.SystemColors.Control;
            this.txtBasicAuthIdentity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBasicAuthIdentity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtBasicAuthIdentity.Location = new System.Drawing.Point(296, 15);
            this.txtBasicAuthIdentity.Name = "txtBasicAuthIdentity";
            this.txtBasicAuthIdentity.ReadOnly = true;
            this.txtBasicAuthIdentity.Size = new System.Drawing.Size(209, 13);
            this.txtBasicAuthIdentity.TabIndex = 199;
            // 
            // rbtnDevDntl
            // 
            this.rbtnDevDntl.AutoSize = true;
            this.rbtnDevDntl.Location = new System.Drawing.Point(108, 85);
            this.rbtnDevDntl.Name = "rbtnDevDntl";
            this.rbtnDevDntl.Size = new System.Drawing.Size(67, 17);
            this.rbtnDevDntl.TabIndex = 198;
            this.rbtnDevDntl.TabStop = true;
            this.rbtnDevDntl.Text = "Dev-Dntl";
            this.rbtnDevDntl.UseVisualStyleBackColor = true;
            this.rbtnDevDntl.CheckedChanged += new System.EventHandler(this.rbtnDevDntl_CheckedChanged);
            // 
            // rbtnStagRX
            // 
            this.rbtnStagRX.AutoSize = true;
            this.rbtnStagRX.Location = new System.Drawing.Point(353, 60);
            this.rbtnStagRX.Name = "rbtnStagRX";
            this.rbtnStagRX.Size = new System.Drawing.Size(65, 17);
            this.rbtnStagRX.TabIndex = 197;
            this.rbtnStagRX.TabStop = true;
            this.rbtnStagRX.Text = "Stag-RX";
            this.rbtnStagRX.UseVisualStyleBackColor = true;
            this.rbtnStagRX.CheckedChanged += new System.EventHandler(this.rbtnStagRX_CheckedChanged);
            // 
            // rbtnDevRX
            // 
            this.rbtnDevRX.AutoSize = true;
            this.rbtnDevRX.Location = new System.Drawing.Point(108, 60);
            this.rbtnDevRX.Name = "rbtnDevRX";
            this.rbtnDevRX.Size = new System.Drawing.Size(63, 17);
            this.rbtnDevRX.TabIndex = 196;
            this.rbtnDevRX.TabStop = true;
            this.rbtnDevRX.Text = "Dev-RX";
            this.rbtnDevRX.UseVisualStyleBackColor = true;
            this.rbtnDevRX.CheckedChanged += new System.EventHandler(this.rbtnDevRX_CheckedChanged);
            // 
            // rbtnQADntl
            // 
            this.rbtnQADntl.AutoSize = true;
            this.rbtnQADntl.Location = new System.Drawing.Point(224, 85);
            this.rbtnQADntl.Name = "rbtnQADntl";
            this.rbtnQADntl.Size = new System.Drawing.Size(62, 17);
            this.rbtnQADntl.TabIndex = 195;
            this.rbtnQADntl.TabStop = true;
            this.rbtnQADntl.Text = "QA-Dntl";
            this.rbtnQADntl.UseVisualStyleBackColor = true;
            this.rbtnQADntl.CheckedChanged += new System.EventHandler(this.rbtnQADntl_CheckedChanged);
            // 
            // rbtnStagDntl
            // 
            this.rbtnStagDntl.AutoSize = true;
            this.rbtnStagDntl.Location = new System.Drawing.Point(353, 85);
            this.rbtnStagDntl.Name = "rbtnStagDntl";
            this.rbtnStagDntl.Size = new System.Drawing.Size(69, 17);
            this.rbtnStagDntl.TabIndex = 194;
            this.rbtnStagDntl.TabStop = true;
            this.rbtnStagDntl.Text = "Stag-Dntl";
            this.rbtnStagDntl.UseVisualStyleBackColor = true;
            this.rbtnStagDntl.CheckedChanged += new System.EventHandler(this.rbtnStagDntl_CheckedChanged);
            // 
            // rbtnQARX
            // 
            this.rbtnQARX.AutoSize = true;
            this.rbtnQARX.Location = new System.Drawing.Point(224, 60);
            this.rbtnQARX.Name = "rbtnQARX";
            this.rbtnQARX.Size = new System.Drawing.Size(58, 17);
            this.rbtnQARX.TabIndex = 193;
            this.rbtnQARX.TabStop = true;
            this.rbtnQARX.Text = "QA-RX";
            this.rbtnQARX.UseVisualStyleBackColor = true;
            this.rbtnQARX.CheckedChanged += new System.EventHandler(this.rbtnQARX_CheckedChanged);
            // 
            // lnkServiceURI
            // 
            this.lnkServiceURI.AutoSize = true;
            this.lnkServiceURI.Location = new System.Drawing.Point(46, 34);
            this.lnkServiceURI.Name = "lnkServiceURI";
            this.lnkServiceURI.Size = new System.Drawing.Size(67, 13);
            this.lnkServiceURI.TabIndex = 192;
            this.lnkServiceURI.TabStop = true;
            this.lnkServiceURI.Text = "- service uri -";
            this.lnkServiceURI.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkServiceURI_LinkClicked);
            // 
            // btnPaymentSubmissionNew
            // 
            this.btnPaymentSubmissionNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaymentSubmissionNew.Location = new System.Drawing.Point(83, 153);
            this.btnPaymentSubmissionNew.Name = "btnPaymentSubmissionNew";
            this.btnPaymentSubmissionNew.Size = new System.Drawing.Size(345, 34);
            this.btnPaymentSubmissionNew.TabIndex = 193;
            this.btnPaymentSubmissionNew.Text = "Payment Submission";
            this.btnPaymentSubmissionNew.UseVisualStyleBackColor = true;
            this.btnPaymentSubmissionNew.Click += new System.EventHandler(this.btnPaymentSubmissionNew_Click);
            // 
            // btnNewStatusInquiry
            // 
            this.btnNewStatusInquiry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStatusInquiry.Location = new System.Drawing.Point(83, 195);
            this.btnNewStatusInquiry.Name = "btnNewStatusInquiry";
            this.btnNewStatusInquiry.Size = new System.Drawing.Size(345, 34);
            this.btnNewStatusInquiry.TabIndex = 194;
            this.btnNewStatusInquiry.Text = "Payment Status Inquiry";
            this.btnNewStatusInquiry.UseVisualStyleBackColor = true;
            this.btnNewStatusInquiry.Click += new System.EventHandler(this.btnNewStatusInquiry_Click);
            // 
            // btnNewRejectedPaymentInquiry
            // 
            this.btnNewRejectedPaymentInquiry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewRejectedPaymentInquiry.Location = new System.Drawing.Point(83, 237);
            this.btnNewRejectedPaymentInquiry.Name = "btnNewRejectedPaymentInquiry";
            this.btnNewRejectedPaymentInquiry.Size = new System.Drawing.Size(345, 34);
            this.btnNewRejectedPaymentInquiry.TabIndex = 195;
            this.btnNewRejectedPaymentInquiry.Text = "Rejected Payment Inquiry";
            this.btnNewRejectedPaymentInquiry.UseVisualStyleBackColor = true;
            this.btnNewRejectedPaymentInquiry.Click += new System.EventHandler(this.btnNewRejectedPaymentInquiry_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(188, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 18);
            this.label1.TabIndex = 196;
            this.label1.Text = "Service Operations";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(83, 383);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 34);
            this.btnTest.TabIndex = 401;
            this.btnTest.Text = "TestEFT";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnTest2
            // 
            this.btnTest2.Location = new System.Drawing.Point(253, 383);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(89, 34);
            this.btnTest2.TabIndex = 402;
            this.btnTest2.Text = "TestADQuery";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // EAMIServiceClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 382);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPing);
            this.Controls.Add(this.btnNewRejectedPaymentInquiry);
            this.Controls.Add(this.btnNewStatusInquiry);
            this.Controls.Add(this.btnPaymentSubmissionNew);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EAMIServiceClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EAMI Client";
            this.Load += new System.EventHandler(this.EAMIServiceClient_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserIdentity;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkServiceURI;
        private System.Windows.Forms.Button btnPaymentSubmissionNew;
        private System.Windows.Forms.Button btnNewStatusInquiry;
        private System.Windows.Forms.Button btnNewRejectedPaymentInquiry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtnDevRX;
        private System.Windows.Forms.RadioButton rbtnQADntl;
        private System.Windows.Forms.RadioButton rbtnStagDntl;
        private System.Windows.Forms.RadioButton rbtnQARX;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.RadioButton rbtnDevDntl;
        private System.Windows.Forms.RadioButton rbtnStagRX;
        private System.Windows.Forms.TextBox txtBasicAuthIdentity;
    }
}