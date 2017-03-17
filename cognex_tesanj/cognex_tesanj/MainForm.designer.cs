namespace cognex_tesanj
{
    partial class MainForm
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
            this.picResultImage = new System.Windows.Forms.PictureBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnTrigger = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDeviceIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.listBoxDetectedSystems = new System.Windows.Forms.ListBox();
            this.btnRefreshDeviceList = new System.Windows.Forms.Button();
            this.cbEnableKeepAlive = new System.Windows.Forms.CheckBox();
            this.cbLiveDisplay = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.Panel();
            this.cbAutoReconnect = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.Panel();
            this.lbReadString = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbLoggingEnabled = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picResultImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picResultImage
            // 
            this.picResultImage.Location = new System.Drawing.Point(15, 55);
            this.picResultImage.Name = "picResultImage";
            this.picResultImage.Size = new System.Drawing.Size(368, 350);
            this.picResultImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picResultImage.TabIndex = 0;
            this.picResultImage.TabStop = false;
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(16, 129);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(217, 30);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(16, 165);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(217, 30);
            this.btnDisconnect.TabIndex = 2;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.Location = new System.Drawing.Point(15, 23);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(448, 186);
            this.listBox1.TabIndex = 3;
            // 
            // btnTrigger
            // 
            this.btnTrigger.Enabled = false;
            this.btnTrigger.Location = new System.Drawing.Point(15, 19);
            this.btnTrigger.Name = "btnTrigger";
            this.btnTrigger.Size = new System.Drawing.Size(217, 30);
            this.btnTrigger.TabIndex = 5;
            this.btnTrigger.Text = "Trigger";
            this.btnTrigger.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTrigger_MouseDown);
            this.btnTrigger.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnTrigger_MouseUp);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Device:";
            // 
            // txtDeviceIP
            // 
            this.txtDeviceIP.Location = new System.Drawing.Point(76, 24);
            this.txtDeviceIP.Name = "txtDeviceIP";
            this.txtDeviceIP.ReadOnly = true;
            this.txtDeviceIP.Size = new System.Drawing.Size(157, 20);
            this.txtDeviceIP.TabIndex = 7;
            this.txtDeviceIP.TextChanged += new System.EventHandler(this.txtDeviceIP_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(13, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(76, 51);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(157, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // listBoxDetectedSystems
            // 
            this.listBoxDetectedSystems.Location = new System.Drawing.Point(240, 24);
            this.listBoxDetectedSystems.Name = "listBoxDetectedSystems";
            this.listBoxDetectedSystems.Size = new System.Drawing.Size(222, 134);
            this.listBoxDetectedSystems.TabIndex = 10;
            this.listBoxDetectedSystems.SelectedIndexChanged += new System.EventHandler(this.listBoxDetectedSystems_SelectedIndexChanged);
            // 
            // btnRefreshDeviceList
            // 
            this.btnRefreshDeviceList.Location = new System.Drawing.Point(240, 165);
            this.btnRefreshDeviceList.Name = "btnRefreshDeviceList";
            this.btnRefreshDeviceList.Size = new System.Drawing.Size(224, 30);
            this.btnRefreshDeviceList.TabIndex = 11;
            this.btnRefreshDeviceList.Text = "Refresh";
            this.btnRefreshDeviceList.Click += new System.EventHandler(this.btnRefreshSystemList_Click);
            // 
            // cbEnableKeepAlive
            // 
            this.cbEnableKeepAlive.Location = new System.Drawing.Point(76, 78);
            this.cbEnableKeepAlive.Name = "cbEnableKeepAlive";
            this.cbEnableKeepAlive.Size = new System.Drawing.Size(123, 17);
            this.cbEnableKeepAlive.TabIndex = 12;
            this.cbEnableKeepAlive.Text = "Run Keep Alive Thread";
            // 
            // cbLiveDisplay
            // 
            this.cbLiveDisplay.Location = new System.Drawing.Point(238, 27);
            this.cbLiveDisplay.Name = "cbLiveDisplay";
            this.cbLiveDisplay.Size = new System.Drawing.Size(83, 17);
            this.cbLiveDisplay.TabIndex = 15;
            this.cbLiveDisplay.Text = "Live Display";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAutoReconnect);
            this.groupBox1.Controls.Add(this.btnRefreshDeviceList);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnDisconnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbEnableKeepAlive);
            this.groupBox1.Controls.Add(this.txtDeviceIP);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.listBoxDetectedSystems);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 204);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.Text = "Connect";
            // 
            // cbAutoReconnect
            // 
            this.cbAutoReconnect.Location = new System.Drawing.Point(76, 102);
            this.cbAutoReconnect.Name = "cbAutoReconnect";
            this.cbAutoReconnect.Size = new System.Drawing.Size(99, 17);
            this.cbAutoReconnect.TabIndex = 13;
            this.cbAutoReconnect.Text = "Auto-reconnect";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 222);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 228);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.Text = "Events";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbReadString);
            this.groupBox3.Controls.Add(this.btnTrigger);
            this.groupBox3.Controls.Add(this.picResultImage);
            this.groupBox3.Controls.Add(this.cbLiveDisplay);
            this.groupBox3.Location = new System.Drawing.Point(497, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(398, 437);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.Text = "Results";
            // 
            // lbReadString
            // 
            this.lbReadString.Location = new System.Drawing.Point(15, 396);
            this.lbReadString.Name = "lbReadString";
            this.lbReadString.Size = new System.Drawing.Size(368, 23);
            this.lbReadString.TabIndex = 16;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel2);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(15, 464);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(879, 314);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.Text = "Dmcc event log";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbLog);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(879, 278);
            this.panel2.TabIndex = 2;
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(0, 0);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(879, 278);
            this.tbLog.TabIndex = 1;
            this.tbLog.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbLoggingEnabled);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(879, 36);
            this.panel1.TabIndex = 1;
            // 
            // cbLoggingEnabled
            // 
            this.cbLoggingEnabled.Location = new System.Drawing.Point(18, 10);
            this.cbLoggingEnabled.Name = "cbLoggingEnabled";
            this.cbLoggingEnabled.Size = new System.Drawing.Size(105, 17);
            this.cbLoggingEnabled.TabIndex = 0;
            this.cbLoggingEnabled.Text = "Logging enabled";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(904, 789);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "DataMan SDK Sample Application";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picResultImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picResultImage;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnTrigger;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDeviceIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.ListBox listBoxDetectedSystems;
        private System.Windows.Forms.Button btnRefreshDeviceList;
        private System.Windows.Forms.CheckBox cbEnableKeepAlive;
        private System.Windows.Forms.CheckBox cbLiveDisplay;
        private System.Windows.Forms.Panel groupBox1;
        private System.Windows.Forms.Panel groupBox2;
        private System.Windows.Forms.Panel groupBox3;
        private System.Windows.Forms.Label lbReadString;
        private System.Windows.Forms.CheckBox cbAutoReconnect;
        private System.Windows.Forms.Panel groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbLoggingEnabled;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbLog;
    }
}

