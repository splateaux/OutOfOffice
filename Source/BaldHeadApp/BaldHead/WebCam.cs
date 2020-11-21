using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Touchless.Vision.Camera;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BaldHead
{
	/// <summary>
	/// Controller for the WebCam.
	/// </summary>
	public class WebCam
	{
		// Diagnostic mode.
		private const bool MessageboxUponException = true;

		// MainForm for controls.
		MainForm _mainForm;

		// Constants.
		private int Default_CaptureWidth = 640;
		private int Default_CaptureHeight = 480;
		private int Default_Fps = 10;				// -1 means as fast as possible.

		public bool IsCapturing
		{
			get 
			{ 
				if (_frameSource != null)
				{
					return _frameSource.IsCapturing;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public WebCam(MainForm mainForm)
		{
			_mainForm = mainForm;
		}

		public WebCam()
			: this(null)
		{}

		/// <summary>
		/// Start/Stop controller. 
		/// </summary>
		private CameraFrameSource _frameSource;

		/// <summary>
		/// Latest frame from Webcam.
		/// </summary>
		private static Bitmap _latestFrame;

		/// <summary>
		/// PictureBox control to update.  Subscribed to Paint event when rendering.
		/// </summary>
		Control _renderViewControl;

		/// <summary>
		/// Backing store - SelectedCamera property.
		/// </summary>
		private Camera _selectedCamera;

		/// <summary>
		/// Get current camera object.
		/// </summary>
		/// <returns></returns>
		public Camera GetCamera()
		{
			return _selectedCamera;
		}

		/// <summary>
		/// Last frame captured. Is not synchronized to anything.
		/// </summary>
		public Bitmap LatestFrame
		{
			get
			{
				return _latestFrame;
			}

		}
		/// <summary>
		/// Set a new camera object.
		/// </summary>
		/// <param name="camera"></param>
		public void SetCamera(Camera camera)
		{
			// If same camera, return immediately.
			if (_frameSource != null
				&& _frameSource.Camera == _selectedCamera)
			{
				return;
			}

			// New camera.
			this.Stop();
			_selectedCamera = camera;
		}

		/// <summary>
		/// Picture box control used for rendering and subscribing events.
		/// </summary>
		/// <param name="ctrl"></param>
		public void SetViewControl(Control ctrl)
		{
			_renderViewControl = ctrl;
		}

		/// <summary>
		/// Start streaming from SelectedCamera to PictureBox.
		/// </summary>
		/// <returns></returns>
		public bool Start()
		{
			bool success = false;

			try
			{
				if (_selectedCamera == null)
				{
					string emsg = "Error: Must set SelectedCamera before calling Webcam.Start().";
					Debug.WriteLine(emsg);
					throw new ApplicationException(emsg);
				}

				Camera c = _selectedCamera;
				SetFrameSource(new CameraFrameSource(c));
				_frameSource.Camera.CaptureWidth = Default_CaptureWidth;
				_frameSource.Camera.CaptureHeight = Default_CaptureHeight;
				_frameSource.Camera.Fps = Default_Fps;
				_frameSource.NewFrame += OnImageCaptured;

				_renderViewControl.Paint += DrawLatestImage;
				_frameSource.StartFrameCapture();

				success = true;
			}
			catch (Exception ex)
			{
				if (MessageboxUponException)
				{
					MessageBox.Show("Exception attempting to start camera streaming. "
						+ ex.Message, ex.GetType().Name);
				}
			}

			return success;
		}

		/// <summary>
		/// Flip camera image horizontally.
		/// </summary>
		/// <param name="value"></param>
		public void FlipImageHorizontally(bool value)
		{
			_frameSource.Camera.FlipHorizontal = value;
		}

		/// <summary>
		/// Stop streaming from SelectedCamera to PictureBox.
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			bool success = false;

			try
			{
				if (_frameSource != null)
				{
					_frameSource.NewFrame -= OnImageCaptured;
					_frameSource.Camera.Dispose();
					SetFrameSource(null);
					_renderViewControl.Paint -= DrawLatestImage;
				}
			}
			catch (Exception ex)
			{
				if (MessageboxUponException)
				{
					MessageBox.Show("Exception attempting to stop camera streaming. "
						+ ex.Message, ex.GetType().Name);
				}
			}

			return success;
		}


		/// <summary>
		/// All steps necessary to dispose current camera.
		/// </summary>
		public void DisposeCamera()
		{
			Stop();
		}

		/// <summary>
		/// Refresh list of all cameras and pass it back.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Camera> GetCameras()
		{
			return CameraService.AvailableCameras;
		}

		/// <summary>
		/// Handler for new image captured from WebCam device.
		/// </summary>
		/// <param name="frameSource"></param>
		/// <param name="frame"></param>
		/// <param name="fps"></param>
		public void OnImageCaptured(Touchless.Vision.Contracts.IFrameSource frameSource, 
			Touchless.Vision.Contracts.Frame frame, 
			double fps)
		{
			_latestFrame = frame.Image;
			_renderViewControl.Invalidate();
		}

		/// <summary>
		/// Set source for camera streaming.  Pass null to remove all sources.
		/// </summary>
		/// <param name="cameraFrameSource"></param>
		private void SetFrameSource(CameraFrameSource cameraFrameSource)
		{
			if (_frameSource == cameraFrameSource)
				return;

			_frameSource = cameraFrameSource;
		}

		public static Bitmap Crop(Bitmap origBitmap, Rectangle cropArea)
		{
			// Make sure crop area is inside bitmap or will throw Out of memory error.
			cropArea.X = Math.Max(0, cropArea.X);
			cropArea.Y = Math.Max(0, cropArea.Y);
			//cropArea.Width = Min

			if (cropArea.X < 0 
				|| cropArea.Y < 0
				|| (cropArea.X + cropArea.Width > origBitmap.Width)
				|| (cropArea.Y + cropArea.Height) > origBitmap.Height)
			{
				Debug.Fail("Bad crop area coordinates.");
				origBitmap.Clone();
			}

			return origBitmap.Clone(cropArea, origBitmap.PixelFormat); 
		}

		public static Bitmap Resize(Bitmap origBitmap, Size size)
		{
			float percent = Math.Min(
				((float)size.Width) / ((float)origBitmap.PhysicalDimension.Width),
				((float)size.Height) / ((float)origBitmap.PhysicalDimension.Height)
				);

			int destWidth = (int)(origBitmap.PhysicalDimension.Width * percent);
			int destHeight = (int)(origBitmap.PhysicalDimension.Height * percent);

			int offsetWidth = (size.Width - destWidth) / 2;
			int offsetHeight = (size.Height - destHeight) / 2;

			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.DrawImage(origBitmap, offsetWidth, offsetHeight, destWidth, destHeight);
			g.Dispose();

			return b;
		}

		/// <summary>
		/// Return scaler to multiply src rectangle so coordinates fit in viewport.
		/// </summary>
		/// <param name="src">Initial size</param>
		/// <param name="viewport">Size to fit src into.</param>
		/// <returns></returns>
		public static float SourceScale_Fit(Rectangle src, Rectangle viewport)
		{
			return Math.Min(
				((float)viewport.Width) / ((float)src.Width),
				((float)viewport.Height) / ((float)src.Height));
		}

		/// <summary>
		/// Draw the latest image from the active camera. Paint event from control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DrawLatestImage(object sender, PaintEventArgs e)
		{
			if (_latestFrame != null)
			{
				// Camera viewport.
				Rectangle src = new Rectangle(0, 0, _latestFrame.Width, _latestFrame.Height);
				RectangleF viewTranslate = src;

				// Output control size viewport.
				Rectangle dest = _renderViewControl.DisplayRectangle;

				switch (_mainForm.ConfigScreen)
				{
					case ConfigScreenEnum.DoNotDraw:
						return;

					case ConfigScreenEnum.StretchFill:
						e.Graphics.DrawImage(_latestFrame, dest, src, GraphicsUnit.Pixel);
						break;

					case ConfigScreenEnum.UnstretchFill:
						{
							float scale = SourceScale_Fit(src, dest);

							viewTranslate.Width *= scale;
							viewTranslate.Height *= scale;
							viewTranslate.X = (dest.Width - (float)viewTranslate.Width) / 2;
							viewTranslate.Y = (dest.Height - (float)viewTranslate.Height) / 2;
							e.Graphics.Clear(Color.DarkBlue);
							e.Graphics.DrawImage(_latestFrame, viewTranslate, src, GraphicsUnit.Pixel);
						}
						break;

					case ConfigScreenEnum.Middle:
						{
							float scale = SourceScale_Fit(src, dest);

							viewTranslate.Width *= scale / 3f;
							viewTranslate.Height *= scale;

							viewTranslate.X = (dest.Width - viewTranslate.Width) / 2;
							viewTranslate.Y = (dest.Height - viewTranslate.Height) / 2;

							float offset = viewTranslate.Width;
							src.X = (int)(offset / scale);
							src.Width = src.X;


							e.Graphics.Clear(Color.DarkBlue);


							e.Graphics.DrawImage(_latestFrame, viewTranslate, src, GraphicsUnit.Pixel);
						}
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Display configuration property dialog box.
		/// </summary>
		public void ShowPropertiesDialogBox()
		{
			if (_frameSource != null)
			{
				_frameSource.Camera.ShowPropertiesDialog();
			}
		}

		public string GetCameraStatistics()
		{
			string s = String.Format(
				"Capture Height: {0}   (Aspect:{1:N2})\r\nCapture Width: {2}\r\nFps: {3}\r\n",
				_selectedCamera.CaptureHeight,
				((double)_selectedCamera.CaptureWidth) / ((double)_selectedCamera.CaptureHeight),
				_selectedCamera.CaptureWidth,
				_selectedCamera.Fps);

			s += String.Format("-------------------\r\nRender Height: {0}   (Aspect:{1:N2})\r\nRender Width: {2}",
				_renderViewControl.Height,
				((double)_renderViewControl.Width) / ((double)_renderViewControl.Height),
				_renderViewControl.Width);

			return s;
		}

	}
}
