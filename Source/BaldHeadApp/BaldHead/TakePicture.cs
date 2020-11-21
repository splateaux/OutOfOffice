using BaldHead.Properties;
using BaldHead.ScoreKeeper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Touchless.Vision.Camera;

namespace BaldHead
{
    public partial class TakePicture : Form
    {
        MainForm _mainForm;

        // Streaming camera.
        WebCam _webCam;

        // When true, save image to disk then reset the flag.
        bool _saveScreenImage;

        // When not zeros, get image under mouse for textured brush.
        Point _texturedBrushLocation;

        // Flag: When true, camera is streaming.
        bool _streamWebCam;

        // Face color.  Changed by user.
        Color _faceColor;

        // Textured brush for face.
        Image _faceTexture;

        // Graphics pens and brushes.
        Pen _penBlack;
        Pen _penCrosshairs;
        SolidBrush _brushWhite;

        // Last frame stored from WebCam.
        Bitmap _bufferedLastFrame;

        public TakePicture(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            _saveScreenImage = false;
            _texturedBrushLocation = Point.Empty;
            _faceTexture = null;
            _mainForm.ConfigScreen = ConfigScreenEnum.DoNotDraw;
            _webCam = new WebCam(mainForm);
            _webCam.SetViewControl(this);
            _streamWebCam = true;
            _faceColor = Color.Green;
            _penBlack = new Pen(Color.Black, 3f);
            _penCrosshairs = new Pen(Color.SkyBlue, 3f);
            _brushWhite = new SolidBrush(Color.White);

            txtScore.Text = _mainForm.Score.ToString();
            chkSolidBrush.Checked = true;
            UpdateUI();

            txtScore.TextChanged += new System.EventHandler(txtScore_TextChanged);

            // Computed screen coordinates.
            Debug.WriteLine("btnHead location: {0}, {1}",
                ((this.Width - btnHead.Width) / 2).ToString(),
                btnHead.Location.Y.ToString());

            Debug.WriteLine("btnStartWebCam location: {0}, {1}",
                ((this.Width - btnStartWebCam.Width) / 2).ToString(),
                btnStartWebCam.Location.Y.ToString());

            // Set scrollbar ranges.
            Helper_SetScrollbarLimits(vScroll_HaircutTool, _mainForm._settings.Haircut_Offset);

            Helper_SetScrollbarLimits(vScroll_HeadHeight, _mainForm._settings.Face_Height);
            Helper_SetScrollbarLimits(hScroll_HeadWidth, _mainForm._settings.Face_Width);

            Helper_SetScrollbarLimits(vScroll_ImageAdjust, _mainForm._settings.HeadImage_VertOffset);
            Helper_SetScrollbarLimits(hScroll_ImageAdjust, _mainForm._settings.HeadImage_HorzOffset);
        }

        void Helper_SetScrollbarLimits(ScrollBar bar, int offset)
        {
            bar.Minimum = -1 * offset;
            bar.Maximum = offset;
        }

        void UpdateUI()
        {
            // Enable/Disable buttons.
            vScroll_ImageAdjust.Enabled = !_streamWebCam;
            hScroll_ImageAdjust.Enabled = !_streamWebCam;

            UpdateSubmitButtonEnable();

            if (_streamWebCam)
            {
                btnStartWebCam.Text = Consts.StartStopButtonLabel_PressToStop;
            }
            else
            {
                btnStartWebCam.Text = Consts.StartStopButtonLabel_PressToStart;
            }

        }

        void UpdateSubmitButtonEnable()
        {
            btnSubmit.Enabled = !(_webCam == null || _streamWebCam || txtName.Text.Length == 0);
        }

        private void TakePicture_Load(object sender, EventArgs e)
        {
            // Make sure there is a persisted camera.
            if (String.IsNullOrEmpty(_mainForm._settings.SelectedCameraDisplayName))
            {
                MessageBox.Show("Error: No camera name has been saved.  "
                    + "Select one with the command line '/config' option.",
                    "No camera name saved");
                return;
            }

            // Select the persisted camera from the available ones.
            bool foundCamera = false;
            foreach (Camera cam in _webCam.GetCameras())
            {
                if (cam.Name == _mainForm._settings.SelectedCameraDisplayName)
                {
                    _webCam.SetCamera(cam);
                    foundCamera = true;
                    break;
                }
            }

            // Return error if couldn't find one.
            if (!foundCamera)
            {
                MessageBox.Show("Could not find camera: " + _mainForm._settings.SelectedCameraDisplayName,
                    "Camera not found");
                return;
            }

            _webCam.Stop();

            // Enable GUI events now that all is set up.


            _webCam.Start();
            _webCam.FlipImageHorizontally(_mainForm._settings.FlipImageHorizontally);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Color.White);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            // Store last WebCam captured frame in case stream paused.
            if (_webCam.LatestFrame != null && _streamWebCam)
            {
                _bufferedLastFrame = _webCam.LatestFrame;

                // Reset all scroll bars.
                hScroll_ImageAdjust.Value = 0;
                vScroll_ImageAdjust.Value = 0;
            }

            // Manipulate WebCam frame.
            if (_bufferedLastFrame != null)
            {
                Brush defaultBrush = new SolidBrush(_faceColor);

                // Set default brush.
                if (!chkSolidBrush.Checked && _faceTexture != null)
                {
                    // Bitmap tiling image.
                    defaultBrush = new TextureBrush(_faceTexture);
                }

                // Calculate master clip region to prevent images spilling out of face region.
                //  We will use the entire head ellipse.
                Rectangle masterClippingRect = new Rectangle(pnlHeadOutline.Location, pnlHeadOutline.Size);
                GraphicsPath masterClippingPath = new GraphicsPath();
                masterClippingPath.AddEllipse(masterClippingRect);
                Region masterClippingRegion = new Region(masterClippingPath);

                // Master clipping region set.
                e.Graphics.Clip = masterClippingRegion;

                // Get bitmap of center portion of webcam.
                Rectangle cropRegion = new Rectangle(
                    new Point(
                        (int)(((float)_bufferedLastFrame.Width) * (1.0f - _mainForm._settings.CropWebCam_WidthPercent) / 2),
                        (int)((float)_bufferedLastFrame.Height * (1.0f - _mainForm._settings.CropWebCam_HeightPercent) / 2)
                        ),
                    new Size(
                        (int)(((float)_bufferedLastFrame.Width) * _mainForm._settings.CropWebCam_WidthPercent),
                        (int)((float)_bufferedLastFrame.Height * _mainForm._settings.CropWebCam_HeightPercent)
                        )
                    );

                // Adjust webcam image by user scrollbars.
                cropRegion.Offset(hScroll_ImageAdjust.Value * -1, vScroll_ImageAdjust.Value);
                Bitmap bm = WebCam.Crop(_bufferedLastFrame, cropRegion);
                bm = WebCam.Resize(bm, pnlHeadOutline.Size);

                if (chkInvertFaceColors.Checked)
                {
                    Color c;
                    for (int i = 0; i < bm.Width; i++)
                    {
                        for (int j = 0; j < bm.Height; j++)
                        {
                            c = bm.GetPixel(i, j);
                            bm.SetPixel(i, j,
                                Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                        }
                    }
                }

                int offset = (pnlHeadOutline.Width - bm.Width) / 2;
                Rectangle webCamImageRect = new Rectangle(pnlHeadOutline.Location.X + offset, pnlHeadOutline.Location.Y,
                        bm.Width, bm.Height);

                // Draw camera image.
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(bm, webCamImageRect);
                
                // Draw vertical alignment crosshairs.
                if (_streamWebCam && chkCrosshairs.Checked)
                {
                    e.Graphics.DrawLine(_penCrosshairs, this.Width / 2, 0, this.Width / 2, this.Height);
                }
                
                // Add ears.
                e.Graphics.ResetClip();
                Rectangle rightEar = new Rectangle(pnlRightEar.Location, pnlRightEar.Size);
                e.Graphics.FillEllipse(defaultBrush, rightEar);
                e.Graphics.DrawEllipse(_penBlack, rightEar);
                Rectangle leftEar = new Rectangle(pnlLeftEar.Location, pnlLeftEar.Size);
                e.Graphics.FillEllipse(defaultBrush, leftEar);
                e.Graphics.DrawEllipse(_penBlack, leftEar);

                // Draw eye line out of bound crosshair.
                e.Graphics.Clip = masterClippingRegion;
                if (_streamWebCam && chkCrosshairs.Checked)
                {
                    e.Graphics.DrawLine(_penCrosshairs, leftEar.Location, rightEar.Location);
                }

                // Create inner face ellipse and graphis path.                
                Rectangle innerEllipse = new Rectangle(btnHead.Location, btnHead.Size);
                innerEllipse.Inflate(hScroll_HeadWidth.Value, vScroll_HeadHeight.Value);

                GraphicsPath innerPath = new GraphicsPath();
                innerPath.AddEllipse(innerEllipse);

                // Create haircut ellipse and graphis path.
                //  Zero offset coverage is 20% down the face.
                Rectangle haircutRect = masterClippingRect;
                haircutRect.Height = haircutRect.Height / 5 + vScroll_HaircutTool.Value;

                GraphicsPath pathHaircut = new GraphicsPath();
                pathHaircut.AddRectangle(haircutRect);

                // Create drawing region from face and haircut pieces.
                Region head = new Region(masterClippingPath);
                head.Exclude(innerPath);
                head.Union(pathHaircut);
                e.Graphics.FillRegion(defaultBrush, head);

                // Outline head ellipse.       
                e.Graphics.ResetClip();
                e.Graphics.DrawEllipse(_penBlack, masterClippingRect);

                // Draw neck.
                int neckEllipseWidth = 30;
                e.Graphics.FillRectangle(defaultBrush, new Rectangle(pnlNeck.Location, pnlNeck.Size));

                Rectangle leftSideNeck = new Rectangle(pnlNeck.Location, pnlNeck.Size);
                leftSideNeck.Width = neckEllipseWidth;
                leftSideNeck.X = leftSideNeck.X - leftSideNeck.Width / 2;
                e.Graphics.FillEllipse(_brushWhite, leftSideNeck);
                e.Graphics.DrawArc(_penBlack, leftSideNeck, -90f, 180f);
                Point leftShoulder = new Point(leftSideNeck.X + neckEllipseWidth / 2, leftSideNeck.Y + leftSideNeck.Height);

                Rectangle rightSideNeck = new Rectangle(pnlNeck.Location, pnlNeck.Size);
                rightSideNeck.Width = neckEllipseWidth;
                rightSideNeck.X = rightSideNeck.X + pnlNeck.Size.Width - rightSideNeck.Width / 2;
                e.Graphics.FillEllipse(_brushWhite, rightSideNeck);
                e.Graphics.DrawArc(_penBlack, rightSideNeck, 90f, 180f);
                Point rightShoulder = new Point(rightSideNeck.X + neckEllipseWidth / 2, rightSideNeck.Y + rightSideNeck.Height);

                e.Graphics.DrawLine(_penBlack, leftShoulder, rightShoulder);

                // Capture image from screen and write it to file.
                Bitmap memoryImage = new Bitmap(pnlBitmapImage.Width, pnlBitmapImage.Height, e.Graphics);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);

                Point screenCoords = this.PointToScreen(pnlBitmapImage.Location);

                // Image from screen is "memoryImage"
                memoryGraphics.CopyFromScreen(screenCoords.X, screenCoords.Y, 0, 0, pnlBitmapImage.Size);
                
                if (_saveScreenImage)
                {
                    memoryImage.Save("LastestImage.png", ImageFormat.Png);
                    _saveScreenImage = false;

                    SendToServer(memoryImage, _mainForm.Score, txtName.Text);
                }

                // See if need to get texture for brush.
                if (_texturedBrushLocation != Point.Empty)
                {
                    // Capture image from screen and write it to file.
                    int textureWidth = 6;
                    memoryImage = new Bitmap(textureWidth, textureWidth, e.Graphics);
                    memoryGraphics = Graphics.FromImage(memoryImage);

                    screenCoords = this.PointToScreen(_texturedBrushLocation);

                    memoryGraphics.CopyFromScreen(screenCoords.X, screenCoords.Y, 0, 0, pnlBitmapImage.Size);

                    _faceTexture = (Bitmap)memoryImage.Clone();

                    // Reset location to prevent continuous updating.
                    _texturedBrushLocation = Point.Empty;
                }
            }
        }

        /// <summary>
        /// Send all info to scoring server.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="score"></param>
        /// <param name="name"></param>
        private void SendToServer(Image image, int score, string name)
        {
            Score item = new Score();
            item.Name = txtName.Text;
            item.Points = _mainForm.Score;

            if (image != null)
            {
                // Scale image to desired send size.
                Image scaledForServer = new Bitmap(image, _mainForm._settings.Size_HeadImageToScoringServer);

                using (MemoryStream memory = new MemoryStream())
                {
                    scaledForServer.Save(memory, ImageFormat.Jpeg);
                    item.Picture = memory.ToArray();
                }
            }

            try
            {
                if (_mainForm._settings.WriteToServerDuringSubmit)
                {
                    using (ScoreServiceClient client = new ScoreServiceClient())
                    {
                        client.Add(item);

                        // Wait brief time then exit application.
                        Thread.Sleep(500);
                    }
                }

                // Exit app.
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(Utilities.GetAllExceptions(ex), "Exception attempting to send info to server.");
            }
        }

        private void btnStartWebCam_Click(object sender, EventArgs e)
        {
            _streamWebCam = !_streamWebCam;
            UpdateUI();
            UpdateSubmitButtonEnable();
        }


        private void TakePicture_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(e.X.ToString() + ", " + e.Y.ToString());

            // Use this color for solid brush.
            _faceColor = ColorUnderCursor.Get();

            // Set flag for next paint to get a bitmap under the cursor.
            _texturedBrushLocation = new Point(e.X, e.Y);
        }

        /// <summary>
        /// Kick of save of all data to server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_webCam == null)
            {
                return;
            }

            if (_streamWebCam)
            {
                MessageBox.Show("Pause the WebCam before submitting.  We want you to look half-way decent!",
                    "Pause the WebCam first");
                return;
            }

            if (txtName.Text.Length == 0)
            {
                MessageBox.Show("It's your score - own it.  Put your name int the Employee Name box.",
                    "Add your name");
                return;
            }

            _saveScreenImage = true;
        }

        private void txtScore_TextChanged(object sender, EventArgs e)
        {
            txtScore.Text = "No way, sucker!  " + _mainForm.Score.ToString();
        }

        private void TakePicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_webCam.IsCapturing)
            {
                _webCam.Stop();
                _webCam.DisposeCamera();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            btnSubmit.Enabled = txtName.Text.Length > 0;
            UpdateSubmitButtonEnable();
        }
    }

    public class ColorUnderCursor
    {
        [DllImport("gdi32")]
        public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        /// <summary> 
        /// Gets the System.Drawing.Color from under the mouse cursor. 
        /// </summary> 
        /// <returns>The color value.</returns> 
        public static Color Get()
        {
            IntPtr dc = GetWindowDC(IntPtr.Zero);

            POINT p;
            GetCursorPos(out p);

            // GetPixel get R and B reversed.
            long pixel = GetPixel(dc, p.X, p.Y);
            Color color =
                Color.FromArgb(255, (int)(pixel) & 255, (int)(pixel >> 8) & 255, (int)(pixel >> 16) & 255);

            return color;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


}
