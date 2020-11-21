namespace BaldHead
{
    partial class TakePicture
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
            this.btnHead = new System.Windows.Forms.Button();
            this.btnStartWebCam = new System.Windows.Forms.Button();
            this.hScroll_ImageAdjust = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.vScroll_ImageAdjust = new System.Windows.Forms.HScrollBar();
            this.pnlHeadOutline = new System.Windows.Forms.Panel();
            this.grpImageAdjustment = new System.Windows.Forms.GroupBox();
            this.chkCrosshairs = new System.Windows.Forms.CheckBox();
            this.chkInvertFaceColors = new System.Windows.Forms.CheckBox();
            this.chkSolidBrush = new System.Windows.Forms.CheckBox();
            this.vScroll_HaircutTool = new System.Windows.Forms.HScrollBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.hScroll_HeadWidth = new System.Windows.Forms.HScrollBar();
            this.label5 = new System.Windows.Forms.Label();
            this.vScroll_HeadHeight = new System.Windows.Forms.HScrollBar();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlNeck = new System.Windows.Forms.Panel();
            this.pnlRightEar = new System.Windows.Forms.Panel();
            this.pnlLeftEar = new System.Windows.Forms.Panel();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSubmit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlBitmapImage = new System.Windows.Forms.Panel();
            this.grpImageAdjustment.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnHead
            // 
            this.btnHead.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHead.Location = new System.Drawing.Point(495, 138);
            this.btnHead.Name = "btnHead";
            this.btnHead.Size = new System.Drawing.Size(284, 340);
            this.btnHead.TabIndex = 0;
            this.btnHead.Visible = false;
            // 
            // btnStartWebCam
            // 
            this.btnStartWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartWebCam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartWebCam.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStartWebCam.Location = new System.Drawing.Point(553, 621);
            this.btnStartWebCam.Name = "btnStartWebCam";
            this.btnStartWebCam.Size = new System.Drawing.Size(173, 49);
            this.btnStartWebCam.TabIndex = 1;
            this.btnStartWebCam.Text = "Start WebCam";
            this.btnStartWebCam.UseVisualStyleBackColor = true;
            this.btnStartWebCam.Click += new System.EventHandler(this.btnStartWebCam_Click);
            // 
            // hScroll_ImageAdjust
            // 
            this.hScroll_ImageAdjust.LargeChange = 1;
            this.hScroll_ImageAdjust.Location = new System.Drawing.Point(17, 291);
            this.hScroll_ImageAdjust.Maximum = 0;
            this.hScroll_ImageAdjust.Name = "hScroll_ImageAdjust";
            this.hScroll_ImageAdjust.Size = new System.Drawing.Size(199, 17);
            this.hScroll_ImageAdjust.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 268);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Horizontal Image Adjust";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // vScroll_ImageAdjust
            // 
            this.vScroll_ImageAdjust.LargeChange = 1;
            this.vScroll_ImageAdjust.Location = new System.Drawing.Point(17, 244);
            this.vScroll_ImageAdjust.Maximum = 0;
            this.vScroll_ImageAdjust.Name = "vScroll_ImageAdjust";
            this.vScroll_ImageAdjust.Size = new System.Drawing.Size(199, 17);
            this.vScroll_ImageAdjust.TabIndex = 5;
            // 
            // pnlHeadOutline
            // 
            this.pnlHeadOutline.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlHeadOutline.Location = new System.Drawing.Point(458, 99);
            this.pnlHeadOutline.Name = "pnlHeadOutline";
            this.pnlHeadOutline.Size = new System.Drawing.Size(360, 426);
            this.pnlHeadOutline.TabIndex = 7;
            this.pnlHeadOutline.Visible = false;
            // 
            // grpImageAdjustment
            // 
            this.grpImageAdjustment.Controls.Add(this.chkCrosshairs);
            this.grpImageAdjustment.Controls.Add(this.chkInvertFaceColors);
            this.grpImageAdjustment.Controls.Add(this.chkSolidBrush);
            this.grpImageAdjustment.Controls.Add(this.vScroll_HaircutTool);
            this.grpImageAdjustment.Controls.Add(this.label6);
            this.grpImageAdjustment.Controls.Add(this.label4);
            this.grpImageAdjustment.Controls.Add(this.hScroll_HeadWidth);
            this.grpImageAdjustment.Controls.Add(this.label5);
            this.grpImageAdjustment.Controls.Add(this.vScroll_HeadHeight);
            this.grpImageAdjustment.Controls.Add(this.label3);
            this.grpImageAdjustment.Controls.Add(this.hScroll_ImageAdjust);
            this.grpImageAdjustment.Controls.Add(this.label1);
            this.grpImageAdjustment.Controls.Add(this.vScroll_ImageAdjust);
            this.grpImageAdjustment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpImageAdjustment.Location = new System.Drawing.Point(74, 154);
            this.grpImageAdjustment.Name = "grpImageAdjustment";
            this.grpImageAdjustment.Size = new System.Drawing.Size(238, 470);
            this.grpImageAdjustment.TabIndex = 9;
            this.grpImageAdjustment.TabStop = false;
            this.grpImageAdjustment.Text = "Image Adjustment";
            // 
            // chkCrosshairs
            // 
            this.chkCrosshairs.AutoSize = true;
            this.chkCrosshairs.Location = new System.Drawing.Point(17, 398);
            this.chkCrosshairs.Name = "chkCrosshairs";
            this.chkCrosshairs.Size = new System.Drawing.Size(168, 20);
            this.chkCrosshairs.TabIndex = 16;
            this.chkCrosshairs.Text = "WebCam Crosshairs";
            this.chkCrosshairs.UseVisualStyleBackColor = true;
            // 
            // chkInvertFaceColors
            // 
            this.chkInvertFaceColors.AutoSize = true;
            this.chkInvertFaceColors.Location = new System.Drawing.Point(17, 372);
            this.chkInvertFaceColors.Name = "chkInvertFaceColors";
            this.chkInvertFaceColors.Size = new System.Drawing.Size(153, 20);
            this.chkInvertFaceColors.TabIndex = 15;
            this.chkInvertFaceColors.Text = "Invert Face Colors";
            this.chkInvertFaceColors.UseVisualStyleBackColor = true;
            // 
            // chkSolidBrush
            // 
            this.chkSolidBrush.AutoSize = true;
            this.chkSolidBrush.Location = new System.Drawing.Point(17, 346);
            this.chkSolidBrush.Name = "chkSolidBrush";
            this.chkSolidBrush.Size = new System.Drawing.Size(173, 20);
            this.chkSolidBrush.TabIndex = 14;
            this.chkSolidBrush.Text = "Single Color Shading";
            this.chkSolidBrush.UseVisualStyleBackColor = true;
            // 
            // vScroll_HaircutTool
            // 
            this.vScroll_HaircutTool.LargeChange = 1;
            this.vScroll_HaircutTool.Location = new System.Drawing.Point(17, 56);
            this.vScroll_HaircutTool.Maximum = 0;
            this.vScroll_HaircutTool.Name = "vScroll_HaircutTool";
            this.vScroll_HaircutTool.Size = new System.Drawing.Size(199, 17);
            this.vScroll_HaircutTool.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(226, 19);
            this.label6.TabIndex = 13;
            this.label6.Text = "Haircut Tool";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(226, 19);
            this.label4.TabIndex = 11;
            this.label4.Text = "Head Height";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // hScroll_HeadWidth
            // 
            this.hScroll_HeadWidth.LargeChange = 1;
            this.hScroll_HeadWidth.Location = new System.Drawing.Point(17, 172);
            this.hScroll_HeadWidth.Maximum = 0;
            this.hScroll_HeadWidth.Name = "hScroll_HeadWidth";
            this.hScroll_HeadWidth.Size = new System.Drawing.Size(199, 17);
            this.hScroll_HeadWidth.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(226, 19);
            this.label5.TabIndex = 9;
            this.label5.Text = "Head Width";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // vScroll_HeadHeight
            // 
            this.vScroll_HeadHeight.LargeChange = 1;
            this.vScroll_HeadHeight.Location = new System.Drawing.Point(17, 125);
            this.vScroll_HeadHeight.Maximum = 0;
            this.vScroll_HeadHeight.Name = "vScroll_HeadHeight";
            this.vScroll_HeadHeight.Size = new System.Drawing.Size(199, 17);
            this.vScroll_HeadHeight.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 221);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 19);
            this.label3.TabIndex = 7;
            this.label3.Text = "Vertical Image Adjust";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlNeck
            // 
            this.pnlNeck.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNeck.Location = new System.Drawing.Point(551, 500);
            this.pnlNeck.Name = "pnlNeck";
            this.pnlNeck.Size = new System.Drawing.Size(175, 66);
            this.pnlNeck.TabIndex = 10;
            this.pnlNeck.Visible = false;
            // 
            // pnlRightEar
            // 
            this.pnlRightEar.BackColor = System.Drawing.Color.LightGray;
            this.pnlRightEar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRightEar.Location = new System.Drawing.Point(808, 233);
            this.pnlRightEar.Name = "pnlRightEar";
            this.pnlRightEar.Size = new System.Drawing.Size(32, 123);
            this.pnlRightEar.TabIndex = 11;
            this.pnlRightEar.Visible = false;
            // 
            // pnlLeftEar
            // 
            this.pnlLeftEar.BackColor = System.Drawing.Color.LightGray;
            this.pnlLeftEar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeftEar.Location = new System.Drawing.Point(436, 233);
            this.pnlLeftEar.Name = "pnlLeftEar";
            this.pnlLeftEar.Size = new System.Drawing.Size(32, 123);
            this.pnlLeftEar.TabIndex = 12;
            this.pnlLeftEar.Visible = false;
            // 
            // txtScore
            // 
            this.txtScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScore.Location = new System.Drawing.Point(9, 112);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(248, 26);
            this.txtScore.TabIndex = 13;
            this.toolTip1.SetToolTip(this.txtScore, "Bet you\'d love the change this - wouldn\'t you!");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "Score:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(127, 16);
            this.label7.TabIndex = 16;
            this.label7.Text = "Employee Name:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(9, 57);
            this.txtName.MaxLength = 8;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(248, 26);
            this.txtName.TabIndex = 15;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Enabled = false;
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSubmit.Location = new System.Drawing.Point(27, 160);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(203, 41);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.toolTip1.SetToolTip(this.btnSubmit, "Make it count!  You only get one shot at this!");
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtScore);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(929, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 218);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enter your name";
            // 
            // pnlBitmapImage
            // 
            this.pnlBitmapImage.Location = new System.Drawing.Point(401, 75);
            this.pnlBitmapImage.Name = "pnlBitmapImage";
            this.pnlBitmapImage.Size = new System.Drawing.Size(470, 512);
            this.pnlBitmapImage.TabIndex = 18;
            this.pnlBitmapImage.Visible = false;
            // 
            // TakePicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlRightEar);
            this.Controls.Add(this.pnlLeftEar);
            this.Controls.Add(this.pnlNeck);
            this.Controls.Add(this.btnStartWebCam);
            this.Controls.Add(this.btnHead);
            this.Controls.Add(this.grpImageAdjustment);
            this.Controls.Add(this.pnlHeadOutline);
            this.Controls.Add(this.pnlBitmapImage);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.SteelBlue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TakePicture";
            this.Text = "TakePicture";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TakePicture_FormClosing);
            this.Load += new System.EventHandler(this.TakePicture_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TakePicture_MouseClick);
            this.grpImageAdjustment.ResumeLayout(false);
            this.grpImageAdjustment.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHead;
        private System.Windows.Forms.Button btnStartWebCam;
        private System.Windows.Forms.HScrollBar hScroll_ImageAdjust;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HScrollBar vScroll_ImageAdjust;
        private System.Windows.Forms.Panel pnlHeadOutline;
        private System.Windows.Forms.GroupBox grpImageAdjustment;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.HScrollBar vScroll_HaircutTool;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.HScrollBar hScroll_HeadWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.HScrollBar vScroll_HeadHeight;
        private System.Windows.Forms.Panel pnlNeck;
        private System.Windows.Forms.Panel pnlRightEar;
        private System.Windows.Forms.Panel pnlLeftEar;
        private System.Windows.Forms.TextBox txtScore;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Panel pnlBitmapImage;
        private System.Windows.Forms.CheckBox chkSolidBrush;
        private System.Windows.Forms.CheckBox chkInvertFaceColors;
        private System.Windows.Forms.CheckBox chkCrosshairs;
    }
}