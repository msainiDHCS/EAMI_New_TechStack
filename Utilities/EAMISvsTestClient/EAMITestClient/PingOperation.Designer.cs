namespace EAMITestClient
{
    partial class PingOperation
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGetCurrentTime = new System.Windows.Forms.Button();
            this.txtReqSenderId = new System.Windows.Forms.TextBox();
            this.txtReqReceiverID = new System.Windows.Forms.TextBox();
            this.txtReqCliTimeStamp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRespMsg = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRespSvsTimeStamp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRespSenderID = new System.Windows.Forms.TextBox();
            this.txtRespReceiverId = new System.Windows.Forms.TextBox();
            this.txtRespCliTimeStamp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnExecutePing = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGetCurrentTime);
            this.groupBox1.Controls.Add(this.txtReqSenderId);
            this.groupBox1.Controls.Add(this.txtReqReceiverID);
            this.groupBox1.Controls.Add(this.txtReqCliTimeStamp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(11, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 110);
            this.groupBox1.TabIndex = 144;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ping Request";
            // 
            // btnGetCurrentTime
            // 
            this.btnGetCurrentTime.CausesValidation = false;
            this.btnGetCurrentTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetCurrentTime.Location = new System.Drawing.Point(221, 60);
            this.btnGetCurrentTime.Margin = new System.Windows.Forms.Padding(1);
            this.btnGetCurrentTime.Name = "btnGetCurrentTime";
            this.btnGetCurrentTime.Size = new System.Drawing.Size(15, 13);
            this.btnGetCurrentTime.TabIndex = 17;
            this.btnGetCurrentTime.TabStop = false;
            this.btnGetCurrentTime.Text = "\"";
            this.toolTip1.SetToolTip(this.btnGetCurrentTime, "Enable/Disable auto timer");
            this.btnGetCurrentTime.UseVisualStyleBackColor = true;
            this.btnGetCurrentTime.Click += new System.EventHandler(this.btnGetCurrentTime_Click);
            // 
            // txtReqSenderId
            // 
            this.txtReqSenderId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReqSenderId.Location = new System.Drawing.Point(81, 26);
            this.txtReqSenderId.Name = "txtReqSenderId";
            this.txtReqSenderId.Size = new System.Drawing.Size(141, 13);
            this.txtReqSenderId.TabIndex = 1;
            // 
            // txtReqReceiverID
            // 
            this.txtReqReceiverID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReqReceiverID.Location = new System.Drawing.Point(81, 43);
            this.txtReqReceiverID.Name = "txtReqReceiverID";
            this.txtReqReceiverID.Size = new System.Drawing.Size(141, 13);
            this.txtReqReceiverID.TabIndex = 2;
            // 
            // txtReqCliTimeStamp
            // 
            this.txtReqCliTimeStamp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReqCliTimeStamp.Location = new System.Drawing.Point(81, 60);
            this.txtReqCliTimeStamp.Name = "txtReqCliTimeStamp";
            this.txtReqCliTimeStamp.Size = new System.Drawing.Size(141, 13);
            this.txtReqCliTimeStamp.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(26, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SenderID*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(17, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "ReceiverID*";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(4, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Cli-TimeStamp*";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtRespMsg);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtRespSvsTimeStamp);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtRespSenderID);
            this.groupBox2.Controls.Add(this.txtRespReceiverId);
            this.groupBox2.Controls.Add(this.txtRespCliTimeStamp);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(265, 34);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(510, 110);
            this.groupBox2.TabIndex = 145;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ping Response";
            // 
            // txtRespMsg
            // 
            this.txtRespMsg.BackColor = System.Drawing.SystemColors.Info;
            this.txtRespMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRespMsg.Location = new System.Drawing.Point(237, 26);
            this.txtRespMsg.Multiline = true;
            this.txtRespMsg.Name = "txtRespMsg";
            this.txtRespMsg.ReadOnly = true;
            this.txtRespMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRespMsg.Size = new System.Drawing.Size(264, 64);
            this.txtRespMsg.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(234, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Msg";
            // 
            // txtRespSvsTimeStamp
            // 
            this.txtRespSvsTimeStamp.BackColor = System.Drawing.SystemColors.Info;
            this.txtRespSvsTimeStamp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRespSvsTimeStamp.Location = new System.Drawing.Point(90, 77);
            this.txtRespSvsTimeStamp.Name = "txtRespSvsTimeStamp";
            this.txtRespSvsTimeStamp.ReadOnly = true;
            this.txtRespSvsTimeStamp.Size = new System.Drawing.Size(141, 13);
            this.txtRespSvsTimeStamp.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(7, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Svs-TimeStamp";
            // 
            // txtRespSenderID
            // 
            this.txtRespSenderID.BackColor = System.Drawing.SystemColors.Info;
            this.txtRespSenderID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRespSenderID.Location = new System.Drawing.Point(90, 26);
            this.txtRespSenderID.Name = "txtRespSenderID";
            this.txtRespSenderID.ReadOnly = true;
            this.txtRespSenderID.Size = new System.Drawing.Size(141, 13);
            this.txtRespSenderID.TabIndex = 1;
            // 
            // txtRespReceiverId
            // 
            this.txtRespReceiverId.BackColor = System.Drawing.SystemColors.Info;
            this.txtRespReceiverId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRespReceiverId.Location = new System.Drawing.Point(90, 43);
            this.txtRespReceiverId.Name = "txtRespReceiverId";
            this.txtRespReceiverId.ReadOnly = true;
            this.txtRespReceiverId.Size = new System.Drawing.Size(141, 13);
            this.txtRespReceiverId.TabIndex = 2;
            // 
            // txtRespCliTimeStamp
            // 
            this.txtRespCliTimeStamp.BackColor = System.Drawing.SystemColors.Info;
            this.txtRespCliTimeStamp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRespCliTimeStamp.Location = new System.Drawing.Point(90, 60);
            this.txtRespCliTimeStamp.Name = "txtRespCliTimeStamp";
            this.txtRespCliTimeStamp.ReadOnly = true;
            this.txtRespCliTimeStamp.Size = new System.Drawing.Size(141, 13);
            this.txtRespCliTimeStamp.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(35, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "SenderID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(26, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "ReceiverID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(13, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Cli-TimeStamp";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(724, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 22);
            this.btnClose.TabIndex = 147;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnExecutePing
            // 
            this.btnExecutePing.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnExecutePing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecutePing.ForeColor = System.Drawing.SystemColors.Info;
            this.btnExecutePing.Location = new System.Drawing.Point(640, 7);
            this.btnExecutePing.Name = "btnExecutePing";
            this.btnExecutePing.Size = new System.Drawing.Size(64, 24);
            this.btnExecutePing.TabIndex = 185;
            this.btnExecutePing.Text = "PING";
            this.toolTip1.SetToolTip(this.btnExecutePing, "Execute PING request");
            this.btnExecutePing.UseVisualStyleBackColor = false;
            this.btnExecutePing.Click += new System.EventHandler(this.btnExecutePing_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PingOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 155);
            this.Controls.Add(this.btnExecutePing);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PingOperation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ping Operation";
            this.Load += new System.EventHandler(this.PingOperation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGetCurrentTime;
        private System.Windows.Forms.TextBox txtReqSenderId;
        private System.Windows.Forms.TextBox txtReqReceiverID;
        private System.Windows.Forms.TextBox txtReqCliTimeStamp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtRespMsg;
        private System.Windows.Forms.TextBox txtRespSvsTimeStamp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRespSenderID;
        private System.Windows.Forms.TextBox txtRespReceiverId;
        private System.Windows.Forms.TextBox txtRespCliTimeStamp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnExecutePing;
        private System.Windows.Forms.Timer timer1;
    }
}