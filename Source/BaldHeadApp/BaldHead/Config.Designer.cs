namespace BaldHead
{
	partial class Config
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblUrl = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.grpSendItemTest = new System.Windows.Forms.GroupBox();
            this.lblBitmapFilename = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSendItem = new System.Windows.Forms.Button();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkFlipImageHorizontally = new System.Windows.Forms.CheckBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioFullScaled = new System.Windows.Forms.RadioButton();
            this.pnlWebCam = new System.Windows.Forms.Panel();
            this.txtCameraStats = new System.Windows.Forms.TextBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbCameras = new System.Windows.Forms.ComboBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.grpSendItemTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Service host URL:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUrl);
            this.groupBox1.Controls.Add(this.lblTime);
            this.groupBox1.Controls.Add(this.btnTestConnection);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(659, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Host";
            // 
            // lblUrl
            // 
            this.lblUrl.BackColor = System.Drawing.Color.GhostWhite;
            this.lblUrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUrl.Enabled = false;
            this.lblUrl.Location = new System.Drawing.Point(126, 25);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(517, 23);
            this.lblUrl.TabIndex = 4;
            this.lblUrl.Text = "(Cannot find it in BaldHead.exe.config)";
            this.lblUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.GhostWhite;
            this.lblTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTime.Location = new System.Drawing.Point(127, 56);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(231, 23);
            this.lblTime.TabIndex = 3;
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(6, 56);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(114, 23);
            this.btnTestConnection.TabIndex = 2;
            this.btnTestConnection.Text = "Test connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // grpSendItemTest
            // 
            this.grpSendItemTest.Controls.Add(this.lblBitmapFilename);
            this.grpSendItemTest.Controls.Add(this.btnBrowse);
            this.grpSendItemTest.Controls.Add(this.btnSendItem);
            this.grpSendItemTest.Controls.Add(this.txtScore);
            this.grpSendItemTest.Controls.Add(this.txtName);
            this.grpSendItemTest.Controls.Add(this.picImage);
            this.grpSendItemTest.Controls.Add(this.label4);
            this.grpSendItemTest.Controls.Add(this.label3);
            this.grpSendItemTest.Controls.Add(this.label2);
            this.grpSendItemTest.Location = new System.Drawing.Point(13, 120);
            this.grpSendItemTest.Name = "grpSendItemTest";
            this.grpSendItemTest.Size = new System.Drawing.Size(659, 120);
            this.grpSendItemTest.TabIndex = 3;
            this.grpSendItemTest.TabStop = false;
            this.grpSendItemTest.Text = "Send Item Test";
            // 
            // lblBitmapFilename
            // 
            this.lblBitmapFilename.BackColor = System.Drawing.Color.GhostWhite;
            this.lblBitmapFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBitmapFilename.Location = new System.Drawing.Point(482, 49);
            this.lblBitmapFilename.Name = "lblBitmapFilename";
            this.lblBitmapFilename.Size = new System.Drawing.Size(136, 23);
            this.lblBitmapFilename.TabIndex = 5;
            this.lblBitmapFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(624, 46);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(29, 23);
            this.btnBrowse.TabIndex = 12;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnSendItem
            // 
            this.btnSendItem.Location = new System.Drawing.Point(258, 82);
            this.btnSendItem.Name = "btnSendItem";
            this.btnSendItem.Size = new System.Drawing.Size(142, 23);
            this.btnSendItem.TabIndex = 5;
            this.btnSendItem.Text = "Send item to server";
            this.btnSendItem.UseVisualStyleBackColor = true;
            this.btnSendItem.Click += new System.EventHandler(this.btnSendItem_Click);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(126, 46);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(231, 20);
            this.txtScore.TabIndex = 11;
            this.txtScore.Text = "-1";
            this.txtScore.TextChanged += new System.EventHandler(this.txtScore_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(126, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(231, 20);
            this.txtName.TabIndex = 10;
            this.txtName.Text = "Larry\'s your uncle!";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.Color.White;
            this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.errorProvider.SetIconAlignment(this.picImage, System.Windows.Forms.ErrorIconAlignment.TopRight);
            this.picImage.Location = new System.Drawing.Point(425, 20);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(52, 52);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picImage.TabIndex = 9;
            this.picImage.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(514, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "Bitmap image:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Score:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(305, 450);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.pnlWebCam);
            this.groupBox2.Controls.Add(this.txtCameraStats);
            this.groupBox2.Controls.Add(this.btnConfig);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cmbCameras);
            this.groupBox2.Location = new System.Drawing.Point(13, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(659, 183);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkFlipImageHorizontally);
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Controls.Add(this.radioFullScaled);
            this.groupBox3.Location = new System.Drawing.Point(309, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(142, 155);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Render image type";
            // 
            // chkFlipImageHorizontally
            // 
            this.chkFlipImageHorizontally.AutoSize = true;
            this.chkFlipImageHorizontally.Location = new System.Drawing.Point(7, 132);
            this.chkFlipImageHorizontally.Name = "chkFlipImageHorizontally";
            this.chkFlipImageHorizontally.Size = new System.Drawing.Size(131, 17);
            this.chkFlipImageHorizontally.TabIndex = 3;
            this.chkFlipImageHorizontally.Text = "Flip Image Horizontally";
            this.chkFlipImageHorizontally.UseVisualStyleBackColor = true;
            this.chkFlipImageHorizontally.CheckedChanged += new System.EventHandler(this.chkFlipImageHorizontally_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(7, 67);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(127, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.Text = "Middle 33%, centered";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(7, 44);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(84, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.Text = "Fit, centered";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioFullScaled
            // 
            this.radioFullScaled.AutoSize = true;
            this.radioFullScaled.Checked = true;
            this.radioFullScaled.Location = new System.Drawing.Point(7, 20);
            this.radioFullScaled.Name = "radioFullScaled";
            this.radioFullScaled.Size = new System.Drawing.Size(78, 17);
            this.radioFullScaled.TabIndex = 0;
            this.radioFullScaled.TabStop = true;
            this.radioFullScaled.Text = "Full, scaled";
            this.radioFullScaled.UseVisualStyleBackColor = true;
            this.radioFullScaled.CheckedChanged += new System.EventHandler(this.radioFullScaled_CheckedChanged);
            // 
            // pnlWebCam
            // 
            this.pnlWebCam.BackColor = System.Drawing.Color.Gray;
            this.pnlWebCam.Location = new System.Drawing.Point(457, 19);
            this.pnlWebCam.Name = "pnlWebCam";
            this.pnlWebCam.Size = new System.Drawing.Size(196, 126);
            this.pnlWebCam.TabIndex = 19;
            // 
            // txtCameraStats
            // 
            this.txtCameraStats.Location = new System.Drawing.Point(6, 60);
            this.txtCameraStats.Multiline = true;
            this.txtCameraStats.Name = "txtCameraStats";
            this.txtCameraStats.Size = new System.Drawing.Size(247, 99);
            this.txtCameraStats.TabIndex = 18;
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.Gainsboro;
            this.btnConfig.Location = new System.Drawing.Point(599, 151);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(52, 23);
            this.btnConfig.TabIndex = 17;
            this.btnConfig.Text = "Config";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Gainsboro;
            this.btnStop.Location = new System.Drawing.Point(528, 151);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(52, 23);
            this.btnStop.TabIndex = 16;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Gainsboro;
            this.btnStart.Location = new System.Drawing.Point(457, 151);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(52, 23);
            this.btnStart.TabIndex = 15;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Active camera";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmbCameras
            // 
            this.cmbCameras.FormattingEnabled = true;
            this.cmbCameras.Location = new System.Drawing.Point(6, 32);
            this.cmbCameras.Name = "cmbCameras";
            this.cmbCameras.Size = new System.Drawing.Size(247, 21);
            this.cmbCameras.TabIndex = 0;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Enabled = false;
            this.btnSaveSettings.Location = new System.Drawing.Point(573, 450);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(99, 23);
            this.btnSaveSettings.TabIndex = 6;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Config
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(684, 476);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grpSendItemTest);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Config";
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.Config_Load);
            this.groupBox1.ResumeLayout(false);
            this.grpSendItemTest.ResumeLayout(false);
            this.grpSendItemTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Button btnTestConnection;
		private System.Windows.Forms.Label lblUrl;
		private System.Windows.Forms.GroupBox grpSendItemTest;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox picImage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox txtScore;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Button btnSendItem;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Label lblBitmapFilename;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cmbCameras;
		private System.Windows.Forms.Button btnSaveSettings;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnConfig;
		private System.Windows.Forms.TextBox txtCameraStats;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Panel pnlWebCam;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioFullScaled;
		private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.CheckBox chkFlipImageHorizontally;
	}
}